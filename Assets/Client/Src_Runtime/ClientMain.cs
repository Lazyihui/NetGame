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
        [SerializeField] public string username;

        Dictionary<string, RoleEntity> players = new Dictionary<string, RoleEntity>(); // 角色列表
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
                int typeID = MessageHeper.ReadHeader(message.Array); // 读取消息头 这里后面应该是有错的
                if (typeID == MessageConst.login_res) {
                    // 登录响应消息
                    // LoginResMessage msg = MessageHeper.ReadData<LoginResMessage>(message.Array); // 反序列化
                    // Debug.Log("收到登录响应消息: " + msg.ToString()); // 消息内容
                } else if (typeID == MessageConst.login_req) {

                } else if (typeID == MessageConst.roleSpawn_bro) {
                    // 角色出生广播消息
                    RoleSpawnBroMessage msg = MessageHeper.ReadData<RoleSpawnBroMessage>(message.Array); // 反序列化
                    OnSpawn(msg); // 处理消息
                } else if (typeID == MessageConst.roleSpawn_res) {

                }
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
                Debug.Log("发送消息: " + Time.time); // 消息内容
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
                LoginReqMessage msg = new LoginReqMessage(); // 创建消息对象
                msg.username = username;
                msg.password = "123";
                // string str = msg.ToJson(); // 转换为Json字符串
                // byte[] data = System.Text.Encoding.UTF8.GetBytes(str); // 转换为字节数组
                // client.Send(data); // 发送消息
                // Debug.Log("发送消息: " + str); // 消息内容
                // 4.天际MessageHeper类发送消息
                byte[] data = MessageHeper.ToData(msg); // 消息头+消息体
                client.Send(data); // 发送消息
                Debug.Log("发送消息: " + msg.ToJson()); // 消息内容
            }

            if (Input.GetKeyUp(KeyCode.R)) {
                Debug.Log("发送消息: " + Time.time); // 消息内容

                // 发送角色出生消息
                // RoleSpawnReqMessage msg = new RoleSpawnReqMessage(); // 创建消息对象
                // msg.position = new float[2] { 1, 2 }; // 设置位置
                // // string str = msg.ToJson(); // 转换为Json字符串
                // // byte[] data = System.Text.Encoding.UTF8.GetBytes(str); // 转换为字节数组
                // // client.Send(data); // 发送消息
                // // Debug.Log("发送角色出生消息: " + str); // 消息内容
                // byte[] data = MessageHeper.ToData(msg); // 消息头+消息体
                // client.Send(data); // 发送消息
                // Debug.Log("发送角色出生消息: " + msg.ToJson()); // 消息内容
                // 2.
                RoleSpawnReqMessage msg = new RoleSpawnReqMessage(); // 创建消息对象
                msg.username = username; // 设置用户名
                msg.position = new float[2] { 1, 2 }; // 设置位置
                byte[] data = MessageHeper.ToData(msg); // 消息头+消息体
                client.Send(data); // 发送消息
                Debug.Log("发送角色出生消息: " + msg.ToJson()); // 消息内容
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

        // === Game Login ===
        void OnSpawn(RoleSpawnBroMessage meg) {
            // 角色出生
            Debug.Log("角色出生: " + meg.username + " " + meg.position[0] + " " + meg.position[1]);
            // 生成的时候添加到角色列表
            RoleEntity role = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<RoleEntity>(); // 创建角色对象
            role.username = meg.username; // 设置角色名称
            role.transform.position = new Vector3(meg.position[0], 0, meg.position[1]); // 设置角色位置
            players.TryAdd(role.username, role); // 添加到角色列表
        }

        void MoveTo(string username, float[] position) {
            // 角色移动
            Debug.Log("角色移动: " + username + " " + position[0] + " " + position[1]);
        }
    }
}
