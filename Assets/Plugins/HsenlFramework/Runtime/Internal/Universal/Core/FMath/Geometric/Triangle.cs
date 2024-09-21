#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public struct Triangle {
        /// <summary>
        /// First triangle vertex
        /// </summary>
        public Vector3 v0;

        /// <summary>
        /// Second triangle vertex
        /// </summary>
        public Vector3 v1;

        /// <summary>
        /// Third triangle vertex
        /// </summary>
        public Vector3 v2;

        /// <summary>
        /// Gets or sets triangle vertex by index: 0, 1 or 2
        /// </summary>
        public Vector3 this[int index] {
            get {
                return index switch {
                    0 => this.v0,
                    1 => this.v1,
                    2 => this.v2,
                    _ => Vector3.Zero,
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
        /// Creates Triangle3 from 3 vertices
        /// </summary>
        public Triangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2) {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }

        /// <summary>
        /// Creates Triangle3 from 3 vertices
        /// </summary>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2) {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }

        /// <summary>
        /// Returns triangle edge by index 0, 1 or 2
        /// Edge[i] = V[i+1]-V[i]
        /// </summary>
        public Vector3 CalcEdge(int edgeIndex) {
            return edgeIndex switch {
                0 => this.v1 - this.v0,
                1 => this.v2 - this.v1,
                2 => this.v0 - this.v2,
                _ => Vector3.Zero,
            };
        }

        /// <summary>
        /// Returns triangle normal as (V1-V0)x(V2-V0)
        /// </summary>
        public Vector3 CalcNormal() {
            return Vector3.Cross(this.v1 - this.v0, this.v2 - this.v0);
        }

        /// <summary>
        /// Returns triangle area as 0.5*Abs(Length((V1-V0)x(V2-V0)))
        /// </summary>
        public FLOAT CalcArea() {
            return 0.5f * Vector3.Cross(this.v1 - this.v0, this.v2 - this.v0).magnitude;
        }

        /// <summary>
        /// Returns triangle area defined by 3 points.
        /// </summary>
        public static FLOAT CalcArea(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2) {
            return 0.5f * Vector3.Cross(v1 - v0, v2 - v0).magnitude;
        }

        /// <summary>
        /// Returns triangle area defined by 3 points.
        /// </summary>
        public static FLOAT CalcArea(Vector3 v0, Vector3 v1, Vector3 v2) {
            return 0.5f * Vector3.Cross(v1 - v0, v2 - v0).magnitude;
        }

        /// <summary>
        /// Calculates angles of the triangle in degrees.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public Vector3 CalcAnglesDeg() {
            FLOAT num = this.v2.x - this.v1.x;
            FLOAT num2 = this.v2.y - this.v1.y;
            FLOAT num3 = this.v2.z - this.v1.z;
            FLOAT num4 = num * num + num2 * num2 + num3 * num3;
            num = this.v2.x - this.v0.x;
            num2 = this.v2.y - this.v0.y;
            num3 = this.v2.z - this.v0.z;
            FLOAT num5 = num * num + num2 * num2 + num3 * num3;
            num = this.v1.x - this.v0.x;
            num2 = this.v1.y - this.v0.y;
            num3 = this.v1.z - this.v0.z;
            FLOAT num6 = num * num + num2 * num2 + num3 * num3;
            FLOAT num7 = 2f * Math.Sqrt(num6);
            Vector3 result = default(Vector3);
            result.x = Math.Acos((num5 + num6 - num4) / (Math.Sqrt(num5) * num7)) * 57.29578f;
            result.y = Math.Acos((num4 + num6 - num5) / (Math.Sqrt(num4) * num7)) * 57.29578f;
            result.z = 180f - result.x - result.y;
            return result;
        }

        /// <summary>
        /// Calculates angles of the triangle defined by 3 points in degrees.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public static Vector3 CalcAnglesDeg(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2) {
            FLOAT num = v2.x - v1.x;
            FLOAT num2 = v2.y - v1.y;
            FLOAT num3 = v2.z - v1.z;
            FLOAT num4 = num * num + num2 * num2 + num3 * num3;
            num = v2.x - v0.x;
            num2 = v2.y - v0.y;
            num3 = v2.z - v0.z;
            FLOAT num5 = num * num + num2 * num2 + num3 * num3;
            num = v1.x - v0.x;
            num2 = v1.y - v0.y;
            num3 = v1.z - v0.z;
            FLOAT num6 = num * num + num2 * num2 + num3 * num3;
            FLOAT num7 = 2f * Math.Sqrt(num6);
            Vector3 result = default(Vector3);
            result.x = Math.Acos((num5 + num6 - num4) / (Math.Sqrt(num5) * num7)) * 57.29578f;
            result.y = Math.Acos((num4 + num6 - num5) / (Math.Sqrt(num4) * num7)) * 57.29578f;
            result.z = 180f - result.x - result.y;
            return result;
        }

        /// <summary>
        /// Calculates angles of the triangle defined by 3 points in degrees.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public static Vector3 CalcAnglesDeg(Vector3 v0, Vector3 v1, Vector3 v2) {
            return CalcAnglesDeg(ref v0, ref v1, ref v2);
        }

        /// <summary>
        /// Calculates angles of the triangle in radians.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public Vector3 CalcAnglesRad() {
            FLOAT num = this.v2.x - this.v1.x;
            FLOAT num2 = this.v2.y - this.v1.y;
            FLOAT num3 = this.v2.z - this.v1.z;
            FLOAT num4 = num * num + num2 * num2 + num3 * num3;
            num = this.v2.x - this.v0.x;
            num2 = this.v2.y - this.v0.y;
            num3 = this.v2.z - this.v0.z;
            FLOAT num5 = num * num + num2 * num2 + num3 * num3;
            num = this.v1.x - this.v0.x;
            num2 = this.v1.y - this.v0.y;
            num3 = this.v1.z - this.v0.z;
            FLOAT num6 = num * num + num2 * num2 + num3 * num3;
            FLOAT num7 = 2f * Math.Sqrt(num6);
            Vector3 result = default(Vector3);
            result.x = Math.Acos((num5 + num6 - num4) / (Math.Sqrt(num5) * num7));
            result.y = Math.Acos((num4 + num6 - num5) / (Math.Sqrt(num4) * num7));
            result.z = Math.Pi - result.x - result.y;
            return result;
        }

        /// <summary>
        /// Calculates angles of the triangle defined by 3 points in radians.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public static Vector3 CalcAnglesRad(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2) {
            FLOAT num = v2.x - v1.x;
            FLOAT num2 = v2.y - v1.y;
            FLOAT num3 = v2.z - v1.z;
            FLOAT num4 = num * num + num2 * num2 + num3 * num3;
            num = v2.x - v0.x;
            num2 = v2.y - v0.y;
            num3 = v2.z - v0.z;
            FLOAT num5 = num * num + num2 * num2 + num3 * num3;
            num = v1.x - v0.x;
            num2 = v1.y - v0.y;
            num3 = v1.z - v0.z;
            FLOAT num6 = num * num + num2 * num2 + num3 * num3;
            FLOAT num7 = 2f * Math.Sqrt(num6);
            Vector3 result = default(Vector3);
            result.x = Math.Acos((num5 + num6 - num4) / (Math.Sqrt(num5) * num7));
            result.y = Math.Acos((num4 + num6 - num5) / (Math.Sqrt(num4) * num7));
            result.z = Math.Pi - result.x - result.y;
            return result;
        }

        /// <summary>
        /// Calculates angles of the triangle defined by 3 points in radians.
        /// Angles are returned in the instance of FVector3 following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
        /// </summary>
        public static Vector3 CalcAnglesRad(Vector3 v0, Vector3 v1, Vector3 v2) {
            return CalcAnglesRad(ref v0, ref v1, ref v2);
        }

        /// <summary>
        /// Gets point on the triangle using barycentric coordinates.
        /// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1, c2 is calculated as 1-c0-c1.
        /// </summary>
        public Vector3 EvalBarycentric(FLOAT c0, FLOAT c1) {
            FLOAT num = 1f - c0 - c1;
            return c0 * this.v0 + c1 * this.v1 + num * this.v2;
        }

        /// <summary>
        /// Gets point on the triangle using barycentric coordinates. baryCoords parameter is (c0,c1,c2).
        /// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1
        /// </summary>
        public Vector3 EvalBarycentric(ref Vector3 baryCoords) {
            return baryCoords.x * this.v0 + baryCoords.y * this.v1 + baryCoords.z * this.v2;
        }

        /// <summary>
        /// Gets point on the triangle using barycentric coordinates. baryCoords parameter is (c0,c1,c2).
        /// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1
        /// </summary>
        public Vector3 EvalBarycentric(Vector3 baryCoords) {
            return baryCoords.x * this.v0 + baryCoords.y * this.v1 + baryCoords.z * this.v2;
        }

        /// <summary>
        /// Calculate barycentric coordinates for the input point with regarding to triangle defined by 3 points.
        /// </summary>
        public static void CalcBarycentricCoords(ref Vector3 point, ref Vector3 v0, ref Vector3 v1,
            ref Vector3 v2, out Vector3 baryCoords) {
            Vector3 vector = v1 - v0;
            Vector3 value = v2 - v0;
            Vector3 vector2 = point - v0;
            FLOAT num = Vector3.Dot(ref vector, ref vector);
            FLOAT num2 = Vector3.Dot(ref vector, ref value);
            FLOAT num3 = Vector3.Dot(ref value, ref value);
            FLOAT num4 = Vector3.Dot(ref vector2, ref vector);
            FLOAT num5 = Vector3.Dot(ref vector2, ref value);
            FLOAT num6 = 1f / (num * num3 - num2 * num2);
            baryCoords.y = (num3 * num4 - num2 * num5) * num6;
            baryCoords.z = (num * num5 - num2 * num4) * num6;
            baryCoords.x = 1f - baryCoords.y - baryCoords.z;
        }

        /// <summary>
        /// Calculate barycentric coordinates for the input point regarding to the triangle.
        /// </summary>
        public Vector3 CalcBarycentricCoords(ref Vector3 point) {
            CalcBarycentricCoords(ref point, ref this.v0, ref this.v1, ref this.v2, out var baryCoords);
            return baryCoords;
        }

        /// <summary>
        /// Calculate barycentric coordinates for the input point regarding to the triangle.
        /// </summary>
        public Vector3 CalcBarycentricCoords(Vector3 point) {
            CalcBarycentricCoords(ref point, ref this.v0, ref this.v1, ref this.v2, out var baryCoords);
            return baryCoords;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[V0: {this.v0.ToString()} V1: {this.v1.ToString()} V2: {this.v2.ToString()}]";
        }
    }
}