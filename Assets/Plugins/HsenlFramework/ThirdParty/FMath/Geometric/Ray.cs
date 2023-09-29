#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// The ray is represented as P+t*D, where P is the ray origin, D is a
    /// unit-length direction vector, and t &gt;= 0.  The user must ensure that D
    /// is indeed unit length.
    /// </summary>
    public struct Ray {
        /// <summary>
        /// Ray origin
        /// </summary>
        public FVector3 origin;

        /// <summary>
        /// Ray direction. Must be unit length!
        /// </summary>
        public FVector3 direction;

        /// <summary>
        /// Creates the ray
        /// </summary>
        /// <param name="origin">Ray origin</param>
        /// <param name="direction">Ray direction. Must be unit length!</param>
        public Ray(ref FVector3 origin, ref FVector3 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Creates the ray
        /// </summary>
        /// <param name="origin">Ray origin</param>
        /// <param name="direction">Ray direction. Must be unit length!</param>
        public Ray(FVector3 origin, FVector3 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Evaluates ray using P+t*D formula, where P is the ray origin, D is a
        /// unit-length direction vector, t is parameter.
        /// </summary>
        /// <param name="t">Evaluation parameter</param>
        public FVector3 Eval(FLOAT t) {
            return this.origin + this.direction * t;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(FVector3 point) {
            return Distance.Point3Ray3(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public FVector3 Project(FVector3 point) {
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