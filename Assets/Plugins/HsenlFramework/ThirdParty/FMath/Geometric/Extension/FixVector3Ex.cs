using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public static class FVector3Ex {
        internal class Information {
            public int Dimension;

            public FVector3 Min;

            public FVector3 Max;

            public FLOAT MaxRange;

            public FVector3 Origin;

            public FVector3[] Direction = new FVector3[3];

            public int[] Extreme = new int[4];

            public bool ExtremeCCW;
        }

        internal static Information GetInformation(IList<FVector3> points, FLOAT epsilon) {
            if (points == null) {
                return null;
            }

            int count = points.Count;
            if (count == 0 || epsilon < 0f) {
                return null;
            }

            Information information = new Information();
            information.ExtremeCCW = false;
            FLOAT x;
            FLOAT num = (x = points[0].x);
            FLOAT num2 = x;
            int num3 = 0;
            int num4 = 0;
            FLOAT num5 = (x = points[0].y);
            FLOAT num6 = x;
            int num7 = 0;
            int num8 = 0;
            FLOAT num9 = (x = points[0].z);
            FLOAT num10 = x;
            int num11 = 0;
            int num12 = 0;
            for (int i = 1; i < count; i++) {
                x = points[i].x;
                if (x < num) {
                    num = x;
                    num3 = i;
                }
                else if (x > num2) {
                    num2 = x;
                    num4 = i;
                }

                x = points[i].y;
                if (x < num5) {
                    num5 = x;
                    num7 = i;
                }
                else if (x > num6) {
                    num6 = x;
                    num8 = i;
                }

                x = points[i].z;
                if (x < num9) {
                    num9 = x;
                    num11 = i;
                }
                else if (x > num10) {
                    num10 = x;
                    num12 = i;
                }
            }

            information.Min.x = num;
            information.Min.y = num5;
            information.Min.z = num9;
            information.Max.x = num2;
            information.Max.y = num6;
            information.Max.z = num10;
            information.MaxRange = num2 - num;
            information.Extreme[0] = num3;
            information.Extreme[1] = num4;
            FLOAT num13 = num6 - num5;
            if (num13 > information.MaxRange) {
                information.MaxRange = num13;
                information.Extreme[0] = num7;
                information.Extreme[1] = num8;
            }

            num13 = num10 - num9;
            if (num13 > information.MaxRange) {
                information.MaxRange = num13;
                information.Extreme[0] = num11;
                information.Extreme[1] = num12;
            }

            information.Origin = points[information.Extreme[0]];
            if (information.MaxRange < epsilon) {
                information.Dimension = 0;
                information.Extreme[1] = information.Extreme[0];
                information.Extreme[2] = information.Extreme[0];
                information.Extreme[3] = information.Extreme[0];
                ref FVector3 reference = ref information.Direction[0];
                reference = FVector3.Zero;
                ref FVector3 reference2 = ref information.Direction[1];
                reference2 = FVector3.Zero;
                ref FVector3 reference3 = ref information.Direction[2];
                reference3 = FVector3.Zero;
                return information;
            }

            ref FVector3 reference4 = ref information.Direction[0];
            reference4 = points[information.Extreme[1]] - information.Origin;
            information.Direction[0].Normalize();
            FLOAT num14 = 0f;
            information.Extreme[2] = information.Extreme[0];
            FLOAT num15;
            for (int j = 0; j < count; j++) {
                FVector3 vector = points[j] - information.Origin;
                num15 = information.Direction[0].Dot(vector);
                FLOAT magnitude = (vector - num15 * information.Direction[0]).magnitude;
                if (magnitude > num14) {
                    num14 = magnitude;
                    information.Extreme[2] = j;
                }
            }

            if (num14 < epsilon * information.MaxRange) {
                information.Dimension = 1;
                information.Extreme[2] = information.Extreme[1];
                information.Extreme[3] = information.Extreme[1];
                return information;
            }

            ref FVector3 reference5 = ref information.Direction[1];
            reference5 = points[information.Extreme[2]] - information.Origin;
            num15 = information.Direction[0].Dot(information.Direction[1]);
            information.Direction[1] -= num15 * information.Direction[0];
            information.Direction[1].Normalize();
            ref FVector3 reference6 = ref information.Direction[2];
            reference6 = information.Direction[0].Cross(information.Direction[1]);
            num14 = 0f;
            FLOAT num16 = 0f;
            information.Extreme[3] = information.Extreme[0];
            for (int k = 0; k < count; k++) {
                FVector3 value = points[k] - information.Origin;
                FLOAT magnitude = information.Direction[2].Dot(value);
                FLOAT num17 = FMath.Sign(magnitude);
                magnitude = FMath.Abs(magnitude);
                if (magnitude > num14) {
                    num14 = magnitude;
                    num16 = num17;
                    information.Extreme[3] = k;
                }
            }

            if (num14 < epsilon * information.MaxRange) {
                information.Dimension = 2;
                information.Extreme[3] = information.Extreme[2];
                return information;
            }

            information.Dimension = 3;
            information.ExtremeCCW = num16 > 0f;
            return information;
        }

        /// <summary>
        /// Returns normalized cross product of vectors
        /// </summary>
        public static FVector3 NormalizeCross(this FVector3 vector, FVector3 value) {
            FVector3 vector2 = new FVector3(vector.y * value.z - vector.z * value.y,
                vector.z * value.x - vector.x * value.z, vector.x * value.y - vector.y * value.x);
            FVector3.Normalize(ref vector2);
            return vector2;
        }

        /// <summary>
        /// Input W must be a unit-length vector. The output vectors {U,V} are
        /// unit length and mutually perpendicular, and {U,V,W} is an orthonormal basis.
        /// </summary>
        public static void CreateOrthonormalBasis(out FVector3 u, out FVector3 v, ref FVector3 w) {
            if (FMath.Abs(w.x) >= FMath.Abs(w.y)) {
                FLOAT num = FMathfEx.InvSqrt(w.x * w.x + w.z * w.z);
                u.x = (0f - w.z) * num;
                u.y = 0f;
                u.z = w.x * num;
                v.x = w.y * u.z;
                v.y = w.z * u.x - w.x * u.z;
                v.z = (0f - w.y) * u.x;
            }
            else {
                FLOAT num2 = FMathfEx.InvSqrt(w.y * w.y + w.z * w.z);
                u.x = 0f;
                u.y = w.z * num2;
                u.z = (0f - w.y) * num2;
                v.x = w.y * u.z - w.z * u.y;
                v.y = (0f - w.x) * u.z;
                v.z = w.x * u.y;
            }
        }

        /// <summary>
        /// Converts FVector3 to FVector2, copying x and y components of FVector3 to x and y components of FVector2 respectively.
        /// </summary>
        public static FVector2 ToFVector2XY(this FVector3 vector) {
            return new FVector2(vector.x, vector.y);
        }

        /// <summary>
        /// Converts FVector3 to FVector2, copying x and z components of FVector3 to x and y components of FVector2 respectively.
        /// </summary>
        public static FVector2 ToFVector2XZ(this FVector3 vector) {
            return new FVector2(vector.x, vector.z);
        }

        /// <summary>
        /// Converts FVector3 to FVector2, copying y and z components of FVector3 to x and y components of FVector2 respectively.
        /// </summary>
        public static FVector2 ToFVector2YZ(this FVector3 vector) {
            return new FVector2(vector.y, vector.z);
        }

        /// <summary>
        /// Converts FVector3 to FVector2 using specified projection plane.
        /// </summary>
        public static FVector2 ToFVector2(this FVector3 vector, ProjectionPlanes projectionPlane) {
            return projectionPlane switch {
                ProjectionPlanes.XY => new FVector2(vector.x, vector.y),
                ProjectionPlanes.XZ => new FVector2(vector.x, vector.z),
                _ => new FVector2(vector.y, vector.z),
            };
        }

        /// <summary>
        /// 返回最合适的投影平面，将向量视为法线(例如，如果y分量最大，则返回XZ平面)。（配合 ToFVector2方法使用 使用）
        /// </summary>
        public static ProjectionPlanes GetProjectionPlane(this FVector3 vector) {
            var result = ProjectionPlanes.YZ;
            var num = FMath.Abs(vector.x);
            var num2 = FMath.Abs(vector.y);
            if (num2 > num) {
                result = ProjectionPlanes.XZ;
                num = num2;
            }

            num2 = FMath.Abs(vector.z);
            if (num2 > num) {
                result = ProjectionPlanes.XY;
            }

            return result;
        }
    }
}