package main

import (
	"fmt"
	"github.com/fwhezfwhez/errorx"
	"github.com/xtaci/kcp-go"
	"io"
	"net"
)

func main() {
	fmt.Println("kcp listens on 13145")
	lis, err := kcp.Listen("0.0.0.0:13145") //监听1000端口
	if err != nil {
		panic(err)
	}
	for {
		//不断监听接收数据
		conn, e := lis.Accept()
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
		conn.Write(buffer[:n])
		fmt.Println("receive from client:", buffer[:n])

		//request := &pb.Request{}
		//proto.Unmarshal(buffer[:n], request)
		//request.ProtoReflect().WhichOneof()
	}
}
