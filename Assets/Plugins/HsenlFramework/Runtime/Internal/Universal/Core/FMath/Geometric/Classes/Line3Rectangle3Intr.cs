namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Line3 and Rectangle3 (rectangle considered to be solid)
    /// </summary>
    public struct Line3Rectangle3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
        /// (including the case when a line lies in the plane of a rectangle)
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point
        /// </summary>
        public Vector3 Point;
    }
}