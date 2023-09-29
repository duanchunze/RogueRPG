﻿#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Line3 and Sphere3
    /// </summary>
    public struct Line3Sphere3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Segment if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Number of intersection points.
        /// IntersectionTypes.Empty: 0;
        /// IntersectionTypes.Point: 1;
        /// IntersectionTypes.Segment: 2.
        /// </summary>
        public int Quantity;

        /// <summary>
        /// First intersection point
        /// </summary>
        public FVector3 Point0;

        /// <summary>
        /// Second intersection point
        /// </summary>
        public FVector3 Point1;

        /// <summary>
        /// Line evaluation parameter of the first intersection point
        /// </summary>
        public FLOAT LineParameter0;

        /// <summary>
        /// Line evaluation parameter of the second intersection point
        /// </summary>
        public FLOAT LineParameter1;
    }
}