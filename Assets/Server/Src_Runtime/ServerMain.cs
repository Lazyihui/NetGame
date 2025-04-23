using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Telepathy;

// 服务器主类

namespace GameServer {

    // 服务器主类
    public class ServerMain : MonoBehaviour {
        // 服务器主类
        Server server; // 服务器对象
        bool isTearDown; // 是否已经关闭

        void Start() {
            int port = 7777; // 服务器端口 一般要高于1000
            int messageSize = 1024; // 消息大小


            // 1. 创建服务器
            server = new Server(messageSize);
            server.Start(port); // 启动服务器

            server.OnConnected += (connectionId, str) => {
                Debug.Log("链接成功: " + connectionId + " " + str);
            };

            server.OnData += (connectionId, message) => {
                Debug.Log("收到消息: " + message.ToString() + " from " + connectionId);
                server.Send(connectionId, message); // 回发消息
            };

            server.OnDisconnected += (connectionId) => {
                Debug.Log("链接断开: " + connectionId);
            };

            Application.runInBackground = true; // 允许后台运行
        }

        void Update() {
            if (server != null) {
                server.Tick(10); // 处理网络消息10ms
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
                server.Stop();
            }
        }
    }
}