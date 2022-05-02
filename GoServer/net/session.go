package net

import (
	"GoServer/net/iface"
	"GoServer/pb"
	"GoServer/utils"
	"errors"
	"fmt"
	"google.golang.org/protobuf/proto"
	"net"
	"sync"
)

type Session struct {
	Server             iface.IServer             //当前conn属于哪个server，在conn初始化的时候添加即可
	Connection         net.Conn                  //当前连接的socket
	ConnectionID       uint32                    //当前连接的ID 也可以称作为SessionID，ID全局唯一
	isClosed           bool                      //当前连接的关闭状态
	MessageDistributer iface.IMessageDistributer //该连接的处理方法msgHandler
	ExitBuffChan       chan bool                 //告知该链接已经退出/停止的channel
	msgChan            chan []byte               //无缓冲管道，用于读、写两个goroutine之间的消息通信
	msgBuffChan        chan []byte               //有关冲管道，用于读、写两个goroutine之间的消息通信
	property           map[string]interface{}    //链接属性
	propertyLock       sync.RWMutex              //保护链接属性修改的锁
	rspMsg             *pb.Message               //返回消息
}

func (c *Session) GetRspMessage() *pb.Message {
	if c.rspMsg == nil {
		c.rspMsg = &pb.Message{}
	}
	return c.rspMsg
}

func (c *Session) SendBuffMsg() error {
	if c.isClosed == true {
		return errors.New("NetConnection closed when send buff message")
	}
	bytes, err := proto.Marshal(c.rspMsg)
	if err != nil {
		return err
	}
	c.msgBuffChan <- bytes

	return nil
}

func (c *Session) SendMsg() error {
	if c.isClosed == true {
		return errors.New("NetConnection closed when send message")
	}

	bytes, err := proto.Marshal(c.rspMsg)
	if err != nil {
		return err
	}

	//写回客户端
	c.msgChan <- bytes //将之前直接回写给conn.Write的方法 改为 发送给Channel 供Writer读取

	return nil
}

func NewConnection(server iface.IServer, conn net.Conn, connID uint32, msgHandler iface.IMessageDistributer) *Session {
	c := &Session{
		Server:             server,
		Connection:         conn,
		ConnectionID:       connID,
		isClosed:           false,
		MessageDistributer: msgHandler,
		ExitBuffChan:       make(chan bool, 1),
		msgChan:            make(chan []byte),
		msgBuffChan:        make(chan []byte, utils.ConfigInstance.MaxMsgChanLen),
		property:           make(map[string]interface{}), //对链接属性map初始化
	}
	//将新创建的Conn添加到链接管理中
	server.GetSessionMgr().Add(c)
	return c
}

func (c *Session) StartReader() {
	fmt.Println("Reader Goroutine is  running")
	defer fmt.Println(c.RemoteAddr().String(), " session reader exit!")
	defer c.Stop()
	for {
		headData := make([]byte, 512)
		n, err := c.GetConnection().Read(headData)
		if err != nil {
			fmt.Println("read message error ", err)
			c.ExitBuffChan <- true
			continue
		}

		if n > 0 {
			data := headData[:n]
			if data[0] == 101 {
				_, err := c.GetConnection().Write(data)
				if err != nil {
					continue
				}

				continue
			}

			message := &pb.Message{}
			err := proto.Unmarshal(data, message)
			if err != nil {
				fmt.Println("Unmarshal message error ", err)
				c.ExitBuffChan <- true
				continue
			}

			req := Request{
				session: c,
				message: message,
			}

			if utils.ConfigInstance.WorkerPoolSize > 0 {
				//已经启动工作池机制，将消息交给Worker处理
				c.MessageDistributer.SendMsgToTaskQueue(&req)
			} else {
				//从绑定好的消息和对应的处理方法中执行对应的Handle方法
				go c.MessageDistributer.DoMsgHandler(c, message)
			}
		}

	}
}

// StartWriter 写消息Goroutine， 用户将数据发送给客户端
func (c *Session) StartWriter() {

	fmt.Println("[Writer Goroutine is running]")
	defer fmt.Println(c.RemoteAddr().String(), "[session Writer exit!]")

	for {
		select {
		//针对有缓冲channel需要些的数据处理
		case data, ok := <-c.msgBuffChan:
			if ok {
				//有数据要写给客户端
				if _, err := c.Connection.Write(data); err != nil {
					fmt.Println("Send Buff Data error:, ", err, " Session Writer exit")
					return
				}
			} else {
				fmt.Println("msgBuffChan is Closed")
				break
			}
		case data := <-c.msgChan:
			//有数据要写给客户端
			if _, err := c.Connection.Write(data); err != nil {
				fmt.Println("Send Data error:, ", err, " Session Writer exit")
				return
			}
		case <-c.ExitBuffChan:
			//conn已经关闭
			return
		}
	}
}

// Start 启动连接，让当前连接开始工作
func (c *Session) Start() {
	//1 开启用户从客户端读取数据流程的Goroutine
	go c.StartReader()
	//2 开启用于写回客户端数据流程的Goroutine
	go c.StartWriter()

	c.Server.CallOnConnStart(c)

	for {
		select {
		case <-c.ExitBuffChan:
			//得到退出消息，不再阻塞
			return
		}
	}
}

// Stop 停止连接，结束当前连接状态M
func (c *Session) Stop() {
	fmt.Println("Session Stop()...ConnectionID = ", c.ConnectionID)
	//如果当前链接已经关闭
	if c.isClosed == true {
		return
	}
	c.isClosed = true
	c.Server.CallOnConnStop(c)

	// 关闭socket链接
	err := c.Connection.Close()
	if err != nil {
		return
	}
	//关闭Writer Goroutine
	c.ExitBuffChan <- true

	//将链接从连接管理器中删除
	c.Server.GetSessionMgr().Remove(c) //删除conn从ConnManager中

	//关闭该链接全部管道
	close(c.ExitBuffChan)
	close(c.msgChan)
	close(c.msgBuffChan)
}

// GetConnection 从当前连接获取原始的socket TCPConn
func (c *Session) GetConnection() net.Conn {
	return c.Connection
}

// GetConnID 获取当前连接ID
func (c *Session) GetConnID() uint32 {
	return c.ConnectionID
}

// RemoteAddr 获取远程客户端地址信息
func (c *Session) RemoteAddr() net.Addr {
	return c.Connection.RemoteAddr()
}

// SetProperty 设置链接属性
func (c *Session) SetProperty(key string, value interface{}) {
	c.propertyLock.Lock()
	defer c.propertyLock.Unlock()

	c.property[key] = value
}

// GetProperty 获取链接属性
func (c *Session) GetProperty(key string) (interface{}, error) {
	c.propertyLock.RLock()
	defer c.propertyLock.RUnlock()

	if value, ok := c.property[key]; ok {
		return value, nil
	} else {
		return nil, errors.New("no property found")
	}
}

// RemoveProperty 移除链接属性
func (c *Session) RemoveProperty(key string) {
	c.propertyLock.Lock()
	defer c.propertyLock.Unlock()

	delete(c.property, key)
}
