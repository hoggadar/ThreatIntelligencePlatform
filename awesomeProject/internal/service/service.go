package service

import (
	"awesomeProject/models"
	"awesomeProject/pkg/logger"
	"context"
	"go.uber.org/zap"
)

type Service struct {
	logger  logger.CustomZapLogger
	storage Storage
}

type Storage interface {
	//Store(ctx context.Context, stream chan models.IoCDto) error
	//Load(ctx context.Context, request models.LoadRequest) (error, chan models.IoCDto)
	UnaryStore(ctx context.Context, iocs []models.IoCDto) error
	UnaryLoad(ctx context.Context, request models.LoadRequest) (error, []models.IoCDto)
}

func NewService(logger logger.CustomZapLogger, storage Storage) *Service {
	return &Service{
		logger:  logger,
		storage: storage,
	}
}

func (s Service) Store(ctx context.Context, stream chan models.IoCDto) error {
	//TODO implement me
	panic("implement me")
}

func (s Service) Load(ctx context.Context, request models.LoadRequest) (error, chan models.IoCDto) {
	//TODO implement me
	panic("implement me")
}

func (s Service) UnaryStore(ctx context.Context, iocs []models.IoCDto) error {
	err := s.storage.UnaryStore(ctx, iocs)
	if err != nil {
		s.logger.Error("Error in store", zap.Error(err))
		return err
	}

	return nil
}

func (s Service) UnaryLoad(ctx context.Context, request models.LoadRequest) (error, []models.IoCDto) {
	err, i := s.storage.UnaryLoad(ctx, request)
	if err != nil {
		return err, nil
	}
	return nil, i
}
