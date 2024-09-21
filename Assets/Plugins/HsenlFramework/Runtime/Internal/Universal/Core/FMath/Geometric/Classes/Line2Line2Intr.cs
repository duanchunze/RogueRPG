#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of two Line2
    /// </summary>
    public struct Line2Line2Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Line (lines are the same)
        /// if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// In case of IntersectionTypes.Point constains single point of intersection.
        /// OtherwiseFVector2.zero.
        /// </summary>
        public Vector2 Point;

        /// <summary>
        /// In case of IntersectionTypes.Point contains evaluation parameter of single
        /// intersection point according to first line.
        /// Otherwise 0.
        /// </summary>
        public FLOAT Parameter;
    }
}