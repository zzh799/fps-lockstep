package manager

import (
	"GoServer/net/iface"
)

type GameManager struct {
	Server iface.IServer
}

var gameManager *GameManager

func GetGMInstance() *GameManager {
	if gameManager == nil {
		gameManager = &GameManager{}
	}
	return gameManager
}
