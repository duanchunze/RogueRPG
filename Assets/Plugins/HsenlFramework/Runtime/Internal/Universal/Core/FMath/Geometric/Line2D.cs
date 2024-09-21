using System;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// The line is represented as P+t*D where P is the line origin, D is a
    /// unit-length direction vector, and t is any real number.  The user must
    /// ensure that D is indeed unit length.
    /// </summary>
    public struct Line2D {
        /// <summary>
        /// Line origin
        /// </summary>
        public Vector2 origin;

        /// <summary>
        /// Line direction. Must be unit length!
        /// </summary>
        public Vector2 direction;

        /// <summary>
        /// Creates the line
        /// </summary>
        /// <param name="origin">Line origin</param>
        /// <param name="direction">Line direction. Must be unit length!</param>
        public Line2D(ref Vector2 origin, ref Vector2 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Creates the line
        /// </summary>
        /// <param name="origin">Line origin</param>
        /// <param name="direction">Line direction. Must be unit length!</param>
        public Line2D(Vector2 origin, Vector2 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Creates the line. Origin is p0, Direction is Normalized(p1-p0).
        /// </summary>
        /// <param name="p0">First point</param>
        /// <param name="p1">Second point</param>
        public static Line2D CreateFromTwoPoints(ref Vector2 p0, ref Vector2 p1) {
            Line2D result = default(Line2D);
            result.origin = p0;
            result.direction = (p1 - p0).normalized;
            return result;
        }

        /// <summary>
        /// Creates the line. Origin is p0, Direction is Normalized(p1-p0).
        /// </summary>
        /// <param name="p0">First point</param>
        /// <param name="p1">Second point</param>
        public static Line2D CreateFromTwoPoints(Vector2 p0, Vector2 p1) {
            Line2D result = default(Line2D);
            result.origin = p0;
            result.direction = (p1 - p0).normalized;
            return result;
        }

        /// <summary>
        /// Creates the line which is perpendicular to given line and goes through given point.
        /// </summary>
        public static Line2D CreatePerpToLineTrhoughPoint(Line2D line, Vector2 point) {
            Line2D result = default(Line2D);
            result.origin = point;
            result.direction = line.direction.Perpendicular();
            return result;
        }

        /// <summary>
        /// Creates the line which is perpendicular to segment [point0,point1] and line origin goes through middle of the segment.
        /// </summary>
        public static Line2D CreateBetweenAndEquidistantToPoints(Vector2 point0, Vector2 point1) {
            Line2D result = default(Line2D);
            result.origin.x = (point0.x + point1.x) * 0.5f;
            result.origin.y = (point0.y + point1.y) * 0.5f;
            result.direction.x = point1.y - point0.y;
            result.direction.y = point0.x - point1.x;
            return result;
        }

        /// <summary>
        /// Creates the line which is parallel to given line on the specified distance from given line.
        /// </summary>
        public static Line2D CreateParallelToGivenLineAtGivenDistance(Line2D line, FLOAT distance) {
            Line2D result = default(Line2D);
            result.direction = line.direction;
            result.origin = line.origin + distance * new Vector2(line.direction.y, 0f - line.direction.x);
            return result;
        }

        /// <summary>
        /// Evaluates line using P+t*D formula, where P is the line origin, D is a
        /// unit-length direction vector, t is parameter.
        /// </summary>
        /// <param name="t">Evaluation parameter</param>
        public Vector2 Eval(FLOAT t) {
            return this.origin + this.direction * t;
        }

        /// <summary>
        /// Returns signed distance to a point. Where positive distance is on the right of the line,
        /// zero is on the line, negative on the left side of the line.
        /// </summary>
        public FLOAT SignedDistanceTo(Vector2 point) {
            return (point - this.origin).DotPerpendicular(this.direction);
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector2 point) {
            return Distance.Point2Line2(ref point, ref this);
        }

        /// <summary>
        /// Determines on which side of the line a point is. Returns +1 if a point
        /// is to the right of the line, 0 if it's on the line, -1 if it's on the left.
        /// </summary>
        public int QuerySide(Vector2 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = (point - this.origin).DotPerpendicular(this.direction);
            if (num < 0f - epsilon) {
                return -1;
            }

            if (num > epsilon) {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Determines on which side of the line a point is. Returns +1 if a point
        /// is to the right of the line, 0 if it's on the line, -1 if it's on the left.
        /// </summary>
        public int QuerySide(Vector2 point, FLOAT epsilon) {
            FLOAT num = (point - this.origin).DotPerpendicular(this.direction);
            if (num < 0f - epsilon) {
                return -1;
            }

            if (num > epsilon) {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Returns true if a point is on the negative side of the line, false otherwise.
        /// </summary>
        public bool QuerySideNegative(Vector2 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = (point - this.origin).DotPerpendicular(this.direction);
            return num <= epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the negative side of the line, false otherwise.
        /// </summary>
        public bool QuerySideNegative(Vector2 point, FLOAT epsilon) {
            FLOAT num = (point - this.origin).DotPerpendicular(this.direction);
            return num <= epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the positive side of the line, false otherwise.
        /// </summary>
        public bool QuerySidePositive(Vector2 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = (point - this.origin).DotPerpendicular(this.direction);
            return num >= 0f - epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the positive side of the line, false otherwise.
        /// </summary>
        public bool QuerySidePositive(Vector2 point, FLOAT epsilon) {
            FLOAT num = (point - this.origin).DotPerpendicular(this.direction);
            return num >= 0f - epsilon;
        }

        /// <summary>
        /// Determines on which side of the line a box is. Returns +1 if a box
        /// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
        /// </summary>
        public int QuerySide(ref Box2D box) {
            FLOAT epsilon = 1E-05f;
            FLOAT f = box.extents.x * box.axis0.DotPerpendicular(this.direction);
            FLOAT f2 = box.extents.y * box.axis1.DotPerpendicular(this.direction);
            FLOAT num = Math.Abs(f) + Math.Abs(f2);
            FLOAT num2 = (box.center - this.origin).DotPerpendicular(this.direction);
            if (!(num2 < 0f - num + epsilon)) {
                if (!(num2 > num - epsilon)) {
                    return 0;
                }

                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Determines on which side of the line a box is. Returns +1 if a box
        /// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
        /// </summary>
        public int QuerySide(ref Box2D box, FLOAT epsilon) {
            FLOAT f = box.extents.x * box.axis0.DotPerpendicular(this.direction);
            FLOAT f2 = box.extents.y * box.axis1.DotPerpendicular(this.direction);
            FLOAT num = Math.Abs(f) + Math.Abs(f2);
            FLOAT num2 = (box.center - this.origin).DotPerpendicular(this.direction);
            if (!(num2 < 0f - num + epsilon)) {
                if (!(num2 > num - epsilon)) {
                    return 0;
                }

                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Returns true if a box is on the negative side of the line, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref Box2D box) {
            FLOAT epsilon = 1E-05f;
            FLOAT f = box.extents.x * box.axis0.DotPerpendicular(this.direction);
            FLOAT f2 = box.extents.y * box.axis1.DotPerpendicular(this.direction);
            FLOAT num = Math.Abs(f) + Math.Abs(f2);
            FLOAT num2 = (box.center - this.origin).DotPerpendicular(this.direction);
            return num2 <= 0f - num + epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the negative side of the line, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref Box2D box, FLOAT epsilon) {
            FLOAT f = box.extents.x * box.axis0.DotPerpendicular(this.direction);
            FLOAT f2 = box.extents.y * box.axis1.DotPerpendicular(this.direction);
            FLOAT num = Math.Abs(f) + Math.Abs(f2);
            FLOAT num2 = (box.center - this.origin).DotPerpendicular(this.direction);
            return num2 <= 0f - num + epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the positive side of the line, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref Box2D box) {
            FLOAT epsilon = 1E-05f;
            FLOAT f = box.extents.x * box.axis0.DotPerpendicular(this.direction);
            FLOAT f2 = box.extents.y * box.axis1.DotPerpendicular(this.direction);
            FLOAT num = Math.Abs(f) + Math.Abs(f2);
            FLOAT num2 = (box.center - this.origin).DotPerpendicular(this.direction);
            return num2 >= num - epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the positive side of the line, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref Box2D box, FLOAT epsilon) {
            FLOAT f = box.extents.x * box.axis0.DotPerpendicular(this.direction);
            FLOAT f2 = box.extents.y * box.axis1.DotPerpendicular(this.direction);
            FLOAT num = Math.Abs(f) + Math.Abs(f2);
            FLOAT num2 = (box.center - this.origin).DotPerpendicular(this.direction);
            return num2 >= num - epsilon;
        }

        /// <summary>
        /// Determines on which side of the line a box is. Returns +1 if a box
        /// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
        /// </summary>
        public int QuerySide(ref AABB2D box) {
            FLOAT epsilon = 1E-05f;
            Vector2 vector = default(Vector2);
            vector.x = this.direction.y;
            vector.y = 0f - this.direction.x;
            Vector2 vector2 = default(Vector2);
            Vector2 vector3 = default(Vector2);
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

            if (vector.Dot(vector2 - this.origin) > 0f - epsilon) {
                return 1;
            }

            if (vector.Dot(vector3 - this.origin) < epsilon) {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Determines on which side of the line a box is. Returns +1 if a box
        /// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
        /// </summary>
        public int QuerySide(ref AABB2D box, FLOAT epsilon) {
            Vector2 vector = default(Vector2);
            vector.x = this.direction.y;
            vector.y = 0f - this.direction.x;
            Vector2 vector2 = default(Vector2);
            Vector2 vector3 = default(Vector2);
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

            if (vector.Dot(vector2 - this.origin) > 0f - epsilon) {
                return 1;
            }

            if (vector.Dot(vector3 - this.origin) < epsilon) {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Returns true if a box is on the negative side of the line, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref AABB2D box) {
            FLOAT epsilon = 1E-05f;
            Vector2 vector = default(Vector2);
            vector.x = this.direction.y;
            vector.y = 0f - this.direction.x;
            Vector2 vector2 = default(Vector2);
            if (vector.x >= 0f) {
                vector2.x = box.max.x;
            }
            else {
                vector2.x = box.min.x;
            }

            if (vector.y >= 0f) {
                vector2.y = box.max.y;
            }
            else {
                vector2.y = box.min.y;
            }

            return vector.Dot(vector2 - this.origin) <= epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the negative side of the line, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref AABB2D box, FLOAT epsilon) {
            Vector2 vector = default(Vector2);
            vector.x = this.direction.y;
            vector.y = 0f - this.direction.x;
            Vector2 vector2 = default(Vector2);
            if (vector.x >= 0f) {
                vector2.x = box.max.x;
            }
            else {
                vector2.x = box.min.x;
            }

            if (vector.y >= 0f) {
                vector2.y = box.max.y;
            }
            else {
                vector2.y = box.min.y;
            }

            return vector.Dot(vector2 - this.origin) <= epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the positive side of the line, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref AABB2D box) {
            FLOAT epsilon = 1E-05f;
            Vector2 vector = default(Vector2);
            vector.x = this.direction.y;
            vector.y = 0f - this.direction.x;
            Vector2 vector2 = default(Vector2);
            if (vector.x >= 0f) {
                vector2.x = box.min.x;
            }
            else {
                vector2.x = box.max.x;
            }

            if (vector.y >= 0f) {
                vector2.y = box.min.y;
            }
            else {
                vector2.y = box.max.y;
            }

            return vector.Dot(vector2 - this.origin) >= 0f - epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the positive side of the line, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref AABB2D box, FLOAT epsilon) {
            Vector2 vector = default(Vector2);
            vector.x = this.direction.y;
            vector.y = 0f - this.direction.x;
            Vector2 vector2 = default(Vector2);
            if (vector.x >= 0f) {
                vector2.x = box.min.x;
            }
            else {
                vector2.x = box.max.x;
            }

            if (vector.y >= 0f) {
                vector2.y = box.min.y;
            }
            else {
                vector2.y = box.max.y;
            }

            return vector.Dot(vector2 - this.origin) >= 0f - epsilon;
        }

        /// <summary>
        /// Determines on which side of the line a circle is. Returns +1 if a circle
        /// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
        /// </summary>
        public int QuerySide(ref Circle2D circle) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = (circle.center - this.origin).DotPerpendicular(this.direction);
            if (!(num > circle.radius - epsilon)) {
                if (!(num < 0f - circle.radius + epsilon)) {
                    return 0;
                }

                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Determines on which side of the line a circle is. Returns +1 if a circle
        /// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
        /// </summary>
        public int QuerySide(ref Circle2D circle, FLOAT epsilon) {
            FLOAT num = (circle.center - this.origin).DotPerpendicular(this.direction);
            if (!(num > circle.radius - epsilon)) {
                if (!(num < 0f - circle.radius + epsilon)) {
                    return 0;
                }

                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Returns true if a circle is on the negative side of the line, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref Circle2D circle) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = (circle.center - this.origin).DotPerpendicular(this.direction);
            return num <= 0f - circle.radius + epsilon;
        }

        /// <summary>
        /// Returns true if a circle is on the negative side of the line, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref Circle2D circle, FLOAT epsilon) {
            FLOAT num = (circle.center - this.origin).DotPerpendicular(this.direction);
            return num <= 0f - circle.radius + epsilon;
        }

        /// <summary>
        /// Returns true if a circle is on the positive side of the line, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref Circle2D circle) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = (circle.center - this.origin).DotPerpendicular(this.direction);
            return num >= circle.radius - epsilon;
        }

        /// <summary>
        /// Returns true if a circle is on the positive side of the line, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref Circle2D circle, FLOAT epsilon) {
            FLOAT num = (circle.center - this.origin).DotPerpendicular(this.direction);
            return num >= circle.radius - epsilon;
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public Vector2 Project(Vector2 point) {
            Distance.SqrPoint2Line2(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Returns angle between this line's direction and another line's direction as: arccos(dot(this.Direction,another.Direction))
        /// If acuteAngleDesired is true, then in resulting angle is &gt; pi/2, then result is transformed to be pi-angle.
        /// </summary>
        public FLOAT AngleBetweenTwoLines(Line2D anotherLine, bool acuteAngleDesired = false) {
            FLOAT num = Math.Acos(this.direction.Dot(anotherLine.direction));
            if (acuteAngleDesired && num > (FLOAT)Math.Pi / 2f) {
                return (FLOAT)Math.Pi - num;
            }

            return num;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[Origin: {this.origin.ToString()} Direction: {this.direction.ToString()}]";
        }
    }
}