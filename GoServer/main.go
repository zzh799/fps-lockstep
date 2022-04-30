package main

import (
	"GoServer/manager"
	"GoServer/net"
	"GoServer/service"
	"GoServer/utils"
)

func main() {
	utils.ConfigInstance.Reload()
	manager.GetGMInstance().Server = net.NewServer()
	service.GetUserService().Init()

	manager.GetGMInstance().Server.Serve()
}
