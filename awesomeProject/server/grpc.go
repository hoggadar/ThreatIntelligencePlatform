package server

import (
	handlers "awesomeProject/internal/transport"
	protogen "awesomeProject/internal/transport/protogen/ioc"
	"awesomeProject/pkg/logger"
	"context"
	"fmt"
	"google.golang.org/grpc"
	"google.golang.org/grpc/reflection"
	"net"
	"time"
)

// Server - структура для gRPC сервера
type Server struct {
	logger     logger.CustomZapLogger
	grpcServer *grpc.Server
	handler    protogen.DatabaseServer // Хендлер для обработки запросов
}

// NewServer - конструктор для создания нового gRPC сервера
func NewServer(handler *handlers.Handler, logger logger.CustomZapLogger) *Server {
	return &Server{
		grpcServer: grpc.NewServer(),
		handler:    handler,
		logger:     logger,
	}
}

// RegisterServices - регистрация всех сервисов на сервере
func (s *Server) RegisterServices() {
	// Регистрируем наш хендлер для сервиса Database
	protogen.RegisterDatabaseServer(s.grpcServer, s.handler)
}

// Start - запуск gRPC сервера
func (s *Server) Start(port string) error {
	// Прослушиваем указанный адрес
	listener, err := net.Listen("tcp", port)
	if err != nil {
		return fmt.Errorf("failed to listen: %v", err)
	}

	// Регистрируем сервис
	protogen.RegisterDatabaseServer(s.grpcServer, s.handler)

	// Регистрируем рефлексию
	reflection.Register(s.grpcServer)

	// Запускаем сервер в горутине
	go func() {
		if err := s.grpcServer.Serve(listener); err != nil {
			s.logger.Fatal(fmt.Sprintf("failed to serve: %v", err))
		}
	}()

	return nil
}

// Shutdown - завершение работы gRPC сервера
func (s *Server) Shutdown(ctx context.Context) error {
	// Вызываем graceful stop с таймаутом
	stopCtx, cancel := context.WithTimeout(ctx, 10*time.Second)
	defer cancel()

	// Завершаем работу gRPC сервера с учетом контекста
	s.grpcServer.GracefulStop()

	// Пример дополнительной операции с контекстом
	// Например, закрытие базы данных или других сервисов
	select {
	case <-stopCtx.Done(): // Если контекст отменен из-за таймаута
		s.logger.Printf("Shutdown timeout exceeded, force stopping.")
		// В этом случае можно выполнить принудительное завершение работы.
		return stopCtx.Err()
	default:
		// Если все завершилось в пределах таймаута, продолжаем выполнение
		s.logger.Printf("Server gracefully stopped.")
	}

	return nil
}
