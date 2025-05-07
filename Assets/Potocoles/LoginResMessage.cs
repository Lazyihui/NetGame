using System;

namespace NetGame_Protocoles {
    // request message 客服端发给服务端
    public struct LoginResMessage {
        public sbyte statusCode; //1成功 -1失败
    }
}