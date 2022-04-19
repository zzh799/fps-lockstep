package net

import (
	iface2 "GoServer/net/iface"
	"GoServer/pb"
	"GoServer/utils"
	"fmt"
	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/reflect/protoreflect"
)

type MessageDistributer struct {
	Apis           map[protoreflect.Name]iface2.IRouter //存放每个MsgId 所对应的处理方法的map属性
	WorkerPoolSize uint32                               //业务工作Worker池的数量
	TaskQueue      []chan iface2.IRequest               //Worker负责取任务的消息队列

}

func (mh *MessageDistributer) SendMsgToTaskQueue(request iface2.IRequest) {
	//根据ConnID来分配当前的连接应该由哪个worker负责处理
	//轮询的平均分配法则
	//得到需要处理此条连接的workerID
	workerID := request.GetConnection().GetConnID() % mh.WorkerPoolSize
	fmt.Println("Add ConnectionID=", request.GetConnection().GetConnID(), "to workerID=", workerID)
	mh.TaskQueue[workerID] <- request
}

//启动一个Worker工作流程
func (mh *MessageDistributer) StartOneWorker(workerID int, taskQueue chan iface2.IRequest) {
	fmt.Println("Worker ID = ", workerID, " is started.")
	//不断的等待队列中的消息
	for {
		select {
		//有消息则取出队列的Request，并执行绑定的业务方法
		case request := <-taskQueue:
			message := request.GetData().(*pb.Message)
			if message.GetRequest() != nil {
				mh.DispatchRequest(request, message.GetRequest())
			}

			if message.GetResponse() != nil {
				mh.DispatchResponse(request, message.GetResponse())
			}
		}
	}
}

//启动worker工作池
func (mh *MessageDistributer) StartWorkerPool() {
	//遍历需要启动worker的数量，依此启动
	for i := 0; i < int(mh.WorkerPoolSize); i++ {
		//一个worker被启动
		//给当前worker对应的任务队列开辟空间
		mh.TaskQueue[i] = make(chan iface2.IRequest, utils.ConfigInstance.MaxWorkerTaskLen)
		//启动当前Worker，阻塞的等待对应的任务队列是否有消息传递进来
		go mh.StartOneWorker(i, mh.TaskQueue[i])
	}
}

func NewMsgHandle() *MessageDistributer {
	return &MessageDistributer{
		Apis:           make(map[protoreflect.Name]iface2.IRouter),
		WorkerPoolSize: utils.ConfigInstance.WorkerPoolSize,
		//一个worker对应一个queue
		TaskQueue: make([]chan iface2.IRequest, utils.ConfigInstance.WorkerPoolSize),
	}
}

//马上以非阻塞方式处理消息
func (mh *MessageDistributer) DoMsgHandler(request iface2.IRequest, message proto.Message) {
	name := message.ProtoReflect().Descriptor().Name()
	handler, ok := mh.Apis[name]
	if !ok {
		fmt.Println("api msgId = ", name, " is not FOUND!")
		return
	}

	//执行对应处理方法
	handler.PreHandle(request, message)
	handler.Handle(request, message)
	handler.PostHandle(request, message)
}

//为消息添加具体的处理逻辑
func (mh *MessageDistributer) AddRouter(msgId protoreflect.Name, router iface2.IRouter) {
	//1 判断当前msg绑定的API处理方法是否已经存在
	if _, ok := mh.Apis[msgId]; ok {
		panic("repeated api , msgId = " + msgId)
	}
	//2 添加msg与api的绑定关系
	mh.Apis[msgId] = router
	fmt.Println("Add api msgId = ", msgId)
}
