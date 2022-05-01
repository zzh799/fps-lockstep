package net

import (
	"GoServer/net/iface"
	"GoServer/utils"
	"fmt"
	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/reflect/protoreflect"
)

type MessageDistributer struct {
	Apis           map[protoreflect.Name]iface.IRouter //存放每个MsgId 所对应的处理方法的map属性
	WorkerPoolSize uint32                              //业务工作Worker池的数量
	TaskQueue      []chan iface.IRequest               //Worker负责取任务的消息队列

}

// SendMsgToTaskQueue 发送请求至任务队列
func (md *MessageDistributer) SendMsgToTaskQueue(request iface.IRequest) {
	//根据ConnID来分配当前的连接应该由哪个worker负责处理
	//轮询的平均分配法则
	//得到需要处理此条连接的workerID
	workerID := request.GetSession().GetConnID() % md.WorkerPoolSize
	fmt.Println("Add ConnectionID=", request.GetSession().GetConnID(), "to workerID=", workerID)
	md.TaskQueue[workerID] <- request
}

// StartWorkerPool 启动worker工作池
func (md *MessageDistributer) StartWorkerPool() {
	//遍历需要启动worker的数量，依此启动
	for i := 0; i < int(md.WorkerPoolSize); i++ {
		//一个worker被启动
		//给当前worker对应的任务队列开辟空间
		md.TaskQueue[i] = make(chan iface.IRequest, utils.ConfigInstance.MaxWorkerTaskLen)
		//启动当前Worker，阻塞的等待对应的任务队列是否有消息传递进来
		go md.StartOneWorker(i, md.TaskQueue[i])
	}
}

// DoMsgHandler 马上以非阻塞方式处理消息
func (md *MessageDistributer) DoMsgHandler(request iface.ISession, message proto.Message) {
	name := message.ProtoReflect().Descriptor().Name()
	handler, ok := md.Apis[name]
	if !ok {
		fmt.Println("api msgId = ", name, " is not FOUND!")
		return
	}

	//执行对应处理方法
	handler.PreHandle(request, message)
	handler.Handle(request, message)
	handler.PostHandle(request, message)
}

// AddRouter 为消息添加具体的处理逻辑
func (md *MessageDistributer) AddRouter(messageInfo protoreflect.Message, router iface.IRouter) {
	name := messageInfo.Descriptor().Name()
	//1 判断当前msg绑定的API处理方法是否已经存在
	if _, ok := md.Apis[name]; ok {
		panic("repeated api , messageInfo = " + name)
	}
	//2 添加msg与api的绑定关系
	md.Apis[name] = router
	fmt.Println("Add api messageInfo = ", name)
}

// StartOneWorker 启动一个Worker工作流程
func (md *MessageDistributer) StartOneWorker(workerID int, taskQueue chan iface.IRequest) {
	fmt.Println("Worker ID = ", workerID, " is started.")
	//不断的等待队列中的消息
	for {
		select {
		//有消息则取出队列的Request，并执行绑定的业务方法
		case request := <-taskQueue:
			message := request.GetMessage()
			if message.GetRequest() != nil {
				md.DispatchRequest(request.GetSession(), message.GetRequest())
			}

			if message.GetResponse() != nil {
				md.DispatchResponse(request.GetSession(), message.GetResponse())
			}
		}
	}
}

func NewMsgHandle() *MessageDistributer {
	return &MessageDistributer{
		Apis:           make(map[protoreflect.Name]iface.IRouter),
		WorkerPoolSize: utils.ConfigInstance.WorkerPoolSize,
		//一个worker对应一个queue
		TaskQueue: make([]chan iface.IRequest, utils.ConfigInstance.WorkerPoolSize),
	}
}
