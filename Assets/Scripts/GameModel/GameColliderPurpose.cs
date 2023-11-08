using System;

namespace Hsenl {
    // Collider的用途, 例如创建一个碰撞体, 目的是要用来做伤害碰撞的检测
    [Flags]
    public enum GameColliderPurpose {
        Receptor = 1, // 受体, 特指人物或物品的碰撞主体
        Detection = 1 << 1, // 检测, 与受体相对应

        Pickable = 1 << 2,
        Picker = 1 << 3,
    }
}