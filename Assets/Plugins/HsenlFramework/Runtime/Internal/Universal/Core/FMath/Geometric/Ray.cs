#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// The ray is represented as P+t*D, where P is the ray origin, D is a
    /// unit-length direction vector, and t &gt;= 0.  The user must ensure that D
    /// is indeed unit length.
    /// </summary>
    public struct Ray {
        /// <summary>
        /// Ray origin
        /// </summary>
        public Vector3 origin;

        /// <summary>
        /// Ray direction. Must be unit length!
        /// </summary>
        public Vector3 direction;

        /// <summary>
        /// Creates the ray
        /// </summary>
        /// <param name="origin">Ray origin</param>
        /// <param name="direction">Ray direction. Must be unit length!</param>
        public Ray(ref Vector3 origin, ref Vector3 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Creates the ray
        /// </summary>
        /// <param name="origin">Ray origin</param>
        /// <param name="direction">Ray direction. Must be unit length!</param>
        public Ray(Vector3 origin, Vector3 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Evaluates ray using P+t*D formula, where P is the ray origin, D is a
        /// unit-length direction vector, t is parameter.
        /// </summary>
        /// <param name="t">Evaluation parameter</param>
        public Vector3 Eval(FLOAT t) {
            return this.origin + this.direction * t;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector3 point) {
            return Distance.Point3Ray3(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public Vector3 Project(Vector3 point) {
            Distance.SqrPoint3Ray3(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[Origin: {this.origin.ToString()} Direction: {this.direction.ToString()}]";
        }
    }
}