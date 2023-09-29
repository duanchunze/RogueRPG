using System;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Implements presudo-random number generator using Xorshift128 algorithm.
    /// </summary>
    public class FRandom {
        private const int a = 5;

        private const int b = 14;

        private const int c = 1;

        private const uint DefaultY = 273326509u;

        private const uint DefaultZ = 3579807591u;

        private const uint DefaultW = 842502087u;

        private const uint PositiveMask = 2147483647u;

        private const uint BoolModuloMask = 1u;

        private const uint ByteModuloMask = 255u;

        // private static readonly FLOAT One_div_uintMaxValuePlusOne = (float) 2.3283064365386963E-10;

        private static readonly FLOAT TwoPi = (FLOAT)(FMath.Pi * 2.0);

        private static FRandom _seedGenerator;

        private uint _x;

        private uint _y;

        private uint _z;

        private uint _w;

        public static FRandom Instance;

        static FRandom() {
            _seedGenerator = new FRandom(Environment.TickCount);
            Instance = new FRandom();
        }

        /// <summary>
        /// Creates random number generator using randomized seed.
        /// </summary>
        public FRandom() {
            this.ResetSeed(_seedGenerator.NextInt());
        }

        /// <summary>
        /// Creates random number generator using specified seed.
        /// </summary>
        public FRandom(int seed) {
            this.ResetSeed(seed);
        }

        /// <summary>
        /// Resets generator using specified seed.
        /// </summary>
        public void ResetSeed(int seed) {
            this._x = (uint)(seed * 1183186591 + seed * 1431655781 + seed * 338294347 + seed * 622729787);
            this._y = 273326509u;
            this._z = 3579807591u;
            this._w = 842502087u;
        }

        /// <summary>
        /// Gets generator inner state represented by four uints. Can be used for generator serialization.
        /// </summary>
        public void GetState(out uint x, out uint y, out uint z, out uint w) {
            x = this._x;
            y = this._y;
            z = this._z;
            w = this._w;
        }

        /// <summary>
        /// Sets generator inner state from four uints. Can be used for generator deserialization.
        /// </summary>
        public void SetState(uint x, uint y, uint z, uint w) {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;
        }

        /// <summary>
        /// Generates a random integer in the range [int.MinValue,int.MaxValue].
        /// </summary>
        public int NextInt() {
            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return (int)this._w;
        }

        /// <summary>
        /// Generates a random integer in the range [0,max)
        /// </summary>
        public int NextInt(int max) {
            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return (int)((FLOAT)this._w * 2.3283064365386963E-10 * (FLOAT)max);
        }

        /// <summary>
        /// Generates a random integer in the range [min,max). max must be &gt;= min.
        /// </summary>
        public int NextInt(int min, int max) {
            if (min > max) {
                return 0;
            }

            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            int num2 = max - min;
            if (num2 >= 0) {
                return min + (int)((FLOAT)this._w * 2.3283064365386963E-10 * (FLOAT)num2);
            }

            long num3 = min;
            return (int)(num3 + (long)((FLOAT)this._w * 2.3283064365386963E-10 * (FLOAT)(max - num3)));
        }

        /// <summary>
        /// Generates a random integer in the range [min,max]. max must be &gt;= min.
        /// The method simply calls NextInt(min,max+1), thus largest allowable value for max is int.MaxValue-1.
        /// </summary>
        public int NextIntInclusive(int min, int max) {
            return this.NextInt(min, max + 1);
        }

        /// <summary>
        /// Generates a random integer in the range [0,int.MaxValue].
        /// </summary>
        public int NextPositiveInt() {
            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return (int)(this._w & 0x7FFFFFFF);
        }

        /// <summary>
        /// Generates a random unsigned integer in the range [0,uint.MaxValue].
        /// </summary>
        public uint NextUInt() {
            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return this._w;
        }

        /// <summary>
        /// Generates a random FLOAT in the range [0,1).
        /// </summary>
        public double NextDouble() {
            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return this._w * 2.3283064365386963E-10;
        }

        /// <summary>
        /// Generates a random FLOAT in the range [min,max).
        /// </summary>
        public double NextDouble(double min, double max) {
            if (min > max) {
                return (FLOAT)0.0;
            }

            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return min + (max - min) * (this._w * 2.3283064365386963E-10);
        }

        /// <summary>
        /// Generates a random FLOAT in the range [0,1).
        /// </summary>
        public FLOAT NextFloat() {
            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return (FLOAT)((FLOAT)this._w * 2.3283064365386963E-10);
        }

        /// <summary>
        /// Generates a random FLOAT in the range [min,max).
        /// </summary>
        public FLOAT NextFloat(FLOAT min, FLOAT max) {
            if (min > max) {
                return 0f;
            }

            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return min + (max - min) * (FLOAT)((FLOAT)this._w * 2.3283064365386963E-10);
        }

        /// <summary>
        /// Generates a random bool.
        /// </summary>
        public bool NextBool() {
            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return (this._w & 1) == 0;
        }

        /// <summary>
        /// Generates a random byte.
        /// </summary>
        public byte NextByte() {
            uint num = this._x ^ (this._x << 5);
            this._x = this._y;
            this._y = this._z;
            this._z = this._w;
            this._w = this._w ^ (this._w >> 1) ^ (num ^ (num >> 14));
            return (byte)(this._w & 0xFFu);
        }

        /// <summary>
        /// Generates a random angle [0,2*pi)
        /// </summary>
        public FLOAT RandomAngleRadians() {
            return this.NextFloat() * (FMath.Pi * 2f);
        }

        /// <summary>
        /// Generates a random angle [0,360)
        /// </summary>
        public FLOAT RandomAngleDegrees() {
            return this.NextFloat() * 360f;
        }

        /// <summary>
        /// Generates a random point inside the square with specified side size.
        /// </summary>
        public FVector2 InSquare() {
            FLOAT side = 1f;
            FLOAT num = side * 0.5f;
            FLOAT min = 0f - num;
            return new FVector2(this.NextFloat(min, num), this.NextFloat(min, num));
        }

        /// <summary>
        /// Generates a random point inside the square with specified side size.
        /// </summary>
        public FVector2 InSquare(FLOAT side) {
            FLOAT num = side * 0.5f;
            FLOAT min = 0f - num;
            return new FVector2(this.NextFloat(min, num), this.NextFloat(min, num));
        }

        /// <summary>
        /// Generates a random point on the border of the square with specified side size.
        /// </summary>
        public FVector2 OnSquare() {
            FLOAT side = 1f;
            FLOAT num = side * 0.5f;
            FLOAT num2 = 0f - num;
            switch (this.NextInt(0, 4)) {
                case 0:
                    return new FVector2(this.NextFloat(num2, num), num);
                case 1:
                    return new FVector2(this.NextFloat(num2, num), num2);
                case 2:
                    return new FVector2(num, this.NextFloat(num2, num));
                case 3:
                    return new FVector2(num2, this.NextFloat(num2, num));
                default:
                    return FVector2.Zero;
            }
        }

        /// <summary>
        /// Generates a random point on the border of the square with specified side size.
        /// </summary>
        public FVector2 OnSquare(FLOAT side) {
            FLOAT num = side * 0.5f;
            FLOAT num2 = 0f - num;
            switch (this.NextInt(0, 4)) {
                case 0:
                    return new FVector2(this.NextFloat(num2, num), num);
                case 1:
                    return new FVector2(this.NextFloat(num2, num), num2);
                case 2:
                    return new FVector2(num, this.NextFloat(num2, num));
                case 3:
                    return new FVector2(num2, this.NextFloat(num2, num));
                default:
                    return FVector2.Zero;
            }
        }

        /// <summary>
        /// Generates a random point inside the cube with specified side size.
        /// </summary>
        public FVector3 InCube() {
            FLOAT side = 1f;
            FLOAT num = side * 0.5f;
            FLOAT min = 0f - num;
            return new FVector3(this.NextFloat(min, num), this.NextFloat(min, num), this.NextFloat(min, num));
        }

        /// <summary>
        /// Generates a random point inside the cube with specified side size.
        /// </summary>
        public FVector3 InCube(FLOAT side) {
            FLOAT num = side * 0.5f;
            FLOAT min = 0f - num;
            return new FVector3(this.NextFloat(min, num), this.NextFloat(min, num), this.NextFloat(min, num));
        }

        /// <summary>
        /// Generates a random point on the surface of the cube with specified side size.
        /// </summary>
        public FVector3 OnCube() {
            FLOAT side = 1f;
            FLOAT num = side * 0.5f;
            FLOAT num2 = 0f - num;
            switch (this.NextInt(0, 6)) {
                case 0:
                    return new FVector3(this.NextFloat(num2, num), this.NextFloat(num2, num), num);
                case 1:
                    return new FVector3(this.NextFloat(num2, num), this.NextFloat(num2, num), num2);
                case 2:
                    return new FVector3(this.NextFloat(num2, num), num, this.NextFloat(num2, num));
                case 3:
                    return new FVector3(this.NextFloat(num2, num), num2, this.NextFloat(num2, num));
                case 4:
                    return new FVector3(num, this.NextFloat(num2, num), this.NextFloat(num2, num));
                case 5:
                    return new FVector3(num2, this.NextFloat(num2, num), this.NextFloat(num2, num));
                default:
                    return FVector3.Zero;
            }
        }

        /// <summary>
        /// Generates a random point on the surface of the cube with specified side size.
        /// </summary>
        public FVector3 OnCube(FLOAT side) {
            FLOAT num = side * 0.5f;
            FLOAT num2 = 0f - num;
            switch (this.NextInt(0, 6)) {
                case 0:
                    return new FVector3(this.NextFloat(num2, num), this.NextFloat(num2, num), num);
                case 1:
                    return new FVector3(this.NextFloat(num2, num), this.NextFloat(num2, num), num2);
                case 2:
                    return new FVector3(this.NextFloat(num2, num), num, this.NextFloat(num2, num));
                case 3:
                    return new FVector3(this.NextFloat(num2, num), num2, this.NextFloat(num2, num));
                case 4:
                    return new FVector3(num, this.NextFloat(num2, num), this.NextFloat(num2, num));
                case 5:
                    return new FVector3(num2, this.NextFloat(num2, num), this.NextFloat(num2, num));
                default:
                    return FVector3.Zero;
            }
        }

        /// <summary>
        /// Generates a random point inside the circle with specified radius.
        /// </summary>
        public FVector2 InCircle() {
            FLOAT radius = 1f;
            FLOAT num = radius * FMath.Sqrt(this.NextFloat());
            FLOAT f = this.RandomAngleRadians();
            return new FVector2(num * FMath.Cos(f), num * FMath.Sin(f));
        }

        /// <summary>
        /// Generates a random point inside the circle with specified radius.
        /// </summary>
        public FVector2 InCircle(FLOAT radius) {
            FLOAT num = radius * FMath.Sqrt(this.NextFloat());
            FLOAT f = this.RandomAngleRadians();
            return new FVector2(num * FMath.Cos(f), num * FMath.Sin(f));
        }

        /// <summary>
        /// Generates a random point inside the ring with specified radia.
        /// </summary>
        public FVector2 InCircle(FLOAT radiusMin, FLOAT radiusMax) {
            FLOAT num = 2f / (radiusMax * radiusMax - radiusMin * radiusMin);
            FLOAT num2 = FMath.Sqrt(2f * this.NextFloat() / num + radiusMin * radiusMin);
            FLOAT f = this.RandomAngleRadians();
            return new FVector2(num2 * FMath.Cos(f), num2 * FMath.Sin(f));
        }

        /// <summary>
        /// Generates a random point on the border of the circle with specified radius.
        /// </summary>
        public FVector2 OnCircle() {
            FLOAT radius = 1f;
            FLOAT f = this.RandomAngleRadians();
            return new FVector2(FMath.Cos(f) * radius, FMath.Sin(f) * radius);
        }

        /// <summary>
        /// Generates a random point on the border of the circle with specified radius.
        /// </summary>
        public FVector2 OnCircle(FLOAT radius) {
            FLOAT f = this.RandomAngleRadians();
            return new FVector2(FMath.Cos(f) * radius, FMath.Sin(f) * radius);
        }

        /// <summary>
        /// Generates a random point inside the sphere with specified radius.
        /// </summary>
        public FVector3 InSphere() {
            FLOAT radius = 1f;
            FLOAT num = radius * 2f;
            FLOAT num2 = radius * radius;
            FLOAT num3;
            FLOAT num4;
            FLOAT num5;
            do {
                num3 = this.NextFloat() * num - radius;
                num4 = this.NextFloat() * num - radius;
                num5 = this.NextFloat() * num - radius;
            } while (!(num3 * num3 + num4 * num4 + num5 * num5 <= num2));

            return new FVector3(num3, num4, num5);
        }

        /// <summary>
        /// Generates a random point inside the sphere with specified radius.
        /// </summary>
        public FVector3 InSphere(FLOAT radius) {
            FLOAT num = radius * 2f;
            FLOAT num2 = radius * radius;
            FLOAT num3;
            FLOAT num4;
            FLOAT num5;
            do {
                num3 = this.NextFloat() * num - radius;
                num4 = this.NextFloat() * num - radius;
                num5 = this.NextFloat() * num - radius;
            } while (!(num3 * num3 + num4 * num4 + num5 * num5 <= num2));

            return new FVector3(num3, num4, num5);
        }

        /// <summary>
        /// Generates a random point on the surface of the sphere with specified radius.
        /// </summary>
        public FVector3 OnSphere() {
            FLOAT radius = 1f;
            FLOAT num = this.NextFloat() * 2f - 1f;
            FLOAT f = this.NextFloat() * ((FLOAT)FMath.Pi * 2f);
            FLOAT num2 = FMath.Sqrt(1f - num * num) * radius;
            return new FVector3(num2 * FMath.Cos(f), num2 * FMath.Sin(f), num * radius);
        }

        /// <summary>
        /// Generates a random point on the surface of the sphere with specified radius.
        /// </summary>
        public FVector3 OnSphere(FLOAT radius) {
            FLOAT num = this.NextFloat() * 2f - 1f;
            FLOAT f = this.NextFloat() * ((FLOAT)FMath.Pi * 2f);
            FLOAT num2 = FMath.Sqrt(1f - num * num) * radius;
            return new FVector3(num2 * FMath.Cos(f), num2 * FMath.Sin(f), num * radius);
        }

        /// <summary>
        /// Generates a random point inside the triangle.
        /// </summary>
        public FVector3 InTriangle(ref FVector3 v0, ref FVector3 v1, ref FVector3 v2) {
            FLOAT num = FMath.Sqrt((FLOAT)this.NextDouble());
            FLOAT num2 = (FLOAT)this.NextDouble();
            FVector3 vector = (FLOAT)(1.0 - num) * v0;
            FVector3 vector2 = (FLOAT)(num * (1.0 - num2)) * v1;
            FVector3 vector3 = (FLOAT)(num2 * num) * v2;
            return new FVector3(vector.x + vector2.x + vector3.x, vector.y + vector2.y + vector3.y,
                vector.z + vector2.z + vector3.z);
        }

        /// <summary>
        /// Generates a random point inside the triangle.
        /// </summary>
        public FVector3 InTriangle(FVector3 v0, FVector3 v1, FVector3 v2) {
            FLOAT num = FMath.Sqrt((FLOAT)this.NextDouble());
            FLOAT num2 = (FLOAT)this.NextDouble();
            FVector3 vector = (FLOAT)(1.0 - num) * v0;
            FVector3 vector2 = (FLOAT)(num * (1.0 - num2)) * v1;
            FVector3 vector3 = (FLOAT)(num2 * num) * v2;
            return new FVector3(vector.x + vector2.x + vector3.x, vector.y + vector2.y + vector3.y,
                vector.z + vector2.z + vector3.z);
        }

        /// <summary>
        /// Generates a random rotation.
        /// </summary>
        public FQuaternion RandomRotation() {
            FLOAT num = (FLOAT)this.NextDouble();
            FLOAT num2 = (FLOAT)this.NextDouble();
            FLOAT num3 = (FLOAT)this.NextDouble();
            FLOAT num4 = FMath.Sqrt(num);
            FLOAT num5 = FMath.Sqrt((FLOAT)(1.0 - num));
            FLOAT d = (FLOAT)(FMath.Pi * 2.0 * num2);
            FLOAT d2 = (FLOAT)(FMath.Pi * 2.0 * num3);
            return new FQuaternion((FLOAT)(num5 * FMath.Sin(d)), (FLOAT)(num5 * FMath.Cos(d)),
                (FLOAT)(num4 * FMath.Sin(d2)), (FLOAT)(num4 * FMath.Cos(d2)));
        }
    }
}