using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// A box has center C, axis directions A0 and A1 (perpendicular and
    /// unit-length vectors), and extents e0 and e1 (nonnegative numbers).
    /// A point X = C + y0*A0 + y1*A1 is inside or on the box whenever
    /// |y[i]| &lt;= e[i] for all i.
    /// </summary>
    public struct Box2D {
        /// <summary>
        /// Box center
        /// </summary>
        public FVector2 center;

        /// <summary>
        /// First box axis. Must be unit length!
        /// </summary>
        public FVector2 axis0;

        /// <summary>
        /// Second box axis. Must be unit length!
        /// </summary>
        public FVector2 axis1;

        /// <summary>
        /// Extents (half sizes) along Axis0 and Axis1. Must be non-negative!
        /// </summary>
        public FVector2 extents;

        /// <summary>
        /// Creates new Box2 instance.
        /// </summary>
        /// <param name="center">Box center</param>
        /// <param name="axis0">First box axis. Must be unit length!</param>
        /// <param name="axis1">Second box axis. Must be unit length!</param>
        /// <param name="extents">Extents (half sizes) along Axis0 and Axis1. Must be non-negative!</param>
        public Box2D(ref FVector2 center, ref FVector2 axis0, ref FVector2 axis1, ref FVector2 extents) {
            this.center = center;
            this.axis0 = axis0;
            this.axis1 = axis1;
            this.extents = extents;
        }

        /// <summary>
        /// Creates new Box2 instance.
        /// </summary>
        /// <param name="center">Box center</param>
        /// <param name="axis0">First box axis. Must be unit length!</param>
        /// <param name="axis1">Second box axis. Must be unit length!</param>
        /// <param name="extents">Extents (half sizes) along Axis0 and Axis1. Must be non-negative!</param>
        public Box2D(FVector2 center, FVector2 axis0, FVector2 axis1, FVector2 extents) {
            this.center = center;
            this.axis0 = axis0;
            this.axis1 = axis1;
            this.extents = extents;
        }

        /// <summary>
        /// Creates Box2 from AxisAlignedBox2
        /// </summary>
        public Box2D(ref AABB2D box) {
            box.CalcCenterExtents(out this.center, out this.extents);
            this.axis0 = FVector2.Right;
            this.axis1 = FVector2.Up;
        }

        /// <summary>
        /// Creates Box2 from AxisAlignedBox2
        /// </summary>
        public Box2D(AABB2D box) {
            box.CalcCenterExtents(out this.center, out this.extents);
            this.axis0 = FVector2.Right;
            this.axis1 = FVector2.Up;
        }

        /// <summary>
        /// Computes oriented bounding box from a set of points.
        /// If a set is empty returns new Box2().
        /// </summary>
        public static Box2D CreateFromPoints(IList<FVector2> points) {
            int count = points.Count;
            if (count == 0) {
                return default(Box2D);
            }

            Box2D result = Approximation.GaussPointsFit2(points);
            FVector2 vector = points[0] - result.center;
            FVector2 vector2 = new FVector2(vector.Dot(result.axis0), vector.Dot(result.axis1));
            FVector2 vector3 = vector2;
            for (int i = 1; i < count; i++) {
                vector = points[i] - result.center;
                for (int j = 0; j < 2; j++) {
                    FLOAT num = vector.Dot(result.GetAxis(j));
                    if (num < vector2[j]) {
                        vector2[j] = num;
                    }
                    else if (num > vector3[j]) {
                        vector3[j] = num;
                    }
                }
            }

            result.center += 0.5f * (vector2[0] + vector3[0]) * result.axis0 +
                             0.5f * (vector2[1] + vector3[1]) * result.axis1;
            result.extents.x = 0.5f * (vector3[0] - vector2[0]);
            result.extents.y = 0.5f * (vector3[1] - vector2[1]);
            return result;
        }

        /// <summary>
        /// Returns axis by index (0, 1)
        /// </summary>
        public FVector2 GetAxis(int index) {
            return index switch {
                0 => this.axis0,
                1 => this.axis1,
                _ => FVector2.Zero,
            };
        }

        /// <summary>
        /// Calculates 4 box corners. extAxis[i] is Axis[i]*Extent[i], i=0,1.
        /// </summary>
        /// <param name="vertex0">Center - extAxis0 - extAxis1</param>
        /// <param name="vertex1">Center + extAxis0 - extAxis1</param>
        /// <param name="vertex2">Center + extAxis0 + extAxis1</param>
        /// <param name="vertex3">Center - extAxis0 + extAxis1</param>
        public void CalcVertices(out FVector2 vertex0, out FVector2 vertex1, out FVector2 vertex2,
            out FVector2 vertex3) {
            FVector2 vector = this.axis0 * this.extents.x;
            FVector2 vector2 = this.axis1 * this.extents.y;
            vertex0 = this.center - vector - vector2;
            vertex1 = this.center + vector - vector2;
            vertex2 = this.center + vector + vector2;
            vertex3 = this.center - vector + vector2;
        }

        /// <summary>
        /// Calculates 4 box corners and returns them in an allocated array.
        /// See array-less overload for the description.
        /// </summary>
        public FVector2[] CalcVertices() {
            FVector2 vector = this.axis0 * this.extents.x;
            FVector2 vector2 = this.axis1 * this.extents.y;
            return new FVector2[4]
                { this.center - vector - vector2, this.center + vector - vector2, this.center + vector + vector2, this.center - vector + vector2 };
        }

        /// <summary>
        /// Calculates 4 box corners and fills the input array with them (array length must be 4).
        /// See array-less overload for the description.
        /// </summary>
        public void CalcVertices(FVector2[] array) {
            FVector2 vector = this.axis0 * this.extents.x;
            FVector2 vector2 = this.axis1 * this.extents.y;
            ref FVector2 reference = ref array[0];
            reference = this.center - vector - vector2;
            ref FVector2 reference2 = ref array[1];
            reference2 = this.center + vector - vector2;
            ref FVector2 reference3 = ref array[2];
            reference3 = this.center + vector + vector2;
            ref FVector2 reference4 = ref array[3];
            reference4 = this.center - vector + vector2;
        }

        /// <summary>
        /// Returns area of the box as Extents.x * Extents.y * 4
        /// </summary>
        public FLOAT CalcArea() {
            return 4f * this.extents.x * this.extents.y;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(FVector2 point) {
            return Distance.Point2Box2(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public FVector2 Project(FVector2 point) {
            Distance.SqrPoint2Box2(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Tests whether a point is contained by the box
        /// </summary>
        public bool Contains(ref FVector2 point) {
            FVector2 vector = default(FVector2);
            vector.x = point.x - this.center.x;
            vector.y = point.y - this.center.y;
            FLOAT num = vector.Dot(this.axis0);
            if (num < 0f - this.extents.x) {
                return false;
            }

            if (num > this.extents.x) {
                return false;
            }

            num = vector.Dot(this.axis1);
            if (num < 0f - this.extents.y) {
                return false;
            }

            if (num > this.extents.y) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether a point is contained by the box
        /// </summary>
        public bool Contains(FVector2 point) {
            FVector2 vector = default(FVector2);
            vector.x = point.x - this.center.x;
            vector.y = point.y - this.center.y;
            FLOAT num = vector.Dot(this.axis0);
            if (num < 0f - this.extents.x) {
                return false;
            }

            if (num > this.extents.x) {
                return false;
            }

            num = vector.Dot(this.axis1);
            if (num < 0f - this.extents.y) {
                return false;
            }

            if (num > this.extents.y) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enlarges the box so it includes another box.
        /// </summary>
        public void Include(ref Box2D box) {
            Box2D box2D = default(Box2D);
            box2D.center = 0.5f * (this.center + box.center);
            if (this.axis0.Dot(box.axis0) >= 0f) {
                box2D.axis0 = 0.5f * (this.axis0 + box.axis0);
                box2D.axis0.Normalize();
            }
            else {
                box2D.axis0 = 0.5f * (this.axis0 - box.axis0);
                box2D.axis0.Normalize();
            }

            box2D.axis1 = -box2D.axis0.Perpendicular();
            FVector2 zero = FVector2.Zero;
            FVector2 zero2 = FVector2.Zero;
            FVector2[] array = this.CalcVertices();
            for (int i = 0; i < 4; i++) {
                FVector2 vector = array[i] - box2D.center;
                for (int j = 0; j < 2; j++) {
                    FLOAT num = vector.Dot(box2D.GetAxis(j));
                    if (num > zero2[j]) {
                        zero2[j] = num;
                    }
                    else if (num < zero[j]) {
                        zero[j] = num;
                    }
                }
            }

            box.CalcVertices(out array[0], out array[1], out array[2], out array[3]);
            for (int i = 0; i < 4; i++) {
                FVector2 vector = array[i] - box2D.center;
                for (int j = 0; j < 2; j++) {
                    FLOAT num = vector.Dot(box2D.GetAxis(j));
                    if (num > zero2[j]) {
                        zero2[j] = num;
                    }
                    else if (num < zero[j]) {
                        zero[j] = num;
                    }
                }
            }

            for (int j = 0; j < 2; j++) {
                box2D.center += box2D.GetAxis(j) * (0.5f * (zero2[j] + zero[j]));
                box2D.extents[j] = 0.5f * (zero2[j] - zero[j]);
            }

            this = box2D;
        }

        /// <summary>
        /// Enlarges the box so it includes another box.
        /// </summary>
        public void Include(Box2D box) {
            this.Include(ref box);
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return
                $"[Center: {this.center.ToString()} Axis0: {this.axis0.ToString()} Axis1: {this.axis1.ToString()} Extents: {this.extents.ToString()}]";
        }
    }
}