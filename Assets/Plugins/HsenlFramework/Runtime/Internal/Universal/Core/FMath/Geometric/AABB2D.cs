using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Axis aligned bounding box in 2D
    /// </summary>
    public struct AABB2D {
        /// <summary>
        /// Min point
        /// </summary>
        public Vector2 min;

        /// <summary>
        /// Max point
        /// </summary>
        public Vector2 max;

        /// <summary>
        /// Creates AAB from min and max points.
        /// </summary>
        public AABB2D(ref Vector2 min, ref Vector2 max) {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Creates AAB from min and max points.
        /// </summary>
        public AABB2D(Vector2 min, Vector2 max) {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Creates AAB. The caller must ensure that xmin &lt;= xmax and ymin &lt;= ymax.
        /// </summary>
        public AABB2D(FLOAT xMin, FLOAT xMax, FLOAT yMin, FLOAT yMax) {
            this.min.x = xMin;
            this.min.y = yMin;
            this.max.x = xMax;
            this.max.y = yMax;
        }

        /// <summary>
        /// Creates AAB from single point. Min and Max are set to point. Use Include() method to grow the resulting AAB.
        /// </summary>
        public static AABB2D CreateFromPoint(ref Vector2 point) {
            AABB2D result = default(AABB2D);
            result.min = point;
            result.max = point;
            return result;
        }

        /// <summary>
        /// Creates AAB from single point. Min and Max are set to point. Use Include() method to grow the resulting AAB.
        /// </summary>
        public static AABB2D CreateFromPoint(Vector2 point) {
            AABB2D result = default(AABB2D);
            result.min = point;
            result.max = point;
            return result;
        }

        /// <summary>
        /// Computes AAB from two points extracting min and max values. In case min and max points are known, use constructor instead.
        /// </summary>
        public static AABB2D CreateFromTwoPoints(ref Vector2 point0, ref Vector2 point1) {
            AABB2D result = default(AABB2D);
            if (point0.x < point1.x) {
                result.min.x = point0.x;
                result.max.x = point1.x;
            }
            else {
                result.min.x = point1.x;
                result.max.x = point0.x;
            }

            if (point0.y < point1.y) {
                result.min.y = point0.y;
                result.max.y = point1.y;
            }
            else {
                result.min.y = point1.y;
                result.max.y = point0.y;
            }

            return result;
        }

        /// <summary>
        /// Computes AAB from two points. In case min and max points are known, use constructor instead.
        /// </summary>
        public static AABB2D CreateFromTwoPoints(Vector2 point0, Vector2 point1) {
            return CreateFromTwoPoints(ref point0, ref point1);
        }

        /// <summary>
        /// Computes AAB from the a of points. Method includes points from a set one by one to create the AAB.
        /// If a set is empty, returns new AAB2().
        /// </summary>
        public static AABB2D CreateFromPoints(IEnumerable<Vector2> points) {
            IEnumerator<Vector2> enumerator = points.GetEnumerator();
            enumerator.Reset();
            if (!enumerator.MoveNext()) {
                return default(AABB2D);
            }

            AABB2D result = CreateFromPoint(enumerator.Current);
            while (enumerator.MoveNext()) {
                result.Include(enumerator.Current);
            }

            return result;
        }

        /// <summary>
        /// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
        /// If a set is empty, returns new AAB2().
        /// </summary>
        public static AABB2D CreateFromPoints(IList<Vector2> points) {
            int count = points.Count;
            if (count > 0) {
                AABB2D result = CreateFromPoint(points[0]);
                for (int i = 1; i < count; i++) {
                    result.Include(points[i]);
                }

                return result;
            }

            return default(AABB2D);
        }

        /// <summary>
        /// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
        /// If a set is empty, returns new AAB2()
        /// </summary>
        public static AABB2D CreateFromPoints(Vector2[] points) {
            int num = points.Length;
            if (num > 0) {
                AABB2D result = CreateFromPoint(ref points[0]);
                for (int i = 1; i < num; i++) {
                    result.Include(ref points[i]);
                }

                return result;
            }

            return default(AABB2D);
        }

        /// <summary>
        /// Computes box center and extents (half sizes)
        /// </summary>
        public void CalcCenterExtents(out Vector2 center, out Vector2 extents) {
            center.x = 0.5f * (this.max.x + this.min.x);
            center.y = 0.5f * (this.max.y + this.min.y);
            extents.x = 0.5f * (this.max.x - this.min.x);
            extents.y = 0.5f * (this.max.y - this.min.y);
        }

        /// <summary>
        /// Calculates 4 box corners.
        /// </summary>
        /// <param name="vertex0">FVector2(Min.x, Min.y)</param>
        /// <param name="vertex1">FVector2(Max.x, Min.y)</param>
        /// <param name="vertex2">FVector2(Max.x, Max.y)</param>
        /// <param name="vertex3">FVector2(Min.x, Max.y)</param>
        public void CalcVertices(out Vector2 vertex0, out Vector2 vertex1, out Vector2 vertex2,
            out Vector2 vertex3) {
            vertex0 = this.min;
            vertex1 = new Vector2(this.max.x, this.min.y);
            vertex2 = this.max;
            vertex3 = new Vector2(this.min.x, this.max.y);
        }

        /// <summary>
        /// Calculates 4 box corners and returns them in an allocated array.
        /// See array-less overload for the description.
        /// </summary>
        public Vector2[] CalcVertices() {
            return new Vector2[4] { this.min, new Vector2(this.max.x, this.min.y), this.max, new Vector2(this.min.x, this.max.y) };
        }

        /// <summary>
        /// Calculates 4 box corners and fills the input array with them (array length must be 4).
        /// See array-less overload for the description.
        /// </summary>
        public void CalcVertices(Vector2[] array) {
            ref Vector2 reference = ref array[0];
            reference = this.min;
            ref Vector2 reference2 = ref array[1];
            reference2 = new Vector2(this.max.x, this.min.y);
            ref Vector2 reference3 = ref array[2];
            reference3 = this.max;
            ref Vector2 reference4 = ref array[3];
            reference4 = new Vector2(this.min.x, this.max.y);
        }

        /// <summary>
        /// Returns box area
        /// </summary>
        public FLOAT CalcArea() {
            return (this.max.x - this.min.x) * (this.max.y - this.min.y);
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector2 point) {
            return Distance.Point2AAB2(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public Vector2 Project(Vector2 point) {
            Distance.SqrPoint2AAB2(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Tests whether a point is contained by the aab
        /// </summary>
        public bool Contains(ref Vector2 point) {
            if (point.x < this.min.x) {
                return false;
            }

            if (point.x > this.max.x) {
                return false;
            }

            if (point.y < this.min.y) {
                return false;
            }

            if (point.y > this.max.y) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether a point is contained by the aab
        /// </summary>
        public bool Contains(Vector2 point) {
            if (point.x < this.min.x) {
                return false;
            }

            if (point.x > this.max.x) {
                return false;
            }

            if (point.y < this.min.y) {
                return false;
            }

            if (point.y > this.max.y) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enlarges the aab to include the point. If the point is inside the AAB does nothing.
        /// </summary>
        public void Include(ref Vector2 point) {
            if (point.x < this.min.x) {
                this.min.x = point.x;
            }
            else if (point.x > this.max.x) {
                this.max.x = point.x;
            }

            if (point.y < this.min.y) {
                this.min.y = point.y;
            }
            else if (point.y > this.max.y) {
                this.max.y = point.y;
            }
        }

        /// <summary>
        /// Enlarges the aab to include the point. If the point is inside the AAB does nothing.
        /// </summary>
        public void Include(Vector2 point) {
            if (point.x < this.min.x) {
                this.min.x = point.x;
            }
            else if (point.x > this.max.x) {
                this.max.x = point.x;
            }

            if (point.y < this.min.y) {
                this.min.y = point.y;
            }
            else if (point.y > this.max.y) {
                this.max.y = point.y;
            }
        }

        /// <summary>
        /// Enlarges the aab so it includes another aab.
        /// </summary>
        public void Include(ref AABB2D box) {
            this.Include(ref box.min);
            this.Include(ref box.max);
        }

        /// <summary>
        /// Enlarges the aab so it includes another aab.
        /// </summary>
        public void Include(AABB2D box) {
            this.Include(ref box.min);
            this.Include(ref box.max);
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[Min: {this.min.ToString()} Max: {this.max.ToString()}]";
        }
    }
}