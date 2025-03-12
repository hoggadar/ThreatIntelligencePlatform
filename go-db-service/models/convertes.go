package models

import (
	"awesomeProject/internal/transport/protogen/ioc"
	"time"

	"github.com/golang/protobuf/ptypes/timestamp"
)

// ToTime конвертирует protobuf Timestamp в Go time.Time
func ToTime(t *timestamp.Timestamp) *time.Time {
	if t == nil {
		return nil
	}
	tm := time.Unix(t.Seconds, int64(t.Nanos))
	return &tm
}

// ToProtoTimestamp конвертирует Go time.Time в protobuf Timestamp
func ToProtoTimestamp(t *time.Time) *timestamp.Timestamp {
	if t == nil {
		return nil
	}
	return &timestamp.Timestamp{
		Seconds: t.Unix(),
		Nanos:   int32(t.Nanosecond()),
	}
}

// ToModelIoC конвертирует IoCDto из protobuf в models
func ToModelIoC(protoIoC *ioc.IoCDto) IoCDto {
	return IoCDto{
		ID:             protoIoC.Id,
		Source:         protoIoC.Source,
		FirstSeen:      ToTime(protoIoC.FirstSeen),
		LastSeen:       ToTime(protoIoC.LastSeen),
		Type:           protoIoC.Type,
		Value:          protoIoC.Value,
		Tags:           protoIoC.Tags,
		AdditionalData: protoIoC.AdditionalData,
	}
}

// ToProtoIoC конвертирует IoCDto из models в protobuf
func ToProtoIoC(modelIoC IoCDto) *ioc.IoCDto {
	return &ioc.IoCDto{
		Id:             modelIoC.ID,
		Source:         modelIoC.Source,
		FirstSeen:      ToProtoTimestamp(modelIoC.FirstSeen),
		LastSeen:       ToProtoTimestamp(modelIoC.LastSeen),
		Type:           modelIoC.Type,
		Value:          modelIoC.Value,
		Tags:           modelIoC.Tags,
		AdditionalData: modelIoC.AdditionalData,
	}
}

// ToModelIoCs конвертирует массив IoCDto из protobuf в models
func ToModelIoCs(protoIoCs []*ioc.IoCDto) []IoCDto {
	modelIoCs := make([]IoCDto, len(protoIoCs))
	for i, protoIoC := range protoIoCs {
		modelIoCs[i] = ToModelIoC(protoIoC)
	}
	return modelIoCs
}

// ToProtoIoCs конвертирует массив IoCDto из models в protobuf
func ToProtoIoCs(modelIoCs []IoCDto) []*ioc.IoCDto {
	protoIoCs := make([]*ioc.IoCDto, len(modelIoCs))
	for i, modelIoC := range modelIoCs {
		protoIoCs[i] = ToProtoIoC(modelIoC)
	}
	return protoIoCs
}

// Конвертация LoadRequest из protobuf в модель
func ToModelLoadRequest(proto *ioc.LoadRequest) LoadRequest {
	return LoadRequest{
		Limit:  proto.Limit,
		Offset: proto.Offset,
	}
}

// Конвертация модели LoadRequest в protobuf
func ToProtoLoadRequest(model LoadRequest) *ioc.LoadRequest {
	return &ioc.LoadRequest{
		Limit:  model.Limit,
		Offset: model.Offset,
	}
}
