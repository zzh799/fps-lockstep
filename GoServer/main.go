package main

import (
	"GoServer/manager"
	"GoServer/net"
	"GoServer/service"
	"GoServer/utils"
)

func main() {
	utils.ConfigInstance.Reload()
	gameMgr := utils.GetInstance[*manager.GameManager]()
	gameMgr.Server = net.NewServer()
	utils.GetInstance[*service.UserService]().Init()
	gameMgr.Server.Serve()
}
