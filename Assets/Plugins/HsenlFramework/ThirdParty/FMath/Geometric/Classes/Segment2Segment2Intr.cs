﻿#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of two Segment2.
    /// </summary>
    public struct Segment2Segment2Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Segment (segments are collinear and overlap in more than one point)
        /// if intersection occured otherwise IntersectionTypes.Empty.
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// In case of IntersectionTypes.Point contains single point of intersection.
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
        /// intersection point according to first segment.
        /// In case of IntersectionTypes.Segment contains evaluation parameter of the
        /// first intersection point according to first segment.
        /// Otherwise 0.
        /// </summary>
        public FLOAT Parameter0;

        /// <summary>
        /// In case of IntersectionTypes.Segment contains evaluation parameter of the
        /// second intersection point according to first segment.
        /// Otherwise 0.
        /// </summary>
        public FLOAT Parameter1;
    }
}