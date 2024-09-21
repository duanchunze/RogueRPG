#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of two Ray2
    /// </summary>
    public struct Ray2Ray2Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Ray (rays are collinear and overlap in more than one point)
        /// if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// In case of IntersectionTypes.Point constains single point of intersection.
        /// In case of IntersectionTypes.Ray contains second ray's origin.
        /// OtherwiseFVector2.zero.
        /// </summary>
        public Vector2 Point;

        /// <summary>
        /// In case of IntersectionTypes.Point contains evaluation parameter of single
        /// intersection point according to first ray.
        /// In case of IntersectionTypes.Ray contains evaluation parameter of the
        /// second ray's origin according to first ray.
        /// Otherwise 0.
        /// </summary>
        public FLOAT Parameter;
    }
}