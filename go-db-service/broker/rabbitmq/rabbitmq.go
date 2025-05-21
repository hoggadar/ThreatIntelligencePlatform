package rabbitmq

import (
	"awesomeProject/broker"
	"awesomeProject/models"
	"awesomeProject/pkg/logger"
	"context"
	"encoding/json"
	"fmt"
	"github.com/streadway/amqp"
	"log"
	"sync"
	"time"
)

const retryCount = 5
const pause = 5

// IoCDto - структура данных для обработки

// RabbitMQConsumer - реализация Consumer для RabbitMQ
type RabbitMQConsumer struct {
	logger  logger.CustomZapLogger
	channel *amqp.Channel
	config  broker.BrokerConfig
}

// NewRabbitMQConsumer - конструктор RabbitMQConsumer
func NewRabbitMQConsumer(config broker.BrokerConfig, logger logger.CustomZapLogger) (*RabbitMQConsumer, error) {
	var retries int
retry:
	conn, err := amqp.Dial(config.BrokerAddr)
	if err != nil {
		if retries < retryCount {
			time.Sleep(pause * time.Second)
			retries++
			logger.Error(fmt.Sprintf("Cannot connect to RabbitMQ broker. Retrying in %d/%d time", retries, retryCount))
			goto retry
		}
		return nil, fmt.Errorf("failed to connect to RabbitMQ: %w", err)
	}

	ch, err := conn.Channel()
	if err != nil {
		if retries < 5 {
			time.Sleep(5 * time.Second)
			retries++
			goto retry
		}
		return nil, fmt.Errorf("failed to open a channel: %w", err)
	}

	return &RabbitMQConsumer{
		channel: ch,
		config:  config,
		logger:  logger,
	}, nil
}

// Read - реализация метода Read интерфейса Consumer
func (c *RabbitMQConsumer) Read(batchSize int) ([]byte, error) {
	msgs, err := c.channel.Consume(
		c.config.Topic,
		"",
		true,
		false,
		false,
		false,
		nil,
	)
	if err != nil {
		return nil, fmt.Errorf("failed to register a consumer: %w", err)
	}

	var batch []models.IoCDto
	for i := 0; i < batchSize; i++ {
		msg, ok := <-msgs
		if !ok {
			break
		}
		var ioc models.IoCDto
		if err := json.Unmarshal(msg.Body, &ioc); err != nil {
			c.logger.Error(fmt.Sprintf("Error decoding message: %v", err))
			continue
		}
		batch = append(batch, ioc)
	}

	data, err := json.Marshal(batch)
	if err != nil {
		return nil, fmt.Errorf("failed to marshal messages: %w", err)
	}

	return data, nil
}

func (c *RabbitMQConsumer) RunWorker(ctx context.Context, handler func(ctx context.Context, iocs []models.IoCDto) error) error {
	msgs, err := c.channel.Consume(
		c.config.Topic,
		"",
		true,
		false,
		false,
		false,
		nil,
	)
	if err != nil {
		return fmt.Errorf("failed to register a consumer: %w", err)
	}

	wg := &sync.WaitGroup{}

	go func() {
		defer wg.Wait()
		for {
			select {
			case <-ctx.Done():
				log.Println("Shutting down gracefully...")
				return
			default:
				var batch []models.IoCDto
				for i := 0; i < c.config.BatchSize; i++ {
					select {
					case <-ctx.Done():
						return
					case msg, ok := <-msgs:
						if !ok {
							return
						}
						var ioc models.IoCDto
						if err := json.Unmarshal(msg.Body, &ioc); err != nil {
							log.Printf("Error decoding message: %v", err)
							continue
						}
						batch = append(batch, ioc)
					}
				}
				if len(batch) > 0 {
					c.logger.Info(fmt.Sprintf("Read %d IoCDto", len(batch)))
					wg.Add(1)
					go func(batch []models.IoCDto) {
						defer wg.Done()
						if err := handler(ctx, batch); err != nil {
							log.Printf("Handler error: %v", err)
						}
					}(batch)
				}
			}
		}
	}()

	return nil
}
