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

            int port = 7777; // 服务器端口 一般要高于1000
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
