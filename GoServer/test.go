package main

import (
	"GoServer/pb"
	"fmt"
	"google.golang.org/protobuf/proto"
)

func testOneOf() {
	//var a = &pb.Response{
	//	UserLogin: &pb.UserLoginResponse{
	//		Result: &pb.Result{
	//			Success: true,
	//			Error:   "test",
	//		},
	//	},
	//	UserRegister: &pb.UserRegisterResponse{
	//		Result: &pb.Result{
	//			Success: true,
	//			Error:   "test",
	//		},
	//	},
	//}
	var a = &pb.Response{
		UserLogin: &pb.UserLoginResponse{
			Result: &pb.Result{
				Success: true,
				Error:   "test",
			},
		},
	}

	bytes := buffer
	bytes, _ = proto.Marshal(a)
	fmt.Println("a size:", len(bytes))

	var oneOfResponse_UserLogin = &pb.OneOfResponse{
		Response: &pb.OneOfResponse_UserLogin{
			UserLogin: &pb.UserLoginResponse{
				Result: &pb.Result{
					Success: true,
					Error:   "test",
				},
			},
		},
	}

	//var oneOfResponse_UserRegister = &pb.OneOfResponse{
	//	Response: &pb.OneOfResponse_UserRegister{
	//		UserRegister: &pb.UserRegisterResponse{
	//			Result: &pb.Result{
	//				Success: true,
	//				Error:   "test",
	//			},
	//		},
	//	},
	//}

	var list = []*pb.OneOfResponse{oneOfResponse_UserLogin, oneOfResponse_UserLogin}
	var b = &pb.Responses{
		List: list,
	}
	bytes, _ = proto.Marshal(b)
	fmt.Println("b size:", len(bytes))
}
