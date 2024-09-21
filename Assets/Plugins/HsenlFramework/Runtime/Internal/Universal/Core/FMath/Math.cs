using System;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public static class Math {
        /// <summary>表示圆的周长与其直径的比值，由常数 π 指定。</summary>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.PI?view=netframework-4.7.2">`Math.PI` on docs.microsoft.com</a></footer>
#if FIXED_MATH
        public static readonly FLOAT Pi = new FLOAT(13493037704L);
#else
        public static readonly FLOAT Pi = (float)System.Math.PI;
#endif

        /// <summary>
        /// 2分之π
        /// </summary>
        public static readonly FLOAT PiOver2 = Pi * 0.5f;

        /// <summary>
        /// ε 表示一个比较小的数，可以用来防止除0
        /// </summary>
        public static readonly FLOAT Epsilon = 0.000001f;

        /// <summary>
        /// 角度转弧度
        /// </summary>
        public static readonly FLOAT Deg2Rad = Pi / 180f;

        /// <summary>
        /// 弧度转角度
        /// </summary>
        public static readonly FLOAT Rad2Deg = 180f / Pi;

        /// <summary>
        ///   表示自然对数的底，它由常数 <see langword="e" /> 指定。
        /// </summary>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.E?view=netframework-4.7.2">`Math.E` on docs.microsoft.com</a></footer>
        public static FLOAT E = 2.71828182845905f;

        public static bool CompareApproximate(FLOAT a, FLOAT b) {
            return Abs(a - b) < Epsilon;
        }

        public static bool CompareApproximateZero(FLOAT a) {
            return Abs(a - 0) < Epsilon;
        }

        /// <summary>返回指定数字的平方根。</summary>
        /// <param name="x">将查找其平方根的数字。</param>
        /// <returns>
        /// 下表中的值之一。
        /// 
        ///         <paramref name="x" />参数
        /// 
        ///         返回值
        /// 
        ///         零或正数
        /// 
        ///         正平方根<paramref name="x" />。
        /// 
        ///         负数
        /// 
        ///         <see cref="F:System.Double.NaN" />
        /// 
        ///         等于<see cref="F:System.Double.NaN" />
        /// 
        ///         <see cref="F:System.Double.NaN" />
        /// 
        ///         等于<see cref="F:System.Double.PositiveInfinity" />
        /// 
        ///         <see cref="F:System.Double.PositiveInfinity" />
        ///       </returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.Sqrt?view=netframework-4.7.2">`Math.Sqrt` on docs.microsoft.com</a></footer>
        public static FLOAT Sqrt(FLOAT x) {
#if FIXED_MATH
            long serializedValue = x.m_serializedValue;
            if (serializedValue < 0)
            {
                throw new ArgumentOutOfRangeException($"Negative value passed to Sqrt: '{x}'");
            }

            ulong num = (ulong) serializedValue;
            ulong num2 = 0uL;
            ulong num3;
            for (num3 = 4611686018427387904uL; num3 > num; num3 >>= 2)
            {
            }

            for (int i = 0; i < 2; i++)
            {
                while (num3 != 0)
                {
                    if (num >= num2 + num3)
                    {
                        num -= num2 + num3;
                        num2 = (num2 >> 1) + num3;
                    }
                    else
                    {
                        num2 >>= 1;
                    }

                    num3 >>= 2;
                }

                if (i == 0)
                {
                    if (num > uint.MaxValue)
                    {
                        num -= num2;
                        num = (num << 32) - 2147483648u;
                        num2 = (num2 << 32) + 2147483648u;
                    }
                    else
                    {
                        num <<= 32;
                        num2 <<= 32;
                    }

                    num3 = 1073741824uL;
                }
            }

            if (num > num2)
            {
                num2++;
            }

            return new FLOAT((long) num2);
#else
            return (FLOAT)System.Math.Sqrt(x);
#endif
        }

        public static FLOAT Max(FLOAT val1, FLOAT val2) {
            return val1 > val2 ? val1 : val2;
        }

        public static FLOAT Max(FLOAT val1, FLOAT val2, FLOAT val3) {
            FLOAT fix = val1 > val2 ? val1 : val2;
            return fix > val3 ? fix : val3;
        }
        
        public static int Max(int val1, int val2) {
            return val1 > val2 ? val1 : val2;
        }

        public static int Max(int val1, int val2, int val3) {
            int fix = val1 > val2 ? val1 : val2;
            return fix > val3 ? fix : val3;
        }

        public static FLOAT Min(FLOAT val1, FLOAT val2) {
            return val1 < val2 ? val1 : val2;
        }

        public static FLOAT Min(FLOAT val1, FLOAT val2, FLOAT val3) {
            val1 = val1 < val2 ? val1 : val2;
            return val1 < val3 ? val1 : val3;
        }

        public static int Min(int val1, int val2) {
            return val1 > val2 ? val2 : val1;
        }

        public static int Min(int val1, int val2, int val3) {
            val1 = val1 < val2 ? val1 : val2;
            return val1 < val3 ? val1 : val3;
        }

        public static FLOAT Clamp(FLOAT value, FLOAT min, FLOAT max) {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        public static int Clamp(int value, int min, int max) {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        public static FLOAT Clamp01(FLOAT value) {
            if (value < 0) return 0;

            if (value > 1) return 1;

            return value;
        }

        /// <summary>
        /// 正弦值
        /// </summary>
        /// <param name="rad">以弧度计量的角度。</param>
        /// <returns>
        ///   <paramref name="rad" /> 的正弦值。
        ///    如果 <paramref name="rad" /> 等于 <see cref="F:System.Double.NaN" />、<see cref="F:System.Double.NegativeInfinity" /> 或 <see cref="F:System.Double.PositiveInfinity" />，此方法将返回 <see cref="F:System.Double.NaN" />。
        /// </returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.Sin?view=netframework-4.7.2">`Math.Sin` on docs.microsoft.com</a></footer>
        public static FLOAT Sin(FLOAT rad) {
#if FIXED_MATH
            bool flipHorizontal;
            bool flipVertical;
            long rawValue = ClampSinValue(rad.m_serializedValue, out flipHorizontal, out flipVertical);
            FLOAT x2 = new FLOAT(rawValue);
            FLOAT value = FLOAT.FastMul(x2, FLOAT.LutInterval);
            FLOAT fix = Round(value);
            int num = 0;
            FLOAT x3 = new FLOAT(FLOAT.SinLut[(int) (flipHorizontal? (FLOAT.SinLut.Length - 1 - (int) (long) fix) : ((long) fix))]);
            FLOAT y = new FLOAT(FLOAT.SinLut[
                flipHorizontal
                        ? (FLOAT.SinLut.Length - 1 - (int) (long) fix - Sign(num))
                        : ((int) (long) fix + Sign(num))]);
            long serializedValue = FLOAT.FastMul(num, FastAbs(FLOAT.FastSub(x3, y))).m_serializedValue;
            long num2 = x3.m_serializedValue + (flipHorizontal? (-serializedValue) : serializedValue);
            long rawValue2 = (flipVertical? (-num2) : num2);
            return new FLOAT(rawValue2);
#else
            return (FLOAT)System.Math.Sin(rad);
#endif
        }

        public static FLOAT FastSin(FLOAT rad) {
#if FIXED_MATH
            bool flipHorizontal;
            bool flipVertical;
            long num = ClampSinValue(rad.m_serializedValue, out flipHorizontal, out flipVertical);
            uint num2 = (uint) (num >> 15);
            if (num2 >= 205887)
            {
                num2 = 205886u;
            }

            long num3 = FLOAT.SinLut[flipHorizontal? (FLOAT.SinLut.Length - 1 - (int) num2) : ((int) num2)];
            return new FLOAT(flipVertical? (-num3) : num3);
#else
            return (FLOAT)System.Math.Sin(rad);
#endif
        }

        public static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical) {
            long num = angle % 26986075409L;
            if (angle < 0) {
                num += 26986075409L;
            }

            flipVertical = num >= 13493037704L;
            long num2;
            for (num2 = num; num2 >= 13493037704L; num2 -= 13493037704L) { }

            flipHorizontal = num2 >= 6746518852L;
            long num3 = num2;
            if (num3 >= 6746518852L) {
                num3 -= 6746518852L;
            }

            return num3;
        }

        /// <summary>
        /// 余弦值
        /// </summary>
        /// <param name="rad">以弧度计量的角度。</param>
        /// <returns>
        ///   <paramref name="rad" /> 的余弦值。
        ///    如果 <paramref name="rad" /> 等于 <see cref="F:System.Double.NaN" />、<see cref="F:System.Double.NegativeInfinity" /> 或 <see cref="F:System.Double.PositiveInfinity" />，此方法将返回 <see cref="F:System.Double.NaN" />。
        /// </returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.Cos?view=netframework-4.7.2">`Math.Cos` on docs.microsoft.com</a></footer>
        public static FLOAT Cos(FLOAT rad) {
#if FIXED_MATH
            long serializedValue = rad.m_serializedValue;
            long rawValue = serializedValue + ((serializedValue > 0)? (-20239556556L) : 6746518852L);
            return Sin(new FLOAT(rawValue));
#else
            return (FLOAT)System.Math.Cos(rad);
#endif
        }

        public static FLOAT FastCos(FLOAT rad) {
#if FIXED_MATH
            long serializedValue = rad.m_serializedValue;
            long rawValue = serializedValue + ((serializedValue > 0)? (-20239556556L) : 6746518852L);
            return FastSin(new FLOAT(rawValue));
#else
            return (FLOAT)System.Math.Cos(rad);
#endif
        }

        /// <summary>
        /// 正切值
        /// </summary>
        /// <param name="rad">以弧度计量的角度。</param>
        /// <returns>
        ///   <paramref name="rad" /> 的正切值。
        ///    如果 <paramref name="rad" /> 等于 <see cref="F:System.Double.NaN" />、<see cref="F:System.Double.NegativeInfinity" /> 或 <see cref="F:System.Double.PositiveInfinity" />，此方法将返回 <see cref="F:System.Double.NaN" />。
        /// </returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.Tan?view=netframework-4.7.2">`Math.Tan` on docs.microsoft.com</a></footer>
        public static FLOAT Tan(FLOAT rad) {
#if FIXED_MATH
            long num = rad.m_serializedValue % 13493037704L;
            bool flag = false;
            if (num < 0)
            {
                num = -num;
                flag = true;
            }

            if (num > 6746518852L)
            {
                flag = !flag;
                num = 6746518852L - (num - 6746518852L);
            }

            FLOAT x2 = new FLOAT(num);
            FLOAT fix = FLOAT.FastMul(x2, FLOAT.LutInterval);
            FLOAT fix2 = Round(fix);
            FLOAT fix3 = FLOAT.FastSub(fix, fix2);
            FLOAT x3 = new FLOAT(FLOAT.TanLut[(int) (long) fix2]);
            long serializedValue = FLOAT.FastMul(fix3,
                        FastAbs(FLOAT.FastSub(y: new FLOAT(FLOAT.TanLut[(int) (long) fix2 + Sign(fix3)]), x: x3)))
                    .m_serializedValue;
            long num2 = x3.m_serializedValue + serializedValue;
            long rawValue = (flag? (-num2) : num2);
            return new FLOAT(rawValue);
#else
            return (FLOAT)System.Math.Tan(rad);
#endif
        }

        /// <summary>
        /// 反正弦值
        /// </summary>
        /// <param name="rad">
        ///   一个表示正弦值的数字，其中 <paramref name="rad" /> 必须大于或等于 -1 但小于或等于 1。
        /// </param>
        /// <returns>
        ///   角度 θ，以弧度为单位，满足 π/2 ≤θ≤π/2
        /// 
        ///   - 或 -
        /// 
        ///   如果 <paramref name="rad" /> &lt; -1 或 <paramref name="rad" /> &gt; 1 或 <paramref name="rad" /> 等于 <see cref="F:System.Double.NaN" />，则为 <see cref="F:System.Double.NaN" />。
        /// </returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.Asin?view=netframework-4.7.2">`Math.Asin` on docs.microsoft.com</a></footer>
        public static FLOAT Asin(FLOAT rad) {
#if FIXED_MATH
            return FLOAT.FastSub(PiOver2, Acos(rad));
#else
            return (FLOAT)System.Math.Asin(rad);
#endif
        }

        /// <summary>
        /// 反余弦值
        /// </summary>
        /// <param name="rad">
        ///   一个表示余弦值的数字，其中 <paramref name="rad" /> 必须大于或等于 -1 但小于或等于 1。
        /// </param>
        /// <returns>
        ///   角度 θ，以弧度为单位，满足 0 ≤θ≤π
        /// 
        ///   - 或 -
        /// 
        ///   如果 <paramref name="rad" /> &lt; -1 或 <paramref name="rad" /> &gt; 1 或 <paramref name="rad" /> 等于 <see cref="F:System.Double.NaN" />，则为 <see cref="F:System.Double.NaN" />。
        /// </returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.Acos?view=netframework-4.7.2">`Math.Acos` on docs.microsoft.com</a></footer>
        public static FLOAT Acos(FLOAT rad) {
#if FIXED_MATH
            if (rad == 0)
            {
                return PiOver2;
            }

            bool flag = false;
            if (rad < 0)
            {
                rad = -rad;
                flag = true;
            }

            FLOAT fix = FLOAT.FastMul(rad, 205887);
            FLOAT fix2 = Round(fix);
            if (fix2 >= 205887)
            {
                fix2 = 205886;
            }

            FLOAT fix3 = FLOAT.FastSub(fix, fix2);
            FLOAT x = new FLOAT(FLOAT.AcosLut[(int) (long) fix2]);
            int num = (int) (long) fix2 + Sign(fix3);
            if (num >= 205887)
            {
                num = 205886;
            }

            long serializedValue = FLOAT.FastMul(fix3, FastAbs(FLOAT.FastSub(y: new FLOAT(FLOAT.AcosLut[num]), x: x)))
                    .m_serializedValue;
            FLOAT fix4 = new FLOAT(x.m_serializedValue + serializedValue);
            return flag? (Pi - fix4) : fix4;
#else
            return (FLOAT)System.Math.Acos(rad);
#endif
        }

        /// <summary>
        /// 反正切值
        /// </summary>
        /// <param name="rad">表示正切值的数字。</param>
        /// <returns>
        ///   角度 θ，以弧度为单位，满足 -π/2 ≤θ≤π/2。
        /// 
        ///   - 或 -
        /// 
        ///   如果 <paramref name="rad" /> 等于 <see cref="F:System.Double.NaN" />，则为 <see cref="F:System.Double.NaN" />；如果 <paramref name="rad" /> 等于 <see cref="F:System.Double.NegativeInfinity" />，则为舍入为双精度值 (-1.5707963267949) 的 -π/2；或者如果 <paramref name="rad" /> 等于 <see cref="F:System.Double.PositiveInfinity" />，则为舍入为双精度值 (1.5707963267949) 的 π/2。
        /// </returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.Atan?view=netframework-4.7.2">`Math.Atan` on docs.microsoft.com</a></footer>
        public static FLOAT Atan(FLOAT rad) {
#if FIXED_MATH
            return Atan2(rad, 1);
#else
            return (FLOAT)System.Math.Atan(rad);
#endif
        }

        /// <summary>返回正切值为两个指定数字的商的角度。</summary>
        /// <param name="y">点的 y 坐标。</param>
        /// <param name="x">点的 x 坐标。</param>
        /// <returns>
        ///   角度 θ，以弧度为单位，满足 -π≤θ≤π，且 tan(θ) = <paramref name="y" /> / <paramref name="x" />，其中 (<paramref name="x" />, <paramref name="y" />) 是笛卡尔平面中的点。
        ///    请看下面：
        /// 
        ///       For (<paramref name="x" />, <paramref name="y" />) in quadrant 1, 0 &lt;&gt;θ &lt;&gt;π/2.
        /// 
        ///       For (<paramref name="x" />, <paramref name="y" />) in quadrant 2, π/2 &lt;&gt;θ≤π.
        /// 
        ///       For (<paramref name="x" />, <paramref name="y" />) in quadrant 3, -π &lt;&gt;θ &lt;&gt;π/2.
        /// 
        ///       For (<paramref name="x" />, <paramref name="y" />) in quadrant 4, -π/2 &lt;&gt;θ&lt; 0.&gt;&lt;/ 0.&gt;
        /// 
        ///   如果点在象限的边界上，则返回值如下：
        /// 
        ///       如果 y 为 0 并且 x 不为负值，则 θ = 0。
        /// 
        ///       如果 y 为 0 并且 x 为负值，则 θ = π。
        /// 
        ///       如果 y 为正值并且 x 为 0，则 θ = π/2。
        /// 
        ///       如果 y 为负值并且 x 为 0，则 θ = -π/2。
        /// 
        ///       如果 y 为 0 并且 x 为 0，则 θ = 0。
        /// 
        ///   如果 <paramref name="x" /> 或 <paramref name="y" /> 为 <see cref="F:System.Double.NaN" />，或者如果 <paramref name="x" /> 和 <paramref name="y" /> 为 <see cref="F:System.Double.PositiveInfinity" /> 或 <see cref="F:System.Double.NegativeInfinity" />，则该方法返回 <see cref="F:System.Double.NaN" />。
        /// </returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Math.Atan2?view=netframework-4.7.2">`Math.Atan2` on docs.microsoft.com</a></footer>
        public static FLOAT Atan2(FLOAT y, FLOAT x) {
#if FIXED_MATH
            long serializedValue = y.m_serializedValue;
            long serializedValue2 = x.m_serializedValue;
            if (serializedValue2 == 0)
            {
                if (serializedValue > 0)
                {
                    return PiOver2;
                }

                if (serializedValue == 0)
                {
                    return FLOAT.Zero;
                }

                return -PiOver2;
            }

            FLOAT fix = y / x;
            FLOAT fix2 = FLOAT.EN2 * 28;
            if (FLOAT.One + fix2 * fix * fix == FLOAT.MaxValue)
            {
                return (y < FLOAT.Zero)? (-PiOver2) : PiOver2;
            }

            FLOAT fix3;
            if (Abs(fix) < FLOAT.One)
            {
                fix3 = fix / (FLOAT.One + fix2 * fix * fix);
                if (serializedValue2 < 0)
                {
                    if (serializedValue < 0)
                    {
                        return fix3 - Pi;
                    }

                    return fix3 + Pi;
                }
            }
            else
            {
                fix3 = PiOver2 - fix / (fix * fix + fix2);
                if (serializedValue < 0)
                {
                    return fix3 - Pi;
                }
            }

            return fix3;
#else
            return (FLOAT)System.Math.Atan2(y, x);
#endif
        }

        /// <summary>
        /// 将数字向下舍入最接近的整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FLOAT Floor(FLOAT value) {
#if FIXED_MATH
            return new FLOAT(value.m_serializedValue & -4294967296L);
#else
            return (FLOAT)System.Math.Floor(value);
#endif
        }

        /// <summary>
        /// 将数字向上舍入最接近的整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FLOAT Ceiling(FLOAT value) {
#if FIXED_MATH
            return ((value.m_serializedValue & 0xFFFFFFFFu) != 0)? (Floor(value) + FLOAT.One) : value;
#else
            return (FLOAT)System.Math.Ceiling(value);
#endif
        }

        /// <summary>
        /// 四舍五入为最接近的整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FLOAT Round(FLOAT value) {
#if FIXED_MATH
            long num = value.m_serializedValue & 0xFFFFFFFFu;
            FLOAT fix = Floor(value);
            if (num < 2147483648u)
            {
                return fix;
            }

            if (num > 2147483648u)
            {
                return fix + FLOAT.One;
            }

            return ((fix.m_serializedValue & 0x100000000L) == 0L)? fix : (fix + FLOAT.One);
#else
            return (FLOAT)System.Math.Round(value);
#endif
        }

        /// <summary>
        /// 返回一个表示Fix64数字符号的数字。
        /// 如果值为正则返回1，如果值为0则返回0，如果值为负则返回-1。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Sign(FLOAT value) {
            // 这个方法统一做了修改，改成和unity的一致，也就是 == 0时，按照正数表示，这样更合理，因为 0本身就是个正数
#if FIXED_MATH
            // return (value.m_serializedValue < 0)? (-1) : ((value.m_serializedValue > 0)? 1 : 0);
            return value.m_serializedValue >= 0? 1 : -1;
#else
            // return System.Math.Sign(value);
            return value >= 0 ? 1 : -1;
#endif
        }

        /// <summary>
        /// 绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FLOAT Abs(FLOAT value) {
#if FIXED_MATH
            if (value.m_serializedValue == long.MinValue)
            {
                return FLOAT.MaxValue;
            }

            long num = value.m_serializedValue >> 63;
            return new FLOAT((value.m_serializedValue + num) ^ num);
#else
            return System.Math.Abs(value);
#endif
        }

        public static int Abs(int value) {
            if (value < 0) {
                return -value;
            }

            return value;
        }

        public static FLOAT FastAbs(FLOAT value) {
#if FIXED_MATH
            long num = value.m_serializedValue >> 63;
            return new FLOAT((value.m_serializedValue + num) ^ num);
#else
            return System.Math.Abs(value);
#endif
        }

        /// <summary>
        /// 重心
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="amount1"></param>
        /// <param name="amount2"></param>
        /// <returns></returns>
        public static FLOAT Barycentric(FLOAT value1, FLOAT value2, FLOAT value3, FLOAT amount1, FLOAT amount2) {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        /// <summary>
        /// 曲线插值
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static FLOAT CatmullRom(FLOAT value1, FLOAT value2, FLOAT value3, FLOAT value4, FLOAT percent) {
            FLOAT fix = percent * percent;
            FLOAT fix2 = fix * percent;
            var result = 0.5 * (2.0 * value2 + (value3 - value1) * percent +
                                (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * fix +
                                (3.0 * value2 - value1 - 3.0 * value3 + value4) * fix2);
            return (FLOAT)result;
        }

        /// <summary>
        /// 埃尔米特插值
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="tangent1"></param>
        /// <param name="value2"></param>
        /// <param name="tangent2"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static FLOAT Hermite(FLOAT value1, FLOAT tangent1, FLOAT value2, FLOAT tangent2, FLOAT percent) {
            FLOAT fix = percent * percent * percent;
            FLOAT fix2 = percent * percent;
            if (CompareApproximateZero(percent)) {
                return value1;
            }

            if (CompareApproximate(percent, 1f)) {
                return value2;
            }

            return (2 * value1 - 2 * value2 + tangent2 + tangent1) * fix +
                   (3 * value2 - 3 * value1 - 2 * tangent1 - tangent2) * fix2 + tangent1 * percent + value1;
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static FLOAT Lerp(FLOAT value1, FLOAT value2, FLOAT percent) {
            return value1 + (value2 - value1) * Clamp01(percent);
        }

        public static FLOAT LerpUnclamped(FLOAT value1, FLOAT value2, FLOAT percent) {
            return value1 + (value2 - value1) * percent;
        }

        /// <summary>
        /// 反线性插值
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static FLOAT InverseLerp(FLOAT value1, FLOAT value2, FLOAT percent) {
            if (!CompareApproximate(value1, value2)) {
                return Clamp01((percent - value1) / (value2 - value1));
            }

            return 0;
        }

        /// <summary>
        /// 平滑插值
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static FLOAT SmoothStep(FLOAT value1, FLOAT value2, FLOAT percent) {
            FLOAT amount2 = Clamp(percent, 0f, 1f);
            return Hermite(value1, 0f, value2, 0f, amount2);
        }

        /// <summary>
        /// 平方
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        internal static FLOAT Pow2(FLOAT x) {
#if FIXED_MATH
            if (x.RawValue == 0)
            {
                return FLOAT.One;
            }

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = x.RawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == FLOAT.One)
            {
                return neg? FLOAT.One / (FLOAT) 2 : (FLOAT) 2;
            }

            if (x >= FLOAT.Log2Max)
            {
                return neg? FLOAT.One / FLOAT.MaxValue : FLOAT.MaxValue;
            }

            if (x <= FLOAT.Log2Min)
            {
                return neg? FLOAT.MaxValue : FLOAT.Zero;
            }

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = (int) Floor(x);
            // Take fractional part of exponent
            x = new FLOAT(x.RawValue & 0x00000000FFFFFFFF);

            var result = FLOAT.One;
            var term = FLOAT.One;
            int i = 1;
            while (term.RawValue != 0)
            {
                term = FLOAT.FastMul(FLOAT.FastMul(x, term), FLOAT.Ln2) / (FLOAT) i;
                result += term;
                i++;
            }

            result = new FLOAT(result.RawValue << integerPart);
            if (neg)
            {
                result = FLOAT.One / result;
            }

            return result;
#else
            return (FLOAT)System.Math.Pow(x, 2);
#endif
        }

        /// <summary>
        /// b的exp次方
        /// </summary>
        /// <param name="b"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static FLOAT Pow(FLOAT b, FLOAT exp) {
#if FIXED_MATH
            if (b == FLOAT.One)
            {
                return FLOAT.One;
            }

            if (exp.RawValue == 0)
            {
                return FLOAT.One;
            }

            if (b.RawValue == 0)
            {
                if (exp.RawValue < 0)
                {
                    //throw new DivideByZeroException();
                    return FLOAT.MaxValue;
                }

                return FLOAT.Zero;
            }

            FLOAT log2 = Log2(b);
            return Pow2(exp * log2);
#else
            return (FLOAT)System.Math.Pow(b, exp);
#endif
        }

        /// <summary>
        /// 返回以指定基数表示的指定数的对数
        /// </summary>
        /// <param name="f">指定数</param>
        /// <param name="p">基数</param>
        /// <returns></returns>
        public static FLOAT Log(FLOAT f, FLOAT p) {
#if FIXED_MATH
            return System.Math.Log(f.AsFloat(), p.AsFloat());
#else
            return (FLOAT)System.Math.Log(f, p);
#endif
        }

        /// <summary>
        /// 返回以指定基数e表示的指定数的对数
        /// </summary>
        /// <param name="f">指定数</param>
        /// <returns></returns>
        public static FLOAT Log(FLOAT f) {
#if FIXED_MATH
            return System.Math.Log(f.AsFloat());
#else
            return (FLOAT)System.Math.Log(f);
#endif
        }

        /// <summary>
        /// 返回指定数字以 2 为底的对数。
        /// 提供至少9个小数的精度。
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static FLOAT Log2(FLOAT x) {
#if FIXED_MATH
            if (x.RawValue <= 0)
            {
                throw new ArgumentOutOfRangeException($"Non-positive value passed to Ln: '{x}'");
            }

            // This implementation is based on Clay. S. Turner's fast binary logarithm
            // algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
            //     Processing Mag., pp. 124,140, Sep. 2010.)

            long b = 1U << (FLOAT.FRACTIONAL_PLACES - 1);
            long y = 0;

            long rawX = x.RawValue;
            while (rawX < FLOAT.ONE)
            {
                rawX <<= 1;
                y -= FLOAT.ONE;
            }

            while (rawX >= (FLOAT.ONE << 1))
            {
                rawX >>= 1;
                y += FLOAT.ONE;
            }

            var z = new FLOAT(rawX);

            for (int i = 0; i < FLOAT.FRACTIONAL_PLACES; i++)
            {
                z = FLOAT.FastMul(z, z);
                if (z.RawValue >= (FLOAT.ONE << 1))
                {
                    z = new FLOAT(z.RawValue >> 1);
                    y += b;
                }

                b >>= 1;
            }

            return new FLOAT(y);
#else
            return (FLOAT)System.Math.Log(x, 2);
#endif
        }

        /// <summary>
        /// 返回指定数字以 2 为底的对数。
        /// 提供至少9个小数的精度。
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static FLOAT Log10(FLOAT f) {
#if FIXED_MATH
            return System.Math.Log10(f.AsFloat());
#else
            return (FLOAT)System.Math.Log10(f);
#endif
        }

        public static FLOAT Exp(FLOAT f) {
#if FIXED_MATH
            return System.Math.Exp(f.AsFloat());
#else
            return (FLOAT)System.Math.Exp(f);
#endif
        }

        /// <summary>
        /// 趋向目标值
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDelta"></param>
        /// <returns></returns>
        public static FLOAT MoveTowards(FLOAT current, FLOAT target, FLOAT maxDelta) {
            if (Abs(target - current) <= maxDelta) return target;
            return (current + (Sign(target - current)) * maxDelta);
        }

        /// <summary>
        /// 区间循环，确保t不会大于length，也不会小于0
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static FLOAT Repeat(FLOAT t, FLOAT length) {
            return (t - (Floor(t / length) * length));
        }

        /// <summary>
        /// 夹角的增量（即两夹角的差），确保夹角在-180 ~ 180区间
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static FLOAT DeltaAngle(FLOAT current, FLOAT target) {
            FLOAT num = Repeat(target - current, 360f);
            if (num > 180f) {
                num -= 360f;
            }

            return num;
        }

        /// <summary>
        /// 转向角
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDelta"></param>
        /// <returns></returns>
        public static FLOAT MoveTowardsAngle(FLOAT current, FLOAT target, float maxDelta) {
            target = current + DeltaAngle(current, target);
            return MoveTowards(current, target, maxDelta);
        }

        /// <summary>
        /// 平滑阻尼
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="smoothTime"></param>
        /// <param name="maxSpeed"></param>
        /// <returns></returns>
        public static FLOAT SmoothDamp(FLOAT current, FLOAT target, ref FLOAT currentVelocity, FLOAT smoothTime,
            FLOAT maxSpeed) {
#if FIXED_MATH
            FLOAT deltaTime = FLOAT.EN2;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
#else
            FLOAT deltaTime = 0.01f;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
#endif
        }

        /// <summary>
        /// 平滑阻尼
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="smoothTime"></param>
        /// <returns></returns>
        public static FLOAT SmoothDamp(FLOAT current, FLOAT target, ref FLOAT currentVelocity, FLOAT smoothTime) {
#if FIXED_MATH
            FLOAT deltaTime = FLOAT.EN2;
            FLOAT positiveInfinity = -FLOAT.MaxValue;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
#else
            FLOAT deltaTime = 0.01f;
            FLOAT positiveInfinity = -FLOAT.MaxValue;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
#endif
        }

        /// <summary>
        /// 平滑阻尼
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="smoothTime"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static FLOAT SmoothDamp(FLOAT current, FLOAT target, ref FLOAT currentVelocity, FLOAT smoothTime,
            FLOAT maxSpeed, FLOAT deltaTime) {
#if FIXED_MATH
            smoothTime = Max(FLOAT.EN4, smoothTime);
            FLOAT num = (FLOAT) 2f / smoothTime;
            FLOAT num2 = num * deltaTime;
            FLOAT num3 = FLOAT.One / (((FLOAT.One + num2) + (((FLOAT) 0.48f * num2) * num2)) +
                ((((FLOAT) 0.235f * num2) * num2) * num2));
            FLOAT num4 = current - target;
            FLOAT num5 = target;
            FLOAT max = maxSpeed * smoothTime;
            num4 = Clamp(num4, -max, max);
            target = current - num4;
            FLOAT num7 = (currentVelocity + (num * num4)) * deltaTime;
            currentVelocity = (currentVelocity - (num * num7)) * num3;
            FLOAT num8 = target + ((num4 + num7) * num3);
            if (((num5 - current) > FLOAT.Zero) == (num8 > num5))
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }

            return num8;
#else
            smoothTime = Max(0.0001f, smoothTime);
            FLOAT num = 2f / smoothTime;
            FLOAT num2 = num * deltaTime;
            FLOAT num3 = 1 / (((1 + num2) + ((0.48f * num2) * num2)) +
                              (((0.235f * num2) * num2) * num2));
            FLOAT num4 = current - target;
            FLOAT num5 = target;
            FLOAT max = maxSpeed * smoothTime;
            num4 = Clamp(num4, -max, max);
            target = current - num4;
            FLOAT num7 = (currentVelocity + (num * num4)) * deltaTime;
            currentVelocity = (currentVelocity - (num * num7)) * num3;
            FLOAT num8 = target + ((num4 + num7) * num3);
            if (((num5 - current) > 0) == (num8 > num5)) {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }

            return num8;
#endif
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static FLOAT RadToDeg(FLOAT radians) {
            return radians * Rad2Deg;
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static FLOAT DegToRad(FLOAT degrees) {
            return degrees * Deg2Rad;
        }

        /// <summary>
        /// 只要大一点点都取+1后的值。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RoundToMaxInt(FLOAT value) {
            int iv = (int)value;
            return value > iv ? iv + 1 : iv;
        }

        /// <summary>
        /// 取到小数点后指定位数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="afterDP">小数点后几位</param>
        /// <returns></returns>
        public static FLOAT RoundToFloat(FLOAT value, int afterDP = 2) {
            int number = 1;
            for (int i = 0; i < afterDP; i++) {
                number *= 10;
            }

            return (int)(value * number) / (FLOAT)number;
        }
    }
}