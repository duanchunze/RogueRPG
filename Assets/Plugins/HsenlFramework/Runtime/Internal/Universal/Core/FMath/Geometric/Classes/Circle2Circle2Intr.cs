namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of two Circle2.
    /// The quantity Q is 0, 1, or 2. When Q &gt; 0, the interpretation depends
    /// on the intersection type.
    ///   IntersectionTypes.Point:  Q distinct points of intersection
    ///   IntersectionTypes.Other:  The circles are the same
    /// </summary>
    public struct Circle2Circle2Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if there is intersection,
        /// IntersectionTypes.Other if circles are the same and IntersectionTypes.Empty
        /// if circles do not intersect
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Number of intersection points
        /// </summary>
        public int Quantity;

        /// <summary>
        /// First intersection point
        /// </summary>
        public Vector2 Point0;

        /// <summary>
        /// Second intersection point
        /// </summary>
        public Vector2 Point1;
    }
}