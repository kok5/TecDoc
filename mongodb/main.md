
### 1. 安装
sudo apt-get install mongodb

### 2. 放开ip访问
![](_v_images/20190904181837351_20901.png)


### 3. 版本
windows: 4.2

ubuntu: 2.6
    mongo登录
    db.version() 

### 4. 查看是否运行
pgrep mongo -l

### 5. 查看在哪
locate mongo
which mongo


### 6. 连接mogo cloud & 环境变量
mongo "mongodb+srv://laoguaicluster-wplwx.mongodb.net/test" --username laoguai

vi ~/.bashrc

连接云的客户端的路径：/home/mongodb-linux-x86_64-ubuntu1604-4.2.0/bin  
客户端版本：MongoDB shell version v4.2.0

### 7. 手动启动
```CSharp
先进入mongod所在的目录（/usr/bin/mongod），然后运行“./mongod --dbpath /var/lib/mongodb/ --logpath /var/log/mongodb/mongodb.log --logappend &”

--dbpath：指定mongo的数据库文件在哪个文件夹

--logpath：指定mongo的log日志是哪个，这里log一定要指定到具体的文件名

--logappend：表示log的写入是采用附加的方式，默认的是覆盖之前的文件

&：表示程序在后台运行
```

### 8. 常用操作
```pytho
关闭／启动
　　sudo service mongodb stop 　　sudo service mongodb start

 

设置数据库连接密码：
       　　在跟目录创建文件夹： data/db

　　关闭现有服务。

　　　　sudo service mongodb stop

　　重新启动服务

　　　　$ mongod –auth

　　创建连接用户

　　　　$ mongo

　　　　>use admin

　　　　switched to db admin

　　　　>db.addUser("root","1983")

　　关闭服务（直接在 mongod 启动的命令窗口 “ctrl + C”）

        　　重启服务：

         　　　　$:  mongod –auth

　　查看是否开始验证：、

　　　　$ mongo

　　　　MongoDB shell version: 2.0.4

　　　　connecting to: test

　　　　>use admin

　　　　switched to db admin

　　　　>show collections

　　　　Fri Mar 14 09:07:08 uncaught exception: error: {

　　　　"$err" : "unauthorized db:admin lock type:-1 client:127.0.0.1",

　　　　"code" : 10057

　　　　}

　　有提示 链接错误。

         　　进行用户验证：

　　　　>db.auth("root","1983")

　　　　1

　　重新查看就可以查看数据集

　　　　>show collections

　　　　system.indexes

　　　　system.users

 

设置客户端连接：
       　　默认安装的话只允许 127.0.0.1 的IP 连接.

　　需要修改/etc/mongodb.conf 注释下列记录：

       　　打开文件：          

             　　　　$ sudo gedit /etc/mongodb.conf

　　注释记录：            

              　　　　#bind_ip = 0.0.0.0 

mongodb 远程访问配置(ubuntu)

1、首先修改mongodb的配置文件 让其监听所有外网ip,如果不行,连接的时候肯定会有异常
编辑文件：vi /etc/mongodb.conf
修改后的内容如下：
    bind_ip = 0.0.0.0  或者 #bind_ip 127.0.0.1
    port = 27017
    auth=true (添加帐号,密码认证)
2、/etc/init.d/mongodb restart
3、连接 
#本地连接
/usr/local/mongodb/bin/mongo
#远程连接
/usr/local/mongodb/bin/mongo127.0.0.1/admin-u username -p password
4、给某个数据库添加用户访问权限
  db.addUser('user','pwd')
  db.auth('user','pwd')
5、删除用户
  db.removeUser('username')
```
