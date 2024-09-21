#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public struct Triangle2D {
        /// <summary>
        /// First triangle vertex
        /// </summary>
        public Vector2 v0;

        /// <summary>
        /// Second triangle vertex
        /// </summary>
        public Vector2 v1;

        /// <summary>
        /// Third triangle vertex
        /// </summary>
        public Vector2 v2;

        /// <summary>
        /// Gets or sets triangle vertex by index: 0, 1 or 2
        /// </summary>
        public Vector2 this[int index] {
            get {
                return index switch {
                    0 => this.v0,
                    1 => this.v1,
                    2 => this.v2,
                    _ => Vector2.Zero,
                };
            }
            set {
                switch (index) {
                    case 0:
                        this.v0 = value;
                        break;
                    case 1:
                        this.v1 = value;
                        break;
                    case 2:
                        this.v2 = value;
                        break;
                }
            }
        }

        /// <summary>
        /// Creates Triangle2 from 3 vertices
        /// </summary>
        public Triangle2D(ref Vector2 v0, ref Vector2 v1, ref Vector2 v2) {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }

        /// <summary>
        /// Creates Triangle2 from 3 vertices
        /// </summary>
        public Triangle2D(Vector2 v0, Vector2 v1, Vector2 v2) {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }

        /// <summary>
        /// Returns triangle edge by index 0, 1 or 2
        /// Edge[i] = V[i+1]-V[i]
        /// </summary>
        public Vector2 CalcEdge(int edgeIndex) {
            return edgeIndex switch {
                0 => this.v1 - this.v0,
                1 => this.v2 - this.v1,
                2 => this.v0 - this.v2,
                _ => Vector2.Zero,
            };
        }

        /// <summary>
        /// Calculates cross product of triangle edges: (V1-V0)x(V2-V0).
        /// If the result is positive then triangle is ordered counter clockwise,
        /// if the result is negative then triangle is ordered clockwise,
        /// if the result is zero then triangle is degenerate.
        /// </summary>
        public FLOAT CalcDeterminant() {
            return this.v1.x * this.v2.y + this.v0.x * this.v1.y + this.v2.x * this.v0.y - this.v1.x * this.v0.y - this.v2.x * this.v1.y -
                   this.v0.x * this.v2.y;
        }

        /// <summary>
        /// Calculates triangle orientation. See CalcDeterminant() for the description.
        /// </summary>
        public Orientations CalcOrientation() {
            FLOAT threshold = 1E-05f;
            FLOAT num = this.CalcDeterminant();
            if (num > threshold) {
                return Orientations.CCW;
            }

            if (num < 0f - threshold) {
                return Orientations.CW;
            }

            return Orientations.None;
        }

        /// <summary>
        /// Calculates triangle orientation. See CalcDeterminant() for the description.
        /// </summary>
        public Orientations CalcOrientation(FLOAT threshold) {
            FLOAT num = this.CalcDeterminant();
            if (num > threshold) {
                return Orientations.CCW;
            }

            if (num < 0f - threshold) {
                return Orientations.CW;
            }

            return Orientations.None;
        }

        /// <summary>
        /// Calculates area of the triangle. It's equal to Abs(Determinant())/2
        /// </summary>
        /// <returns></returns>
        public FLOAT CalcArea() {
            return 0.5f * Math.Abs(this.CalcDeterminant());
        }

        /// <summary>
        /// Calculates area of the triangle defined by 3 points.
        /// </summary>
        public static FLOAT CalcArea(ref Vector2 v0, ref Vector2 v1, ref Vector2 v2) {
            return 0.5f *
                   Math.Abs(v1.x * v2.y + v0.x * v1.y + v2.x * v0.y - v1.x * v0.y - v2.x * v1.y - v0.x * v2.y);
        }

        /// <summary>
        /// Calculates area of the triangle defined by 3 points.
        /// </summary>
        public static FLOAT CalcArea(Vector2 v0, Vector2 v1, Vector2 v2) {
            return 0.5f *
                   Math.Abs(v1.x * v2.y + v0.x * v1.y + v2.x * v0.y - v1.x * v0.y - v2.x * v1.y - v0.x * v2.y);
        }

        /// <summary>
        /// Calculates angles of the triangle in degrees.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public Vector3 CalcAnglesDeg() {
            FLOAT num = this.v2.x - this.v1.x;
            FLOAT num2 = this.v2.y - this.v1.y;
            FLOAT num3 = num * num + num2 * num2;
            num = this.v2.x - this.v0.x;
            num2 = this.v2.y - this.v0.y;
            FLOAT num4 = num * num + num2 * num2;
            num = this.v1.x - this.v0.x;
            num2 = this.v1.y - this.v0.y;
            FLOAT num5 = num * num + num2 * num2;
            FLOAT num6 = 2f * Math.Sqrt(num5);
            Vector3 result = default(Vector3);
            result.x = Math.Acos((num4 + num5 - num3) / (Math.Sqrt(num4) * num6)) * 57.29578f;
            result.y = Math.Acos((num3 + num5 - num4) / (Math.Sqrt(num3) * num6)) * 57.29578f;
            result.z = 180f - result.x - result.y;
            return result;
        }

        /// <summary>
        /// Calculates angles of the triangle defined by 3 points in degrees.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public static Vector3 CalcAnglesDeg(ref Vector2 v0, ref Vector2 v1, ref Vector2 v2) {
            FLOAT num = v2.x - v1.x;
            FLOAT num2 = v2.y - v1.y;
            FLOAT num3 = num * num + num2 * num2;
            num = v2.x - v0.x;
            num2 = v2.y - v0.y;
            FLOAT num4 = num * num + num2 * num2;
            num = v1.x - v0.x;
            num2 = v1.y - v0.y;
            FLOAT num5 = num * num + num2 * num2;
            FLOAT num6 = 2f * Math.Sqrt(num5);
            Vector3 result = default(Vector3);
            result.x = Math.Acos((num4 + num5 - num3) / (Math.Sqrt(num4) * num6)) * 57.29578f;
            result.y = Math.Acos((num3 + num5 - num4) / (Math.Sqrt(num3) * num6)) * 57.29578f;
            result.z = 180f - result.x - result.y;
            return result;
        }

        /// <summary>
        /// Calculates angles of the triangle defined by 3 points in degrees.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public static Vector3 CalcAnglesDeg(Vector2 v0, Vector2 v1, Vector2 v2) {
            return CalcAnglesDeg(ref v0, ref v1, ref v2);
        }

        /// <summary>
        /// Calculates angles of the triangle in radians.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public Vector3 CalcAnglesRad() {
            FLOAT num = this.v2.x - this.v1.x;
            FLOAT num2 = this.v2.y - this.v1.y;
            FLOAT num3 = num * num + num2 * num2;
            num = this.v2.x - this.v0.x;
            num2 = this.v2.y - this.v0.y;
            FLOAT num4 = num * num + num2 * num2;
            num = this.v1.x - this.v0.x;
            num2 = this.v1.y - this.v0.y;
            FLOAT num5 = num * num + num2 * num2;
            FLOAT num6 = 2f * Math.Sqrt(num5);
            Vector3 result = default(Vector3);
            result.x = Math.Acos((num4 + num5 - num3) / (Math.Sqrt(num4) * num6));
            result.y = Math.Acos((num3 + num5 - num4) / (Math.Sqrt(num3) * num6));
            result.z = Math.Pi - result.x - result.y;
            return result;
        }

        /// <summary>
        /// Calculates angles of the triangle defined by 3 points in radians.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public static Vector3 CalcAnglesRad(ref Vector2 v0, ref Vector2 v1, ref Vector2 v2) {
            FLOAT num = v2.x - v1.x;
            FLOAT num2 = v2.y - v1.y;
            FLOAT num3 = num * num + num2 * num2;
            num = v2.x - v0.x;
            num2 = v2.y - v0.y;
            FLOAT num4 = num * num + num2 * num2;
            num = v1.x - v0.x;
            num2 = v1.y - v0.y;
            FLOAT num5 = num * num + num2 * num2;
            FLOAT num6 = 2f * Math.Sqrt(num5);
            Vector3 result = default(Vector3);
            result.x = Math.Acos((num4 + num5 - num3) / (Math.Sqrt(num4) * num6));
            result.y = Math.Acos((num3 + num5 - num4) / (Math.Sqrt(num3) * num6));
            result.z = Math.Pi - result.x - result.y;
            return result;
        }

        /// <summary>
        /// Calculates angles of the triangle defined by 3 points in radians.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public static Vector3 CalcAnglesRad(Vector2 v0, Vector2 v1, Vector2 v2) {
            return CalcAnglesRad(ref v0, ref v1, ref v2);
        }

        /// <summary>
        /// Gets point on the triangle using barycentric coordinates.
        /// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1, c2 is calculated as 1-c0-c1.
        /// </summary>
        public Vector2 EvalBarycentric(FLOAT c0, FLOAT c1) {
            FLOAT num = 1f - c0 - c1;
            return c0 * this.v0 + c1 * this.v1 + num * this.v2;
        }

        /// <summary>
        /// Gets point on the triangle using barycentric coordinates. baryCoords parameter is (c0,c1,c2).
        /// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1
        /// </summary>
        public Vector2 EvalBarycentric(ref Vector3 baryCoords) {
            return baryCoords.x * this.v0 + baryCoords.y * this.v1 + baryCoords.z * this.v2;
        }

        /// <summary>
        /// Gets point on the triangle using barycentric coordinates. baryCoords parameter is (c0,c1,c2).
        /// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1
        /// </summary>
        public Vector2 EvalBarycentric(Vector3 baryCoords) {
            return baryCoords.x * this.v0 + baryCoords.y * this.v1 + baryCoords.z * this.v2;
        }

        /// <summary>
        /// Calculate barycentric coordinates for the input point with regarding to triangle defined by 3 points.
        /// </summary>
        public static void CalcBarycentricCoords(ref Vector2 point, ref Vector2 v0, ref Vector2 v1,
            ref Vector2 v2, out Vector3 baryCoords) {
            Vector2 vector = v1 - v0;
            Vector2 value = v2 - v0;
            Vector2 vector2 = point - v0;
            Vector2.Dot(ref vector, ref vector, out var num);
            Vector2.Dot(ref vector, ref value, out var num2);
            Vector2.Dot(ref value, ref value, out var num3);
            Vector2.Dot(ref vector2, ref vector, out var num4);
            Vector2.Dot(ref vector2, ref value, out var num5);
            FLOAT num6 = 1f / (num * num3 - num2 * num2);
            baryCoords.y = (num3 * num4 - num2 * num5) * num6;
            baryCoords.z = (num * num5 - num2 * num4) * num6;
            baryCoords.x = 1f - baryCoords.y - baryCoords.z;
        }

        /// <summary>
        /// Calculate barycentric coordinates for the input point regarding to the triangle.
        /// </summary>
        public Vector3 CalcBarycentricCoords(ref Vector2 point) {
            CalcBarycentricCoords(ref point, ref this.v0, ref this.v1, ref this.v2, out var baryCoords);
            return baryCoords;
        }

        /// <summary>
        /// Calculate barycentric coordinates for the input point regarding to the triangle.
        /// </summary>
        public Vector3 CalcBarycentricCoords(Vector2 point) {
            CalcBarycentricCoords(ref point, ref this.v0, ref this.v1, ref this.v2, out var baryCoords);
            return baryCoords;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector2 point) {
            return Distance.Point2Triangle2(ref point, ref this);
        }

        /// <summary>
        /// Determines on which side of the triangle a point is. Returns +1 if a point
        /// is outside of the triangle, 0 if it's on the triangle border, -1 if it's inside the triangle.
        /// Method must be called for CCW ordered triangles.
        /// </summary>
        public int QuerySideCCW(Vector2 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = (point.x - this.v0.x) * (this.v1.y - this.v0.y) - (point.y - this.v0.y) * (this.v1.x - this.v0.x);
            if (num > epsilon) {
                return 1;
            }

            FLOAT num2 = (point.x - this.v1.x) * (this.v2.y - this.v1.y) - (point.y - this.v1.y) * (this.v2.x - this.v1.x);
            if (num2 > epsilon) {
                return 1;
            }

            FLOAT num3 = (point.x - this.v2.x) * (this.v0.y - this.v2.y) - (point.y - this.v2.y) * (this.v0.x - this.v2.x);
            if (num3 > epsilon) {
                return 1;
            }

            FLOAT num4 = 0f - epsilon;
            if (!(num < num4) || !(num2 < num4) || !(num3 < num4)) {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Determines on which side of the triangle a point is. Returns +1 if a point
        /// is outside of the triangle, 0 if it's on the triangle border, -1 if it's inside the triangle.
        /// Method must be called for CCW ordered triangles.
        /// </summary>
        public int QuerySideCCW(Vector2 point, FLOAT epsilon) {
            FLOAT num = (point.x - this.v0.x) * (this.v1.y - this.v0.y) - (point.y - this.v0.y) * (this.v1.x - this.v0.x);
            if (num > epsilon) {
                return 1;
            }

            FLOAT num2 = (point.x - this.v1.x) * (this.v2.y - this.v1.y) - (point.y - this.v1.y) * (this.v2.x - this.v1.x);
            if (num2 > epsilon) {
                return 1;
            }

            FLOAT num3 = (point.x - this.v2.x) * (this.v0.y - this.v2.y) - (point.y - this.v2.y) * (this.v0.x - this.v2.x);
            if (num3 > epsilon) {
                return 1;
            }

            FLOAT num4 = 0f - epsilon;
            if (!(num < num4) || !(num2 < num4) || !(num3 < num4)) {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Determines on which side of the triangle a point is. Returns +1 if a point
        /// is outside of the triangle, 0 if it's on the triangle border, -1 if it's inside the triangle.
        /// Method must be called for CW ordered triangles.
        /// </summary>
        public int QuerySideCW(Vector2 point) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = 0f - epsilon;
            FLOAT num2 = (point.x - this.v0.x) * (this.v1.y - this.v0.y) - (point.y - this.v0.y) * (this.v1.x - this.v0.x);
            if (num2 < num) {
                return 1;
            }

            FLOAT num3 = (point.x - this.v1.x) * (this.v2.y - this.v1.y) - (point.y - this.v1.y) * (this.v2.x - this.v1.x);
            if (num3 < num) {
                return 1;
            }

            FLOAT num4 = (point.x - this.v2.x) * (this.v0.y - this.v2.y) - (point.y - this.v2.y) * (this.v0.x - this.v2.x);
            if (num4 < num) {
                return 1;
            }

            if (!(num2 > epsilon) || !(num3 > epsilon) || !(num4 > epsilon)) {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Determines on which side of the triangle a point is. Returns +1 if a point
        /// is outside of the triangle, 0 if it's on the triangle border, -1 if it's inside the triangle.
        /// Method must be called for CW ordered triangles.
        /// </summary>
        public int QuerySideCW(Vector2 point, FLOAT epsilon) {
            FLOAT num = 0f - epsilon;
            FLOAT num2 = (point.x - this.v0.x) * (this.v1.y - this.v0.y) - (point.y - this.v0.y) * (this.v1.x - this.v0.x);
            if (num2 < num) {
                return 1;
            }

            FLOAT num3 = (point.x - this.v1.x) * (this.v2.y - this.v1.y) - (point.y - this.v1.y) * (this.v2.x - this.v1.x);
            if (num3 < num) {
                return 1;
            }

            FLOAT num4 = (point.x - this.v2.x) * (this.v0.y - this.v2.y) - (point.y - this.v2.y) * (this.v0.x - this.v2.x);
            if (num4 < num) {
                return 1;
            }

            if (!(num2 > epsilon) || !(num3 > epsilon) || !(num4 > epsilon)) {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public Vector2 Project(Vector2 point) {
            Distance.SqrPoint2Triangle2(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Tests whether a point is contained by the triangle (CW or CCW ordered).
        /// Note however that if the triangle is CCW then points which are on triangle border considered inside, but
        /// if the triangle is CW then points which are on triangle border considered outside.
        /// For consistent (and faster) test use appropriate overloads for CW and CCW triangles.
        /// </summary>
        public bool Contains(ref Vector2 point) {
            bool flag = (point.x - this.v1.x) * (this.v0.y - this.v1.y) - (point.y - this.v1.y) * (this.v0.x - this.v1.x) < 0f;
            bool flag2 = (point.x - this.v2.x) * (this.v1.y - this.v2.y) - (point.y - this.v2.y) * (this.v1.x - this.v2.x) < 0f;
            if (flag != flag2) {
                return false;
            }

            bool flag3 = (point.x - this.v0.x) * (this.v2.y - this.v0.y) - (point.y - this.v0.y) * (this.v2.x - this.v0.x) < 0f;
            return flag2 == flag3;
        }

        /// <summary>
        /// Tests whether a point is contained by the triangle (CW or CCW ordered).
        /// Note however that if the triangle is CCW then points which are on triangle border considered inside, but
        /// if the triangle is CW then points which are on triangle border considered outside.
        /// For consistent (and faster) test use appropriate overloads for CW and CCW triangles.
        /// </summary>
        public bool Contains(Vector2 point) {
            return this.Contains(ref point);
        }

        /// <summary>
        /// Tests whether a point is contained by the CCW triangle
        /// </summary>
        public bool ContainsCCW(ref Vector2 point) {
            if ((point.x - this.v0.x) * (this.v1.y - this.v0.y) - (point.y - this.v0.y) * (this.v1.x - this.v0.x) > 0f) {
                return false;
            }

            if ((point.x - this.v1.x) * (this.v2.y - this.v1.y) - (point.y - this.v1.y) * (this.v2.x - this.v1.x) > 0f) {
                return false;
            }

            if ((point.x - this.v2.x) * (this.v0.y - this.v2.y) - (point.y - this.v2.y) * (this.v0.x - this.v2.x) > 0f) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether a point is contained by the CCW triangle
        /// </summary>
        public bool ContainsCCW(Vector2 point) {
            return this.ContainsCCW(ref point);
        }

        /// <summary>
        /// Tests whether a point is contained by the CW triangle
        /// </summary>
        public bool ContainsCW(ref Vector2 point) {
            if ((point.x - this.v0.x) * (this.v1.y - this.v0.y) - (point.y - this.v0.y) * (this.v1.x - this.v0.x) < 0f) {
                return false;
            }

            if ((point.x - this.v1.x) * (this.v2.y - this.v1.y) - (point.y - this.v1.y) * (this.v2.x - this.v1.x) < 0f) {
                return false;
            }

            if ((point.x - this.v2.x) * (this.v0.y - this.v2.y) - (point.y - this.v2.y) * (this.v0.x - this.v2.x) < 0f) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether a point is contained by the CW triangle
        /// </summary>
        public bool ContainsCW(Vector2 point) {
            return this.ContainsCW(ref point);
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[V0: {this.v0.ToString()} V1: {this.v1.ToString()} V2: {this.v2.ToString()}]";
        }
    }
}