#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public struct BrentsRoot {
        /// <summary>
        /// Function root
        /// </summary>
        public FLOAT x;

        /// <summary>
        /// Number of cycles in the inner loop which were performed to find the root.
        /// </summary>
        public int iterations;

        /// <summary>
        /// True when inner loop exceeds maxIterations variable (in which case root is assigned current approximation), false otherwise.
        /// </summary>
        public bool exceededMaxIterations;
    }
}