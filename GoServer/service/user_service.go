package service

import (
	"GoServer/manager"
	"GoServer/model"
	"GoServer/net"
	"GoServer/net/iface"
	"GoServer/pb"
	"GoServer/utils"
	"fmt"
	"google.golang.org/protobuf/proto"
)

type UserService struct {
}

type UserLoginRequestRouter struct {
	net.BaseRouter
}

type UserRegisterRequestRouter struct {
	net.BaseRouter
}

func (u *UserLoginRequestRouter) Handle(sender iface.ISession, message proto.Message) {
	userLoginRequest := message.(*pb.UserLoginRequest)
	outMessage := sender.GetRspMessage()
	outMessage.Response = &pb.Response{
		UserLogin: &pb.UserLoginResponse{
			Result: &pb.Result{},
		},
	}

	var user model.User
	result := utils.GetInstance[manager.DBManager]().DB.First(&user, "username = ?", userLoginRequest.UserName)
	if result.Error == nil {
		//if user == nil {
		//	fmt.Println("UserService:Can't Find User named,user:" + userLoginRequest.UserName)
		//	outMessage.Response.UserLogin.Result.Success = false
		//	outMessage.Response.UserLogin.Result.Error = "no user"
		//
		//} else
		if user.Password != userLoginRequest.Password {
			fmt.Println("UserService:User Password no match,user:" + userLoginRequest.UserName)
			outMessage.Response.UserLogin.Result.Success = false
			outMessage.Response.UserLogin.Result.Error = "password error"

		} else {
			fmt.Println("UserService:User Login Success,user:" + userLoginRequest.UserName)
			sender.SetProperty("user", user)
			outMessage.Response.UserLogin.Result.Success = true
		}
	} else {
		outMessage.Response.UserLogin.Result.Success = false
		outMessage.Response.UserLogin.Result.Error = result.Error.Error()
	}

	err := sender.SendMsg()
	if err != nil {
		return
	}
}

func (u *UserRegisterRequestRouter) Handle(sender iface.ISession, message proto.Message) {
	userRegisterRequest := message.(*pb.UserRegisterRequest)

	outMessage := sender.GetRspMessage()
	outMessage.Response = &pb.Response{
		UserRegister: &pb.UserRegisterResponse{
			Result: &pb.Result{},
		},
	}

	var user *model.User
	db := utils.GetInstance[manager.DBManager]().DB
	db.First(user, "username = ?", userRegisterRequest.UserName)

	if user != nil {
		fmt.Println("UserService:Can't Find User named,user:" + userRegisterRequest.UserName)
		outMessage.Response.UserRegister.Result.Success = false
		outMessage.Response.UserRegister.Result.Error = "already has user"
	} else {
		user = &model.User{}
		user.Username = userRegisterRequest.UserName
		user.Password = userRegisterRequest.Password
		result := db.Create(user)
		if result.Error != nil {
			outMessage.Response.UserRegister.Result.Success = false
		} else {
			outMessage.Response.UserRegister.Result.Success = true

		}

	}

	err := sender.SendMsg()
	if err != nil {
		return
	}
}

func (s *UserService) Init() {

}

func (s *UserService) RegisterRouter() {
	gameMgr := utils.GetInstance[manager.GameManager]()
	gameMgr.Server.AddRouter((&pb.UserLoginRequest{}).ProtoReflect(), &UserLoginRequestRouter{})
	gameMgr.Server.AddRouter((&pb.UserRegisterRequest{}).ProtoReflect(), &UserRegisterRequestRouter{})
}
