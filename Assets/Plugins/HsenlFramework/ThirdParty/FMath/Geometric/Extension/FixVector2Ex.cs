using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public static class FVector2Ex {
        internal class Information {
            public int Dimension;

            public FVector2 Min;

            public FVector2 Max;

            public FLOAT MaxRange;

            public FVector2 Origin;

            public FVector2[] Direction = new FVector2[2];

            public int[] Extreme = new int[3];

            public bool ExtremeCCW;
        }

        internal static Information GetInformation(IList<FVector2> points, FLOAT epsilon) {
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
            }

            information.Min.x = num;
            information.Min.y = num5;
            information.Max.x = num2;
            information.Max.y = num6;
            information.MaxRange = num2 - num;
            information.Extreme[0] = num3;
            information.Extreme[1] = num4;
            FLOAT num9 = num6 - num5;
            if (num9 > information.MaxRange) {
                information.MaxRange = num9;
                information.Extreme[0] = num7;
                information.Extreme[1] = num8;
            }

            information.Origin = points[information.Extreme[0]];
            if (information.MaxRange < epsilon) {
                information.Dimension = 0;
                information.Extreme[1] = information.Extreme[0];
                information.Extreme[2] = information.Extreme[0];
                ref FVector2 reference = ref information.Direction[0];
                reference = FVector2.Zero;
                ref FVector2 reference2 = ref information.Direction[1];
                reference2 = FVector2.Zero;
                return information;
            }

            ref FVector2 reference3 = ref information.Direction[0];
            reference3 = points[information.Extreme[1]] - information.Origin;
            information.Direction[0].Normalize();
            ref FVector2 reference4 = ref information.Direction[1];
            reference4 = -information.Direction[0].Perpendicular();
            FLOAT num10 = 0f;
            FLOAT num11 = 0f;
            information.Extreme[2] = information.Extreme[0];
            for (int j = 0; j < count; j++) {
                FVector2 value = points[j] - information.Origin;
                FLOAT f = information.Direction[1].Dot(value);
                FLOAT num12 = FMath.Sign(f);
                f = FMath.Abs(f);
                if (f > num10) {
                    num10 = f;
                    num11 = num12;
                    information.Extreme[2] = j;
                }
            }

            if (num10 < epsilon * information.MaxRange) {
                information.Dimension = 1;
                information.Extreme[2] = information.Extreme[1];
                return information;
            }

            information.Dimension = 2;
            information.ExtremeCCW = num11 > 0f;
            return information;
        }

        /// <summary>
        /// Returns x0*y1 - y0*x1
        /// </summary>
        public static FLOAT DotPerpendicular(this FVector2 vector, FVector2 value) {
            return vector.x * value.y - vector.y * value.x;
        }

        /// <summary>
        /// Returns x0*y1 - y0*x1
        /// </summary>
        public static FLOAT DotPerpendicular(this FVector2 vector, ref FVector2 value) {
            return vector.x * value.y - vector.y * value.x;
        }

        /// <summary>
        /// Returns x0*y1 - y0*x1
        /// </summary>
        public static FLOAT DotPerpendicular(ref FVector2 vector, ref FVector2 value) {
            return vector.x * value.y - vector.y * value.x;
        }

        /// <summary>
        /// Returns (y,-x)
        /// </summary>
        public static FVector2 Perpendicular(this FVector2 vector) {
            return new FVector2(vector.y, 0f - vector.x);
        }

        public static FVector3 ToFVector3XY(this FVector2 self) {
            return new FVector3(self.x, self.y, 0);
        }

        public static FVector3 ToFVector3XZ(this FVector2 self) {
            return new FVector3(self.x, 0, self.y);
        }

        public static FVector3 ToFVector3YZ(this FVector2 self) {
            return new FVector3(0, self.x, self.y);
        }
    }
}