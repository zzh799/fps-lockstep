package net

import (
	"GoServer/net/iface"
	"GoServer/pb"
)

func (md *MessageDistributer) DispatchRequest(sender iface.ISession, request *pb.Request) {
	if request.UserRegister != nil {
		md.DoMsgHandler(sender, request.UserRegister)
	}

	if request.UserLogin != nil {
		md.DoMsgHandler(sender, request.UserLogin)
	}
}

func (md *MessageDistributer) DispatchResponse(sender iface.ISession, request *pb.Response) {
	if request.UserRegister != nil {
		md.DoMsgHandler(sender, request.UserRegister)
	}

	if request.UserLogin != nil {
		md.DoMsgHandler(sender, request.UserLogin)
	}
}
