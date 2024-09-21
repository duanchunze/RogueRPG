namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Line3 and AxisAlignedBox3
    /// </summary>
    public struct Line3AAB3Intr {
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
        public Vector3 Point0;

        /// <summary>
        /// Second intersection point
        /// </summary>
        public Vector3 Point1;
    }
}