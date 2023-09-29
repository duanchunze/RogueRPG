using System;
using System.Globalization;
using System.IO;

namespace FixedMath {
    [Serializable]
    public partial struct Fixp : IEquatable<Fixp>, IComparable<Fixp> {
        public const long MAX_VALUE = long.MaxValue;
        public const long MIN_VALUE = long.MinValue;
        public const int NUM_BITS = 64; // 位数
        public const int FRACTIONAL_PLACES = 32; // 小数部分大小
        public const long ONE = 4294967296L; // 1
        public const long TEN = 42949672960L; // 10
        public const long HALF = 2147483648L; // 0.5
        public const long PI_TIMES_2 = 26986075409L; // 2π
        public const long PI = 13493037704L; // π
        public const long PI_OVER_2 = 6746518852L; // π/2
        public const long LN2 = 2977044471L;
        public const long LOG2MAX = 133143986176L;
        public const long LOG2MIN = -137438953472L;
        public const int LUT_SIZE = 205887; // lut参考数（因为Fix64的lut值不是算出来的，而是使用查找的方式，所以参考越多，就越准确）

        public static readonly decimal Precision = (decimal)new Fixp(1L);
        public static readonly Fixp MaxValue = new Fixp(9223372036854775806L);
        public static readonly Fixp MinValue = new Fixp(-9223372036854775806L);
        public static readonly Fixp One = new Fixp(4294967296L);
        public static readonly Fixp Ten = new Fixp(42949672960L);
        public static readonly Fixp Half = new Fixp(2147483648L);
        public static readonly Fixp Zero = default(Fixp);

        /// <summary>
        /// 正无穷
        /// </summary>
        public static readonly Fixp PositiveInfinity = new Fixp(long.MaxValue);

        public static readonly Fixp NegativeInfinity = new Fixp(-9223372036854775807L);
        public static readonly Fixp NaN = new Fixp(long.MinValue);
        public static readonly Fixp EN1 = One / 10;
        public static readonly Fixp EN2 = One / 100;
        public static readonly Fixp EN3 = One / 1000;
        public static readonly Fixp EN4 = One / 10000;
        public static readonly Fixp EN5 = One / 100000;
        public static readonly Fixp EN6 = One / 1000000;
        public static readonly Fixp EN7 = One / 10000000;

        public static readonly Fixp EN8 = One / 100000000;
        public static readonly Fixp Epsilon = EN3;
        public static readonly Fixp Pi = new Fixp(13493037704L);
        public static readonly Fixp PiOver2 = new Fixp(6746518852L);
        public static readonly Fixp PiTimes2 = new Fixp(26986075409L);
        public static readonly Fixp PiInv = (Fixp)0.3183098861837906715377675267m;
        public static readonly Fixp PiOver2Inv = (Fixp)0.6366197723675813430755350535m;
        public static readonly Fixp Deg2Rad = Pi / new Fixp(180); // 角度转弧度
        public static readonly Fixp Rad2Deg = new Fixp(180) / Pi; // 弧度转角度
        public static readonly Fixp LutInterval = 205886 / PiOver2; // 每隔多少角度有一个lut的参考
        public static readonly Fixp Log2Max = new Fixp(LOG2MAX);
        public static readonly Fixp Log2Min = new Fixp(LOG2MIN);
        public static readonly Fixp Ln2 = new Fixp(LN2);

        internal long serializedValue;

        public long RawValue => this.serializedValue;

        public Fixp(long rawValue) {
            this.serializedValue = rawValue;
        }

        public Fixp(int value) {
            this.serializedValue = value * 4294967296L;
        }

        public float AsFloat() {
            return (float)this;
        }

        public int AsInt() {
            return (int)(long)this;
        }

        public long AsLong() {
            return (long)this;
        }

        public double AsDouble() {
            return (double)this;
        }

        public decimal AsDecimal() {
            return (decimal)this;
        }

        public static float ToFloat(Fixp value) {
            return (float)value;
        }

        public static int ToInt(Fixp value) {
            return (int)(long)value;
        }

        public static Fixp FromFloat(float value) {
            return value;
        }

        public static bool IsInfinity(Fixp value) {
            return value == NegativeInfinity || value == PositiveInfinity;
        }

        public static bool IsNaN(Fixp value) {
            return value == NaN;
        }

        public override bool Equals(object obj) {
            return obj is Fixp && ((Fixp)obj).serializedValue == this.serializedValue;
        }

        public override int GetHashCode() {
            return this.serializedValue.GetHashCode();
        }

        public bool Equals(Fixp other) {
            return this.serializedValue == other.serializedValue;
        }

        public int CompareTo(Fixp other) {
            return this.serializedValue.CompareTo(other.serializedValue);
        }

        public override string ToString() {
            return ((float)this).ToString(CultureInfo.InvariantCulture);
        }

        internal static void GenerateAcosLut() {
            using StreamWriter streamWriter = new StreamWriter("Fix64AcosLut.cs");
            streamWriter.Write("namespace TrueSync {\r\n    partial struct FP {\r\n        public static readonly long[] AcosLut = new[] {");
            int num = 0;
            for (int i = 0; i < 205887; i++) {
                float num2 = (float)i / 205886f;
                if (num++ % 8 == 0) {
                    streamWriter.WriteLine();
                    streamWriter.Write("            ");
                }

                double num3 = Math.Acos(num2);
                long serializedValue = ((Fixp)num3).serializedValue;
                streamWriter.Write($"0x{serializedValue:X}L, ");
            }

            streamWriter.Write("\r\n        };\r\n    }\r\n}");
        }

        internal static void GenerateSinLut() {
            using StreamWriter streamWriter = new StreamWriter("Fix64SinLut.cs");
            streamWriter.Write("namespace FixMath.NF {\r\n    partial struct Fix64 {\r\n        public static readonly long[] SinLut = new[] {");
            int num = 0;
            for (int i = 0; i < 205887; i++) {
                double a = (double)i * Math.PI * 0.5 / 205886.0;
                if (num++ % 8 == 0) {
                    streamWriter.WriteLine();
                    streamWriter.Write("            ");
                }

                double num2 = Math.Sin(a);
                long serializedValue = ((Fixp)num2).serializedValue;
                streamWriter.Write($"0x{serializedValue:X}L, ");
            }

            streamWriter.Write("\r\n        };\r\n    }\r\n}");
        }

        internal static void GenerateTanLut() {
            using StreamWriter streamWriter = new StreamWriter("Fix64TanLut.cs");
            streamWriter.Write("namespace FixMath.NF {\r\n    partial struct Fix64 {\r\n        public static readonly long[] TanLut = new[] {");
            int num = 0;
            for (int i = 0; i < 205887; i++) {
                double a = (double)i * Math.PI * 0.5 / 205886.0;
                if (num++ % 8 == 0) {
                    streamWriter.WriteLine();
                    streamWriter.Write("            ");
                }

                double num2 = Math.Tan(a);
                if (num2 > (double)MaxValue || num2 < 0.0) {
                    num2 = (double)MaxValue;
                }

                Fixp obj = (((decimal)num2 > (decimal)MaxValue || num2 < 0.0) ? MaxValue : ((Fixp)num2));
                long serializedValue = obj.serializedValue;
                streamWriter.Write($"0x{serializedValue:X}L, ");
            }

            streamWriter.Write("\r\n        };\r\n    }\r\n}");
        }

        // ------------------------------------------------------------

        public static Fixp operator +(Fixp x, Fixp y) {
            var v = x.serializedValue + y.serializedValue;

            if (Math.Abs(v) <= 2) {
                v = 0;
            }

            return new Fixp(v);
        }

        public static Fixp OverflowAdd(Fixp x, Fixp y) {
            long serializedValue = x.serializedValue;
            long serializedValue2 = y.serializedValue;
            long num = serializedValue + serializedValue2;
            if ((~(serializedValue ^ serializedValue2) & (serializedValue ^ num) & long.MinValue) != 0) {
                num = ((serializedValue > 0) ? long.MaxValue : long.MinValue);
            }

            return new Fixp(num);
        }

        public static Fixp FastAdd(Fixp x, Fixp y) {
            return new Fixp(x.serializedValue + y.serializedValue);
        }

        public static Fixp operator -(Fixp x, Fixp y) {
            var v = x.serializedValue - y.serializedValue;

            if (Math.Abs(v) <= 2) {
                v = 0;
            }

            return new Fixp(v);
        }

        public static Fixp OverflowSub(Fixp x, Fixp y) {
            long serializedValue = x.serializedValue;
            long serializedValue2 = y.serializedValue;
            long num = serializedValue - serializedValue2;
            if (((serializedValue ^ serializedValue2) & (serializedValue ^ num) & long.MinValue) != 0) {
                num = ((serializedValue < 0) ? long.MinValue : long.MaxValue);
            }

            return new Fixp(num);
        }

        public static Fixp FastSub(Fixp x, Fixp y) {
            return new Fixp(x.serializedValue - y.serializedValue);
        }

        private static long AddOverflowHelper(long x, long y, ref bool overflow) {
            long num = x + y;
            overflow |= ((x ^ y ^ num) & long.MinValue) != 0;
            return num;
        }

        public static Fixp operator *(Fixp x, Fixp y) {
            // 貌似这个误差有两方面：
            // 一个是小数部分相乘，而后 << 32 操作导致精度损失，出现误差。（所以在 + -运算中，添加了误差包容）
            // 另一个正是因为上面的误差导致的乘以相同的数的正负值，得到的却不是完全相反的结果。例如：(1.123 * 1.23) + (1.123 * -1.23)，结果不为0，误差为2.328306E-10（所以把相乘的数统一转成正数再乘）
            // 除法没这些问题

            // x是否为负数
            bool minusX = x.serializedValue < 0;
            // y是否为负数
            bool minusY = y.serializedValue < 0;
            bool minusRes = minusX != minusY;

            if (minusX) {
                if (x.serializedValue == long.MinValue) {
                    x.serializedValue = long.MaxValue;
                }
                else {
                    x.serializedValue = -x.serializedValue;
                }
            }

            if (minusY) {
                if (y.serializedValue == long.MinValue) {
                    y.serializedValue = long.MaxValue;
                }
                else {
                    y.serializedValue = -y.serializedValue;
                }
            }

            // xDecimal 为 x 的小数部分，这里的xDecimal是带正负的，因为转成了ulong，所以负数是大于long.max的
            ulong xFraction = (ulong)(x.serializedValue & 0xFFFFFFFFu);
            // xInteger 为 x 的整数部分
            long xInteger = x.serializedValue >> 32;
            // yDecimal 为 y 的小数部分
            ulong yFraction = (ulong)(y.serializedValue & 0xFFFFFFFFu);
            // yInteger 为 y 的整数部分
            long yInteger = y.serializedValue >> 32;

            // 小数部分乘积
            ulong productFraction = xFraction * yFraction;
            // x小数部分 * y整数部分
            long productXdYi = (long)xFraction * yInteger;
            // y小数部分 * x整数部分
            long productYdXi = (long)yFraction * xInteger;
            // 整数部分乘积
            long productInteger = xInteger * yInteger;

            // 最终的小数部分

            ulong finalFraction = productFraction >> 32;
            // 最终的整数部分
            long finalInteger = productInteger << 32;

            Fixp result = default;
            // 两个数相乘 = 整数部分的乘积 + 小数部分的乘积 + 相互间小数与整数的交叉乘积的和
            result.serializedValue = (long)finalFraction + productXdYi + productYdXi + finalInteger;

            if (minusRes) {
                result.serializedValue = result.serializedValue == long.MinValue
                    ? long.MaxValue
                    : -result.serializedValue;
            }

            return result;
        }

        public static Fixp OverflowMul(Fixp x, Fixp y) {
            long serializedValue = x.serializedValue;
            long serializedValue2 = y.serializedValue;
            ulong num = (ulong)(serializedValue & 0xFFFFFFFFu);
            long num2 = serializedValue >> 32;
            ulong num3 = (ulong)(serializedValue2 & 0xFFFFFFFFu);
            long num4 = serializedValue2 >> 32;
            ulong num5 = num * num3;
            long num6 = (long)num * num4;
            long num7 = num2 * (long)num3;
            long num8 = num2 * num4;
            ulong x2 = num5 >> 32;
            long y2 = num6;
            long y3 = num7;
            long y4 = num8 << 32;
            bool overflow = false;
            long x3 = AddOverflowHelper((long)x2, y2, ref overflow);
            x3 = AddOverflowHelper(x3, y3, ref overflow);
            x3 = AddOverflowHelper(x3, y4, ref overflow);
            bool flag = ((serializedValue ^ serializedValue2) & long.MinValue) == 0;
            if (flag) {
                if (x3 < 0 || (overflow && serializedValue > 0)) {
                    return MaxValue;
                }
            }
            else if (x3 > 0) {
                return MinValue;
            }

            long num9 = num8 >> 32;
            if (num9 != 0L && num9 != -1) {
                return flag ? MaxValue : MinValue;
            }

            if (!flag) {
                long num10;
                long num11;
                if (serializedValue > serializedValue2) {
                    num10 = serializedValue;
                    num11 = serializedValue2;
                }
                else {
                    num10 = serializedValue2;
                    num11 = serializedValue;
                }

                if (x3 > num11 && num11 < -4294967296L && num10 > 4294967296L) {
                    return MinValue;
                }
            }

            return new Fixp(x3);
        }

        public static Fixp FastMul(Fixp x, Fixp y) {
            long serializedValue = x.serializedValue;
            long serializedValue2 = y.serializedValue;
            ulong num = (ulong)(serializedValue & 0xFFFFFFFFu);
            long num2 = serializedValue >> 32;
            ulong num3 = (ulong)(serializedValue2 & 0xFFFFFFFFu);
            long num4 = serializedValue2 >> 32;
            ulong num5 = num * num3;
            long num6 = (long)num * num4;
            long num7 = num2 * (long)num3;
            long num8 = num2 * num4;
            ulong num9 = num5 >> 32;
            long num10 = num6;
            long num11 = num7;
            long num12 = num8 << 32;
            Fixp result = default(Fixp);
            long num13 = (result.serializedValue = (long)num9 + num10 + num11 + num12);
            return result;
        }

        public static int CountLeadingZeroes(ulong x) {
            int num = 0;
            while ((x & 0xF000000000000000uL) == 0) {
                num += 4;
                x <<= 4;
            }

            while ((x & 0x8000000000000000uL) == 0) {
                num++;
                x <<= 1;
            }

            return num;
        }

        public static Fixp operator /(Fixp x, Fixp y) {
            long serializedValue = x.serializedValue;
            long serializedValue2 = y.serializedValue;
            if (serializedValue2 == 0) {
                return long.MaxValue;
            }

            ulong num = (ulong)((serializedValue >= 0) ? serializedValue : (-serializedValue));
            ulong num2 = (ulong)((serializedValue2 >= 0) ? serializedValue2 : (-serializedValue2));
            ulong num3 = 0uL;
            int num4 = 33;
            while ((num2 & 0xF) == 0L && num4 >= 4) {
                num2 >>= 4;
                num4 -= 4;
            }

            while (num != 0L && num4 >= 0) {
                int num5 = CountLeadingZeroes(num);
                if (num5 > num4) {
                    num5 = num4;
                }

                num <<= num5;
                num4 -= num5;
                ulong num6 = num / num2;
                num %= num2;
                num3 += num6 << num4;
                if ((num6 & ~(ulong.MaxValue >> num4)) != 0) {
                    return (((serializedValue ^ serializedValue2) & long.MinValue) == 0L) ? MaxValue : MinValue;
                }

                num <<= 1;
                num4--;
            }

            num3++;
            long num7 = (long)(num3 >> 1);
            if (((serializedValue ^ serializedValue2) & long.MinValue) != 0) {
                num7 = -num7;
            }

            return new Fixp(num7);
        }

        public static Fixp operator %(Fixp x, Fixp y) {
            return new Fixp(((x.serializedValue == long.MinValue) & (y.serializedValue == -1))
                ? 0
                : (x.serializedValue % y.serializedValue));
        }

        public static Fixp FastMod(Fixp x, Fixp y) {
            return new Fixp(x.serializedValue % y.serializedValue);
        }

        public static Fixp operator -(Fixp x) {
            return (x.serializedValue == long.MinValue) ? MaxValue : new Fixp(-x.serializedValue);
        }

        public static bool operator ==(Fixp x, Fixp y) {
            return x.serializedValue == y.serializedValue;
        }

        public static bool operator !=(Fixp x, Fixp y) {
            return x.serializedValue != y.serializedValue;
        }

        public static bool operator >(Fixp x, Fixp y) {
            return x.serializedValue > y.serializedValue;
        }

        public static bool operator <(Fixp x, Fixp y) {
            return x.serializedValue < y.serializedValue;
        }

        public static bool operator >=(Fixp x, Fixp y) {
            return x.serializedValue >= y.serializedValue;
        }

        public static bool operator <=(Fixp x, Fixp y) {
            return x.serializedValue <= y.serializedValue;
        }

        public static implicit operator Fixp(long value) {
            return new Fixp(value * 4294967296L);
        }

        public static explicit operator long(Fixp value) {
            return value.serializedValue >> 32;
        }

        // public static implicit operator Fixp(float value) { return new Fixp((long) (value * 4.2949673E+09f)); }

        public static implicit operator Fixp(float value) {
            return new Fixp((long)(value * 4294967296.0f));
        }

        // public static explicit operator float(Fixp value) { return (float) value.m_serializedValue / 4.2949673E+09f; }

        public static explicit operator float(Fixp value) {
            return (float)value.serializedValue / 4294967296.0f;
        }

        public static implicit operator Fixp(double value) {
            return new Fixp((long)(value * 4294967296.0));
        }

        public static explicit operator double(Fixp value) {
            return (double)value.serializedValue / 4294967296.0;
        }

        public static explicit operator Fixp(decimal value) {
            return new Fixp((long)(value * new decimal(4294967296L)));
        }

        public static implicit operator Fixp(int value) {
            return new Fixp(value * 4294967296L);
        }

        public static explicit operator decimal(Fixp value) {
            return (decimal)value.serializedValue / new decimal(4294967296L);
        }
    }
}