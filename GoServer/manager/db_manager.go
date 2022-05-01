package manager

import (
	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

type DBManager struct {
	DB *gorm.DB
}

var dsn = "root@tcp(127.0.0.1:3306)/lockstep?charset=utf8mb4&parseTime=True&loc=Local"

func (dbMgr *DBManager) Init() {
	db, err := gorm.Open(mysql.Open(dsn), &gorm.Config{})
	if err != nil {
		panic(err)
	}
	dbMgr.DB = db
}
