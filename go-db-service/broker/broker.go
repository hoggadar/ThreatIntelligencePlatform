package broker

import (
	"awesomeProject/models"
	"context"
)

type BrokerConfig struct {
	BrokerAddr string
	Topic      string
	BatchSize  int
}

type Consumer interface {
	Read(topic string, batchSize int) ([]byte, error)
	RunWorker(ctx context.Context, handler func(ctx context.Context, iocs []models.IoCDto))
}
