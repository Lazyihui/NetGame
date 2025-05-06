using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using System.Threading;
using System;
using NetGame_Protocoles; // 引入协议命名空间
using TinyJson;


namespace GameClient {

    // 客户端主类
    public class ClientMain : MonoBehaviour {
        Client client;

        bool isTearDown;

        void Start() {

            int port = 7777; // 服务器端口 一般要高于10000 1-65535
            int messageSize = 1024; // 消息大小
            string ip = "127.0.0.1"; // 服务器IP地址

            // 1. 创建客户端
            client = new Client(messageSize);
            client.Connect(ip, port); // 连接服务器 

            client.OnConnected += () => {
                Debug.Log("链接成功");
            };

            client.OnData += (message) => {
                Debug.Log("收到消息: " + message.ToString());
            };

            client.OnDisconnected += () => {
                Debug.Log("链接断开");
            };

            Application.runInBackground = true; // 允许后台运行

        }

        void Update() {

            if (client != null) {
                client.Tick(10); // 处理网络消息10ms
            }
            if (Input.GetKeyUp(KeyCode.Space)) {
                // 发送消息
                // 1.发送原始数据
                // Debug.Log("发送消息");
                // string message = "Hello World! " + Time.time; // 消息内容
                // byte[] data = System.Text.Encoding.UTF8.GetBytes(message); // 转换为字节数组
                // client.Send(data); // 发送消息
                // 2.发送二进制序列化数据
                // int a = Time.frameCount; // 帧数
                // byte[] date = BitConverter.GetBytes(a);
                // client.Send(date);
                // Debug.Log("发送消息: " + a); // 消息内容
                // 3.发送复杂数据 
                LoginMessage msg = new LoginMessage(); // 创建消息对象
                msg.username = "cyh";
                msg.password = "123";
                string str = msg.ToJson(); // 转换为Json字符串
                byte[] data = System.Text.Encoding.UTF8.GetBytes(str); // 转换为字节数组
                client.Send(data); // 发送消息
                Debug.Log("发送消息: " + str); // 消息内容
            }

            if (Input.GetKeyUp(KeyCode.A)) {
                // 发送角色出生消息
                RoleSpawnMessage msg = new RoleSpawnMessage(); // 创建消息对象
                msg.position = new float[2] { 1, 2 }; // 设置位置
                string str = msg.ToJson(); // 转换为Json字符串
                byte[] data = System.Text.Encoding.UTF8.GetBytes(str); // 转换为字节数组
                client.Send(data); // 发送消息
                Debug.Log("发送角色出生消息: " + str); // 消息内容
            }
        }

        void OnApplicationQuit() {
            TearDown();
        }

        void TearDown() {
            if (isTearDown) return;
            isTearDown = true;

            if (client != null) {
                client.Disconnect();
            }
        }
    }
}
