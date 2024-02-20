using FixedMath;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// 碰撞信息
    /// </summary>
    public struct FixCollisionInfo {
        public FixCollider collider;
        public FVector3 point;
        public FVector3 normal;
        public FLOAT penetration;
    }
}