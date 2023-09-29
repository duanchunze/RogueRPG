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
    public struct FVector2 : IEquatable<FVector2> {
        public static readonly FVector2 Zero;
        public static readonly FVector2 One = new FVector2(1);
        public static readonly FVector2 Left = new FVector2(-1, 0);
        public static readonly FVector2 Right = new FVector2(1, 0);
        public static readonly FVector2 Up = new FVector2(0, 1);
        public static readonly FVector2 Down = new FVector2(0, -1);

        public FLOAT x, y;

        public FLOAT magnitude {
            get {
                var sqrMag = (this.x * this.x) + (this.y * this.y);
                return FMath.Sqrt(sqrMag);
            }
        }

        public FLOAT sqrMagnitude {
            get {
                var sqrMag = (this.x * this.x) + (this.y * this.y);
                return sqrMag;
            }
        }

        public FVector2 normalized {
            get {
                var result = new FVector2(this.x, this.y);
                result.Normalize();

                return result;
            }
        }

        public FLOAT this[int index] {
            get {
                return index switch {
                    0 => this.x,
                    1 => this.y,
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
                    default:
                        throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }

        public static FVector2 ClampMagnitude(FVector2 vector, FLOAT min, FLOAT max) {
            var result = vector;
            var magnitude = vector.magnitude;
            var val = FMath.Clamp(magnitude, min, max);
            if (!FMath.CompareApproximate(val, magnitude)) {
                var num = val / magnitude;
                result.x *= num;
                result.y *= num;
            }

            return result;
        }

        public static void ClampMagnitude(ref FVector2 vector, FLOAT min, FLOAT max) {
            var magnitude = vector.magnitude;
            var val = FMath.Clamp(magnitude, min, max);
            if (!FMath.CompareApproximate(val, magnitude)) {
                var num = val / magnitude;
                vector.x *= num;
                vector.y *= num;
            }
        }

        public static FVector2 Lerp(FVector2 v1, FVector2 v2, FLOAT percent) {
            return new FVector2(FMath.Lerp(v1.x, v2.x, percent), FMath.Lerp(v1.y, v2.y, percent));
        }

        public static void Lerp(ref FVector2 v1, ref FVector2 v2, FLOAT percent, out FVector2 result) {
            result = new FVector2(FMath.Lerp(v1.x, v2.x, percent), FMath.Lerp(v1.y, v2.y, percent));
        }

        public static FVector2 LerpUnclamped(FVector2 v1, FVector2 v2, FLOAT percent) {
            return new FVector2(FMath.Lerp(v1.x, v2.x, percent), FMath.Lerp(v1.y, v2.y, percent));
        }

        public static void
            LerpUnclamped(ref FVector2 v1, ref FVector2 v2, FLOAT percent, out FVector2 result) {
            result = new FVector2(FMath.Lerp(v1.x, v2.x, percent), FMath.Lerp(v1.y, v2.y, percent));
        }

        public static FVector2 Barycentric(FVector2 v1, FVector2 v2, FVector2 v3, FLOAT amount1, FLOAT amount2) {
            return new FVector2(FMath.Barycentric(v1.x, v2.x, v3.x, amount1, amount2),
                FMath.Barycentric(v1.y, v2.y, v3.y, amount1, amount2));
        }

        /// <summary>
        /// 重心
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="amount1"></param>
        /// <param name="amount2"></param>
        /// <param name="result"></param>
        public static void Barycentric(ref FVector2 v1, ref FVector2 v2, ref FVector2 v3, FLOAT amount1,
            FLOAT amount2, out FVector2 result) {
            result = new FVector2(FMath.Barycentric(v1.x, v2.x, v3.x, amount1, amount2),
                FMath.Barycentric(v1.y, v2.y, v3.y, amount1, amount2));
        }

        /// <summary>
        /// Catmull-Rom曲线
        /// </summary>
        /// <returns></returns>
        public static FVector2 CatmullRom(FVector2 v1, FVector2 v2, FVector2 v3, FVector2 v4, FLOAT amount) {
            return new FVector2(FMath.CatmullRom(v1.x, v2.x, v3.x, v4.x, amount),
                FMath.CatmullRom(v1.y, v2.y, v3.y, v4.y, amount));
        }

        public static void CatmullRom(ref FVector2 v1, ref FVector2 v2, ref FVector2 v3, ref FVector2 v4,
            FLOAT amount, out FVector2 result) {
            result = new FVector2(FMath.CatmullRom(v1.x, v2.x, v3.x, v4.x, amount),
                FMath.CatmullRom(v1.y, v2.y, v3.y, v4.y, amount));
        }

        public static FVector2 Hermite(FVector2 v1, FVector2 tangent1, FVector2 v2, FVector2 tangent2,
            FLOAT amount) {
            var result = new FVector2 {
                x = FMath.Hermite(v1.x, tangent1.x, v2.x, tangent2.x, amount), y = FMath.Hermite(v1.y, tangent1.y, v2.y, tangent2.y, amount)
            };
            return result;
        }

        public static void Hermite(ref FVector2 v1, ref FVector2 tangent1, ref FVector2 v2,
            ref FVector2 tangent2, FLOAT amount, out FVector2 result) {
            result = new FVector2 {
                x = FMath.Hermite(v1.x, tangent1.x, v2.x, tangent2.x, amount), y = FMath.Hermite(v1.y, tangent1.y, v2.y, tangent2.y, amount)
            };
        }

        public static FVector2 SmoothStep(FVector2 v1, FVector2 v2, FLOAT amount) {
            return new FVector2(FMath.SmoothStep(v1.x, v2.x, amount), FMath.SmoothStep(v1.y, v2.y, amount));
        }

        public static void SmoothStep(ref FVector2 v1, ref FVector2 v2, FLOAT amount, out FVector2 result) {
            result = new FVector2(FMath.SmoothStep(v1.x, v2.x, amount), FMath.SmoothStep(v1.y, v2.y, amount));
        }

        public static FVector2 Max(FVector2 v1, FVector2 v2) {
            return new FVector2(FMath.Max(v1.x, v2.x), FMath.Max(v1.y, v2.y));
        }

        public static void Max(ref FVector2 v1, ref FVector2 v2, out FVector2 result) {
            result = new FVector2(FMath.Max(v1.x, v2.x), FMath.Max(v1.y, v2.y));
        }

        public static FVector2 Min(FVector2 v1, FVector2 v2) {
            return new FVector2(FMath.Min(v1.x, v2.x), FMath.Min(v1.y, v2.y));
        }

        public static void Min(ref FVector2 v1, ref FVector2 v2, out FVector2 result) {
            result = new FVector2(FMath.Min(v1.x, v2.x), FMath.Min(v1.y, v2.y));
        }

        public static FVector2 Reflect(FVector2 v, FVector2 normal) {
            Dot(ref v, ref normal, out var dot);
            var result = new FVector2 { x = v.x - ((2f * dot) * normal.x), y = v.y - ((2f * dot) * normal.y) };
            return result;
        }

        public static void Reflect(ref FVector2 v, ref FVector2 normal, out FVector2 result) {
            Dot(ref v, ref normal, out var dot);
            result = new FVector2 { x = v.x - ((2f * dot) * normal.x), y = v.y - ((2f * dot) * normal.y) };
        }

        public static FVector2 Clamp(FVector2 v, FVector2 min, FVector2 max) {
            return new FVector2(FMath.Clamp(v.x, min.x, max.x), FMath.Clamp(v.y, min.y, max.y));
        }

        public static void Clamp(ref FVector2 v, ref FVector2 min, ref FVector2 max, out FVector2 result) {
            result = new FVector2(FMath.Clamp(v.x, min.x, max.x), FMath.Clamp(v.y, min.y, max.y));
        }

        public static FLOAT Distance(FVector2 v1, FVector2 v2) {
            var num = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
            return FMath.Sqrt(num);
        }

        public static void Distance(ref FVector2 v1, ref FVector2 v2, out FLOAT result) {
            var num = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
            result = FMath.Sqrt(num);
        }

        public static FLOAT DistanceSquared(FVector2 v1, FVector2 v2) {
            var result = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
            return result;
        }

        public static void DistanceSquared(ref FVector2 v1, ref FVector2 v2, out FLOAT result) {
            result = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
        }

        public static FLOAT Dot(FVector2 v1, FVector2 v2) {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static void Dot(ref FVector2 v1, ref FVector2 v2, out FLOAT result) {
            result = v1.x * v2.x + v1.y * v2.y;
        }

        public static FLOAT Angle(FVector2 a, FVector2 b) {
            FLOAT num = FMath.Sqrt(a.sqrMagnitude * b.sqrMagnitude);
            if (FMath.CompareApproximateZero(num)) {
                return 0;
            }

            Dot(ref a, ref b, out var dot);
            dot = FMath.Clamp(dot / num, -1, 1);
            var rad = FMath.Acos(dot);
            return FMath.RadToDeg(rad);
        }

        public static void Angle(ref FVector2 a, ref FVector2 b, out FLOAT result) {
            FLOAT num = FMath.Sqrt(a.sqrMagnitude * b.sqrMagnitude);
            if (FMath.CompareApproximateZero(num)) {
                result = 0;
                return;
            }

            Dot(ref a, ref b, out var dot);
            dot = FMath.Clamp(dot / num, -1, 1);
            var rad = FMath.Acos(dot);
            result = FMath.RadToDeg(rad);
        }

        public static FLOAT Normalize(ref FVector2 value) {
            var mag = value.magnitude;
            if (FMath.CompareApproximateZero(mag)) {
                return 0;
            }

            var num = 1 / mag;
            value.x = num * value.x;
            value.y = num * value.y;

            return mag;
        }

        /// <summary>
        /// 设置长度
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="lengthValue"></param>
        /// <returns></returns>
        public static FLOAT SetLength(ref FVector2 vector, FLOAT lengthValue) {
            FLOAT num = FMath.Sqrt(vector.x * vector.x + vector.y * vector.y);
            if (num >= 0) {
                FLOAT num2 = lengthValue / num;
                vector.x *= num2;
                vector.y *= num2;
                return lengthValue;
            }

            vector.x = 0f;
            vector.y = 0f;
            return 0f;
        }

        /// <summary>
        /// 增加长度
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="lengthDelta"></param>
        /// <returns></returns>
        public static FLOAT GrowLength(ref FVector2 vector, FLOAT lengthDelta) {
            FLOAT num = FMath.Sqrt(vector.x * vector.x + vector.y * vector.y);
            if (num >= 0) {
                FLOAT num2 = num + lengthDelta;
                FLOAT num3 = num2 / num;
                vector.x *= num3;
                vector.y *= num3;
                return num2;
            }

            vector.x = 0f;
            vector.y = 0f;
            return 0f;
        }

        public FVector2(FLOAT xyz) {
            this.x = xyz;
            this.y = xyz;
        }

        public FVector2(FLOAT x, FLOAT y) {
            this.x = x;
            this.y = y;
        }

        public void Set(FLOAT x, FLOAT y) {
            this.x = x;
            this.y = y;
        }

        public void Scale(FVector2 other) {
            this.x = this.x * other.x;
            this.y = this.y * other.y;
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
        }

        public void Negate() {
            this.x = -this.x;
            this.y = -this.y;
        }

        public FLOAT Dot(FVector2 value) {
            return this.x * value.x + this.y * value.y;
        }

        public FVector3 ToFVector3() {
            return new FVector3(this.x, this.y, 0);
        }

        public bool Equals(FVector2 other) {
            return FMath.CompareApproximate(this.x, other.x) && FMath.CompareApproximate(this.y, other.y);
        }

        public override bool Equals(object obj) {
            if (!(obj is FVector2 other)) {
                return false;
            }

            return FMath.CompareApproximate(this.x, other.x) && FMath.CompareApproximate(this.y, other.y);
        }

        public override int GetHashCode() {
            return (int)(this.x + this.y);
        }

#if FIXED_MATH
        public override string ToString() { return $"({x.AsFloat():f1}, {y.AsFloat():f1})"; }
#else
        public override string ToString() {
            return $"({this.x:f1}, {this.y:f1})";
        }
#endif

        public static void Add(ref FVector2 value1, ref FVector2 value2, out FVector2 result) {
            var x = value1.x + value2.x;
            var y = value1.y + value2.y;

            result.x = x;
            result.y = y;
        }

        public static void Subtract(ref FVector2 value1, ref FVector2 value2, out FVector2 result) {
            var x = value1.x - value2.x;
            var y = value1.y - value2.y;

            result.x = x;
            result.y = y;
        }

        public static void Multiply(ref FVector2 value1, FLOAT scaleFactor, out FVector2 result) {
            var x = value1.x * scaleFactor;
            var y = value1.y * scaleFactor;

            result.x = x;
            result.y = y;
        }

        public static void Divide(ref FVector2 value1, FLOAT divider, out FVector2 result) {
            var factor = 1 / divider;
            var x = value1.x * factor;
            var y = value1.y * factor;

            result.x = x;
            result.y = y;
        }

        public static void Negate(ref FVector2 value, out FVector2 result) {
            var x = -value.x;
            var y = -value.y;

            result.x = x;
            result.y = y;
        }

        public static bool operator ==(FVector2 v1, FVector2 v2) {
            return FMath.CompareApproximate(v1.x, v2.x) && FMath.CompareApproximate(v1.y, v2.y);
        }

        public static bool operator !=(FVector2 v1, FVector2 v2) {
            return !FMath.CompareApproximate(v1.x, v2.x) || !FMath.CompareApproximate(v1.y, v2.y);
        }

        public static FVector2 operator +(FVector2 v1, FVector2 v2) {
            v1.x += v2.x;
            v1.y += v2.y;
            return v1;
        }

        public static FVector2 operator -(FVector2 v1, FVector2 v2) {
            v1.x -= v2.x;
            v1.y -= v2.y;
            return v1;
        }

        public static FVector2 operator -(FVector2 v) {
            v.x = -v.x;
            v.y = -v.y;
            return v;
        }

        public static FVector2 operator *(FVector2 v, FLOAT scaleFactor) {
            v.x *= scaleFactor;
            v.y *= scaleFactor;
            return v;
        }

        public static FVector2 operator *(FLOAT scaleFactor, FVector2 v) {
            v.x *= scaleFactor;
            v.y *= scaleFactor;
            return v;
        }

        public static FVector2 operator /(FVector2 v1, FVector2 v2) {
            v1.x /= v2.x;
            v1.y /= v2.y;
            return v1;
        }

        public static FVector2 operator /(FVector2 v1, FLOAT divider) {
            var factor = 1 / divider;
            v1.x *= factor;
            v1.y *= factor;
            return v1;
        }

#if UNITY
        public static implicit operator UnityEngine.Vector2(FVector2 v)
        {
            return new UnityEngine.Vector2(v.x, v.y);
        }

        public static implicit operator FVector2(UnityEngine.Vector2 v)
        {
            return new FVector2(v.x, v.y);
        }
#endif
    }
}