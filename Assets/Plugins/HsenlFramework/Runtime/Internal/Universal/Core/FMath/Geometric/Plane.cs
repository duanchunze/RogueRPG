#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// The plane is represented as Dot(N,X) = c where N is a unit-length
    /// normal vector, c is the plane constant, and X is any point on the
    /// plane.  The user must ensure that the normal vector is unit length.
    /// </summary>
    public struct Plane {
        /// <summary>
        /// Plane normal. Must be unit length!
        /// </summary>
        public Vector3 normal;

        /// <summary>
        /// Plane constant c from the equation Dot(N,X) = c
        /// </summary>
        public FLOAT constant;

        /// <summary>
        /// Creates the plane by specifying N and c directly.
        /// </summary>
        /// <param name="normal">Must be unit length!</param>
        public Plane(ref Vector3 normal, FLOAT constant) {
            this.normal = normal;
            this.constant = constant;
        }

        /// <summary>
        /// Creates the plane by specifying N and c directly.
        /// </summary>
        /// <param name="normal">Must be unit length!</param>
        public Plane(Vector3 normal, FLOAT constant) {
            this.normal = normal;
            this.constant = constant;
        }

        /// <summary>
        /// N is specified, c = Dot(N,P) where P is a point on the plane.
        /// </summary>
        /// <param name="normal">Must be unit length!</param>
        public Plane(ref Vector3 normal, ref Vector3 point) {
            this.normal = normal;
            this.constant = normal.Dot(point);
        }

        /// <summary>
        /// N is specified, c = Dot(N,P) where P is a point on the plane.
        /// </summary>
        /// <param name="normal">Must be unit length!</param>
        public Plane(Vector3 normal, Vector3 point) {
            this.normal = normal;
            this.constant = normal.Dot(point);
        }

        /// <summary>
        /// Creates the plane from 3 points.
        /// N = Cross(P1-P0,P2-P0)/Length(Cross(P1-P0,P2-P0)), c = Dot(N,P0) where
        /// P0, P1, P2 are points on the plane.
        /// </summary>
        public Plane(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2) {
            Vector3 vector = p1 - p0;
            Vector3 value = p2 - p0;
            this.normal = vector.NormalizeCross(value);
            this.constant = this.normal.Dot(p0);
        }

        /// <summary>
        /// Creates the plane from 3 points.
        /// N = Cross(P1-P0,P2-P0)/Length(Cross(P1-P0,P2-P0)), c = Dot(N,P0) where
        /// P0, P1, P2 are points on the plane.
        /// </summary>
        public Plane(Vector3 p0, Vector3 p1, Vector3 p2) {
            Vector3 vector = p1 - p0;
            Vector3 value = p2 - p0;
            this.normal = vector.NormalizeCross(value);
            this.constant = this.normal.Dot(p0);
        }

        /// <summary>
        /// Returns N*c
        /// </summary>
        public Vector3 CalcOrigin() {
            return this.normal * this.constant;
        }

        /// <summary>
        /// Creates orthonormal basis from plane. In the output n - is the plane normal.
        /// </summary>
        public void CreateOrthonormalBasis(out Vector3 u, out Vector3 v, out Vector3 n) {
            n = this.normal;
            if (Math.Abs(n.x) >= Math.Abs(n.y)) {
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

            v = Vector3.Cross(n, u);
        }

        /// <summary>
        /// Compute d = Dot(N,P)-c where N is the plane normal and c is the plane
        /// constant.  This is a signed distance.  The sign of the return value is
        /// positive if the point is on the positive side of the plane, negative if
        /// the point is on the negative side, and zero if the point is on the plane.
        /// </summary>
        internal FLOAT SignedDistanceTo(ref Vector3 point) {
            return this.normal.Dot(point) - this.constant;
        }

        /// <summary>
        /// Compute d = Dot(N,P)-c where N is the plane normal and c is the plane
        /// constant.  This is a signed distance.  The sign of the return value is
        /// positive if the point is on the positive side of the plane, negative if
        /// the point is on the negative side, and zero if the point is on the plane.
        /// </summary>
        public FLOAT SignedDistanceTo(Vector3 point) {
            return this.normal.Dot(point) - this.constant;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector3 point) {
            return Math.Abs(this.normal.Dot(point) - this.constant);
        }

        /// <summary>
        /// Determines on which side of the plane a point is. Returns +1 if a point
        /// is on the positive side of the plane, 0 if it's on the plane, -1 if it's on the negative side.
        /// The positive side of the plane is the half-space to which the plane normal points.
        /// </summary>
        public int QuerySide(Vector3 point) {
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
        public int QuerySide(Vector3 point, FLOAT epsilon) {
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
        public bool QuerySideNegative(Vector3 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = this.normal.Dot(point) - this.constant;
            return num <= epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the negative side of the plane, false otherwise.
        /// </summary>
        public bool QuerySideNegative(Vector3 point, FLOAT epsilon) {
            FLOAT num = this.normal.Dot(point) - this.constant;
            return num <= epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(Vector3 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = this.normal.Dot(point) - this.constant;
            return num >= 0f - epsilon;
        }

        /// <summary>
        /// Returns true if a point is on the positive side of the plane, false otherwise.
        /// </summary>
        public bool QuerySidePositive(Vector3 point, FLOAT epsilon) {
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
            FLOAT num = Math.Abs(f) + Math.Abs(f2) + Math.Abs(f3);
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
            FLOAT num = Math.Abs(f) + Math.Abs(f2) + Math.Abs(f3);
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
            FLOAT num = Math.Abs(f) + Math.Abs(f2) + Math.Abs(f3);
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
            FLOAT num = Math.Abs(f) + Math.Abs(f2) + Math.Abs(f3);
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
            FLOAT num = Math.Abs(f) + Math.Abs(f2) + Math.Abs(f3);
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
            FLOAT num = Math.Abs(f) + Math.Abs(f2) + Math.Abs(f3);
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
            Vector3 value = default(Vector3);
            Vector3 value2 = default(Vector3);
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
            Vector3 value = default(Vector3);
            Vector3 value2 = default(Vector3);
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
            Vector3 value = default(Vector3);
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
            Vector3 value = default(Vector3);
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
            Vector3 value = default(Vector3);
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
            Vector3 value = default(Vector3);
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
        public Vector3 Project(Vector3 point) {
            Distance.SqrPoint3Plane3(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Returns projected vector
        /// </summary>
        public Vector3 ProjectVector(Vector3 vector) {
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

            return Math.Acos(num);
        }

        /// <summary>
        /// Returns angle in radians between plane normal and line direction which is: arccos(dot(normal,direction)). Direction will be normalized.
        /// </summary>
        public FLOAT AngleBetweenPlaneNormalAndLine(Vector3 direction) {
            Vector3.Normalize(ref direction);
            FLOAT num = this.normal.Dot(direction);
            if (num > 1f) {
                num = 1f;
            }
            else if (num < -1f) {
                num = -1f;
            }

            return Math.Acos(num);
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

            return Math.Pi / 2f - Math.Acos(num);
        }

        /// <summary>
        /// Returns angle in radians between plane itself and direction which is: pi/2 - arccos(dot(normal,direction)).  Direction will be normalized.
        /// </summary>
        public FLOAT AngleBetweenPlaneAndLine(Vector3 direction) {
            Vector3.Normalize(ref direction);
            FLOAT num = this.normal.Dot(direction);
            if (num > 1f) {
                num = 1f;
            }
            else if (num < -1f) {
                num = -1f;
            }

            return Math.Pi / 2f - Math.Acos(num);
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

            return Math.Acos(num);
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[Normal: {this.normal.ToString()} Constant: {this.constant.ToString()}]";
        }
    }
}