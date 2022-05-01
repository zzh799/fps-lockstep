package net

import (
	"GoServer/net/iface"
	"google.golang.org/protobuf/proto"
)

type BaseRouter struct {
}

func (r *BaseRouter) PreHandle(sender iface.ISession, message proto.Message) {

}

func (r *BaseRouter) Handle(sender iface.ISession, message proto.Message) {

}

func (r *BaseRouter) PostHandle(sender iface.ISession, message proto.Message) {

}
