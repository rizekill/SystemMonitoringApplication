Test:
=============================
Design, document and implement a system that monitors currently running processes (like 'top' ulity on *nix). 
Add simple web interface that shows gathered information. Design the system to support multiple clients created 
with various technologies. Document APIs that other developers could use to create additional user interfaces to 
the system. Solution should demonstrate your knowledge and experience in various technologies and ability to 
eﬀecvely mix them together and communicate your decisions to other team members. Do not use obvious solutions 
aka SignalR. Minimize a number of external libraries and dependencies you use in this project - imagine that you 
have to do this task on computer with clean Visual Studio installation without internet access. The goal is to 
demonstrate your ability to write code and address new problems. 
Optional: Implement noﬁcaons about high load (CPU, Memory, etc.) to all connected clients.

System Monitoring Application
=============================

**Service Interaction Interfaces:**

- [SystemMonitoringApplication.Protos.MonitorProcess](https://github.com/rizekill/SystemMonitoringApplication/blob/master/SystemMonitoringApplication/Protos/MonitorProcess.proto) - [gRPC protocol](https://docs.microsoft.com/en-us/aspnet/core/grpc/?view=aspnetcore-3.1)
- [SystemMonitoringApplication.Controllers](https://github.com/rizekill/SystemMonitoringApplication/tree/master/SystemMonitoringApplication/Controllers) - [REST APIs](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-implementation)
- [SM.Core.Web](https://github.com/rizekill/SystemMonitoringApplication/tree/master/SM.Core.Web) - [WebSocketMessages](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-3.1)

***The Solution has [application for highload](https://github.com/rizekill/SystemMonitoringApplication/tree/master/HighLoadTest)***

REST APIs
-------------------------

**Get state processes:**

      
```Json
HTTP Methods: GET
Host: "baseUrl/api/ProcessStates"
Exapmle response: 
[
   {
      "ProcessId":1,
      "ProcessName":"ProcessName",
      "Cpu":30,
      "Memory":354
   },
   {
      "ProcessId":2,
      "ProcessName":"ProcessName",
      "Cpu":5,
      "Memory":599
   }
]
```

Web socket messages(WSM)
-------------------------
WSM use websocket transport protocol for data exchange.
```C#
/// <summary>
/// Сообщение для общения между сокетами 
/// </summary>
public class WebSocketMessage
{
    // нужен для десериализации
    public WebSocketMessage(){}

    public WebSocketMessage(object data)
    {
        MessageType = data.GetType().Name;

        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        Data = JsonSerializer.Serialize(data, serializeOptions);
    }

    /// <summary>
    /// Тип сообщения
    /// </summary>
    public string MessageType { get; set; }

    /// <summary>
    /// Сериализованый объект
    /// </summary>
    public string Data { get; set; }
}
```
**It supports multiple commands:**
- **SubscribeCommand** - used to subscribe to messages that implement the interface ```IMessage```
```C#
    /// <summary>
    /// Команда для подписки на тип сообщения
    /// </summary>
    public class SubscribeCommand : IMessage
    {
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public string MessageType { get; set; }
    }
```
- **UnsubscribeCommand** - used to unsubscribe to messages that implement the interface ```IMessage```
```C#
    /// <summary>
    /// Команда для подписки на тип сообщения
    /// </summary>
    public class UnsubscribeCommand : IMessage
    {
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public string MessageType { get; set; }
    }
 ```
 **System Monitoring Application has support for the event list interface:**
 ```C#
 /// <summary>
/// Событие о высокой нагрузке в системе
/// </summary>
public class HighLoadEvent : EventMessage
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="systemInfo"></param>
    public HighLoadEvent(SystemInfo systemInfo) => SystemInfo = systemInfo;

    /// <summary>
    /// Системная информация
    /// </summary>
    public SystemInfo SystemInfo { get; set; }
}

/// <summary>
/// Событие о начале наблюдения за процессом
/// </summary>
public class ProcessObserveOpenEvent : EventMessage
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public ProcessObserveOpenEvent(ProcessState processState) => ProcessState = processState;

    /// <summary>
    /// Состояние процесса
    /// </summary>
    public ProcessState ProcessState { get; set; }
}

/// <summary>
/// Событие об обновлении состояния процесса
/// </summary>
public class ProcessStateUpdateEvent : EventMessage
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public ProcessStateUpdateEvent(ProcessState processState) => ProcessState = processState;

    /// <summary>
    /// Состояние процесса
    /// </summary>
    public ProcessState ProcessState { get; set; }
}

/// <summary>
/// Событие о завершении наблюдения за процессом
/// </summary>
public class ProcessObserveCompleteEvent : EventMessage
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public ProcessObserveCompleteEvent(ProcessState processState) => ProcessState = processState;

    /// <summary>
    /// Состояние процесса
    /// </summary>
    public ProcessState ProcessState { get; set; }
}
 ```
