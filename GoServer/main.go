package main

import (
	"GoServer/manager"
	"GoServer/net"
	"GoServer/utils"
)

func main() {
	utils.ConfigInstance.Reload()
	manager.GetGMInstance().Server = net.NewServer()
	manager.GetGMInstance().Server.Serve()
}
