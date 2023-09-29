#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Ray3 and Plane3
    /// </summary>
    public struct Ray3Plane3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Ray (a ray lies in a plane) if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FVector3 Point;

        /// <summary>
        /// Ray evaluation parameter of the intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FLOAT RayParameter;
    }
}