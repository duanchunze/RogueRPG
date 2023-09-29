using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// A box has center C, axis directions U[0], U[1], and U[2] (mutually
    /// perpendicular unit-length vectors), and extents e[0], e[1], and e[2]
    /// (all nonnegative numbers).  A point X = C+y[0]*U[0]+y[1]*U[1]+y[2]*U[2]
    /// is inside or on the box whenever |y[i]| &lt;= e[i] for all i.
    /// 可以当做OBB(Oriented Bounding Box)去使用
    /// </summary>
    public struct Box {
        /// <summary>
        /// Box center
        /// </summary>
        public FVector3 center;

        /// <summary>
        /// First box axis. Must be unit length!
        /// </summary>
        public FVector3 axis0;

        /// <summary>
        /// Second box axis. Must be unit length!
        /// </summary>
        public FVector3 axis1;

        /// <summary>
        /// Third box axis. Must be unit length!
        /// </summary>
        public FVector3 axis2;

        /// <summary>
        /// Extents (half sizes) along Axis0, Axis1 and Axis2. Must be non-negative!
        /// </summary>
        public FVector3 extents;

        /// <summary>
        /// Creates new Box3 instance.
        /// </summary>
        /// <param name="center">Box center</param>
        /// <param name="axis0">First box axis. Must be unit length!</param>
        /// <param name="axis1">Second box axis. Must be unit length!</param>
        /// <param name="axis2">Third box axis. Must be unit length!</param>
        /// <param name="extents">Extents (half sizes) along Axis0, Axis1 and Axis2. Must be non-negative!</param>
        public Box(ref FVector3 center, ref FVector3 axis0, ref FVector3 axis1, ref FVector3 axis2,
            ref FVector3 extents) {
            this.center = center;
            this.axis0 = axis0;
            this.axis1 = axis1;
            this.axis2 = axis2;
            this.extents = extents;
        }

        /// <summary>
        /// Creates new Box3 instance.
        /// </summary>
        /// <param name="center">Box center</param>
        /// <param name="axis0">First box axis. Must be unit length!</param>
        /// <param name="axis1">Second box axis. Must be unit length!</param>
        /// <param name="axis2">Third box axis. Must be unit length!</param>
        /// <param name="extents">Extents (half sizes) along Axis0, Axis1 and Axis2. Must be non-negative!</param>
        public Box(FVector3 center, FVector3 axis0, FVector3 axis1, FVector3 axis2, FVector3 extents) {
            this.center = center;
            this.axis0 = axis0;
            this.axis1 = axis1;
            this.axis2 = axis2;
            this.extents = extents;
        }

        /// <summary>
        /// Create Box3 from AxisAlignedBox3
        /// </summary>
        public Box(ref AABB box) {
            box.CalcCenterExtents(out this.center, out this.extents);
            this.axis0 = FVector3.Right;
            this.axis1 = FVector3.Up;
            this.axis2 = FVector3.Forward;
        }

        /// <summary>
        /// Create Box3 from AxisAlignedBox3
        /// </summary>
        public Box(AABB box) {
            box.CalcCenterExtents(out this.center, out this.extents);
            this.axis0 = FVector3.Right;
            this.axis1 = FVector3.Up;
            this.axis2 = FVector3.Forward;
        }

        /// <summary>
        /// Computes oriented bounding box from a set of points.
        /// If a set is empty returns new Box3().
        /// </summary>
        public static Box CreateFromPoints(IList<FVector3> points) {
            int count = points.Count;
            if (count == 0) {
                return default(Box);
            }

            Box result = Approximation.GaussPointsFit3(points);
            FVector3 vector = points[0] - result.center;
            FVector3 vector2 = new FVector3(vector.Dot(result.axis0), vector.Dot(result.axis1),
                vector.Dot(result.axis2));
            FVector3 vector3 = vector2;
            for (int i = 1; i < count; i++) {
                vector = points[i] - result.center;
                for (int j = 0; j < 3; j++) {
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
                             0.5f * (vector2[1] + vector3[1]) * result.axis1 +
                             0.5f * (vector2[2] + vector3[2]) * result.axis2;
            result.extents.x = 0.5f * (vector3[0] - vector2[0]);
            result.extents.y = 0.5f * (vector3[1] - vector2[1]);
            result.extents.z = 0.5f * (vector3[2] - vector2[2]);
            return result;
        }

        /// <summary>
        /// Returns axis by index (0, 1, 2)
        /// </summary>
        public FVector3 GetAxis(int index) {
            return index switch {
                0 => this.axis0,
                1 => this.axis1,
                2 => this.axis2,
                _ => FVector3.Zero,
            };
        }

        /// <summary>
        /// Calculates 8 box corners. extAxis[i] is Axis[i]*Extent[i], i=0,1,2
        /// </summary>
        /// <param name="vertex0">Center - extAxis0 - extAxis1 - extAxis2</param>
        /// <param name="vertex1">Center + extAxis0 - extAxis1 - extAxis2</param>
        /// <param name="vertex2">Center + extAxis0 + extAxis1 - extAxis2</param>
        /// <param name="vertex3">Center - extAxis0 + extAxis1 - extAxis2</param>
        /// <param name="vertex4">Center - extAxis0 - extAxis1 + extAxis2</param>
        /// <param name="vertex5">Center + extAxis0 - extAxis1 + extAxis2</param>
        /// <param name="vertex6">Center + extAxis0 + extAxis1 + extAxis2</param>
        /// <param name="vertex7">Center - extAxis0 + extAxis1 + extAxis2</param>
        public void CalcVertices(out FVector3 vertex0, out FVector3 vertex1, out FVector3 vertex2,
            out FVector3 vertex3, out FVector3 vertex4, out FVector3 vertex5, out FVector3 vertex6,
            out FVector3 vertex7) {
            FVector3 vector = this.extents.x * this.axis0;
            FVector3 vector2 = this.extents.y * this.axis1;
            FVector3 vector3 = this.extents.z * this.axis2;
            vertex0 = this.center - vector - vector2 - vector3;
            vertex1 = this.center + vector - vector2 - vector3;
            vertex2 = this.center + vector + vector2 - vector3;
            vertex3 = this.center - vector + vector2 - vector3;
            vertex4 = this.center - vector - vector2 + vector3;
            vertex5 = this.center + vector - vector2 + vector3;
            vertex6 = this.center + vector + vector2 + vector3;
            vertex7 = this.center - vector + vector2 + vector3;
        }

        /// <summary>
        /// Calculates 8 box corners and returns them in an allocated array.
        /// See array-less overload for the description.
        /// </summary>
        public FVector3[] CalcVertices() {
            FVector3 vector = this.extents.x * this.axis0;
            FVector3 vector2 = this.extents.y * this.axis1;
            FVector3 vector3 = this.extents.z * this.axis2;
            return new FVector3[8] {
                this.center - vector - vector2 - vector3, this.center + vector - vector2 - vector3, this.center + vector + vector2 - vector3,
                this.center - vector + vector2 - vector3, this.center - vector - vector2 + vector3, this.center + vector - vector2 + vector3,
                this.center + vector + vector2 + vector3, this.center - vector + vector2 + vector3
            };
        }

        /// <summary>
        /// Calculates 8 box corners and fills the input array with them (array length must be 8).
        /// See array-less overload for the description.
        /// </summary>
        public void CalcVertices(FVector3[] array) {
            FVector3 vector = this.extents.x * this.axis0;
            FVector3 vector2 = this.extents.y * this.axis1;
            FVector3 vector3 = this.extents.z * this.axis2;
            ref FVector3 reference = ref array[0];
            reference = this.center - vector - vector2 - vector3;
            ref FVector3 reference2 = ref array[1];
            reference2 = this.center + vector - vector2 - vector3;
            ref FVector3 reference3 = ref array[2];
            reference3 = this.center + vector + vector2 - vector3;
            ref FVector3 reference4 = ref array[3];
            reference4 = this.center - vector + vector2 - vector3;
            ref FVector3 reference5 = ref array[4];
            reference5 = this.center - vector - vector2 + vector3;
            ref FVector3 reference6 = ref array[5];
            reference6 = this.center + vector - vector2 + vector3;
            ref FVector3 reference7 = ref array[6];
            reference7 = this.center + vector + vector2 + vector3;
            ref FVector3 reference8 = ref array[7];
            reference8 = this.center - vector + vector2 + vector3;
        }

        /// <summary>
        /// Returns volume of the box as Extents.x * Extents.y * Extents.z * 8
        /// </summary>
        public FLOAT CalcVolume() {
            return 8f * this.extents.x * this.extents.y * this.extents.z;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(FVector3 point) {
            return Distance.Point3Box3(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public FVector3 Project(FVector3 point) {
            Distance.SqrPoint3Box3(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Tests whether a point is contained by the box
        /// </summary>
        public bool Contains(ref FVector3 point) {
            FVector3 vector = default(FVector3);
            vector.x = point.x - this.center.x;
            vector.y = point.y - this.center.y;
            vector.z = point.z - this.center.z;
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

            num = vector.Dot(this.axis2);
            if (num < 0f - this.extents.z) {
                return false;
            }

            if (num > this.extents.z) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether a point is contained by the box
        /// </summary>
        public bool Contains(FVector3 point) {
            return this.Contains(ref point);
        }

        /// <summary>
        /// Enlarges the box so it includes another box.
        /// </summary>
        public void Include(ref Box box) {
            Box box2 = default(Box);
            box2.center = 0.5f * (this.center + box.center);
            FMatrix4x4Ex.CreateRotationFromColumns(ref this.axis0, ref this.axis1, ref this.axis2, out var matrix);
            var quaternion = matrix.rotation;
            FMatrix4x4Ex.CreateRotationFromColumns(ref box.axis0, ref box.axis1, ref box.axis2, out var matrix2);
            var quaternion2 = matrix2.rotation;

            FQuaternion.Dot(ref quaternion, ref quaternion2, out var result);
            if (result < 0f) {
                quaternion2.x = 0f - quaternion2.x;
                quaternion2.y = 0f - quaternion2.y;
                quaternion2.z = 0f - quaternion2.z;
                quaternion2.w = 0f - quaternion2.w;
            }

            FQuaternion quaternion3 = default(FQuaternion);
            quaternion3.x = quaternion.x + quaternion2.x;
            quaternion3.y = quaternion.x + quaternion2.y;
            quaternion3.z = quaternion.x + quaternion2.z;
            quaternion3.w = quaternion.x + quaternion2.w;
            FQuaternion.Dot(ref quaternion3, ref quaternion3, out result);
            FLOAT num = FMathfEx.InvSqrt(result);
            quaternion3.x *= num;
            quaternion3.y *= num;
            quaternion3.z *= num;
            quaternion3.w *= num;
            FMatrix4x4.CreateRotation(ref quaternion3, out var matrix3);
            box2.axis0 = matrix3.GetColumn(0).ToFVector3();
            box2.axis1 = matrix3.GetColumn(1).ToFVector3();
            box2.axis2 = matrix3.GetColumn(2).ToFVector3();
            FVector3 zero = FVector3.Zero;
            FVector3 zero2 = FVector3.Zero;
            FVector3[] array = this.CalcVertices();
            for (int i = 0; i < 8; i++) {
                FVector3 vector = array[i] - box2.center;
                for (int j = 0; j < 3; j++) {
                    FLOAT num2 = vector.Dot(box2.GetAxis(j));
                    if (num2 > zero2[j]) {
                        zero2[j] = num2;
                    }
                    else if (num2 < zero[j]) {
                        zero[j] = num2;
                    }
                }
            }

            box.CalcVertices(out array[0], out array[1], out array[2], out array[3], out array[4], out array[5],
                out array[6], out array[7]);
            for (int i = 0; i < 8; i++) {
                FVector3 vector = array[i] - box2.center;
                for (int j = 0; j < 3; j++) {
                    FLOAT num2 = vector.Dot(box2.GetAxis(j));
                    if (num2 > zero2[j]) {
                        zero2[j] = num2;
                    }
                    else if (num2 < zero[j]) {
                        zero[j] = num2;
                    }
                }
            }

            for (int j = 0; j < 3; j++) {
                box2.center += 0.5f * (zero2[j] + zero[j]) * box2.GetAxis(j);
                box2.extents[j] = 0.5f * (zero2[j] - zero[j]);
            }

            this = box2;
        }

        /// <summary>
        /// Enlarges the box so it includes another box.
        /// </summary>
        public void Include(Box box) {
            this.Include(ref box);
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return
                $"[Center: {this.center.ToString()} Axis0: {this.axis0.ToString()} Axis1: {this.axis1.ToString()} Axis2: {this.axis2.ToString()} Extents: {this.extents.ToString()}]";
        }
    }
}