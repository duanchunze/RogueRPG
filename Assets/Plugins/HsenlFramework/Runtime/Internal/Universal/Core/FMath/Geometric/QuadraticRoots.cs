#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public struct QuadraticRoots {
        public FLOAT x0;

        public FLOAT x1;

        public int rootCount;

        public FLOAT this[int rootIndex] => rootIndex switch {
            0 => this.x0,
            1 => this.x1,
            _ => FLOAT.NaN,
        };
    }
}