package iface

import (
	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/reflect/protoreflect"
)

type IMessageDistributer interface {
	DoMsgHandler(request ISession, message proto.Message) //马上以非阻塞方式处理消息
	AddRouter(msgId protoreflect.Message, router IRouter) //为消息添加具体的处理逻辑
	StartWorkerPool()                                     //启动worker工作池
	SendMsgToTaskQueue(request IRequest)                  //将消息交给TaskQueue,由worker进行处理
}
