package service

import (
	"GoServer/manager"
	"GoServer/model"
	"GoServer/net"
	"GoServer/net/iface"
	"GoServer/pb"
	"fmt"
	"google.golang.org/protobuf/proto"
)

type UserLoginRequestRouter struct {
	net.BaseRouter
}

type UserRegisterRequestRouter struct {
	net.BaseRouter
}

func (u *UserLoginRequestRouter) Handle(request iface.IRequest, message proto.Message) {
	userLoginRequest := message.(*pb.UserLoginRequest)

	outMessage := &pb.Message{
		Response: &pb.Response{
			UserLogin: &pb.UserLoginResponse{
				Result: &pb.Result{},
			},
		},
	}

	var user *model.User
	manager.GetDBMgrInstance().DB.First(user, "username = ?", userLoginRequest.UserName)
	if user == nil {
		fmt.Println("UserService:Can't Find User named,user:" + userLoginRequest.UserName)
		outMessage.Response.UserLogin.Result.Success = false
		outMessage.Response.UserLogin.Result.Error = "no user"

	} else if user.Password != userLoginRequest.Password {
		fmt.Println("UserService:User Password no match,user:" + userLoginRequest.UserName)
		outMessage.Response.UserLogin.Result.Success = false
		outMessage.Response.UserLogin.Result.Error = "password error"

	} else {
		fmt.Println("UserService:User Login Success,user:" + userLoginRequest.UserName)
		request.GetConnection().SetProperty("user", user)
		outMessage.Response.UserLogin.Result.Success = true
	}

	err := request.GetConnection().SendMsg(outMessage)
	if err != nil {
		return
	}
}

func (u *UserRegisterRequestRouter) Handle(request iface.IRequest, message proto.Message) {
	userRegisterRequest := message.(*pb.UserRegisterRequest)

	outMessage := &pb.Message{
		Response: &pb.Response{
			UserRegister: &pb.UserRegisterResponse{
				Result: &pb.Result{},
			},
		},
	}

	var user *model.User
	manager.GetDBMgrInstance().DB.First(user, "username = ?", userRegisterRequest.UserName)

	if user != nil {
		fmt.Println("UserService:Can't Find User named,user:" + userRegisterRequest.UserName)
		outMessage.Response.UserRegister.Result.Success = false
		outMessage.Response.UserRegister.Result.Error = "already has user"
	} else {
		user = &model.User{}
		user.UserName = userRegisterRequest.UserName
		user.Password = userRegisterRequest.Password
		manager.GetDBMgrInstance().DB.Create(user)
		outMessage.Response.UserRegister.Result.Success = true
	}

	err := request.GetConnection().SendMsg(outMessage)
	if err != nil {
		return
	}
}

func (s *UserService) Init() {

	manager.GetGMInstance().Server.AddRouter("UserLoginRequest", &UserLoginRequestRouter{})
	manager.GetGMInstance().Server.AddRouter("UserRegisterRequest", &UserRegisterRequestRouter{})
}

type UserService struct {
}

var userService *UserService

func GetUserService() *UserService {
	if userService == nil {
		userService = &UserService{}
	}
	return userService
}
