namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Segment3 and Circle3 (circle considered to be solid)
    /// </summary>
    public struct Segment3Circle3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
        /// (including the case when a segment lies in the plane of a circle)
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point
        /// </summary>
        public FVector3 Point;
    }
}