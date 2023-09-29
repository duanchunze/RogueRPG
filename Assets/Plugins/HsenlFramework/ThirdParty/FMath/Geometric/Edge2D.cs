#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Polygon2 edge
    /// </summary>
    public struct Edge2D {
        /// <summary>
        /// Edge start vertex
        /// </summary>
        public FVector2 point0;

        /// <summary>
        /// Edge end vertex
        /// </summary>
        public FVector2 point1;

        /// <summary>
        /// Unit length direction vector
        /// </summary>
        public FVector2 direction;

        /// <summary>
        /// Unit length normal vector
        /// </summary>
        public FVector2 normal;

        /// <summary>
        /// Edge length
        /// </summary>
        public FLOAT length;
    }
}