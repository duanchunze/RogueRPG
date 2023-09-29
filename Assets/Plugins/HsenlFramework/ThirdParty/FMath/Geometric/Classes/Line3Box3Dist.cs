#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public struct Line3Box3Dist {
        public FVector3 ClosestPoint0;
        public FVector3 ClosestPoint1;
        public FLOAT LineParameter;
    }
}