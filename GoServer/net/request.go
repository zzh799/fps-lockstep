package net

import (
	"GoServer/net/iface"
	"google.golang.org/protobuf/proto"
)

type Request struct {
	conn iface.IConnection //已经和客户端建立好的 链接
	msg  proto.Message     //客户端请求的数据
}

func (r *Request) GetConnection() iface.IConnection {
	return r.conn
}

func (r *Request) GetData() proto.Message {
	return r.msg
}
