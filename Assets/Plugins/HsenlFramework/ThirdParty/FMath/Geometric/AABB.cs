using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Axis aligned bounding box in 3D
    /// <para>AABB(axis aligned bounding boxes)可以用来做包围盒的相交检测，但不可旋转。对应的还有OBB(Oriented Bounding Box)，就是可以旋转的AABB</para>>
    /// </summary>
    public struct AABB {
        /// <summary>
        /// Min point
        /// </summary>
        public FVector3 min;

        /// <summary>
        /// Max point
        /// </summary>
        public FVector3 max;

        /// <summary>
        /// Creates AAB from min and max points.
        /// </summary>
        public AABB(ref FVector3 min, ref FVector3 max) {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Creates AAB from min and max points.
        /// </summary>
        public AABB(FVector3 min, FVector3 max) {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Creates AAB. The caller must ensure that xmin &lt;= xmax, ymin &lt;= ymax and zmin &lt;= zmax.
        /// </summary>
        public AABB(FLOAT xMin, FLOAT xMax, FLOAT yMin, FLOAT yMax, FLOAT zMin, FLOAT zMax) {
            this.min.x = xMin;
            this.min.y = yMin;
            this.min.z = zMin;
            this.max.x = xMax;
            this.max.y = yMax;
            this.max.z = zMax;
        }

        /// <summary>
        /// Creates AAB from single point. Min and Max are set to point. Use Include() method to grow the resulting AAB.
        /// </summary>
        public static AABB CreateFromPoint(ref FVector3 point) {
            AABB result = default(AABB);
            result.min = point;
            result.max = point;
            return result;
        }

        /// <summary>
        /// Creates AAB from single point. Min and Max are set to point. Use Include() method to grow the resulting AAB.
        /// </summary>
        public static AABB CreateFromPoint(FVector3 point) {
            AABB result = default(AABB);
            result.min = point;
            result.max = point;
            return result;
        }

        /// <summary>
        /// Computes AAB from two points extracting min and max values. In case min and max points are known, use constructor instead.
        /// </summary>
        public static AABB CreateFromTwoPoints(ref FVector3 point0, ref FVector3 point1) {
            AABB result = default(AABB);
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

            if (point0.z < point1.z) {
                result.min.z = point0.z;
                result.max.z = point1.z;
            }
            else {
                result.min.z = point1.z;
                result.max.z = point0.z;
            }

            return result;
        }

        /// <summary>
        /// Computes AAB from two points. In case min and max points are known, use constructor instead.
        /// </summary>
        public static AABB CreateFromTwoPoints(FVector3 point0, FVector3 point1) {
            return CreateFromTwoPoints(ref point0, ref point1);
        }

        /// <summary>
        /// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
        /// If a set is empty, returns new AAB3().
        /// </summary>
        public static AABB CreateFromPoints(IEnumerable<FVector3> points) {
            IEnumerator<FVector3> enumerator = points.GetEnumerator();
            enumerator.Reset();
            if (!enumerator.MoveNext()) {
                return default(AABB);
            }

            AABB result = CreateFromPoint(enumerator.Current);
            while (enumerator.MoveNext()) {
                result.Include(enumerator.Current);
            }

            return result;
        }

        /// <summary>
        /// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
        /// If a set is empty, returns new AAB3().
        /// </summary>
        public static AABB CreateFromPoints(IList<FVector3> points) {
            int count = points.Count;
            if (count > 0) {
                AABB result = CreateFromPoint(points[0]);
                for (int i = 1; i < count; i++) {
                    result.Include(points[i]);
                }

                return result;
            }

            return default(AABB);
        }

        /// <summary>
        /// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
        /// If a set is empty, returns new AAB3().
        /// </summary>
        public static AABB CreateFromPoints(FVector3[] points) {
            int num = points.Length;
            if (num > 0) {
                AABB result = CreateFromPoint(ref points[0]);
                for (int i = 1; i < num; i++) {
                    result.Include(ref points[i]);
                }

                return result;
            }

            return default(AABB);
        }

        /// <summary>
        /// Computes box center and extents (half sizes)
        /// </summary>
        public void CalcCenterExtents(out FVector3 center, out FVector3 extents) {
            center.x = 0.5f * (this.max.x + this.min.x);
            center.y = 0.5f * (this.max.y + this.min.y);
            center.z = 0.5f * (this.max.z + this.min.z);
            extents.x = 0.5f * (this.max.x - this.min.x);
            extents.y = 0.5f * (this.max.y - this.min.y);
            extents.z = 0.5f * (this.max.z - this.min.z);
        }

        /// <summary>
        /// Calculates 8 box corners.
        /// </summary>
        /// <param name="vertex0">FVector3(Min.x, Min.y, Min.z)</param>
        /// <param name="vertex1">FVector3(Max.x, Min.y, Min.z)</param>
        /// <param name="vertex2">FVector3(Max.x, Max.y, Min.z)</param>
        /// <param name="vertex3">FVector3(Min.x, Max.y, Min.z)</param>
        /// <param name="vertex4">FVector3(Min.x, Min.y, Max.z)</param>
        /// <param name="vertex5">FVector3(Max.x, Min.y, Max.z)</param>
        /// <param name="vertex6">FVector3(Max.x, Max.y, Max.z)</param>
        /// <param name="vertex7">FVector3(Min.x, Max.y, Max.z)</param>
        public void CalcVertices(out FVector3 vertex0, out FVector3 vertex1, out FVector3 vertex2,
            out FVector3 vertex3, out FVector3 vertex4, out FVector3 vertex5, out FVector3 vertex6,
            out FVector3 vertex7) {
            vertex0 = this.min;
            vertex1 = new FVector3(this.max.x, this.min.y, this.min.z);
            vertex2 = new FVector3(this.max.x, this.max.y, this.min.z);
            vertex3 = new FVector3(this.min.x, this.max.y, this.min.z);
            vertex4 = new FVector3(this.min.x, this.min.y, this.max.z);
            vertex5 = new FVector3(this.max.x, this.min.y, this.max.z);
            vertex6 = this.max;
            vertex7 = new FVector3(this.min.x, this.max.y, this.max.z);
        }

        /// <summary>
        /// Calculates 8 box corners and returns them in an allocated array.
        /// See array-less overload for the description.
        /// </summary>
        public FVector3[] CalcVertices() {
            return new FVector3[8] {
                this.min, new FVector3(this.max.x, this.min.y, this.min.z), new FVector3(this.max.x, this.max.y, this.min.z),
                new FVector3(this.min.x, this.max.y, this.min.z),
                new FVector3(this.min.x, this.min.y, this.max.z), new FVector3(this.max.x, this.min.y, this.max.z), this.max,
                new FVector3(this.min.x, this.max.y, this.max.z)
            };
        }

        /// <summary>
        /// Calculates 8 box corners and fills the input array with them (array length must be 8).
        /// See array-less overload for the description.
        /// </summary>
        public void CalcVertices(FVector3[] array) {
            ref FVector3 reference = ref array[0];
            reference = this.min;
            ref FVector3 reference2 = ref array[1];
            reference2 = new FVector3(this.max.x, this.min.y, this.min.z);
            ref FVector3 reference3 = ref array[2];
            reference3 = new FVector3(this.max.x, this.max.y, this.min.z);
            ref FVector3 reference4 = ref array[3];
            reference4 = new FVector3(this.min.x, this.max.y, this.min.z);
            ref FVector3 reference5 = ref array[4];
            reference5 = new FVector3(this.min.x, this.min.y, this.max.z);
            ref FVector3 reference6 = ref array[5];
            reference6 = new FVector3(this.max.x, this.min.y, this.max.z);
            ref FVector3 reference7 = ref array[6];
            reference7 = this.max;
            ref FVector3 reference8 = ref array[7];
            reference8 = new FVector3(this.min.x, this.max.y, this.max.z);
        }

        /// <summary>
        /// Returns box volume
        /// </summary>
        public FLOAT CalcVolume() {
            return (this.max.x - this.min.x) * (this.max.y - this.min.y) * (this.max.z - this.min.z);
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(FVector3 point) {
            return Distance.Point3AAB3(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public FVector3 Project(FVector3 point) {
            Distance.SqrPoint3AAB3(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Tests whether a point is contained by the aab
        /// </summary>
        public bool Contains(ref FVector3 point) {
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

            if (point.z < this.min.z) {
                return false;
            }

            if (point.z > this.max.z) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether a point is contained by the aab
        /// </summary>
        public bool Contains(FVector3 point) {
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

            if (point.z < this.min.z) {
                return false;
            }

            if (point.z > this.max.z) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enlarging the aab to include the point. If the point is inside the AAB does nothing.
        /// </summary>
        public void Include(ref FVector3 point) {
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

            if (point.z < this.min.z) {
                this.min.z = point.z;
            }
            else if (point.z > this.max.z) {
                this.max.z = point.z;
            }
        }

        /// <summary>
        /// Enlarging the aab to include the point. If the point is inside the AAB does nothing.
        /// </summary>
        public void Include(FVector3 point) {
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

            if (point.z < this.min.z) {
                this.min.z = point.z;
            }
            else if (point.z > this.max.z) {
                this.max.z = point.z;
            }
        }

        /// <summary>
        /// Enlarges the aab so it includes another aab.
        /// </summary>
        public void Include(ref AABB box) {
            this.Include(ref box.min);
            this.Include(ref box.max);
        }

        /// <summary>
        /// Enlarges the aab so it includes another aab.
        /// </summary>
        public void Include(AABB box) {
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