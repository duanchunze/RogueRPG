using System;

namespace Hsenl {
    // Collider的用途, 例如创建一个碰撞体, 目的是要用来做伤害碰撞的检测
    [Flags]
    public enum GameColliderPurpose {
        /// 受体, 特指人物或物品的碰撞主体
        Receptor = 1,

        /// 检测, 与受体相对应
        Detection = 1 << 1,

        /// 被拾取物的碰撞体
        Pickable = 1 << 2,
        
        /// 检测拾取物的碰撞体
        Picker = 1 << 3,
    }
}