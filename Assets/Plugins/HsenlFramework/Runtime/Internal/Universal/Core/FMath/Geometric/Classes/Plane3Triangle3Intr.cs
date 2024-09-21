namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Plane3 and Triangle3
    /// </summary>
    public struct Plane3Triangle3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point (a triangle is touching a plane by a vertex) or
        /// IntersectionTypes.Segment (a triangle is touching a plane by an edge or intersecting the plane) or
        /// IntersectionTypes.Polygon (a triangle is lying in a plane), otherwise IntersectionTypes.Empty (no intersection).
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Number of intersection points.
        /// 0 - IntersectionTypes.Empty;
        /// 1 - IntersectionTypes.Point;
        /// 2 - IntersectionTypes.Segment;
        /// 3 - IntersectionTypes.Polygon;
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

        /// <summary>
        /// Third intersection point
        /// </summary>
        public Vector3 Point2;
    }
}