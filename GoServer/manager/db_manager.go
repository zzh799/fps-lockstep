package manager

import (
	"GoServer/utils"
	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

type DBManager struct {
	DB *gorm.DB
}

func (dbMgr *DBManager) Init() {
	db, err := gorm.Open(mysql.Open(utils.DbConfigInstance.Dsn), &gorm.Config{})
	if err != nil {
		panic(err)
	}
	dbMgr.DB = db
}
