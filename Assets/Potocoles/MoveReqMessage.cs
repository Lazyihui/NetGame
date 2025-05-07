using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetGame_Protocoles{
    public struct MoveReqMessage {
        public string username; // 角色名称
        public float[] position; // 角色位置
    } 
}