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
func GetInstance[T Singleton]() (t T) {
	hash := reflect.TypeOf(t)
	v, ok := cache.Load(hash)

	if ok {
		return v.(T)
	}
	v, _ = cache.LoadOrStore(hash, v)
	instance := v.(T)
	instance.Init()
	return instance
}
