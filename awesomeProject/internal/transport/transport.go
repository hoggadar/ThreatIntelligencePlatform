package transport

import (
	protogen "awesomeProject/internal/transport/protogen/ioc"
	"awesomeProject/models"
	log "awesomeProject/pkg/logger"
	"context"
	"github.com/golang/protobuf/ptypes/empty"
	"google.golang.org/protobuf/types/known/timestamppb"
	"time"
)

type Service interface {
	Store(ctx context.Context, stream chan models.IoCDto) error
	Load(ctx context.Context, request models.LoadRequest) (error, chan models.IoCDto)
	UnaryLoad(ctx context.Context, request models.LoadRequest) (error, []models.IoCDto)
	UnaryStore(ctx context.Context, iocs []models.IoCDto) error
}

// Handler - обработчик для базы данных
type Handler struct {
	protogen.UnimplementedDatabaseServer
	service Service
	logger  log.CustomZapLogger
}

func NewHandler(service Service, logger log.CustomZapLogger) *Handler {
	return &Handler{service: service, logger: logger}
}

// Store - метод сохранения данных
func (h *Handler) Store(ctx context.Context, req *protogen.StoreRequest) (*empty.Empty, error) {
	for _, ioc := range req.IoCs {
		h.logger.Printf("Storing IoC: %v", ioc)
		// Проверка контекста на завершение
		select {
		case <-ctx.Done():
			h.logger.Printf("Store: Graceful shutdown detected. Exiting...")
			return nil, ctx.Err()
		default:
			// продолжаем выполнение
		}
	}
	return &empty.Empty{}, nil
}

// StreamStore - метод стримовой записи данных
func (h *Handler) StreamStore(stream protogen.Database_StreamStoreServer) error {
	for {
		req, err := stream.Recv()
		if err != nil {
			h.logger.Printf("Error receiving stream: %v", err)
			return err
		}

		time.Sleep(5 * time.Second)

		h.logger.Printf("Storing IoC from stream: %v", req.Ioc)

		if err := stream.SendMsg(&empty.Empty{}); err != nil {
			h.logger.Printf("Error sending response: %v", err)
			return err
		}

		// Проверка контекста на завершение
		if stream.Context().Err() != nil {
			h.logger.Printf("StreamStore: Graceful shutdown detected. Exiting...")
			return stream.Context().Err()
		}
	}
}

// Load - метод загрузки данных
func (h *Handler) Load(ctx context.Context, req *protogen.LoadRequest) (*protogen.LoadResponse, error) {
	err, response := h.service.UnaryLoad(ctx, models.LoadRequest{
		Limit:  req.Limit,
		Offset: req.Offset,
	})
	if err != nil {
		h.logger.Printf("Load: Error while loading data: %v", err)
		return nil, err
	}

	select {
	case <-ctx.Done():
		h.logger.Printf("Load: Graceful shutdown detected. Exiting...")
		return nil, ctx.Err()
	default:
		// продолжаем выполнение
	}

	// Преобразование моделей из models.IoCDto в protogen.IoCDto
	var protogenIocs []*protogen.IoCDto
	for _, ioc := range response {
		protogenIocs = append(protogenIocs, &protogen.IoCDto{
			Id:             ioc.ID,
			Source:         ioc.Source,
			FirstSeen:      timestamppb.New(*ioc.FirstSeen),
			LastSeen:       timestamppb.New(*ioc.LastSeen),
			Type:           ioc.Type,
			Value:          ioc.Value,
			Tags:           ioc.Tags,
			AdditionalData: ioc.AdditionalData,
		})
	}

	return &protogen.LoadResponse{
		IoCs: protogenIocs,
	}, nil
}

// StreamLoad - метод стримовой загрузки данных
func (h *Handler) StreamLoad(req *protogen.LoadRequest, stream protogen.Database_StreamLoadServer) error {
	iocs := []models.IoCDto{
		{ID: "ioc-1", Source: "source-1", Type: "type-1", Value: "value-1"},
		{ID: "ioc-2", Source: "source-2", Type: "type-2", Value: "value-2"},
		{ID: "ioc-3", Source: "source-3", Type: "type-3", Value: "value-3"},
	}

	for _, ioc := range iocs {
		time.Sleep(5 * time.Second)
		if err := stream.Send(&protogen.StreamLoadResponse{
			Ioc: &protogen.IoCDto{
				Id:     ioc.ID,
				Source: ioc.Source,
				Type:   ioc.Type,
				Value:  ioc.Value,
			},
		}); err != nil {
			h.logger.Printf("Error sending stream: %v", err)
			return err
		}

		// Проверка контекста на завершение
		if stream.Context().Err() != nil {
			h.logger.Printf("StreamLoad: Graceful shutdown detected. Exiting...")
			return stream.Context().Err()
		}
	}

	return nil
}
