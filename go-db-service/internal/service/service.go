package service

import (
	"awesomeProject/models"
	"awesomeProject/pkg/logger"
	"context"
	"fmt"

	"go.uber.org/zap"
)

const (
	WorkerPoolSize = 10  // Количество воркеров в пуле
	TaskQueueSize  = 100 // Буферизация очереди задач
	loadBuffSize   = 100
)

// Service основная структура сервисного слоя
type Service struct {
	logger    logger.CustomZapLogger
	storage   Storage
	taskQueue chan func() // Канал задач, где функция представляет работу, выполняемую для клиента
}

type Storage interface {
	// Унарные операции
	UnaryStore(ctx context.Context, iocs []models.IoCDto) error
	UnaryLoad(ctx context.Context, request models.LoadRequest) ([]models.IoCDto, error)

	// Стримовые операции
	StreamStore(ctx context.Context, stream <-chan models.IoCDto) error
	StreamLoad(ctx context.Context, request models.LoadRequest) (chan *models.IoCDto, error)

	// Методы для подсчета IoC
	AllIocsCount(ctx context.Context) (int64, error)
	CountByType(ctx context.Context) (map[string]int64, error)
	CountSpecificType(ctx context.Context, typeName string) (int64, error)
	CountBySource(ctx context.Context) (map[string]int64, error)
	CountSpecificSource(ctx context.Context, sourceName string) (int64, error)
	CountTypesBySource(ctx context.Context) (map[string]map[string]int64, error)
	CountBySourceAndType(ctx context.Context, sourceName string) (map[string]int64, error)
	CountByTypeAndSource(ctx context.Context, typeName string) (map[string]int64, error)
}

// Конструктор для создания сервиса с воркер пулом
func NewService(logger logger.CustomZapLogger, storage Storage) *Service {
	service := &Service{
		logger:    logger,
		storage:   storage,
		taskQueue: make(chan func(), TaskQueueSize), // Задаем очередь задач
	}

	// Инициализируем воркер пул
	for i := 0; i < WorkerPoolSize; i++ {
		go service.worker(i)
	}

	logger.Info("Worker Pool initialized", zap.Int("poolSize", WorkerPoolSize), zap.Int("taskQueueSize", TaskQueueSize))
	return service
}

func (s *Service) worker(workerID int) {
	for {
		select {
		case task, ok := <-s.taskQueue:
			if !ok {
				s.logger.Debug("Worker shutting down", zap.Int("workerID", workerID))
				return
			}
			s.logger.Debug("Worker started processing task", zap.Int("workerID", workerID))
			task()
			s.logger.Debug("Worker finished task", zap.Int("workerID", workerID))
		}
	}
}

func (s *Service) enqueueTask(task func()) error {
	select {
	case s.taskQueue <- task:
		s.logger.Debug("Task enqueued successfully")
		return nil
	default:
		s.logger.Warn("Task queue is full, rejecting task.")
		return fmt.Errorf("task queue is full") // ЕСЛИ НЕТ МЕСТО ДЛЯ ЗАДАЧИ
	}
}

// UnaryStore выполняет унарный запрос на запись данных
func (s *Service) UnaryStore(ctx context.Context, iocs []models.IoCDto) error {
	task := func() {
		s.logger.Info("UnaryStore task started")
		err := s.storage.UnaryStore(ctx, iocs)
		if err != nil {
			s.logger.Error("Error storing IoCs in UnaryStore", zap.Error(err))
			return
		}
		s.logger.Info("Successfully stored IoCs in UnaryStore", zap.Int("count", len(iocs)))
	}
	err := s.enqueueTask(task)
	if err != nil {
		s.logger.Error("Error enqueuing task in UnaryStore", zap.Error(err))
		return err
	}
	return nil
}

// UnaryLoad выполняет унарный запрос на загрузку данных
func (s *Service) UnaryLoad(ctx context.Context, request models.LoadRequest) ([]models.IoCDto, error) {
	outputChan := make(chan []models.IoCDto)
	errChan := make(chan error)

	task := func() {
		defer close(outputChan)
		defer close(errChan)

		s.logger.Info("UnaryLoad task started")
		iocs, err := s.storage.UnaryLoad(ctx, request)
		if err != nil {
			s.logger.Error("Error loading IoCs in UnaryLoad", zap.Error(err))
			errChan <- err

			return
		}
		s.logger.Info("Successfully loaded IoCs in UnaryLoad", zap.Int("count", len(iocs)))
		outputChan <- iocs
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(outputChan)
		close(errChan)
		return nil, err
	}

	select {
	case result := <-outputChan:
		return result, nil
	case err := <-errChan:
		return nil, err
	case <-ctx.Done():
		return nil, ctx.Err()
	}
}

func (s *Service) Store(ctx context.Context, stream chan models.IoCDto) error {
	task := func() {
		s.logger.Info("Processing StreamStore task")
		err := s.storage.StreamStore(ctx, stream) // Передаём поток канала напрямую в хранилище
		if err != nil {
			s.logger.Error("Failed to process StreamStore task", zap.Error(err))
			return
		}
		s.logger.Info("StreamStore task completed successfully")
	}

	// Добавляем задачу в очереди worker pool
	err := s.enqueueTask(task)
	if err != nil {
		s.logger.Error("Failed to enqueue StreamStore task", zap.Error(err))
		return err
	}
	return nil
}

func (s *Service) Load(ctx context.Context, request models.LoadRequest) (chan *models.IoCDto, error) {
	output := make(chan *models.IoCDto, loadBuffSize) // Создаём буферизированный канал

	task := func() {
		defer close(output)

		s.logger.Info("Processing StreamLoad task with pagination", zap.Int64("limit", request.Limit), zap.Int64("offset", request.Offset))

		// Вызываем StreamLoad из слоя базы данных
		storageStream, err := s.storage.StreamLoad(ctx, request)
		if err != nil {
			s.logger.Error("Failed to start StreamLoad from storage", zap.Error(err))

			return
		}

		// Переключаем поток данных из слоя хранилища

		for {
			select {
			case <-ctx.Done():
				s.logger.Warn("StreamLoad cancelled due to context timeout or cancellation")
				return
			case ioc, open := <-storageStream:
				if !open {
					s.logger.Info("StreamLoad completed successfully")
					return
				}
				output <- ioc
			}
		}
	}

	// Добавляем задачу в пул воркеров
	err := s.enqueueTask(task)
	if err != nil {
		close(output)
		s.logger.Error("Failed to enqueue StreamLoad task", zap.Error(err))
		return nil, err
	}
	return output, nil
}

// Count возвращает общее количество IoC
func (s *Service) Count(ctx context.Context) (int64, error) {
	resultChan := make(chan int64, 1)
	errChan := make(chan error, 1)

	task := func() {
		defer close(resultChan)
		defer close(errChan)

		s.logger.Info("Count task started")
		count, err := s.storage.AllIocsCount(ctx)
		if err != nil {
			s.logger.Error("Error counting IoCs", zap.Error(err))
			errChan <- err
			return
		}
		s.logger.Info("Successfully counted IoCs", zap.Int64("count", count))
		resultChan <- count
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(resultChan)
		close(errChan)
		return 0, err
	}

	select {
	case result := <-resultChan:
		return result, nil
	case err := <-errChan:
		return 0, err
	case <-ctx.Done():
		return 0, ctx.Err()
	}
}

// CountByType возвращает количество IoC по типам
func (s *Service) CountByType(ctx context.Context) (map[string]int64, error) {
	resultChan := make(chan map[string]int64, 1)
	errChan := make(chan error, 1)

	task := func() {
		defer close(resultChan)
		defer close(errChan)

		s.logger.Info("CountByType task started")
		typeCounts, err := s.storage.CountByType(ctx)
		if err != nil {
			s.logger.Error("Error counting IoCs by type", zap.Error(err))
			errChan <- err
			return
		}
		s.logger.Info("Successfully counted IoCs by type", zap.Any("typeCounts", typeCounts))
		resultChan <- typeCounts
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(resultChan)
		close(errChan)
		return nil, err
	}

	select {
	case result := <-resultChan:
		return result, nil
	case err := <-errChan:
		return nil, err
	case <-ctx.Done():
		return nil, ctx.Err()
	}
}

// CountSpecificType возвращает количество IoC конкретного типа
func (s *Service) CountSpecificType(ctx context.Context, typeName string) (int64, error) {
	resultChan := make(chan int64, 1)
	errChan := make(chan error, 1)

	task := func() {
		defer close(resultChan)
		defer close(errChan)

		s.logger.Info("CountSpecificType task started", zap.String("type", typeName))
		count, err := s.storage.CountSpecificType(ctx, typeName)
		if err != nil {
			s.logger.Error("Error counting IoCs of specific type", zap.String("type", typeName), zap.Error(err))
			errChan <- err
			return
		}
		s.logger.Info("Successfully counted IoCs of specific type", zap.String("type", typeName), zap.Int64("count", count))
		resultChan <- count
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(resultChan)
		close(errChan)
		return 0, err
	}

	select {
	case result := <-resultChan:
		return result, nil
	case err := <-errChan:
		return 0, err
	case <-ctx.Done():
		return 0, ctx.Err()
	}
}

// CountBySource возвращает количество IoC по источникам
func (s *Service) CountBySource(ctx context.Context) (map[string]int64, error) {
	resultChan := make(chan map[string]int64, 1)
	errChan := make(chan error, 1)

	task := func() {
		defer close(resultChan)
		defer close(errChan)

		s.logger.Info("CountBySource task started")
		sourceCounts, err := s.storage.CountBySource(ctx)
		if err != nil {
			s.logger.Error("Error counting IoCs by source", zap.Error(err))
			errChan <- err
			return
		}
		s.logger.Info("Successfully counted IoCs by source", zap.Any("sourceCounts", sourceCounts))
		resultChan <- sourceCounts
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(resultChan)
		close(errChan)
		return nil, err
	}

	select {
	case result := <-resultChan:
		return result, nil
	case err := <-errChan:
		return nil, err
	case <-ctx.Done():
		return nil, ctx.Err()
	}
}

// CountSpecificSource возвращает количество IoC конкретного источника
func (s *Service) CountSpecificSource(ctx context.Context, sourceName string) (int64, error) {
	resultChan := make(chan int64, 1)
	errChan := make(chan error, 1)

	task := func() {
		defer close(resultChan)
		defer close(errChan)

		s.logger.Info("CountSpecificSource task started", zap.String("source", sourceName))
		count, err := s.storage.CountSpecificSource(ctx, sourceName)
		if err != nil {
			s.logger.Error("Error counting IoCs of specific source", zap.String("source", sourceName), zap.Error(err))
			errChan <- err
			return
		}
		s.logger.Info("Successfully counted IoCs of specific source", zap.String("source", sourceName), zap.Int64("count", count))
		resultChan <- count
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(resultChan)
		close(errChan)
		return 0, err
	}

	select {
	case result := <-resultChan:
		return result, nil
	case err := <-errChan:
		return 0, err
	case <-ctx.Done():
		return 0, ctx.Err()
	}
}

func (s *Service) CountTypesBySource(ctx context.Context) (map[string]map[string]int64, error) {
	resultChan := make(chan map[string]map[string]int64, 1)
	errChan := make(chan error, 1)

	task := func() {
		defer close(resultChan)
		defer close(errChan)

		s.logger.Info("CountTypesBySource task started")
		counts, err := s.storage.CountTypesBySource(ctx)
		if err != nil {
			s.logger.Error("Error counting IoCs by types and sources", zap.Error(err))
			errChan <- err
			return
		}
		resultChan <- counts
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(resultChan)
		close(errChan)
		return nil, err
	}

	select {
	case result := <-resultChan:
		return result, nil
	case err := <-errChan:
		return nil, err
	case <-ctx.Done():
		return nil, ctx.Err()
	}
}

func (s *Service) CountBySourceAndType(ctx context.Context, sourceName string) (map[string]int64, error) {
	resultChan := make(chan map[string]int64, 1)
	errChan := make(chan error, 1)

	task := func() {
		defer close(resultChan)
		defer close(errChan)

		s.logger.Info("CountBySourceAndType task started", zap.String("source", sourceName))
		counts, err := s.storage.CountBySourceAndType(ctx, sourceName)
		if err != nil {
			s.logger.Error("Error counting IoCs by source and type", zap.Error(err))
			errChan <- err
			return
		}
		resultChan <- counts
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(resultChan)
		close(errChan)
		return nil, err
	}

	select {
	case result := <-resultChan:
		return result, nil
	case err := <-errChan:
		return nil, err
	case <-ctx.Done():
		return nil, ctx.Err()
	}
}

func (s *Service) CountByTypeAndSource(ctx context.Context, typeName string) (map[string]int64, error) {
	resultChan := make(chan map[string]int64, 1)
	errChan := make(chan error, 1)

	task := func() {
		defer close(resultChan)
		defer close(errChan)

		s.logger.Info("CountByTypeAndSource task started", zap.String("type", typeName))
		counts, err := s.storage.CountByTypeAndSource(ctx, typeName)
		if err != nil {
			s.logger.Error("Error counting IoCs by type and source", zap.Error(err))
			errChan <- err
			return
		}
		resultChan <- counts
	}

	err := s.enqueueTask(task)
	if err != nil {
		close(resultChan)
		close(errChan)
		return nil, err
	}

	select {
	case result := <-resultChan:
		return result, nil
	case err := <-errChan:
		return nil, err
	case <-ctx.Done():
		return nil, ctx.Err()
	}
}
