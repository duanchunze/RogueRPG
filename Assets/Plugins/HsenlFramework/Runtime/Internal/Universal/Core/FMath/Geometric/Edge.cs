#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Polygon3 edge
    /// </summary>
    public struct Edge {
        /// <summary>
        /// Edge start vertex
        /// </summary>
        public Vector3 point0;

        /// <summary>
        /// Edge end vertex
        /// </summary>
        public Vector3 point1;

        /// <summary>
        /// Unit length direction vector
        /// </summary>
        public Vector3 direction;

        /// <summary>
        /// Unit length normal vector
        /// </summary>
        public Vector3 normal;

        /// <summary>
        /// Edge length
        /// </summary>
        public FLOAT length;
    }
}