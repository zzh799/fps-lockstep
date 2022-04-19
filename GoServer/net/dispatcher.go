package net

import (
	"GoServer/net/iface"
	"GoServer/pb"
)

func (m *MessageDistributer) DispatchRequest(sender iface.IRequest, request *pb.Request) {
	if request.UserRegister != nil {
		m.DoMsgHandler(sender, request.UserRegister)
	}

	if request.UserLogin != nil {
		m.DoMsgHandler(sender, request.UserLogin)
	}
}

func (m *MessageDistributer) DispatchResponse(sender iface.IRequest, request *pb.Response) {
	if request.UserRegister != nil {
		m.DoMsgHandler(sender, request.UserRegister)
	}

	if request.UserLogin != nil {
		m.DoMsgHandler(sender, request.UserLogin)
	}
}
