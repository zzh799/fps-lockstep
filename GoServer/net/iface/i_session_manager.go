package iface

type ISessionManager interface {
	Add(conn ISession)                   //添加链接
	Remove(conn ISession)                //删除连接
	Get(connID uint32) (ISession, error) //利用ConnID获取链接
	Len() int                            //获取当前连接
	Clear()                              //删除并停止所有链接
}
