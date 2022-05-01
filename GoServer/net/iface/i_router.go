package iface

import (
	"google.golang.org/protobuf/proto"
)

type IRouter interface {
	PreHandle(sender ISession, message proto.Message)  //在处理conn业务之前的钩子方法
	Handle(sender ISession, message proto.Message)     //处理conn业务的方法
	PostHandle(sender ISession, message proto.Message) //处理conn业务之后的钩子方法
}
