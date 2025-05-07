using System;
using UnityEngine.Experimental.GlobalIllumination;

namespace NetGame_Protocoles {

    public struct RoleSpawnReqMessage {
        public string username; // 用户名
        public float[] position; // 角色位置
        
    }
}