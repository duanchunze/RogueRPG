#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Segment3 and Plane3
    /// </summary>
    public struct Segment3Plane3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Segment (a segment lies in a plane) if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FVector3 Point;

        /// <summary>
        /// Segment evaluation parameter of the intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FLOAT SegmentParameter;
    }
}