package manager

import (
	"GoServer/model"
)

type UserManager struct {
	Users map[string]model.User
}

func (userMgr *UserManager) Init() {
	userMgr.Users = make(map[string]model.User)
}

func (userMgr UserManager) Add(user model.User) {
	_, ok := userMgr.Users[user.Username]
	if !ok {
		userMgr.Users[user.Username] = user
	}
}

func (userMgr UserManager) Del(user model.User) {
	_, ok := userMgr.Users[user.Username]
	if ok {
		delete(userMgr.Users, user.Username)
	}
}
