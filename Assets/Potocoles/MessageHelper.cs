
using System;
using TinyJson;
using Unity.Plastic.Antlr3.Runtime.Tree;

namespace NetGame_Protocoles {

    public static class MessageHeper {
        // public static byte[] ToData(int typeID, object obj) {
        //     // // 1.序列化数据
        //     // string str = obj.ToJson(); // 序列化
        //     // byte[] data = System.Text.Encoding.UTF8.GetBytes(str); // 转换为字节数组
        //     // return data; // 返回字节数组

        //     // 2.序列化数据
        //     string str = obj.ToJson(); // 序列化
        //     byte[] msg_header = BitConverter.GetBytes(typeID); // 消息头

        // }

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
    }
}