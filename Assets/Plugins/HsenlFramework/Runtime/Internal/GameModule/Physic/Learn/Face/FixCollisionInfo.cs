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
        public Vector3 point;
        public Vector3 normal;
        public FLOAT penetration;
    }
}