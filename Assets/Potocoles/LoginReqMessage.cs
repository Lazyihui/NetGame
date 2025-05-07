using System;

namespace NetGame_Protocoles {
    // request message 客服端发给服务端
    public struct LoginReqMessage {
        public string username; // 用户名
        public string password; // 密码
    }
}