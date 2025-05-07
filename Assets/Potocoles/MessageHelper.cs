
using System;
using System.Collections.Generic;
using TinyJson;
using UnityEngine;
namespace NetGame_Protocoles {

    public static class MessageHelper {

        // 用来识别发的是什么类型的信息
        static Dictionary<Type, int> typeIDMap = new Dictionary<Type, int>() {
            {typeof(LoginReqMessage), MessageConst.login_req}, // 登录请求消息
            {typeof(LoginResMessage), MessageConst.login_res}, // 登录响应消息
            {typeof(RoleSpawnReqMessage), MessageConst.roleSpawn_req}, // 角色出生请求消息
            // {typeof(RoleSpawnResMessage), MessageConst.roleSpawn_res}, // 角色出生响应消息
            {typeof(RoleSpawnBroMessage), MessageConst.roleSpawn_bro}, // 角色出生广播消息
            {typeof(MoveReqMessage), MessageConst.move_req}, // 角色移动请求消息
            // {typeof(MoveResMessage), MessageConst.move_res}, // 角色移动响应消息
            {typeof(MoveBroMessage), MessageConst.move_bro}, // 角色移动广播消息
        };

        public static int GetTypeID<T>() {
            // 获取类型ID
            Type type = typeof(T); // 获取类型
            if (typeIDMap.ContainsKey(type)) { // 如果字典中包含该类型
                return typeIDMap[type]; // 返回类型ID
            } else {
                Debug.Log("没有该类型的消息"); // 没有该类型的消息
                return -1; // 返回-1
                // throw new Exception("没有该类型的消息"); // 抛出异常
            }
        }

        public static byte[] ToData<T>(T msg) {

            string str = JsonUtility.ToJson(msg);

            int typeID = GetTypeID<T>();

            byte[] msg_header = BitConverter.GetBytes(typeID);
            byte[] msg_data = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] msg_length = BitConverter.GetBytes(msg_data.Length);

            byte[] msg_dst = new byte[msg_header.Length + 4 + msg_data.Length];

            // header 写入 dst
            Buffer.BlockCopy(msg_header, 0, msg_dst, 0, msg_header.Length);

            // length 写入 dst
            Buffer.BlockCopy(msg_length, 0, msg_dst, msg_header.Length, msg_length.Length);

            // data 写入 dst
            Buffer.BlockCopy(msg_data, 0, msg_dst, msg_header.Length + msg_length.Length, msg_data.Length);

            Debug.Log("[MessageHelper]ToData: " + typeID + " " + str);

            return msg_dst;
        }

        public static int ReadHeader(byte[] data) {
            // 读取消息头
            if (data.Length < 4) {
                Debug.Log("消息头长度不足"); // 消息头长度不足
                return -1; // 消息头长度不足
            } else {
                int typeID = BitConverter.ToInt32(data, 0); // 消息头类型ID
                return typeID; // 返回消息头类型ID
            }
        }

        public static T ReadData<T>(byte[] data) where T : struct {
            if (data.Length < 4) {
                return default;
            } else {
                int typeID = ReadHeader(data);
                if (typeID != GetTypeID<T>()) {
                    throw new Exception("MessageHelper: Type mismatch");
                } else {
                    int length = BitConverter.ToInt32(data, 4);
                    string str = System.Text.Encoding.UTF8.GetString(data, 8, length);
                    T msg = JsonUtility.FromJson<T>(str);
                    return msg;
                }
            }
        }
    }
}