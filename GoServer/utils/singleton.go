package utils

import (
	"reflect"
	"sync"
)

type Singleton interface {
	Init()
}

var cache sync.Map

// GetInstance returns a singleton of T.
func GetInstance[T any]() (t *T) {
	hash := reflect.TypeOf(t)
	v, ok := cache.Load(hash)

	if ok {
		return v.(*T)
	}
	v = new(T)
	v, _ = cache.LoadOrStore(hash, v)
	_, b := hash.MethodByName("Init")
	if b {
		instance := v.(Singleton)
		instance.Init()
	}

	return v.(*T)
}
