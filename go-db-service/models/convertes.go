package models

import (
	"awesomeProject/internal/transport/protgen/ioc"
	"google.golang.org/protobuf/types/known/timestamppb"
	"time"
)

// ToProtoIoC преобразует IoCDto в protobuf формат
func ToProtoIoC(dto IoCDto) *ioc.IoCDto {
	var protoFirstSeen, protoLastSeen *timestamppb.Timestamp

	if dto.FirstSeen != nil {
		protoFirstSeen = timestamppb.New(*dto.FirstSeen)
	}
	if dto.LastSeen != nil {
		protoLastSeen = timestamppb.New(*dto.LastSeen)
	}

	return &ioc.IoCDto{
		Id:             dto.ID,
		Source:         dto.Source,
		FirstSeen:      protoFirstSeen,
		LastSeen:       protoLastSeen,
		Type:           dto.Type,
		Value:          dto.Value,
		Tags:           dto.Tags,
		AdditionalData: dto.AdditionalData,
	}
}

// ToModelIoC преобразует protobuf формат в IoCDto
func ToModelIoC(proto *ioc.IoCDto) IoCDto {
	var firstSeen, lastSeen *time.Time

	if proto.FirstSeen != nil {
		t := proto.FirstSeen.AsTime()
		firstSeen = &t
	}
	if proto.LastSeen != nil {
		t := proto.LastSeen.AsTime()
		lastSeen = &t
	}

	return IoCDto{
		ID:             proto.Id,
		Source:         proto.Source,
		FirstSeen:      firstSeen,
		LastSeen:       lastSeen,
		Type:           proto.Type,
		Value:          proto.Value,
		Tags:           proto.Tags,
		AdditionalData: proto.AdditionalData,
	}
}

// ToModelIoCs преобразует массив protobuf в массив IoCDto
func ToModelIoCs(protoIoCs []*ioc.IoCDto) []IoCDto {
	result := make([]IoCDto, len(protoIoCs))
	for i, proto := range protoIoCs {
		result[i] = ToModelIoC(proto)
	}
	return result
}

// ToProtoIoCs преобразует массив IoCDto в массив protobuf
func ToProtoIoCs(dtos []IoCDto) []*ioc.IoCDto {
	result := make([]*ioc.IoCDto, len(dtos))
	for i, dto := range dtos {
		result[i] = ToProtoIoC(dto)
	}
	return result
}

// ToModelLoadRequest преобразует protobuf LoadRequest в модель LoadRequest
func ToModelLoadRequest(proto *ioc.LoadRequest) LoadRequest {
	return LoadRequest{
		Limit:  proto.Limit,
		Offset: proto.Offset,
		Filter: proto.Filter,
	}
}

// ToProtoLoadRequest преобразует модель LoadRequest в protobuf LoadRequest
func ToProtoLoadRequest(req LoadRequest) *ioc.LoadRequest {
	return &ioc.LoadRequest{
		Limit:  req.Limit,
		Offset: req.Offset,
		Filter: req.Filter,
	}
}
