﻿namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Segment2 and Triangle2
    /// </summary>
    public struct Segment2Triangle2Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Segment
        /// if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Number of intersection points
        /// </summary>
        public int Quantity;

        /// <summary>
        /// First intersection point
        /// </summary>
        public FVector2 Point0;

        /// <summary>
        /// Second intersection point
        /// </summary>
        public FVector2 Point1;
    }
}