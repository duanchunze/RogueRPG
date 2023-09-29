namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Triangle3 and Triangle3
    /// </summary>
    public struct Triangle3Triangle3Intr {
        /// <summary>
        /// Equals to:
        /// IntersectionTypes.Empty if no intersection occurs;
        /// IntersectionTypes.Point if non-coplanar triangles touch in a point, see Touching member for the description;
        /// IntersectionTypes.Segment if non-coplanar triangles intersect or are touch in a segment, see Touching member for the description;
        /// IntersectionTypes.Plane if reportCoplanarIntersections is specified to true when calling Find method and triangles are coplanar
        /// and intersect, if reportCoplanarIntersections is specified to false, coplanar triangles are reported as not intersecting.
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// If triangles are non-coplanar equals to IntersectionType.Empty. If triangles are coplanar, equals to the following options:
        /// IntersectionTypes.Empty if coplanar triangles do not intersect;
        /// IntersectionTypes.Point is coplanar triangles touch in a point;
        /// IntersectionTypes.Segment if coplanar triangles touch in a segment;
        /// IntersectionTypes.Polygon if coplanar triangles intersect.
        /// </summary>
        public IntersectionTypes CoplanarIntersectionType;

        /// <summary>
        /// Equals to true if triangles are non-coplanar and touching in a point 
        /// (IntersectionTypes.Point; touch variants are: a vertex lies in the plane of a triangle and contained by a triangle (including border), two non-collinear edges touch) or
        /// if triangles are not coplanar and touching in a segment
        /// (IntersectionTypes.Segment; an edge lies in the plane of a triangle and intersects triangle in more than one point).
        /// Generally speaking, touching is true when non-coplanar triangles touch each other by some parts of their borders.
        /// Otherwise false.
        /// </summary>
        public bool Touching;

        /// <summary>
        /// Number of intersection points.
        /// IntersectionTypes.Empty: 0;
        /// IntersectionTypes.Point: 1;
        /// IntersectionTypes.Segment: 2;
        /// IntersectionTypes.Polygon: 1 to 6.
        /// </summary>
        public int Quantity;

        public FVector3 Point0;

        public FVector3 Point1;

        public FVector3 Point2;

        public FVector3 Point3;

        public FVector3 Point4;

        public FVector3 Point5;

        /// <summary>
        /// Gets intersection point by index (0 to 5). Points could be also accessed individually using Point0,...,Point5 fields.
        /// </summary>
        public FVector3 this[int i] {
            get {
                return i switch {
                    0 => this.Point0,
                    1 => this.Point1,
                    2 => this.Point2,
                    3 => this.Point3,
                    4 => this.Point4,
                    5 => this.Point5,
                    _ => FVector3.Zero,
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