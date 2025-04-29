using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;


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
            if (Input.GetKeyDown(KeyCode.Space)) {
                // 发送消息
                Debug.Log("发送消息");
                string message = "Hello World! " + Time.time; // 消息内容
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message); // 转换为字节数组
                client.Send(data); // 发送消息
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
