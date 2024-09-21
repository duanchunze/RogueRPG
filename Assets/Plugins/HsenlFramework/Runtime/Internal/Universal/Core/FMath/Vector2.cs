using System;
using System.Runtime.InteropServices;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Vector2 : IEquatable<Vector2> {
        public static readonly Vector2 Zero = default;
        public static readonly Vector2 One = new Vector2(1);
        public static readonly Vector2 Left = new Vector2(-1, 0);
        public static readonly Vector2 Right = new Vector2(1, 0);
        public static readonly Vector2 Up = new Vector2(0, 1);
        public static readonly Vector2 Down = new Vector2(0, -1);

        public FLOAT x;
        public FLOAT y;

        public FLOAT magnitude {
            get {
                var sqrMag = (this.x * this.x) + (this.y * this.y);
                return Math.Sqrt(sqrMag);
            }
        }

        public FLOAT sqrMagnitude {
            get {
                var sqrMag = (this.x * this.x) + (this.y * this.y);
                return sqrMag;
            }
        }

        public Vector2 normalized {
            get {
                var result = new Vector2(this.x, this.y);
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

        public static Vector2 ClampMagnitude(Vector2 vector, FLOAT min, FLOAT max) {
            var result = vector;
            var magnitude = vector.magnitude;
            var val = Math.Clamp(magnitude, min, max);
            if (!Math.CompareApproximate(val, magnitude)) {
                var num = val / magnitude;
                result.x *= num;
                result.y *= num;
            }

            return result;
        }

        public static void ClampMagnitude(ref Vector2 vector, FLOAT min, FLOAT max) {
            var magnitude = vector.magnitude;
            var val = Math.Clamp(magnitude, min, max);
            if (!Math.CompareApproximate(val, magnitude)) {
                var num = val / magnitude;
                vector.x *= num;
                vector.y *= num;
            }
        }

        public static Vector2 Lerp(Vector2 v1, Vector2 v2, FLOAT percent) {
            return new Vector2(Math.Lerp(v1.x, v2.x, percent), Math.Lerp(v1.y, v2.y, percent));
        }

        public static void Lerp(ref Vector2 v1, ref Vector2 v2, FLOAT percent, out Vector2 result) {
            result = new Vector2(Math.Lerp(v1.x, v2.x, percent), Math.Lerp(v1.y, v2.y, percent));
        }

        public static Vector2 LerpUnclamped(Vector2 v1, Vector2 v2, FLOAT percent) {
            return new Vector2(Math.Lerp(v1.x, v2.x, percent), Math.Lerp(v1.y, v2.y, percent));
        }

        public static void
            LerpUnclamped(ref Vector2 v1, ref Vector2 v2, FLOAT percent, out Vector2 result) {
            result = new Vector2(Math.Lerp(v1.x, v2.x, percent), Math.Lerp(v1.y, v2.y, percent));
        }

        public static Vector2 Barycentric(Vector2 v1, Vector2 v2, Vector2 v3, FLOAT amount1, FLOAT amount2) {
            return new Vector2(Math.Barycentric(v1.x, v2.x, v3.x, amount1, amount2),
                Math.Barycentric(v1.y, v2.y, v3.y, amount1, amount2));
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
        public static void Barycentric(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3, FLOAT amount1,
            FLOAT amount2, out Vector2 result) {
            result = new Vector2(Math.Barycentric(v1.x, v2.x, v3.x, amount1, amount2),
                Math.Barycentric(v1.y, v2.y, v3.y, amount1, amount2));
        }

        /// <summary>
        /// Catmull-Rom曲线
        /// </summary>
        /// <returns></returns>
        public static Vector2 CatmullRom(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, FLOAT amount) {
            return new Vector2(Math.CatmullRom(v1.x, v2.x, v3.x, v4.x, amount),
                Math.CatmullRom(v1.y, v2.y, v3.y, v4.y, amount));
        }

        public static void CatmullRom(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3, ref Vector2 v4,
            FLOAT amount, out Vector2 result) {
            result = new Vector2(Math.CatmullRom(v1.x, v2.x, v3.x, v4.x, amount),
                Math.CatmullRom(v1.y, v2.y, v3.y, v4.y, amount));
        }

        public static Vector2 Hermite(Vector2 v1, Vector2 tangent1, Vector2 v2, Vector2 tangent2,
            FLOAT amount) {
            var result = new Vector2 {
                x = Math.Hermite(v1.x, tangent1.x, v2.x, tangent2.x, amount), y = Math.Hermite(v1.y, tangent1.y, v2.y, tangent2.y, amount)
            };
            return result;
        }

        public static void Hermite(ref Vector2 v1, ref Vector2 tangent1, ref Vector2 v2,
            ref Vector2 tangent2, FLOAT amount, out Vector2 result) {
            result = new Vector2 {
                x = Math.Hermite(v1.x, tangent1.x, v2.x, tangent2.x, amount), y = Math.Hermite(v1.y, tangent1.y, v2.y, tangent2.y, amount)
            };
        }

        public static Vector2 SmoothStep(Vector2 v1, Vector2 v2, FLOAT amount) {
            return new Vector2(Math.SmoothStep(v1.x, v2.x, amount), Math.SmoothStep(v1.y, v2.y, amount));
        }

        public static void SmoothStep(ref Vector2 v1, ref Vector2 v2, FLOAT amount, out Vector2 result) {
            result = new Vector2(Math.SmoothStep(v1.x, v2.x, amount), Math.SmoothStep(v1.y, v2.y, amount));
        }

        public static Vector2 Max(Vector2 v1, Vector2 v2) {
            return new Vector2(Math.Max(v1.x, v2.x), Math.Max(v1.y, v2.y));
        }

        public static void Max(ref Vector2 v1, ref Vector2 v2, out Vector2 result) {
            result = new Vector2(Math.Max(v1.x, v2.x), Math.Max(v1.y, v2.y));
        }

        public static Vector2 Min(Vector2 v1, Vector2 v2) {
            return new Vector2(Math.Min(v1.x, v2.x), Math.Min(v1.y, v2.y));
        }

        public static void Min(ref Vector2 v1, ref Vector2 v2, out Vector2 result) {
            result = new Vector2(Math.Min(v1.x, v2.x), Math.Min(v1.y, v2.y));
        }

        public static Vector2 Reflect(Vector2 v, Vector2 normal) {
            Dot(ref v, ref normal, out var dot);
            var result = new Vector2 { x = v.x - ((2f * dot) * normal.x), y = v.y - ((2f * dot) * normal.y) };
            return result;
        }

        public static void Reflect(ref Vector2 v, ref Vector2 normal, out Vector2 result) {
            Dot(ref v, ref normal, out var dot);
            result = new Vector2 { x = v.x - ((2f * dot) * normal.x), y = v.y - ((2f * dot) * normal.y) };
        }

        public static Vector2 Clamp(Vector2 v, Vector2 min, Vector2 max) {
            return new Vector2(Math.Clamp(v.x, min.x, max.x), Math.Clamp(v.y, min.y, max.y));
        }

        public static void Clamp(ref Vector2 v, ref Vector2 min, ref Vector2 max, out Vector2 result) {
            result = new Vector2(Math.Clamp(v.x, min.x, max.x), Math.Clamp(v.y, min.y, max.y));
        }

        public static FLOAT Distance(Vector2 v1, Vector2 v2) {
            var num = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
            return Math.Sqrt(num);
        }

        public static void Distance(ref Vector2 v1, ref Vector2 v2, out FLOAT result) {
            var num = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
            result = Math.Sqrt(num);
        }

        public static FLOAT DistanceSquared(Vector2 v1, Vector2 v2) {
            var result = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
            return result;
        }

        public static void DistanceSquared(ref Vector2 v1, ref Vector2 v2, out FLOAT result) {
            result = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
        }

        public static FLOAT Dot(Vector2 v1, Vector2 v2) {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static void Dot(ref Vector2 v1, ref Vector2 v2, out FLOAT result) {
            result = v1.x * v2.x + v1.y * v2.y;
        }

        public static FLOAT Angle(Vector2 a, Vector2 b) {
            FLOAT num = Math.Sqrt(a.sqrMagnitude * b.sqrMagnitude);
            if (Math.CompareApproximateZero(num)) {
                return 0;
            }

            Dot(ref a, ref b, out var dot);
            dot = Math.Clamp(dot / num, -1, 1);
            var rad = Math.Acos(dot);
            return Math.RadToDeg(rad);
        }

        public static void Angle(ref Vector2 a, ref Vector2 b, out FLOAT result) {
            FLOAT num = Math.Sqrt(a.sqrMagnitude * b.sqrMagnitude);
            if (Math.CompareApproximateZero(num)) {
                result = 0;
                return;
            }

            Dot(ref a, ref b, out var dot);
            dot = Math.Clamp(dot / num, -1, 1);
            var rad = Math.Acos(dot);
            result = Math.RadToDeg(rad);
        }

        public static FLOAT Normalize(ref Vector2 value) {
            var mag = value.magnitude;
            if (Math.CompareApproximateZero(mag)) {
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
        public static FLOAT SetLength(ref Vector2 vector, FLOAT lengthValue) {
            FLOAT num = Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
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
        public static FLOAT GrowLength(ref Vector2 vector, FLOAT lengthDelta) {
            FLOAT num = Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
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

        public Vector2(FLOAT xyz) {
            this.x = xyz;
            this.y = xyz;
        }

        public Vector2(FLOAT x, FLOAT y) {
            this.x = x;
            this.y = y;
        }

        public void Set(FLOAT x, FLOAT y) {
            this.x = x;
            this.y = y;
        }

        public void Scale(Vector2 other) {
            this.x = this.x * other.x;
            this.y = this.y * other.y;
        }

        /// <summary>
        /// 是否为0（使用了Epsilon）
        /// </summary>
        /// <returns></returns>
        public bool IsZero() {
            return Math.CompareApproximateZero(this.sqrMagnitude);
        }

        public void Normalize() {
            /*
             * 向量 / 向量长度 = 单位向量
             *
             * 对于趋近与 0的向量，normalize后，会自动规整为 0
             */
            FLOAT mag = this.magnitude;
            FLOAT num = 0;
            if (!Math.CompareApproximateZero(mag)) {
                num = 1 / mag;
            }

            this.x *= num;
            this.y *= num;
        }

        public void Negate() {
            this.x = -this.x;
            this.y = -this.y;
        }

        public FLOAT Dot(Vector2 value) {
            return this.x * value.x + this.y * value.y;
        }

        public Vector3 ToFVector3() {
            return new Vector3(this.x, this.y, 0);
        }

        public bool Equals(Vector2 other) {
            return Math.CompareApproximate(this.x, other.x) && Math.CompareApproximate(this.y, other.y);
        }

        public override bool Equals(object obj) {
            if (!(obj is Vector2 other)) {
                return false;
            }

            return Math.CompareApproximate(this.x, other.x) && Math.CompareApproximate(this.y, other.y);
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

        public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result) {
            var x = value1.x + value2.x;
            var y = value1.y + value2.y;

            result.x = x;
            result.y = y;
        }

        public static void Subtract(ref Vector2 value1, ref Vector2 value2, out Vector2 result) {
            var x = value1.x - value2.x;
            var y = value1.y - value2.y;

            result.x = x;
            result.y = y;
        }

        public static void Multiply(ref Vector2 value1, FLOAT scaleFactor, out Vector2 result) {
            var x = value1.x * scaleFactor;
            var y = value1.y * scaleFactor;

            result.x = x;
            result.y = y;
        }

        public static void Divide(ref Vector2 value1, FLOAT divider, out Vector2 result) {
            var factor = 1 / divider;
            var x = value1.x * factor;
            var y = value1.y * factor;

            result.x = x;
            result.y = y;
        }

        public static void Negate(ref Vector2 value, out Vector2 result) {
            var x = -value.x;
            var y = -value.y;

            result.x = x;
            result.y = y;
        }

        public static bool operator ==(Vector2 v1, Vector2 v2) {
            return Math.CompareApproximate(v1.x, v2.x) && Math.CompareApproximate(v1.y, v2.y);
        }

        public static bool operator !=(Vector2 v1, Vector2 v2) {
            return !Math.CompareApproximate(v1.x, v2.x) || !Math.CompareApproximate(v1.y, v2.y);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2) {
            v1.x += v2.x;
            v1.y += v2.y;
            return v1;
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2) {
            v1.x -= v2.x;
            v1.y -= v2.y;
            return v1;
        }

        public static Vector2 operator -(Vector2 v) {
            v.x = -v.x;
            v.y = -v.y;
            return v;
        }

        public static Vector2 operator *(Vector2 v, FLOAT scaleFactor) {
            v.x *= scaleFactor;
            v.y *= scaleFactor;
            return v;
        }

        public static Vector2 operator *(FLOAT scaleFactor, Vector2 v) {
            v.x *= scaleFactor;
            v.y *= scaleFactor;
            return v;
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2) {
            v1.x /= v2.x;
            v1.y /= v2.y;
            return v1;
        }

        public static Vector2 operator /(Vector2 v1, FLOAT divider) {
            var factor = 1 / divider;
            v1.x *= factor;
            v1.y *= factor;
            return v1;
        }

#if UNITY_5_3_OR_NEWER
        public static implicit operator UnityEngine.Vector2(Vector2 v)
        {
            return new UnityEngine.Vector2(v.x, v.y);
        }

        public static implicit operator Vector2(UnityEngine.Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }
#endif
    }
}