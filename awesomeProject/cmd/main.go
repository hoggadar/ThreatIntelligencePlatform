package main

import (
	"awesomeProject/broker/rabbitmq"
	"awesomeProject/config"
	"awesomeProject/internal/service"
	"awesomeProject/internal/storage"
	"awesomeProject/internal/transport"
	"awesomeProject/pkg/logger"
	"awesomeProject/server"
	"context"
	"fmt"
	"go.uber.org/zap"
	"net/http"
	"os"
	"os/signal"
)

func main() {
	cfg, err := config.LoadConfig()
	if err != nil {
		panic(fmt.Sprintf("Error loading configuration: %s", err.Error()))
	}

	appLogger := logger.NewCustomZapLogger((*logger.LoggerConfig)(&cfg.LoggerConfig))

	appLogger.Info(cfg.String())

	connStr := cfg.DBConfig.ConnStr()

	storageImpl, err := storage.NewClickHouseStorage(connStr, appLogger)
	if err != nil {
		appLogger.Fatal("Error connecting to database", zap.Error(err))
	}
	err = storageImpl.ApplyHardcodedMigration()
	if err != nil {
		appLogger.Fatal("Error performing migration", zap.Error(err))
	}

	broker, err := rabbitmq.NewRabbitMQConsumer(cfg.BrokerConfig, *appLogger)
	if err != nil {
		appLogger.Fatal("Error creating rabbitmq consumer", zap.Error(err))
	}

	serviceImpl := service.NewService(*appLogger, storageImpl)

	err = broker.RunWorker(context.Background(), serviceImpl.UnaryStore) //TODO ПРОВЕРИТЬ СРАБОТАЕТ ЛИ ГРЕЙСФУЛ
	if err != nil {
		appLogger.Fatal("Error running worker", zap.Error(err))
		return
	}

	handler := transport.NewHandler(serviceImpl, *appLogger)

	srv := server.NewServer(handler, *appLogger)

	go func() {
		if err := srv.Start(":" + cfg.ServerConfig.Port); err != nil && err != http.ErrServerClosed {
			appLogger.Fatal("Failed to start server", zap.Error(err))
		}
	}()

	appLogger.Info("Server is running", zap.String("port", cfg.ServerConfig.Port))

	// Graceful shutdown
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, os.Interrupt)
	<-quit

	if err := srv.Shutdown(context.Background()); err != nil {
		appLogger.Error("Server shutdown failed", zap.Error(err))
	}

	appLogger.Info("Server shutdown successfully")

}
