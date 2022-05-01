package net

import (
	"GoServer/net/iface"
	"GoServer/pb"
)

type Request struct {
	session iface.ISession //已经和客户端建立好的 链接
	message *pb.Message    //客户端请求的数据
}

func (r *Request) GetSession() iface.ISession {
	return r.session
}

func (r *Request) GetMessage() *pb.Message {
	return r.message
}
