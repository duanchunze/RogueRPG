#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public class FixPhysicWorldManager {
        static FixPhysicWorldManager _instance;

        internal static FixPhysicWorldManager _Instance {
            get {
                if (_instance == null) {
                    _instance = new FixPhysicWorldManager();
                }

                return _instance;
            }
        }

        private FixPhysicWorld m_defaultWorld;

        internal FixPhysicWorld _defaultWorld {
            get {
                if (m_defaultWorld == null) {
                    Init();
                }

                return m_defaultWorld;
            }
        }

        internal void Init(FixPhysicWorld defaultWorld = null) {
            if (this.m_defaultWorld != null) {
                return;
            }

            this.m_defaultWorld = defaultWorld;
            if (this.m_defaultWorld == null) {
                this.m_defaultWorld = new FixPhysicWorld(new FixCollisionWorld());
            }
        }

        internal void Update(FLOAT deltaTime, DetectPrecisionType detectPrecisionType) {
            m_defaultWorld.Update(deltaTime);
        }

        internal void Reset() {
            this.m_defaultWorld = null;
        }
    }
}