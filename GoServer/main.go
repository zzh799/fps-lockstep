package main

import (
	"GoServer/manager"
	"GoServer/model"
	"GoServer/pb"
	"fmt"
	"github.com/fwhezfwhez/errorx"
	"github.com/xtaci/kcp-go"
	"google.golang.org/protobuf/proto"
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

		message := &pb.Message{}
		err := proto.Unmarshal(buffer[:n], message)
		if err != nil {
			return
		}
		if message.GetRequest() != nil {
			userLogin := message.GetRequest().GetUserLogin()
			if userLogin != nil {
				userManager := manager.GetInstance()
				_, ok := userManager.Users[userLogin.UserName]
				if ok {
					return
				}

				userManager.Add(model.User{userLogin.UserName, userLogin.Password})

				responseMessage := &pb.Message{
					Response: &pb.Response{
						UserLogin: &pb.UserLoginResponse{
							Result: &pb.Result{
								Success: true,
							},
						},
					},
				}
				marshal, e := proto.Marshal(responseMessage)
				if e != nil {
					conn.Write(marshal)
					fmt.Println("receive from client:", buffer[:n])
				}

			}
		}
		//message.ProtoReflect().WhichOneof()
	}
}
