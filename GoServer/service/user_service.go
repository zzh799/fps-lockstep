package service

import (
	"GoServer/manager"
	"GoServer/net"
	"GoServer/net/iface"
	"GoServer/pb"
	"google.golang.org/protobuf/proto"
)

type UserLoginRequestRouter struct {
	net.BaseRouter
}

func (u *UserLoginRequestRouter) Handle(request iface.IRequest, message proto.Message) {
	userLoginRequest := message.(*pb.UserLoginRequest)
	userLoginRequest.ProtoMessage()
}

func init() {
	manager.GetGMInstance().Server.AddRouter("UserLoginRequest", &UserLoginRequestRouter{})
}
