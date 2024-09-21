namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Segment3 and Polygon3 (polygon considered to be solid)
    /// </summary>
    public struct Segment3Polygon3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
        /// (including the case when a segment lies in the plane of a polygon)
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point
        /// </summary>
        public Vector3 Point;
    }
}