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
	// Загружаем конфигурацию
	cfg, err := config.LoadConfig()
	if err != nil {
		panic(fmt.Sprintf("Error loading configuration: %s", err.Error()))
	}

	// Логирование
	appLogger := logger.NewCustomZapLogger((*logger.LoggerConfig)(&cfg.LoggerConfig))
	appLogger.Info(cfg.String())

	// Запуск профайлера
	server.StartProfiler()
	appLogger.Info("Profiler started at :6060/debug/pprof/")

	// Подключение к базе данных
	connStr := cfg.DBConfig.ConnStr()
	storageImpl, err := storage.NewClickHouseStorage(connStr, appLogger)
	if err != nil {
		appLogger.Fatal("Error connecting to database", zap.Error(err))
	}
	err = storageImpl.ApplyHardcodedMigration()
	if err != nil {
		appLogger.Fatal("Error performing migration", zap.Error(err))
	}

	// Инициализация брокера
	broker, err := rabbitmq.NewRabbitMQConsumer(cfg.BrokerConfig, *appLogger)
	if err != nil {
		appLogger.Fatal("Error creating rabbitmq consumer", zap.Error(err))
	}

	// Инициализация сервиса
	serviceImpl := service.NewService(*appLogger, storageImpl)
	err = broker.RunWorker(context.Background(), serviceImpl.UnaryStore)
	if err != nil {
		appLogger.Fatal("Error running worker", zap.Error(err))
		return
	}

	// HTTP сервер
	handler := transport.NewHandler(serviceImpl, *appLogger)
	srv := server.NewServer(handler, *appLogger)

	// Асинхронный запуск HTTP сервера
	go func() {
		if err := srv.Start(":" + cfg.ServerConfig.Port); err != nil && err != http.ErrServerClosed {
			appLogger.Fatal("Failed to start server", zap.Error(err))
		}
	}()
	appLogger.Info("Server is running", zap.String("port", cfg.ServerConfig.Port))

	// Ожидание завершения работы
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, os.Interrupt)
	<-quit

	// Завершаем работу сервера
	if err := srv.Shutdown(context.Background()); err != nil {
		appLogger.Error("Server shutdown failed", zap.Error(err))
	}
	appLogger.Info("Server shutdown successfully")
}
