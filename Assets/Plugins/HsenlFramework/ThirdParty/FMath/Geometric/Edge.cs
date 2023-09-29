#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Polygon3 edge
    /// </summary>
    public struct Edge {
        /// <summary>
        /// Edge start vertex
        /// </summary>
        public FVector3 point0;

        /// <summary>
        /// Edge end vertex
        /// </summary>
        public FVector3 point1;

        /// <summary>
        /// Unit length direction vector
        /// </summary>
        public FVector3 direction;

        /// <summary>
        /// Unit length normal vector
        /// </summary>
        public FVector3 normal;

        /// <summary>
        /// Edge length
        /// </summary>
        public FLOAT length;
    }
}