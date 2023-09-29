#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Line2 and Ray2
    /// </summary>
    public struct Line2Ray2Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Segment (line and ray are collinear)
        /// if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// In case of IntersectionTypes.Point constains single point of intersection.
        /// OtherwiseFVector2.zero.
        /// </summary>
        public FVector2 Point;

        /// <summary>
        /// In case of IntersectionTypes.Point contains evaluation parameter of single
        /// intersection point according to line.
        /// Otherwise 0.
        /// </summary>
        public FLOAT Parameter;
    }
}