using System;

namespace Hsenl {
    // Collider的用途, 例如创建一个碰撞体, 目的是要用来做伤害碰撞的检测
    [Flags]
    public enum GameColliderPurpose {
        Body = 1,
        BodyTrigger = 1 << 1,
        Pickable = 1 << 2,
        Picker = 1 << 3,
    }
}