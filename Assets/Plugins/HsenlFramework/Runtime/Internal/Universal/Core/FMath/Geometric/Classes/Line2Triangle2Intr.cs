namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Line2 and Triangle2
    /// </summary>
    public struct Line2Triangle2Intr {
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
        public Vector2 Point0;

        /// <summary>
        /// Second intersection point
        /// </summary>
        public Vector2 Point1;
    }
}