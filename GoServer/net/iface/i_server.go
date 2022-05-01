package iface

import "google.golang.org/protobuf/reflect/protoreflect"

type IServer interface {
	Start()
	Stop()
	Serve()
	AddRouter(msgId protoreflect.Message, router IRouter)
	GetSessionMgr() ISessionManager //得到链接管理
	SetOnConnStart(func(ISession))  //设置该Server的连接创建时Hook函数
	SetOnConnStop(func(ISession))   //设置该Server的连接断开时的Hook函数
	CallOnConnStart(conn ISession)  //调用连接OnConnStart Hook函数
	CallOnConnStop(conn ISession)   //调用连接OnConnStop Hook函数
}
