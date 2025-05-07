using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Telepathy;
using System.Threading;
using NetGame_Protocoles; // 引入协议命名空间
using TinyJson;
using System.Text;
// 服务器主类
namespace GameServer {

    // 服务器主类
    public class ServerMain : MonoBehaviour {
        // 服务器主类
        Server server; // 服务器对象
        bool isTearDown; // 是否已经关闭

        // 存下来
        List<int/*connID*/> client = new List<int>(); // 客户端连接ID列表

        void Start() {
            int port = 7777; // 服务器端口 一般要高于1000
            int messageSize = 1024; // 消息大小


            // 1. 创建服务器
            server = new Server(messageSize);
            server.Start(port); // 启动服务器
            Debug.Log("服务器启动成功: " + port);

            server.OnConnected += (connectionId, str) => {
                Debug.Log("链接成功: " + connectionId + " " + str);
                client.Add(connectionId); // 添加连接ID到列表
            };

            server.OnData += (connectionId, message) => {
                // 1.
                // string str = System.Text.Encoding.UTF8.GetString(message); // 转换为字符串
                // Debug.Log(" from " + connectionId + "收到的信息 " + str);
                // // server.Send(connectionId, message); // 回发消息
                // // 2.
                // int a = BitConverter.ToInt32(message); // 转换为整数
                // Debug.Log(" from " + connectionId + "收到的信息 " + a); // 消息内容
                // 3.
                // 反序列化数据
                // string str = Encoding.UTF8.GetString(message); // 转换为字符串
                // LoginMessage msg = str.FromJson<LoginMessage>(); // 反序列化
                // Debug.Log(" from " + connectionId + "收到的信息 " + msg.ToString()); // 消息内容
                // 4.要先处理id在处理数据
                int typeID = MessageHeper.ReadHeader(message.Array); // 读取消息头 这里后面应该是有错的
                if (typeID == MessageConst.login_req) {
                    // LoginMessage
                    LoginReqMessage msg = MessageHeper.ReadData<LoginReqMessage>(message.Array); // 反序列化
                    Debug.Log(" from " + connectionId + "收到的信息 " + msg.ToString()); // 消息内容
                } else if (typeID == MessageConst.roleSpawn_req) {
                    // ChatMessage
                    RoleSpawnReqMessage msg = MessageHeper.ReadData<RoleSpawnReqMessage>(message.Array); // 反序列化
                    OnSpawnRole(connectionId, msg); // 处理消息
                }
            };

            server.OnDisconnected += (connectionId) => {
                Debug.Log("链接断开: " + connectionId);
                client.Remove(connectionId); // 移除连接ID
            };

            Application.runInBackground = true; // 允许后台运行
        }

        void Update() {
            if (server != null) {
                server.Tick(10); // 处理网络消息10ms
            }

            if (Input.GetKeyUp(KeyCode.Space)) {
                // 广播
                for (int i = 0; i < client.Count; i++) {
                    int connID = client[i]; // 获取连接ID

                    // 发送消息
                    // 1.发送原始数据
                    RoleSpawnReqMessage msg = new RoleSpawnReqMessage(); // 创建消息对象
                    msg.position = new float[2] { 1, 2 }; // 设置位置
                    byte[] data = MessageHeper.ToData(msg); // 消息头+消息体
                    server.Send(connID, data); // 发送消息

                }
            }
        }

        void OnDestroy() {
            TearDown();
        }

        void OnApplicationQuit() {
            TearDown();
        }

        void TearDown() {
            if (isTearDown) return;
            isTearDown = true;

            if (server != null) {
                // 因为是子线程必需关闭
                server.Stop();
            }
        }

        // === Game Server ===
        void OnSpawnRole(int connID, RoleSpawnReqMessage req) {
            // 1.当有一位玩家请求出生角色时，给所有玩家广播

            // 2.回传给本人

            // 3.给所有人广播
            for (int i = 0; i < client.Count; i++) {
                int id = client[i];
                // 获取连接ID
                // if (id == connID) {
                //     // 给本人回传
                //     RoleSpawnResMessage res = new RoleSpawnResMessage(); // 创建消息对象
                //     res.position = req.position; // 设置位置
                //     byte[] data = MessageHeper.ToData(res); // 消息头+消息体
                //     server.Send(id, data); // 发送消息
                // } else {
                // 广播给其他人
                RoleSpawnBroMessage bro = new RoleSpawnBroMessage(); // 创建消息对象
                bro.username = req.username; // 设置用户名
                bro.position = req.position; // 设置位置
                
                byte[] data = MessageHeper.ToData(bro); // 消息头+消息体
                server.Send(id, data); // 发送消息
            }
        }
    }
}