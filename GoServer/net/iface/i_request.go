package iface

import (
	"google.golang.org/protobuf/proto"
)

type IRequest interface {
	GetConnection() IConnection //获取请求连接信息
	GetData() proto.Message
}
