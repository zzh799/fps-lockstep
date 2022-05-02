package manager

import (
	"GoServer/model"
	"GoServer/utils"
	"fmt"
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

func (dbMgr *DBManager) AutoMigrate() {
	db := dbMgr.DB
	E(db.AutoMigrate(&model.User{}))

}

func E(err error) {
	if err != nil {
		fmt.Println("DBManager->AutoMigrate->Error:" + err.Error())
	}
}
