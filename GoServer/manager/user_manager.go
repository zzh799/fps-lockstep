package manager

import (
	"GoServer/model"
)

type UserManager struct {
	Users map[string]model.User
}

var instance *UserManager

func GetInstance() *UserManager {
	if instance == nil {
		instance = New()
	}
	return instance
}

func New() *UserManager {
	r := &UserManager{Users: make(map[string]model.User)}
	return r
}

func (m UserManager) Add(user model.User) {
	_, ok := m.Users[user.UserName]
	if !ok {
		m.Users[user.UserName] = user
	}
}

func (m UserManager) Del(user model.User) {
	_, ok := m.Users[user.UserName]
	if !ok {
		delete(m.Users, user.UserName)
	}
}
