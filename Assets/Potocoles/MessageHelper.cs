
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TinyJson;
using Unity.Plastic.Antlr3.Runtime.Tree;

namespace NetGame_Protocoles {

    public static class MessageHeper {

        // 用来识别发的是什么类型的信息
        static Dictionary<Type, int> typeIDMap = new Dictionary<Type, int>() {
            {typeof(LoginReqMessage), MessageConst.login_req}, // 登录请求消息
            {typeof(LoginResMessage), MessageConst.login_res}, // 登录响应消息
            {typeof(RoleSpawnReqMessage), MessageConst.roleSpawn_req}, // 角色出生请求消息
            // {typeof(RoleSpawnResMessage), MessageConst.roleSpawn_res}, // 角色出生响应消息
            {typeof(RoleSpawnBroMessage), MessageConst.roleSpawn_bro}, // 角色出生广播消息
        };

        public static int GetTypeID<T>() {
            // 获取类型ID
            Type type = typeof(T); // 获取类型
            if (typeIDMap.ContainsKey(type)) { // 如果字典中包含该类型
                return typeIDMap[type]; // 返回类型ID
            } else {
                Debug.WriteLine("没有该类型的消息"); // 没有该类型的消息
                return -1; // 返回-1
                // throw new Exception("没有该类型的消息"); // 抛出异常
            }
        }

        public static byte[] ToData<T>(T msg) {
            
            string str = msg.ToJson(); // 将消息转换为json字符串

            int typeID = GetTypeID<T>(); // 获取类型ID

            byte[] msg_header = BitConverter.GetBytes(typeID); // 消息头给定类型ID
            byte[] msg_data = System.Text.Encoding.UTF8.GetBytes(str); // 消息体

            byte[] msg_dst = new byte[msg_header.Length + msg_data.Length]; // 消息体长度

            // b把header写人dst
            Buffer.BlockCopy(msg_header, 0, msg_dst, 0, msg_header.Length); // 拷贝消息头到消息体

            // 把data写入Dst
            Buffer.BlockCopy(msg_data, 0, msg_dst, msg_header.Length, msg_data.Length); // 拷贝消息体到消息体

            return msg_dst; // 返回消息体
        }

        public static int ReadHeader(byte[] data) {
            // 读取消息头
            if (data.Length < 4) {
                Debug.WriteLine("消息头长度不足"); // 消息头长度不足
                return -1; // 消息头长度不足
            } else {
                int typeID = BitConverter.ToInt32(data, 0); // 消息头类型ID
                return typeID; // 返回消息头类型ID
            }
        }

        public static T ReadData<T>(byte[] data) {
            // 读取消息体
            if (data.Length < 4) {
                Debug.WriteLine("消息体长度不足"); // 消息体长度不足
                return default(T); // 返回默认值
            } else {
                string str = System.Text.Encoding.UTF8.GetString(data, 4, data.Length - 4); // 消息体转换为字符串
                T msg = str.FromJson<T>(); // 消息体反序列化为对象
                return msg; // 返回消息体对象
            }
        }
    }
}