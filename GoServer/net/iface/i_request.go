package iface

import (
	"GoServer/pb"
)

type IRequest interface {
	GetSession() ISession //获取请求连接信息
	GetMessage() *pb.Message
}
