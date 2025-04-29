using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Telepathy;
using System.Threading;

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
            Debug.Log("服务器启动成功: " + port);

            server.OnConnected += (connectionId, str) => {
                Debug.Log("链接成功: " + connectionId + " " + str);
            };

            server.OnData += (connectionId, message) => {
                // 1.
                // string str = System.Text.Encoding.UTF8.GetString(message); // 转换为字符串
                // Debug.Log(" from " + connectionId + "收到的信息 " + str);
                // // server.Send(connectionId, message); // 回发消息
                // 2.
                int a = BitConverter.ToInt32(message); // 转换为整数
                Debug.Log(" from " + connectionId + "收到的信息 " + a); // 消息内容
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
                // 因为是子线程必需关闭
                server.Stop();
            }
        }
    }
}