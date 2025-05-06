
using System;
using System.Diagnostics;
using TinyJson;
using Unity.Plastic.Antlr3.Runtime.Tree;

namespace NetGame_Protocoles {

    public static class MessageHeper {

        public static byte[] ToData(int typeID, object msg) {
            byte[] msg_header = BitConverter.GetBytes(typeID); // 消息头给定类型ID
            byte[] msg_data = System.Text.Encoding.UTF8.GetBytes(msg.ToJson()); // 消息体

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
    }
}