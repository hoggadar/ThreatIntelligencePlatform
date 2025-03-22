package transport

import (
	protogen "awesomeProject/internal/transport/protgen/ioc"
	"awesomeProject/models"
	log "awesomeProject/pkg/logger"
	"context"
	"errors"
	"fmt"
	"github.com/golang/protobuf/ptypes/empty"
)

const storeBufferSize = 100

// Service
type Service interface {
	Store(ctx context.Context, stream chan models.IoCDto) error
	Load(ctx context.Context, request models.LoadRequest) (chan *models.IoCDto, error)
	UnaryLoad(ctx context.Context, request models.LoadRequest) ([]models.IoCDto, error)
	UnaryStore(ctx context.Context, iocs []models.IoCDto) error
}

type Handler struct {
	protogen.UnimplementedDatabaseServer
	service Service
	logger  log.CustomZapLogger
}

func NewHandler(service Service, logger log.CustomZapLogger) *Handler {
	return &Handler{service: service, logger: logger}
}

func (h *Handler) Store(ctx context.Context, req *protogen.StoreRequest) (*empty.Empty, error) {
	var iocs []models.IoCDto = models.ToModelIoCs(req.IoCs)
	err := h.service.UnaryStore(ctx, iocs)
	if err != nil {
		h.logger.Error(fmt.Sprintf("Failed to store IoCs: %v", err))
		return nil, err
	}

	h.logger.Info(fmt.Sprintf("Successfully stored %d IoCs", len(iocs)))

	return &empty.Empty{}, nil
}

func (h *Handler) StreamStore(stream protogen.Database_StreamStoreServer) error {
	ch := make(chan models.IoCDto, storeBufferSize)

	ctx, cancel := context.WithCancel(stream.Context()) // для того чтобы отозвать горутину если вылезла ошибка
	defer cancel()

	go func() {
		defer close(ch)
		for {
			select {
			case <-ctx.Done():
				h.logger.Warn("StreamStore: Context cancelled, exiting goroutine")
				stream.SendMsg(errors.New("context cancelled.Server is shutting down"))
				return
			default:
				req, err := stream.Recv()
				if err != nil {
					if err == context.Canceled {
						h.logger.Warn("StreamStore: Graceful shutdown detected")
						return
					}
					if err.Error() == "EOF" {
						h.logger.Debug("StreamStore: Client finished sending")
						return
					}
					h.logger.Error(fmt.Sprintf("StreamStore: Error receiving stream: %v", err))
					return
				}
				ioc := models.ToModelIoC(req.Ioc)
				ch <- ioc
			}
		}
	}()

	err := h.service.Store(stream.Context(), ch)
	if err != nil {
		close(ch)
		h.logger.Error(fmt.Sprintf("StreamStore: Failed to store IoCs: %v", err))
		return err
	}

	return nil
}

func (h *Handler) Load(ctx context.Context, req *protogen.LoadRequest) (*protogen.LoadResponse, error) {
	modelReq := models.ToModelLoadRequest(req)
	response, err := h.service.UnaryLoad(ctx, modelReq)
	if err != nil {
		h.logger.Error(fmt.Sprintf("Error in loading: %v", err))
		return nil, err
	}

	var respProto protogen.LoadResponse

	for _, ioc := range response {
		respProto.IoCs = append(respProto.IoCs, models.ToProtoIoC(ioc))
	}
	return &respProto, nil
}

// StreamLoad handles the bidirectional streaming load of data from the client to server
func (h *Handler) StreamLoad(req *protogen.LoadRequest, stream protogen.Database_StreamLoadServer) error {
	reqModel := models.ToModelLoadRequest(req)
	loadChannel, err := h.service.Load(stream.Context(), reqModel)
	if err != nil {
		h.logger.Error(fmt.Sprintf("Error start stream loading: %v", err))
		return err
	}

	var streamMsg protogen.StreamLoadResponse

	for {
		select {
		case <-stream.Context().Done():
			err := stream.Context().Err()
			if err == context.Canceled {
				h.logger.Info("StreamLoad: Context canceled, graceful shutdown")
				return nil // Возвращаем nil, если завершение контролируемое
			}
			h.logger.Error(fmt.Sprintf("StreamLoad: Context done with error: %v", err))
			return err
		case ioc, open := <-loadChannel:
			if !open {
				h.logger.Info("Stream load is done") // тут может не считать остатки ioc если канал буфферезированный
				return nil
			}
			streamMsg.Ioc = models.ToProtoIoC(*ioc)
			err := stream.Send(&streamMsg)
			if err != nil {
				h.logger.Error(fmt.Sprintf("Error sending response: %v", err.Error()))
				return err // не знаю есть ли нефатальные ошибки, после которых стоит продолжать стрим
			} //TODO надо выбрать метод для отправки и наверное выучить что они значат
		}
	}
}
