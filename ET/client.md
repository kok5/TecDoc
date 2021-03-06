
# 1. 网络

## 1.1. NetOuterComponent

## 1.2. Kcp
ET的Kcp来至[ github] (https://github.com/skywind3000/kcp)


# 2. 消息
## 2.1. 消息发送
### 2.1.1. RPC消息发送
```CSharp
G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });
```
### 2.1.2. 直接消息发送
```CSharp
ETModel.SessionComponent.Instance.Session.Send(frameClickMap);
```
### 2.1.3. 心跳
发送,两种方式都可以
```CSharp
    R2C_Ping p = await ETModel.SessionComponent.Instance.Session.Call(new C2R_Ping()) as R2C_Ping;
    Log.Debug("收到R2C_Ping 111");
    R2C_Ping pong = await SessionComponent.Instance.Session.session.Call(new C2R_Ping()) as R2C_Ping;
    Log.Debug("收到R2C_Ping 222");
```
接收：
```CSharp
	[MessageHandler(AppType.AllServer)]
	public class C2R_PingHandler : AMRpcHandler<C2R_Ping, R2C_Ping>
	{
		protected override async ETTask Run(Session session, C2R_Ping request, R2C_Ping response, Action reply)
		{
			reply();
			await ETTask.CompletedTask;
		}
	}
```


## 2.2. 热更消息id>10000

## 2.3. 消息处理类注册
消息处理的函数是根据类特性自动注册的，由组件MessageDispatcherComponent负责
![](_v_images/20190828163005565_25302.png)

MessageDispatcherComponent寻找带MessageHandler标识的类并自动注册：

![](_v_images/20190828165559152_17266.png)

## 2.4. 消息id-->消息类-->消息处理
### 2.4.1. id
	public static partial class OuterOpcode
	{
		 public const ushort C2M_TestRequest = 101;
		 public const ushort M2C_TestResponse = 102;
		 public const ushort Actor_TransferRequest = 103;
		 public const ushort Actor_TransferResponse = 104;
		 public const ushort C2G_EnterMap = 105;
		 public const ushort G2C_EnterMap = 106;
		 public const ushort UnitInfo = 107;
		 public const ushort M2C_CreateUnits = 108;
		 public const ushort Frame_ClickMap = 109;
		 public const ushort M2C_PathfindingResult = 110;
		 public const ushort C2R_Ping = 111;
		 public const ushort R2C_Ping = 112;
		 public const ushort G2C_Test = 113;
		 public const ushort C2M_Reload = 114;
		 public const ushort M2C_Reload = 115;
	}
	`
### 2.4.2. 消息类 
消息类分为两个部分a，b
a放在OuterOpcode.cs里，用来自动识别消息反序列化类型和注册消息处理函数
b放在OuterMessage.cs里，里面办好用工具生成好的protobuf定义（消息）
a:
![](_v_images/20190828152628710_25041.png)
b:
![](_v_images/20190828155641452_28750.png)
### 2.4.3. 消息处理

# 3. 数据结构
## 3.1. DoubleMap
