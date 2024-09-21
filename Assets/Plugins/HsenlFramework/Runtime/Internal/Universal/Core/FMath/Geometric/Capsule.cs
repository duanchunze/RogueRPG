#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Capsule is defined by the volume around the segment with certain radius.
    /// </summary>
    public struct Capsule {
        /// <summary>
        /// Capsule base segment
        /// </summary>
        public Segment segment;

        /// <summary>
        /// Capsule radius
        /// </summary>
        public FLOAT radius;

        /// <summary>
        /// Creates new Capsule3 instance.
        /// </summary>
        public Capsule(ref Segment segment, FLOAT radius) {
            this.segment = segment;
            this.radius = radius;
        }

        /// <summary>
        /// Creates new Capsule3 instance.
        /// </summary>
        public Capsule(Segment segment, FLOAT radius) {
            this.segment = segment;
            this.radius = radius;
        }
    }
}