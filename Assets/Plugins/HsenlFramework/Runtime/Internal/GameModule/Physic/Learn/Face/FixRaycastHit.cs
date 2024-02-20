using FixedMath;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// 射线碰撞
    /// </summary>
    public class FixRaycastHit {
        /// <summary>
        /// 碰撞体
        /// </summary>
        public FixCollider collider;

        /// <summary>
        /// 射线起点
        /// </summary>
        public FVector3 origin;

        /// <summary>
        /// 射线方向
        /// </summary>
        public FVector3 direction;

        /// <summary>
        /// 射线碰撞的面的法线
        /// </summary>
        public FVector3 normal;

        /// <summary>
        /// 起点到碰撞点的长度
        /// </summary>
        public FLOAT fraction;

        private FVector3 m_collisionPoint;
        private bool m_alrealyCalcuCollisionPoint;

        /// <summary>
        /// 碰撞点
        /// </summary>
        public FVector3 collisionPoint {
            get {
                if (!m_alrealyCalcuCollisionPoint) {
                    m_alrealyCalcuCollisionPoint = true;
                    m_collisionPoint = origin + direction.normalized * fraction;
                }

                return m_collisionPoint;
            }
        }
    }
}