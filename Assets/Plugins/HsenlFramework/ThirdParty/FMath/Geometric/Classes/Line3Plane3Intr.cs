#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Line3 and Plane3
    /// </summary>
    public struct Line3Plane3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Line (a line lies in a plane) if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FVector3 Point;

        /// <summary>
        /// Line evaluation parameter of the intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FLOAT LineParameter;
    }
}