namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Ray3 and Rectangle3 (rectangle considered to be solid)
    /// </summary>
    public struct Ray3Rectangle3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
        /// (including the case when a ray lies in the plane of a rectangle)
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point
        /// </summary>
        public FVector3 Point;
    }
}