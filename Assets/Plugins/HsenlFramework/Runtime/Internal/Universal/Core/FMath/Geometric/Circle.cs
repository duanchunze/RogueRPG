#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// The plane containing the circle is Dot(N,X-C) = 0, where X is any point
    /// in the plane.  Vectors U, V, and N form an orthonormal set
    /// (matrix [U V N] is orthonormal and has determinant 1).  The circle
    /// within the plane is parameterized by X = C + R*(cos(t)*U + sin(t)*V),
    /// where t is an angle in [0,2*pi).
    /// </summary>
    public struct Circle {
        /// <summary>
        /// Circle center.
        /// </summary>
        public Vector3 center;

        /// <summary>
        /// First circle axis. Must be unit length!
        /// </summary>
        public Vector3 axis0;

        /// <summary>
        /// Second circle axis. Must be unit length!
        /// </summary>
        public Vector3 axis1;

        /// <summary>
        /// Circle normal which is Cross(Axis0, Axis1). Must be unit length!
        /// </summary>
        public Vector3 normal;

        /// <summary>
        /// Circle radius.
        /// </summary>
        public FLOAT radius;

        /// <summary>
        /// Creates new circle instance from center, axes and radius. Normal is calculated as cross product of the axes.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="axis0">Must be unit length!</param>
        /// <param name="axis1">Must be unit length!</param>
        /// <param name="radius"></param>
        public Circle(ref Vector3 center, ref Vector3 axis0, ref Vector3 axis1, FLOAT radius) {
            this.center = center;
            this.axis0 = axis0;
            this.axis1 = axis1;
            this.normal = axis0.Cross(axis1);
            this.radius = radius;
        }

        /// <summary>
        /// Creates new circle instance from center, axes and radius. Normal is calculated as cross product of the axes.
        /// </summary>
        public Circle(Vector3 center, Vector3 axis0, Vector3 axis1, FLOAT radius) {
            this.center = center;
            this.axis0 = axis0;
            this.axis1 = axis1;
            this.normal = axis0.Cross(axis1);
            this.radius = radius;
        }

        /// <summary>
        /// Creates new circle instance. Computes axes from specified normal.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal">Must be unit length!</param>
        /// <param name="radius"></param>
        public Circle(ref Vector3 center, ref Vector3 normal, FLOAT radius) {
            this.center = center;
            this.normal = normal;
            FVector3Ex.CreateOrthonormalBasis(out this.axis0, out this.axis1, ref this.normal);
            this.radius = radius;
        }

        /// <summary>
        /// Creates new circle instance. Computes axes from specified normal.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal">Must be unit length!</param>
        /// <param name="radius"></param>
        public Circle(Vector3 center, Vector3 normal, FLOAT radius) {
            this.center = center;
            this.normal = normal;
            FVector3Ex.CreateOrthonormalBasis(out this.axis0, out this.axis1, ref this.normal);
            this.radius = radius;
        }

        /// <summary>
        /// Creates circle which is circumscribed around triangle.
        /// Returns 'true' if circle has been constructed, 'false' otherwise (input points are linearly dependent).
        /// </summary>
        public static bool CreateCircumscribed(Vector3 v0, Vector3 v1, Vector3 v2, out Circle circle) {
            Vector3 vector = v0 - v2;
            Vector3 vector2 = v1 - v2;
            FLOAT num = vector.Dot(vector);
            FLOAT num2 = vector.Dot(vector2);
            FLOAT num3 = vector2.Dot(vector2);
            FLOAT num4 = num * num3 - num2 * num2;
            if (Math.Abs(num4) < 1E-05f) {
                circle = default(Circle);
                return false;
            }

            FLOAT num5 = 0.5f / num4;
            FLOAT num6 = num5 * num3 * (num - num2);
            FLOAT num7 = num5 * num * (num3 - num2);
            Vector3 vector3 = num6 * vector + num7 * vector2;
            circle.center = v2 + vector3;
            circle.radius = vector3.magnitude;
            circle.normal = vector.NormalizeCross(vector2);
            if (Math.Abs(circle.normal.x) >= Math.Abs(circle.normal.y) &&
                Math.Abs(circle.normal.x) >= Math.Abs(circle.normal.z)) {
                circle.axis0.x = 0f - circle.normal.y;
                circle.axis0.y = circle.normal.x;
                circle.axis0.z = 0f;
            }
            else {
                circle.axis0.x = 0f;
                circle.axis0.y = circle.normal.z;
                circle.axis0.z = 0f - circle.normal.y;
            }

            circle.axis0.Normalize();
            circle.axis1 = circle.normal.Cross(circle.axis0);
            return true;
        }

        /// <summary>
        /// Creates circle which is insribed into triangle.
        /// Returns 'true' if circle has been constructed, 'false' otherwise (input points are linearly dependent).
        /// </summary>
        public static bool CreateInscribed(Vector3 v0, Vector3 v1, Vector3 v2, out Circle circle) {
            Vector3 value = v1 - v0;
            Vector3 vector = v2 - v1;
            Vector3 value2 = v0 - v2;
            circle.normal = vector.Cross(value);
            Vector3 vector2 = circle.normal.NormalizeCross(value);
            Vector3 vector3 = circle.normal.NormalizeCross(vector);
            Vector3 vector4 = circle.normal.NormalizeCross(value2);
            FLOAT num = vector3.Dot(value);
            if (Math.Abs(num) < 1E-05f) {
                circle = default(Circle);
                return false;
            }

            FLOAT num2 = vector4.Dot(vector);
            if (Math.Abs(num2) < 1E-05f) {
                circle = default(Circle);
                return false;
            }

            FLOAT num3 = vector2.Dot(value2);
            if (Math.Abs(num3) < 1E-05f) {
                circle = default(Circle);
                return false;
            }

            FLOAT num4 = 1f / num;
            FLOAT num5 = 1f / num2;
            FLOAT num6 = 1f / num3;
            circle.radius = 1f / (num4 + num5 + num6);
            circle.center = circle.radius * (num4 * v0 + num5 * v1 + num6 * v2);
            circle.normal.Normalize();
            circle.axis0 = vector2;
            circle.axis1 = circle.normal.Cross(circle.axis0);
            return true;
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
        /// Evaluates circle using formula X = C + R*cos(t)*U + R*sin(t)*V
        /// where t is an angle in [0,2*pi).
        /// </summary>
        /// <param name="t">Evaluation parameter</param>
        public Vector3 Eval(FLOAT t) {
            return this.center + this.radius * (Math.Cos(t) * this.axis0 + Math.Sin(t) * this.axis1);
        }

        /// <summary>
        /// Evaluates disk using formula X = C + radius*cos(t)*U + radius*sin(t)*V
        /// where t is an angle in [0,2*pi).
        /// </summary>
        /// <param name="t">Evaluation parameter</param>
        /// <param name="radius">Evaluation radius</param>
        public Vector3 Eval(FLOAT t, FLOAT radius) {
            return this.center + radius * (Math.Cos(t) * this.axis0 + Math.Sin(t) * this.axis1);
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector3 point, bool solid = true) {
            return Distance.Point3Circle3(ref point, ref this, solid);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public Vector3 Project(Vector3 point, bool solid = true) {
            Distance.SqrPoint3Circle3(ref point, ref this, out var closestPoint, solid);
            return closestPoint;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return
                $"[Center: {this.center.ToString()} Axis0: {this.axis0.ToString()} Axis1: {this.axis1.ToString()} Normal: {this.normal.ToString()} Radius: {this.radius.ToString()}]";
        }
    }
}