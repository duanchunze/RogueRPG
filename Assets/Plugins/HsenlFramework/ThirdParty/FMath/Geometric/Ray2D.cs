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
    public struct Ray2D {
        /// <summary>
        /// Ray origin
        /// </summary>
        public FVector2 origin;

        /// <summary>
        /// Ray direction. Must be unit length!
        /// </summary>
        public FVector2 direction;

        /// <summary>
        /// Creates the ray
        /// </summary>
        /// <param name="origin">Ray origin</param>
        /// <param name="direction">Ray direction. Must be unit length!</param>
        public Ray2D(ref FVector2 origin, ref FVector2 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Creates the ray
        /// </summary>
        /// <param name="origin">Ray origin</param>
        /// <param name="direction">Ray direction. Must be unit length!</param>
        public Ray2D(FVector2 origin, FVector2 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Evaluates ray using P+t*D formula, where P is the ray origin, D is a
        /// unit-length direction vector, t is parameter.
        /// </summary>
        /// <param name="t">Evaluation parameter</param>
        public FVector2 Eval(FLOAT t) {
            return this.origin + this.direction * t;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(FVector2 point) {
            return Distance.Point2Ray2(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public FVector2 Project(FVector2 point) {
            Distance.SqrPoint2Ray2(ref point, ref this, out var closestPoint);
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