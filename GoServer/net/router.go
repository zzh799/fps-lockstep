package net

import (
	"GoServer/net/iface"
	"google.golang.org/protobuf/proto"
)

type BaseRouter struct {
}

func (r *BaseRouter) PreHandle(req iface.IRequest, message proto.Message) {

}

func (r *BaseRouter) Handle(req iface.IRequest, message proto.Message) {

}

func (r *BaseRouter) PostHandle(req iface.IRequest, message proto.Message) {

}
