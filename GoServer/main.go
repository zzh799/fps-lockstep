package main

import (
	"fmt"
	"github.com/fwhezfwhez/errorx"
	"github.com/xtaci/kcp-go"
	"io"
	"net"
)

func main() {
	fmt.Println("kcp listens on 10000")
	lis, err := kcp.ListenWithOptions(":10000", nil, 10, 3) //监听1000端口
	if err != nil {
		panic(err)
	}
	for {
		//不断监听接收数据
		conn, e := lis.AcceptKCP()
		//有数据接受
		if e != nil {
			panic(e)
		}
		go OnReceive(conn)
	}
}

var buffer = make([]byte, 1024, 1024)

func OnReceive(conn net.Conn) {
	for {
		n, e := conn.Read(buffer)
		if e != nil {
			if e == io.EOF {
				break
			}
			fmt.Println(errorx.Wrap(e))
			break
		}

		fmt.Println("receive from client:", buffer[:n])
	}
}
