using System;
using System.Globalization;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public struct Num {
        public const int Factor = 10000;
        public const float FactorFloat = 10000f;
        
        public static Num Empty(byte type = 2) {
            return new Num { _type = type };
        }

#if UNITY_EDITOR
        [ShowInInspector, HideLabel, HorizontalGroup("h")]
#endif
        private long _raw;

#if UNITY_EDITOR
        [ShowInInspector, HideLabel, HorizontalGroup("h")]
#endif
        private byte _type; // 0 float, 1 int, 2 long

        public byte Type => this._type;

        public Num(float value) {
            this._raw = (long)(value * Factor);
            this._type = 0;
        }

        public Num(double value) {
            this._raw = (long)(value * Factor);
            this._type = 0;
        }

        public Num(int value) {
            this._raw = value;
            this._type = 1;
        }

        public Num(long value) {
            this._raw = value;
            this._type = 2;
        }

        public bool IsZero() => this._raw == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetAsFloat() {
            if (this._type == 0)
                return this._raw / FactorFloat; // 使用 / 10000f 比使用 * 0.0001f 精度更高

            return this._raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetAsDouble() {
            if (this._type == 0)
                return this._raw / FactorFloat; // 使用 / 10000f 比使用 * 0.0001f 精度更高

            return this._raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAsInt() {
            if (this._type == 0) {
                return (int)(this._raw / Factor);
            }

            return (int)this._raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetAsLong() {
            if (this._type == 0) {
                return this._raw / Factor;
            }

            return this._raw;
        }

        public void ToFloat() {
            if (this._type == 0) return;
            this._raw *= Factor;
            this._type = 0;
        }

        public void ToInt() {
            switch (this._type) {
                case 1:
                    return;
                case 0:
                    this._raw = (this._raw / Factor);
                    break;
            }

            this._type = 1;
        }

        public void ToLong() {
            switch (this._type) {
                case 2:
                    return;
                case 0:
                    this._raw = (this._raw / Factor);
                    break;
            }

            this._type = 2;
        }

        public void Convert(byte type) {
            if (this._type == type) return;
            if (this._type != 0 && type != 0) {
                this._type = type;
                return;
            }

            if (this._type == 0) {
                this._raw = (this._raw / Factor);
                this._type = type;
                return;
            }

            if (type == 0) {
                this._raw *= Factor;
                this._type = type;
            }
        }

        public override string ToString() {
            return this._type switch {
                0 => this.GetAsFloat().ToString(CultureInfo.InvariantCulture),
                1 => this.GetAsInt().ToString(CultureInfo.InvariantCulture),
                2 => this.GetAsLong().ToString(CultureInfo.InvariantCulture),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool Equals(Num other) {
            // 先把对方转换成和自己一样的类型, 在比较
            if (other._type != this._type) {
                other.Convert(this._type);
            }

            return this._raw == other._raw;
        }

        public override bool Equals(object obj) {
            return obj is Num other && this.Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(this._raw, this._type);
        }

        public static bool operator ==(Num lhs, Num rhs) {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Num lhs, Num rhs) {
            return !lhs.Equals(rhs);
        }

        public static Num operator +(Num lhs, Num rhs) {
            // 如果同类型的, 直接相加, 且保留类型
            if (lhs._type == rhs._type) return new Num(lhs._raw + rhs._raw) { _type = lhs._type };

            // 如果不是同类型, 但都不是浮点类型, 则直接相加, 且类型保留为最大类型
            if (lhs._type != 0 && rhs._type != 0) return new Num(lhs._raw + rhs._raw) { _type = System.Math.Max(lhs._type, rhs._type) };

            // 如果不是同类型, 且存在浮点类型, 则都转成浮点, 然后让原始值相加, 结果取为浮点类型
            lhs.ToFloat();
            rhs.ToFloat();
            return new Num(lhs._raw + rhs._raw) { _type = 0 };
        }

        public static Num operator -(Num lhs, Num rhs) {
            if (lhs._type == rhs._type) return new Num(lhs._raw - rhs._raw) { _type = lhs._type };
            if (lhs._type != 0 && rhs._type != 0) return new Num(lhs._raw - rhs._raw) { _type = System.Math.Max(lhs._type, rhs._type) };
            lhs.ToFloat();
            rhs.ToFloat();
            return new Num(lhs._raw - rhs._raw) { _type = 0 };
        }

        public static Num operator *(Num lhs, Num rhs) {
            // 如果两数均不为浮点数, 则直接相乘, 类型取最大类型
            if (lhs._type != 0 && rhs._type != 0) return new Num(lhs._raw * rhs._raw) { _type = System.Math.Max(lhs._type, rhs._type) };

            // 如果两数存在浮点数, 则用两数的浮点结果相乘, 结果取为浮点数
            return new Num(lhs.GetAsDouble() * rhs.GetAsDouble());
        }

        public static Num operator /(Num lhs, Num rhs) {
            if (lhs._type != 0 && rhs._type != 0) return new Num(lhs._raw / rhs._raw) { _type = System.Math.Max(lhs._type, rhs._type) };
            return new Num(lhs.GetAsDouble() / rhs.GetAsDouble());
        }

        public static implicit operator float(Num value) {
            return value.GetAsFloat();
        }

        public static implicit operator double(Num value) {
            return value.GetAsDouble();
        }

        public static implicit operator int(Num value) {
            return value.GetAsInt();
        }

        public static implicit operator long(Num value) {
            return value.GetAsLong();
        }

        public static implicit operator Num(float value) {
            return new Num(value);
        }

        public static implicit operator Num(double value) {
            return new Num(value);
        }

        public static implicit operator Num(int value) {
            return new Num(value);
        }

        public static implicit operator Num(long value) {
            return new Num(value);
        }
    }
}