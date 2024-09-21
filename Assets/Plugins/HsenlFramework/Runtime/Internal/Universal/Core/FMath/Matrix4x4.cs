using System;
using System.Runtime.InteropServices;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /*
     * 一些不常用的方法，给改成私有的了
     * 比如创建就用 TRS创建
     * 
     */
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Matrix4x4 : IEquatable<Matrix4x4> {
        internal static Matrix4x4 InternalIdentity;
        public static readonly Matrix4x4 Identity;
        public static readonly Matrix4x4 Zero;

        static Matrix4x4() {
            Zero = new Matrix4x4();
            Identity = new Matrix4x4 { m11 = 1, m22 = 1, m33 = 1, m44 = 1 };
            InternalIdentity = Identity;
        }

        public FLOAT m11; // 1st row vector
        public FLOAT m12;
        public FLOAT m13;
        public FLOAT m14;
        public FLOAT m21; // 2nd row vector
        public FLOAT m22;
        public FLOAT m23;
        public FLOAT m24;
        public FLOAT m31; // 3rd row vector
        public FLOAT m32;
        public FLOAT m33;
        public FLOAT m34;
        public FLOAT m41; // 4rd row vector
        public FLOAT m42;
        public FLOAT m43;
        public FLOAT m44;

        public unsafe FLOAT this[int row, int col] {
            get {
                fixed (FLOAT* numPtr = &this.m11) {
                    return numPtr[row * 4 + col];
                }
            }
            set {
                fixed (FLOAT* numPtr = &this.m11) {
                    numPtr[row * 4 + col] = value;
                }
            }
        }

        public unsafe FLOAT this[int index] {
            get {
                fixed (FLOAT* numPtr = &this.m11) {
                    return numPtr[index];
                }
            }
            set {
                fixed (FLOAT* numPtr = &this.m11) {
                    numPtr[index] = value;
                }
            }
        }

        public Vector3 up {
            get {
                Vector3 vector3;

                vector3.x = this.m12;
                vector3.y = this.m22;
                vector3.z = this.m32;

                return vector3;
            }
            set {
                this.m12 = value.x;
                this.m22 = value.y;
                this.m32 = value.z;
            }
        }

        public Vector3 down {
            get {
                Vector3 vector3;

                vector3.x = -this.m12;
                vector3.y = -this.m22;
                vector3.z = -this.m32;

                return vector3;
            }
            set {
                this.m12 = -value.x;
                this.m22 = -value.y;
                this.m32 = -value.z;
            }
        }

        public Vector3 right {
            get {
                Vector3 vector3;

                vector3.x = this.m11;
                vector3.y = this.m21;
                vector3.z = this.m31;

                return vector3;
            }
            set {
                this.m11 = value.x;
                this.m21 = value.y;
                this.m31 = value.z;
            }
        }

        public Vector3 left {
            get {
                Vector3 vector3;

                vector3.x = -this.m11;
                vector3.y = -this.m21;
                vector3.z = -this.m31;

                return vector3;
            }
            set {
                this.m11 = -value.x;
                this.m21 = -value.y;
                this.m31 = -value.z;
            }
        }

        public Vector3 forward {
            get {
                Vector3 vector3;

                vector3.x = -this.m13;
                vector3.y = -this.m23;
                vector3.z = -this.m33;

                return vector3;
            }
            set {
                this.m13 = -value.x;
                this.m23 = -value.y;
                this.m33 = -value.z;
            }
        }

        public Vector3 back {
            get {
                Vector3 vector3;

                vector3.x = this.m13;
                vector3.y = this.m23;
                vector3.z = this.m33;

                return vector3;
            }
            set {
                this.m13 = value.x;
                this.m23 = value.y;
                this.m33 = value.z;
            }
        }

        /// <summary>
        /// 矩阵的行列式
        /// 几何意义：体积，谁的还不清楚
        /// </summary>
        public FLOAT determinant {
            get {
                // | a b c d |     | f g h |     | e g h |     | e f h |     | e f g |
                // | e f g h | = a | j k l | - b | i k l | + c | i j l | - d | i j k |
                // | i j k l |     | n o p |     | m o p |     | m n p |     | m n o |
                // | m n o p |
                //
                //   | f g h |
                // a | j k l | = a ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
                //   | n o p |
                //
                //   | e g h |     
                // b | i k l | = b ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
                //   | m o p |     
                //
                //   | e f h |
                // c | i j l | = c ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
                //   | m n p |
                //
                //   | e f g |
                // d | i j k | = d ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
                //   | m n o |
                //
                // Cost of operation
                // 17 adds and 28 muls.
                //
                // add: 6 + 8 + 3 = 17
                // mul: 12 + 16 = 28

                FLOAT num11 = this.m11, num12 = this.m12, num13 = this.m13, num14 = this.m14;
                FLOAT num21 = this.m21, num22 = this.m22, num23 = this.m23, num24 = this.m24;
                FLOAT num31 = this.m31, num32 = this.m32, num33 = this.m33, num34 = this.m34;
                FLOAT num41 = this.m41, num42 = this.m42, num43 = this.m43, num44 = this.m44;

                var num33443443 = num33 * num44 - num34 * num43;
                var num32443442 = num32 * num44 - num34 * num42;
                var num32433342 = num32 * num43 - num33 * num42;
                var num31443441 = num31 * num44 - num34 * num41;
                var num31433341 = num31 * num43 - num33 * num41;
                var num31423241 = num31 * num42 - num32 * num41;

                return num11 * (num22 * num33443443 - num23 * num32443442 + num24 * num32433342) -
                       num12 * (num21 * num33443443 - num23 * num31443441 + num24 * num31433341) +
                       num13 * (num21 * num32443442 - num22 * num31443441 + num24 * num31423241) -
                       num14 * (num21 * num32433342 - num22 * num31433341 + num23 * num31423241);
            }
        }

        /// <summary>
        /// 矩阵的位置
        /// </summary>
        public Vector3 position {
            get => new Vector3(this.m14, this.m24, this.m34);
            /* 不应该直接修改矩阵的 position
            set
            {
                m14 = value.x;
                m24 = value.y;
                m34 = value.z;
            }
            */
        }

        /// <summary>
        /// 矩阵的旋转
        /// </summary>
        public Quaternion rotation {
            get {
                var v1 = new Vector3(this.m13, this.m23, this.m33); // 获得第三列，第三列代表的是坐标系的正方向
                var v2 = new Vector3(this.m12, this.m22, this.m32); // 获得第二列，代表的是坐标轴的正上方
                Quaternion.CreateLookRotation(ref v1, ref v2, out var result);
                return result;
            }
        }

        /// <summary>
        /// 矩阵的缩放
        /// </summary>
        public Vector3 scale {
            get {
                var x = Math.Sqrt(this.m11 * this.m11 + this.m21 * this.m21 + this.m31 * this.m31);
                var y = Math.Sqrt(this.m12 * this.m12 + this.m22 * this.m22 + this.m32 * this.m32);
                var z = Math.Sqrt(this.m13 * this.m13 + this.m23 * this.m23 + this.m33 * this.m33);
                x *= Math.Sign(this.m11);
                y *= Math.Sign(this.m12);
                z *= Math.Sign(this.m13);
                return new Vector3(x, y, z);

                /* 用下面的方法得出的结果，缺少正负号
                 * return new FVector3(this.GetColumn(0).magnitude, this.GetColumn(1).magnitude, this.GetColumn(2).magnitude);
                 */
            }
        }

        /// <summary>
        /// 矩阵的逆矩阵
        /// </summary>
        public Matrix4x4 inverse {
            get {
                Invert(ref this, out var result);
                return result;
            }
        }

        /// <summary>
        /// 矩阵的转置矩阵
        /// </summary>
        public Matrix4x4 transposition {
            get {
                Transpose(ref this, out var result);
                return result;
            }
        }

        /// <summary>
        /// 矩阵的迹
        /// </summary>
        /// <returns></returns>
        public FLOAT trace => this.m11 + this.m22 + this.m33 + this.m44;

        public Matrix4x4(FLOAT m11, FLOAT m12, FLOAT m13, FLOAT m14, FLOAT m21, FLOAT m22, FLOAT m23, FLOAT m24, FLOAT m31,
            FLOAT m32, FLOAT m33, FLOAT m34, FLOAT m41, FLOAT m42, FLOAT m43, FLOAT m44) {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m14 = m14;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m24 = m24;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
            this.m34 = m34;
            this.m41 = m41;
            this.m42 = m42;
            this.m43 = m43;
            this.m44 = m44;
        }

        #region -------------创建矩阵静态函数

        /// <summary>
        /// 创建一个平移矩阵
        /// </summary>
        /// <param name="position">位移</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateTranslation(Vector3 position) {
            Matrix4x4 result;

            result.m11 = 1;
            result.m12 = 0;
            result.m13 = 0;
            result.m14 = position.x;
            result.m21 = 0;
            result.m22 = 1;
            result.m23 = 0;
            result.m24 = position.y;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = 1;
            result.m34 = position.z;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个平移矩阵
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <param name="zPosition"></param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateTranslation(FLOAT xPosition, FLOAT yPosition, FLOAT zPosition) {
            Matrix4x4 result;

            result.m11 = 1;
            result.m12 = 0;
            result.m13 = 0;
            result.m14 = xPosition;
            result.m21 = 0;
            result.m22 = 1;
            result.m23 = 0;
            result.m24 = yPosition;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = 1;
            result.m34 = zPosition;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个平移矩阵
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <param name="zPosition"></param>
        /// <param name="result">返回一个矩阵</param>
        private static void CreateTranslation(FLOAT xPosition, FLOAT yPosition, FLOAT zPosition,
            out Matrix4x4 result) {
            result.m11 = 1;
            result.m12 = 0;
            result.m13 = 0;
            result.m14 = xPosition;
            result.m21 = 0;
            result.m22 = 1;
            result.m23 = 0;
            result.m24 = yPosition;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = 1;
            result.m34 = zPosition;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;
        }

        /// <summary>
        /// 创建一个平移矩阵
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="result">返回一个矩阵</param>
        private static void CreateTranslation(ref Vector3 position, out Matrix4x4 result) {
            result.m11 = 1;
            result.m12 = 0;
            result.m13 = 0;
            result.m14 = position.x;
            result.m21 = 0;
            result.m22 = 1;
            result.m23 = 0;
            result.m24 = position.y;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = 1;
            result.m34 = position.z;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;
        }

        /// <summary>
        /// 创建一个旋转矩阵，通过欧拉角
        /// </summary>
        /// <param name="euler">旋转欧拉角</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix4x4 CreateRotation(Vector3 euler) {
            Quaternion.Euler(ref euler, out var q);
            CreateRotation(ref q, out var result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        private static Matrix4x4 CreateRotationOfEuler(Vector3 euler) {
            /* 该方法仅仅作为兴趣拓展，实际上不会用这种方法，因为直接用欧拉角来创建旋转矩阵是错误的。通过该方法创建的旋转矩阵，就需要用下面的方式来获得欧拉角
             *
             * var p = FMath.Asin(matrix4X4.m32);
             * var h = FMath.Atan2(-matrix4X4.m31, matrix4X4.m33);
             * var b = FMath.Atan2(-matrix4X4.m12, matrix4X4.m22);
             * return new FVector3(FMath.RadToDeg(p), FMath.RadToDeg(h), FMath.RadToDeg(b));
             *
             * 
             * 左手坐标系下的基础旋转矩阵
             * 
             * 绕 x轴旋转 p度，对应矩阵 Rx
             *      [1  0     0   ]
             * Rx = [0  cosp  sinp]
             *      [0 -sinp  cosp]
             *
             * 绕 y轴旋转 h度，对应矩阵 Ry
             *      [cosh  0  -sinh]
             * Ry = [0     1  0    ]
             *      [sinh  0  cosh ]
             *
             * 绕 z轴旋转 b度，对应矩阵 Rz
             *      [cosb  sinb  0]
             * Rz = [-sinb cosb  0]
             *      [0     0     1]
             *
             * 则 R = Rz * Rx * Ry（Z-X-Y顺序）
             * 
             *     [cosb*cosh+sinb*sinp*sinh   sinb*cosp  -cosb*sinh+sinb*sinp*cosh]
             * R = [-sinb*cosh+cosb*sinp*sinh  cosb*cosp  sinb*sinh+cosb*sinp*cosh ]
             *     [cosp*sinh                 -sinp       cosp*cosh                ]
             * 
             */

            Matrix4x4 result = new Matrix4x4();
            var p = Math.DegToRad(euler.x);
            var h = Math.DegToRad(euler.y);
            var b = Math.DegToRad(euler.z);
            var cosp = Math.Cos(p);
            var cosh = Math.Cos(h);
            var cosb = Math.Cos(b);
            var sinp = Math.Sin(p);
            var sinh = Math.Sin(h);
            var sinb = Math.Sin(b);

            result.m11 = cosb * cosh + sinb * sinp * sinh;
            result.m12 = sinb * cosp;
            result.m13 = -cosb * sinh + sinb * sinp * cosh;
            result.m14 = 0;
            result.m21 = -sinb * cosh + cosb * sinp * sinh;
            result.m22 = cosb * cosp;
            result.m23 = sinb * sinh + cosb * sinp * cosh;
            result.m24 = 0;
            result.m31 = cosp * sinh;
            result.m32 = -sinp;
            result.m33 = cosp * cosh;
            result.m34 = 0;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;
            return result;
        }

        /// <summary>
        /// 创建一个旋转矩阵，通过欧拉角
        /// </summary>
        /// <param name="xEuler"></param>
        /// <param name="yEuler"></param>
        /// <param name="zEuler"></param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix4x4 CreateRotation(FLOAT xEuler, FLOAT yEuler, FLOAT zEuler) {
            Quaternion.CreateYawPitchRoll(yEuler, xEuler, zEuler, out var q);
            CreateRotation(ref q, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个旋转矩阵，通过欧拉角
        /// </summary>
        /// <param name="euler">旋转欧拉角</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateRotation(ref Vector3 euler, out Matrix4x4 result) {
            Quaternion.Euler(ref euler, out var q);
            CreateRotation(ref q, out result);
        }

        /// <summary>
        /// 创建一个旋转矩阵，通过四元数
        /// </summary>
        /// <param name="quaternion">旋转四元数</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix4x4 CreateRotation(Quaternion quaternion) {
            CreateRotation(ref quaternion, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个旋转矩阵，通过四元数
        /// </summary>
        /// <param name="quaternion">旋转四元数</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateRotation(ref Quaternion quaternion, out Matrix4x4 result) {
            // Precalculate coordinate products
            var qx = quaternion.x;
            var qy = quaternion.y;
            var qz = quaternion.z;
            var qw = quaternion.w;
            var x = qx * 2;
            var y = qy * 2;
            var z = qz * 2;
            var xx = qx * x;
            var yy = qy * y;
            var zz = qz * z;
            var xy = qx * y;
            var xz = qx * z;
            var yz = qy * z;
            var wx = qw * x;
            var wy = qw * y;
            var wz = qw * z;

            // Calculate 3x3 matrix from orthonormal basis
            result.m11 = 1 - (yy + zz);
            result.m21 = xy + wz;
            result.m31 = xz - wy;
            result.m41 = 0;
            result.m12 = xy - wz;
            result.m22 = 1 - (xx + zz);
            result.m32 = yz + wx;
            result.m42 = 0;
            result.m13 = xz + wy;
            result.m23 = yz - wx;
            result.m33 = 1 - (xx + yy);
            result.m43 = 0;
            result.m14 = 0;
            result.m24 = 0;
            result.m34 = 0;
            result.m44 = 1;
        }

        /// <summary>
        /// 创建一个缩放矩阵
        /// </summary>
        /// <param name="scales">缩放</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateScale(Vector3 scales) {
            CreateScale(scales.x, scales.y, scales.z, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个缩放矩阵
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateScale(FLOAT xScale, FLOAT yScale, FLOAT zScale) {
            Matrix4x4 result;

            result.m11 = xScale;
            result.m12 = 0;
            result.m13 = 0;
            result.m14 = 0;
            result.m21 = 0;
            result.m22 = yScale;
            result.m23 = 0;
            result.m24 = 0;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = zScale;
            result.m34 = 0;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个缩放矩阵
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <param name="result">返回一个矩阵</param>
        private static void CreateScale(FLOAT xScale, FLOAT yScale, FLOAT zScale, out Matrix4x4 result) {
            result.m11 = xScale;
            result.m12 = 0;
            result.m13 = 0;
            result.m14 = 0;
            result.m21 = 0;
            result.m22 = yScale;
            result.m23 = 0;
            result.m24 = 0;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = zScale;
            result.m34 = 0;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;
        }

        /// <summary>
        /// 创建一个缩放矩阵
        /// </summary>
        /// <param name="scales">缩放</param>
        /// <param name="result">返回一个矩阵</param>
        private static void CreateScale(ref Vector3 scales, out Matrix4x4 result) {
            CreateScale(scales.x, scales.y, scales.z, out result);
        }

        /// <summary>
        /// 创建一个缩放矩阵，并指定缩放的中心点
        /// </summary>
        /// <param name="scales">缩放</param>
        /// <param name="centerPoint">中心点</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateScale(Vector3 scales, Vector3 centerPoint) {
            CreateScale(scales.x, scales.y, scales.z, ref centerPoint, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个缩放矩阵，并指定缩放的中心点
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <param name="centerPoint">中心点</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateScale(FLOAT xScale, FLOAT yScale, FLOAT zScale, Vector3 centerPoint) {
            Matrix4x4 result;

            var tx = centerPoint.x * (1 - xScale);
            var ty = centerPoint.y * (1 - yScale);
            var tz = centerPoint.z * (1 - zScale);

            result.m11 = xScale;
            result.m12 = 0;
            result.m13 = 0;
            result.m14 = tx;
            result.m21 = 0;
            result.m22 = yScale;
            result.m23 = 0;
            result.m24 = ty;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = zScale;
            result.m34 = tz;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个缩放矩阵，并指定缩放的中心点
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <param name="centerPoint">中心点</param>
        /// <param name="result">返回一个矩阵</param>
        private static void CreateScale(FLOAT xScale, FLOAT yScale, FLOAT zScale, ref Vector3 centerPoint,
            out Matrix4x4 result) {
            var tx = centerPoint.x * (1 - xScale);
            var ty = centerPoint.y * (1 - yScale);
            var tz = centerPoint.z * (1 - zScale);

            result.m11 = xScale;
            result.m12 = 0;
            result.m13 = 0;
            result.m14 = tx;
            result.m21 = 0;
            result.m22 = yScale;
            result.m23 = 0;
            result.m24 = ty;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = zScale;
            result.m34 = tz;
            result.m41 = 0;
            result.m42 = 0;
            result.m43 = 0;
            result.m44 = 1;
        }

        /// <summary>
        /// 创建一个缩放矩阵，并指定缩放的中心点
        /// </summary>
        /// <param name="scales">缩放</param>
        /// <param name="centerPoint">中心点</param>
        /// <param name="result">返回一个矩阵</param>
        private static void CreateScale(ref Vector3 scales, ref Vector3 centerPoint, out Matrix4x4 result) {
            CreateScale(scales.x, scales.y, scales.z, ref centerPoint, out result);
        }

        /// <summary>
        ///  创建一个X轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateRotateX(FLOAT angle) {
            Matrix4x4 result;

            var rad = angle * Math.Deg2Rad;
            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            // [  1  0  0  0 ]
            // [  0  c  s  0 ]
            // [  0 -s  c  0 ]
            // [  0  0  0  1 ]
            result.m11 = 1;
            result.m21 = 0;
            result.m31 = 0;
            result.m41 = 0;
            result.m12 = 0;
            result.m22 = c;
            result.m32 = -s;
            result.m42 = 0;
            result.m13 = 0;
            result.m23 = s;
            result.m33 = c;
            result.m43 = 0;
            result.m14 = 0;
            result.m24 = 0;
            result.m34 = 0;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个X轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="centerPoint">中心点</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateRotateX(FLOAT angle, Vector3 centerPoint) {
            Matrix4x4 result;

            var rad = angle * Math.Deg2Rad;
            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            var y = centerPoint.y * (1 - c) + centerPoint.z * s;
            var z = centerPoint.z * (1 - c) - centerPoint.y * s;

            // [  1  0  0  0 ]
            // [  0  c  s  y ]
            // [  0 -s  c  z ]
            // [  0  0  0  1 ]
            result.m11 = 1;
            result.m21 = 0;
            result.m31 = 0;
            result.m41 = 0;
            result.m12 = 0;
            result.m22 = c;
            result.m32 = -s;
            result.m42 = 0;
            result.m13 = 0;
            result.m23 = s;
            result.m33 = c;
            result.m43 = 0;
            result.m14 = 0;
            result.m24 = y;
            result.m34 = z;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个Y轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateRotateY(FLOAT angle) {
            Matrix4x4 result;

            var rad = angle * Math.Deg2Rad;
            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            // [  c  0 -s  0 ]
            // [  0  1  0  0 ]
            // [  s  0  c  0 ]
            // [  0  0  0  1 ]
            result.m11 = c;
            result.m21 = 0;
            result.m31 = s;
            result.m41 = 0;
            result.m12 = 0;
            result.m22 = 1;
            result.m32 = 0;
            result.m42 = 0;
            result.m13 = -s;
            result.m23 = 0;
            result.m33 = c;
            result.m43 = 0;
            result.m14 = 0;
            result.m24 = 0;
            result.m34 = 0;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个Y轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="centerPoint">中心点</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateRotateY(FLOAT angle, Vector3 centerPoint) {
            Matrix4x4 result;

            var rad = angle * Math.Deg2Rad;
            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            var x = centerPoint.x * (1 - c) - centerPoint.z * s;
            var z = centerPoint.x * (1 - c) + centerPoint.x * s;

            // [  c  0 -s  x ]
            // [  0  1  0  0 ]
            // [  s  0  c  z ]
            // [  0  0  0  1 ]
            result.m11 = c;
            result.m21 = 0;
            result.m31 = s;
            result.m41 = 0;
            result.m12 = 0;
            result.m22 = 1;
            result.m32 = 0;
            result.m42 = 0;
            result.m13 = -s;
            result.m23 = 0;
            result.m33 = c;
            result.m43 = 0;
            result.m14 = x;
            result.m24 = 0;
            result.m34 = z;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个Z轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateRotateZ(FLOAT angle) {
            Matrix4x4 result;

            var rad = angle * Math.Deg2Rad;
            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            // [  c  s  0  0 ]
            // [ -s  c  0  0 ]
            // [  0  0  1  0 ]
            // [  0  0  0  1 ]
            result.m11 = c;
            result.m21 = -s;
            result.m31 = 0;
            result.m41 = 0;
            result.m12 = s;
            result.m22 = c;
            result.m32 = 0;
            result.m42 = 0;
            result.m13 = 0;
            result.m23 = 0;
            result.m33 = 1;
            result.m43 = 0;
            result.m14 = 0;
            result.m24 = 0;
            result.m34 = 0;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个Z轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="centerPoint">中心点</param>
        /// <returns>返回一个矩阵</returns>
        private static Matrix4x4 CreateRotateZ(FLOAT angle, Vector3 centerPoint) {
            Matrix4x4 result;

            var rad = angle * Math.Deg2Rad;
            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            var x = centerPoint.x * (1 - c) + centerPoint.y * s;
            var y = centerPoint.y * (1 - c) - centerPoint.x * s;

            // [  c  s  0  x ]
            // [ -s  c  0  y ]
            // [  0  0  1  0 ]
            // [  0  0  0  1 ]
            result.m11 = c;
            result.m21 = -s;
            result.m31 = 0;
            result.m41 = 0;
            result.m12 = s;
            result.m22 = c;
            result.m32 = 0;
            result.m42 = 0;
            result.m13 = 0;
            result.m23 = 0;
            result.m33 = 1;
            result.m43 = 0;
            result.m14 = x;
            result.m24 = y;
            result.m34 = 0;
            result.m44 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个轴角度的矩阵
        /// </summary>
        /// <param name="axis">轴</param>
        /// <param name="angle">角度</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix4x4 CreateAngleAxis(Vector3 axis, FLOAT angle) {
            CreateAngleAxis(ref axis, angle, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个轴角度的矩阵
        /// </summary>
        /// <param name="axis">轴</param>
        /// <param name="angle">角度</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateAngleAxis(ref Vector3 axis, FLOAT angle, out Matrix4x4 result) {
            /* https://blog.csdn.net/weixin_49299600/article/details/124181903 这里面包含完整的公式推导过程
             * 下面是额外拓展的二维旋转矩阵推导，根据上面连接中的图示来看
             * od = cos * |oc| * oc^ + sin * |oc| * oe^
             * od = cos * oc + sin * oa x oc，因为oa作为z轴，所以oa = (0,0,1) = [0 -1  0]
             *                                                               [1  0  0]
             *                                                               [0  0  0]
             * od = cos * op + sin * op x oc，转成矩阵形式就是
             * od = cos * [x] + sin * [0 -1  0] * [x]
             *            [y]         [1  0  0]   [y]
             *            [z]         [0  0  0]   [z]
             * 提取op后
             * od = (cos * I + sin * [0 -1  0]) * [x]
             *                       [1  0  0]    [y]
             *                       [0  0  0]    [z]
             * M = [cos  -sin  0  ]
             *     [sin   cos  0  ]
             *     [0     0    cos]
             * 但由于是二维，所以z轴，始终为0，所以
             * M = [cos  -sin]
             *     [sin   cos]
             */

            /* 
             * 下面是简要的推导，详细可以去看上面的连接
             * Rotation matrix M can compute by using below equation.
             *
             *  M = n * nT + cosθ * (I - n * nT) + sinθ * S --- 这是罗德里格斯公式
             * 上面 n代表任意旋转轴；nT表示n的转置；I表示单位矩阵；S表示n的叉乘矩阵
             *
             * Where:
             *
             *      [ x ]
             *  n = [ y ]
             *      [ z ]
             *
             *      [  0 -z  y ]
             *  S = [  z  0 -x ]
             *      [ -y  x  0 ]
             *
             *      [ 1 0 0 ]
             *  I = [ 0 1 0 ]
             *      [ 0 0 1 ]
             *
             *     [ xx + cosθ * (1 - xx)          yx - cosθ * yx - sinθ * z     zx - cosθ * xz + sinθ * y ]
             * M = [ xy - cosθ * yx + sinθ * z     yy + cosθ(1 - yy)             yz - cosθ * yz - sinθ * x ]
             *     [ zx - cosθ * zx - sinθ * y     zy - cosθ * zy + sinθ * x     zz + cosθ * (1 - zz)      ]
             * 
             */

            var normalAxis = axis.normalized;
            if (normalAxis.IsZero()) {
                throw new Exception($"<轴角旋转中，轴不可为0> '{normalAxis}'");
            }

            var rad = angle * Math.Deg2Rad;

            FLOAT x = normalAxis.x, y = normalAxis.y, z = normalAxis.z;
            FLOAT sa = Math.Sin(rad), ca = Math.Cos(rad);
            FLOAT xx = x * x, yy = y * y, zz = z * z;
            FLOAT xy = x * y, xz = x * z, yz = y * z;

            result.m11 = xx + ca * (1 - xx);
            result.m21 = xy - ca * xy + sa * z;
            result.m31 = xz - ca * xz - sa * y;
            result.m41 = 0;
            result.m12 = xy - ca * xy - sa * z;
            result.m22 = yy + ca * (1 - yy);
            result.m32 = yz - ca * yz + sa * x;
            result.m42 = 0;
            result.m13 = xz - ca * xz + sa * y;
            result.m23 = yz - ca * yz - sa * x;
            result.m33 = zz + ca * (1 - zz);
            result.m43 = 0;
            result.m14 = 0;
            result.m24 = 0;
            result.m34 = 0;
            result.m44 = 1;
        }

        /// <summary>
        /// 创建一个看向目标的矩阵
        /// </summary>
        /// <param name="from">自己位置</param>
        /// <param name="to">看向的位置</param>
        /// <param name="up">自己的Up轴</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix4x4 CreateLookAt(Vector3 from, Vector3 to, Vector3 up) {
            CreateLookAt(ref from, ref to, ref up, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个看向目标的矩阵（这是后来改的方法，和微软、等都不一样，和unity的计算结果保持一致）
        /// </summary>
        /// <param name="from">自己位置</param>
        /// <param name="to">看向的位置</param>
        /// <param name="up">自己的Up轴</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateLookAt(ref Vector3 from, ref Vector3 to, ref Vector3 up,
            out Matrix4x4 result) {
            /* 
             * 旋转矩阵的推导（指向型）
             * https://blog.csdn.net/weixin_49299600/article/details/124181903
             * 
             * u = x^ = cross(up, dir).normalize
             * v = y^ = cross(dir, u).normalize
             * w = z^ = dir.normalize
             * u v w代表 x y z三个轴方向上的单位向量
             * 核心思路是，世界坐标系下任意点在局部坐标系下，可以表示为该点在各个轴上的投影长度，其中轴为单位向量 u v w
             * 
             *     [u.x   u.y   u.z]
             * M = [v.x   v.y   v.z]
             *     [w.x   w.y   w.z]
             * 
             * 取逆后
             * 
             *     [u.x   v.x   w.x]
             * M = [u.y   v.y   w.y]
             *     [u.z   v.z   w.z]
             * 
             *
             * [u.x  v.x  w.x  from.x]
             * [u.y  v.y  w.y  from.y]
             * [u.z  v.z  w.z  from.z]
             * [0    0    0    1     ]
             */

            if (up.IsZero()) {
                throw new Exception($"<CreateLookAt方法参数中，up可为0> '{up}'");
            }

            var w = to - from;
            w.Normalize();
            if (w.IsZero()) {
                throw new Exception($"<CreateLookAt方法参数中，方向不可为0> '{w}'");
            }

            Vector3.Cross(ref up, ref w, out var u);
            u.Normalize();
            Vector3.Cross(ref w, ref u, out var v);

            result.m11 = u.x;
            result.m21 = u.y;
            result.m31 = u.z;
            result.m41 = 0;
            result.m12 = v.x;
            result.m22 = v.y;
            result.m32 = v.z;
            result.m42 = 0;
            result.m13 = w.x;
            result.m23 = w.y;
            result.m33 = w.z;
            result.m43 = 0;
            result.m14 = from.x;
            result.m24 = from.y;
            result.m34 = from.z;
            result.m44 = 1;
        }

        /// <summary>
        /// Creates directional light shadow matrix that flattens geometry into a plane.
        /// </summary>
        /// <param name="shadowPlane">Projection plane</param>
        /// <param name="dirLightOppositeDirection">Light source is a directional light and parameter contains
        /// opposite direction of directional light (e.g. if light direction is L, caller must pass -L as a parameter)</param>
        /// <param name="result"></param>
        private static void CreateShadowDirectional(ref Plane shadowPlane, ref Vector3 dirLightOppositeDirection, out Matrix4x4 result) {
            Vector3 normal = shadowPlane.normal;
            FLOAT constant = shadowPlane.constant;
            FLOAT num = normal.x * dirLightOppositeDirection.x + normal.y * dirLightOppositeDirection.y + normal.z * dirLightOppositeDirection.z;
            result.m11 = num - dirLightOppositeDirection.x * normal.x;
            result.m12 = (0f - dirLightOppositeDirection.x) * normal.y;
            result.m13 = (0f - dirLightOppositeDirection.x) * normal.z;
            result.m14 = dirLightOppositeDirection.x * constant;
            result.m21 = (0f - dirLightOppositeDirection.y) * normal.x;
            result.m22 = num - dirLightOppositeDirection.y * normal.y;
            result.m23 = (0f - dirLightOppositeDirection.y) * normal.z;
            result.m24 = dirLightOppositeDirection.y * constant;
            result.m31 = (0f - dirLightOppositeDirection.z) * normal.x;
            result.m32 = (0f - dirLightOppositeDirection.z) * normal.y;
            result.m33 = num - dirLightOppositeDirection.z * normal.z;
            result.m34 = dirLightOppositeDirection.z * constant;
            result.m44 = num;
            result.m41 = 0f;
            result.m42 = 0f;
            result.m43 = 0f;
        }

        /// <summary>
        /// Creates point light shadow matrix that flattens geometry into a plane.
        /// </summary>
        /// <param name="shadowPlane">Projection plane</param>
        /// <param name="pointLightPosition">Light source is a point light and parameter contains
        /// position of a point light</param>
        /// <param name="result"></param>
        private static void CreateShadowPoint(ref Plane shadowPlane, ref Vector3 pointLightPosition, out Matrix4x4 result) {
            Vector3 normal = shadowPlane.normal;
            FLOAT constant = shadowPlane.constant;
            FLOAT num = normal.x * pointLightPosition.x + normal.y * pointLightPosition.y + normal.z * pointLightPosition.z;
            result.m11 = num + pointLightPosition.x * normal.x - constant;
            result.m12 = (0f - pointLightPosition.x) * normal.y;
            result.m13 = (0f - pointLightPosition.x) * normal.z;
            result.m14 = pointLightPosition.x * constant;
            result.m21 = (0f - pointLightPosition.y) * normal.x;
            result.m22 = num - pointLightPosition.y * normal.y - constant;
            result.m23 = (0f - pointLightPosition.y) * normal.z;
            result.m24 = pointLightPosition.y * constant;
            result.m31 = (0f - pointLightPosition.z) * normal.x;
            result.m32 = (0f - pointLightPosition.z) * normal.y;
            result.m33 = num - pointLightPosition.z * normal.z - constant;
            result.m34 = pointLightPosition.z * constant;
            result.m41 = 0f - normal.x;
            result.m42 = 0f - normal.y;
            result.m43 = 0f - normal.z;
            result.m44 = num;
        }

        /// <summary>
        /// Creates a generic shadow matrix that flattens geometry into a plane.
        /// </summary>
        /// <param name="shadowPlane">Projection plane</param>
        /// <param name="lightData">If w component is 0.0f, then light source is directional light and
        /// x,y,z components contain opposite direction of directional light. If w component is 1.0f
        /// then source is point light and x,y,z components contain position of point light.</param>
        /// <param name="result"></param>
        private static void CreateShadow(ref Plane shadowPlane, ref Vector4 lightData, out Matrix4x4 result) {
            Vector3 normal = shadowPlane.normal;
            FLOAT constant = shadowPlane.constant;
            FLOAT num = normal.x * lightData.x + normal.y * lightData.y + normal.z * lightData.z;
            result.m11 = num + lightData.x * normal.x - constant * lightData.w;
            result.m12 = (0f - lightData.x) * normal.y;
            result.m13 = (0f - lightData.x) * normal.z;
            result.m14 = lightData.x * constant;
            result.m21 = (0f - lightData.y) * normal.x;
            result.m22 = num - lightData.y * normal.y - constant * lightData.w;
            result.m23 = (0f - lightData.y) * normal.z;
            result.m24 = lightData.y * constant;
            result.m31 = (0f - lightData.z) * normal.x;
            result.m32 = (0f - lightData.z) * normal.y;
            result.m33 = num - lightData.z * normal.z - constant * lightData.w;
            result.m34 = lightData.z * constant;
            result.m41 = (0f - normal.x) * lightData.w;
            result.m42 = (0f - normal.y) * lightData.w;
            result.m43 = (0f - normal.z) * lightData.w;
            result.m44 = num;
        }

        /// <summary>
        /// 创建一个平移、旋转、缩放组合的矩阵
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="scale">缩放</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix4x4 CreateTRS(Vector3 position, Quaternion rotation, Vector3 scale) {
            CreateTRS(ref position, ref rotation, ref scale, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个平移、旋转、缩放组合的矩阵
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="scale">缩放</param>
        /// <param name="matrix">返回一个矩阵</param>
        public static void CreateTRS(ref Vector3 position, ref Quaternion rotation, ref Vector3 scale,
            out Matrix4x4 matrix) {
            CreateRotation(ref rotation, out matrix);
            matrix.m11 *= scale.x;
            matrix.m21 *= scale.x;
            matrix.m31 *= scale.x;
            matrix.m12 *= scale.y;
            matrix.m22 *= scale.y;
            matrix.m32 *= scale.y;
            matrix.m13 *= scale.z;
            matrix.m23 *= scale.z;
            matrix.m33 *= scale.z;
            matrix.m14 = position.x;
            matrix.m24 = position.y;
            matrix.m34 = position.z;
        }

        /// <summary>
        /// Creates a transformation matrix. Transformation order is: scaling, moving to rotation origin, rotation, moving to translation point.
        /// </summary>
        private static void CreateTRS(ref Vector3 position, ref Vector3 rotationOrigin, ref Quaternion rotation, ref Vector3 scale,
            out Matrix4x4 result) {
            CreateRotation(ref rotation, out result);
            result.m14 = 0f - (result.m11 * rotationOrigin.x + result.m12 * rotationOrigin.y + result.m13 * rotationOrigin.z - rotationOrigin.x) +
                         position.x;
            result.m24 = 0f - (result.m21 * rotationOrigin.x + result.m22 * rotationOrigin.y + result.m23 * rotationOrigin.z - rotationOrigin.y) +
                         position.y;
            result.m34 = 0f - (result.m31 * rotationOrigin.x + result.m32 * rotationOrigin.y + result.m33 * rotationOrigin.z - rotationOrigin.z) +
                         position.z;
            result.m11 *= scale.x;
            result.m21 *= scale.x;
            result.m31 *= scale.x;
            result.m12 *= scale.y;
            result.m22 *= scale.y;
            result.m32 *= scale.y;
            result.m13 *= scale.z;
            result.m23 *= scale.z;
            result.m33 *= scale.z;
        }

        /// <summary>
        /// Creates a transformation matrix. Transformation order is: rotation, translation.
        /// </summary>
        public static void CreateTR(ref Vector3 position, ref Quaternion rotation, out Matrix4x4 result) {
            CreateRotation(ref rotation, out result);
            result.m14 = position.x;
            result.m24 = position.y;
            result.m34 = position.z;
        }

        /// <summary>
        /// Creates a transformation matrix. Transformation order is: moving to rotation origin, rotation, moving to translation point.
        /// </summary>
        private static void CreateTR(ref Vector3 position, ref Vector3 rotationOrigin, ref Quaternion rotation,
            out Matrix4x4 result) {
            CreateRotation(ref rotation, out result);
            result.m14 = 0f -
                (result.m11 * rotationOrigin.x + result.m12 * rotationOrigin.y + result.m13 * rotationOrigin.z -
                 rotationOrigin.x) + position.x;
            result.m24 = 0f -
                (result.m21 * rotationOrigin.x + result.m22 * rotationOrigin.y + result.m23 * rotationOrigin.z -
                 rotationOrigin.y) + position.y;
            result.m34 = 0f -
                (result.m31 * rotationOrigin.x + result.m32 * rotationOrigin.y + result.m33 * rotationOrigin.z -
                 rotationOrigin.z) + position.z;
        }

        /// <summary>
        /// Creates a transformation matrix. Transformation includes scaling and translation (order is unimportant).
        /// </summary>
        public static void CreateTS(ref Vector3 position, ref Vector3 scale, out Matrix4x4 result) {
            result.m11 = scale.x;
            result.m22 = scale.y;
            result.m33 = scale.z;
            result.m14 = position.x;
            result.m24 = position.y;
            result.m34 = position.z;
            result.m12 = 0f;
            result.m13 = 0f;
            result.m21 = 0f;
            result.m23 = 0f;
            result.m31 = 0f;
            result.m32 = 0f;
            result.m41 = 0f;
            result.m42 = 0f;
            result.m43 = 0f;
            result.m44 = 1f;
        }

        /// <summary>
        /// 不了解
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="zNearPlane"></param>
        /// <param name="zFarPlane"></param>
        /// <returns></returns>
        private static Matrix4x4 CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane) {
            Matrix4x4 matrix44;

            matrix44.m11 = 2f / width;
            matrix44.m21 = matrix44.m31 = matrix44.m41 = 0.0f;
            matrix44.m22 = 2f / height;
            matrix44.m12 = matrix44.m32 = matrix44.m42 = 0.0f;
            matrix44.m33 = (float)(1.0 / ((double)zNearPlane - zFarPlane));
            matrix44.m13 = matrix44.m23 = matrix44.m43 = 0.0f;
            matrix44.m14 = matrix44.m24 = 0.0f;
            matrix44.m34 = zNearPlane / (zNearPlane - zFarPlane);
            matrix44.m44 = 1f;

            return matrix44;
        }

        #endregion

        #region -------------其他矩阵静态函数

        /// <summary>
        /// 绝对矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix4x4 Abs(Matrix4x4 matrix) {
            Abs(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// 绝对矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void Abs(ref Matrix4x4 matrix, out Matrix4x4 result) {
            var m11 = Math.Abs(matrix.m11);
            var m12 = Math.Abs(matrix.m12);
            var m13 = Math.Abs(matrix.m13);
            var m14 = Math.Abs(matrix.m14);
            var m21 = Math.Abs(matrix.m21);
            var m22 = Math.Abs(matrix.m22);
            var m23 = Math.Abs(matrix.m23);
            var m24 = Math.Abs(matrix.m24);
            var m31 = Math.Abs(matrix.m31);
            var m32 = Math.Abs(matrix.m32);
            var m33 = Math.Abs(matrix.m33);
            var m34 = Math.Abs(matrix.m34);
            var m41 = Math.Abs(matrix.m31);
            var m42 = Math.Abs(matrix.m32);
            var m43 = Math.Abs(matrix.m33);
            var m44 = Math.Abs(matrix.m34);

            result.m11 = m11;
            result.m21 = m21;
            result.m31 = m31;
            result.m41 = m41;
            result.m12 = m12;
            result.m22 = m22;
            result.m32 = m32;
            result.m42 = m42;
            result.m13 = m13;
            result.m23 = m23;
            result.m33 = m33;
            result.m43 = m43;
            result.m14 = m14;
            result.m24 = m24;
            result.m34 = m34;
            result.m44 = m44;
        }

        /// <summary>
        /// 逆矩阵 - 当矩阵的平移为0、缩放为1时，矩阵的逆矩阵 = 矩阵的转置
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix4x4 Invert(Matrix4x4 matrix) {
            Invert(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// 逆矩阵 - 当矩阵的平移为0、缩放为1时，矩阵的逆矩阵 = 矩阵的转置
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void Invert(ref Matrix4x4 matrix, out Matrix4x4 result) {
            //                                       -1
            // If you have matrix M, inverse Matrix M   can compute
            //
            //     -1       1      
            //    M   = --------- A
            //            det(M)
            //
            // A is adjugate (adjoint) of M, where,
            //
            //      T
            // A = C
            //
            // C is Cofactor matrix of M, where,
            //           i + j
            // C   = (-1)      * det(M  )
            //  ij                    ij
            //
            //     [ a b c d ]
            // M = [ e f g h ]
            //     [ i j k l ]
            //     [ m n o p ]
            //
            // First Row
            //           2 | f g h |
            // C   = (-1)  | j k l | = + ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
            //  11         | n o p |
            //
            //           3 | e g h |
            // C   = (-1)  | i k l | = - ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
            //  12         | m o p |
            //
            //           4 | e f h |
            // C   = (-1)  | i j l | = + ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
            //  13         | m n p |
            //
            //           5 | e f g |
            // C   = (-1)  | i j k | = - ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
            //  14         | m n o |
            //
            // Second Row
            //           3 | b c d |
            // C   = (-1)  | j k l | = - ( b ( kp - lo ) - c ( jp - ln ) + d ( jo - kn ) )
            //  21         | n o p |
            //
            //           4 | a c d |
            // C   = (-1)  | i k l | = + ( a ( kp - lo ) - c ( ip - lm ) + d ( io - km ) )
            //  22         | m o p |
            //
            //           5 | a b d |
            // C   = (-1)  | i j l | = - ( a ( jp - ln ) - b ( ip - lm ) + d ( in - jm ) )
            //  23         | m n p |
            //
            //           6 | a b c |
            // C   = (-1)  | i j k | = + ( a ( jo - kn ) - b ( io - km ) + c ( in - jm ) )
            //  24         | m n o |
            //
            // Third Row
            //           4 | b c d |
            // C   = (-1)  | f g h | = + ( b ( gp - ho ) - c ( fp - hn ) + d ( fo - gn ) )
            //  31         | n o p |
            //
            //           5 | a c d |
            // C   = (-1)  | e g h | = - ( a ( gp - ho ) - c ( ep - hm ) + d ( eo - gm ) )
            //  32         | m o p |
            //
            //           6 | a b d |
            // C   = (-1)  | e f h | = + ( a ( fp - hn ) - b ( ep - hm ) + d ( en - fm ) )
            //  33         | m n p |
            //
            //           7 | a b c |
            // C   = (-1)  | e f g | = - ( a ( fo - gn ) - b ( eo - gm ) + c ( en - fm ) )
            //  34         | m n o |
            //
            // Fourth Row
            //           5 | b c d |
            // C   = (-1)  | f g h | = - ( b ( gl - hk ) - c ( fl - hj ) + d ( fk - gj ) )
            //  41         | j k l |
            //
            //           6 | a c d |
            // C   = (-1)  | e g h | = + ( a ( gl - hk ) - c ( el - hi ) + d ( ek - gi ) )
            //  42         | i k l |
            //
            //           7 | a b d |
            // C   = (-1)  | e f h | = - ( a ( fl - hj ) - b ( el - hi ) + d ( ej - fi ) )
            //  43         | i j l |
            //
            //           8 | a b c |
            // C   = (-1)  | e f g | = + ( a ( fk - gj ) - b ( ek - gi ) + c ( ej - fi ) )
            //  44         | i j k |
            //
            // Cost of operation
            // 53 adds, 104 muls, and 1 div.
            FLOAT num11 = matrix.m11, num12 = matrix.m12, num13 = matrix.m13, num14 = matrix.m14;
            FLOAT num21 = matrix.m21, num22 = matrix.m22, num23 = matrix.m23, num24 = matrix.m24;
            FLOAT num31 = matrix.m31, num32 = matrix.m32, num33 = matrix.m33, num34 = matrix.m34;
            FLOAT num41 = matrix.m41, num42 = matrix.m42, num43 = matrix.m43, num44 = matrix.m44;

            var num33443443 = num33 * num44 - num34 * num43;
            var num32443442 = num32 * num44 - num34 * num42;
            var num32433342 = num32 * num43 - num33 * num42;
            var num31443441 = num31 * num44 - num34 * num41;
            var num31433341 = num31 * num43 - num33 * num41;
            var num31423241 = num31 * num42 - num32 * num41;

            var a11 = (num22 * num33443443 - num23 * num32443442 + num24 * num32433342);
            var a12 = -(num21 * num33443443 - num23 * num31443441 + num24 * num31433341);
            var a13 = (num21 * num32443442 - num22 * num31443441 + num24 * num31423241);
            var a14 = -(num21 * num32433342 - num22 * num31433341 + num23 * num31423241);

            var det = num11 * a11 + num12 * a12 + num13 * a13 + num14 * a14;

            if (det == 0) {
                result.m11 = FLOAT.PositiveInfinity;
                result.m12 = FLOAT.PositiveInfinity;
                result.m13 = FLOAT.PositiveInfinity;
                result.m14 = FLOAT.PositiveInfinity;
                result.m21 = FLOAT.PositiveInfinity;
                result.m22 = FLOAT.PositiveInfinity;
                result.m23 = FLOAT.PositiveInfinity;
                result.m24 = FLOAT.PositiveInfinity;
                result.m31 = FLOAT.PositiveInfinity;
                result.m32 = FLOAT.PositiveInfinity;
                result.m33 = FLOAT.PositiveInfinity;
                result.m34 = FLOAT.PositiveInfinity;
                result.m41 = FLOAT.PositiveInfinity;
                result.m42 = FLOAT.PositiveInfinity;
                result.m43 = FLOAT.PositiveInfinity;
                result.m44 = FLOAT.PositiveInfinity;
            }
            else {
                var invDet = 1 / det;

                result.m11 = a11 * invDet;
                result.m21 = a12 * invDet;
                result.m31 = a13 * invDet;
                result.m41 = a14 * invDet;

                result.m12 = -(num12 * num33443443 - num13 * num32443442 + num14 * num32433342) * invDet;
                result.m22 = (num11 * num33443443 - num13 * num31443441 + num14 * num31433341) * invDet;
                result.m32 = -(num11 * num32443442 - num12 * num31443441 + num14 * num31423241) * invDet;
                result.m42 = (num11 * num32433342 - num12 * num31433341 + num13 * num31423241) * invDet;

                var num23442443 = num23 * num44 - num24 * num43;
                var num22442443 = num22 * num44 - num24 * num42;
                var num22432342 = num22 * num43 - num23 * num42;
                var num21442441 = num21 * num44 - num24 * num41;
                var num21432341 = num21 * num43 - num23 * num41;
                var num21422241 = num21 * num42 - num22 * num41;

                result.m13 = (num12 * num23442443 - num13 * num22442443 + num14 * num22432342) * invDet;
                result.m23 = -(num11 * num23442443 - num13 * num21442441 + num14 * num21432341) * invDet;
                result.m33 = (num11 * num22442443 - num12 * num21442441 + num14 * num21422241) * invDet;
                result.m43 = -(num11 * num22432342 - num12 * num21432341 + num13 * num21422241) * invDet;

                var num23342433 = num23 * num34 - num24 * num33;
                var num22342432 = num22 * num34 - num24 * num32;
                var num22332332 = num22 * num33 - num23 * num32;
                var num21342431 = num21 * num34 - num24 * num31;
                var num21332331 = num21 * num33 - num23 * num31;
                var num21322231 = num21 * num32 - num22 * num31;

                result.m14 = -(num12 * num23342433 - num13 * num22342432 + num14 * num22332332) * invDet;
                result.m24 = (num11 * num23342433 - num13 * num21342431 + num14 * num21332331) * invDet;
                result.m34 = -(num11 * num22342432 - num12 * num21342431 + num14 * num21322231) * invDet;
                result.m44 = (num11 * num22332332 - num12 * num21332331 + num13 * num21322231) * invDet;
            }
        }

        /// <summary>
        /// 转置 - 当矩阵的平移为0、缩放为1时，矩阵的转置 = 矩阵的逆矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix4x4 Transpose(Matrix4x4 matrix) {
            Transpose(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// 转置 - 当矩阵的平移为0、缩放为1时，矩阵的转置 = 矩阵的逆矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void Transpose(ref Matrix4x4 matrix, out Matrix4x4 result) {
            var m11 = matrix.m11;
            var m12 = matrix.m21;
            var m13 = matrix.m31;
            var m14 = matrix.m41;
            var m21 = matrix.m12;
            var m22 = matrix.m22;
            var m23 = matrix.m32;
            var m24 = matrix.m42;
            var m31 = matrix.m13;
            var m32 = matrix.m23;
            var m33 = matrix.m33;
            var m34 = matrix.m43;
            var m41 = matrix.m14;
            var m42 = matrix.m24;
            var m43 = matrix.m34;
            var m44 = matrix.m44;

            result.m11 = m11;
            result.m21 = m21;
            result.m31 = m31;
            result.m41 = m41;
            result.m12 = m12;
            result.m22 = m22;
            result.m32 = m32;
            result.m42 = m42;
            result.m13 = m13;
            result.m23 = m23;
            result.m33 = m33;
            result.m43 = m43;
            result.m14 = m14;
            result.m24 = m24;
            result.m34 = m34;
            result.m44 = m44;
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Matrix4x4 Lerp(Matrix4x4 matrix1, Matrix4x4 matrix2, FLOAT amount) {
            Lerp(ref matrix1, ref matrix2, amount, out var result);
            return result;
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <param name="amount"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void Lerp(ref Matrix4x4 matrix1, ref Matrix4x4 matrix2, FLOAT amount,
            out Matrix4x4 result) {
            var m11 = matrix1.m11 + (matrix2.m11 - matrix1.m11) * amount;
            var m12 = matrix1.m12 + (matrix2.m12 - matrix1.m12) * amount;
            var m13 = matrix1.m13 + (matrix2.m13 - matrix1.m13) * amount;
            var m14 = matrix1.m14 + (matrix2.m14 - matrix1.m14) * amount;
            var m21 = matrix1.m21 + (matrix2.m21 - matrix1.m21) * amount;
            var m22 = matrix1.m22 + (matrix2.m22 - matrix1.m22) * amount;
            var m23 = matrix1.m23 + (matrix2.m23 - matrix1.m23) * amount;
            var m24 = matrix1.m24 + (matrix2.m24 - matrix1.m24) * amount;
            var m31 = matrix1.m31 + (matrix2.m31 - matrix1.m31) * amount;
            var m32 = matrix1.m32 + (matrix2.m32 - matrix1.m32) * amount;
            var m33 = matrix1.m33 + (matrix2.m33 - matrix1.m33) * amount;
            var m34 = matrix1.m34 + (matrix2.m34 - matrix1.m34) * amount;
            var m41 = matrix1.m41 + (matrix2.m41 - matrix1.m41) * amount;
            var m42 = matrix1.m42 + (matrix2.m42 - matrix1.m42) * amount;
            var m43 = matrix1.m43 + (matrix2.m43 - matrix1.m43) * amount;
            var m44 = matrix1.m44 + (matrix2.m44 - matrix1.m44) * amount;

            result.m11 = m11;
            result.m21 = m21;
            result.m31 = m31;
            result.m41 = m41;
            result.m12 = m12;
            result.m22 = m22;
            result.m32 = m32;
            result.m42 = m42;
            result.m13 = m13;
            result.m23 = m23;
            result.m33 = m33;
            result.m43 = m43;
            result.m14 = m14;
            result.m24 = m24;
            result.m34 = m34;
            result.m44 = m44;
        }

        /// <summary>
        /// 根据矩阵变换一个点（通用）
        /// 当m44不为1的时候，用这个，能确保算出的始终是标准的。大多数情况下m44都是为1的
        /// <para>坐标 * 矩阵的意义就是转换坐标系</para>>
        /// <para>【局部坐标】 * 【矩阵】 = 【世界坐标】</para>>
        /// <para>【世界坐标】 * 【逆矩阵】 = 【局部坐标】</para>>
        /// <para>【局部坐标】 定义为 【某坐标 - 矩阵位置】</para>>
        /// <para>【世界坐标】 定义为 【某坐标 - 原点位置】</para>>
        /// <para>形象的解释坐标系转换：1、世界转局部，相当于本来A要顶两个苹果，现在他要跟我混，于是我帮他顶了一个，我俩一起顶两个苹果，是一种拿走，做减法。
        /// 2、局部转世界，相当于A不跟我混了，于是我不帮A顶了，把原来的那个苹果还给他，是一种偿还，做加法。所以世界转局部时，A会减去自己的坐标，旋转也会反着转，
        /// 而局部转世界时，A又会加上我的坐标，旋转也正着转过来</para>>
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 TransformPoint(Matrix4x4 value1, Vector3 point) {
            // matrix是不能乘vector的，只能vector乘matrix，根据数学公式，所以这里乘的顺序是对的
            Vector3 result;
            result.x = value1.m11 * point.x + value1.m12 * point.y + value1.m13 * point.z + value1.m14;
            result.y = value1.m21 * point.x + value1.m22 * point.y + value1.m23 * point.z + value1.m24;
            result.z = value1.m31 * point.x + value1.m32 * point.y + value1.m33 * point.z + value1.m34;
            var num = value1.m41 * point.x + value1.m42 * point.y + value1.m43 * point.z + value1.m44;
            num = 1f / num;
            result.x *= num;
            result.y *= num;
            result.z *= num;
            return result;
        }

        /// <summary>
        /// 根据矩阵变换一个点（通用）
        /// 当m44不为1的时候，用这个，能确保算出的始终是标准的。大多数情况下m44都是为1的
        /// <para>坐标 * 矩阵的意义就是转换坐标系</para>>
        /// <para>【局部坐标】 * 【矩阵】 = 【世界坐标】</para>>
        /// <para>【世界坐标】 * 【逆矩阵】 = 【局部坐标】</para>>
        /// <para>【局部坐标】 定义为 【某坐标 - 矩阵位置】</para>>
        /// <para>【世界坐标】 定义为 【某坐标 - 原点位置】</para>>
        /// <para>形象的解释坐标系转换：1、世界转局部，相当于本来A要顶两个苹果，现在他要跟我混，于是我帮他顶了一个，我俩一起顶两个苹果，是一种拿走，做减法。
        /// 2、局部转世界，相当于A不跟我混了，于是我不帮A顶了，把原来的那个苹果还给他，是一种偿还，做加法。所以世界转局部时，A会减去自己的坐标，旋转也会反着转，
        /// 而局部转世界时，A又会加上我的坐标，旋转也正着转过来</para>>
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public static void TransformPoint(ref Matrix4x4 value1, ref Vector3 point, out Vector3 result) {
            var x = value1.m11 * point.x + value1.m12 * point.y + value1.m13 * point.z + value1.m14;
            var y = value1.m21 * point.x + value1.m22 * point.y + value1.m23 * point.z + value1.m24;
            var z = value1.m31 * point.x + value1.m32 * point.y + value1.m33 * point.z + value1.m34;
            var num = value1.m41 * point.x + value1.m42 * point.y + value1.m43 * point.z + value1.m44;
            num = 1f / num;

            result.x = x * num;
            result.y = y * num;
            result.z = z * num;
        }

        /// <summary>
        /// 根据矩阵变换一个点（快速）
        /// 快速：如果m44为1的时候，可以用这个，因为算出的结果和上面是一样的，且少一些计算量
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 TransformPointFast(Matrix4x4 value1, Vector3 point) {
            Vector3 result;
            result.x = value1.m11 * point.x + value1.m12 * point.y + value1.m13 * point.z + value1.m14;
            result.y = value1.m21 * point.x + value1.m22 * point.y + value1.m23 * point.z + value1.m24;
            result.z = value1.m31 * point.x + value1.m32 * point.y + value1.m33 * point.z + value1.m34;
            return result;
        }

        /// <summary>
        /// 根据矩阵变换一个点（快速）
        /// 快速：如果m44为1的时候，可以用这个，因为算出的结果和上面是一样的，且少一些计算量
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public static void TransformPointFast(ref Matrix4x4 value1, ref Vector3 point, out Vector3 result) {
            result.x = value1.m11 * point.x + value1.m12 * point.y + value1.m13 * point.z + value1.m14;
            result.y = value1.m21 * point.x + value1.m22 * point.y + value1.m23 * point.z + value1.m24;
            result.z = value1.m31 * point.x + value1.m32 * point.y + value1.m33 * point.z + value1.m34;
        }

        /// <summary>
        /// 根据矩阵变换一个方向
        /// 只计算方向，不计算平移
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 TransformDirection(Matrix4x4 value1, Vector3 vector) {
            TransformDirection(ref value1, ref vector, out var result);
            return result;
        }

        /// <summary>
        /// 根据矩阵变换一个方向
        /// 只计算方向，不计算平移
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void TransformDirection(ref Matrix4x4 value1, ref Vector3 vector, out Vector3 result) {
            result.x = value1.m11 * vector.x + value1.m12 * vector.y + value1.m13 * vector.z;
            result.y = value1.m21 * vector.x + value1.m22 * vector.y + value1.m23 * vector.z;
            result.z = value1.m31 * vector.x + value1.m32 * vector.y + value1.m33 * vector.z;
        }

        /// <summary>
        /// 根据矩阵的转置矩阵变换一个方向
        /// 只计算方向，不计算平移
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 TransposedTransformDirection(Matrix4x4 value1, Vector3 vector) {
            TransposedTransformDirection(ref value1, ref vector, out var result);
            return result;
        }

        /// <summary>
        /// 根据矩阵的转置矩阵变换一个方向
        /// 只计算方向，不计算平移
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void TransposedTransformDirection(ref Matrix4x4 value1, ref Vector3 vector,
            out Vector3 result) {
            result.x = value1.m11 * vector.x + value1.m21 * vector.y + value1.m31 * vector.z;
            result.y = value1.m12 * vector.x + value1.m22 * vector.y + value1.m32 * vector.z;
            result.z = value1.m13 * vector.x + value1.m23 * vector.y + value1.m33 * vector.z;
        }

        #endregion

        #region -------------矩阵本地函数

        /// <summary>
        /// 获得行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector4 GetRow(int index) {
            Vector4 result;
            result.x = this[index, 0];
            result.y = this[index, 1];
            result.z = this[index, 2];
            result.w = this[index, 3];
            return result;
        }

        /// <summary>
        /// 设置行
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetRow(int index, Vector4 value) {
            this[index, 0] = value.x;
            this[index, 1] = value.y;
            this[index, 2] = value.z;
            this[index, 3] = value.w;
        }

        /// <summary>
        /// 获得列
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector4 GetColumn(int index) {
            Vector4 result;
            result.x = this[0, index];
            result.y = this[1, index];
            result.z = this[2, index];
            result.w = this[3, index];
            return result;
        }

        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetColumn(int index, Vector4 value) {
            this[0, index] = value.x;
            this[1, index] = value.y;
            this[2, index] = value.z;
            this[3, index] = value.w;
        }

        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetColumn(int index, Vector3 value) {
            this[0, index] = value.x;
            this[1, index] = value.y;
            this[2, index] = value.z;
        }

        /// <summary>
        /// 转置
        /// </summary>
        public void Transpose() {
            // var m111 = this.m11;
            var m112 = this.m21;
            var m113 = this.m31;
            var m114 = this.m41;

            var m121 = this.m12;
            // var m122 = this.m22;
            var m123 = this.m32;
            var m124 = this.m42;

            var m131 = this.m13;
            var m132 = this.m23;
            // var m133 = this.m33;
            var m134 = this.m43;

            var m141 = this.m14;
            var m142 = this.m24;
            var m143 = this.m34;
            // var m144 = this.m44;

            // this.m11 = m111;
            this.m21 = m121;
            this.m31 = m131;
            this.m41 = m141;

            this.m12 = m112;
            // this.m22 = m122;
            this.m32 = m132;
            this.m42 = m142;

            this.m13 = m113;
            this.m23 = m123;
            // this.m33 = m133;
            this.m43 = m143;

            this.m14 = m114;
            this.m24 = m124;
            this.m34 = m134;
            // this.m44 = m144;
        }

        /// <summary>
        /// 取逆
        /// </summary>
        public void Invert() {
            FLOAT num11 = this.m11, num12 = this.m12, num13 = this.m13, num14 = this.m14;
            FLOAT num21 = this.m21, num22 = this.m22, num23 = this.m23, num24 = this.m24;
            FLOAT num31 = this.m31, num32 = this.m32, num33 = this.m33, num34 = this.m34;
            FLOAT num41 = this.m41, num42 = this.m42, num43 = this.m43, num44 = this.m44;

            var num33443443 = num33 * num44 - num34 * num43;
            var num32443442 = num32 * num44 - num34 * num42;
            var num32433342 = num32 * num43 - num33 * num42;
            var num31443441 = num31 * num44 - num34 * num41;
            var num31433341 = num31 * num43 - num33 * num41;
            var num31423241 = num31 * num42 - num32 * num41;

            var a11 = (num22 * num33443443 - num23 * num32443442 + num24 * num32433342);
            var a12 = -(num21 * num33443443 - num23 * num31443441 + num24 * num31433341);
            var a13 = (num21 * num32443442 - num22 * num31443441 + num24 * num31423241);
            var a14 = -(num21 * num32433342 - num22 * num31433341 + num23 * num31423241);

            var det = num11 * a11 + num12 * a12 + num13 * a13 + num14 * a14;

            if (det == 0) {
                this.m11 = FLOAT.PositiveInfinity;
                this.m12 = FLOAT.PositiveInfinity;
                this.m13 = FLOAT.PositiveInfinity;
                this.m14 = FLOAT.PositiveInfinity;
                this.m21 = FLOAT.PositiveInfinity;
                this.m22 = FLOAT.PositiveInfinity;
                this.m23 = FLOAT.PositiveInfinity;
                this.m24 = FLOAT.PositiveInfinity;
                this.m31 = FLOAT.PositiveInfinity;
                this.m32 = FLOAT.PositiveInfinity;
                this.m33 = FLOAT.PositiveInfinity;
                this.m34 = FLOAT.PositiveInfinity;
                this.m41 = FLOAT.PositiveInfinity;
                this.m42 = FLOAT.PositiveInfinity;
                this.m43 = FLOAT.PositiveInfinity;
                this.m44 = FLOAT.PositiveInfinity;
            }
            else {
                var invDet = 1 / det;

                this.m11 = a11 * invDet;
                this.m21 = a12 * invDet;
                this.m31 = a13 * invDet;
                this.m41 = a14 * invDet;

                this.m12 = -(num12 * num33443443 - num13 * num32443442 + num14 * num32433342) * invDet;
                this.m22 = (num11 * num33443443 - num13 * num31443441 + num14 * num31433341) * invDet;
                this.m32 = -(num11 * num32443442 - num12 * num31443441 + num14 * num31423241) * invDet;
                this.m42 = (num11 * num32433342 - num12 * num31433341 + num13 * num31423241) * invDet;

                var num23442443 = num23 * num44 - num24 * num43;
                var num22442443 = num22 * num44 - num24 * num42;
                var num22432342 = num22 * num43 - num23 * num42;
                var num21442441 = num21 * num44 - num24 * num41;
                var num21432341 = num21 * num43 - num23 * num41;
                var num21422241 = num21 * num42 - num22 * num41;

                this.m13 = (num12 * num23442443 - num13 * num22442443 + num14 * num22432342) * invDet;
                this.m23 = -(num11 * num23442443 - num13 * num21442441 + num14 * num21432341) * invDet;
                this.m33 = (num11 * num22442443 - num12 * num21442441 + num14 * num21422241) * invDet;
                this.m43 = -(num11 * num22432342 - num12 * num21432341 + num13 * num21422241) * invDet;

                var num23342433 = num23 * num34 - num24 * num33;
                var num22342432 = num22 * num34 - num24 * num32;
                var num22332332 = num22 * num33 - num23 * num32;
                var num21342431 = num21 * num34 - num24 * num31;
                var num21332331 = num21 * num33 - num23 * num31;
                var num21322231 = num21 * num32 - num22 * num31;

                this.m14 = -(num12 * num23342433 - num13 * num22342432 + num14 * num22332332) * invDet;
                this.m24 = (num11 * num23342433 - num13 * num21342431 + num14 * num21332331) * invDet;
                this.m34 = -(num11 * num22342432 - num12 * num21342431 + num14 * num21322231) * invDet;
                this.m44 = (num11 * num22332332 - num12 * num21332331 + num13 * num21322231) * invDet;
            }
        }

        /// <summary>
        /// 设置矩阵的 TRS，这相当于重新创建一个矩阵
        /// <para>任何时候，都不应该单独的设置矩阵的 T、R、S</para>>
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scl"></param>
        public void SetTRS(Vector3 pos, Quaternion rot, Vector3 scl) {
            CreateTRS(ref pos, ref rot, ref scl, out this);
        }

        /* 经过测试，通过该方法获得 rts，比 position、rotation、scale里的方法要慢一倍左右，至于为什么，我也不清楚，所以暂时禁用该方法
        /// <summary>
        /// 分解
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="qua">旋转</param>
        /// <param name="scl">缩放</param>
        public void Decompose(out FVector3 pos, out FQuaternion qua, out FVector3 scl)
        {
            var identity = Identity;
            var num00 = this[0, 0];
            var num10 = this[1, 0];
            var num20 = this[2, 0];
            var num01 = this[0, 1];
            var num11 = this[1, 1];
            var num21 = this[2, 1];
            var num02 = this[0, 2];
            var num12 = this[1, 2];
            var num22 = this[2, 2];

            var num1 = 1 / FMath.Sqrt(num00 * num00 + num10 * num10 + num20 * num20);
            identity[0, 0] = num00 * num1;
            identity[1, 0] = num10 * num1;
            identity[2, 0] = num20 * num1;
            var num2 = (identity[0, 0] * num01 + identity[1, 0] * num11 + identity[2, 0] * num21);
            identity[0, 1] = num01 - num2 * identity[0, 0];
            identity[1, 1] = num11 - num2 * identity[1, 0];
            identity[2, 1] = num21 - num2 * identity[2, 0];
            var num3 = 1f / FMath.Sqrt(identity[0, 1] * identity[0, 1] + identity[1, 1] * identity[1, 1] +
                identity[2, 1] * identity[2, 1]);
            identity[0, 1] *= num3;
            identity[1, 1] *= num3;
            identity[2, 1] *= num3;
            var num4 = (identity[0, 0] * num02 + identity[1, 0] * num12 + identity[2, 0] * num22);
            identity[0, 2] = num02 - num4 * identity[0, 0];
            identity[1, 2] = num12 - num4 * identity[1, 0];
            identity[2, 2] = num22 - num4 * identity[2, 0];
            var num5 = (identity[0, 1] * num02 + identity[1, 1] * num12 + identity[2, 1] * num22);
            identity[0, 2] -= num5 * identity[0, 1];
            identity[1, 2] -= num5 * identity[1, 1];
            identity[2, 2] -= num5 * identity[2, 1];
            var num6 = 1f / FMath.Sqrt(identity[0, 2] * identity[0, 2] + identity[1, 2] * identity[1, 2] +
                identity[2, 2] * identity[2, 2]);
            identity[0, 2] *= num6;
            identity[1, 2] *= num6;
            identity[2, 2] *= num6;
            if (identity[0, 0] * identity[1, 1] * identity[2, 2] + identity[0, 1] * identity[1, 2] * identity[2, 0] +
                identity[0, 2] * identity[1, 0] * identity[2, 1] - identity[0, 2] * identity[1, 1] * identity[2, 0] -
                identity[0, 1] * identity[1, 0] * identity[2, 2] - identity[0, 0] * identity[1, 2] * identity[2, 1] <
                0.0)
            {
                for (int index1 = 0; index1 < 3; ++index1)
                {
                    for (int index2 = 0; index2 < 3; ++index2)
                    {
                        identity[index1, index2] = -identity[index1, index2];
                    }
                }
            }

            scl = new FVector3((identity[0, 0] * num00 + identity[1, 0] * num10 + identity[2, 0] * num20),
                (identity[0, 1] * num01 + identity[1, 1] * num11 + identity[2, 1] * num21),
                (identity[0, 2] * num02 + identity[1, 2] * num12 + identity[2, 2] * num22));
            qua = FQuaternion.CreateMatrix(identity);
            pos = new FVector3(this[0, 3], this[1, 3], this[2, 3]);
        }
        */

        /// <summary>
        /// 转FMatrix3x3
        /// </summary>
        /// <returns></returns>
        internal Matrix3x3 ToFMatrix3x3() {
            Matrix3x3 result = default;

            for (int row = 0; row < 3; row++) {
                for (int column = 0; column < 3; column++) {
                    result[row, column] = this[row, column];
                }
            }

            return result;
        }

        #endregion

        #region -------------矩阵运算函数

        public bool Equals(Matrix4x4 other) {
            return Math.CompareApproximate(this.m11, other.m11) &&
                   Math.CompareApproximate(this.m12, other.m12) &&
                   Math.CompareApproximate(this.m13, other.m13) &&
                   Math.CompareApproximate(this.m14, other.m14) &&
                   Math.CompareApproximate(this.m21, other.m21) &&
                   Math.CompareApproximate(this.m22, other.m22) &&
                   Math.CompareApproximate(this.m23, other.m23) &&
                   Math.CompareApproximate(this.m24, other.m24) &&
                   Math.CompareApproximate(this.m31, other.m31) &&
                   Math.CompareApproximate(this.m32, other.m32) &&
                   Math.CompareApproximate(this.m33, other.m33) &&
                   Math.CompareApproximate(this.m34, other.m44) &&
                   Math.CompareApproximate(this.m41, other.m41) &&
                   Math.CompareApproximate(this.m42, other.m42) &&
                   Math.CompareApproximate(this.m43, other.m43) &&
                   Math.CompareApproximate(this.m44, other.m44);
        }

        public override bool Equals(object obj) {
            if (!(obj is Matrix4x4 other)) return false;

            return Math.CompareApproximate(this.m11, other.m11) &&
                   Math.CompareApproximate(this.m12, other.m12) &&
                   Math.CompareApproximate(this.m13, other.m13) &&
                   Math.CompareApproximate(this.m14, other.m14) &&
                   Math.CompareApproximate(this.m21, other.m21) &&
                   Math.CompareApproximate(this.m22, other.m22) &&
                   Math.CompareApproximate(this.m23, other.m23) &&
                   Math.CompareApproximate(this.m24, other.m24) &&
                   Math.CompareApproximate(this.m31, other.m31) &&
                   Math.CompareApproximate(this.m32, other.m32) &&
                   Math.CompareApproximate(this.m33, other.m33) &&
                   Math.CompareApproximate(this.m34, other.m44) &&
                   Math.CompareApproximate(this.m41, other.m41) &&
                   Math.CompareApproximate(this.m42, other.m42) &&
                   Math.CompareApproximate(this.m43, other.m43) &&
                   Math.CompareApproximate(this.m44, other.m44);
        }

        public override int GetHashCode() {
            return this.m11.GetHashCode() ^ this.m12.GetHashCode() ^ this.m13.GetHashCode() ^ this.m14.GetHashCode() ^ this.m21.GetHashCode() ^
                   this.m22.GetHashCode() ^ this.m23.GetHashCode() ^ this.m24.GetHashCode() ^ this.m31.GetHashCode() ^ this.m32.GetHashCode() ^
                   this.m33.GetHashCode() ^ this.m34.GetHashCode() ^ this.m41.GetHashCode() ^ this.m42.GetHashCode() ^ this.m43.GetHashCode() ^
                   this.m44.GetHashCode();
        }

#if FIXED_MATH
        public override string ToString() {
            return $"{m11.AsFloat():F5}\t{m12.AsFloat():F5}\t{m13.AsFloat():F5}\t{m14.AsFloat():F5}" + "\n" +
                   $"{m21.AsFloat():F5}\t{m22.AsFloat():F5}\t{m23.AsFloat():F5}\t{m24.AsFloat():F5}" + "\n" +
                   $"{m31.AsFloat():F5}\t{m32.AsFloat():F5}\t{m33.AsFloat():F5}\t{m34.AsFloat():F5}" + "\n" +
                   $"{m41.AsFloat():F5}\t{m42.AsFloat():F5}\t{m43.AsFloat():F5}\t{m44.AsFloat():F5}" + "\n";
        }
#else
        public override string ToString() {
            return $"{this.m11:F5}\t{this.m12:F5}\t{this.m13:F5}\t{this.m14:F5}" + "\n" +
                   $"{this.m21:F5}\t{this.m22:F5}\t{this.m23:F5}\t{this.m24:F5}" + "\n" +
                   $"{this.m31:F5}\t{this.m32:F5}\t{this.m33:F5}\t{this.m34:F5}" + "\n" +
                   $"{this.m41:F5}\t{this.m42:F5}\t{this.m43:F5}\t{this.m44:F5}" + "\n";
        }
#endif

        public static void Add(ref Matrix4x4 value1, ref Matrix4x4 value2, out Matrix4x4 result) {
            var m11 = value1.m11 + value2.m11;
            var m12 = value1.m12 + value2.m12;
            var m13 = value1.m13 + value2.m13;
            var m14 = value1.m14 + value2.m14;

            var m21 = value1.m21 + value2.m21;
            var m22 = value1.m22 + value2.m22;
            var m23 = value1.m23 + value2.m23;
            var m24 = value1.m24 + value2.m24;

            var m31 = value1.m31 + value2.m31;
            var m32 = value1.m32 + value2.m32;
            var m33 = value1.m33 + value2.m33;
            var m34 = value1.m34 + value2.m34;

            var m41 = value1.m41 + value2.m41;
            var m42 = value1.m42 + value2.m42;
            var m43 = value1.m43 + value2.m43;
            var m44 = value1.m44 + value2.m44;

            result.m11 = m11;
            result.m21 = m21;
            result.m31 = m31;
            result.m41 = m41;
            result.m12 = m12;
            result.m22 = m22;
            result.m32 = m32;
            result.m42 = m42;
            result.m13 = m13;
            result.m23 = m23;
            result.m33 = m33;
            result.m43 = m43;
            result.m14 = m14;
            result.m24 = m24;
            result.m34 = m34;
            result.m44 = m44;
        }

        public static void Subtract(ref Matrix4x4 value1, ref Matrix4x4 value2, out Matrix4x4 result) {
            var m11 = value1.m11 - value2.m11;
            var m12 = value1.m12 - value2.m12;
            var m13 = value1.m13 - value2.m13;
            var m14 = value1.m14 - value2.m14;

            var m21 = value1.m21 - value2.m21;
            var m22 = value1.m22 - value2.m22;
            var m23 = value1.m23 - value2.m23;
            var m24 = value1.m24 - value2.m24;

            var m31 = value1.m31 - value2.m31;
            var m32 = value1.m32 - value2.m32;
            var m33 = value1.m33 - value2.m33;
            var m34 = value1.m34 - value2.m34;

            var m41 = value1.m41 - value2.m41;
            var m42 = value1.m42 - value2.m42;
            var m43 = value1.m43 - value2.m43;
            var m44 = value1.m44 - value2.m44;

            result.m11 = m11;
            result.m21 = m21;
            result.m31 = m31;
            result.m41 = m41;
            result.m12 = m12;
            result.m22 = m22;
            result.m32 = m32;
            result.m42 = m42;
            result.m13 = m13;
            result.m23 = m23;
            result.m33 = m33;
            result.m43 = m43;
            result.m14 = m14;
            result.m24 = m24;
            result.m34 = m34;
            result.m44 = m44;
        }

        public static void Multiply(ref Matrix4x4 value1, ref Matrix4x4 value2, out Matrix4x4 result) {
            var m11 = value1.m11 * value2.m11 + value1.m12 * value2.m21 + value1.m13 * value2.m31 +
                      value1.m14 * value2.m41;
            var m12 = value1.m11 * value2.m12 + value1.m12 * value2.m22 + value1.m13 * value2.m32 +
                      value1.m14 * value2.m42;
            var m13 = value1.m11 * value2.m13 + value1.m12 * value2.m23 + value1.m13 * value2.m33 +
                      value1.m14 * value2.m43;
            var m14 = value1.m11 * value2.m14 + value1.m12 * value2.m24 + value1.m13 * value2.m34 +
                      value1.m14 * value2.m44;

            // Second row
            var m21 = value1.m21 * value2.m11 + value1.m22 * value2.m21 + value1.m23 * value2.m31 +
                      value1.m24 * value2.m41;
            var m22 = value1.m21 * value2.m12 + value1.m22 * value2.m22 + value1.m23 * value2.m32 +
                      value1.m24 * value2.m42;
            var m23 = value1.m21 * value2.m13 + value1.m22 * value2.m23 + value1.m23 * value2.m33 +
                      value1.m24 * value2.m43;
            var m24 = value1.m21 * value2.m14 + value1.m22 * value2.m24 + value1.m23 * value2.m34 +
                      value1.m24 * value2.m44;

            // Third row
            var m31 = value1.m31 * value2.m11 + value1.m32 * value2.m21 + value1.m33 * value2.m31 +
                      value1.m34 * value2.m41;
            var m32 = value1.m31 * value2.m12 + value1.m32 * value2.m22 + value1.m33 * value2.m32 +
                      value1.m34 * value2.m42;
            var m33 = value1.m31 * value2.m13 + value1.m32 * value2.m23 + value1.m33 * value2.m33 +
                      value1.m34 * value2.m43;
            var m34 = value1.m31 * value2.m14 + value1.m32 * value2.m24 + value1.m33 * value2.m34 +
                      value1.m34 * value2.m44;

            // Fourth row
            var m41 = value1.m41 * value2.m11 + value1.m42 * value2.m21 + value1.m43 * value2.m31 +
                      value1.m44 * value2.m41;
            var m42 = value1.m41 * value2.m12 + value1.m42 * value2.m22 + value1.m43 * value2.m32 +
                      value1.m44 * value2.m42;
            var m43 = value1.m41 * value2.m13 + value1.m42 * value2.m23 + value1.m43 * value2.m33 +
                      value1.m44 * value2.m43;
            var m44 = value1.m41 * value2.m14 + value1.m42 * value2.m24 + value1.m43 * value2.m34 +
                      value1.m44 * value2.m44;

            result.m11 = m11;
            result.m21 = m21;
            result.m31 = m31;
            result.m41 = m41;
            result.m12 = m12;
            result.m22 = m22;
            result.m32 = m32;
            result.m42 = m42;
            result.m13 = m13;
            result.m23 = m23;
            result.m33 = m33;
            result.m43 = m43;
            result.m14 = m14;
            result.m24 = m24;
            result.m34 = m34;
            result.m44 = m44;
        }

        public static void Multiply(ref Matrix4x4 value1, ref Vector4 value2, out Vector4 result) {
            result.x = (value1.m11 * value2.x + value1.m12 * value2.y + value1.m13 * value2.z + value1.m14 * value2.w);
            result.y = (value1.m21 * value2.x + value1.m22 * value2.y + value1.m23 * value2.z + value1.m24 * value2.w);
            result.z = (value1.m31 * value2.x + value1.m32 * value2.y + value1.m33 * value2.z + value1.m34 * value2.w);
            result.w = (value1.m41 * value2.x + value1.m42 * value2.y + value1.m43 * value2.z + value1.m44 * value2.w);
        }

        public static void Multiply(ref Matrix4x4 value1, FLOAT value2, out Matrix4x4 result) {
            var m11 = value1.m11 * value2;
            var m12 = value1.m12 * value2;
            var m13 = value1.m13 * value2;
            var m14 = value1.m14 * value2;

            var m21 = value1.m21 * value2;
            var m22 = value1.m22 * value2;
            var m23 = value1.m23 * value2;
            var m24 = value1.m24 * value2;

            var m31 = value1.m31 * value2;
            var m32 = value1.m32 * value2;
            var m33 = value1.m33 * value2;
            var m34 = value1.m34 * value2;

            var m41 = value1.m41 * value2;
            var m42 = value1.m42 * value2;
            var m43 = value1.m43 * value2;
            var m44 = value1.m44 * value2;

            result.m11 = m11;
            result.m21 = m21;
            result.m31 = m31;
            result.m41 = m41;
            result.m12 = m12;
            result.m22 = m22;
            result.m32 = m32;
            result.m42 = m42;
            result.m13 = m13;
            result.m23 = m23;
            result.m33 = m33;
            result.m43 = m43;
            result.m14 = m14;
            result.m24 = m24;
            result.m34 = m34;
            result.m44 = m44;
        }

        public static void Negate(ref Matrix4x4 value, out Matrix4x4 result) {
            var m11 = -value.m11;
            var m12 = -value.m12;
            var m13 = -value.m13;
            var m14 = -value.m14;
            var m21 = -value.m21;
            var m22 = -value.m22;
            var m23 = -value.m23;
            var m24 = -value.m24;
            var m31 = -value.m31;
            var m32 = -value.m32;
            var m33 = -value.m33;
            var m34 = -value.m34;
            var m41 = -value.m41;
            var m42 = -value.m42;
            var m43 = -value.m43;
            var m44 = -value.m44;

            result.m11 = m11;
            result.m21 = m21;
            result.m31 = m31;
            result.m41 = m41;
            result.m12 = m12;
            result.m22 = m22;
            result.m32 = m32;
            result.m42 = m42;
            result.m13 = m13;
            result.m23 = m23;
            result.m33 = m33;
            result.m43 = m43;
            result.m14 = m14;
            result.m24 = m24;
            result.m34 = m34;
            result.m44 = m44;
        }

        public static bool operator ==(Matrix4x4 v1, Matrix4x4 v2) {
            return Math.CompareApproximate(v1.m11, v2.m11) &&
                   Math.CompareApproximate(v1.m12, v2.m12) &&
                   Math.CompareApproximate(v1.m13, v2.m13) &&
                   Math.CompareApproximate(v1.m14, v2.m14) &&
                   Math.CompareApproximate(v1.m21, v2.m21) &&
                   Math.CompareApproximate(v1.m22, v2.m22) &&
                   Math.CompareApproximate(v1.m23, v2.m23) &&
                   Math.CompareApproximate(v1.m24, v2.m24) &&
                   Math.CompareApproximate(v1.m31, v2.m31) &&
                   Math.CompareApproximate(v1.m32, v2.m32) &&
                   Math.CompareApproximate(v1.m33, v2.m33) &&
                   Math.CompareApproximate(v1.m34, v2.m44) &&
                   Math.CompareApproximate(v1.m41, v2.m41) &&
                   Math.CompareApproximate(v1.m42, v2.m42) &&
                   Math.CompareApproximate(v1.m43, v2.m43) &&
                   Math.CompareApproximate(v1.m44, v2.m44);
        }

        public static bool operator !=(Matrix4x4 v1, Matrix4x4 v2) {
            return !Math.CompareApproximate(v1.m11, v2.m11) ||
                   !Math.CompareApproximate(v1.m12, v2.m12) ||
                   !Math.CompareApproximate(v1.m13, v2.m13) ||
                   !Math.CompareApproximate(v1.m14, v2.m14) ||
                   !Math.CompareApproximate(v1.m21, v2.m21) ||
                   !Math.CompareApproximate(v1.m22, v2.m22) ||
                   !Math.CompareApproximate(v1.m23, v2.m23) ||
                   !Math.CompareApproximate(v1.m24, v2.m24) ||
                   !Math.CompareApproximate(v1.m31, v2.m31) ||
                   !Math.CompareApproximate(v1.m32, v2.m32) ||
                   !Math.CompareApproximate(v1.m33, v2.m33) ||
                   !Math.CompareApproximate(v1.m34, v2.m44) ||
                   !Math.CompareApproximate(v1.m41, v2.m41) ||
                   !Math.CompareApproximate(v1.m42, v2.m42) ||
                   !Math.CompareApproximate(v1.m43, v2.m43) ||
                   !Math.CompareApproximate(v1.m44, v2.m44);
        }

        public static Matrix4x4 operator +(Matrix4x4 v1, Matrix4x4 v2) {
            Add(ref v1, ref v2, out var result);
            return result;
        }

        public static Matrix4x4 operator -(Matrix4x4 v) {
            Negate(ref v, out var result);
            return result;
        }

        public static Matrix4x4 operator -(Matrix4x4 v1, Matrix4x4 v2) {
            Subtract(ref v1, ref v2, out var result);
            return result;
        }

        public static Matrix4x4 operator *(Matrix4x4 v1, FLOAT v2) {
            Multiply(ref v1, v2, out var result);
            return result;
        }

        public static Matrix4x4 operator *(FLOAT v2, Matrix4x4 v1) {
            Multiply(ref v1, v2, out var result);
            return result;
        }

        public static Matrix4x4 operator *(Matrix4x4 v1, Matrix4x4 v2) {
            Multiply(ref v1, ref v2, out var result);
            return result;
        }

        public static Vector4 operator *(Matrix4x4 v1, Vector4 v2) {
            Multiply(ref v1, ref v2, out var result);
            return result;
        }

        public static Vector4 operator *(Vector4 v2, Matrix4x4 v1) {
            Multiply(ref v1, ref v2, out var result);
            return result;
        }

        public static Vector3 operator *(Matrix4x4 v1, Vector3 v2) {
            TransformPoint(ref v1, ref v2, out var result);
            return result;
        }

        public static Vector3 operator *(Vector3 v2, Matrix4x4 v1) {
            TransformPoint(ref v1, ref v2, out var result);
            return result;
        }

#if UNITY_5_3_OR_NEWER
        public static implicit operator UnityEngine.Matrix4x4(Matrix4x4 mt)
        {
            return new UnityEngine.Matrix4x4()
            {
                m00 = mt.m11,
                m01 = mt.m12,
                m02 = mt.m13,
                m03 = mt.m14,
                m10 = mt.m21,
                m11 = mt.m22,
                m12 = mt.m23,
                m13 = mt.m24,
                m20 = mt.m31,
                m21 = mt.m32,
                m22 = mt.m33,
                m23 = mt.m34,
                m30 = mt.m41,
                m31 = mt.m42,
                m32 = mt.m43,
                m33 = mt.m44
            };
        }

        public static implicit operator Matrix4x4(UnityEngine.Matrix4x4 mt)
        {
            return new Matrix4x4()
            {
                m11 = mt.m00,
                m12 = mt.m01,
                m13 = mt.m02,
                m14 = mt.m03,
                m21 = mt.m10,
                m22 = mt.m11,
                m23 = mt.m12,
                m24 = mt.m13,
                m31 = mt.m20,
                m32 = mt.m21,
                m33 = mt.m22,
                m34 = mt.m23,
                m41 = mt.m30,
                m42 = mt.m31,
                m43 = mt.m32,
                m44 = mt.m33
            };
        }
#endif

        #endregion
    }
}