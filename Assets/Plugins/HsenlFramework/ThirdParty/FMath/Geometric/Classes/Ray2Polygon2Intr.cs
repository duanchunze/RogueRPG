#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Ray2 and general Polygon2
    /// </summary>
    public struct Ray2Polygon2Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Segment (ray and some polygon segment are collinear and overlap in more than one point)
        /// if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// In case of IntersectionTypes.Point constains single point of intersection.
        /// In case of IntersectionTypes.Segment contains first point of intersection.
        /// OtherwiseFVector2.zero.
        /// </summary>
        public FVector2 Point0;

        /// <summary>
        /// In case of IntersectionTypes.Segment contains second point of intersection.
        /// OtherwiseFVector2.zero.
        /// </summary>
        public FVector2 Point1;

        /// <summary>
        /// In case of IntersectionTypes.Point contains evaluation parameter of single
        /// intersection point according to ray.
        /// In case of IntersectionTypes.Segment contains evaluation parameter of the
        /// first intersection point according to ray.
        /// Otherwise 0.
        /// </summary>
        public FLOAT Parameter0;

        /// <summary>
        /// In case of IntersectionTypes.Segment contains evaluation parameter of the
        /// second intersection point according to ray.
        /// Otherwise 0.
        /// </summary>
        public FLOAT Parameter1;
    }
}