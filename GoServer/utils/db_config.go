package utils

import (
	"encoding/json"
	"io/ioutil"
)

type DBConfig struct {
	Dsn string
}

var DbConfigInstance *DBConfig

func (c *DBConfig) Reload() {
	bytes, err := ioutil.ReadFile("db_config.json")
	if err != nil {
		panic(err)
	}
	err = json.Unmarshal(bytes, &DbConfigInstance)
	if err != nil {
		panic(err)
	}
}

func init() {
	DbConfigInstance = &DBConfig{
		Dsn: "root@tcp(127.0.0.1:3306)/lockstep?charset=utf8mb4&parseTime=True&loc=Local",
	}

	DbConfigInstance.Reload()
}
