#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public struct CubicRoots {
        public FLOAT x0;

        public FLOAT x1;

        public FLOAT x2;

        public int rootCount;

        public FLOAT this[int rootIndex] => rootIndex switch {
            0 => this.x0,
            1 => this.x1,
            2 => this.x2,
            _ => FLOAT.NaN,
        };
    }
}