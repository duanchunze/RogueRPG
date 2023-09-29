#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public enum IntersectionTypes {
        /// <summary>
        /// Entities do not intersect
        /// </summary>
        Empty,

        /// <summary>
        /// Entities intersect in a point
        /// </summary>
        Point,

        /// <summary>
        /// Entities intersect in a segment
        /// </summary>
        Segment,

        /// <summary>
        /// Entities intersect in a ray
        /// </summary>
        Ray,

        /// <summary>
        /// Entities intersect in a line
        /// </summary>
        Line,

        /// <summary>
        /// Entities intersect in a polygon
        /// </summary>
        Polygon,

        /// <summary>
        /// Entities intersect in a plane
        /// </summary>
        Plane,

        /// <summary>
        /// Entities intersect in a polyhedron
        /// </summary>
        Polyhedron,

        /// <summary>
        /// Entities intersect somehow
        /// </summary>
        Other
    }

    /// <summary>
    /// Contains various intersection methods.
    /// </summary>
    public static class Intersection {
        private static FLOAT _intervalThreshold;

        private static FLOAT _dotThreshold;

        private static FLOAT _distanceThreshold;

        /// <summary>
        /// Used in interval comparisons. Default is FMathEx.ZeroTolerance.
        /// </summary>
        public static FLOAT IntervalThreshold {
            get { return _intervalThreshold; }
            set {
                if (value >= 0f) {
                    _intervalThreshold = value;
                }
                else {
                    _intervalThreshold = 0; // Interval threshold must be nonnegative.
                }
            }
        }

        /// <summary>
        /// Used in dot product comparisons. Default is FMathEx.ZeroTolerance.
        /// </summary>
        public static FLOAT DotThreshold {
            get { return _dotThreshold; }
            set {
                if (value >= 0f) {
                    _dotThreshold = value;
                }
                else {
                    _dotThreshold = 0; // Dot threshold must be nonnegative.
                }
            }
        }

        /// <summary>
        /// Used in distance comparisons. Default is FMathEx.ZeroTolerance.
        /// </summary>
        public static FLOAT DistanceThreshold {
            get { return _distanceThreshold; }
            set {
                if (value >= 0f) {
                    _distanceThreshold = value;
                }
                else {
                    _distanceThreshold = 0; // Distance threshold must be nonnegative.
                }
            }
        }

        /// <summary>
        /// Tests whether two AAB intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestAABB2AABB2(ref AABB2D box0, ref AABB2D box1) {
            if (box0.max.x < box1.min.x || box0.min.x > box1.max.x) {
                return false;
            }

            if (box0.max.y < box1.min.y || box0.min.y > box1.max.y) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether two AAB intersect and finds intersection which is AAB itself.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindAABB2AABB2(ref AABB2D box0, ref AABB2D box1, out AABB2D intersection) {
            if (box0.max.x < box1.min.x || box0.min.x > box1.max.x) {
                intersection = default(AABB2D);
                return false;
            }

            if (box0.max.y < box1.min.y || box0.min.y > box1.max.y) {
                intersection = default(AABB2D);
                return false;
            }

            intersection.max.x = ((box0.max.x <= box1.max.x) ? box0.max.x : box1.max.x);
            intersection.min.x = ((box0.min.x <= box1.min.x) ? box1.min.x : box0.min.x);
            intersection.max.y = ((box0.max.y <= box1.max.y) ? box0.max.y : box1.max.y);
            intersection.min.y = ((box0.min.y <= box1.min.y) ? box1.min.y : box0.min.y);
            return true;
        }

        /// <summary>
        /// Checks whether two aab has x overlap
        /// </summary>
        public static bool TestAABB2AABB2OverlapX(ref AABB2D box0, ref AABB2D box1) {
            if (box0.max.x >= box1.min.x) {
                return box0.min.x <= box1.max.x;
            }

            return false;
        }

        /// <summary>
        /// Checks whether two aab has y overlap
        /// </summary>
        public static bool TestAABB2AABB2OverlapY(ref AABB2D box0, ref AABB2D box1) {
            if (box0.max.y >= box1.min.y) {
                return box0.min.y <= box1.max.y;
            }

            return false;
        }

        /// <summary>
        /// Tests if an axis aligned box intersects a circle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestAABB2Circle2(ref AABB2D box, ref Circle2D circle) {
            FLOAT num = 0f;
            FLOAT x = circle.center.x;
            if (x < box.min.x) {
                FLOAT num2 = x - box.min.x;
                num += num2 * num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
            }

            x = circle.center.y;
            if (x < box.min.y) {
                FLOAT num2 = x - box.min.y;
                num += num2 * num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
            }

            return num <= circle.radius * circle.radius;
        }

        /// <summary>
        /// Tests if a box intersects another box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestBox2Box2(ref Box2D box0, ref Box2D box1) {
            FVector2 axis = box0.axis0;
            FVector2 axis2 = box0.axis1;
            FVector2 axis3 = box1.axis0;
            FVector2 axis4 = box1.axis1;
            FLOAT x = box0.extents.x;
            FLOAT y = box0.extents.y;
            FLOAT x2 = box1.extents.x;
            FLOAT y2 = box1.extents.y;
            FVector2 value = box1.center - box0.center;
            FLOAT num = FMath.Abs(axis.Dot(axis3));
            FLOAT num2 = FMath.Abs(axis.Dot(axis4));
            FLOAT num3 = FMath.Abs(axis.Dot(value));
            FLOAT num4 = x + x2 * num + y2 * num2;
            if (num3 > num4) {
                return false;
            }

            FLOAT num5 = FMath.Abs(axis2.Dot(axis3));
            FLOAT num6 = FMath.Abs(axis2.Dot(axis4));
            num3 = FMath.Abs(axis2.Dot(value));
            num4 = y + x2 * num5 + y2 * num6;
            if (num3 > num4) {
                return false;
            }

            num3 = FMath.Abs(axis3.Dot(value));
            num4 = x2 + x * num + y * num5;
            if (num3 > num4) {
                return false;
            }

            num3 = FMath.Abs(axis4.Dot(value));
            num4 = y2 + x * num2 + y * num6;
            if (num3 > num4) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a box intersects a circle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestBox2Circle2(ref Box2D box, ref Circle2D circle) {
            FLOAT num = 0f;
            FVector2 vector = circle.center - box.center;
            FLOAT num2 = vector.Dot(box.axis0);
            FLOAT x = box.extents.x;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
            }

            num2 = vector.Dot(box.axis1);
            x = box.extents.y;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
            }

            return num <= circle.radius * circle.radius;
        }

        /// <summary>
        /// Tests if a circle intersects another circle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestCircle2Circle2(ref Circle2D circle0, ref Circle2D circle1) {
            FVector2 vector = circle0.center - circle1.center;
            FLOAT num = circle0.radius + circle1.radius;
            return vector.sqrMagnitude <= num * num;
        }

        /// <summary>
        /// Tests if a circle intersects another circle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindCircle2Circle2(ref Circle2D circle0, ref Circle2D circle1, out Circle2Circle2Intr info) {
            info.Point0 = (info.Point1 = FVector2.Zero);
            FVector2 vector = circle1.center - circle0.center;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT radius = circle0.radius;
            FLOAT radius2 = circle1.radius;
            FLOAT num = radius - radius2;
            if (sqrMagnitude < 9.99999944E-11f && FMath.Abs(num) < 1E-05f) {
                info.IntersectionType = IntersectionTypes.Other;
                info.Quantity = 0;
                return true;
            }

            FLOAT num2 = num * num;
            if (sqrMagnitude < num2) {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Quantity = 0;
                return false;
            }

            FLOAT num3 = radius + radius2;
            FLOAT num4 = num3 * num3;
            if (sqrMagnitude > num4) {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Quantity = 0;
                return false;
            }

            if (sqrMagnitude < num4) {
                if (num2 < sqrMagnitude) {
                    FLOAT num5 = 1f / sqrMagnitude;
                    FLOAT num6 = 0.5f * ((radius * radius - radius2 * radius2) * num5 + 1f);
                    FVector2 vector2 = circle0.center + num6 * vector;
                    FLOAT num7 = radius * radius * num5 - num6 * num6;
                    if (num7 < 0f) {
                        num7 = 0f;
                    }

                    FLOAT num8 = FMath.Sqrt(num7);
                    FVector2 vector3 = new FVector2(vector.y, 0f - vector.x);
                    info.Quantity = 2;
                    info.Point0 = vector2 - num8 * vector3;
                    info.Point1 = vector2 + num8 * vector3;
                }
                else {
                    info.Quantity = 1;
                    info.Point0 = circle0.center + radius / num * vector;
                }
            }
            else {
                info.Quantity = 1;
                info.Point0 = circle0.center + radius / num3 * vector;
            }

            info.IntersectionType = IntersectionTypes.Point;
            return true;
        }

        private static int WhichSide(Polygon2D V, FVector2 P, ref FVector2 D) {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < V.vertexCount; i++) {
                FLOAT num4 = D.Dot(V[i] - P);
                if (num4 > 0f) {
                    num++;
                }
                else if (num4 < 0f) {
                    num2++;
                }
                else {
                    num3++;
                }

                if (num > 0 && num2 > 0) {
                    return 0;
                }
            }

            if (num3 != 0) {
                return 0;
            }

            if (num <= 0) {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Tests whether two convex CCW ordered polygons intersect.
        /// Returns true if intersection occurs false otherwise.
        /// Note that caller is responsibile for supplying proper polygons (convex and CCW ordered).
        /// </summary>
        public static bool TestConvexPolygon2ConvexPolygon2(Polygon2D convexPolygon0, Polygon2D convexPolygon1) {
            int i = 0;
            int vertexIndex = convexPolygon0.vertexCount - 1;
            for (; i < convexPolygon0.vertexCount; i++) {
                FVector2 d = (convexPolygon0[i] - convexPolygon0[vertexIndex]).Perpendicular();
                if (WhichSide(convexPolygon1, convexPolygon0[i], ref d) > 0) {
                    return false;
                }

                vertexIndex = i;
            }

            int j = 0;
            int vertexIndex2 = convexPolygon1.vertexCount - 1;
            for (; j < convexPolygon1.vertexCount; j++) {
                FVector2 d = (convexPolygon1[j] - convexPolygon1[vertexIndex2]).Perpendicular();
                if (WhichSide(convexPolygon0, convexPolygon1[j], ref d) > 0) {
                    return false;
                }

                vertexIndex2 = j;
            }

            return true;
        }

        private static bool DoClipping(FLOAT t0, FLOAT t1, ref FVector2 origin, ref FVector2 direction,
            ref AABB2D box, bool solid, out int quantity, out FVector2 point0, out FVector2 point1,
            out IntersectionTypes intrType) {
            box.CalcCenterExtents(out var center, out var extents);
            FVector2 vector = new FVector2(origin.x - center.x, origin.y - center.y);
            FLOAT num = t0;
            FLOAT num2 = t1;
            if (Clip(direction.x, 0f - vector.x - extents.x, ref t0, ref t1) &&
                Clip(0f - direction.x, vector.x - extents.x, ref t0, ref t1) &&
                Clip(direction.y, 0f - vector.y - extents.y, ref t0, ref t1) &&
                Clip(0f - direction.y, vector.y - extents.y, ref t0, ref t1) && (solid || t0 != num || t1 != num2)) {
                if (t1 > t0) {
                    intrType = IntersectionTypes.Segment;
                    quantity = 2;
                    point0 = origin + t0 * direction;
                    point1 = origin + t1 * direction;
                }
                else {
                    intrType = IntersectionTypes.Point;
                    quantity = 1;
                    point0 = origin + t0 * direction;
                    point1 = FVector2.Zero;
                }
            }
            else {
                intrType = IntersectionTypes.Empty;
                quantity = 0;
                point0 = FVector2.Zero;
                point1 = FVector2.Zero;
            }

            return intrType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a line intersects an axis aligned box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine2AABB2(ref Line2D line, ref AABB2D box) {
            FVector2 vector = line.direction.Perpendicular();
            FVector2 vector2 = default(FVector2);
            FVector2 vector3 = default(FVector2);
            if (vector.x >= 0f) {
                vector2.x = box.min.x;
                vector3.x = box.max.x;
            }
            else {
                vector2.x = box.max.x;
                vector3.x = box.min.x;
            }

            if (vector.y >= 0f) {
                vector2.y = box.min.y;
                vector3.y = box.max.y;
            }
            else {
                vector2.y = box.max.y;
                vector3.y = box.min.y;
            }

            FLOAT num = vector.Dot(vector2 - line.origin);
            if (num >= 0f) {
                return false;
            }

            FLOAT num2 = vector.Dot(vector3 - line.origin);
            return num2 > 0f;
        }

        /// <summary>
        /// Tests if a line intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine2AABB2(ref Line2D line, ref AABB2D box, out Line2AAB2Intr info) {
            return DoClipping(FLOAT.NegativeInfinity, FLOAT.PositiveInfinity, ref line.origin, ref line.direction,
                ref box, solid: true, out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        private static bool Clip(FLOAT denom, FLOAT numer, ref FLOAT t0, ref FLOAT t1) {
            if (denom > 0f) {
                if (numer > denom * t1) {
                    return false;
                }

                if (numer > denom * t0) {
                    t0 = numer / denom;
                }

                return true;
            }

            if (denom < 0f) {
                if (numer > denom * t0) {
                    return false;
                }

                if (numer > denom * t1) {
                    t1 = numer / denom;
                }

                return true;
            }

            return numer <= 0f;
        }

        private static bool DoClipping(FLOAT t0, FLOAT t1, ref FVector2 origin, ref FVector2 direction, ref Box2D box,
            bool solid, out int quantity, out FVector2 point0, out FVector2 point1,
            out IntersectionTypes intrType) {
            FVector2 vector = new FVector2(origin.x - box.center.x, origin.y - box.center.y);
            FVector2 vector2 = new FVector2(vector.Dot(box.axis0), vector.Dot(box.axis1));
            FVector2 vector3 = new FVector2(direction.Dot(box.axis0), direction.Dot(box.axis1));
            FLOAT num = t0;
            FLOAT num2 = t1;
            if (Clip(vector3.x, 0f - vector2.x - box.extents.x, ref t0, ref t1) &&
                Clip(0f - vector3.x, vector2.x - box.extents.x, ref t0, ref t1) &&
                Clip(vector3.y, 0f - vector2.y - box.extents.y, ref t0, ref t1) &&
                Clip(0f - vector3.y, vector2.y - box.extents.y, ref t0, ref t1) && (solid || t0 != num || t1 != num2)) {
                if (t1 > t0) {
                    intrType = IntersectionTypes.Segment;
                    quantity = 2;
                    point0 = origin + t0 * direction;
                    point1 = origin + t1 * direction;
                }
                else {
                    intrType = IntersectionTypes.Point;
                    quantity = 1;
                    point0 = origin + t0 * direction;
                    point1 = FVector2.Zero;
                }
            }
            else {
                intrType = IntersectionTypes.Empty;
                quantity = 0;
                point0 = FVector2.Zero;
                point1 = FVector2.Zero;
            }

            return intrType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether line and box intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine2Box2(ref Line2D line, ref Box2D box) {
            FVector2 value = line.origin - box.center;
            FVector2 vector = line.direction.Perpendicular();
            FLOAT num = FMath.Abs(vector.Dot(value));
            FLOAT num2 = FMath.Abs(vector.Dot(box.axis0));
            FLOAT num3 = FMath.Abs(vector.Dot(box.axis1));
            FLOAT num4 = box.extents.x * num2 + box.extents.y * num3;
            return num <= num4;
        }

        /// <summary>
        /// Tests whether line and box intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine2Box2(ref Line2D line, ref Box2D box, out Line2Box2Intr info) {
            return DoClipping(FLOAT.NegativeInfinity, FLOAT.PositiveInfinity, ref line.origin, ref line.direction,
                ref box, solid: true, out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        private static bool Find(ref FVector2 origin, ref FVector2 direction, ref FVector2 center, FLOAT radius,
            out int rootCount, out FLOAT t0, out FLOAT t1) {
            FVector2 value = origin - center;
            FLOAT num = value.sqrMagnitude - radius * radius;
            FLOAT num2 = direction.Dot(value);
            FLOAT num3 = num2 * num2 - num;
            if (num3 > 1E-05f) {
                rootCount = 2;
                num3 = FMath.Sqrt(num3);
                t0 = 0f - num2 - num3;
                t1 = 0f - num2 + num3;
            }
            else if (num3 < -1E-05f) {
                rootCount = 0;
                t0 = (t1 = 0f);
            }
            else {
                rootCount = 1;
                t0 = 0f - num2;
                t1 = 0f;
            }

            return rootCount != 0;
        }

        /// <summary>
        /// Tests whether line and circle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine2Circle2(ref Line2D line, ref Circle2D circle) {
            FVector2 rhs = line.origin - circle.center;
            FLOAT num = rhs.sqrMagnitude - circle.radius * circle.radius;
            if (num <= 1E-05f) {
                return true;
            }

            FVector2.Dot(ref line.direction, ref rhs, out var num2);
            FLOAT num3 = num2 * num2 - num;
            return num3 >= -1E-05f;
        }

        /// <summary>
        /// Tests whether line and circle intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine2Circle2(ref Line2D line, ref Circle2D circle, out Line2Circle2Intr info) {
            if (Find(ref line.origin, ref line.direction, ref circle.center, circle.radius, out var rootCount,
                    out var t, out var t2)) {
                if (rootCount == 1) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point0 = line.origin + t * line.direction;
                    info.Point1 = FVector2.Zero;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Segment;
                    info.Point0 = line.origin + t * line.direction;
                    info.Point1 = line.origin + t2 * line.direction;
                }
            }
            else {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Point0 = (info.Point1 = FVector2.Zero);
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a line intersects a convex polygon. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine2ConvexPolygon2(ref Line2D line, Polygon2D convexPolygon) {
            Edge2D[] edges = convexPolygon.edges;
            int num = edges.Length;
            for (int i = 0; i < num; i++) {
                Segment2D segment = new Segment2D(ref edges[i].point0, ref edges[i].point1);
                if (TestLine2Segment2(ref line, ref segment)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if a line intersects a convex ccw ordered polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine2ConvexPolygon2(ref Line2D line, Polygon2D convexPolygon,
            out Line2ConvexPolygon2Intr info) {
            Edge2D[] edges = convexPolygon.edges;
            int num = edges.Length;
            FLOAT num2 = FLOAT.NegativeInfinity;
            FLOAT num3 = FLOAT.PositiveInfinity;
            FVector2 direction = line.direction;
            for (int i = 0; i < num; i++) {
                FVector2 vector = edges[i].point1 - edges[i].point0;
                FVector2 vector2 = new FVector2(vector.y, 0f - vector.x);
                FLOAT num4 = vector2.Dot(edges[i].point0 - line.origin);
                FLOAT num5 = vector2.Dot(direction);
                if (FMath.Abs(num5) < 1E-05f) {
                    if (num4 < 0f) {
                        info = default(Line2ConvexPolygon2Intr);
                        return false;
                    }

                    continue;
                }

                FLOAT num6 = num4 / num5;
                if (num5 < 0f) {
                    if (num6 > num2) {
                        num2 = num6;
                        if (num2 > num3) {
                            info = default(Line2ConvexPolygon2Intr);
                            return false;
                        }
                    }
                }
                else if (num6 < num3) {
                    num3 = num6;
                    if (num3 < num2) {
                        info = default(Line2ConvexPolygon2Intr);
                        return false;
                    }
                }
            }

            if (num3 - num2 > 1E-05f) {
                info.IntersectionType = IntersectionTypes.Segment;
                info.Quantity = 2;
                info.Point0 = line.origin + num2 * direction;
                info.Point1 = line.origin + num3 * direction;
                info.Parameter0 = num2;
                info.Parameter1 = num3;
            }
            else {
                info.IntersectionType = IntersectionTypes.Point;
                info.Quantity = 1;
                info.Point0 = line.origin + num2 * direction;
                info.Point1 = FVector2.Zero;
                info.Parameter0 = num2;
                info.Parameter1 = 0f;
            }

            return true;
        }

        private static IntersectionTypes Classify(ref Line2D line0, ref Line2D line1, out FLOAT s0) {
            FVector2 vector = line1.origin - line0.origin;
            s0 = 0f;
            FLOAT num = line0.direction.DotPerpendicular(line1.direction);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = vector.DotPerpendicular(line1.direction);
                s0 = num2 / num;
                return IntersectionTypes.Point;
            }

            vector.Normalize();
            FLOAT f = vector.DotPerpendicular(line1.direction);
            if (FMath.Abs(f) <= _dotThreshold) {
                return IntersectionTypes.Line;
            }

            return IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether two lines intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Line),
        /// or false if lines do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestLine2Line2(ref Line2D line0, ref Line2D line1, out IntersectionTypes intersectionType) {
            intersectionType = Classify(ref line0, ref line1, out var _);
            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether two lines intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Line),
        /// or false if lines do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestLine2Line2(ref Line2D line0, ref Line2D line1) {
            return TestLine2Line2(ref line0, ref line1, out _);
        }

        /// <summary>
        /// Tests whether two lines intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Line),
        /// or false if lines do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool FindLine2Line2(ref Line2D line0, ref Line2D line1, out Line2Line2Intr info) {
            info.IntersectionType = Classify(ref line0, ref line1, out var s);
            if (info.IntersectionType == IntersectionTypes.Point) {
                info.Point = line0.origin + s * line0.direction;
                info.Parameter = s;
            }
            else {
                info.Point = FVector2.Zero;
                info.Parameter = 0f;
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        private static IntersectionTypes Classify(ref Line2D line, ref Ray2D ray, out FLOAT s0, out FLOAT s1) {
            FVector2 vector = ray.origin - line.origin;
            s0 = (s1 = 0f);
            FLOAT num = line.direction.DotPerpendicular(ray.direction);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = 1f / num;
                FLOAT num3 = vector.DotPerpendicular(ray.direction);
                s0 = num3 * num2;
                FLOAT num4 = vector.DotPerpendicular(line.direction);
                s1 = num4 * num2;
                return IntersectionTypes.Point;
            }

            vector.Normalize();
            FLOAT f = vector.DotPerpendicular(ray.direction);
            if (FMath.Abs(f) <= _dotThreshold) {
                return IntersectionTypes.Ray;
            }

            return IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether line and ray intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Ray),
        /// or false if line and ray do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestLine2Ray2(ref Line2D line, ref Ray2D ray, out IntersectionTypes intersectionType) {
            intersectionType = Classify(ref line, ref ray, out var _, out var s2);
            if (intersectionType == IntersectionTypes.Point && !(s2 >= 0f - _intervalThreshold)) {
                intersectionType = IntersectionTypes.Empty;
            }

            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether line and ray intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Ray),
        /// or false if line and ray do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestLine2Ray2(ref Line2D line, ref Ray2D ray) {
            return TestLine2Ray2(ref line, ref ray, out _);
        }

        /// <summary>
        /// Tests whether line and ray intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Ray),
        /// or false if line and ray do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool FindLine2Ray2(ref Line2D line, ref Ray2D ray, out Line2Ray2Intr info) {
            info.IntersectionType = Classify(ref line, ref ray, out var s, out var s2);
            info.Point = FVector2.Zero;
            info.Parameter = 0f;
            if (info.IntersectionType == IntersectionTypes.Point) {
                if (s2 >= 0f - _intervalThreshold) {
                    info.Point = line.origin + s * line.direction;
                    info.Parameter = s;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Empty;
                }
            }
            else if (info.IntersectionType == IntersectionTypes.Ray) {
                info.Point = ray.origin;
                info.Parameter = s;
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        private static IntersectionTypes Classify(ref Segment2D segment, ref Line2D line, out FLOAT s0, out FLOAT s1) {
            FVector2 vector = segment.center - line.origin;
            s0 = (s1 = 0f);
            FLOAT num = line.direction.DotPerpendicular(segment.direction);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = 1f / num;
                FLOAT num3 = vector.DotPerpendicular(segment.direction);
                s0 = num3 * num2;
                FLOAT num4 = vector.DotPerpendicular(line.direction);
                s1 = num4 * num2;
                return IntersectionTypes.Point;
            }

            vector.Normalize();
            FLOAT f = vector.DotPerpendicular(segment.direction);
            if (FMath.Abs(f) <= _dotThreshold) {
                return IntersectionTypes.Segment;
            }

            return IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether line and segment intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if line and segment do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestLine2Segment2(ref Line2D line, ref Segment2D segment,
            out IntersectionTypes intersectionType) {
            intersectionType = Classify(ref segment, ref line, out var _, out var s2);
            if (intersectionType == IntersectionTypes.Point && !(FMath.Abs(s2) <= segment.extent)) {
                intersectionType = IntersectionTypes.Empty;
            }

            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether line and segment intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if line and segment do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestLine2Segment2(ref Line2D line, ref Segment2D segment) {
            return TestLine2Segment2(ref line, ref segment, out _);
        }

        /// <summary>
        /// Tests whether line and segment intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if line and segment do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool FindLine2Segment2(ref Line2D line, ref Segment2D segment, out Line2Segment2Intr info) {
            info.IntersectionType = Classify(ref segment, ref line, out var s, out var s2);
            info.Point = FVector2.Zero;
            info.Parameter = 0f;
            if (info.IntersectionType == IntersectionTypes.Point) {
                if (FMath.Abs(s2) <= segment.extent + _intervalThreshold) {
                    info.Point = line.origin + s * line.direction;
                    info.Parameter = s;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Empty;
                }
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        private static void TriangleLineRelations(ref FVector2 origin, ref FVector2 direction,
            ref Triangle2D triangle, out FLOAT dist0, out FLOAT dist1, out FLOAT dist2, out int sign0, out int sign1,
            out int sign2, out int positive, out int negative, out int zero) {
            positive = 0;
            negative = 0;
            zero = 0;
            FVector2 vector = triangle.v0 - origin;
            dist0 = vector.DotPerpendicular(direction);
            if (dist0 > 1E-05f) {
                sign0 = 1;
                positive++;
            }
            else if (dist0 < -1E-05f) {
                sign0 = -1;
                negative++;
            }
            else {
                dist0 = 0f;
                sign0 = 0;
                zero++;
            }

            vector = triangle.v1 - origin;
            dist1 = vector.DotPerpendicular(direction);
            if (dist1 > 1E-05f) {
                sign1 = 1;
                positive++;
            }
            else if (dist1 < -1E-05f) {
                sign1 = -1;
                negative++;
            }
            else {
                dist1 = 0f;
                sign1 = 0;
                zero++;
            }

            vector = triangle.v2 - origin;
            dist2 = vector.DotPerpendicular(direction);
            if (dist2 > 1E-05f) {
                sign2 = 1;
                positive++;
            }
            else if (dist2 < -1E-05f) {
                sign2 = -1;
                negative++;
            }
            else {
                dist2 = 0f;
                sign2 = 0;
                zero++;
            }
        }

        private static bool GetInterval(ref FVector2 origin, ref FVector2 direction, ref Triangle2D triangle,
            FLOAT dist0, FLOAT dist1, FLOAT dist2, int sign0, int sign1, int sign2, out FLOAT param0, out FLOAT param1) {
            FVector2 value = triangle.v0 - origin;
            FLOAT num = direction.Dot(value);
            value = triangle.v1 - origin;
            FLOAT num2 = direction.Dot(value);
            value = triangle.v2 - origin;
            FLOAT num3 = direction.Dot(value);
            param0 = 0f;
            param1 = 0f;
            int num4 = 0;
            if (sign2 * sign0 < 0) {
                param0 = (dist2 * num - dist0 * num3) / (dist2 - dist0);
                num4++;
            }

            if (sign0 * sign1 < 0) {
                if (num4 == 0) {
                    param0 = (dist0 * num2 - dist1 * num) / (dist0 - dist1);
                }
                else {
                    param1 = (dist0 * num2 - dist1 * num) / (dist0 - dist1);
                }

                num4++;
            }

            if (sign1 * sign2 < 0) {
                if (num4 > 1) {
                    return true;
                }

                if (num4 == 0) {
                    param0 = (dist1 * num3 - dist2 * num2) / (dist1 - dist2);
                }
                else {
                    param1 = (dist1 * num3 - dist2 * num2) / (dist1 - dist2);
                }

                num4++;
            }

            if (num4 < 2) {
                if (sign0 == 0) {
                    if (num4 == 0) {
                        param0 = num;
                    }
                    else {
                        param1 = num;
                    }

                    num4++;
                }

                if (sign1 == 0) {
                    if (num4 > 1) {
                        return true;
                    }

                    if (num4 == 0) {
                        param0 = num2;
                    }
                    else {
                        param1 = num2;
                    }

                    num4++;
                }

                if (sign2 == 0) {
                    if (num4 > 1) {
                        return true;
                    }

                    if (num4 == 0) {
                        param0 = num3;
                    }
                    else {
                        param1 = num3;
                    }

                    num4++;
                }
            }

            if (num4 < 1) {
                return true;
            }

            if (num4 == 2) {
                if (param0 > param1) {
                    (param0, param1) = (param1, param0);
                }
            }
            else {
                param1 = param0;
            }

            return false;
        }

        /// <summary>
        /// Tests whether line and triangle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine2Triangle2(ref Line2D line, ref Triangle2D triangle,
            out IntersectionTypes intersectionType) {
            TriangleLineRelations(ref line.origin, ref line.direction, ref triangle, out var dist, out var dist2,
                out var dist3, out var sign, out var sign2, out var sign3, out var positive, out var negative,
                out var _);
            if (positive == 3 || negative == 3) {
                intersectionType = IntersectionTypes.Empty;
            }
            else if (GetInterval(ref line.origin, ref line.direction, ref triangle, dist, dist2, dist3, sign, sign2,
                         sign3, out var param, out var param2)) {
                intersectionType = IntersectionTypes.Empty;
            }
            else {
                switch (FindSegment1Segment1(param, param2, FLOAT.NegativeInfinity, FLOAT.PositiveInfinity, out _,
                            out _)) {
                    case 2:
                        intersectionType = IntersectionTypes.Segment;
                        break;
                    case 1:
                        intersectionType = IntersectionTypes.Point;
                        break;
                    default:
                        intersectionType = IntersectionTypes.Empty;
                        break;
                }
            }

            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether line and triangle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine2Triangle2(ref Line2D line, ref Triangle2D triangle) {
            return TestLine2Triangle2(ref line, ref triangle, out _);
        }

        /// <summary>
        /// Tests whether line and triangle intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine2Triangle2(ref Line2D line, ref Triangle2D triangle, out Line2Triangle2Intr info) {
            TriangleLineRelations(ref line.origin, ref line.direction, ref triangle, out var dist, out var dist2,
                out var dist3, out var sign, out var sign2, out var sign3, out var positive, out var negative,
                out var _);
            FLOAT param;
            FLOAT param2;
            if (positive == 3 || negative == 3) {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Quantity = 0;
                info.Point0 = FVector2.Zero;
                info.Point1 = FVector2.Zero;
            }
            else if (GetInterval(ref line.origin, ref line.direction, ref triangle, dist, dist2, dist3, sign, sign2,
                         sign3, out param, out param2)) {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Quantity = 0;
                info.Point0 = FVector2.Zero;
                info.Point1 = FVector2.Zero;
            }
            else {
                info.Quantity = FindSegment1Segment1(param, param2, FLOAT.NegativeInfinity, FLOAT.PositiveInfinity,
                    out var w, out var w2);
                if (info.Quantity == 2) {
                    info.IntersectionType = IntersectionTypes.Segment;
                    info.Point0 = line.origin + w * line.direction;
                    info.Point1 = line.origin + w2 * line.direction;
                }
                else if (info.Quantity == 1) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point0 = line.origin + w * line.direction;
                    info.Point1 = FVector2.Zero;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Empty;
                    info.Point0 = FVector2.Zero;
                    info.Point1 = FVector2.Zero;
                }
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a ray intersects an axis aligned box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay2AABB2(ref Ray2D ray, ref AABB2D box) {
            box.CalcCenterExtents(out var center, out var extents);
            FVector2 value = ray.origin - center;
            FLOAT x = ray.direction.x;
            FLOAT x2 = value.x;
            FLOAT num = FMath.Abs(x2);
            if (num > extents.x && x2 * x >= 0f) {
                return false;
            }

            FLOAT y = ray.direction.y;
            FLOAT y2 = value.y;
            FLOAT num2 = FMath.Abs(y2);
            if (num2 > extents.y && y2 * y >= 0f) {
                return false;
            }

            FVector2 vector = ray.direction.Perpendicular();
            FLOAT num3 = FMath.Abs(vector.Dot(value));
            FLOAT num4 = FMath.Abs(vector.x);
            FLOAT num5 = FMath.Abs(vector.y);
            FLOAT num6 = extents.x * num4 + extents.y * num5;
            return num3 <= num6;
        }

        /// <summary>
        /// Tests if a ray intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay2AABB2(ref Ray2D ray, ref AABB2D box, out Ray2AAB2Intr info) {
            return DoClipping(0f, FLOAT.PositiveInfinity, ref ray.origin, ref ray.direction, ref box, solid: true,
                out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests whether ray and box intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay2Box2(ref Ray2D ray, ref Box2D box) {
            FVector2 vector = ray.origin - box.center;
            FLOAT num = ray.direction.Dot(box.axis0);
            FLOAT num2 = vector.Dot(box.axis0);
            FLOAT num3 = FMath.Abs(num2);
            if (num3 > box.extents.x && num2 * num >= 0f) {
                return false;
            }

            FLOAT num4 = ray.direction.Dot(box.axis1);
            FLOAT num5 = vector.Dot(box.axis1);
            FLOAT num6 = FMath.Abs(num5);
            if (num6 > box.extents.y && num5 * num4 >= 0f) {
                return false;
            }

            FVector2 vector2 = ray.direction.Perpendicular();
            FLOAT num7 = FMath.Abs(vector2.Dot(vector));
            FLOAT num8 = FMath.Abs(vector2.Dot(box.axis0));
            FLOAT num9 = FMath.Abs(vector2.Dot(box.axis1));
            FLOAT num10 = box.extents.x * num8 + box.extents.y * num9;
            return num7 <= num10;
        }

        /// <summary>
        /// Tests whether ray and box intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay2Box2(ref Ray2D ray, ref Box2D box, out Ray2Box2Intr info) {
            return DoClipping(0f, FLOAT.PositiveInfinity, ref ray.origin, ref ray.direction, ref box, solid: true,
                out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests whether ray and circle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay2Circle2(ref Ray2D ray, ref Circle2D circle) {
            FVector2 rhs = ray.origin - circle.center;
            FLOAT num = rhs.sqrMagnitude - circle.radius * circle.radius;
            if (num <= 1E-05f) {
                return true;
            }

            FVector2.Dot(ref ray.direction, ref rhs, out var num2);
            if (num2 >= 0f) {
                return false;
            }

            FLOAT num3 = num2 * num2 - num;
            return num3 >= -1E-05f;
        }

        /// <summary>
        /// Tests whether ray and circle intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay2Circle2(ref Ray2D ray, ref Circle2D circle, out Ray2Circle2Intr info) {
            int rootCount;
            FLOAT t;
            FLOAT t2;
            bool flag = Find(ref ray.origin, ref ray.direction, ref circle.center, circle.radius, out rootCount, out t,
                out t2);
            info.Point0 = (info.Point1 = FVector2.Zero);
            if (flag) {
                if (rootCount == 1) {
                    if (t < 0f) {
                        info.IntersectionType = IntersectionTypes.Empty;
                    }
                    else {
                        info.IntersectionType = IntersectionTypes.Point;
                        info.Point0 = ray.origin + t * ray.direction;
                    }
                }
                else if (t2 < 0f) {
                    info.IntersectionType = IntersectionTypes.Empty;
                }
                else if (t < 0f) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point0 = ray.origin + t2 * ray.direction;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Segment;
                    info.Point0 = ray.origin + t * ray.direction;
                    info.Point1 = ray.origin + t2 * ray.direction;
                }
            }
            else {
                info.IntersectionType = IntersectionTypes.Empty;
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a ray intersects a convex polygon. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay2ConvexPolygon2(ref Ray2D ray, Polygon2D convexPolygon) {
            Edge2D[] edges = convexPolygon.edges;
            int num = edges.Length;
            for (int i = 0; i < num; i++) {
                Segment2D segment = new Segment2D(ref edges[i].point0, ref edges[i].point1);
                if (TestRay2Segment2(ref ray, ref segment)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a convex ccw ordered polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay2ConvexPolygon2(ref Ray2D ray, Polygon2D convexPolygon,
            out Ray2ConvexPolygon2Intr info) {
            Edge2D[] edges = convexPolygon.edges;
            int num = edges.Length;
            FLOAT num2 = 0f;
            FLOAT num3 = FLOAT.PositiveInfinity;
            FVector2 direction = ray.direction;
            for (int i = 0; i < num; i++) {
                FVector2 vector = edges[i].point1 - edges[i].point0;
                FVector2 vector2 = new FVector2(vector.y, 0f - vector.x);
                FLOAT num4 = vector2.Dot(edges[i].point0 - ray.origin);
                FLOAT num5 = vector2.Dot(direction);
                if (FMath.Abs(num5) < 1E-05f) {
                    if (num4 < 0f) {
                        info = default(Ray2ConvexPolygon2Intr);
                        return false;
                    }

                    continue;
                }

                FLOAT num6 = num4 / num5;
                if (num5 < 0f) {
                    if (num6 > num2) {
                        num2 = num6;
                        if (num2 > num3) {
                            info = default(Ray2ConvexPolygon2Intr);
                            return false;
                        }
                    }
                }
                else if (num6 < num3) {
                    num3 = num6;
                    if (num3 < num2) {
                        info = default(Ray2ConvexPolygon2Intr);
                        return false;
                    }
                }
            }

            if (num3 - num2 > 1E-05f) {
                info.IntersectionType = IntersectionTypes.Segment;
                info.Quantity = 2;
                info.Point0 = ray.origin + num2 * direction;
                info.Point1 = ray.origin + num3 * direction;
                info.Parameter0 = num2;
                info.Parameter1 = num3;
            }
            else {
                info.IntersectionType = IntersectionTypes.Point;
                info.Quantity = 1;
                info.Point0 = ray.origin + num2 * direction;
                info.Point1 = FVector2.Zero;
                info.Parameter0 = num2;
                info.Parameter1 = 0f;
            }

            return true;
        }

        /// <summary>
        /// Tests if a ray intersects a polygon. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay2Polygon2(ref Ray2D ray, Polygon2D polygon) {
            Edge2D[] edges = polygon.edges;
            int num = edges.Length;
            for (int i = 0; i < num; i++) {
                Segment2D segment = new Segment2D(ref edges[i].point0, ref edges[i].point1);
                if (TestRay2Segment2(ref ray, ref segment)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a segment array. Returns true if intersection occurs false otherwise.
        /// Using this method allows to pass non-closed polyline instead of a polygon. Also if you
        /// have static polygon which is queried often, it is better to convert polygon to Segment2 array
        /// once and then call this method. Overload which accepts a polygon will convert edges to Segment2
        /// every time, while this overload simply accepts Segment2 array and avoids this overhead.
        /// </summary>
        public static bool TestRay2Polygon2(ref Ray2D ray, Segment2D[] segments) {
            int num = segments.Length;
            for (int i = 0; i < num; i++) {
                if (TestRay2Segment2(ref ray, ref segments[i])) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay2Polygon2(ref Ray2D ray, Polygon2D polygon, out Ray2Polygon2Intr info) {
            Edge2D[] edges = polygon.edges;
            int num = edges.Length;
            Ray2Segment2Intr ray2Segment2Intr = default(Ray2Segment2Intr);
            FLOAT num2 = FLOAT.PositiveInfinity;
            for (int i = 0; i < num; i++) {
                Segment2D segment = new Segment2D(edges[i].point0, edges[i].point1);
                if (FindRay2Segment2(ref ray, ref segment, out var info2) && info2.Parameter0 < num2) {
                    if (info2.IntersectionType == IntersectionTypes.Segment) {
                        num2 = info2.Parameter0;
                        ray2Segment2Intr = info2;
                    }
                    else if (num2 - info2.Parameter0 > 1E-05f) {
                        num2 = info2.Parameter0;
                        ray2Segment2Intr = info2;
                    }
                }
            }

            if (num2 != FLOAT.PositiveInfinity) {
                info.IntersectionType = ray2Segment2Intr.IntersectionType;
                info.Point0 = ray2Segment2Intr.Point0;
                info.Point1 = ray2Segment2Intr.Point1;
                info.Parameter0 = ray2Segment2Intr.Parameter0;
                info.Parameter1 = ray2Segment2Intr.Parameter1;
                return true;
            }

            info = default(Ray2Polygon2Intr);
            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// Using this method allows to pass non-closed polyline instead of a polygon. Also if you
        /// have static polygon which is queried often, it is better to convert polygon to Segment2 array
        /// once and then call this method. Overload which accepts a polygon will convert edges to Segment2
        /// every time, while this overload simply accepts Segment2 array and avoids this overhead.
        /// </summary>
        public static bool FindRay2Polygon2(ref Ray2D ray, Segment2D[] segments, out Ray2Polygon2Intr info) {
            int num = segments.Length;
            Ray2Segment2Intr ray2Segment2Intr = default(Ray2Segment2Intr);
            FLOAT num2 = FLOAT.PositiveInfinity;
            for (int i = 0; i < num; i++) {
                if (FindRay2Segment2(ref ray, ref segments[i], out var info2) && info2.Parameter0 < num2) {
                    if (info2.IntersectionType == IntersectionTypes.Segment) {
                        num2 = info2.Parameter0;
                        ray2Segment2Intr = info2;
                    }
                    else if (num2 - info2.Parameter0 > 1E-05f) {
                        num2 = info2.Parameter0;
                        ray2Segment2Intr = info2;
                    }
                }
            }

            if (num2 != FLOAT.PositiveInfinity) {
                info.IntersectionType = ray2Segment2Intr.IntersectionType;
                info.Point0 = ray2Segment2Intr.Point0;
                info.Point1 = ray2Segment2Intr.Point1;
                info.Parameter0 = ray2Segment2Intr.Parameter0;
                info.Parameter1 = ray2Segment2Intr.Parameter1;
                return true;
            }

            info = default(Ray2Polygon2Intr);
            return false;
        }

        private static IntersectionTypes Classify(ref Ray2D ray0, ref Ray2D ray1, out FLOAT s0, out FLOAT s1) {
            FVector2 vector = ray1.origin - ray0.origin;
            s0 = (s1 = 0f);
            FLOAT num = ray0.direction.DotPerpendicular(ray1.direction);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = 1f / num;
                FLOAT num3 = vector.DotPerpendicular(ray1.direction);
                s0 = num3 * num2;
                FLOAT num4 = vector.DotPerpendicular(ray0.direction);
                s1 = num4 * num2;
                return IntersectionTypes.Point;
            }

            vector.Normalize();
            FLOAT f = vector.DotPerpendicular(ray1.direction);
            if (FMath.Abs(f) <= _dotThreshold) {
                var dir = ray1.origin - ray0.origin;
                FVector2.Dot(ref dir, ref ray0.direction, out s0);
                return IntersectionTypes.Ray;
            }

            return IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether two rays intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Ray),
        /// or false if rays do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestRay2Ray2(ref Ray2D ray0, ref Ray2D ray1, out IntersectionTypes intersectionType) {
            intersectionType = Classify(ref ray0, ref ray1, out var s, out var s2);
            if (intersectionType == IntersectionTypes.Point) {
                if (!(s >= 0f - _intervalThreshold) || !(s2 >= 0f - _intervalThreshold)) {
                    intersectionType = IntersectionTypes.Empty;
                }
            }
            else if (intersectionType == IntersectionTypes.Ray) {
                if (FMath.Abs(s) == 0f) {
                    FVector2.Dot(ref ray0.direction, ref ray1.direction, out var num);
                    if (num < 0f) {
                        intersectionType = IntersectionTypes.Point;
                    }
                }
                else if (s < 0f) {
                    FVector2.Dot(ref ray0.direction, ref ray1.direction, out var num2);
                    if (num2 < 0f) {
                        intersectionType = IntersectionTypes.Empty;
                    }
                }
            }

            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether two rays intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Ray),
        /// or false if rays do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestRay2Ray2(ref Ray2D ray0, ref Ray2D ray1) {
            return TestRay2Ray2(ref ray0, ref ray1, out _);
        }

        /// <summary>
        /// Tests whether two rays intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Ray),
        /// or false if rays do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool FindRay2Ray2(ref Ray2D ray0, ref Ray2D ray1, out Ray2Ray2Intr info) {
            info.IntersectionType = Classify(ref ray0, ref ray1, out var s, out var s2);
            info.Point = FVector2.Zero;
            info.Parameter = 0f;
            if (info.IntersectionType == IntersectionTypes.Point) {
                if (s >= 0f - _intervalThreshold && s2 >= 0f - _intervalThreshold) {
                    if (s < 0f) {
                        s = 0f;
                    }

                    info.Point = ray0.origin + s * ray0.direction;
                    info.Parameter = s;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Empty;
                }
            }
            else if (info.IntersectionType == IntersectionTypes.Ray) {
                if (FMath.Abs(s) == 0f) {
                    FVector2.Dot(ref ray0.direction, ref ray1.direction, out var num);
                    if (num < 0f) {
                        info.IntersectionType = IntersectionTypes.Point;
                    }

                    info.Point = ray1.origin;
                }
                else if (s < 0f) {
                    FVector2.Dot(ref ray0.direction, ref ray1.direction, out var num2);
                    if (num2 < 0f) {
                        info.IntersectionType = IntersectionTypes.Empty;
                    }
                    else {
                        info.Point = ray1.origin;
                        info.Parameter = s;
                    }
                }
                else {
                    info.Point = ray1.origin;
                    info.Parameter = s;
                }
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        private static IntersectionTypes Classify(ref Ray2D ray, ref Segment2D segment, out FLOAT s0, out FLOAT s1) {
            FVector2 vector = segment.center - ray.origin;
            s0 = (s1 = 0f);
            FLOAT num = ray.direction.DotPerpendicular(segment.direction);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = 1f / num;
                FLOAT num3 = vector.DotPerpendicular(segment.direction);
                s0 = num3 * num2;
                FLOAT num4 = vector.DotPerpendicular(ray.direction);
                s1 = num4 * num2;
                return IntersectionTypes.Point;
            }

            vector.Normalize();
            FLOAT f = vector.DotPerpendicular(segment.direction);
            if (FMath.Abs(f) <= _dotThreshold) {
                var dir1 = segment.p0 - ray.origin;
                var dir2 = segment.p1 - ray.origin;
                FVector2.Dot(ref dir1, ref ray.direction, out s0);
                FVector2.Dot(ref dir2, ref ray.direction, out s1);
                if (s0 > s1) {
                    FLOAT num5 = s0;
                    s0 = s1;
                    s1 = num5;
                }

                return IntersectionTypes.Segment;
            }

            return IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether ray and segment intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if ray and segment do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestRay2Segment2(ref Ray2D ray, ref Segment2D segment,
            out IntersectionTypes intersectionType) {
            intersectionType = Classify(ref ray, ref segment, out var s, out var s2);
            if (intersectionType == IntersectionTypes.Point) {
                if (!(s >= 0f - _intervalThreshold) || !(FMath.Abs(s2) <= segment.extent + _intervalThreshold)) {
                    intersectionType = IntersectionTypes.Empty;
                }
            }
            else if (intersectionType == IntersectionTypes.Segment) {
                switch (FindSegment1Segment1(0f, FLOAT.PositiveInfinity, s, s2, out _, out _)) {
                    case 1:
                        intersectionType = IntersectionTypes.Point;
                        break;
                    case 0:
                        intersectionType = IntersectionTypes.Empty;
                        break;
                }
            }

            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether ray and segment intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if ray and segment do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestRay2Segment2(ref Ray2D ray, ref Segment2D segment) {
            return TestRay2Segment2(ref ray, ref segment, out _);
        }

        /// <summary>
        /// Tests whether ray and segment intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if ray and segment do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool FindRay2Segment2(ref Ray2D ray, ref Segment2D segment, out Ray2Segment2Intr info) {
            info.IntersectionType = Classify(ref ray, ref segment, out var s, out var s2);
            info.Point0 = (info.Point1 = FVector2.Zero);
            info.Parameter0 = (info.Parameter1 = 0f);
            if (info.IntersectionType == IntersectionTypes.Point) {
                if (s >= 0f - _intervalThreshold && FMath.Abs(s2) <= segment.extent) {
                    info.Point0 = ray.origin + s * ray.direction;
                    info.Parameter0 = s;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Empty;
                }
            }
            else if (info.IntersectionType == IntersectionTypes.Segment) {
                FLOAT w;
                FLOAT w2;
                switch (FindSegment1Segment1(0f, FLOAT.PositiveInfinity, s, s2, out w, out w2)) {
                    case 2:
                        info.Point0 = ray.origin + w * ray.direction;
                        info.Point1 = ray.origin + w2 * ray.direction;
                        info.Parameter0 = w;
                        info.Parameter1 = w2;
                        break;
                    case 1:
                        info.IntersectionType = IntersectionTypes.Point;
                        info.Point0 = ray.origin + w * ray.direction;
                        info.Parameter0 = w;
                        break;
                    default:
                        info.IntersectionType = IntersectionTypes.Empty;
                        break;
                }
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether ray and triangle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay2Triangle2(ref Ray2D ray, ref Triangle2D triangle,
            out IntersectionTypes intersectionType) {
            TriangleLineRelations(ref ray.origin, ref ray.direction, ref triangle, out var dist, out var dist2,
                out var dist3, out var sign, out var sign2, out var sign3, out var positive, out var negative,
                out var _);
            if (positive == 3 || negative == 3) {
                intersectionType = IntersectionTypes.Empty;
            }
            else if (GetInterval(ref ray.origin, ref ray.direction, ref triangle, dist, dist2, dist3, sign, sign2,
                         sign3, out var param, out var param2)) {
                intersectionType = IntersectionTypes.Empty;
            }
            else {
                switch (FindSegment1Segment1(param, param2, 0f, FLOAT.PositiveInfinity, out _, out _)) {
                    case 2:
                        intersectionType = IntersectionTypes.Segment;
                        break;
                    case 1:
                        intersectionType = IntersectionTypes.Point;
                        break;
                    default:
                        intersectionType = IntersectionTypes.Empty;
                        break;
                }
            }

            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether ray and triangle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay2Triangle2(ref Ray2D ray, ref Triangle2D triangle) {
            return TestRay2Triangle2(ref ray, ref triangle, out _);
        }

        /// <summary>
        /// Tests whether ray and triangle intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay2Triangle2(ref Ray2D ray, ref Triangle2D triangle, out Ray2Triangle2Intr info) {
            TriangleLineRelations(ref ray.origin, ref ray.direction, ref triangle, out var dist, out var dist2,
                out var dist3, out var sign, out var sign2, out var sign3, out var positive, out var negative,
                out var _);
            FLOAT param;
            FLOAT param2;
            if (positive == 3 || negative == 3) {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Quantity = 0;
                info.Point0 = FVector2.Zero;
                info.Point1 = FVector2.Zero;
            }
            else if (GetInterval(ref ray.origin, ref ray.direction, ref triangle, dist, dist2, dist3, sign, sign2,
                         sign3, out param, out param2)) {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Quantity = 0;
                info.Point0 = FVector2.Zero;
                info.Point1 = FVector2.Zero;
            }
            else {
                info.Quantity = FindSegment1Segment1(param, param2, 0f, FLOAT.PositiveInfinity, out var w, out var w2);
                if (info.Quantity == 2) {
                    info.IntersectionType = IntersectionTypes.Segment;
                    info.Point0 = ray.origin + w * ray.direction;
                    info.Point1 = ray.origin + w2 * ray.direction;
                }
                else if (info.Quantity == 1) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point0 = ray.origin + w * ray.direction;
                    info.Point1 = FVector2.Zero;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Empty;
                    info.Point0 = FVector2.Zero;
                    info.Point1 = FVector2.Zero;
                }
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a segment intersects an axis aligned box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment2AABB2(ref Segment2D segment, ref AABB2D box) {
            box.CalcCenterExtents(out var center, out var extents);
            FVector2 value = segment.center - center;
            FLOAT num = FMath.Abs(segment.direction.x);
            FLOAT num2 = FMath.Abs(value.x);
            FLOAT num3 = extents.x + segment.extent * num;
            if (num2 > num3) {
                return false;
            }

            FLOAT num4 = FMath.Abs(segment.direction.y);
            FLOAT num5 = FMath.Abs(value.y);
            num3 = extents.y + segment.extent * num4;
            if (num5 > num3) {
                return false;
            }

            FVector2 vector = segment.direction.Perpendicular();
            FLOAT num6 = FMath.Abs(vector.Dot(value));
            FLOAT num7 = FMath.Abs(vector.x);
            FLOAT num8 = FMath.Abs(vector.y);
            num3 = extents.x * num7 + extents.y * num8;
            return num6 <= num3;
        }

        /// <summary>
        /// Tests if a segment intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment2AABB2(ref Segment2D segment, ref AABB2D box, out Segment2AAB2Intr info) {
            return DoClipping(0f - segment.extent, segment.extent, ref segment.center, ref segment.direction, ref box,
                solid: true, out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests whether segment and box intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment2Box2(ref Segment2D segment, ref Box2D box) {
            FVector2 vector = segment.center - box.center;
            FLOAT num = FMath.Abs(segment.direction.Dot(box.axis0));
            FLOAT num2 = FMath.Abs(vector.Dot(box.axis0));
            FLOAT num3 = box.extents.x + segment.extent * num;
            if (num2 > num3) {
                return false;
            }

            FLOAT num4 = FMath.Abs(segment.direction.Dot(box.axis1));
            FLOAT num5 = FMath.Abs(vector.Dot(box.axis1));
            num3 = box.extents.y + segment.extent * num4;
            if (num5 > num3) {
                return false;
            }

            FVector2 vector2 = segment.direction.Perpendicular();
            FLOAT num6 = FMath.Abs(vector2.Dot(vector));
            FLOAT num7 = FMath.Abs(vector2.Dot(box.axis0));
            FLOAT num8 = FMath.Abs(vector2.Dot(box.axis1));
            num3 = box.extents.x * num7 + box.extents.y * num8;
            return num6 <= num3;
        }

        /// <summary>
        /// Tests whether segment and box intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment2Box2(ref Segment2D segment, ref Box2D box, out Segment2Box2Intr info) {
            return DoClipping(0f - segment.extent, segment.extent, ref segment.center, ref segment.direction, ref box,
                solid: true, out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests whether segment and circle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment2Circle2(ref Segment2D segment, ref Circle2D circle) {
            FVector2 rhs = segment.center - circle.center;
            FLOAT num = rhs.sqrMagnitude - circle.radius * circle.radius;
            if (num <= 1E-05f) {
                return true;
            }

            FVector2.Dot(ref segment.direction, ref rhs, out var num2);
            FLOAT num3 = num2 * num2 - num;
            if (num3 < -1E-05f) {
                return false;
            }

            FLOAT num4 = FMath.Abs(num2);
            FLOAT num5 = segment.extent * (segment.extent - 2f * num4) + num;
            if (!(num5 <= 1E-05f)) {
                return num4 <= segment.extent;
            }

            return true;
        }

        /// <summary>
        /// Tests whether segment and circle intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment2Circle2(ref Segment2D segment, ref Circle2D circle,
            out Segment2Circle2Intr info) {
            int rootCount;
            FLOAT t;
            FLOAT t2;
            bool flag = Find(ref segment.center, ref segment.direction, ref circle.center, circle.radius, out rootCount,
                out t, out t2);
            info.Point0 = (info.Point1 = FVector2.Zero);
            if (flag) {
                if (rootCount == 1) {
                    if (FMath.Abs(t) > segment.extent + 1E-05f) {
                        info.IntersectionType = IntersectionTypes.Empty;
                    }
                    else {
                        info.IntersectionType = IntersectionTypes.Point;
                        info.Point0 = segment.center + t * segment.direction;
                    }
                }
                else {
                    FLOAT num = segment.extent + 1E-05f;
                    if (t2 < 0f - num || t > num) {
                        info.IntersectionType = IntersectionTypes.Empty;
                    }
                    else {
                        if (t2 <= num) {
                            if (t < 0f - num) {
                                rootCount = 1;
                                t = t2;
                            }
                        }
                        else {
                            rootCount = ((t >= 0f - num) ? 1 : 0);
                        }

                        switch (rootCount) {
                            default:
                                info.IntersectionType = IntersectionTypes.Empty;
                                break;
                            case 1:
                                info.IntersectionType = IntersectionTypes.Point;
                                info.Point0 = segment.center + t * segment.direction;
                                break;
                            case 2:
                                info.IntersectionType = IntersectionTypes.Segment;
                                info.Point0 = segment.center + t * segment.direction;
                                info.Point1 = segment.center + t2 * segment.direction;
                                break;
                        }
                    }
                }
            }
            else {
                info.IntersectionType = IntersectionTypes.Empty;
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a ray intersects a convex ccw ordered polygon. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment2ConvexPolygon2(ref Segment2D segment, Polygon2D convexPolygon) {
            return FindSegment2ConvexPolygon2(ref segment, convexPolygon, out _);
        }

        /// <summary>
        /// Tests if a ray intersects a convex ccw ordered polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment2ConvexPolygon2(ref Segment2D segment, Polygon2D convexPolygon,
            out Segment2ConvexPolygon2Intr info) {
            Edge2D[] edges = convexPolygon.edges;
            int num = edges.Length;
            FLOAT num2 = 0f;
            FLOAT num3 = 1f;
            FVector2 vector = segment.p1 - segment.p0;
            for (int i = 0; i < num; i++) {
                FVector2 vector2 = edges[i].point1 - edges[i].point0;
                FVector2 vector3 = new FVector2(vector2.y, 0f - vector2.x);
                FLOAT num4 = vector3.Dot(edges[i].point0 - segment.p0);
                FLOAT num5 = vector3.Dot(vector);
                if (FMath.Abs(num5) < 1E-05f) {
                    if (num4 < 0f) {
                        info = default(Segment2ConvexPolygon2Intr);
                        return false;
                    }

                    continue;
                }

                FLOAT num6 = num4 / num5;
                if (num5 < 0f) {
                    if (num6 > num2) {
                        num2 = num6;
                        if (num2 > num3) {
                            info = default(Segment2ConvexPolygon2Intr);
                            return false;
                        }
                    }
                }
                else if (num6 < num3) {
                    num3 = num6;
                    if (num3 < num2) {
                        info = default(Segment2ConvexPolygon2Intr);
                        return false;
                    }
                }
            }

            if (num3 - num2 > 1E-05f) {
                info.IntersectionType = IntersectionTypes.Segment;
                info.Quantity = 2;
                info.Point0 = segment.p0 + num2 * vector;
                info.Point1 = segment.p0 + num3 * vector;
                info.Parameter0 = num2;
                info.Parameter1 = num3;
            }
            else {
                info.IntersectionType = IntersectionTypes.Point;
                info.Quantity = 1;
                info.Point0 = segment.p0 + num2 * vector;
                info.Point1 = FVector2.Zero;
                info.Parameter0 = num2;
                info.Parameter1 = 0f;
            }

            return true;
        }

        private static IntersectionTypes Classify(ref Segment2D segment0, ref Segment2D segment1, out FLOAT s0,
            out FLOAT s1) {
            FVector2 vector = segment1.center - segment0.center;
            s0 = (s1 = 0f);
            FLOAT num = segment0.direction.DotPerpendicular(segment1.direction);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = 1f / num;
                FLOAT num3 = vector.DotPerpendicular(segment1.direction);
                s0 = num3 * num2;
                FLOAT num4 = vector.DotPerpendicular(segment0.direction);
                s1 = num4 * num2;
                return IntersectionTypes.Point;
            }

            vector.Normalize();
            FLOAT f = vector.DotPerpendicular(segment1.direction);
            if (FMath.Abs(f) <= _dotThreshold) {
                var dir1 = segment1.p0 - segment0.center;
                var dir2 = segment1.p1 - segment0.center;
                FVector2.Dot(ref dir1, ref segment0.direction, out s0);
                FVector2.Dot(ref dir2, ref segment0.direction, out s1);
                if (s0 > s1) {
                    (s0, s1) = (s1, s0);
                }

                return IntersectionTypes.Segment;
            }

            return IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether two segments intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if segments do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestSegment2Segment2(ref Segment2D segment0, ref Segment2D segment1,
            out IntersectionTypes intersectionType) {
            intersectionType = Classify(ref segment0, ref segment1, out var s, out var s2);
            if (intersectionType == IntersectionTypes.Point) {
                if (!(FMath.Abs(s) <= segment0.extent + _intervalThreshold) ||
                    !(FMath.Abs(s2) <= segment1.extent + _intervalThreshold)) {
                    intersectionType = IntersectionTypes.Empty;
                }
            }
            else if (intersectionType == IntersectionTypes.Segment) {
                switch (FindSegment1Segment1(0f - segment0.extent, segment0.extent, s, s2, out _, out _)) {
                    case 1:
                        intersectionType = IntersectionTypes.Point;
                        break;
                    case 0:
                        intersectionType = IntersectionTypes.Empty;
                        break;
                }
            }

            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether two segments intersect.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if segments do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool TestSegment2Segment2(ref Segment2D segment0, ref Segment2D segment1) {
            return TestSegment2Segment2(ref segment0, ref segment1, out _);
        }

        /// <summary>
        /// Tests whether two segments intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Segment),
        /// or false if segments do not intersect (IntersectionTypes.Empty).
        /// </summary>
        public static bool FindSegment2Segment2(ref Segment2D segment0, ref Segment2D segment1,
            out Segment2Segment2Intr info) {
            info.IntersectionType = Classify(ref segment0, ref segment1, out var s, out var s2);
            info.Point0 = (info.Point1 = FVector2.Zero);
            info.Parameter0 = (info.Parameter1 = 0f);
            if (info.IntersectionType == IntersectionTypes.Point) {
                if (FMath.Abs(s) <= segment0.extent + _intervalThreshold &&
                    FMath.Abs(s2) <= segment1.extent + _intervalThreshold) {
                    info.Point0 = segment0.center + s * segment0.direction;
                    info.Parameter0 = s / (segment0.extent * 2f) + 0.5f;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Empty;
                }
            }
            else if (info.IntersectionType == IntersectionTypes.Segment) {
                FLOAT w;
                FLOAT w2;
                switch (FindSegment1Segment1(0f - segment0.extent, segment0.extent, s, s2, out w, out w2)) {
                    case 2: {
                        info.Point0 = segment0.center + w * segment0.direction;
                        info.Point1 = segment0.center + w2 * segment0.direction;
                        FLOAT num2 = segment0.extent * 2f;
                        info.Parameter0 = w / num2 + 0.5f;
                        info.Parameter1 = w2 / num2 + 0.5f;
                        break;
                    }
                    case 1: {
                        info.IntersectionType = IntersectionTypes.Point;
                        info.Point0 = segment0.center + w * segment0.direction;
                        FLOAT num = segment0.extent * 2f;
                        info.Parameter0 = w / num + 0.5f;
                        break;
                    }
                    default:
                        info.IntersectionType = IntersectionTypes.Empty;
                        break;
                }
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether segment and triangle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment2Triangle2(ref Segment2D segment, ref Triangle2D triangle,
            out IntersectionTypes intersectionType) {
            TriangleLineRelations(ref segment.center, ref segment.direction, ref triangle, out var dist, out var dist2,
                out var dist3, out var sign, out var sign2, out var sign3, out var positive, out var negative,
                out var _);
            FLOAT param;
            FLOAT param2;
            if (positive == 3 || negative == 3) {
                intersectionType = IntersectionTypes.Empty;
            }
            else if (GetInterval(ref segment.center, ref segment.direction, ref triangle, dist, dist2, dist3, sign,
                         sign2, sign3, out param, out param2)) {
                intersectionType = IntersectionTypes.Empty;
            }
            else {
                switch (FindSegment1Segment1(param, param2, 0f - segment.extent, segment.extent, out _, out _)) {
                    case 2:
                        intersectionType = IntersectionTypes.Segment;
                        break;
                    case 1:
                        intersectionType = IntersectionTypes.Point;
                        break;
                    default:
                        intersectionType = IntersectionTypes.Empty;
                        break;
                }
            }

            return intersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests whether segment and triangle intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment2Triangle2(ref Segment2D segment, ref Triangle2D triangle) {
            return TestSegment2Triangle2(ref segment, ref triangle, out _);
        }

        /// <summary>
        /// Tests whether segment and triangle intersect and finds actual intersection parameters.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment2Triangle2(ref Segment2D segment, ref Triangle2D triangle,
            out Segment2Triangle2Intr info) {
            TriangleLineRelations(ref segment.center, ref segment.direction, ref triangle, out var dist, out var dist2,
                out var dist3, out var sign, out var sign2, out var sign3, out var positive, out var negative,
                out var _);
            FLOAT param;
            FLOAT param2;
            if (positive == 3 || negative == 3) {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Quantity = 0;
                info.Point0 = FVector2.Zero;
                info.Point1 = FVector2.Zero;
            }
            else if (GetInterval(ref segment.center, ref segment.direction, ref triangle, dist, dist2, dist3, sign,
                         sign2, sign3, out param, out param2)) {
                info.IntersectionType = IntersectionTypes.Empty;
                info.Quantity = 0;
                info.Point0 = FVector2.Zero;
                info.Point1 = FVector2.Zero;
            }
            else {
                info.Quantity = FindSegment1Segment1(param, param2, 0f - segment.extent, segment.extent, out var w,
                    out var w2);
                if (info.Quantity == 2) {
                    info.IntersectionType = IntersectionTypes.Segment;
                    info.Point0 = segment.center + w * segment.direction;
                    info.Point1 = segment.center + w2 * segment.direction;
                }
                else if (info.Quantity == 1) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point0 = segment.center + w * segment.direction;
                    info.Point1 = FVector2.Zero;
                }
                else {
                    info.IntersectionType = IntersectionTypes.Empty;
                    info.Point0 = FVector2.Zero;
                    info.Point1 = FVector2.Zero;
                }
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        private static int WhichSide(ref Triangle2D triangle, ref FVector2 P, ref FVector2 D) {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            FLOAT num4 = D.Dot(triangle.v0 - P);
            if (num4 > 1E-05f) {
                num++;
            }
            else if (num4 < -1E-05f) {
                num2++;
            }
            else {
                num3++;
            }

            if (num > 0 && num2 > 0) {
                return 0;
            }

            num4 = D.Dot(triangle.v1 - P);
            if (num4 > 1E-05f) {
                num++;
            }
            else if (num4 < -1E-05f) {
                num2++;
            }
            else {
                num3++;
            }

            if (num > 0 && num2 > 0) {
                return 0;
            }

            num4 = D.Dot(triangle.v2 - P);
            if (num4 > 1E-05f) {
                num++;
            }
            else if (num4 < -1E-05f) {
                num2++;
            }
            else {
                num3++;
            }

            if (num > 0 && num2 > 0) {
                return 0;
            }

            if (num3 != 0) {
                return 0;
            }

            if (num <= 0) {
                return -1;
            }

            return 1;
        }

        private static void ClipConvexPolygonAgainstLine(ref FVector2 edgeStart, ref FVector2 edgeEnd,
            ref int quantity, ref Triangle2Triangle2Intr info) {
            FVector2 vector = new FVector2(edgeStart.y - edgeEnd.y, edgeEnd.x - edgeStart.x);
            FLOAT num = vector.Dot(edgeStart);
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = -1;
            Float6 f6 = default(Float6);
            for (int i = 0; i < quantity; i++) {
                FLOAT num6 = vector.Dot(info[i]) - num;
                if (num6 > 1E-05f) {
                    num2++;
                    if (num5 < 0) {
                        num5 = i;
                    }
                }
                else if (num6 < -1E-05f) {
                    num3++;
                }
                else {
                    num6 = 0f;
                    num4++;
                }

                f6[i] = num6;
            }

            if (num2 > 0) {
                if (num3 <= 0) {
                    return;
                }

                Triangle2Triangle2Intr triangle2Triangle2Intr = default(Triangle2Triangle2Intr);
                int num7 = 0;
                if (num5 > 0) {
                    int num8 = num5;
                    int i2 = num8 - 1;
                    FLOAT num9 = f6[num8] / (f6[num8] - f6[i2]);
                    triangle2Triangle2Intr[num7++] = info[num8] + num9 * (info[i2] - info[num8]);
                    while (num8 < quantity && f6[num8] > 0f) {
                        triangle2Triangle2Intr[num7++] = info[num8++];
                    }

                    if (num8 < quantity) {
                        i2 = num8 - 1;
                    }
                    else {
                        num8 = 0;
                        i2 = quantity - 1;
                    }

                    num9 = f6[num8] / (f6[num8] - f6[i2]);
                    triangle2Triangle2Intr[num7++] = info[num8] + num9 * (info[i2] - info[num8]);
                }
                else {
                    int num8 = 0;
                    while (num8 < quantity && f6[num8] > 0f) {
                        triangle2Triangle2Intr[num7++] = info[num8++];
                    }

                    int i2 = num8 - 1;
                    FLOAT num10 = f6[num8] / (f6[num8] - f6[i2]);
                    triangle2Triangle2Intr[num7++] = info[num8] + num10 * (info[i2] - info[num8]);
                    for (; num8 < quantity && f6[num8] <= 0f; num8++) { }

                    if (num8 < quantity) {
                        i2 = num8 - 1;
                        num10 = f6[num8] / (f6[num8] - f6[i2]);
                        triangle2Triangle2Intr[num7++] = info[num8] + num10 * (info[i2] - info[num8]);
                        while (num8 < quantity && f6[num8] > 0f) {
                            triangle2Triangle2Intr[num7++] = info[num8++];
                        }
                    }
                    else {
                        i2 = quantity - 1;
                        num10 = f6[0] / (f6[0] - f6[i2]);
                        triangle2Triangle2Intr[num7++] = info[0] + num10 * (info[i2] - info[0]);
                    }
                }

                quantity = num7;
                info = triangle2Triangle2Intr;
                return;
            }

            if (num4 == 0) {
                quantity = 0;
                return;
            }

            int num11 = ((FMath.Abs(vector.y) > FMath.Abs(vector.x)) ? 1 : 0);
            FLOAT num12 = FLOAT.PositiveInfinity;
            FLOAT num13 = FLOAT.NegativeInfinity;
            FLOAT seg1Start;
            FLOAT seg1End;
            if (num11 == 0) {
                for (int j = 0; j < quantity; j++) {
                    if (f6[j] == 0f) {
                        FLOAT y = info[j].y;
                        if (y > num13) {
                            num13 = y;
                        }

                        if (y < num12) {
                            num12 = y;
                        }
                    }
                }

                if (edgeStart.y < edgeEnd.y) {
                    seg1Start = edgeStart.y;
                    seg1End = edgeEnd.y;
                }
                else {
                    seg1Start = edgeEnd.y;
                    seg1End = edgeStart.y;
                }
            }
            else {
                for (int k = 0; k < quantity; k++) {
                    if (f6[k] == 0f) {
                        FLOAT x = info[k].x;
                        if (x > num13) {
                            num13 = x;
                        }

                        if (x < num12) {
                            num12 = x;
                        }
                    }
                }

                if (edgeStart.x < edgeEnd.x) {
                    seg1Start = edgeStart.x;
                    seg1End = edgeEnd.x;
                }
                else {
                    seg1Start = edgeEnd.x;
                    seg1End = edgeStart.x;
                }
            }

            FLOAT w;
            FLOAT w2;
            int num14 = FindSegment1Segment1(num12, num13, seg1Start, seg1End, out w, out w2);
            if (num14 > 0) {
                if (num11 == 0) {
                    info.Point0 = new FVector2((num - vector.y * w) / vector.x, w);
                    if (num14 == 2) {
                        info.Point1 = new FVector2((num - vector.y * w2) / vector.x, w2);
                    }
                }
                else {
                    info.Point0 = new FVector2(w, (num - vector.x * w) / vector.y);
                    if (num14 == 2) {
                        info.Point1 = new FVector2(w2, (num - vector.x * w2) / vector.y);
                    }
                }

                info.IntersectionType = ((num14 == 1) ? IntersectionTypes.Point : IntersectionTypes.Segment);
                info.Quantity = num14;
                quantity = -1;
            }
            else {
                quantity = 0;
            }
        }

        /// <summary>
        /// Tests if a triangle intersects another triangle (both triangles must be ordered counter clockwise). Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestTriangle2Triangle2(ref Triangle2D triangle0, ref Triangle2D triangle1) {
            FVector2 p = triangle0.v0;
            FVector2 v = triangle0.v1;
            FVector2 d = default(FVector2);
            d.x = v.y - p.y;
            d.y = p.x - v.x;
            if (WhichSide(ref triangle1, ref p, ref d) > 0) {
                return false;
            }

            p = triangle0.v1;
            v = triangle0.v2;
            d.x = v.y - p.y;
            d.y = p.x - v.x;
            if (WhichSide(ref triangle1, ref p, ref d) > 0) {
                return false;
            }

            p = triangle0.v2;
            v = triangle0.v0;
            d.x = v.y - p.y;
            d.y = p.x - v.x;
            if (WhichSide(ref triangle1, ref p, ref d) > 0) {
                return false;
            }

            p = triangle1.v0;
            v = triangle1.v1;
            d.x = v.y - p.y;
            d.y = p.x - v.x;
            if (WhichSide(ref triangle0, ref p, ref d) > 0) {
                return false;
            }

            p = triangle1.v1;
            v = triangle1.v2;
            d.x = v.y - p.y;
            d.y = p.x - v.x;
            if (WhichSide(ref triangle0, ref p, ref d) > 0) {
                return false;
            }

            p = triangle1.v2;
            v = triangle1.v0;
            d.x = v.y - p.y;
            d.y = p.x - v.x;
            if (WhichSide(ref triangle0, ref p, ref d) > 0) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a triangle intersects another triangle and finds intersection parameters (both triangles must be ordered counter clockwise). Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindTriangle2Triangle2(ref Triangle2D triangle0, ref Triangle2D triangle1,
            out Triangle2Triangle2Intr info) {
            info = default(Triangle2Triangle2Intr);
            info.Point0 = triangle1.v0;
            info.Point1 = triangle1.v1;
            info.Point2 = triangle1.v2;
            int quantity = 3;
            ClipConvexPolygonAgainstLine(ref triangle0.v2, ref triangle0.v0, ref quantity, ref info);
            if (quantity == 0) {
                return false;
            }

            if (quantity < 0) {
                return true;
            }

            ClipConvexPolygonAgainstLine(ref triangle0.v0, ref triangle0.v1, ref quantity, ref info);
            if (quantity == 0) {
                return false;
            }

            if (quantity < 0) {
                return true;
            }

            ClipConvexPolygonAgainstLine(ref triangle0.v1, ref triangle0.v2, ref quantity, ref info);
            if (quantity == 0) {
                return false;
            }

            if (quantity < 0) {
                return true;
            }

            info.IntersectionType = IntersectionTypes.Polygon;
            info.Quantity = quantity;
            return true;
        }

        /// <summary>
        /// Tests whether two AAB intersect.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestAABB3AABB3(ref AABB box0, ref AABB box1) {
            if (box0.max.x < box1.min.x || box0.min.x > box1.max.x) {
                return false;
            }

            if (box0.max.y < box1.min.y || box0.min.y > box1.max.y) {
                return false;
            }

            if (box0.max.z < box1.min.z || box0.min.z > box1.max.z) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether two AAB intersect and finds intersection which is AAB itself.
        /// Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindAABB3AABB3(ref AABB box0, ref AABB box1, out AABB intersection) {
            if (box0.max.x < box1.min.x || box0.min.x > box1.max.x) {
                intersection = default(AABB);
                return false;
            }

            if (box0.max.y < box1.min.y || box0.min.y > box1.max.y) {
                intersection = default(AABB);
                return false;
            }

            if (box0.max.z < box1.min.z || box0.min.z > box1.max.z) {
                intersection = default(AABB);
                return false;
            }

            intersection.max.x = ((box0.max.x <= box1.max.x) ? box0.max.x : box1.max.x);
            intersection.min.x = ((box0.min.x <= box1.min.x) ? box1.min.x : box0.min.x);
            intersection.max.y = ((box0.max.y <= box1.max.y) ? box0.max.y : box1.max.y);
            intersection.min.y = ((box0.min.y <= box1.min.y) ? box1.min.y : box0.min.y);
            intersection.max.z = ((box0.max.z <= box1.max.z) ? box0.max.z : box1.max.z);
            intersection.min.z = ((box0.min.z <= box1.min.z) ? box1.min.z : box0.min.z);
            return true;
        }

        /// <summary>
        /// Checks whether two aab has x overlap
        /// </summary>
        public static bool TestAABB3AABB3OverlapX(ref AABB box0, ref AABB box1) {
            if (box0.max.x >= box1.min.x) {
                return box0.min.x <= box1.max.x;
            }

            return false;
        }

        /// <summary>
        /// Checks whether two aab has y overlap
        /// </summary>
        public static bool TestAABB3AABB3OverlapY(ref AABB box0, ref AABB box1) {
            if (box0.max.y >= box1.min.y) {
                return box0.min.y <= box1.max.y;
            }

            return false;
        }

        /// <summary>
        /// Checks whether two aab has z overlap
        /// </summary>
        public static bool TestAABB3AABB3OverlapZ(ref AABB box0, ref AABB box1) {
            if (box0.max.z >= box1.min.z) {
                return box0.min.z <= box1.max.z;
            }

            return false;
        }

        /// <summary>
        /// Tests if an axis aligned box intersects a sphere. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestAABB3Sphere3(ref AABB box, ref Sphere sphere) {
            FLOAT num = 0f;
            FLOAT x = sphere.center.x;
            if (x < box.min.x) {
                FLOAT num2 = x - box.min.x;
                num += num2 * num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
            }

            x = sphere.center.y;
            if (x < box.min.y) {
                FLOAT num2 = x - box.min.y;
                num += num2 * num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
            }

            x = sphere.center.z;
            if (x < box.min.z) {
                FLOAT num2 = x - box.min.z;
                num += num2 * num2;
            }
            else if (x > box.max.z) {
                FLOAT num2 = x - box.max.z;
                num += num2 * num2;
            }

            return num <= sphere.radius * sphere.radius;
        }

        /// <summary>
        /// Tests if a box intersects another box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestBox3Box3(ref Box box0, ref Box box1) {
            FLOAT num = 0.99999f;
            bool flag = false;
            FVector3 axis = box0.axis0;
            FVector3 axis2 = box0.axis1;
            FVector3 axis3 = box0.axis2;
            FVector3 axis4 = box1.axis0;
            FVector3 axis5 = box1.axis1;
            FVector3 axis6 = box1.axis2;
            FLOAT x = box0.extents.x;
            FLOAT y = box0.extents.y;
            FLOAT z = box0.extents.z;
            FLOAT x2 = box1.extents.x;
            FLOAT y2 = box1.extents.y;
            FLOAT z2 = box1.extents.z;
            FVector3 value = box1.center - box0.center;

            FLOAT num2 = axis.Dot(axis4);
            FLOAT num3 = FMath.Abs(num2);
            if (num3 > num) {
                flag = true;
            }

            FLOAT num4 = axis.Dot(axis5);
            FLOAT num5 = FMath.Abs(num4);
            if (num5 > num) {
                flag = true;
            }

            FLOAT num6 = axis.Dot(axis6);
            FLOAT num7 = FMath.Abs(num6);
            if (num7 > num) {
                flag = true;
            }

            FLOAT num8 = axis.Dot(value);
            FLOAT num9 = FMath.Abs(num8);
            FLOAT num10 = x2 * num3 + y2 * num5 + z2 * num7;
            FLOAT num11 = x + num10;
            if (num9 > num11) {
                return false;
            }

            FLOAT num12 = axis2.Dot(axis4);
            FLOAT num13 = FMath.Abs(num12);
            if (num13 > num) {
                flag = true;
            }

            FLOAT num14 = axis2.Dot(axis5);
            FLOAT num15 = FMath.Abs(num14);
            if (num15 > num) {
                flag = true;
            }

            FLOAT num16 = axis2.Dot(axis6);
            FLOAT num17 = FMath.Abs(num16);
            if (num17 > num) {
                flag = true;
            }

            FLOAT num18 = axis2.Dot(value);
            num9 = FMath.Abs(num18);
            num10 = x2 * num13 + y2 * num15 + z2 * num17;
            num11 = y + num10;
            if (num9 > num11) {
                return false;
            }

            FLOAT num19 = axis3.Dot(axis4);
            FLOAT num20 = FMath.Abs(num19);
            if (num20 > num) {
                flag = true;
            }

            FLOAT num21 = axis3.Dot(axis5);
            FLOAT num22 = FMath.Abs(num21);
            if (num22 > num) {
                flag = true;
            }

            FLOAT num23 = axis3.Dot(axis6);
            FLOAT num24 = FMath.Abs(num23);
            if (num24 > num) {
                flag = true;
            }

            FLOAT num25 = axis3.Dot(value);
            num9 = FMath.Abs(num25);
            num10 = x2 * num20 + y2 * num22 + z2 * num24;
            num11 = z + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(axis4.Dot(value));
            FLOAT num26 = x * num3 + y * num13 + z * num20;
            num11 = num26 + x2;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(axis5.Dot(value));
            num26 = x * num5 + y * num15 + z * num22;
            num11 = num26 + y2;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(axis6.Dot(value));
            num26 = x * num7 + y * num17 + z * num24;
            num11 = num26 + z2;
            if (num9 > num11) {
                return false;
            }

            if (flag) {
                return true;
            }

            num9 = FMath.Abs(num25 * num12 - num18 * num19);
            num26 = y * num20 + z * num13;
            num10 = y2 * num7 + z2 * num5;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(num25 * num14 - num18 * num21);
            num26 = y * num22 + z * num15;
            num10 = x2 * num7 + z2 * num3;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(num25 * num16 - num18 * num23);
            num26 = y * num24 + z * num17;
            num10 = x2 * num5 + y2 * num3;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(num8 * num19 - num25 * num2);
            num26 = x * num20 + z * num3;
            num10 = y2 * num17 + z2 * num15;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(num8 * num21 - num25 * num4);
            num26 = x * num22 + z * num5;
            num10 = x2 * num17 + z2 * num13;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(num8 * num23 - num25 * num6);
            num26 = x * num24 + z * num7;
            num10 = x2 * num15 + y2 * num13;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(num18 * num2 - num8 * num12);
            num26 = x * num13 + y * num3;
            num10 = y2 * num24 + z2 * num22;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(num18 * num4 - num8 * num14);
            num26 = x * num15 + y * num5;
            num10 = x2 * num24 + z2 * num20;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            num9 = FMath.Abs(num18 * num6 - num8 * num16);
            num26 = x * num17 + y * num7;
            num10 = x2 * num22 + y2 * num20;
            num11 = num26 + num10;
            if (num9 > num11) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a box intersects a capsule. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestBox3Capsule3(ref Box box, ref Capsule capsule) {
            FLOAT num = Distance.Segment3Box3(ref capsule.segment, ref box);
            return num <= capsule.radius;
        }

        /// <summary>
        /// Tests if a box intersects a sphere. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestBox3Sphere3(ref Box box, ref Sphere sphere) {
            FLOAT num = 0f;
            FVector3 vector = sphere.center - box.center;
            FLOAT num2 = vector.Dot(box.axis0);
            FLOAT x = box.extents.x;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
            }

            num2 = vector.Dot(box.axis1);
            x = box.extents.y;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
            }

            num2 = vector.Dot(box.axis2);
            x = box.extents.z;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
            }

            return num <= sphere.radius * sphere.radius;
        }

        private static bool DoClipping(FLOAT t0, FLOAT t1, ref FVector3 origin, ref FVector3 direction, ref AABB box,
            bool solid, out int quantity, out FVector3 point0, out FVector3 point1,
            out IntersectionTypes intrType) {
            box.CalcCenterExtents(out var center, out var extents);
            FVector3 vector = new FVector3(origin.x - center.x, origin.y - center.y, origin.z - center.z);
            FLOAT num = t0;
            FLOAT num2 = t1;
            if (Clip(direction.x, 0f - vector.x - extents.x, ref t0, ref t1) &&
                Clip(0f - direction.x, vector.x - extents.x, ref t0, ref t1) &&
                Clip(direction.y, 0f - vector.y - extents.y, ref t0, ref t1) &&
                Clip(0f - direction.y, vector.y - extents.y, ref t0, ref t1) &&
                Clip(direction.z, 0f - vector.z - extents.z, ref t0, ref t1) &&
                Clip(0f - direction.z, vector.z - extents.z, ref t0, ref t1) && (solid || t0 != num || t1 != num2)) {
                if (t1 > t0) {
                    intrType = IntersectionTypes.Segment;
                    quantity = 2;
                    point0 = origin + t0 * direction;
                    point1 = origin + t1 * direction;
                }
                else {
                    intrType = IntersectionTypes.Point;
                    quantity = 1;
                    point0 = origin + t0 * direction;
                    point1 = FVector3.Zero;
                }
            }
            else {
                intrType = IntersectionTypes.Empty;
                quantity = 0;
                point0 = FVector3.Zero;
                point1 = FVector3.Zero;
            }

            return intrType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a line intersects an axis aligned box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3AABB3(ref Line line, ref AABB box) {
            box.CalcCenterExtents(out var center, out var extents);
            FVector3 value = line.origin - center;
            FVector3 vector = line.direction.Cross(value);
            FLOAT num = FMath.Abs(line.direction.y);
            FLOAT num2 = FMath.Abs(line.direction.z);
            FLOAT num3 = FMath.Abs(vector.x);
            FLOAT num4 = extents.y * num2 + extents.z * num;
            if (num3 > num4) {
                return false;
            }

            FLOAT num5 = FMath.Abs(line.direction.x);
            FLOAT num6 = FMath.Abs(vector.y);
            num4 = extents.x * num2 + extents.z * num5;
            if (num6 > num4) {
                return false;
            }

            FLOAT num7 = FMath.Abs(vector.z);
            num4 = extents.x * num + extents.y * num5;
            if (num7 > num4) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a line intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3AABB3(ref Line line, ref AABB box, out Line3AAB3Intr info) {
            return DoClipping(FLOAT.NegativeInfinity, FLOAT.PositiveInfinity, ref line.origin, ref line.direction,
                ref box, solid: true, out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        private static bool DoClipping(FLOAT t0, FLOAT t1, ref FVector3 origin, ref FVector3 direction, ref Box box,
            bool solid, out int quantity, out FVector3 point0, out FVector3 point1,
            out IntersectionTypes intrType) {
            FVector3 vector = origin - box.center;
            FVector3 vector2 = new FVector3(vector.Dot(box.axis0), vector.Dot(box.axis1), vector.Dot(box.axis2));
            FVector3 vector3 = new FVector3(direction.Dot(box.axis0), direction.Dot(box.axis1),
                direction.Dot(box.axis2));
            FLOAT num = t0;
            FLOAT num2 = t1;
            if (Clip(vector3.x, 0f - vector2.x - box.extents.x, ref t0, ref t1) &&
                Clip(0f - vector3.x, vector2.x - box.extents.x, ref t0, ref t1) &&
                Clip(vector3.y, 0f - vector2.y - box.extents.y, ref t0, ref t1) &&
                Clip(0f - vector3.y, vector2.y - box.extents.y, ref t0, ref t1) &&
                Clip(vector3.z, 0f - vector2.z - box.extents.z, ref t0, ref t1) &&
                Clip(0f - vector3.z, vector2.z - box.extents.z, ref t0, ref t1) && (solid || t0 != num || t1 != num2)) {
                if (t1 > t0) {
                    intrType = IntersectionTypes.Segment;
                    quantity = 2;
                    point0 = origin + t0 * direction;
                    point1 = origin + t1 * direction;
                }
                else {
                    intrType = IntersectionTypes.Point;
                    quantity = 1;
                    point0 = origin + t0 * direction;
                    point1 = FVector3.Zero;
                }
            }
            else {
                intrType = IntersectionTypes.Empty;
                quantity = 0;
                point0 = FVector3.Zero;
                point1 = FVector3.Zero;
            }

            return intrType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a line intersects a box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Box3(ref Line line, ref Box box) {
            FVector3 value = line.origin - box.center;
            FVector3 vector = line.direction.Cross(value);
            FLOAT num = FMath.Abs(line.direction.Dot(box.axis1));
            FLOAT num2 = FMath.Abs(line.direction.Dot(box.axis2));
            FLOAT num3 = FMath.Abs(vector.Dot(box.axis0));
            FLOAT num4 = box.extents.y * num2 + box.extents.z * num;
            if (num3 > num4) {
                return false;
            }

            FLOAT num5 = FMath.Abs(line.direction.Dot(box.axis0));
            FLOAT num6 = FMath.Abs(vector.Dot(box.axis1));
            num4 = box.extents.x * num2 + box.extents.z * num5;
            if (num6 > num4) {
                return false;
            }

            FLOAT num7 = FMath.Abs(vector.Dot(box.axis2));
            num4 = box.extents.x * num + box.extents.y * num5;
            if (num7 > num4) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a line intersects a box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Box3(ref Line line, ref Box box, out Line3Box3Intr info) {
            return DoClipping(FLOAT.NegativeInfinity, FLOAT.PositiveInfinity, ref line.origin, ref line.direction,
                ref box, solid: true, out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests if a line intersects a solid circle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Circle3(ref Line line, ref Circle circle) {
            return FindLine3Circle3(ref line, ref circle, out _);
        }

        /// <summary>
        /// Tests if a line intersects a solid circle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Circle3(ref Line line, ref Circle circle, out Line3Circle3Intr info) {
            FLOAT num = line.direction.Dot(circle.normal);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = circle.normal.Dot(line.origin - circle.center);
                FLOAT t = (0f - num2) / num;
                FVector3 vector = line.Eval(t);
                if ((vector - circle.center).sqrMagnitude <= circle.radius * circle.radius) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point = vector;
                    return true;
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a line intersects a plane. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Plane3(ref Line line, ref Plane plane, out IntersectionTypes intersectionType) {
            FLOAT f = line.direction.Dot(plane.normal);
            if (FMath.Abs(f) > _dotThreshold) {
                intersectionType = IntersectionTypes.Point;
                return true;
            }

            FLOAT f2 = plane.SignedDistanceTo(ref line.origin);
            if (FMath.Abs(f2) <= _distanceThreshold) {
                intersectionType = IntersectionTypes.Line;
                return true;
            }

            intersectionType = IntersectionTypes.Empty;
            return false;
        }

        /// <summary>
        /// Tests if a line intersects a plane. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Plane3(ref Line line, ref Plane plane) {
            return TestLine3Plane3(ref line, ref plane, out _);
        }

        /// <summary>
        /// Tests if a line intersects a plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Plane3(ref Line line, ref Plane plane, out Line3Plane3Intr info) {
            FLOAT num = line.direction.Dot(plane.normal);
            FLOAT num2 = plane.SignedDistanceTo(ref line.origin);
            if (FMath.Abs(num) > _dotThreshold) {
                info.LineParameter = (0f - num2) / num;
                info.IntersectionType = IntersectionTypes.Point;
                info.Point = line.Eval(info.LineParameter);
                return true;
            }

            if (FMath.Abs(num2) <= _distanceThreshold) {
                info.LineParameter = 0f;
                info.IntersectionType = IntersectionTypes.Line;
                info.Point = FVector3.Zero;
                return true;
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.LineParameter = 0f;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a line intersects a solid polygon. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Polygon3(ref Line line, Polygon polygon) {
            return FindLine3Polygon3(ref line, polygon, out _);
        }

        /// <summary>
        /// Tests if a line intersects a solid polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Polygon3(ref Line line, Polygon polygon, out Line3Polygon3Intr info) {
            Plane plane = polygon.Plane;
            FLOAT num = line.direction.Dot(plane.normal);
            FLOAT num2 = plane.SignedDistanceTo(ref line.origin);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT t = (0f - num2) / num;
                FVector3 vector = line.Eval(t);
                ProjectionPlanes projectionPlane = plane.normal.GetProjectionPlane();
                Polygon2D polygon2D = Polygon2D.CreateProjected(polygon, projectionPlane);
                FVector2 point = vector.ToFVector2(projectionPlane);
                if (polygon2D.ContainsSimple(point)) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point = vector;
                    return true;
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        private static bool Point3InsideRectangle3(ref FVector3 point, ref Rectangle rectangle) {
            FVector3 vector = point - rectangle.center;
            FLOAT num = vector.Dot(rectangle.axis0);
            FLOAT num2 = vector.Dot(rectangle.axis1);
            FLOAT x = rectangle.extents.x;
            if (num < 0f - x) {
                return false;
            }

            if (num > x) {
                return false;
            }

            x = rectangle.extents.y;
            if (num2 < 0f - x) {
                return false;
            }

            if (num2 > x) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a line intersects a solid rectangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Rectangle3(ref Line line, ref Rectangle rectangle) {
            return FindLine3Rectangle3(ref line, ref rectangle, out _);
        }

        /// <summary>
        /// Tests if a line intersects a solid rectangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Rectangle3(ref Line line, ref Rectangle rectangle, out Line3Rectangle3Intr info) {
            FLOAT num = line.direction.Dot(rectangle.normal);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = rectangle.normal.Dot(line.origin - rectangle.center);
                FLOAT t = (0f - num2) / num;
                FVector3 point = line.Eval(t);
                if (Point3InsideRectangle3(ref point, ref rectangle)) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point = point;
                    return true;
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a line intersects a sphere. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Sphere3(ref Line line, ref Sphere sphere) {
            FVector3 value = line.origin - sphere.center;
            FLOAT num = value.sqrMagnitude - sphere.radius * sphere.radius;
            FLOAT num2 = line.direction.Dot(value);
            FLOAT num3 = num2 * num2 - num;
            return num3 >= -1E-05f;
        }

        /// <summary>
        /// Tests if a line intersects a sphere and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Sphere3(ref Line line, ref Sphere sphere, out Line3Sphere3Intr info) {
            FVector3 vector = line.origin - sphere.center;
            FLOAT num = vector.Dot(vector) - sphere.radius * sphere.radius;
            FLOAT num2 = line.direction.Dot(vector);
            FLOAT num3 = num2 * num2 - num;
            if (num3 < -1E-05f) {
                info = default(Line3Sphere3Intr);
            }
            else if (num3 > 1E-05f) {
                FLOAT num4 = FMath.Sqrt(num3);
                info.LineParameter0 = 0f - num2 - num4;
                info.LineParameter1 = 0f - num2 + num4;
                info.Point0 = line.origin + info.LineParameter0 * line.direction;
                info.Point1 = line.origin + info.LineParameter1 * line.direction;
                info.IntersectionType = IntersectionTypes.Segment;
                info.Quantity = 2;
            }
            else {
                info.LineParameter0 = 0f - num2;
                info.LineParameter1 = 0f;
                info.Point0 = line.origin + info.LineParameter0 * line.direction;
                info.Point1 = FVector3.Zero;
                info.IntersectionType = IntersectionTypes.Point;
                info.Quantity = 1;
            }

            return (FLOAT)info.Quantity > 0f;
        }

        /// <summary>
        /// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Triangle3(ref Line line, ref Triangle triangle,
            out IntersectionTypes intersectionType) {
            FVector3 vector = line.origin - triangle.v0;
            FVector3 vector2 = triangle.v1 - triangle.v0;
            FVector3 value = triangle.v2 - triangle.v0;
            FVector3 value2 = vector2.Cross(value);
            FLOAT num = line.direction.Dot(value2);
            FLOAT num2;
            if (num > _dotThreshold) {
                num2 = 1f;
            }
            else {
                if (!(num < 0f - _dotThreshold)) {
                    intersectionType = IntersectionTypes.Empty;
                    return false;
                }

                num2 = -1f;
                num = 0f - num;
            }

            FLOAT num3 = num2 * line.direction.Dot(vector.Cross(value));
            if (num3 >= -1E-05f) {
                FLOAT num4 = num2 * line.direction.Dot(vector2.Cross(vector));
                if (num4 >= -1E-05f && num3 + num4 <= num + 1E-05f) {
                    intersectionType = IntersectionTypes.Point;
                    return true;
                }
            }

            intersectionType = IntersectionTypes.Empty;
            return false;
        }

        /// <summary>
        /// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Triangle3(ref Line line, ref FVector3 v0, ref FVector3 v1, ref FVector3 v2,
            out IntersectionTypes intersectionType) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestLine3Triangle3(ref line, ref triangle2, out intersectionType);
        }

        /// <summary>
        /// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Triangle3(ref Line line, FVector3 v0, FVector3 v1, FVector3 v2,
            out IntersectionTypes intersectionType) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestLine3Triangle3(ref line, ref triangle2, out intersectionType);
        }

        /// <summary>
        /// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Triangle3(ref Line line, ref Triangle triangle) {
            return TestLine3Triangle3(ref line, ref triangle, out _);
        }

        /// <summary>
        /// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Triangle3(ref Line line, ref FVector3 v0, ref FVector3 v1, ref FVector3 v2) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestLine3Triangle3(ref line, ref triangle2, out _);
        }

        /// <summary>
        /// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestLine3Triangle3(ref Line line, FVector3 v0, FVector3 v1, FVector3 v2) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestLine3Triangle3(ref line, ref triangle2, out _);
        }

        /// <summary>
        /// Tests if a line intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Triangle3(ref Line line, ref Triangle triangle, out Line3Triangle3Intr info) {
            FVector3 vector = line.origin - triangle.v0;
            FVector3 vector2 = triangle.v1 - triangle.v0;
            FVector3 value = triangle.v2 - triangle.v0;
            FVector3 value2 = vector2.Cross(value);
            FLOAT num = line.direction.Dot(value2);
            FLOAT num2;
            if (num > _dotThreshold) {
                num2 = 1f;
            }
            else {
                if (!(num < 0f - _dotThreshold)) {
                    info = default(Line3Triangle3Intr);
                    return false;
                }

                num2 = -1f;
                num = 0f - num;
            }

            FLOAT num3 = num2 * line.direction.Dot(vector.Cross(value));
            if (num3 >= -1E-05f) {
                FLOAT num4 = num2 * line.direction.Dot(vector2.Cross(vector));
                if (num4 >= -1E-05f && num3 + num4 <= num + 1E-05f) {
                    FLOAT num5 = (0f - num2) * vector.Dot(value2);
                    FLOAT num6 = 1f / num;
                    info.IntersectionType = IntersectionTypes.Point;
                    info.LineParameter = num5 * num6;
                    info.Point = line.Eval(info.LineParameter);
                    info.TriBary1 = num3 * num6;
                    info.TriBary2 = num4 * num6;
                    info.TriBary0 = 1f - info.TriBary1 - info.TriBary2;
                    return true;
                }
            }

            info = default(Line3Triangle3Intr);
            return false;
        }

        /// <summary>
        /// Tests if a line intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Triangle3(ref Line line, ref FVector3 v0, ref FVector3 v1, ref FVector3 v2,
            out Line3Triangle3Intr info) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return FindLine3Triangle3(ref line, ref triangle2, out info);
        }

        /// <summary>
        /// Tests if a line intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindLine3Triangle3(ref Line line, FVector3 v0, FVector3 v1, FVector3 v2,
            out Line3Triangle3Intr info) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return FindLine3Triangle3(ref line, ref triangle2, out info);
        }

        /// <summary>
        /// Tests if a plane intersects a box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestPlane3AABB3(ref Plane plane, ref AABB box) {
            FVector3 point = default(FVector3);
            FVector3 point2 = default(FVector3);
            if (plane.normal.x >= 0f) {
                point.x = box.min.x;
                point2.x = box.max.x;
            }
            else {
                point.x = box.max.x;
                point2.x = box.min.x;
            }

            if (plane.normal.y >= 0f) {
                point.y = box.min.y;
                point2.y = box.max.y;
            }
            else {
                point.y = box.max.y;
                point2.y = box.min.y;
            }

            if (plane.normal.z >= 0f) {
                point.z = box.min.z;
                point2.z = box.max.z;
            }
            else {
                point.z = box.max.z;
                point2.z = box.min.z;
            }

            if (plane.SignedDistanceTo(ref point) >= 0f) {
                return false;
            }

            return plane.SignedDistanceTo(ref point2) > 0f;
        }

        /// <summary>
        /// Tests if a plane intersects a box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestPlane3Box3(ref Plane plane, ref Box box) {
            FLOAT f = box.extents.x * plane.normal.Dot(box.axis0);
            FLOAT f2 = box.extents.y * plane.normal.Dot(box.axis1);
            FLOAT f3 = box.extents.z * plane.normal.Dot(box.axis2);
            FLOAT num = FMath.Abs(f) + FMath.Abs(f2) + FMath.Abs(f3);
            FLOAT f4 = plane.SignedDistanceTo(ref box.center);
            return FMath.Abs(f4) <= num;
        }

        /// <summary>
        /// Tests if a plane intersects another plane. Returns true if intersection occurs false otherwise (also returns false when planes are the same)
        /// </summary>
        public static bool TestPlane3Plane3(ref Plane plane0, ref Plane plane1) {
            FLOAT f = plane0.normal.Dot(plane1.normal);
            return FMath.Abs(f) < 0.99999f;
        }

        /// <summary>
        /// Tests if a plane intersects another plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindPlane3Plane3(ref Plane plane0, ref Plane plane1, out Plane3Plane3Intr info) {
            FLOAT num = plane0.normal.Dot(plane1.normal);
            if (FMath.Abs(num) >= 0.99999f) {
                FLOAT f = ((!(num >= 0f)) ? (plane0.constant + plane1.constant) : (plane0.constant - plane1.constant));
                if (FMath.Abs(f) < 1E-05f) {
                    info.IntersectionType = IntersectionTypes.Plane;
                    info.Line = default(Line);
                    return true;
                }

                info.IntersectionType = IntersectionTypes.Empty;
                info.Line = default(Line);
                return false;
            }

            FLOAT num2 = 1f / (1f - num * num);
            FLOAT num3 = (plane0.constant - num * plane1.constant) * num2;
            FLOAT num4 = (plane1.constant - num * plane0.constant) * num2;
            info.IntersectionType = IntersectionTypes.Line;
            info.Line.origin = num3 * plane0.normal + num4 * plane1.normal;
            info.Line.direction = plane0.normal.NormalizeCross(plane1.normal);
            return true;
        }

        /// <summary>
        /// Tests if a plane intersects a plane. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestPlane3Sphere3(ref Plane plane, ref Sphere sphere) {
            FLOAT f = plane.SignedDistanceTo(ref sphere.center);
            return FMath.Abs(f) <= sphere.radius + 1E-05f;
        }

        /// <summary>
        /// Tests if a plane intersects a plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindPlane3Sphere3(ref Plane plane, ref Sphere sphere, out Plane3Sphere3Intr info) {
            FLOAT num = plane.SignedDistanceTo(ref sphere.center);
            FLOAT num2 = FMath.Abs(num);
            if (num2 <= sphere.radius + 1E-05f) {
                if (num2 >= sphere.radius - 1E-05f) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Circle = default(Circle);
                }
                else {
                    FVector3 center = sphere.center - num * plane.normal;
                    FLOAT radius = FMath.Sqrt(FMath.Abs(sphere.radius * sphere.radius - num2 * num2));
                    info.IntersectionType = IntersectionTypes.Other;
                    info.Circle = new Circle(ref center, ref plane.normal, radius);
                }

                return true;
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Circle = default(Circle);
            return false;
        }

        /// <summary>
        /// Tests if a plane intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestPlane3Triangle3(ref Plane plane, ref Triangle triangle) {
            FLOAT num = 0f;
            FLOAT num2 = plane.SignedDistanceTo(ref triangle.v0);
            if (FMath.Abs(num2) <= _distanceThreshold) {
                num2 = num;
            }

            FLOAT num3 = plane.SignedDistanceTo(ref triangle.v1);
            if (FMath.Abs(num3) <= _distanceThreshold) {
                num3 = num;
            }

            FLOAT num4 = plane.SignedDistanceTo(ref triangle.v2);
            if (FMath.Abs(num4) <= _distanceThreshold) {
                num4 = num;
            }

            if (!(num2 > num) || !(num3 > num) || !(num4 > num)) {
                if (num2 < num && num3 < num) {
                    return !(num4 < num);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tests if a plane intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindPlane3Triangle3(ref Plane plane, ref Triangle triangle, out Plane3Triangle3Intr info) {
            FLOAT num = 0f;
            FLOAT num2 = plane.SignedDistanceTo(ref triangle.v0);
            if (FMath.Abs(num2) <= _distanceThreshold) {
                num2 = num;
            }

            FLOAT num3 = plane.SignedDistanceTo(ref triangle.v1);
            if (FMath.Abs(num3) <= _distanceThreshold) {
                num3 = num;
            }

            FLOAT num4 = plane.SignedDistanceTo(ref triangle.v2);
            if (FMath.Abs(num4) <= _distanceThreshold) {
                num4 = num;
            }

            FVector3 v = triangle.v0;
            FVector3 v2 = triangle.v1;
            FVector3 v3 = triangle.v2;
            info.Point0 = (info.Point1 = (info.Point2 = FVector3.Zero));
            if (num2 > num) {
                if (num3 > num) {
                    if (num4 > num) {
                        info.IntersectionType = IntersectionTypes.Empty;
                        info.Quantity = 0;
                    }
                    else if (num4 < num) {
                        info.Quantity = 2;
                        info.Point0 = v + num2 / (num2 - num4) * (v3 - v);
                        info.Point1 = v2 + num3 / (num3 - num4) * (v3 - v2);
                        info.IntersectionType = IntersectionTypes.Segment;
                    }
                    else {
                        info.Quantity = 1;
                        info.Point0 = v3;
                        info.IntersectionType = IntersectionTypes.Point;
                    }
                }
                else if (num3 < num) {
                    if (num4 > num) {
                        info.Quantity = 2;
                        info.Point0 = v + num2 / (num2 - num3) * (v2 - v);
                        info.Point1 = v2 + num3 / (num3 - num4) * (v3 - v2);
                        info.IntersectionType = IntersectionTypes.Segment;
                    }
                    else if (num4 < num) {
                        info.Quantity = 2;
                        info.Point0 = v + num2 / (num2 - num3) * (v2 - v);
                        info.Point1 = v + num2 / (num2 - num4) * (v3 - v);
                        info.IntersectionType = IntersectionTypes.Segment;
                    }
                    else {
                        info.Quantity = 2;
                        info.Point0 = v + num2 / (num2 - num3) * (v2 - v);
                        info.Point1 = v3;
                        info.IntersectionType = IntersectionTypes.Segment;
                    }
                }
                else if (num4 > num) {
                    info.Quantity = 1;
                    info.Point0 = v2;
                    info.IntersectionType = IntersectionTypes.Point;
                }
                else if (num4 < num) {
                    info.Quantity = 2;
                    info.Point0 = v + num2 / (num2 - num4) * (v3 - v);
                    info.Point1 = v2;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
                else {
                    info.Quantity = 2;
                    info.Point0 = v2;
                    info.Point1 = v3;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
            }
            else if (num2 < num) {
                if (num3 > num) {
                    if (num4 > num) {
                        info.Quantity = 2;
                        info.Point0 = v + num2 / (num2 - num3) * (v2 - v);
                        info.Point1 = v + num2 / (num2 - num4) * (v3 - v);
                        info.IntersectionType = IntersectionTypes.Segment;
                    }
                    else if (num4 < num) {
                        info.Quantity = 2;
                        info.Point0 = v + num2 / (num2 - num3) * (v2 - v);
                        info.Point1 = v2 + num3 / (num3 - num4) * (v3 - v2);
                        info.IntersectionType = IntersectionTypes.Segment;
                    }
                    else {
                        info.Quantity = 2;
                        info.Point0 = v + num2 / (num2 - num3) * (v2 - v);
                        info.Point1 = v3;
                        info.IntersectionType = IntersectionTypes.Segment;
                    }
                }
                else if (num3 < num) {
                    if (num4 > num) {
                        info.Quantity = 2;
                        info.Point0 = v + num2 / (num2 - num4) * (v3 - v);
                        info.Point1 = v2 + num3 / (num3 - num4) * (v3 - v2);
                        info.IntersectionType = IntersectionTypes.Segment;
                    }
                    else if (num4 < num) {
                        info.Quantity = 0;
                        info.IntersectionType = IntersectionTypes.Empty;
                    }
                    else {
                        info.Quantity = 1;
                        info.Point0 = v3;
                        info.IntersectionType = IntersectionTypes.Point;
                    }
                }
                else if (num4 > num) {
                    info.Quantity = 2;
                    info.Point0 = v + num2 / (num2 - num4) * (v3 - v);
                    info.Point1 = v2;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
                else if (num4 < num) {
                    info.Quantity = 1;
                    info.Point0 = v2;
                    info.IntersectionType = IntersectionTypes.Point;
                }
                else {
                    info.Quantity = 2;
                    info.Point0 = v2;
                    info.Point1 = v3;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
            }
            else if (num3 > num) {
                if (num4 > num) {
                    info.Quantity = 1;
                    info.Point0 = v;
                    info.IntersectionType = IntersectionTypes.Point;
                }
                else if (num4 < num) {
                    info.Quantity = 2;
                    info.Point0 = v2 + num3 / (num3 - num4) * (v3 - v2);
                    info.Point1 = v;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
                else {
                    info.Quantity = 2;
                    info.Point0 = v;
                    info.Point1 = v3;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
            }
            else if (num3 < num) {
                if (num4 > num) {
                    info.Quantity = 2;
                    info.Point0 = v2 + num3 / (num3 - num4) * (v3 - v2);
                    info.Point1 = v;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
                else if (num4 < num) {
                    info.Quantity = 1;
                    info.Point0 = v;
                    info.IntersectionType = IntersectionTypes.Point;
                }
                else {
                    info.Quantity = 2;
                    info.Point0 = v;
                    info.Point1 = v3;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
            }
            else if (num4 > num) {
                info.Quantity = 2;
                info.Point0 = v;
                info.Point1 = v2;
                info.IntersectionType = IntersectionTypes.Segment;
            }
            else if (num4 < num) {
                info.Quantity = 2;
                info.Point0 = v;
                info.Point1 = v2;
                info.IntersectionType = IntersectionTypes.Segment;
            }
            else {
                info.Quantity = 3;
                info.Point0 = v;
                info.Point1 = v2;
                info.Point2 = v3;
                info.IntersectionType = IntersectionTypes.Polygon;
            }

            return info.IntersectionType != IntersectionTypes.Empty;
        }

        /// <summary>
        /// Tests if a ray intersects an axis aligned box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3AABB3(ref Ray ray, ref AABB box) {
            box.CalcCenterExtents(out var center, out var extents);
            FVector3 value = ray.origin - center;
            FLOAT x = ray.direction.x;
            FLOAT num = FMath.Abs(x);
            FLOAT x2 = value.x;
            FLOAT num2 = FMath.Abs(x2);
            if (num2 > extents.x && x2 * x >= 0f) {
                return false;
            }

            FLOAT y = ray.direction.y;
            FLOAT num3 = FMath.Abs(y);
            FLOAT y2 = value.y;
            FLOAT num4 = FMath.Abs(y2);
            if (num4 > extents.y && y2 * y >= 0f) {
                return false;
            }

            FLOAT z = ray.direction.z;
            FLOAT num5 = FMath.Abs(z);
            FLOAT z2 = value.z;
            FLOAT num6 = FMath.Abs(z2);
            if (num6 > extents.z && z2 * z >= 0f) {
                return false;
            }

            FVector3 vector = ray.direction.Cross(value);
            FLOAT num7 = FMath.Abs(vector.x);
            FLOAT num8 = extents.y * num5 + extents.z * num3;
            if (num7 > num8) {
                return false;
            }

            FLOAT num9 = FMath.Abs(vector.y);
            num8 = extents.x * num5 + extents.z * num;
            if (num9 > num8) {
                return false;
            }

            FLOAT num10 = FMath.Abs(vector.z);
            num8 = extents.x * num3 + extents.y * num;
            if (num10 > num8) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a ray intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3AABB3(ref Ray ray, ref AABB box, out Ray3AAB3Intr info) {
            return DoClipping(0f, FLOAT.PositiveInfinity, ref ray.origin, ref ray.direction, ref box, solid: true,
                out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests if a ray intersects a box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Box3(ref Ray ray, ref Box box) {
            FVector3 vector = ray.origin - box.center;
            FLOAT num = ray.direction.Dot(box.axis0);
            FLOAT num2 = FMath.Abs(num);
            FLOAT num3 = vector.Dot(box.axis0);
            FLOAT num4 = FMath.Abs(num3);
            if (num4 > box.extents.x && num3 * num >= 0f) {
                return false;
            }

            FLOAT num5 = ray.direction.Dot(box.axis1);
            FLOAT num6 = FMath.Abs(num5);
            FLOAT num7 = vector.Dot(box.axis1);
            FLOAT num8 = FMath.Abs(num7);
            if (num8 > box.extents.y && num7 * num5 >= 0f) {
                return false;
            }

            FLOAT num9 = ray.direction.Dot(box.axis2);
            FLOAT num10 = FMath.Abs(num9);
            FLOAT num11 = vector.Dot(box.axis2);
            FLOAT num12 = FMath.Abs(num11);
            if (num12 > box.extents.z && num11 * num9 >= 0f) {
                return false;
            }

            FVector3 vector2 = ray.direction.Cross(vector);
            FLOAT num13 = FMath.Abs(vector2.Dot(box.axis0));
            FLOAT num14 = box.extents.y * num10 + box.extents.z * num6;
            if (num13 > num14) {
                return false;
            }

            FLOAT num15 = FMath.Abs(vector2.Dot(box.axis1));
            num14 = box.extents.x * num10 + box.extents.z * num2;
            if (num15 > num14) {
                return false;
            }

            FLOAT num16 = FMath.Abs(vector2.Dot(box.axis2));
            num14 = box.extents.x * num6 + box.extents.y * num2;
            if (num16 > num14) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a ray intersects a box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Box3(ref Ray ray, ref Box box, out Ray3Box3Intr info) {
            return DoClipping(0f, FLOAT.PositiveInfinity, ref ray.origin, ref ray.direction, ref box, solid: true,
                out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests if a ray intersects a solid circle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Circle3(ref Ray ray, ref Circle circle) {
            return FindRay3Circle3(ref ray, ref circle, out _);
        }

        /// <summary>
        /// Tests if a ray intersects a solid circle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Circle3(ref Ray ray, ref Circle circle, out Ray3Circle3Intr info) {
            FLOAT num = ray.direction.Dot(circle.normal);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = circle.normal.Dot(ray.origin - circle.center);
                FLOAT num3 = (0f - num2) / num;
                if (num3 >= 0f - _intervalThreshold) {
                    FVector3 vector = ray.origin + num3 * ray.direction;
                    if ((vector - circle.center).sqrMagnitude <= circle.radius * circle.radius) {
                        info.IntersectionType = IntersectionTypes.Point;
                        info.Point = vector;
                        return true;
                    }
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a plane. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Plane3(ref Ray ray, ref Plane plane, out IntersectionTypes intersectionType) {
            Ray3Plane3Intr info;
            bool result = FindRay3Plane3(ref ray, ref plane, out info);
            intersectionType = info.IntersectionType;
            return result;
        }

        /// <summary>
        /// Tests if a ray intersects a plane. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Plane3(ref Ray ray, ref Plane plane) {
            return FindRay3Plane3(ref ray, ref plane, out _);
        }

        /// <summary>
        /// Tests if a ray intersects a plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Plane3(ref Ray ray, ref Plane plane, out Ray3Plane3Intr info) {
            FLOAT num = ray.direction.Dot(plane.normal);
            FLOAT num2 = plane.SignedDistanceTo(ref ray.origin);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num3 = (0f - num2) / num;
                if (num3 >= 0f - _intervalThreshold) {
                    info.RayParameter = num3;
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point = ray.origin + num3 * ray.direction;
                    return true;
                }

                info.IntersectionType = IntersectionTypes.Empty;
                info.RayParameter = 0f;
                info.Point = FVector3.Zero;
                return false;
            }

            if (FMath.Abs(num2) <= _distanceThreshold) {
                info.RayParameter = 0f;
                info.IntersectionType = IntersectionTypes.Ray;
                info.Point = FVector3.Zero;
                return true;
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.RayParameter = 0f;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a line intersects a solid polygon. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Polygon3(ref Ray ray, Polygon polygon) {
            return FindRay3Polygon3(ref ray, polygon, out _);
        }

        /// <summary>
        /// Tests if a ray intersects a solid polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Polygon3(ref Ray ray, Polygon polygon, out Ray3Polygon3Intr info) {
            Plane plane = polygon.Plane;
            FLOAT num = ray.direction.Dot(plane.normal);
            FLOAT num2 = plane.SignedDistanceTo(ref ray.origin);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num3 = (0f - num2) / num;
                if (!(num3 >= 0f - _intervalThreshold)) {
                    info.IntersectionType = IntersectionTypes.Empty;
                    info.Point = FVector3.Zero;
                    return false;
                }

                FVector3 vector = ray.origin + num3 * ray.direction;
                ProjectionPlanes projectionPlane = plane.normal.GetProjectionPlane();
                Polygon2D polygon2D = Polygon2D.CreateProjected(polygon, projectionPlane);
                FVector2 point = vector.ToFVector2(projectionPlane);
                if (polygon2D.ContainsSimple(point)) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point = vector;
                    return true;
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a solid rectangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Rectangle3(ref Ray ray, ref Rectangle rectangle) {
            return FindRay3Rectangle3(ref ray, ref rectangle, out _);
        }

        /// <summary>
        /// Tests if a ray intersects a solid rectangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Rectangle3(ref Ray ray, ref Rectangle rectangle, out Ray3Rectangle3Intr info) {
            FLOAT num = ray.direction.Dot(rectangle.normal);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = rectangle.normal.Dot(ray.origin - rectangle.center);
                FLOAT num3 = (0f - num2) / num;
                if (num3 >= 0f - _intervalThreshold) {
                    FVector3 point = ray.origin + num3 * ray.direction;
                    if (Point3InsideRectangle3(ref point, ref rectangle)) {
                        info.IntersectionType = IntersectionTypes.Point;
                        info.Point = point;
                        return true;
                    }
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a sphere. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Sphere3(ref Ray ray, ref Sphere sphere) {
            FVector3 vector = ray.origin - sphere.center;
            FLOAT num = vector.Dot(vector) - sphere.radius * sphere.radius;
            if (num <= 0f) {
                return true;
            }

            FLOAT num2 = ray.direction.Dot(vector);
            if (num2 >= 0f) {
                return false;
            }

            return num2 * num2 >= num;
        }

        /// <summary>
        /// Tests if a ray intersects a sphere and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Sphere3(ref Ray ray, ref Sphere sphere, out Ray3Sphere3Intr info) {
            FVector3 vector = ray.origin - sphere.center;
            FLOAT num = vector.Dot(vector) - sphere.radius * sphere.radius;
            FLOAT num2;
            FLOAT f;
            if (num <= 0f) {
                num2 = ray.direction.Dot(vector);
                f = num2 * num2 - num;
                FLOAT num3 = FMath.Sqrt(f);
                info.RayParameter0 = 0f - num2 + num3;
                info.RayParameter1 = 0f;
                info.Point0 = ray.origin + info.RayParameter0 * ray.direction;
                info.Point1 = FVector3.Zero;
                info.Quantity = 1;
                info.IntersectionType = IntersectionTypes.Point;
                return true;
            }

            num2 = ray.direction.Dot(vector);
            if (num2 >= 0f) {
                info = default(Ray3Sphere3Intr);
                return false;
            }

            f = num2 * num2 - num;
            if (f < 0f) {
                info = default(Ray3Sphere3Intr);
            }
            else if (f >= 1E-05f) {
                FLOAT num3 = FMath.Sqrt(f);
                info.RayParameter0 = 0f - num2 - num3;
                info.RayParameter1 = 0f - num2 + num3;
                info.Point0 = ray.origin + info.RayParameter0 * ray.direction;
                info.Point1 = ray.origin + info.RayParameter1 * ray.direction;
                info.Quantity = 2;
                info.IntersectionType = IntersectionTypes.Segment;
            }
            else {
                info.RayParameter0 = 0f - num2;
                info.RayParameter1 = 0f;
                info.Point0 = ray.origin + info.RayParameter0 * ray.direction;
                info.Point1 = FVector3.Zero;
                info.Quantity = 1;
                info.IntersectionType = IntersectionTypes.Point;
            }

            return info.Quantity > 0;
        }

        /// <summary>
        /// Tests if a ray intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Triangle3(ref Ray ray, ref Triangle triangle,
            out IntersectionTypes intersectionType) {
            FVector3 vector = ray.origin - triangle.v0;
            FVector3 vector2 = triangle.v1 - triangle.v0;
            FVector3 value = triangle.v2 - triangle.v0;
            FVector3 value2 = vector2.Cross(value);
            FLOAT num = ray.direction.Dot(value2);
            FLOAT num2;
            if (num > _dotThreshold) {
                num2 = 1f;
            }
            else {
                if (!(num < 0f - _dotThreshold)) {
                    intersectionType = IntersectionTypes.Empty;
                    return false;
                }

                num2 = -1f;
                num = 0f - num;
            }

            FLOAT num3 = num2 * ray.direction.Dot(vector.Cross(value));
            if (num3 >= -1E-05f) {
                FLOAT num4 = num2 * ray.direction.Dot(vector2.Cross(vector));
                if (num4 >= -1E-05f && num3 + num4 <= num + 1E-05f) {
                    FLOAT num5 = (0f - num2) * vector.Dot(value2);
                    if (num5 >= 0f - _intervalThreshold) {
                        intersectionType = IntersectionTypes.Point;
                        return true;
                    }
                }
            }

            intersectionType = IntersectionTypes.Empty;
            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Triangle3(ref Ray ray, ref FVector3 v0, ref FVector3 v1, ref FVector3 v2,
            out IntersectionTypes intersectionType) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestRay3Triangle3(ref ray, ref triangle2, out intersectionType);
        }

        /// <summary>
        /// Tests if a ray intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Triangle3(ref Ray ray, FVector3 v0, FVector3 v1, FVector3 v2,
            out IntersectionTypes intersectionType) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestRay3Triangle3(ref ray, ref triangle2, out intersectionType);
        }

        /// <summary>
        /// Tests if a ray intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Triangle3(ref Ray ray, ref Triangle triangle) {
            return TestRay3Triangle3(ref ray, ref triangle, out _);
        }

        /// <summary>
        /// Tests if a ray intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Triangle3(ref Ray ray, ref FVector3 v0, ref FVector3 v1, ref FVector3 v2) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestRay3Triangle3(ref ray, ref triangle2, out _);
        }

        /// <summary>
        /// Tests if a ray intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestRay3Triangle3(ref Ray ray, FVector3 v0, FVector3 v1, FVector3 v2) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestRay3Triangle3(ref ray, ref triangle2, out _);
        }

        /// <summary>
        /// Tests if a ray intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Triangle3(ref Ray ray, ref Triangle triangle, out Ray3Triangle3Intr info) {
            FVector3 vector = ray.origin - triangle.v0;
            FVector3 vector2 = triangle.v1 - triangle.v0;
            FVector3 value = triangle.v2 - triangle.v0;
            FVector3 value2 = vector2.Cross(value);
            FLOAT num = ray.direction.Dot(value2);
            FLOAT num2;
            if (num > _dotThreshold) {
                num2 = 1f;
            }
            else {
                if (!(num < 0f - _dotThreshold)) {
                    info = default(Ray3Triangle3Intr);
                    return false;
                }

                num2 = -1f;
                num = 0f - num;
            }

            FLOAT num3 = num2 * ray.direction.Dot(vector.Cross(value));
            if (num3 >= -1E-05f) {
                FLOAT num4 = num2 * ray.direction.Dot(vector2.Cross(vector));
                if (num4 >= -1E-05f && num3 + num4 <= num + 1E-05f) {
                    FLOAT num5 = (0f - num2) * vector.Dot(value2);
                    if (num5 >= 0f - _intervalThreshold) {
                        FLOAT num6 = 1f / num;
                        info.IntersectionType = IntersectionTypes.Point;
                        info.RayParameter = num5 * num6;
                        info.Point = ray.Eval(info.RayParameter);
                        info.TriBary1 = num3 * num6;
                        info.TriBary2 = num4 * num6;
                        info.TriBary0 = 1f - info.TriBary1 - info.TriBary2;
                        return true;
                    }
                }
            }

            info = default(Ray3Triangle3Intr);
            return false;
        }

        /// <summary>
        /// Tests if a ray intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Triangle3(ref Ray ray, ref FVector3 v0, ref FVector3 v1, ref FVector3 v2,
            out Ray3Triangle3Intr info) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return FindRay3Triangle3(ref ray, ref triangle2, out info);
        }

        /// <summary>
        /// Tests if a ray intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindRay3Triangle3(ref Ray ray, FVector3 v0, FVector3 v1, FVector3 v2,
            out Ray3Triangle3Intr info) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return FindRay3Triangle3(ref ray, ref triangle2, out info);
        }

        /// <summary>
        /// Tests if a segment intersects an axis aligned box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3AABB3(ref Segment segment, ref AABB box) {
            box.CalcCenterExtents(out var center, out var extents);
            FVector3 value = segment.center - center;
            FLOAT num = FMath.Abs(segment.direction.x);
            FLOAT num2 = FMath.Abs(value.x);
            FLOAT num3 = extents.x + segment.extent * num;
            if (num2 > num3) {
                return false;
            }

            FLOAT num4 = FMath.Abs(segment.direction.y);
            FLOAT num5 = FMath.Abs(value.y);
            num3 = extents.y + segment.extent * num4;
            if (num5 > num3) {
                return false;
            }

            FLOAT num6 = FMath.Abs(segment.direction.z);
            FLOAT num7 = FMath.Abs(value.z);
            num3 = extents.z + segment.extent * num6;
            if (num7 > num3) {
                return false;
            }

            FVector3 vector = segment.direction.Cross(value);
            FLOAT num8 = FMath.Abs(vector.x);
            num3 = extents.y * num6 + extents.z * num4;
            if (num8 > num3) {
                return false;
            }

            FLOAT num9 = FMath.Abs(vector.y);
            num3 = extents.x * num6 + extents.z * num;
            if (num9 > num3) {
                return false;
            }

            FLOAT num10 = FMath.Abs(vector.z);
            num3 = extents.x * num4 + extents.y * num;
            if (num10 > num3) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a segment intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3AABB3(ref Segment segment, ref AABB box, out Segment3AAB3Intr info) {
            return DoClipping(0f - segment.extent, segment.extent, ref segment.center, ref segment.direction, ref box,
                solid: true, out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests if a segment intersects a box. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Box3(ref Segment segment, ref Box box) {
            FVector3 vector = segment.center - box.center;
            FLOAT num = FMath.Abs(segment.direction.Dot(box.axis0));
            FLOAT num2 = FMath.Abs(vector.Dot(box.axis0));
            FLOAT num3 = box.extents.x + segment.extent * num;
            if (num2 > num3) {
                return false;
            }

            FLOAT num4 = FMath.Abs(segment.direction.Dot(box.axis1));
            FLOAT num5 = FMath.Abs(vector.Dot(box.axis1));
            num3 = box.extents.y + segment.extent * num4;
            if (num5 > num3) {
                return false;
            }

            FLOAT num6 = FMath.Abs(segment.direction.Dot(box.axis2));
            FLOAT num7 = FMath.Abs(vector.Dot(box.axis2));
            num3 = box.extents.z + segment.extent * num6;
            if (num7 > num3) {
                return false;
            }

            FVector3 vector2 = segment.direction.Cross(vector);
            FLOAT num8 = FMath.Abs(vector2.Dot(box.axis0));
            num3 = box.extents.y * num6 + box.extents.z * num4;
            if (num8 > num3) {
                return false;
            }

            FLOAT num9 = FMath.Abs(vector2.Dot(box.axis1));
            num3 = box.extents.x * num6 + box.extents.z * num;
            if (num9 > num3) {
                return false;
            }

            FLOAT num10 = FMath.Abs(vector2.Dot(box.axis2));
            num3 = box.extents.x * num4 + box.extents.y * num;
            if (num10 > num3) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a segment intersects a box and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Box3(ref Segment segment, ref Box box, out Segment3Box3Intr info) {
            return DoClipping(0f - segment.extent, segment.extent, ref segment.center, ref segment.direction, ref box,
                solid: true, out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
        }

        /// <summary>
        /// Tests if a segment intersects a solid circle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Circle3(ref Segment segment, ref Circle circle) {
            return FindSegment3Circle3(ref segment, ref circle, out _);
        }

        /// <summary>
        /// Tests if a segment intersects a solid circle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Circle3(ref Segment segment, ref Circle circle, out Segment3Circle3Intr info) {
            FLOAT num = segment.direction.Dot(circle.normal);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = circle.normal.Dot(segment.center - circle.center);
                FLOAT num3 = (0f - num2) / num;
                if (FMath.Abs(num3) <= segment.extent + _intervalThreshold) {
                    FVector3 vector = segment.center + num3 * segment.direction;
                    if ((vector - circle.center).sqrMagnitude <= circle.radius * circle.radius) {
                        info.IntersectionType = IntersectionTypes.Point;
                        info.Point = vector;
                        return true;
                    }
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a segment intersects a plane. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Plane3(ref Segment segment, ref Plane plane,
            out IntersectionTypes intersectionType) {
            FVector3 point = segment.p0;
            FLOAT num = plane.SignedDistanceTo(ref point);
            if (FMath.Abs(num) <= _distanceThreshold) {
                num = 0f;
            }

            FVector3 point2 = segment.p1;
            FLOAT num2 = plane.SignedDistanceTo(ref point2);
            if (FMath.Abs(num2) <= _distanceThreshold) {
                num2 = 0f;
            }

            FLOAT num3 = num * num2;
            if (num3 < 0f) {
                intersectionType = IntersectionTypes.Point;
                return true;
            }

            if (num3 > 0f) {
                intersectionType = IntersectionTypes.Empty;
                return false;
            }

            if (num != 0f || num2 != 0f) {
                intersectionType = IntersectionTypes.Point;
                return true;
            }

            intersectionType = IntersectionTypes.Segment;
            return true;
        }

        /// <summary>
        /// Tests if a segment intersects a plane. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Plane3(ref Segment segment, ref Plane plane) {
            return TestSegment3Plane3(ref segment, ref plane, out _);
        }

        /// <summary>
        /// Tests if a segment intersects a plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Plane3(ref Segment segment, ref Plane plane, out Segment3Plane3Intr info) {
            FLOAT num = segment.direction.Dot(plane.normal);
            FLOAT num2 = plane.SignedDistanceTo(ref segment.center);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num3 = (0f - num2) / num;
                if (FMath.Abs(num3) <= segment.extent + _intervalThreshold) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point = segment.center + num3 * segment.direction;
                    info.SegmentParameter = (num3 + segment.extent) / (segment.extent * 2f);
                    return true;
                }

                info.IntersectionType = IntersectionTypes.Empty;
                info.Point = FVector3.Zero;
                info.SegmentParameter = 0f;
                return false;
            }

            if (FMath.Abs(num2) <= _distanceThreshold) {
                info.IntersectionType = IntersectionTypes.Segment;
                info.Point = FVector3.Zero;
                info.SegmentParameter = 0f;
                return true;
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            info.SegmentParameter = 0f;
            return false;
        }

        /// <summary>
        /// Tests if a segment intersects a solid polygon. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Polygon3(ref Segment segment, Polygon polygon) {
            return FindSegment3Polygon3(ref segment, polygon, out _);
        }

        /// <summary>
        /// Tests if a segment intersects a solid polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Polygon3(ref Segment segment, Polygon polygon, out Segment3Polygon3Intr info) {
            Plane plane = polygon.Plane;
            FLOAT num = segment.direction.Dot(plane.normal);
            FLOAT num2 = plane.SignedDistanceTo(ref segment.center);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num3 = (0f - num2) / num;
                if (!(FMath.Abs(num3) <= segment.extent + _intervalThreshold)) {
                    info.IntersectionType = IntersectionTypes.Empty;
                    info.Point = FVector3.Zero;
                    return false;
                }

                FVector3 vector = segment.center + num3 * segment.direction;
                ProjectionPlanes projectionPlane = plane.normal.GetProjectionPlane();
                Polygon2D polygon2D = Polygon2D.CreateProjected(polygon, projectionPlane);
                FVector2 point = vector.ToFVector2(projectionPlane);
                if (polygon2D.ContainsSimple(point)) {
                    info.IntersectionType = IntersectionTypes.Point;
                    info.Point = vector;
                    return true;
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a segment intersects a solid rectangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Rectangle3(ref Segment segment, ref Rectangle rectangle) {
            return FindSegment3Rectangle3(ref segment, ref rectangle, out _);
        }

        /// <summary>
        /// Tests if a segment intersects a solid rectangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Rectangle3(ref Segment segment, ref Rectangle rectangle,
            out Segment3Rectangle3Intr info) {
            FLOAT num = segment.direction.Dot(rectangle.normal);
            if (FMath.Abs(num) > _dotThreshold) {
                FLOAT num2 = rectangle.normal.Dot(segment.center - rectangle.center);
                FLOAT num3 = (0f - num2) / num;
                if (FMath.Abs(num3) <= segment.extent + _intervalThreshold) {
                    FVector3 point = segment.center + num3 * segment.direction;
                    if (Point3InsideRectangle3(ref point, ref rectangle)) {
                        info.IntersectionType = IntersectionTypes.Point;
                        info.Point = point;
                        return true;
                    }
                }
            }

            info.IntersectionType = IntersectionTypes.Empty;
            info.Point = FVector3.Zero;
            return false;
        }

        /// <summary>
        /// Tests if a segment intersects a sphere. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Sphere3(ref Segment segment, ref Sphere sphere) {
            FVector3 vector = segment.center - sphere.center;
            FLOAT num = vector.Dot(vector) - sphere.radius * sphere.radius;
            FLOAT num2 = segment.direction.Dot(vector);
            FLOAT num3 = num2 * num2 - num;
            if (num3 < 0f) {
                return false;
            }

            FLOAT num4 = segment.extent * segment.extent + num;
            FLOAT num5 = 2f * num2 * segment.extent;
            FLOAT num6 = num4 - num5;
            FLOAT num7 = num4 + num5;
            if (num6 * num7 <= 0f) {
                return true;
            }

            if (num6 > 0f) {
                return FMath.Abs(num2) < segment.extent;
            }

            return false;
        }

        /// <summary>
        /// Tests if a segment intersects a sphere and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Sphere3(ref Segment segment, ref Sphere sphere, out Segment3Sphere3Intr info) {
            FVector3 vector = segment.center - sphere.center;
            FLOAT num = vector.Dot(vector) - sphere.radius * sphere.radius;
            FLOAT num2 = segment.direction.Dot(vector);
            FLOAT num3 = num2 * num2 - num;
            if (num3 < 0f) {
                info = default(Segment3Sphere3Intr);
                return false;
            }

            FLOAT num4 = segment.extent * segment.extent + num;
            FLOAT num5 = 2f * num2 * segment.extent;
            FLOAT num6 = num4 - num5;
            FLOAT num7 = num4 + num5;
            if (num6 * num7 <= 0f) {
                FLOAT num8 = FMath.Sqrt(num3);
                info.SegmentParameter0 = ((num6 > 0f) ? (0f - num2 - num8) : (0f - num2 + num8));
                info.SegmentParameter1 = 0f;
                info.Point0 = segment.center + info.SegmentParameter0 * segment.direction;
                info.Point1 = FVector3.Zero;
                info.SegmentParameter0 = (info.SegmentParameter0 + segment.extent) / (2f * segment.extent);
                info.Quantity = 1;
                info.IntersectionType = IntersectionTypes.Point;
                return true;
            }

            if (num6 > 0f && FMath.Abs(num2) < segment.extent) {
                if (num3 >= 1E-05f) {
                    FLOAT num8 = FMath.Sqrt(num3);
                    info.SegmentParameter0 = 0f - num2 - num8;
                    info.SegmentParameter1 = 0f - num2 + num8;
                    info.Point0 = segment.center + info.SegmentParameter0 * segment.direction;
                    info.Point1 = segment.center + info.SegmentParameter1 * segment.direction;
                    info.SegmentParameter0 = (info.SegmentParameter0 + segment.extent) / (2f * segment.extent);
                    info.SegmentParameter1 = (info.SegmentParameter1 + segment.extent) / (2f * segment.extent);
                    info.Quantity = 2;
                    info.IntersectionType = IntersectionTypes.Segment;
                }
                else {
                    info.SegmentParameter0 = 0f - num2;
                    info.SegmentParameter1 = 0f;
                    info.Point0 = segment.center + info.SegmentParameter0 * segment.direction;
                    info.Point1 = FVector3.Zero;
                    info.SegmentParameter0 = (info.SegmentParameter0 + segment.extent) / (2f * segment.extent);
                    info.Quantity = 1;
                    info.IntersectionType = IntersectionTypes.Point;
                }
            }
            else {
                info = default(Segment3Sphere3Intr);
            }

            return info.Quantity > 0;
        }

        /// <summary>
        /// Tests if a segment intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Triangle3(ref Segment segment, ref Triangle triangle,
            out IntersectionTypes intersectionType) {
            FVector3 vector = segment.center - triangle.v0;
            FVector3 vector2 = triangle.v1 - triangle.v0;
            FVector3 value = triangle.v2 - triangle.v0;
            FVector3 value2 = vector2.Cross(value);
            FLOAT num = segment.direction.Dot(value2);
            FLOAT num2;
            if (num > _dotThreshold) {
                num2 = 1f;
            }
            else {
                if (!(num < 0f - _dotThreshold)) {
                    intersectionType = IntersectionTypes.Empty;
                    return false;
                }

                num2 = -1f;
                num = 0f - num;
            }

            FLOAT num3 = num2 * segment.direction.Dot(vector.Cross(value));
            if (num3 >= -1E-05f) {
                FLOAT num4 = num2 * segment.direction.Dot(vector2.Cross(vector));
                if (num4 >= -1E-05f && num3 + num4 <= num + 1E-05f) {
                    FLOAT num5 = (0f - num2) * vector.Dot(value2);
                    FLOAT num6 = segment.extent * num;
                    if (0f - num6 - 1E-05f <= num5 && num5 <= num6 + 1E-05f) {
                        intersectionType = IntersectionTypes.Point;
                        return true;
                    }
                }
            }

            intersectionType = IntersectionTypes.Empty;
            return false;
        }

        /// <summary>
        /// Tests if a segment intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Triangle3(ref Segment segment, ref FVector3 v0, ref FVector3 v1,
            ref FVector3 v2, out IntersectionTypes intersectionType) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestSegment3Triangle3(ref segment, ref triangle2, out intersectionType);
        }

        /// <summary>
        /// Tests if a segment intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Triangle3(ref Segment segment, FVector3 v0, FVector3 v1, FVector3 v2,
            out IntersectionTypes intersectionType) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestSegment3Triangle3(ref segment, ref triangle2, out intersectionType);
        }

        /// <summary>
        /// Tests if a segment intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Triangle3(ref Segment segment, ref Triangle triangle) {
            return TestSegment3Triangle3(ref segment, ref triangle, out _);
        }

        /// <summary>
        /// Tests if a segment intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Triangle3(ref Segment segment, ref FVector3 v0, ref FVector3 v1,
            ref FVector3 v2) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestSegment3Triangle3(ref segment, ref triangle2, out _);
        }

        /// <summary>
        /// Tests if a segment intersects a triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSegment3Triangle3(ref Segment segment, FVector3 v0, FVector3 v1, FVector3 v2) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return TestSegment3Triangle3(ref segment, ref triangle2, out _);
        }

        /// <summary>
        /// Tests if a segment intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Triangle3(ref Segment segment, ref Triangle triangle,
            out Segment3Triangle3Intr info) {
            FVector3 vector = segment.center - triangle.v0;
            FVector3 vector2 = triangle.v1 - triangle.v0;
            FVector3 value = triangle.v2 - triangle.v0;
            FVector3 value2 = vector2.Cross(value);
            FLOAT num = segment.direction.Dot(value2);
            FLOAT num2;
            if (num > _dotThreshold) {
                num2 = 1f;
            }
            else {
                if (!(num < 0f - _dotThreshold)) {
                    info = default(Segment3Triangle3Intr);
                    return false;
                }

                num2 = -1f;
                num = 0f - num;
            }

            FLOAT num3 = num2 * segment.direction.Dot(vector.Cross(value));
            if (num3 >= -1E-05f) {
                FLOAT num4 = num2 * segment.direction.Dot(vector2.Cross(vector));
                if (num4 >= -1E-05f && num3 + num4 <= num + 1E-05f) {
                    FLOAT num5 = (0f - num2) * vector.Dot(value2);
                    FLOAT num6 = segment.extent * num;
                    if (0f - num6 - 1E-05f <= num5 && num5 <= num6 + 1E-05f) {
                        FLOAT num7 = 1f / num;
                        info.IntersectionType = IntersectionTypes.Point;
                        info.SegmentParameter = num5 * num7;
                        info.Point = segment.center + info.SegmentParameter * segment.direction;
                        info.SegmentParameter = (info.SegmentParameter + segment.extent) / (2f * segment.extent);
                        info.TriBary1 = num3 * num7;
                        info.TriBary2 = num4 * num7;
                        info.TriBary0 = 1f - info.TriBary1 - info.TriBary2;
                        return true;
                    }
                }
            }

            info = default(Segment3Triangle3Intr);
            return false;
        }

        /// <summary>
        /// Tests if a segment intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Triangle3(ref Segment segment, ref FVector3 v0, ref FVector3 v1,
            ref FVector3 v2, out Segment3Triangle3Intr info) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return FindSegment3Triangle3(ref segment, ref triangle2, out info);
        }

        /// <summary>
        /// Tests if a segment intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSegment3Triangle3(ref Segment segment, FVector3 v0, FVector3 v1, FVector3 v2,
            out Segment3Triangle3Intr info) {
            Triangle triangle = default(Triangle);
            triangle.v0 = v0;
            triangle.v1 = v1;
            triangle.v2 = v2;
            Triangle triangle2 = triangle;
            return FindSegment3Triangle3(ref segment, ref triangle2, out info);
        }

        /// <summary>
        /// Tests if a sphere intersects another sphere. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestSphere3Sphere3(ref Sphere sphere0, ref Sphere sphere1) {
            FVector3 vector = sphere1.center - sphere0.center;
            FLOAT num = sphere0.radius + sphere1.radius;
            return vector.sqrMagnitude <= num * num;
        }

        /// <summary>
        /// Tests if a sphere intersects another sphere and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindSphere3Sphere3(ref Sphere sphere0, ref Sphere sphere1, out Sphere3Sphere3Intr info) {
            FVector3 normal = sphere1.center - sphere0.center;
            FLOAT sqrMagnitude = normal.sqrMagnitude;
            FLOAT radius = sphere0.radius;
            FLOAT radius2 = sphere1.radius;
            FLOAT num = radius - radius2;
            if (normal.sqrMagnitude < 9.99999944E-11f && FMath.Abs(num) < 1E-05f) {
                info.IntersectionType = Sphere3Sphere3IntrTypes.Same;
                info.ContactPoint = FVector3.Zero;
                info.Circle = default(Circle);
                return true;
            }

            FLOAT num2 = radius + radius2;
            FLOAT num3 = num2 * num2;
            if (sqrMagnitude > num3) {
                info = default(Sphere3Sphere3Intr);
                return false;
            }

            if (sqrMagnitude == num3) {
                normal.Normalize();
                info.ContactPoint = sphere0.center + radius * normal;
                info.Circle = default(Circle);
                info.IntersectionType = Sphere3Sphere3IntrTypes.Point;
                return true;
            }

            FLOAT num4 = num * num;
            if (sqrMagnitude < num4) {
                normal.Normalize();
                info.ContactPoint = 0.5f * (sphere0.center + sphere1.center);
                info.Circle = default(Circle);
                info.IntersectionType =
                    ((num <= 0f) ? Sphere3Sphere3IntrTypes.Sphere0 : Sphere3Sphere3IntrTypes.Sphere1);
                return true;
            }

            if (sqrMagnitude == num4) {
                normal.Normalize();
                if (num <= 0f) {
                    info.IntersectionType = Sphere3Sphere3IntrTypes.Sphere0Point;
                    info.ContactPoint = sphere1.center + radius2 * normal;
                }
                else {
                    info.IntersectionType = Sphere3Sphere3IntrTypes.Sphere1Point;
                    info.ContactPoint = sphere0.center + radius * normal;
                }

                info.Circle = default(Circle);
                return true;
            }

            FLOAT num5 = 0.5f * (1f + num * num2 / sqrMagnitude);
            FVector3 center = sphere0.center + num5 * normal;
            FLOAT radius3 = FMath.Sqrt(FMath.Abs(radius * radius - num5 * num5 * sqrMagnitude));
            normal.Normalize();
            info.Circle = new Circle(ref center, ref normal, radius3);
            info.IntersectionType = Sphere3Sphere3IntrTypes.Circle;
            info.ContactPoint = FVector3.Zero;
            return true;
        }

        private static void ProjectOntoAxis(ref Triangle triangle, ref FVector3 axis, out FLOAT fMin, out FLOAT fMax) {
            FLOAT num = axis.Dot(triangle.v0);
            FLOAT num2 = axis.Dot(triangle.v1);
            FLOAT num3 = axis.Dot(triangle.v2);
            fMin = num;
            fMax = fMin;
            if (num2 < fMin) {
                fMin = num2;
            }
            else if (num2 > fMax) {
                fMax = num2;
            }

            if (num3 < fMin) {
                fMin = num3;
            }
            else if (num3 > fMax) {
                fMax = num3;
            }
        }

        private static void TrianglePlaneRelations(ref Triangle triangle, ref Plane plane, out FLOAT dist0,
            out FLOAT dist1, out FLOAT dist2, out int sign0, out int sign1, out int sign2, out int positive,
            out int negative, out int zero) {
            positive = 0;
            negative = 0;
            zero = 0;
            dist0 = plane.SignedDistanceTo(ref triangle.v0);
            if (dist0 > 1E-05f) {
                sign0 = 1;
                positive++;
            }
            else if (dist0 < -1E-05f) {
                sign0 = -1;
                negative++;
            }
            else {
                dist0 = 0f;
                sign0 = 0;
                zero++;
            }

            dist1 = plane.SignedDistanceTo(ref triangle.v1);
            if (dist1 > 1E-05f) {
                sign1 = 1;
                positive++;
            }
            else if (dist1 < -1E-05f) {
                sign1 = -1;
                negative++;
            }
            else {
                dist1 = 0f;
                sign1 = 0;
                zero++;
            }

            dist2 = plane.SignedDistanceTo(ref triangle.v2);
            if (dist2 > 1E-05f) {
                sign2 = 1;
                positive++;
            }
            else if (dist2 < -1E-05f) {
                sign2 = -1;
                negative++;
            }
            else {
                dist2 = 0f;
                sign2 = 0;
                zero++;
            }
        }

        private static bool TrianglePlaneRelationsQuick(ref Triangle triangle, ref Plane plane) {
            FLOAT num = plane.SignedDistanceTo(ref triangle.v0);
            int num2;
            if (num > 1E-05f) {
                num2 = 1;
            }
            else {
                if (!(num < -1E-05f)) {
                    return false;
                }

                num2 = -1;
            }

            num = plane.SignedDistanceTo(ref triangle.v1);
            if (num > 1E-05f) {
                if (num2 == -1) {
                    return false;
                }
            }
            else {
                if (!(num < -1E-05f)) {
                    return false;
                }

                if (num2 == 1) {
                    return false;
                }
            }

            num = plane.SignedDistanceTo(ref triangle.v2);
            if (num > 1E-05f) {
                if (num2 == -1) {
                    return false;
                }
            }
            else {
                if (!(num < -1E-05f)) {
                    return false;
                }

                if (num2 == 1) {
                    return false;
                }
            }

            return true;
        }

        private static bool IntersectsSegment(ref Plane plane, ref Triangle triangle, ref FVector3 end0,
            ref FVector3 end1, bool grazing, out Triangle3Triangle3Intr info) {
            int num = 0;
            FLOAT num2 = FMath.Abs(plane.normal.x);
            FLOAT num3 = FMath.Abs(plane.normal.y);
            if (num3 > num2) {
                num = 1;
                num2 = num3;
            }

            num3 = FMath.Abs(plane.normal.z);
            if (num3 > num2) {
                num = 2;
            }

            Triangle2D triangle2D = default(Triangle2D);
            FVector2 p = default(FVector2);
            FVector2 p2 = default(FVector2);
            switch (num) {
                case 0:
                    triangle2D.v0.x = triangle.v0.y;
                    triangle2D.v0.y = triangle.v0.z;
                    triangle2D.v1.x = triangle.v1.y;
                    triangle2D.v1.y = triangle.v1.z;
                    triangle2D.v2.x = triangle.v2.y;
                    triangle2D.v2.y = triangle.v2.z;
                    p.x = end0.y;
                    p.y = end0.z;
                    p2.x = end1.y;
                    p2.y = end1.z;
                    break;
                case 1:
                    triangle2D.v0.x = triangle.v0.x;
                    triangle2D.v0.y = triangle.v0.z;
                    triangle2D.v1.x = triangle.v1.x;
                    triangle2D.v1.y = triangle.v1.z;
                    triangle2D.v2.x = triangle.v2.x;
                    triangle2D.v2.y = triangle.v2.z;
                    p.x = end0.x;
                    p.y = end0.z;
                    p2.x = end1.x;
                    p2.y = end1.z;
                    break;
                default:
                    triangle2D.v0.x = triangle.v0.x;
                    triangle2D.v0.y = triangle.v0.y;
                    triangle2D.v1.x = triangle.v1.x;
                    triangle2D.v1.y = triangle.v1.y;
                    triangle2D.v2.x = triangle.v2.x;
                    triangle2D.v2.y = triangle.v2.y;
                    p.x = end0.x;
                    p.y = end0.y;
                    p2.x = end1.x;
                    p2.y = end1.y;
                    break;
            }

            Segment2D segment = new Segment2D(ref p, ref p2);
            if (!FindSegment2Triangle2(ref segment, ref triangle2D, out var info2)) {
                info = default(Triangle3Triangle3Intr);
                return false;
            }

            FVector2 point;
            FVector2 vector;
            if (info2.IntersectionType == IntersectionTypes.Segment) {
                info.IntersectionType = IntersectionTypes.Segment;
                info.Touching = grazing;
                info.Quantity = 2;
                point = info2.Point0;
                vector = info2.Point1;
            }
            else {
                info.IntersectionType = IntersectionTypes.Point;
                info.Touching = true;
                info.Quantity = 1;
                point = info2.Point0;
                vector = FVector2.Zero;
            }

            switch (num) {
                case 0: {
                    FLOAT num6 = 1f / plane.normal.x;
                    info.Point0 =
                        new FVector3(num6 * (plane.constant - plane.normal.y * point.x - plane.normal.z * point.y),
                            point.x, point.y);
                    info.Point1 = ((info.Quantity == 2)
                        ? new FVector3(num6 * (plane.constant - plane.normal.y * vector.x - plane.normal.z * vector.y), vector.x,
                            vector.y)
                        : FVector3.Zero);
                    break;
                }
                case 1: {
                    FLOAT num5 = 1f / plane.normal.y;
                    info.Point0 = new FVector3(point.x,
                        num5 * (plane.constant - plane.normal.x * point.x - plane.normal.z * point.y), point.y);
                    info.Point1 = ((info.Quantity == 2)
                        ? new FVector3(vector.x,
                            num5 * (plane.constant - plane.normal.x * vector.x - plane.normal.z * vector.y), vector.y)
                        : FVector3.Zero);
                    break;
                }
                default: {
                    FLOAT num4 = 1f / plane.normal.z;
                    info.Point0 = new FVector3(point.x, point.y,
                        num4 * (plane.constant - plane.normal.x * point.x - plane.normal.y * point.y));
                    info.Point1 = ((info.Quantity == 2)
                        ? new FVector3(vector.x, vector.y,
                            num4 * (plane.constant - plane.normal.x * vector.x - plane.normal.y * vector.y))
                        : FVector3.Zero);
                    break;
                }
            }

            info.CoplanarIntersectionType = IntersectionTypes.Empty;
            info.Point2 = FVector3.Zero;
            info.Point3 = FVector3.Zero;
            info.Point4 = FVector3.Zero;
            info.Point5 = FVector3.Zero;
            return true;
        }

        private static int QueryToLine(ref FVector2 test, ref FVector2 vec0, ref FVector2 vec1) {
            FLOAT num = test.x - vec0.x;
            FLOAT num2 = test.y - vec0.y;
            FLOAT num3 = vec1.x - vec0.x;
            FLOAT num4 = vec1.y - vec0.y;
            FLOAT num5 = num * num4 - num3 * num2;
            if (!(num5 > 1E-05f)) {
                if (!(num5 < -1E-05f)) {
                    return 0;
                }

                return -1;
            }

            return 1;
        }

        private static int QueryToTriangle(ref FVector2 test, ref FVector2 v0, ref FVector2 v1,
            ref FVector2 v2) {
            int num = QueryToLine(ref test, ref v1, ref v2);
            if (num > 0) {
                return 1;
            }

            int num2 = QueryToLine(ref test, ref v0, ref v2);
            if (num2 < 0) {
                return 1;
            }

            int num3 = QueryToLine(ref test, ref v0, ref v1);
            if (num3 > 0) {
                return 1;
            }

            if (num == 0 || num2 == 0 || num3 == 0) {
                return 0;
            }

            return -1;
        }

        private static bool ContainsPoint(ref Triangle triangle, ref Plane plane, ref FVector3 point,
            out Triangle3Triangle3Intr info) {
            FVector3Ex.CreateOrthonormalBasis(out var u, out var v, ref plane.normal);
            FVector3 value = point - triangle.v0;
            FVector3 value2 = triangle.v1 - triangle.v0;
            FVector3 value3 = triangle.v2 - triangle.v0;
            FVector2 test = new FVector2(u.Dot(value), v.Dot(value));
            FVector2 v2 = FVector2.Zero;
            FVector2 v3 = new FVector2(u.Dot(value2), v.Dot(value2));
            FVector2 v4 = new FVector2(u.Dot(value3), v.Dot(value3));
            int num = QueryToTriangle(ref test, ref v2, ref v3, ref v4);
            if (num <= 0) {
                info.IntersectionType = IntersectionTypes.Point;
                info.CoplanarIntersectionType = IntersectionTypes.Empty;
                info.Touching = true;
                info.Quantity = 1;
                info.Point0 = point;
                info.Point1 = FVector3.Zero;
                info.Point2 = FVector3.Zero;
                info.Point3 = FVector3.Zero;
                info.Point4 = FVector3.Zero;
                info.Point5 = FVector3.Zero;
                return true;
            }

            info = default(Triangle3Triangle3Intr);
            return false;
        }

        private static bool GetCoplanarIntersection(ref Plane plane, ref Triangle tri0, ref Triangle tri1,
            out Triangle3Triangle3Intr info) {
            int num = 0;
            FLOAT num2 = FMath.Abs(plane.normal.x);
            FLOAT num3 = FMath.Abs(plane.normal.y);
            if (num3 > num2) {
                num = 1;
                num2 = num3;
            }

            num3 = FMath.Abs(plane.normal.z);
            if (num3 > num2) {
                num = 2;
            }

            Triangle2D triangle = default(Triangle2D);
            Triangle2D triangle2D = default(Triangle2D);
            switch (num) {
                case 0:
                    triangle.v0.x = tri0.v0.y;
                    triangle.v0.y = tri0.v0.z;
                    triangle2D.v0.x = tri1.v0.y;
                    triangle2D.v0.y = tri1.v0.z;
                    triangle.v1.x = tri0.v1.y;
                    triangle.v1.y = tri0.v1.z;
                    triangle2D.v1.x = tri1.v1.y;
                    triangle2D.v1.y = tri1.v1.z;
                    triangle.v2.x = tri0.v2.y;
                    triangle.v2.y = tri0.v2.z;
                    triangle2D.v2.x = tri1.v2.y;
                    triangle2D.v2.y = tri1.v2.z;
                    break;
                case 1:
                    triangle.v0.x = tri0.v0.x;
                    triangle.v0.y = tri0.v0.z;
                    triangle2D.v0.x = tri1.v0.x;
                    triangle2D.v0.y = tri1.v0.z;
                    triangle.v1.x = tri0.v1.x;
                    triangle.v1.y = tri0.v1.z;
                    triangle2D.v1.x = tri1.v1.x;
                    triangle2D.v1.y = tri1.v1.z;
                    triangle.v2.x = tri0.v2.x;
                    triangle.v2.y = tri0.v2.z;
                    triangle2D.v2.x = tri1.v2.x;
                    triangle2D.v2.y = tri1.v2.z;
                    break;
                default:
                    triangle.v0.x = tri0.v0.x;
                    triangle.v0.y = tri0.v0.y;
                    triangle2D.v0.x = tri1.v0.x;
                    triangle2D.v0.y = tri1.v0.y;
                    triangle.v1.x = tri0.v1.x;
                    triangle.v1.y = tri0.v1.y;
                    triangle2D.v1.x = tri1.v1.x;
                    triangle2D.v1.y = tri1.v1.y;
                    triangle.v2.x = tri0.v2.x;
                    triangle.v2.y = tri0.v2.y;
                    triangle2D.v2.x = tri1.v2.x;
                    triangle2D.v2.y = tri1.v2.y;
                    break;
            }

            FVector2 vector = triangle.v1 - triangle.v0;
            FVector2 value = triangle.v2 - triangle.v0;
            if (vector.DotPerpendicular(value) < 0f) {
                (triangle.v1, triangle.v2) = (triangle.v2, triangle.v1);
            }

            vector = triangle2D.v1 - triangle2D.v0;
            value = triangle2D.v2 - triangle2D.v0;
            if (vector.DotPerpendicular(value) < 0f) {
                (triangle2D.v1, triangle2D.v2) = (triangle2D.v2, triangle2D.v1);
            }

            Triangle2Triangle2Intr info2;
            bool flag = FindTriangle2Triangle2(ref triangle, ref triangle2D, out info2);
            info = default(Triangle3Triangle3Intr);
            if (!flag) {
                return false;
            }

            int num4 = (info.Quantity = info2.Quantity);
            switch (num) {
                case 0: {
                    FLOAT num6 = 1f / plane.normal.x;
                    for (int j = 0; j < num4; j++) {
                        info[j] = new FVector3(num6 * (plane.constant - plane.normal.y * info2[j].x - plane.normal.z * info2[j].y),
                            info2[j].x, info2[j].y);
                    }

                    break;
                }
                case 1: {
                    FLOAT num7 = 1f / plane.normal.y;
                    for (int k = 0; k < num4; k++) {
                        info[k] = new FVector3(info2[k].x,
                            num7 * (plane.constant - plane.normal.x * info2[k].x - plane.normal.z * info2[k].y),
                            info2[k].y);
                    }

                    break;
                }
                default: {
                    FLOAT num5 = 1f / plane.normal.z;
                    for (int i = 0; i < num4; i++) {
                        info[i] = new FVector3(info2[i].x, info2[i].y,
                            num5 * (plane.constant - plane.normal.x * info2[i].x - plane.normal.y * info2[i].y));
                    }

                    break;
                }
            }

            info.IntersectionType = IntersectionTypes.Plane;
            info.CoplanarIntersectionType = info2.IntersectionType;
            return true;
        }

        /// <summary>
        /// Tests if a triangle intersects another triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestTriangle3Triangle3(ref Triangle triangle0, ref Triangle triangle1,
            out IntersectionTypes intersectionType) {
            FVector3 vector = triangle0.v1 - triangle0.v0;
            FVector3 vector2 = triangle0.v2 - triangle0.v1;
            FVector3 vector3 = triangle0.v0 - triangle0.v2;
            FVector3 axis = vector.NormalizeCross(vector2);
            FLOAT num = axis.Dot(triangle0.v0);
            ProjectOntoAxis(ref triangle1, ref axis, out var fmin, out var fmax);
            if (num < fmin || num > fmax) {
                intersectionType = IntersectionTypes.Empty;
                return false;
            }

            FVector3 vector4 = triangle1.v1 - triangle1.v0;
            FVector3 value = triangle1.v2 - triangle1.v1;
            FVector3 value2 = triangle1.v0 - triangle1.v2;
            FVector3 axis2 = vector4.NormalizeCross(value);
            intersectionType = IntersectionTypes.Empty;
            FVector3 vector5 = axis.NormalizeCross(axis2);
            FLOAT fmin2;
            FLOAT fmax2;
            if (vector5.Dot(vector5) >= 1E-05f) {
                FLOAT num2 = axis2.Dot(triangle1.v0);
                ProjectOntoAxis(ref triangle0, ref axis2, out fmin2, out fmax2);
                if (num2 < fmin2 || num2 > fmax2) {
                    return false;
                }

                FVector3 axis3 = vector.NormalizeCross(vector4);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = vector.NormalizeCross(value);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = vector.NormalizeCross(value2);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = vector2.NormalizeCross(vector4);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = vector2.NormalizeCross(value);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = vector2.NormalizeCross(value2);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = vector3.NormalizeCross(vector4);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = vector3.NormalizeCross(value);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = vector3.NormalizeCross(value2);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                intersectionType = IntersectionTypes.Other;
            }
            else {
                FVector3 axis3 = axis.NormalizeCross(vector);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = axis.NormalizeCross(vector2);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = axis.NormalizeCross(vector3);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = axis2.NormalizeCross(vector4);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = axis2.NormalizeCross(value);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                axis3 = axis2.NormalizeCross(value2);
                ProjectOntoAxis(ref triangle0, ref axis3, out fmin2, out fmax2);
                ProjectOntoAxis(ref triangle1, ref axis3, out fmin, out fmax);
                if (fmax2 < fmin || fmax < fmin2) {
                    return false;
                }

                intersectionType = IntersectionTypes.Plane;
            }

            return true;
        }

        /// <summary>
        /// Tests if a triangle intersects another triangle. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool TestTriangle3Triangle3(ref Triangle triangle0, ref Triangle triangle1) {
            return TestTriangle3Triangle3(ref triangle0, ref triangle1, out _);
        }

        /// <summary>
        /// Tests if a triangle intersects another triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
        /// </summary>
        public static bool FindTriangle3Triangle3(ref Triangle triangle0, ref Triangle triangle1,
            out Triangle3Triangle3Intr info, bool reportCoplanarIntersections = false) {
            Plane plane = new Plane(ref triangle0.v0, ref triangle0.v1, ref triangle0.v2);
            TrianglePlaneRelations(ref triangle1, ref plane, out var dist, out var dist2, out var dist3, out var sign,
                out var sign2, out var sign3, out var positive, out var negative, out var zero);
            if (positive == 3 || negative == 3) {
                info = default(Triangle3Triangle3Intr);
                return false;
            }

            if (zero == 3) {
                if (reportCoplanarIntersections) {
                    return GetCoplanarIntersection(ref plane, ref triangle0, ref triangle1, out info);
                }

                info = default(Triangle3Triangle3Intr);
                return false;
            }

            if (positive == 0 || negative == 0) {
                if (zero == 2) {
                    if (sign != 0) {
                        return IntersectsSegment(ref plane, ref triangle0, ref triangle1.v2, ref triangle1.v1,
                            grazing: true, out info);
                    }

                    if (sign2 != 0) {
                        return IntersectsSegment(ref plane, ref triangle0, ref triangle1.v0, ref triangle1.v2,
                            grazing: true, out info);
                    }

                    if (sign3 != 0) {
                        return IntersectsSegment(ref plane, ref triangle0, ref triangle1.v1, ref triangle1.v0,
                            grazing: true, out info);
                    }
                }
                else {
                    if (sign == 0) {
                        return ContainsPoint(ref triangle0, ref plane, ref triangle1.v0, out info);
                    }

                    if (sign2 == 0) {
                        return ContainsPoint(ref triangle0, ref plane, ref triangle1.v1, out info);
                    }

                    if (sign3 == 0) {
                        return ContainsPoint(ref triangle0, ref plane, ref triangle1.v2, out info);
                    }
                }
            }

            Plane plane2 = new Plane(ref triangle1.v0, ref triangle1.v1, ref triangle1.v2);
            if (TrianglePlaneRelationsQuick(ref triangle0, ref plane2)) {
                info = default(Triangle3Triangle3Intr);
                return false;
            }

            if (zero == 0) {
                int num = ((positive == 1) ? 1 : (-1));
                if (sign == num) {
                    FLOAT num2 = dist / (dist - dist3);
                    FVector3 end = triangle1.v0 + num2 * (triangle1.v2 - triangle1.v0);
                    num2 = dist / (dist - dist2);
                    FVector3 end2 = triangle1.v0 + num2 * (triangle1.v1 - triangle1.v0);
                    return IntersectsSegment(ref plane, ref triangle0, ref end, ref end2, grazing: false, out info);
                }

                if (sign2 == num) {
                    FLOAT num2 = dist2 / (dist2 - dist);
                    FVector3 end = triangle1.v1 + num2 * (triangle1.v0 - triangle1.v1);
                    num2 = dist2 / (dist2 - dist3);
                    FVector3 end2 = triangle1.v1 + num2 * (triangle1.v2 - triangle1.v1);
                    return IntersectsSegment(ref plane, ref triangle0, ref end, ref end2, grazing: false, out info);
                }

                if (sign3 == num) {
                    FLOAT num2 = dist3 / (dist3 - dist2);
                    FVector3 end = triangle1.v2 + num2 * (triangle1.v1 - triangle1.v2);
                    num2 = dist3 / (dist3 - dist);
                    FVector3 end2 = triangle1.v2 + num2 * (triangle1.v0 - triangle1.v2);
                    return IntersectsSegment(ref plane, ref triangle0, ref end, ref end2, grazing: false, out info);
                }
            }
            else {
                if (sign == 0) {
                    FLOAT num2 = dist3 / (dist3 - dist2);
                    FVector3 end = triangle1.v2 + num2 * (triangle1.v1 - triangle1.v2);
                    return IntersectsSegment(ref plane, ref triangle0, ref triangle1.v0, ref end, grazing: false,
                        out info);
                }

                if (sign2 == 0) {
                    FLOAT num2 = dist / (dist - dist3);
                    FVector3 end = triangle1.v0 + num2 * (triangle1.v2 - triangle1.v0);
                    return IntersectsSegment(ref plane, ref triangle0, ref triangle1.v1, ref end, grazing: false,
                        out info);
                }

                if (sign3 == 0) {
                    FLOAT num2 = dist2 / (dist2 - dist);
                    FVector3 end = triangle1.v1 + num2 * (triangle1.v0 - triangle1.v1);
                    return IntersectsSegment(ref plane, ref triangle0, ref triangle1.v2, ref end, grazing: false,
                        out info);
                }
            }

            info = default(Triangle3Triangle3Intr);
            return false;
        }

        static Intersection() {
            _intervalThreshold = (_dotThreshold = (_distanceThreshold = 1E-05f));
        }

        /// <summary>
        /// Finds intersection of 1d intervals. Endpoints of the intervals must be sorted,
        /// i.e. seg0Start must be &lt;= seg0End, seg1Start must be &lt;= seg1End. Returns 0 if
        /// intersection is empty, 1 - if intervals intersect in one point, 2 - if intervals
        /// intersect in segment. w0 and w1 will contain intersection point in case intersection occurs.
        /// </summary>
        public static int FindSegment1Segment1(FLOAT seg0Start, FLOAT seg0End, FLOAT seg1Start, FLOAT seg1End, out FLOAT w0,
            out FLOAT w1) {
            w0 = (w1 = 0f);
            FLOAT distanceThreshold = _distanceThreshold;
            if (seg0End < seg1Start - distanceThreshold || seg0Start > seg1End + distanceThreshold) {
                return 0;
            }

            if (seg0End > seg1Start + distanceThreshold) {
                if (seg0Start < seg1End - distanceThreshold) {
                    if (seg0Start < seg1Start) {
                        w0 = seg1Start;
                    }
                    else {
                        w0 = seg0Start;
                    }

                    if (seg0End > seg1End) {
                        w1 = seg1End;
                    }
                    else {
                        w1 = seg0End;
                    }

                    if (w1 - w0 <= distanceThreshold) {
                        return 1;
                    }

                    return 2;
                }

                w0 = seg0Start;
                return 1;
            }

            w0 = seg0End;
            return 1;
        }
    }
}