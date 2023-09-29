using System;
using System.Runtime.InteropServices;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct FVector3 : IEquatable<FVector3> {
        static FLOAT ZeroEpsilonSq = FMath.Epsilon;
        internal static FVector3 InternalZero;
        internal static FVector3 Arbitrary = new FVector3(1);

        public static readonly FVector3 Zero;
        public static readonly FVector3 One = new FVector3(1);
        public static readonly FVector3 Left = new FVector3(-1, 0, 0);
        public static readonly FVector3 Right = new FVector3(1, 0, 0);
        public static readonly FVector3 Up = new FVector3(0, 1, 0);
        public static readonly FVector3 Down = new FVector3(0, -1, 0);
        public static readonly FVector3 Back = new FVector3(0, 0, -1);
        public static readonly FVector3 Forward = new FVector3(0, 0, 1);
        public static readonly FVector3 MinValue = new FVector3(FLOAT.MinValue);
        public static readonly FVector3 MaxValue = new FVector3(FLOAT.MaxValue);

        public FLOAT x;
        public FLOAT y;
        public FLOAT z;

        public FLOAT sqrMagnitude => this.x * this.x + this.y * this.y + this.z * this.z;
        public FLOAT magnitude => FMath.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);

        public FVector3 normalized {
            get {
                var val = new FVector3(this.x, this.y, this.z);
                val.Normalize();
                return val;
            }
        }

        public FLOAT this[int index] {
            get {
                return index switch {
                    0 => this.x,
                    1 => this.y,
                    2 => this.z,
                    _ => throw new IndexOutOfRangeException("vector idx invalid" + index)
                };
            }
            set {
                switch (index) {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }

        /// <summary>
        /// 返回一个区间值始终在0-360之间的欧拉角
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public static FVector3 MakePositive(ref FVector3 euler) {
            if (euler.x < 0) {
                euler.x += 360;
            }
            else if (euler.x >= 360) {
                euler.x -= 360;
            }

            if (euler.y < 0) {
                euler.y += 360;
            }
            else if (euler.y >= 360) {
                euler.y -= 360;
            }

            if (euler.z < 0) {
                euler.z += 360;
            }
            else if (euler.z >= 360) {
                euler.z -= 360;
            }

            return euler;
        }

        /// <summary>
        /// 向量的绝对值
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public static FVector3 Abs(FVector3 other) {
            return new FVector3(FMath.Abs(other.x), FMath.Abs(other.y), FMath.Abs(other.z));
        }

        /// <summary>
        /// 向量的绝对值
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public static void Abs(ref FVector3 other, out FVector3 result) {
            var x = FMath.Abs(other.x);
            var y = FMath.Abs(other.y);
            var z = FMath.Abs(other.z);

            result.x = x;
            result.y = y;
            result.z = z;
        }

        /// <summary>
        /// 限制一个向量的长度
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static FVector3 ClampMagnitude(FVector3 vector, FLOAT min, FLOAT max) {
            var result = vector;
            var magnitude = vector.magnitude;
            var val = FMath.Clamp(magnitude, min, max);
            if (!FMath.CompareApproximate(val, magnitude)) {
                var num = val / magnitude;
                result.x *= num;
                result.y *= num;
                result.z *= num;
            }

            return result;
        }

        /// <summary>
        /// 限制一个向量的长度
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void ClampMagnitude(ref FVector3 vector, FLOAT min, FLOAT max) {
            var magnitude = vector.magnitude;
            var val = FMath.Clamp(magnitude, min, max);
            if (!FMath.CompareApproximate(val, magnitude)) {
                var num = val / magnitude;
                vector.x *= num;
                vector.y *= num;
                vector.z *= num;
            }
        }

        public static void Clamp(ref FVector3 vector, ref FVector3 min, ref FVector3 max) {
            FLOAT xMin = min.x;
            FLOAT yMin = min.y;
            FLOAT zMin = min.z;
            FLOAT xMax = max.x;
            FLOAT yMax = max.y;
            FLOAT zMax = max.z;

            if (min.x > max.x) {
                xMin = max.x;
                xMax = min.x;
            }

            if (min.y > max.y) {
                yMin = max.y;
                yMax = min.y;
            }

            if (min.z > max.z) {
                zMin = max.z;
                zMax = min.z;
            }

            vector.x = FMath.Clamp(vector.x, xMin, xMax);
            vector.y = FMath.Clamp(vector.y, yMin, yMax);
            vector.z = FMath.Clamp(vector.z, zMin, zMax);
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static FVector3 Lerp(FVector3 from, FVector3 to, FLOAT percent) {
            // return from + (to - from) * amount;
            return new FVector3(FMath.Lerp(from.x, to.x, percent), FMath.Lerp(from.y, to.y, percent),
                FMath.Lerp(from.z, to.z, percent));
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="percent"></param>
        /// <param name="result"></param>
        public static void Lerp(ref FVector3 from, ref FVector3 to, FLOAT percent, out FVector3 result) {
            var x = FMath.Lerp(from.x, to.x, percent);
            var y = FMath.Lerp(from.y, to.y, percent);
            var z = FMath.Lerp(from.z, to.z, percent);

            result.x = x;
            result.y = y;
            result.z = z;
        }

        /// <summary>
        /// 不限制线性插值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static FVector3 LerpUnclamped(FVector3 from, FVector3 to, FLOAT percent) {
            return new FVector3(FMath.Lerp(from.x, to.x, percent), FMath.Lerp(from.y, to.y, percent),
                FMath.Lerp(from.z, to.z, percent));
        }

        /// <summary>
        /// 不限制线性插值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="percent"></param>
        /// <param name="result"></param>
        public static void LerpUnclamped(ref FVector3 from, ref FVector3 to, FLOAT percent, out FVector3 result) {
            var x = FMath.Lerp(from.x, to.x, percent);
            var y = FMath.Lerp(from.y, to.y, percent);
            var z = FMath.Lerp(from.z, to.z, percent);

            result.x = x;
            result.y = y;
            result.z = z;
        }

        /// <summary>
        /// 平滑过渡
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static FVector3 SmoothStep(FVector3 v1, FVector3 v2, FLOAT amount) {
            return new FVector3(FMath.SmoothStep(v1.x, v2.x, amount), FMath.SmoothStep(v1.y, v2.y, amount),
                FMath.SmoothStep(v1.z, v2.z, amount));
        }

        /// <summary>
        /// 平滑过渡
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="amount"></param>
        /// <param name="result"></param>
        public static void SmoothStep(ref FVector3 v1, ref FVector3 v2, FLOAT amount, out FVector3 result) {
            var x = FMath.SmoothStep(v1.x, v2.x, amount);
            var y = FMath.SmoothStep(v1.y, v2.y, amount);
            var z = FMath.SmoothStep(v1.z, v2.z, amount);

            result.x = x;
            result.y = y;
            result.z = z;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static FVector3 Scale(FVector3 vector, FVector3 other) {
            var result = new FVector3 { x = vector.x * other.x, y = vector.y * other.y, z = vector.z * other.z };
            return result;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public static void Scale(ref FVector3 vector, ref FVector3 other, out FVector3 result) {
            var x = vector.x * other.x;
            var y = vector.y * other.y;
            var z = vector.z * other.z;

            result.x = x;
            result.y = y;
            result.z = z;
        }

        public static FVector3 Min(FVector3 v1, FVector3 v2) {
            var result = new FVector3 { x = (v1.x < v2.x) ? v1.x : v2.x, y = (v1.y < v2.y) ? v1.y : v2.y, z = (v1.z < v2.z) ? v1.z : v2.z };
            return result;
        }

        public static void Min(ref FVector3 v1, ref FVector3 v2, out FVector3 result) {
            result = new FVector3 { x = (v1.x < v2.x) ? v1.x : v2.x, y = (v1.y < v2.y) ? v1.y : v2.y, z = (v1.z < v2.z) ? v1.z : v2.z };
        }

        public static FVector3 Max(FVector3 v1, FVector3 v2) {
            var result = new FVector3 { x = (v1.x > v2.x) ? v1.x : v2.x, y = (v1.y > v2.y) ? v1.y : v2.y, z = (v1.z > v2.z) ? v1.z : v2.z };
            return result;
        }

        public static void Max(ref FVector3 v1, ref FVector3 v2, out FVector3 result) {
            result = new FVector3 { x = (v1.x > v2.x) ? v1.x : v2.x, y = (v1.y > v2.y) ? v1.y : v2.y, z = (v1.z > v2.z) ? v1.z : v2.z };
        }

        public static FLOAT Distance(FVector3 v1, FVector3 v2) {
            return FMath.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) +
                              (v1.z - v2.z) * (v1.z - v2.z));
        }

        public static void Distance(ref FVector3 v1, ref FVector3 v2, out FLOAT result) {
            result = FMath.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) +
                                (v1.z - v2.z) * (v1.z - v2.z));
        }

        /// <summary>
        /// 距离的平方值
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static FLOAT DistanceSquared(FVector3 v1, FVector3 v2) {
            return (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z);
        }

        /// <summary>
        /// 距离的平方值
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="result"></param>
        public static void DistanceSquared(ref FVector3 v1, ref FVector3 v2, out FLOAT result) {
            result = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z);
        }

        /// <summary>
        /// 点积
        /// <para>点积只能求得0-180度的夹角（判断方向前后），在都是单位向量的前提下，[方向相同为前，点积为1][方向相反为后，点积为-1][相互垂直，点积为0]</para>
        /// <para>几何意义：[两向量夹角的余弦值] * [两个向量的模长]的乘积。例子：两个标准向量夹角为45度时，点积为0.707，cos45°就是0.707，这个0.707也正是投影的长度。</para>
        /// <para>公式：a·b=|a||b|cos(θ); 从公式可以看出当两个向量为标准向量时，点积结果为两向量夹角的余弦值。</para>
        /// <para>核心理解：直角三角形，斜边为a，夹角的余弦值为a·b/|a||b|，临边就是余弦值*斜边</para>
        /// <para>公式推导：|a|cos(θ)=a·b/|b|，而|a|cos(θ)就是斜边 * 余弦 = 临边。所以：已知a b两条向量，求a对于b的投影的长度，就用ab的点积 / b的模长</para>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>一条向量在另一条向量上投影的长度再乘以另一条向量的长度</returns>
        public static FLOAT Dot(FVector3 v1, FVector3 v2) {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        /// <summary>
        /// 点积
        /// <para>点积只能求得0-180度的夹角（判断方向前后），在都是单位向量的前提下，[方向相同为前，点积为1][方向相反为后，点积为-1][相互垂直，点积为0]</para>
        /// <para>几何意义：[两向量夹角的余弦值] * [两个向量的模长]的乘积。例子：两个标准向量夹角为45度时，点积为0.707，cos45°就是0.707，这个0.707也正是投影的长度。</para>
        /// <para>公式：a·b=|a||b|cos(θ); 从公式可以看出当两个向量为标准向量时，点积结果为两向量夹角的余弦值。</para>
        /// <para>核心理解：直角三角形，斜边为a，夹角的余弦值为a·b/|a||b|，临边就是余弦值*斜边</para>
        /// <para>公式推导：|a|cos(θ)=a·b/|b|，而|a|cos(θ)就是斜边 * 余弦 = 临边。所以：已知a b两条向量，求a对于b的投影的长度，就用ab的点积 / b的模长</para>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="result">一条向量在另一条向量上投影的长度再乘以另一条向量的长度</param>
        public static void Dot(ref FVector3 v1, ref FVector3 v2, out FLOAT result) {
            result = v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        /// <summary>
        /// 点积
        /// <para>点积只能求得0-180度的夹角（判断方向前后），在都是单位向量的前提下，[方向相同为前，点积为1][方向相反为后，点积为-1][相互垂直，点积为0]</para>
        /// <para>几何意义：[两向量夹角的余弦值] * [两个向量的模长]的乘积。例子：两个标准向量夹角为45度时，点积为0.707，cos45°就是0.707，这个0.707也正是投影的长度。</para>
        /// <para>公式：a·b=|a||b|cos(θ); 从公式可以看出当两个向量为标准向量时，点积结果为两向量夹角的余弦值。</para>
        /// <para>核心理解：直角三角形，斜边为a，夹角的余弦值为a·b/|a||b|，临边就是余弦值*斜边</para>
        /// <para>公式推导：|a|cos(θ)=a·b/|b|。已知a b两条向量，要求a对于b的投影的长度，就用ab的点积/b的模长</para>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>一条向量在另一条向量上投影的长度再乘以另一条向量的长度</returns>
        public static FLOAT Dot(ref FVector3 v1, ref FVector3 v2) {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        /// <summary>
        /// 叉积
        /// <para>叉积只能求-90 ~ 90度的夹角（判断方向左右）。v1在v2左边时，向量指向后方，v1在v2右边时，向量指向前方，平行时，叉积均为Zero</para>
        /// <para>几何意义：两个向量组成平面的法向量，且该法向量的模长等于两个向量构成的平行四边形的面积。原理可根据下面公式推导</para>
        /// <para>公式：a×b=|a||b|sin(θ) n; 其中n表示ab构成的平面的法线的单位向量，可以看出，同样对于标准向量，dot的cos是临边，
        /// cross的sin是对边，也就是上面说的平行四边形的高</para>
        /// <para>公式推导：|a|sin(θ) * |b|，其中|b|为底长，|a|sin(θ)为高，相乘结果就是平行四边形的面积</para>
        /// <para>其中上面关于前后方问题，貌似和右手法则相反，其实不是，而是因为unity使用的是左手坐标系，要采用左手法则</para>>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>结果为两个向量所组成的面的垂直向量。以b为底边，ab夹角，则模长=底边*对边</returns>
        public static FVector3 Cross(FVector3 v1, FVector3 v2) {
            var result = new FVector3();
            var x = (v1.y * v2.z) - (v1.z * v2.y);
            var y = (v1.z * v2.x) - (v1.x * v2.z);
            var z = (v1.x * v2.y) - (v1.y * v2.x);

            result.x = x;
            result.y = y;
            result.z = z;
            return result;
        }

        /// <summary>
        /// 叉积
        /// <para>叉积只能求-90 ~ 90度的夹角（判断方向左右）。v1在v2左边时，向量指向后方，v1在v2右边时，向量指向前方，平行时，叉积均为Zero</para>
        /// <para>几何意义：两个向量组成平面的法向量，且该法向量的模长等于两个向量构成的平行四边形的面积。原理可根据下面公式推导</para>
        /// <para>公式：a×b=|a||b|sin(θ) n; 其中n表示ab构成的平面的法线的单位向量，可以看出，同样对于标准向量，dot的cos是临边，
        /// cross的sin是对边，也就是上面说的平行四边形的高</para>
        /// <para>公式推导：|a|sin(θ) * |b|，其中|b|为底长，|a|sin(θ)为高，相乘结果就是平行四边形的面积</para>
        /// <para>其中上面关于前后方问题，貌似和右手法则相反，其实不是，而是因为unity使用的是左手坐标系，要采用左手法则</para>>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="result">结果为两个向量所组成的面的垂直向量。以b为底边，ab夹角，则模长=底边*对边</param>
        public static void Cross(ref FVector3 v1, ref FVector3 v2, out FVector3 result) {
            var x = (v1.y * v2.z) - (v1.z * v2.y);
            var y = (v1.z * v2.x) - (v1.x * v2.z);
            var z = (v1.x * v2.y) - (v1.y * v2.x);

            result.x = x;
            result.y = y;
            result.z = z;
        }

        /// <summary>
        /// 叉积
        /// <para>叉积只能求-90 ~ 90度的夹角（判断方向左右）。时针为v1，分针为v2，分针在时针逆时针方向时，
        /// 叉积指向钟表里面，顺时针时，叉积指向钟表外面，平行时，叉积为0</para>
        /// <para>几何意义：两个向量组成平面的法向量，且该法向量的模长等于两个向量构成的平行四边形的面积。原理可根据下面公式推导</para>
        /// <para>公式：a×b=|a||b|sin(θ) n; 其中n表示ab构成的平面的法线的单位向量，可以看出，同样对于标准向量，dot的cos是临边，
        /// cross的sin是对边，也就是上面说的平行四边形的高</para>
        /// <para>公式推导：|a|sin(θ) * |b|，其中|b|为底长，|a|sin(θ)为高，相乘结果就是平行四边形的面积</para>
        /// <para>其中上面关于前后方问题，貌似和右手法则相反，其实不是，而是因为unity使用的是左手坐标系，要采用左手法则</para>>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>结果为两个向量所组成的面的垂直向量。以b为底边，ab夹角，则模长=底边*对边</returns>
        public static FVector3 Cross(ref FVector3 v1, ref FVector3 v2) {
            FVector3 result;
            var x = (v1.y * v2.z) - (v1.z * v2.y);
            var y = (v1.z * v2.x) - (v1.x * v2.z);
            var z = (v1.x * v2.y) - (v1.y * v2.x);

            result.x = x;
            result.y = y;
            result.z = z;
            return result;
        }

        /// <summary>
        /// 三重积（三重叉积）
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void TripleProduct(ref FVector3 v1, ref FVector3 v2, ref FVector3 v3,
            out FVector3 result) {
            Cross(ref v1, ref v2, out var cross);
            Cross(ref cross, ref v3, out result);
        }

        /// <summary>
        /// 投影
        /// <para>一条向量在另一条向量上的投影向量</para>>
        /// <para></para>>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static FVector3 Project(FVector3 vector, FVector3 normal) {
            var sqrMag = normal.sqrMagnitude;
            if (FMath.CompareApproximateZero(sqrMag)) {
                return Zero;
            }

            // 正常公式 =
            // dot / normal.mag = 投影长度
            // 投影长度 * normal.normalize = 最终投影的向量

            // 又由于 normal.normalize = normal / normal.mag;
            // 带入上面算式 =
            // dot / normal.mag = 投影长度
            // 投影长度 * normal / normal.mag = 最终投影的向量
            // 优化算式后 =
            // dot / normal.mag / normal.mag = 投影长度
            // 投影长度 * normal = 最终投影的向量
            // = 
            // dot / normal.sqrMag = 投影长度
            // 投影长度 * normal = 最终投影的向量

            Dot(ref vector, ref normal, out var dot);
            var div = dot / sqrMag;
            Multiply(ref normal, div, out var result);
            return result;
        }

        /// <summary>
        /// 投影
        /// <para>一条向量在另一条向量上的投影向量</para>>
        /// <para></para>>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void Project(ref FVector3 vector, ref FVector3 normal, out FVector3 result) {
            var sqrMag = normal.sqrMagnitude;
            if (FMath.CompareApproximateZero(sqrMag)) {
                result = Zero;
                return;
            }

            Dot(ref vector, ref normal, out var dot);
            var div = dot / sqrMag;
            Multiply(ref normal, div, out result);
        }

        /// <summary>
        /// 投影平面向量
        /// <para>以向量A - A在B向量上的投影点，得到一条由投影点指向A点的向量，这条向量垂直于B向量</para>>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="planeNormal"></param>
        /// <returns></returns>
        public static FVector3 ProjectOnPlane(FVector3 vector, FVector3 planeNormal) {
            var sqrMag = planeNormal.sqrMagnitude;
            if (FMath.CompareApproximateZero(sqrMag)) {
                return vector;
            }

            Dot(ref vector, ref planeNormal, out var dot);
            var div = dot / sqrMag;
            Multiply(ref planeNormal, div, out var project);
            Subtract(ref vector, ref project, out var result);
            return result;
        }

        /// <summary>
        /// 投影平面向量
        /// <para>以向量A - A在B向量上的投影点，得到一条由投影点指向A点的向量，这条向量垂直于B向量</para>>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="planeNormal"></param>
        /// <param name="result"></param>
        public static void ProjectOnPlane(ref FVector3 vector, ref FVector3 planeNormal, out FVector3 result) {
            var sqrMag = planeNormal.sqrMagnitude;
            if (FMath.CompareApproximateZero(sqrMag)) {
                result = vector;
                return;
            }

            Dot(ref vector, ref planeNormal, out var dot);
            var div = dot / sqrMag;
            Multiply(ref planeNormal, div, out var project);
            Subtract(ref vector, ref project, out result);
        }

        /// <summary>
        /// 两个向量的夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>返回一个角度</returns>
        public static FLOAT Angle(FVector3 from, FVector3 to) {
            FLOAT num = FMath.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (FMath.CompareApproximateZero(num)) {
                return 0;
            }

            Dot(ref from, ref to, out var dot);
            dot = FMath.Clamp(dot / num, -1, 1);
            var rad = FMath.Acos(dot);
            return FMath.RadToDeg(rad);
        }

        /// <summary>
        /// 两个向量的夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="result">返回一个角度</param>
        public static void Angle(ref FVector3 from, ref FVector3 to, out FLOAT result) {
            FLOAT num = FMath.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (FMath.CompareApproximateZero(num)) {
                result = 0;
                return;
            }

            Dot(ref from, ref to, out var dot);
            dot = FMath.Clamp(dot / num, -1, 1);
            var rad = FMath.Acos(dot);
            result = FMath.RadToDeg(rad);
        }

        /// <summary>
        /// 返回带符号的向量夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="axis">一般我们习惯from在to左边为负，右边为正，所以，axis指向后方就行；如果要反着来，就axis指向前方</param>
        /// <returns></returns>
        public static FLOAT SignedAngle(FVector3 from, FVector3 to, FVector3 axis) {
            Angle(ref from, ref to, out var unsignedAngle);
            Cross(ref from, ref to, out var cross);
            Dot(ref axis, ref cross, out var dot);
            var sign = FMath.Sign(dot);

            return unsignedAngle * sign;
        }

        /// <summary>
        /// 返回带符号的向量夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="axis">一般我们习惯from在to左边为负，右边为正，所以，axis指向后方就行；如果要反着来，就axis指向前方</param>
        /// <param name="result"></param>
        public static void SignedAngle(ref FVector3 from, ref FVector3 to, ref FVector3 axis, out FLOAT result) {
            Angle(ref from, ref to, out var unsignedAngle);
            Cross(ref from, ref to, out var cross);
            Dot(ref axis, ref cross, out var dot);
            var sign = FMath.Sign(dot);

            result = unsignedAngle * sign;
        }

        /// <summary>
        /// 将一个向量反射出由法线定义的平面
        /// </summary>
        /// <param name="inDirection"></param>
        /// <param name="inNormal"></param>
        /// <returns></returns>
        public static FVector3 Reflect(FVector3 inDirection, FVector3 inNormal) {
            FLOAT num = -2f * Dot(inNormal, inDirection);
            return new FVector3(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y, num * inNormal.z + inDirection.z);
        }

        /// <summary>
        /// 计算当前和目标指定的点之间的位置，移动不超过maxDistanceDelta指定的距离
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDistanceDelta">每次调用时移动的距离</param>
        /// <returns></returns>
        public static FVector3 MoveTowards(FVector3 current, FVector3 target, FLOAT maxDistanceDelta) {
            var num1 = target.x - current.x;
            var num2 = target.y - current.y;
            var num3 = target.z - current.z;
            var d = num1 * num1 + num2 * num2 + num3 * num3;
            if (d == 0 || maxDistanceDelta >= 0 && d <= maxDistanceDelta * maxDistanceDelta) {
                return target;
            }

            var num4 = FMath.Sqrt(d);

            return new FVector3(current.x + num1 / num4 * maxDistanceDelta, current.y + num2 / num4 * maxDistanceDelta,
                current.z + num3 / num4 * maxDistanceDelta);
        }

        public static void Normalize(ref FVector3 value, out FVector3 result) {
            var mag = value.magnitude;
            if (FMath.CompareApproximateZero(mag)) {
                result = Zero;
            }

            var num = 1 / mag;
            result.x = num * value.x;
            result.y = num * value.y;
            result.z = num * value.z;
        }

        public static FLOAT Normalize(ref FVector3 value) {
            var mag = value.magnitude;
            if (FMath.CompareApproximateZero(mag)) {
                return 0;
            }

            var num = 1 / mag;
            value.x = num * value.x;
            value.y = num * value.y;
            value.z = num * value.z;

            return mag;
        }

        /// <summary>
        /// 设置向量长度
        /// </summary>
        /// <param name="vector">向量</param>
        /// <param name="lengthValue">要设置的向量的长度</param>
        /// <param name="epsilon">当向量的模长小于该值时，把向量当做0向量处理</param>
        /// <returns></returns>
        public static FLOAT SetLength(ref FVector3 vector, FLOAT lengthValue, FLOAT epsilon) {
            FLOAT mag = FMath.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
            if (mag >= epsilon) {
                FLOAT num = lengthValue / mag;
                vector.x *= num;
                vector.y *= num;
                vector.z *= num;
                return lengthValue;
            }

            vector.x = 0f;
            vector.y = 0f;
            vector.z = 0f;
            return 0f;
        }

        /// <summary>
        /// 增加向量长度
        /// </summary>
        /// <param name="vector">向量</param>
        /// <param name="lengthDelta">增加多少长度</param>
        /// <param name="epsilon">当向量的模长小于该值时，把向量当做0向量处理</param>
        /// <returns></returns>
        public static FLOAT GrowLength(ref FVector3 vector, FLOAT lengthDelta, FLOAT epsilon) {
            FLOAT mag = FMath.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
            if (mag >= epsilon) {
                FLOAT num = mag + lengthDelta;
                FLOAT num2 = num / mag;
                vector.x *= num2;
                vector.y *= num2;
                vector.z *= num2;
                return num;
            }

            vector.x = 0f;
            vector.y = 0f;
            vector.z = 0f;
            return 0f;
        }

        /// <summary>
        /// 交换两个向量
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public static void Swap(ref FVector3 v1, ref FVector3 v2) {
            var temp = v1.x;
            v1.x = v2.x;
            v2.x = temp;

            temp = v1.y;
            v1.y = v2.y;
            v2.y = temp;

            temp = v1.z;
            v1.z = v2.z;
            v2.z = temp;
        }

        /// <summary>
        /// 拷贝
        /// </summary>
        /// <param name="copier">拷贝者</param>
        /// <param name="target">拷贝到目标</param>
        public static void Copy(ref FVector3 copier, out FVector3 target) {
            target.x = copier.x;
            target.y = copier.y;
            target.z = copier.z;
        }

        /// <summary>
        /// 取反拷贝
        /// </summary>
        /// <param name="copier">拷贝者</param>
        /// <param name="target">拷贝到目标</param>
        public static void NegateCopy(ref FVector3 copier, out FVector3 target) {
            target.x = -copier.x;
            target.y = -copier.y;
            target.z = -copier.z;
        }

        public FVector3(FLOAT xyz) {
            this.x = xyz;
            this.y = xyz;
            this.z = xyz;
        }

        public FVector3(FLOAT x, FLOAT y, FLOAT z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// 是否为0（使用了Epsilon）
        /// </summary>
        /// <returns></returns>
        public bool IsZero() {
            return FMath.CompareApproximateZero(this.sqrMagnitude);
        }

        public void Normalize() {
            /*
             * 向量 / 向量长度 = 单位向量
             *
             * 对于趋近与 0的向量，normalize后，会自动规整为 0
             */
            FLOAT mag = this.magnitude;
            FLOAT num = 0;
            if (!FMath.CompareApproximateZero(mag)) {
                num = 1 / mag;
            }

            this.x *= num;
            this.y *= num;
            this.z *= num;
        }

        public void Scale(FVector3 other) {
            this.x *= other.x;
            this.y *= other.y;
            this.z *= other.z;
        }

        public void Set(FLOAT x, FLOAT y, FLOAT z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Negate() {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
        }

        public FLOAT Dot(FVector3 value) {
            return (this.x * value.x) + (this.y * value.y) + (this.z * value.z);
        }

        public FVector3 Cross(FVector3 value) {
            var result = new FVector3();
            var num1 = (this.y * value.z) - (this.z * value.y);
            var num2 = (this.z * value.x) - (this.x * value.z);
            var num3 = (this.x * value.y) - (this.y * value.x);
            result.x = num1;
            result.y = num2;
            result.z = num3;
            return result;
        }

        /// <summary>
        /// 转成FVector2
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public FVector2 ToFVector2() {
            return new FVector2(this.x, this.y);
        }

        /// <summary>
        /// 转成FVector2
        /// </summary>
        /// <param name="projectionDir">投影方向，传入right up forward分别代表，向x轴投影、向y轴投影、向z轴投影。默认向z轴投影</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public FVector2 ToFVector2(FVector3 projectionDir) {
            if (projectionDir.z != 0) {
                return new FVector2(this.x, this.y);
            }
            else if (projectionDir.x != 0) {
                return new FVector2(this.z, this.y);
            }
            else if (projectionDir.y != 0) {
                return new FVector2(this.x, this.z);
            }
            else {
                throw new Exception($"axis is invalid");
            }
        }

        public FVector4 ToFVector4() {
            return new FVector4(this.x, this.y, this.z, 0);
        }

        /// <summary>
        /// 获得一个向量的大概向量
        /// <para>哪个轴最长，则返回以该轴为方向。当出现相等情况时，有限z轴，然后是y轴</para>>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FVector3 ToRound(FVector3 value) {
            var absX = FMath.Abs(value.x);
            var absY = FMath.Abs(value.y);
            var absZ = FMath.Abs(value.z);
            FLOAT resultX = 0, resultY = 0, resultZ = 0;

            if (absX > absY) {
                if (absZ > absX) {
                    resultZ = 1;
                }
                else if (absZ < absX) {
                    resultX = 1;
                }
                else {
                    // (z=x)>y，默认返回 forward
                    resultZ = 1;
                }
            }
            else if (absX < absY) {
                if (absZ > absY) {
                    resultZ = 1;
                }
                else if (absZ < absY) {
                    resultY = 1;
                }
                else {
                    //（z=y）>x，默认返回 forward
                    resultZ = 1;
                }
            }
            else {
                if (absZ > absY) {
                    resultZ = 1;
                }
                else if (absZ < absY) {
                    //（x=y）>z，默认返回 up
                    resultY = 1;
                }
                else {
                    // xyz全部相等，默认返回 forward
                    resultZ = 1;
                }
            }

            return new FVector3(resultX, resultY, resultZ);
        }

        public bool Equals(FVector3 other) {
            return
                FMath.CompareApproximate(this.x, other.x) &&
                FMath.CompareApproximate(this.y, other.y) &&
                FMath.CompareApproximate(this.z, other.z);
        }

        public override bool Equals(object obj) {
            if (!(obj is FVector3 other)) {
                return false;
            }

            return FMath.CompareApproximate(this.x, other.x) &&
                   FMath.CompareApproximate(this.y, other.y) &&
                   FMath.CompareApproximate(this.z, other.z);
        }

#if FIXED_MATH
        public override string ToString() { return $"({x.AsFloat():f1}, {y.AsFloat():f1}, {z.AsFloat():f1})"; }
#else
        public override string ToString() {
            return $"({this.x:f1}, {this.y:f1}, {this.z:f1})";
        }
#endif

        public override int GetHashCode() {
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
        }

        public static void Add(ref FVector3 value1, ref FVector3 value2, out FVector3 result) {
            var x = value1.x + value2.x;
            var y = value1.y + value2.y;
            var z = value1.z + value2.z;

            result.x = x;
            result.y = y;
            result.z = z;
        }

        public static void Subtract(ref FVector3 value1, ref FVector3 value2, out FVector3 result) {
            var x = value1.x - value2.x;
            var y = value1.y - value2.y;
            var z = value1.z - value2.z;

            result.x = x;
            result.y = y;
            result.z = z;
        }

        public static void Multiply(ref FVector3 value1, FLOAT scaleFactor, out FVector3 result) {
            var x = value1.x * scaleFactor;
            var y = value1.y * scaleFactor;
            var z = value1.z * scaleFactor;

            result.x = x;
            result.y = y;
            result.z = z;
        }

        public static void Divide(ref FVector3 value1, FLOAT scaleFactor, out FVector3 result) {
            var x = value1.x / scaleFactor;
            var y = value1.y / scaleFactor;
            var z = value1.z / scaleFactor;

            result.x = x;
            result.y = y;
            result.z = z;
        }

        public static void Negate(ref FVector3 value, out FVector3 result) {
            var x = -value.x;
            var y = -value.y;
            var z = -value.z;

            result.x = x;
            result.y = y;
            result.z = z;
        }

        public static bool operator ==(FVector3 v1, FVector3 v2) {
            return FMath.CompareApproximate(v1.x, v2.x) &&
                   FMath.CompareApproximate(v1.y, v2.y) &&
                   FMath.CompareApproximate(v1.z, v2.z);
        }

        public static bool operator !=(FVector3 v1, FVector3 v2) {
            return !FMath.CompareApproximate(v1.x, v2.x) ||
                   !FMath.CompareApproximate(v1.y, v2.y) ||
                   !FMath.CompareApproximate(v1.z, v2.z);
        }

        public static FVector3 operator +(FVector3 v1, FVector3 v2) {
            var result = new FVector3();
            var x = v1.x + v2.x;
            var y = v1.y + v2.y;
            var z = v1.z + v2.z;

            result.x = x;
            result.y = y;
            result.z = z;

            return result;
        }

        public static FVector3 operator -(FVector3 v1, FVector3 v2) {
            var result = new FVector3();
            var x = v1.x - v2.x;
            var y = v1.y - v2.y;
            var z = v1.z - v2.z;

            result.x = x;
            result.y = y;
            result.z = z;

            return result;
        }

        public static FVector3 operator -(FVector3 v1) {
            var result = new FVector3 { x = -v1.x, y = -v1.y, z = -v1.z };
            return result;
        }

        public static FVector3 operator *(FVector3 v1, FLOAT v2) {
            var result = new FVector3 { x = v1.x * v2, y = v1.y * v2, z = v1.z * v2 };
            return result;
        }

        public static FVector3 operator *(FLOAT v1, FVector3 v2) {
            var result = new FVector3 { x = v2.x * v1, y = v2.y * v1, z = v2.z * v1 };
            return result;
        }

        public static FVector3 operator /(FVector3 v1, FVector3 v2) {
            var result = new FVector3 { x = v1.x / v2.x, y = v1.y / v2.y, z = v1.z / v2.z };
            return result;
        }

        public static FVector3 operator /(FVector3 v1, FLOAT v2) {
            var result = new FVector3 { x = v1.x / v2, y = v1.y / v2, z = v1.z / v2 };
            return result;
        }

        public static FVector3 operator %(FVector3 v1, FVector3 v2) {
            return Cross(v1, v2);
        }
#if UNITY
        public static implicit operator UnityEngine.Vector3(FVector3 v)
        {
            return new UnityEngine.Vector3(v.x, v.y, v.z);
        }

        public static implicit operator FVector3(UnityEngine.Vector3 v)
        {
            return new FVector3(v.x, v.y, v.z);
        }
#endif
    }
}