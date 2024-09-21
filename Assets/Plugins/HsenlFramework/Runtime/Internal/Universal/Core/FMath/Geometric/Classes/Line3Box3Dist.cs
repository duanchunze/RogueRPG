#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public struct Line3Box3Dist {
        public Vector3 ClosestPoint0;
        public Vector3 ClosestPoint1;
        public FLOAT LineParameter;
    }
}