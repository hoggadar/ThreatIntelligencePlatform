// Code generated by protoc-gen-go-grpc. DO NOT EDIT.
// versions:
// - protoc-gen-go-grpc v1.2.0
// - protoc             v5.29.3
// source: api/proto/database-v2.proto

package ioc

import (
	context "context"
	grpc "google.golang.org/grpc"
	codes "google.golang.org/grpc/codes"
	status "google.golang.org/grpc/status"
	emptypb "google.golang.org/protobuf/types/known/emptypb"
)

// This is a compile-time assertion to ensure that this generated file
// is compatible with the grpc package it is being compiled against.
// Requires gRPC-Go v1.32.0 or later.
const _ = grpc.SupportPackageIsVersion7

// DatabaseClient is the client API for Database service.
//
// For semantics around ctx use and closing/ending streaming RPCs, please refer to https://pkg.go.dev/google.golang.org/grpc/?tab=doc#ClientConn.NewStream.
type DatabaseClient interface {
	// Запись в базу данных
	Store(ctx context.Context, in *StoreRequest, opts ...grpc.CallOption) (*emptypb.Empty, error)
	// Загрузка из базы данных
	Load(ctx context.Context, in *LoadRequest, opts ...grpc.CallOption) (*LoadResponse, error)
	// Стримовая запись данных
	StreamStore(ctx context.Context, opts ...grpc.CallOption) (Database_StreamStoreClient, error)
	// Стримовая загрузка данных
	StreamLoad(ctx context.Context, in *LoadRequest, opts ...grpc.CallOption) (Database_StreamLoadClient, error)
	// Получение общего количества IoC
	Count(ctx context.Context, in *emptypb.Empty, opts ...grpc.CallOption) (*CountResponse, error)
	// Получение количества IoC по всем типам
	CountByType(ctx context.Context, in *emptypb.Empty, opts ...grpc.CallOption) (*CountByTypeResponse, error)
	// Получение количества IoC конкретного типа
	CountSpecificType(ctx context.Context, in *CountSpecificTypeRequest, opts ...grpc.CallOption) (*CountResponse, error)
	// Получение количества IoC по всем источникам
	CountBySource(ctx context.Context, in *CountBySourceRequest, opts ...grpc.CallOption) (*CountBySourceResponse, error)
	// Получение количества IoC конкретного источника
	CountSpecificSource(ctx context.Context, in *CountSpecificSourceRequest, opts ...grpc.CallOption) (*CountResponse, error)
	// Получение количества IoC по типам и источникам
	CountTypesBySource(ctx context.Context, in *emptypb.Empty, opts ...grpc.CallOption) (*CountTypesBySourceResponse, error)
	// Получение количества IoC по источнику и типу
	CountBySourceAndType(ctx context.Context, in *CountBySourceAndTypeRequest, opts ...grpc.CallOption) (*CountByTypeResponse, error)
	// Получение количества IoC по типу и источнику
	CountByTypeAndSource(ctx context.Context, in *CountByTypeAndSourceRequest, opts ...grpc.CallOption) (*CountBySourceResponse, error)
}

type databaseClient struct {
	cc grpc.ClientConnInterface
}

func NewDatabaseClient(cc grpc.ClientConnInterface) DatabaseClient {
	return &databaseClient{cc}
}

func (c *databaseClient) Store(ctx context.Context, in *StoreRequest, opts ...grpc.CallOption) (*emptypb.Empty, error) {
	out := new(emptypb.Empty)
	err := c.cc.Invoke(ctx, "/ioc.Database/Store", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) Load(ctx context.Context, in *LoadRequest, opts ...grpc.CallOption) (*LoadResponse, error) {
	out := new(LoadResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/Load", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) StreamStore(ctx context.Context, opts ...grpc.CallOption) (Database_StreamStoreClient, error) {
	stream, err := c.cc.NewStream(ctx, &Database_ServiceDesc.Streams[0], "/ioc.Database/StreamStore", opts...)
	if err != nil {
		return nil, err
	}
	x := &databaseStreamStoreClient{stream}
	return x, nil
}

type Database_StreamStoreClient interface {
	Send(*StreamStoreRequest) error
	CloseAndRecv() (*emptypb.Empty, error)
	grpc.ClientStream
}

type databaseStreamStoreClient struct {
	grpc.ClientStream
}

func (x *databaseStreamStoreClient) Send(m *StreamStoreRequest) error {
	return x.ClientStream.SendMsg(m)
}

func (x *databaseStreamStoreClient) CloseAndRecv() (*emptypb.Empty, error) {
	if err := x.ClientStream.CloseSend(); err != nil {
		return nil, err
	}
	m := new(emptypb.Empty)
	if err := x.ClientStream.RecvMsg(m); err != nil {
		return nil, err
	}
	return m, nil
}

func (c *databaseClient) StreamLoad(ctx context.Context, in *LoadRequest, opts ...grpc.CallOption) (Database_StreamLoadClient, error) {
	stream, err := c.cc.NewStream(ctx, &Database_ServiceDesc.Streams[1], "/ioc.Database/StreamLoad", opts...)
	if err != nil {
		return nil, err
	}
	x := &databaseStreamLoadClient{stream}
	if err := x.ClientStream.SendMsg(in); err != nil {
		return nil, err
	}
	if err := x.ClientStream.CloseSend(); err != nil {
		return nil, err
	}
	return x, nil
}

type Database_StreamLoadClient interface {
	Recv() (*StreamLoadResponse, error)
	grpc.ClientStream
}

type databaseStreamLoadClient struct {
	grpc.ClientStream
}

func (x *databaseStreamLoadClient) Recv() (*StreamLoadResponse, error) {
	m := new(StreamLoadResponse)
	if err := x.ClientStream.RecvMsg(m); err != nil {
		return nil, err
	}
	return m, nil
}

func (c *databaseClient) Count(ctx context.Context, in *emptypb.Empty, opts ...grpc.CallOption) (*CountResponse, error) {
	out := new(CountResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/Count", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) CountByType(ctx context.Context, in *emptypb.Empty, opts ...grpc.CallOption) (*CountByTypeResponse, error) {
	out := new(CountByTypeResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/CountByType", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) CountSpecificType(ctx context.Context, in *CountSpecificTypeRequest, opts ...grpc.CallOption) (*CountResponse, error) {
	out := new(CountResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/CountSpecificType", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) CountBySource(ctx context.Context, in *CountBySourceRequest, opts ...grpc.CallOption) (*CountBySourceResponse, error) {
	out := new(CountBySourceResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/CountBySource", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) CountSpecificSource(ctx context.Context, in *CountSpecificSourceRequest, opts ...grpc.CallOption) (*CountResponse, error) {
	out := new(CountResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/CountSpecificSource", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) CountTypesBySource(ctx context.Context, in *emptypb.Empty, opts ...grpc.CallOption) (*CountTypesBySourceResponse, error) {
	out := new(CountTypesBySourceResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/CountTypesBySource", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) CountBySourceAndType(ctx context.Context, in *CountBySourceAndTypeRequest, opts ...grpc.CallOption) (*CountByTypeResponse, error) {
	out := new(CountByTypeResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/CountBySourceAndType", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

func (c *databaseClient) CountByTypeAndSource(ctx context.Context, in *CountByTypeAndSourceRequest, opts ...grpc.CallOption) (*CountBySourceResponse, error) {
	out := new(CountBySourceResponse)
	err := c.cc.Invoke(ctx, "/ioc.Database/CountByTypeAndSource", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

// DatabaseServer is the server API for Database service.
// All implementations must embed UnimplementedDatabaseServer
// for forward compatibility
type DatabaseServer interface {
	// Запись в базу данных
	Store(context.Context, *StoreRequest) (*emptypb.Empty, error)
	// Загрузка из базы данных
	Load(context.Context, *LoadRequest) (*LoadResponse, error)
	// Стримовая запись данных
	StreamStore(Database_StreamStoreServer) error
	// Стримовая загрузка данных
	StreamLoad(*LoadRequest, Database_StreamLoadServer) error
	// Получение общего количества IoC
	Count(context.Context, *emptypb.Empty) (*CountResponse, error)
	// Получение количества IoC по всем типам
	CountByType(context.Context, *emptypb.Empty) (*CountByTypeResponse, error)
	// Получение количества IoC конкретного типа
	CountSpecificType(context.Context, *CountSpecificTypeRequest) (*CountResponse, error)
	// Получение количества IoC по всем источникам
	CountBySource(context.Context, *CountBySourceRequest) (*CountBySourceResponse, error)
	// Получение количества IoC конкретного источника
	CountSpecificSource(context.Context, *CountSpecificSourceRequest) (*CountResponse, error)
	// Получение количества IoC по типам и источникам
	CountTypesBySource(context.Context, *emptypb.Empty) (*CountTypesBySourceResponse, error)
	// Получение количества IoC по источнику и типу
	CountBySourceAndType(context.Context, *CountBySourceAndTypeRequest) (*CountByTypeResponse, error)
	// Получение количества IoC по типу и источнику
	CountByTypeAndSource(context.Context, *CountByTypeAndSourceRequest) (*CountBySourceResponse, error)
	mustEmbedUnimplementedDatabaseServer()
}

// UnimplementedDatabaseServer must be embedded to have forward compatible implementations.
type UnimplementedDatabaseServer struct {
}

func (UnimplementedDatabaseServer) Store(context.Context, *StoreRequest) (*emptypb.Empty, error) {
	return nil, status.Errorf(codes.Unimplemented, "method Store not implemented")
}
func (UnimplementedDatabaseServer) Load(context.Context, *LoadRequest) (*LoadResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method Load not implemented")
}
func (UnimplementedDatabaseServer) StreamStore(Database_StreamStoreServer) error {
	return status.Errorf(codes.Unimplemented, "method StreamStore not implemented")
}
func (UnimplementedDatabaseServer) StreamLoad(*LoadRequest, Database_StreamLoadServer) error {
	return status.Errorf(codes.Unimplemented, "method StreamLoad not implemented")
}
func (UnimplementedDatabaseServer) Count(context.Context, *emptypb.Empty) (*CountResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method Count not implemented")
}
func (UnimplementedDatabaseServer) CountByType(context.Context, *emptypb.Empty) (*CountByTypeResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method CountByType not implemented")
}
func (UnimplementedDatabaseServer) CountSpecificType(context.Context, *CountSpecificTypeRequest) (*CountResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method CountSpecificType not implemented")
}
func (UnimplementedDatabaseServer) CountBySource(context.Context, *CountBySourceRequest) (*CountBySourceResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method CountBySource not implemented")
}
func (UnimplementedDatabaseServer) CountSpecificSource(context.Context, *CountSpecificSourceRequest) (*CountResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method CountSpecificSource not implemented")
}
func (UnimplementedDatabaseServer) CountTypesBySource(context.Context, *emptypb.Empty) (*CountTypesBySourceResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method CountTypesBySource not implemented")
}
func (UnimplementedDatabaseServer) CountBySourceAndType(context.Context, *CountBySourceAndTypeRequest) (*CountByTypeResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method CountBySourceAndType not implemented")
}
func (UnimplementedDatabaseServer) CountByTypeAndSource(context.Context, *CountByTypeAndSourceRequest) (*CountBySourceResponse, error) {
	return nil, status.Errorf(codes.Unimplemented, "method CountByTypeAndSource not implemented")
}
func (UnimplementedDatabaseServer) mustEmbedUnimplementedDatabaseServer() {}

// UnsafeDatabaseServer may be embedded to opt out of forward compatibility for this service.
// Use of this interface is not recommended, as added methods to DatabaseServer will
// result in compilation errors.
type UnsafeDatabaseServer interface {
	mustEmbedUnimplementedDatabaseServer()
}

func RegisterDatabaseServer(s grpc.ServiceRegistrar, srv DatabaseServer) {
	s.RegisterService(&Database_ServiceDesc, srv)
}

func _Database_Store_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(StoreRequest)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).Store(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/Store",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).Store(ctx, req.(*StoreRequest))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_Load_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(LoadRequest)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).Load(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/Load",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).Load(ctx, req.(*LoadRequest))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_StreamStore_Handler(srv interface{}, stream grpc.ServerStream) error {
	return srv.(DatabaseServer).StreamStore(&databaseStreamStoreServer{stream})
}

type Database_StreamStoreServer interface {
	SendAndClose(*emptypb.Empty) error
	Recv() (*StreamStoreRequest, error)
	grpc.ServerStream
}

type databaseStreamStoreServer struct {
	grpc.ServerStream
}

func (x *databaseStreamStoreServer) SendAndClose(m *emptypb.Empty) error {
	return x.ServerStream.SendMsg(m)
}

func (x *databaseStreamStoreServer) Recv() (*StreamStoreRequest, error) {
	m := new(StreamStoreRequest)
	if err := x.ServerStream.RecvMsg(m); err != nil {
		return nil, err
	}
	return m, nil
}

func _Database_StreamLoad_Handler(srv interface{}, stream grpc.ServerStream) error {
	m := new(LoadRequest)
	if err := stream.RecvMsg(m); err != nil {
		return err
	}
	return srv.(DatabaseServer).StreamLoad(m, &databaseStreamLoadServer{stream})
}

type Database_StreamLoadServer interface {
	Send(*StreamLoadResponse) error
	grpc.ServerStream
}

type databaseStreamLoadServer struct {
	grpc.ServerStream
}

func (x *databaseStreamLoadServer) Send(m *StreamLoadResponse) error {
	return x.ServerStream.SendMsg(m)
}

func _Database_Count_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(emptypb.Empty)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).Count(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/Count",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).Count(ctx, req.(*emptypb.Empty))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_CountByType_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(emptypb.Empty)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).CountByType(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/CountByType",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).CountByType(ctx, req.(*emptypb.Empty))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_CountSpecificType_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(CountSpecificTypeRequest)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).CountSpecificType(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/CountSpecificType",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).CountSpecificType(ctx, req.(*CountSpecificTypeRequest))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_CountBySource_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(CountBySourceRequest)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).CountBySource(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/CountBySource",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).CountBySource(ctx, req.(*CountBySourceRequest))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_CountSpecificSource_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(CountSpecificSourceRequest)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).CountSpecificSource(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/CountSpecificSource",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).CountSpecificSource(ctx, req.(*CountSpecificSourceRequest))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_CountTypesBySource_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(emptypb.Empty)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).CountTypesBySource(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/CountTypesBySource",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).CountTypesBySource(ctx, req.(*emptypb.Empty))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_CountBySourceAndType_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(CountBySourceAndTypeRequest)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).CountBySourceAndType(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/CountBySourceAndType",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).CountBySourceAndType(ctx, req.(*CountBySourceAndTypeRequest))
	}
	return interceptor(ctx, in, info, handler)
}

func _Database_CountByTypeAndSource_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(CountByTypeAndSourceRequest)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(DatabaseServer).CountByTypeAndSource(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/ioc.Database/CountByTypeAndSource",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(DatabaseServer).CountByTypeAndSource(ctx, req.(*CountByTypeAndSourceRequest))
	}
	return interceptor(ctx, in, info, handler)
}

// Database_ServiceDesc is the grpc.ServiceDesc for Database service.
// It's only intended for direct use with grpc.RegisterService,
// and not to be introspected or modified (even as a copy)
var Database_ServiceDesc = grpc.ServiceDesc{
	ServiceName: "ioc.Database",
	HandlerType: (*DatabaseServer)(nil),
	Methods: []grpc.MethodDesc{
		{
			MethodName: "Store",
			Handler:    _Database_Store_Handler,
		},
		{
			MethodName: "Load",
			Handler:    _Database_Load_Handler,
		},
		{
			MethodName: "Count",
			Handler:    _Database_Count_Handler,
		},
		{
			MethodName: "CountByType",
			Handler:    _Database_CountByType_Handler,
		},
		{
			MethodName: "CountSpecificType",
			Handler:    _Database_CountSpecificType_Handler,
		},
		{
			MethodName: "CountBySource",
			Handler:    _Database_CountBySource_Handler,
		},
		{
			MethodName: "CountSpecificSource",
			Handler:    _Database_CountSpecificSource_Handler,
		},
		{
			MethodName: "CountTypesBySource",
			Handler:    _Database_CountTypesBySource_Handler,
		},
		{
			MethodName: "CountBySourceAndType",
			Handler:    _Database_CountBySourceAndType_Handler,
		},
		{
			MethodName: "CountByTypeAndSource",
			Handler:    _Database_CountByTypeAndSource_Handler,
		},
	},
	Streams: []grpc.StreamDesc{
		{
			StreamName:    "StreamStore",
			Handler:       _Database_StreamStore_Handler,
			ClientStreams: true,
		},
		{
			StreamName:    "StreamLoad",
			Handler:       _Database_StreamLoad_Handler,
			ServerStreams: true,
		},
	},
	Metadata: "api/proto/database-v2.proto",
}
