package net

import (
	"GoServer/net/iface"
	"errors"
	"fmt"
	"sync"
)

/*
	连接管理模块
*/
type SessionManager struct {
	connections map[uint32]iface.IConnection //管理的连接信息
	connLock    sync.RWMutex                 //读写连接的读写锁
}

/*
	创建一个链接管理
*/
func NewConnManager() *SessionManager {
	return &SessionManager{
		connections: make(map[uint32]iface.IConnection),
	}
}

//添加链接
func (sessionMgr *SessionManager) Add(conn iface.IConnection) {
	//保护共享资源Map 加写锁
	sessionMgr.connLock.Lock()
	defer sessionMgr.connLock.Unlock()

	//将conn连接添加到ConnMananger中
	sessionMgr.connections[conn.GetConnID()] = conn

	fmt.Println("connection add to SessionManager successfully: conn num = ", sessionMgr.Len())
}

//删除连接
func (sessionMgr *SessionManager) Remove(conn iface.IConnection) {
	//保护共享资源Map 加写锁
	sessionMgr.connLock.Lock()
	defer sessionMgr.connLock.Unlock()

	//删除连接信息
	delete(sessionMgr.connections, conn.GetConnID())

	fmt.Println("connection Remove ConnectionID=", conn.GetConnID(), " successfully: conn num = ", sessionMgr.Len())
}

//利用ConnID获取链接
func (sessionMgr *SessionManager) Get(connID uint32) (iface.IConnection, error) {
	//保护共享资源Map 加读锁
	sessionMgr.connLock.RLock()
	defer sessionMgr.connLock.RUnlock()

	if conn, ok := sessionMgr.connections[connID]; ok {
		return conn, nil
	} else {
		return nil, errors.New("connection not found")
	}
}

//获取当前连接
func (sessionMgr *SessionManager) Len() int {
	return len(sessionMgr.connections)
}

//清除并停止所有连接
func (sessionMgr *SessionManager) ClearConn() {
	//保护共享资源Map 加写锁
	sessionMgr.connLock.Lock()
	defer sessionMgr.connLock.Unlock()

	//停止并删除全部的连接信息
	for connID, conn := range sessionMgr.connections {
		//停止
		conn.Stop()
		//删除
		delete(sessionMgr.connections, connID)
	}

	fmt.Println("Clear All Connections successfully: conn num = ", sessionMgr.Len())
}
