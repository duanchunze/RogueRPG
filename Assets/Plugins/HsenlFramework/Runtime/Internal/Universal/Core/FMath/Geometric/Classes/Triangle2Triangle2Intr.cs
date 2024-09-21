namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Triangle2 and Triangle2
    /// </summary>
    public struct Triangle2Triangle2Intr {
        /// <summary>
        /// Equals to:
        /// IntersectionTypes.Empty if no intersection occurs;
        /// IntersectionTypes.Point if triangles are touching in a point;
        /// IntersectionTypes.Segment if triangles are touching in a segment;
        /// IntersectionTypes.Polygon if triangles intersect.
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Number of intersection points.
        /// IntersectionTypes.Empty: 0;
        /// IntersectionTypes.Point: 1;
        /// IntersectionTypes.Segment: 2;
        /// IntersectionTypes.Polygon: 3 to 6.
        /// </summary>
        public int Quantity;

        public Vector2 Point0;

        public Vector2 Point1;

        public Vector2 Point2;

        public Vector2 Point3;

        public Vector2 Point4;

        public Vector2 Point5;

        /// <summary>
        /// Gets intersection point by index (0 to 5). Points could be also accessed individually using Point0,...,Point5 fields.
        /// </summary>
        public Vector2 this[int i] {
            get {
                return i switch {
                    0 => this.Point0,
                    1 => this.Point1,
                    2 => this.Point2,
                    3 => this.Point3,
                    4 => this.Point4,
                    5 => this.Point5,
                    _ => Vector2.Zero,
                };
            }
            internal set {
                switch (i) {
                    case 0:
                        this.Point0 = value;
                        break;
                    case 1:
                        this.Point1 = value;
                        break;
                    case 2:
                        this.Point2 = value;
                        break;
                    case 3:
                        this.Point3 = value;
                        break;
                    case 4:
                        this.Point4 = value;
                        break;
                    case 5:
                        this.Point5 = value;
                        break;
                }
            }
        }
    }
}