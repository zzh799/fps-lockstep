package net

import (
	NetInterface "GoServer/net/iface"
	"GoServer/pb"
	"GoServer/utils"
	"errors"
	"fmt"
	"google.golang.org/protobuf/proto"
	"net"
	"sync"
)

type NetConnection struct {
	Server             NetInterface.IServer             //当前conn属于哪个server，在conn初始化的时候添加即可
	Connection         net.Conn                         //当前连接的socket
	ConnectionID       uint32                           //当前连接的ID 也可以称作为SessionID，ID全局唯一
	isClosed           bool                             //当前连接的关闭状态
	MessageDistributer NetInterface.IMessageDistributer //该连接的处理方法msgHandler
	ExitBuffChan       chan bool                        //告知该链接已经退出/停止的channel
	msgChan            chan []byte                      //无缓冲管道，用于读、写两个goroutine之间的消息通信
	msgBuffChan        chan []byte                      //有关冲管道，用于读、写两个goroutine之间的消息通信
	property           map[string]interface{}           //链接属性
	propertyLock       sync.RWMutex                     //保护链接属性修改的锁

}

func (c *NetConnection) SendBuffMsg(message proto.Message) error {
	if c.isClosed == true {
		return errors.New("NetConnection closed when send buff msg")
	}
	bytes, err := proto.Marshal(message)
	if err != nil {
		return err
	}
	c.msgBuffChan <- bytes

	return nil
}

func (c *NetConnection) SendMsg(message proto.Message) error {
	if c.isClosed == true {
		return errors.New("NetConnection closed when send msg")
	}

	bytes, err := proto.Marshal(message)
	if err != nil {
		return err
	}

	//写回客户端
	c.msgChan <- bytes //将之前直接回写给conn.Write的方法 改为 发送给Channel 供Writer读取

	return nil
}

func NewConnection(server NetInterface.IServer, conn net.Conn, connID uint32, msgHandler NetInterface.IMessageDistributer) *NetConnection {
	c := &NetConnection{
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
	server.GetConnMgr().Add(c)
	return c
}

func (c *NetConnection) StartReader() {
	fmt.Println("Reader Goroutine is  running")
	defer fmt.Println(c.RemoteAddr().String(), " conn reader exit!")
	defer c.Stop()
	for {
		headData := make([]byte, 512)
		n, err := c.GetConnection().Read(headData)
		if err != nil {
			fmt.Println("read msg error ", err)
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
				fmt.Println("Unmarshal msg error ", err)
				c.ExitBuffChan <- true
				continue
			}

			req := Request{
				conn: c,
				msg:  message,
			}

			if utils.ConfigInstance.WorkerPoolSize > 0 {
				//已经启动工作池机制，将消息交给Worker处理
				c.MessageDistributer.SendMsgToTaskQueue(&req)
			} else {
				//从绑定好的消息和对应的处理方法中执行对应的Handle方法
				go c.MessageDistributer.DoMsgHandler(&req, message)
			}
		}

	}
}

/*
	写消息Goroutine， 用户将数据发送给客户端
*/
func (c *NetConnection) StartWriter() {

	fmt.Println("[Writer Goroutine is running]")
	defer fmt.Println(c.RemoteAddr().String(), "[conn Writer exit!]")

	for {
		select {
		//针对有缓冲channel需要些的数据处理
		case data, ok := <-c.msgBuffChan:
			if ok {
				//有数据要写给客户端
				if _, err := c.Connection.Write(data); err != nil {
					fmt.Println("Send Buff Data error:, ", err, " Connection Writer exit")
					return
				}
			} else {
				fmt.Println("msgBuffChan is Closed")
				break
			}
		case data := <-c.msgChan:
			//有数据要写给客户端
			if _, err := c.Connection.Write(data); err != nil {
				fmt.Println("Send Data error:, ", err, " Connection Writer exit")
				return
			}
		case <-c.ExitBuffChan:
			//conn已经关闭
			return
		}
	}
}

//启动连接，让当前连接开始工作
func (c *NetConnection) Start() {
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

//停止连接，结束当前连接状态M
func (c *NetConnection) Stop() {
	fmt.Println("Connection Stop()...ConnectionID = ", c.ConnectionID)
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
	c.Server.GetConnMgr().Remove(c) //删除conn从ConnManager中

	//关闭该链接全部管道
	close(c.ExitBuffChan)
	close(c.msgChan)
	close(c.msgBuffChan)
}

//从当前连接获取原始的socket TCPConn
func (c *NetConnection) GetConnection() net.Conn {
	return c.Connection
}

//获取当前连接ID
func (c *NetConnection) GetConnID() uint32 {
	return c.ConnectionID
}

//获取远程客户端地址信息
func (c *NetConnection) RemoteAddr() net.Addr {
	return c.Connection.RemoteAddr()
}

//设置链接属性
func (c *NetConnection) SetProperty(key string, value interface{}) {
	c.propertyLock.Lock()
	defer c.propertyLock.Unlock()

	c.property[key] = value
}

//获取链接属性
func (c *NetConnection) GetProperty(key string) (interface{}, error) {
	c.propertyLock.RLock()
	defer c.propertyLock.RUnlock()

	if value, ok := c.property[key]; ok {
		return value, nil
	} else {
		return nil, errors.New("no property found")
	}
}

//移除链接属性
func (c *NetConnection) RemoveProperty(key string) {
	c.propertyLock.Lock()
	defer c.propertyLock.Unlock()

	delete(c.property, key)
}