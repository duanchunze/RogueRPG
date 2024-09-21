#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// The segment is represented as (1-s)*P0+s*P1, where P0 and P1 are the
    /// endpoints of the segment and 0 &lt;= s &lt;= 1.
    ///
    /// Some algorithms involving segments might prefer a centered
    /// representation similar to how oriented bounding boxes are defined.
    /// This representation is C+t*D, where C = (P0+P1)/2 is the center of
    /// the segment, D = (P1-P0)/Length(P1-P0) is a unit-length direction
    /// vector for the segment, and |t| &lt;= e.  The value e = Length(P1-P0)/2
    /// is the 'extent' (or radius or half-length) of the segment.
    /// </summary>
    public struct Segment2D {
        /// <summary>
        /// Start point
        /// </summary>
        public Vector2 p0;

        /// <summary>
        /// End point
        /// </summary>
        public Vector2 p1;

        /// <summary>
        /// Segment center
        /// </summary>
        public Vector2 center;

        /// <summary>
        /// Segment direction. Must be unit length!
        /// </summary>
        public Vector2 direction;

        /// <summary>
        /// Segment half-length
        /// </summary>
        public FLOAT extent;

        /// <summary>
        /// The constructor computes Center, Dircetion, and Extent from P0 and P1.
        /// </summary>
        /// <param name="p0">Segment start point</param>
        /// <param name="p1">Segment end point</param>
        public Segment2D(ref Vector2 p0, ref Vector2 p1) {
            this.p0 = p0;
            this.p1 = p1;
            this.center = (this.direction = Vector2.Zero);
            this.extent = 0f;
            this.CalcCenterDirectionExtent();
        }

        /// <summary>
        /// The constructor computes Center, Dircetion, and Extent from P0 and P1.
        /// </summary>
        /// <param name="p0">Segment start point</param>
        /// <param name="p1">Segment end point</param>
        public Segment2D(Vector2 p0, Vector2 p1) {
            this.p0 = p0;
            this.p1 = p1;
            this.center = (this.direction = Vector2.Zero);
            this.extent = 0f;
            this.CalcCenterDirectionExtent();
        }

        /// <summary>
        /// The constructor computes P0 and P1 from Center, Direction, and Extent.
        /// </summary>
        /// <param name="center">Center of the segment</param>
        /// <param name="direction">Direction of the segment. Must be unit length!</param>
        /// <param name="extent">Half-length of the segment</param>
        public Segment2D(ref Vector2 center, ref Vector2 direction, FLOAT extent) {
            this.center = center;
            this.direction = direction;
            this.extent = extent;
            this.p0 = (this.p1 = Vector2.Zero);
            this.CalcEndPoints();
        }

        /// <summary>
        /// The constructor computes P0 and P1 from Center, Direction, and Extent.
        /// </summary>
        /// <param name="center">Center of the segment</param>
        /// <param name="direction">Direction of the segment. Must be unit length!</param>
        /// <param name="extent">Half-length of the segment</param>
        public Segment2D(Vector2 center, Vector2 direction, FLOAT extent) {
            this.center = center;
            this.direction = direction;
            this.extent = extent;
            this.p0 = (this.p1 = Vector2.Zero);
            this.CalcEndPoints();
        }

        /// <summary>
        /// Initializes segments from endpoints.
        /// </summary>
        public void SetEndpoints(Vector2 p0, Vector2 p1) {
            this.p0 = p0;
            this.p1 = p1;
            this.CalcCenterDirectionExtent();
        }

        /// <summary>
        /// Initializes segment from center, direction and extent.
        /// </summary>
        public void SetCenterDirectionExtent(Vector2 center, Vector2 direction, FLOAT extent) {
            this.center = center;
            this.direction = direction;
            this.extent = extent;
            this.CalcEndPoints();
        }

        /// <summary>
        /// Call this function when you change P0 or P1.
        /// </summary>
        public void CalcCenterDirectionExtent() {
            this.center = 0.5f * (this.p0 + this.p1);
            this.direction = this.p1 - this.p0;
            FLOAT magnitude = this.direction.magnitude;
            FLOAT num = 1f / magnitude;
            this.direction *= num;
            this.extent = 0.5f * magnitude;
        }

        /// <summary>
        /// Call this function when you change Center, Direction, or Extent.
        /// </summary>
        public void CalcEndPoints() {
            this.p0 = this.center - this.extent * this.direction;
            this.p1 = this.center + this.extent * this.direction;
        }

        /// <summary>
        /// Evaluates segment using (1-s)*P0+s*P1 formula, where P0 and P1
        /// are endpoints, s is parameter.
        /// </summary>
        /// <param name="s">Evaluation parameter</param>
        public Vector2 Eval(FLOAT s) {
            return (1f - s) * this.p0 + s * this.p1;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector2 point) {
            return Distance.Point2Segment2(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public Vector2 Project(Vector2 point) {
            Distance.SqrPoint2Segment2(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return
                $"[P0: {this.p0.ToString()} P1: {this.p1.ToString()} Center: {this.center.ToString()} Direction: {this.direction.ToString()} Extent: {this.extent.ToString()}]";
        }
    }
}