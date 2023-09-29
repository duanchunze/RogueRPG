#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// The plane is represented as Dot(N,X) = c where N is a unit-length
    /// normal vector, c is the plane constant, and X is any point on the
    /// plane.  The user must ensure that the normal vector is unit length.
    /// </summary>
    public struct Plane {
        /// <summary>
        /// Plane normal. Must be unit length!
        /// </summary>
        public FVector3 normal;

        /// <summary>
        /// Plane constant c from the equation Dot(N,X) = c
        /// </summary>
        public FLOAT constant;

        /// <summary>
        /// Creates the plane by specifying N and c directly.
        /// </summary>
        /// <param name="normal">Must be unit length!</param>
        public Plane(ref FVector3 normal, FLOAT constant) {
            this.normal = normal;
            this.constant = constant;
        }

        /// <summary>
        /// Creates the plane by specifying N and c directly.
        /// </summary>
        /// <param name="normal">Must be unit length!</param>
        public Plane(FVector3 normal, FLOAT constant) {
            this.normal = normal;
            this.constant = constant;
        }

        /// <summary>
        /// N is specified, c = Dot(N,P) where P is a point on the plane.
        /// </summary>
        /// <param name="normal">Must be unit length!</param>
        public Plane(ref FVector3 normal, ref FVector3 point) {
            this.normal = normal;
            this.constant = normal.Dot(point);
        }

        /// <summary>
        /// N is specified, c = Dot(N,P) where P is a point on the plane.
        /// </summary>
        /// <param name="normal">Must be unit length!</param>
        public Plane(FVector3 normal, FVector3 point) {
            this.normal = normal;
            this.constant = normal.Dot(point);
        }

        /// <summary>
        /// Creates the plane from 3 points.
        /// N = Cross(P1-P0,P2-P0)/Length(Cross(P1-P0,P2-P0)), c = Dot(N,P0) where
        /// P0, P1, P2 are points on the plane.
        /// </summary>
        public Plane(ref FVector3 p0, ref FVector3 p1, ref FVector3 p2) {
            FVector3 vector = p1 - p0;
            FVector3 value = p2 - p0;
            this.normal = vector.NormalizeCross(value);
            this.constant = this.normal.Dot(p0);
        }

        /// <summary>
        /// Creates the plane from 3 points.
        /// N = Cross(P1-P0,P2-P0)/Length(Cross(P1-P0,P2-P0)), c = Dot(N,P0) where
        /// P0, P1, P2 are points on the plane.
        /// </summary>
        public Plane(FVector3 p0, FVector3 p1, FVector3 p2) {
            FVector3 vector = p1 - p0;
            FVector3 value = p2 - p0;
            this.normal = vector.NormalizeCross(value);
            this.constant = this.normal.Dot(p0);
        }

        /// <summary>
        /// Returns N*c
        /// </summary>
        public FVector3 CalcOrigin() {
            return this.normal * this.constant;
        }

        /// <summary>
        /// Creates orthonormal basis from plane. In the output n - is the plane normal.
        /// </summary>
        public void CreateOrthonormalBasis(out FVector3 u, out FVector3 v, out FVector3 n) {
            n = this.normal;
            if (FMath.Abs(n.x) >= FMath.Abs(n.y)) {
                FLOAT num = FMathfEx.InvSqrt(n.x * n.x + n.z * n.z);
                u.x = n.z * num;
                u.y = 0f;
                u.z = (0f - n.x) * num;
            }
            else {
                FLOAT num2 = FMathfEx.InvSqrt(n.y * n.y + n.z * n.z);
                u.x = 0f;
                u.y = n.z * num2;
                u.z = (0f - n.y) * num2;
            }

            v = FVector3.Cross(n, u);
        }

        /// <summary>
        /// Compute d = Dot(N,P)-c where N is the plane normal and c is the plane
        /// constant.  This is a signed distance.  The sign of the return value is
        /// positive if the point is on the positive side of the plane, negative if
        /// the point is on the negative side, and zero if the point is on the plane.
        /// </summary>
        internal FLOAT SignedDistanceTo(ref FVector3 point) {
            return this.normal.Dot(point) - this.constant;
        }

        /// <summary>
        /// Compute d = Dot(N,P)-c where N is the plane normal and c is the plane
        /// constant.  This is a signed distance.  The sign of the return value is
        /// positive if the point is on the positive side of the plane, negative if
        /// the point is on the negative side, and zero if the point is on the plane.
        /// </summary>
        public FLOAT SignedDistanceTo(FVector3 point) {
            return this.normal.Dot(point) - this.constant;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(FVector3 point) {
            return FMath.Abs(this.normal.Dot(point) - this.constant);
        }

        /// <summary>
        /// Determines on which side of the plane a point is. Returns +1 if a point
        /// is on the positive side of the plane, 0 if it's on the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(FVector3 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = this.normal.Dot(point) - this.constant;
            if (num < 0f - epsilon) {
                return -1;
            }

            if (num > epsilon) {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Determines on which side of the plane a point is. Returns +1 if a point
        /// is on the positive side of the plane, 0 if it's on the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(FVector3 point, FLOAT epsilon) {
            FLOAT num = this.normal.Dot(point) - this.constant;
            if (num < 0f - epsilon) {
                return -1;
            }

            if (num > epsilon) {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Returns true if a point is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(FVector3 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = this.normal.Dot(point) - this.constant;
            return num <= epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(FVector3 point, FLOAT epsilon) {
            FLOAT num = this.normal.Dot(point) - this.constant;
            return num <= epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(FVector3 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = this.normal.Dot(point) - this.constant;
            return num >= 0f - epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(FVector3 point, FLOAT epsilon) {
            FLOAT num = this.normal.Dot(point) - this.constant;
            return num >= 0f - epsilon;
        }

        /// <summary>
        /// Determines on which side of the plane a box is. Returns +1 if a box
        /// is on the positive side of the plane, 0 if it's intersecting the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(ref Box box) {
            FLOAT epsilon = 1E-05f;
            FLOAT f = box.extents.x * this.normal.Dot(box.axis0);
            FLOAT f2 = box.extents.y * this.normal.Dot(box.axis1);
            FLOAT f3 = box.extents.z * this.normal.Dot(box.axis2);
            FLOAT num = FMath.Abs(f) + FMath.Abs(f2) + FMath.Abs(f3);
            FLOAT num2 = this.normal.Dot(box.center) - this.constant;
            if (!(num2 < 0f - num + epsilon)) {
                if (!(num2 > num - epsilon)) {
                    return 0;
                }

                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Determines on which side of the plane a box is. Returns +1 if a box
        /// is on the positive side of the plane, 0 if it's intersecting the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(ref Box box, FLOAT epsilon) {
            FLOAT f = box.extents.x * this.normal.Dot(box.axis0);
            FLOAT f2 = box.extents.y * this.normal.Dot(box.axis1);
            FLOAT f3 = box.extents.z * this.normal.Dot(box.axis2);
            FLOAT num = FMath.Abs(f) + FMath.Abs(f2) + FMath.Abs(f3);
            FLOAT num2 = this.normal.Dot(box.center) - this.constant;
            if (!(num2 < 0f - num + epsilon)) {
                if (!(num2 > num - epsilon)) {
                    return 0;
                }

                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Returns true if a box is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref Box box) {
            FLOAT epsilon = 1E-05f;
            FLOAT f = box.extents.x * this.normal.Dot(box.axis0);
            FLOAT f2 = box.extents.y * this.normal.Dot(box.axis1);
            FLOAT f3 = box.extents.z * this.normal.Dot(box.axis2);
            FLOAT num = FMath.Abs(f) + FMath.Abs(f2) + FMath.Abs(f3);
            FLOAT num2 = this.normal.Dot(box.center) - this.constant;
            return num2 <= 0f - num + epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref Box box, FLOAT epsilon) {
            FLOAT f = box.extents.x * this.normal.Dot(box.axis0);
            FLOAT f2 = box.extents.y * this.normal.Dot(box.axis1);
            FLOAT f3 = box.extents.z * this.normal.Dot(box.axis2);
            FLOAT num = FMath.Abs(f) + FMath.Abs(f2) + FMath.Abs(f3);
            FLOAT num2 = this.normal.Dot(box.center) - this.constant;
            return num2 <= 0f - num + epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref Box box) {
            FLOAT epsilon = 1E-05f;
            FLOAT f = box.extents.x * this.normal.Dot(box.axis0);
            FLOAT f2 = box.extents.y * this.normal.Dot(box.axis1);
            FLOAT f3 = box.extents.z * this.normal.Dot(box.axis2);
            FLOAT num = FMath.Abs(f) + FMath.Abs(f2) + FMath.Abs(f3);
            FLOAT num2 = this.normal.Dot(box.center) - this.constant;
            return num2 >= num - epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref Box box, FLOAT epsilon) {
            FLOAT f = box.extents.x * this.normal.Dot(box.axis0);
            FLOAT f2 = box.extents.y * this.normal.Dot(box.axis1);
            FLOAT f3 = box.extents.z * this.normal.Dot(box.axis2);
            FLOAT num = FMath.Abs(f) + FMath.Abs(f2) + FMath.Abs(f3);
            FLOAT num2 = this.normal.Dot(box.center) - this.constant;
            return num2 >= num - epsilon;
        }

        /// <summary>
        /// Determines on which side of the plane a box is. Returns +1 if a box
        /// is on the positive side of the plane, 0 if it's intersecting the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(ref AABB box) {
            FLOAT epsilon = 1E-05f;
            FVector3 value = default(FVector3);
            FVector3 value2 = default(FVector3);
            if (this.normal.x >= 0f) {
                value.x = box.min.x;
                value2.x = box.max.x;
            }
            else {
                value.x = box.max.x;
                value2.x = box.min.x;
            }

            if (this.normal.y >= 0f) {
                value.y = box.min.y;
                value2.y = box.max.y;
            }
            else {
                value.y = box.max.y;
                value2.y = box.min.y;
            }

            if (this.normal.z >= 0f) {
                value.z = box.min.z;
                value2.z = box.max.z;
            }
            else {
                value.z = box.max.z;
                value2.z = box.min.z;
            }

            if (this.normal.Dot(value) - this.constant > 0f - epsilon) {
                return 1;
            }

            if (this.normal.Dot(value2) - this.constant < epsilon) {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Determines on which side of the plane a box is. Returns +1 if a box
        /// is on the positive side of the plane, 0 if it's intersecting the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(ref AABB box, FLOAT epsilon) {
            FVector3 value = default(FVector3);
            FVector3 value2 = default(FVector3);
            if (this.normal.x >= 0f) {
                value.x = box.min.x;
                value2.x = box.max.x;
            }
            else {
                value.x = box.max.x;
                value2.x = box.min.x;
            }

            if (this.normal.y >= 0f) {
                value.y = box.min.y;
                value2.y = box.max.y;
            }
            else {
                value.y = box.max.y;
                value2.y = box.min.y;
            }

            if (this.normal.z >= 0f) {
                value.z = box.min.z;
                value2.z = box.max.z;
            }
            else {
                value.z = box.max.z;
                value2.z = box.min.z;
            }

            if (this.normal.Dot(value) - this.constant > 0f - epsilon) {
                return 1;
            }

            if (this.normal.Dot(value2) - this.constant < epsilon) {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Returns true if a box is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref AABB box) {
            FLOAT epsilon = 1E-05f;
            FVector3 value = default(FVector3);
            if (this.normal.x >= 0f) {
                value.x = box.max.x;
            }
            else {
                value.x = box.min.x;
            }

            if (this.normal.y >= 0f) {
                value.y = box.max.y;
            }
            else {
                value.y = box.min.y;
            }

            if (this.normal.z >= 0f) {
                value.z = box.max.z;
            }
            else {
                value.z = box.min.z;
            }

            return this.normal.Dot(value) - this.constant <= epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref AABB box, FLOAT epsilon) {
            FVector3 value = default(FVector3);
            if (this.normal.x >= 0f) {
                value.x = box.max.x;
            }
            else {
                value.x = box.min.x;
            }

            if (this.normal.y >= 0f) {
                value.y = box.max.y;
            }
            else {
                value.y = box.min.y;
            }

            if (this.normal.z >= 0f) {
                value.z = box.max.z;
            }
            else {
                value.z = box.min.z;
            }

            return this.normal.Dot(value) - this.constant <= epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref AABB box) {
            FLOAT epsilon = 1E-05f;
            FVector3 value = default(FVector3);
            if (this.normal.x >= 0f) {
                value.x = box.min.x;
            }
            else {
                value.x = box.max.x;
            }

            if (this.normal.y >= 0f) {
                value.y = box.min.y;
            }
            else {
                value.y = box.max.y;
            }

            if (this.normal.z >= 0f) {
                value.z = box.min.z;
            }
            else {
                value.z = box.max.z;
            }

            return this.normal.Dot(value) - this.constant >= 0f - epsilon;
        }

        /// <summary>
        /// Returns true if a box is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref AABB box, FLOAT epsilon) {
            FVector3 value = default(FVector3);
            if (this.normal.x >= 0f) {
                value.x = box.min.x;
            }
            else {
                value.x = box.max.x;
            }

            if (this.normal.y >= 0f) {
                value.y = box.min.y;
            }
            else {
                value.y = box.max.y;
            }

            if (this.normal.z >= 0f) {
                value.z = box.min.z;
            }
            else {
                value.z = box.max.z;
            }

            return this.normal.Dot(value) - this.constant >= 0f - epsilon;
        }

        /// <summary>
        /// Determines on which side of the plane a sphere is. Returns +1 if a sphere
        /// is on the positive side of the plane, 0 if it's intersecting the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(ref Sphere sphere) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = this.normal.Dot(sphere.center) - this.constant;
            if (!(num > sphere.radius - epsilon)) {
                if (!(num < 0f - sphere.radius + epsilon)) {
                    return 0;
                }

                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Determines on which side of the plane a sphere is. Returns +1 if a sphere
        /// is on the positive side of the plane, 0 if it's intersecting the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(ref Sphere sphere, FLOAT epsilon) {
            FLOAT num = this.normal.Dot(sphere.center) - this.constant;
            if (!(num > sphere.radius - epsilon)) {
                if (!(num < 0f - sphere.radius + epsilon)) {
                    return 0;
                }

                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Returns true if a sphere is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref Sphere sphere) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = this.normal.Dot(sphere.center) - this.constant;
            return num <= 0f - sphere.radius + epsilon;
        }

        /// <summary>
        /// Returns true if a sphere is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(ref Sphere sphere, FLOAT epsilon) {
            FLOAT num = this.normal.Dot(sphere.center) - this.constant;
            return num <= 0f - sphere.radius + epsilon;
        }

        /// <summary>
        /// Returns true if a sphere is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref Sphere sphere) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = this.normal.Dot(sphere.center) - this.constant;
            return num >= sphere.radius - epsilon;
        }

        /// <summary>
        /// Returns true if a sphere is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(ref Sphere sphere, FLOAT epsilon) {
            FLOAT num = this.normal.Dot(sphere.center) - this.constant;
            return num >= sphere.radius - epsilon;
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public FVector3 Project(FVector3 point) {
            Distance.SqrPoint3Plane3(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Returns projected vector
        /// </summary>
        public FVector3 ProjectVector(FVector3 vector) {
            return vector - this.normal.Dot(vector) * this.normal;
        }

        /// <summary>
        /// Returns angle in radians between plane normal and line direction which is: arccos(dot(normal,direction))
        /// </summary>
        public FLOAT AngleBetweenPlaneNormalAndLine(Line line) {
            FLOAT num = this.normal.Dot(line.direction);
            if (num > 1f) {
                num = 1f;
            }
            else if (num < -1f) {
                num = -1f;
            }

            return FMath.Acos(num);
        }

        /// <summary>
        /// Returns angle in radians between plane normal and line direction which is: arccos(dot(normal,direction)). Direction will be normalized.
        /// </summary>
        public FLOAT AngleBetweenPlaneNormalAndLine(FVector3 direction) {
            FVector3.Normalize(ref direction);
            FLOAT num = this.normal.Dot(direction);
            if (num > 1f) {
                num = 1f;
            }
            else if (num < -1f) {
                num = -1f;
            }

            return FMath.Acos(num);
        }

        /// <summary>
        /// Returns angle between plane itself and line direction which is: pi/2 - arccos(dot(normal,direction))
        /// </summary>
        public FLOAT AngleBetweenPlaneAndLine(Line line) {
            FLOAT num = this.normal.Dot(line.direction);
            if (num > 1f) {
                num = 1f;
            }
            else if (num < -1f) {
                num = -1f;
            }

            return FMath.Pi / 2f - FMath.Acos(num);
        }

        /// <summary>
        /// Returns angle in radians between plane itself and direction which is: pi/2 - arccos(dot(normal,direction)).  Direction will be normalized.
        /// </summary>
        public FLOAT AngleBetweenPlaneAndLine(FVector3 direction) {
            FVector3.Normalize(ref direction);
            FLOAT num = this.normal.Dot(direction);
            if (num > 1f) {
                num = 1f;
            }
            else if (num < -1f) {
                num = -1f;
            }

            return FMath.Pi / 2f - FMath.Acos(num);
        }

        /// <summary>
        /// Returns angle in radians between this plane's normal and another plane's normal as: arccos(dot(this.Normal,another.Normal))
        /// </summary>
        public FLOAT AngleBetweenTwoPlanes(Plane anotherPlane) {
            FLOAT num = this.normal.Dot(anotherPlane.normal);
            if (num > 1f) {
                num = 1f;
            }
            else if (num < -1f) {
                num = -1f;
            }

            return FMath.Acos(num);
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[Normal: {this.normal.ToString()} Constant: {this.constant.ToString()}]";
        }
    }
}