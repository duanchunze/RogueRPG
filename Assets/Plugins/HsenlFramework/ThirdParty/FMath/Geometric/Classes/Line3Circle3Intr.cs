namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Line3 and Circle3 (circle considered to be solid)
    /// </summary>
    public struct Line3Circle3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
        /// (including the case when a line lies in the plane of a circle)
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point
        /// </summary>
        public FVector3 Point;
    }
}