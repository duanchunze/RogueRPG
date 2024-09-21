using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Circle is described by the formula |X - C|^2 = r^2,
    /// where C - circle center, r - circle radius
    /// </summary>
    public struct Circle2D {
        /// <summary>
        /// Circle center
        /// </summary>
        public Vector2 center;

        /// <summary>
        /// Circle radius
        /// </summary>
        public FLOAT radius;

        /// <summary>
        /// Creates circle from center and radius
        /// </summary>
        public Circle2D(ref Vector2 center, FLOAT radius) {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>
        /// Creates circle from center and radius
        /// </summary>
        public Circle2D(Vector2 center, FLOAT radius) {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>
        /// Computes bounding circle from a set of points.
        /// First compute the axis-aligned bounding box of the points, then compute the circle containing the box.
        /// If a set is empty returns new Circle2().
        /// </summary>
        public static Circle2D CreateFromPointsAAB(IEnumerable<Vector2> points) {
            IEnumerator<Vector2> enumerator = points.GetEnumerator();
            enumerator.Reset();
            if (!enumerator.MoveNext()) {
                return default(Circle2D);
            }

            AABB2D.CreateFromPoints(points).CalcCenterExtents(out var center, out var extents);
            Circle2D result = default(Circle2D);
            result.center = center;
            result.radius = extents.magnitude;
            return result;
        }

        /// <summary>
        /// Computes bounding circle from a set of points.
        /// First compute the axis-aligned bounding box of the points, then compute the circle containing the box.
        /// If a set is empty returns new Circle2().
        /// </summary>
        public static Circle2D CreateFromPointsAAB(IList<Vector2> points) {
            if (points.Count == 0) {
                return default(Circle2D);
            }

            AABB2D.CreateFromPoints(points).CalcCenterExtents(out var center, out var extents);
            Circle2D result = default(Circle2D);
            result.center = center;
            result.radius = extents.magnitude;
            return result;
        }

        /// <summary>
        /// Computes bounding circle from a set of points.
        /// Compute the smallest circle whose center is the average of a point set.
        /// If a set is empty returns new Circle2().
        /// </summary>
        public static Circle2D CreateFromPointsAverage(IEnumerable<Vector2> points) {
            IEnumerator<Vector2> enumerator = points.GetEnumerator();
            enumerator.Reset();
            if (!enumerator.MoveNext()) {
                return default(Circle2D);
            }

            Vector2 current = enumerator.Current;
            int num = 1;
            while (enumerator.MoveNext()) {
                current += enumerator.Current;
                num++;
            }

            current /= (FLOAT)num;
            FLOAT num2 = 0f;
            foreach (Vector2 point in points) {
                FLOAT sqrMagnitude = (point - current).sqrMagnitude;
                if (sqrMagnitude > num2) {
                    num2 = sqrMagnitude;
                }
            }

            Circle2D result = default(Circle2D);
            result.center = current;
            result.radius = Math.Sqrt(num2);
            return result;
        }

        /// <summary>
        /// Computes bounding circle from a set of points.
        /// Compute the smallest circle whose center is the average of a point set.
        /// If a set is empty returns new Circle2().
        /// </summary>
        public static Circle2D CreateFromPointsAverage(IList<Vector2> points) {
            int count = points.Count;
            if (count == 0) {
                return default(Circle2D);
            }

            Vector2 vector = points[0];
            for (int i = 1; i < count; i++) {
                vector += points[i];
            }

            vector /= (FLOAT)count;
            FLOAT num = 0f;
            for (int j = 0; j < count; j++) {
                FLOAT sqrMagnitude = (points[j] - vector).sqrMagnitude;
                if (sqrMagnitude > num) {
                    num = sqrMagnitude;
                }
            }

            Circle2D result = default(Circle2D);
            result.center = vector;
            result.radius = Math.Sqrt(num);
            return result;
        }

        /// <summary>
        /// Creates circle which is circumscribed around triangle.
        /// Returns 'true' if circle has been constructed, 'false' otherwise (input points are linearly dependent).
        /// </summary>
        public static bool CreateCircumscribed(Vector2 v0, Vector2 v1, Vector2 v2, out Circle2D circle) {
            Vector2 vector = v1 - v0;
            Vector2 vector2 = v2 - v0;
            FLOAT[,] a = new FLOAT[2, 2] { { vector.x, vector.y }, { vector2.x, vector2.y } };
            FLOAT[] b = new FLOAT[2] { 0.5f * vector.sqrMagnitude, 0.5f * vector2.sqrMagnitude };
            if (LinearSystem.Solve2(a, b, out Vector2 X, 1E-05f)) {
                circle.center = v0 + X;
                circle.radius = X.magnitude;
                return true;
            }

            circle = default(Circle2D);
            return false;
        }

        /// <summary>
        /// Creates circle which is insribed into triangle.
        /// Returns 'true' if circle has been constructed, 'false' otherwise (input points are linearly dependent).
        /// </summary>
        public static bool CreateInscribed(Vector2 v0, Vector2 v1, Vector2 v2, out Circle2D circle) {
            Vector2 vector = v1 - v0;
            Vector2 value = v2 - v0;
            Vector2 vector2 = v2 - v1;
            FLOAT magnitude = vector.magnitude;
            FLOAT magnitude2 = value.magnitude;
            FLOAT magnitude3 = vector2.magnitude;
            FLOAT num = magnitude + magnitude2 + magnitude3;
            if (num > 1E-05f) {
                FLOAT num2 = 1f / num;
                magnitude *= num2;
                magnitude2 *= num2;
                magnitude3 *= num2;
                circle.center = magnitude3 * v0 + magnitude2 * v1 + magnitude * v2;
                circle.radius = num2 * Math.Abs(vector.DotPerpendicular(value));
                if (circle.radius > 1E-05f) {
                    return true;
                }
            }

            circle = default(Circle2D);
            return false;
        }

        /// <summary>
        /// Returns circle perimeter
        /// </summary>
        public FLOAT CalcPerimeter() {
            return (FLOAT)Math.Pi * 2f * this.radius;
        }

        /// <summary>
        /// Returns circle area
        /// </summary>
        public FLOAT CalcArea() {
            return (FLOAT)Math.Pi * this.radius * this.radius;
        }

        /// <summary>
        /// Evaluates circle using formula X = C + R*[cos(t), sin(t)]
        /// where t is an angle in [0,2*pi).
        /// </summary>
        /// <param name="t">Evaluation parameter</param>
        public Vector2 Eval(FLOAT t) {
            return new Vector2(this.center.x + this.radius * Math.Cos(t), this.center.y + this.radius * Math.Sin(t));
        }

        /// <summary>
        /// Evaluates disk using formula X = C + radius*[cos(t), sin(t)]
        /// where t is an angle in [0,2*pi).
        /// </summary>
        /// <param name="t">Evaluation parameter</param>
        /// <param name="radius">Evaluation radius</param>
        public Vector2 Eval(FLOAT t, FLOAT radius) {
            return new Vector2(this.center.x + radius * Math.Cos(t), this.center.y + radius * Math.Sin(t));
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector2 point) {
            return Distance.Point2Circle2(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public Vector2 Project(Vector2 point) {
            Distance.SqrPoint2Circle2(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Tests whether a point is contained by the circle
        /// </summary>
        public bool Contains(ref Vector2 point) {
            return (point - this.center).sqrMagnitude <= this.radius * this.radius;
        }

        /// <summary>
        /// Tests whether a point is contained by the circle
        /// </summary>
        public bool Contains(Vector2 point) {
            return (point - this.center).sqrMagnitude <= this.radius * this.radius;
        }

        /// <summary>
        /// Enlarges the circle so it includes another circle.
        /// </summary>
        public void Include(ref Circle2D circle) {
            Vector2 vector = circle.center - this.center;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num = circle.radius - this.radius;
            FLOAT num2 = num * num;
            if (num2 >= sqrMagnitude) {
                if (num >= 0f) {
                    this = circle;
                }

                return;
            }

            FLOAT num3 = Math.Sqrt(sqrMagnitude);
            if (num3 > 1E-05f) {
                FLOAT num4 = (num3 + num) / (2f * num3);
                this.center += num4 * vector;
            }

            this.radius = 0.5f * (num3 + this.radius + circle.radius);
        }

        /// <summary>
        /// Enlarges the circle so it includes another circle.
        /// </summary>
        public void Include(Circle2D circle) {
            this.Include(ref circle);
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[Center: {this.center.ToString()} Radius: {this.radius.ToString()}]";
        }
    }
}