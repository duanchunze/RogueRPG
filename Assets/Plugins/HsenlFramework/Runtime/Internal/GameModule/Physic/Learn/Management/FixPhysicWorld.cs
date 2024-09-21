#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public class FixPhysicWorld {
        private FixCollisionWorld m_CollisionWorld;
        internal FixCollisionWorld _collisionWorld => m_CollisionWorld;

        internal FixPhysicWorld(FixCollisionWorld collisionWorld) {
            this.m_CollisionWorld = collisionWorld;
        }

        internal void Update(FLOAT deltaTime) {
            m_CollisionWorld.DetectWorld();
        }
    }
}