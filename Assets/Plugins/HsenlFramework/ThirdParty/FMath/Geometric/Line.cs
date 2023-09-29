﻿#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// The line is represented as P+t*D where P is the line origin, D is a
    /// unit-length direction vector, and t is any real number.  The user must
    /// ensure that D is indeed unit length.
    /// </summary>
    public struct Line {
        /// <summary>
        /// Line origin
        /// </summary>
        public FVector3 origin;

        /// <summary>
        /// Line direction. Must be unit length!
        /// </summary>
        public FVector3 direction;

        /// <summary>
        /// Creates the line
        /// </summary>
        /// <param name="origin">Line origin</param>
        /// <param name="direction">Line direction. Must be unit length!</param>
        public Line(ref FVector3 origin, ref FVector3 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Creates the line
        /// </summary>
        /// <param name="origin">Line origin</param>
        /// <param name="direction">Line direction. Must be unit length!</param>
        public Line(FVector3 origin, FVector3 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        /// <summary>
        /// Creates the line. Origin is p0, Direction is Normalized(p1-p0).
        /// </summary>
        /// <param name="p0">First point</param>
        /// <param name="p1">Second point</param>
        public static Line CreateFromTwoPoints(ref FVector3 p0, ref FVector3 p1) {
            Line result = default(Line);
            result.origin = p0;
            result.direction = (p1 - p0).normalized;
            return result;
        }

        /// <summary>
        /// Creates the line. Origin is p0, Direction is Normalized(p1-p0).
        /// </summary>
        /// <param name="p0">First point</param>
        /// <param name="p1">Second point</param>
        public static Line CreateFromTwoPoints(FVector3 p0, FVector3 p1) {
            Line result = default(Line);
            result.origin = p0;
            result.direction = (p1 - p0).normalized;
            return result;
        }

        /// <summary>
        /// Evaluates line using P+t*D formula, where P is the line origin, D is a
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
            return Distance.Point3Line3(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public FVector3 Project(FVector3 point) {
            Distance.SqrPoint3Line3(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Returns angle between this line's direction and another line's direction as: arccos(dot(this.Direction,another.Direction))
        /// If acuteAngleDesired is true, then in resulting angle is &gt; pi/2, then result is transformed to be pi-angle.
        /// </summary>
        public FLOAT AngleBetweenTwoLines(Line anotherLine, bool acuteAngleDesired = false) {
            FLOAT num = FMath.Acos(this.direction.Dot(anotherLine.direction));
            if (acuteAngleDesired && num > (FLOAT)FMath.Pi / 2f) {
                return (FLOAT)FMath.Pi - num;
            }

            return num;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[Origin: {this.origin.ToString()} Direction: {this.direction.ToString()}]";
        }
    }
}