namespace Hsenl {
    /// <summary>
    /// 试想一下,一个有N个物体的场景,如果我们对这些物体每两个之间进行碰撞检测,需要的计算复杂度是 [N²] ,对于计算机显然是不能接受的.
    /// 所以我们将碰撞检测分成两个阶段来实现,Broad-Phase和Narrow-Phase.
    /// Broad-Phase使用某种Bounding Volume（比如AABB）来表示刚体的碰撞信息,然后用空间划分的方式来保存这些Bounding Volume,就可以再较短的时间内筛选出可能互相碰撞的刚体对.
    /// Narrow-Phase就是将这些刚体对进行真正的精确碰撞检测.
    /// </summary>
    public interface IBroadPhase {
        /// <summary>
        /// 获得包围盒
        /// </summary>
        AABB boundingBox { get; }

        /// <summary>
        /// 获得盒子
        /// </summary>
        Box box { get; }
    }
}