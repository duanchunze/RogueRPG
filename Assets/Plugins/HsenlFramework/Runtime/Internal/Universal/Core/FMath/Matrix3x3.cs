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
    public struct Matrix3x3 : IEquatable<Matrix3x3> {
        internal static Matrix3x3 InternalIdentity;
        public static readonly Matrix3x3 Identity;
        public static readonly Matrix3x3 Zero;

        static Matrix3x3() {
            Zero = new Matrix3x3();
            Identity = new Matrix3x3 { m11 = 1, m22 = 1, m33 = 1 };
            InternalIdentity = Identity;
        }

        public FLOAT m11; // 1st row vector
        public FLOAT m12;
        public FLOAT m13;
        public FLOAT m21; // 2nd row vector
        public FLOAT m22;
        public FLOAT m23;
        public FLOAT m31; // 3rd row vector
        public FLOAT m32;
        public FLOAT m33;

        public unsafe FLOAT this[int row, int col] {
            get {
                fixed (FLOAT* numPtr = &this.m11) {
                    return numPtr[row * 3 + col];
                }
            }
            set {
                fixed (FLOAT* numPtr = &this.m11) {
                    numPtr[row * 3 + col] = value;
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

        /// <summary>
        /// 矩阵的行列式
        /// 几何意义：体积，谁的还不清楚
        /// </summary>
        public FLOAT determinant => this.m11 * this.m22 * this.m33 + this.m12 * this.m23 * this.m31 + this.m13 * this.m21 * this.m32 -
                                    this.m31 * this.m22 * this.m13 - this.m32 * this.m23 * this.m11 - this.m33 * this.m21 * this.m12;

        /// <summary>
        /// 矩阵的逆矩阵
        /// </summary>
        public Matrix3x3 inverse {
            get {
                Invert(ref this, out var result);
                return result;
            }
        }

        /// <summary>
        /// 矩阵的转置矩阵
        /// </summary>
        public Matrix3x3 transposition {
            get {
                Transpose(ref this, out var result);
                return result;
            }
        }

        /// <summary>
        /// 矩阵的迹
        /// </summary>
        /// <returns></returns>
        public FLOAT trace => this.m11 + this.m22 + this.m33;

        public Matrix3x3(FLOAT m11, FLOAT m12, FLOAT m13, FLOAT m21, FLOAT m22, FLOAT m23, FLOAT m31, FLOAT m32, FLOAT m33) {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        #region -------------创建矩阵静态函数

        /// <summary>
        /// 创建一个旋转的矩阵，通过欧拉角
        /// </summary>
        /// <param name="euler">旋转欧拉角</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateRotation(Vector3 euler) {
            Quaternion.Euler(ref euler, out var q);
            CreateRotation(ref q, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个旋转的矩阵，通过欧拉角
        /// </summary>
        /// <param name="xEuler"></param>
        /// <param name="yEuler"></param>
        /// <param name="zEuler"></param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateRotation(FLOAT xEuler, FLOAT yEuler, FLOAT zEuler) {
            Quaternion.CreateYawPitchRoll(yEuler, xEuler, zEuler, out var q);
            CreateRotation(ref q, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个旋转的矩阵，通过欧拉角
        /// </summary>
        /// <param name="euler">旋转欧拉角</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateRotation(ref Vector3 euler, out Matrix3x3 result) {
            Quaternion.Euler(ref euler, out var q);
            CreateRotation(ref q, out result);
        }

        /// <summary>
        /// 创建一个旋转的矩阵，通过四元数
        /// </summary>
        /// <param name="quaternion">旋转四元数</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateRotation(Quaternion quaternion) {
            CreateRotation(ref quaternion, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个旋转的矩阵，通过四元数
        /// </summary>
        /// <param name="quaternion">旋转四元数</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateRotation(ref Quaternion quaternion, out Matrix3x3 result) {
            var x = quaternion.x * 2;
            var y = quaternion.y * 2;
            var z = quaternion.z * 2;
            var xx = quaternion.x * x;
            var yy = quaternion.y * y;
            var zz = quaternion.z * z;
            var xy = quaternion.x * y;
            var xz = quaternion.x * z;
            var yz = quaternion.y * z;
            var wx = quaternion.w * x;
            var wy = quaternion.w * y;
            var wz = quaternion.w * z;

            result.m11 = 1 - (yy + zz);
            result.m21 = xy + wz;
            result.m31 = xz - wy;
            result.m12 = xy - wz;
            result.m22 = 1 - (xx + zz);
            result.m32 = yz + wx;
            result.m13 = xz + wy;
            result.m23 = yz - wx;
            result.m33 = 1 - (xx + yy);
        }

        /// <summary>
        /// 创建一个缩放的矩阵
        /// </summary>
        /// <param name="scales">缩放</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateScale(Vector3 scales) {
            CreateScale(scales.x, scales.y, scales.z, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个缩放的矩阵
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateScale(FLOAT xScale, FLOAT yScale, FLOAT zScale) {
            Matrix3x3 result;

            result.m11 = xScale;
            result.m12 = 0;
            result.m13 = 0;
            result.m21 = 0;
            result.m22 = yScale;
            result.m23 = 0;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = zScale;

            return result;
        }

        /// <summary>
        /// 创建一个缩放的矩阵
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateScale(FLOAT xScale, FLOAT yScale, FLOAT zScale, out Matrix3x3 result) {
            result.m11 = xScale;
            result.m12 = 0;
            result.m13 = 0;
            result.m21 = 0;
            result.m22 = yScale;
            result.m23 = 0;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = zScale;
        }

        /// <summary>
        /// 创建一个缩放的矩阵
        /// </summary>
        /// <param name="scales">缩放</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateScale(ref Vector3 scales, out Matrix3x3 result) {
            CreateScale(scales.x, scales.y, scales.z, out result);
        }

        /// <summary>
        ///  创建一个X轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateRotateX(FLOAT angle) {
            Matrix3x3 result;
            var rad = angle * Math.Deg2Rad;

            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            // [  1  0  0]
            // [  0  c  s]
            // [  0 -s  c]
            result.m11 = 1;
            result.m21 = 0;
            result.m31 = 0;
            result.m12 = 0;
            result.m22 = c;
            result.m32 = -s;
            result.m13 = 0;
            result.m23 = s;
            result.m33 = c;

            return result;
        }

        /// <summary>
        /// 创建一个Y轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateRotateY(FLOAT angle) {
            Matrix3x3 result;
            var rad = angle * Math.Deg2Rad;

            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            // [  c  0 -s]
            // [  0  1  0]
            // [  s  0  c]
            result.m11 = c;
            result.m21 = 0;
            result.m31 = s;
            result.m12 = 0;
            result.m22 = 1;
            result.m32 = 0;
            result.m13 = -s;
            result.m23 = 0;
            result.m33 = c;

            return result;
        }

        /// <summary>
        /// 创建一个Z轴旋转指定弧度的矩阵
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateRotateZ(FLOAT angle) {
            Matrix3x3 result;
            var rad = angle * Math.Deg2Rad;

            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            // [  c  s  0]
            // [ -s  c  0]
            // [  0  0  1]
            result.m11 = c;
            result.m21 = -s;
            result.m31 = 0;
            result.m12 = s;
            result.m22 = c;
            result.m32 = 0;
            result.m13 = 0;
            result.m23 = 0;
            result.m33 = 1;

            return result;
        }

        /// <summary>
        /// 创建一个轴角度的矩阵
        /// </summary>
        /// <param name="axis">轴</param>
        /// <param name="angle">角度</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateAngleAxis(Vector3 axis, FLOAT angle) {
            CreateAngleAxis(ref axis, angle, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个轴角度的矩阵
        /// </summary>
        /// <param name="axis">轴</param>
        /// <param name="angle">角度</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateAngleAxis(ref Vector3 axis, FLOAT angle, out Matrix3x3 result) {
            // a: angle
            // x, y, z: unit vector for axis.
            //
            // Rotation matrix M can compute by using below equation.
            //
            //        T               T
            //  M = uu + (cos a)( I-uu ) + (sin a)S
            //
            // Where:
            //
            //  u = ( x, y, z )
            //
            //      [  0 -z  y ]
            //  S = [  z  0 -x ]
            //      [ -y  x  0 ]
            //
            //      [ 1 0 0 ]
            //  I = [ 0 1 0 ]
            //      [ 0 0 1 ]
            //
            //
            //     [ xx + cosa*(1-xx)      yx-cosa*yx-sina*z    zx-cosa*xz+sina*y ]
            // M = [ xy-cosa*yx+sina*z     yy+cosa(1-yy)        yz-cosa*yz-sina*x ]
            //     [ zx-cosa*zx-sina*y     zy-cosa*zy+sina*x    zz+cosa*(1-zz)  ]
            //

            var normalAxis = axis.normalized;
            var rad = angle * Math.Deg2Rad;

            FLOAT x = normalAxis.x, y = normalAxis.y, z = normalAxis.z;
            FLOAT sa = Math.Sin(rad), ca = Math.Cos(rad);
            FLOAT xx = x * x, yy = y * y, zz = z * z;
            FLOAT xy = x * y, xz = x * z, yz = y * z;

            result.m11 = xx + ca * (1 - xx);
            result.m21 = xy - ca * xy + sa * z;
            result.m31 = xz - ca * xz - sa * y;
            result.m12 = xy - ca * xy - sa * z;
            result.m22 = yy + ca * (1 - yy);
            result.m32 = yz - ca * yz + sa * x;
            result.m13 = xz - ca * xz + sa * y;
            result.m23 = yz - ca * yz - sa * x;
            result.m33 = zz + ca * (1 - zz);
        }

        /// <summary>
        /// 创建一个看向目标的矩阵
        /// </summary>
        /// <param name="from">自己位置</param>
        /// <param name="to">看向的位置</param>
        /// <param name="up">自己的Up轴</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateLookAt(Vector3 from, Vector3 to, Vector3 up) {
            CreateLookAt(ref from, ref to, ref up, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个看向目标的矩阵（这是后来改的方法，和微软、等都不一样，和unity的计算结果保持一致，目前只有该方法是这么修改的）
        /// </summary>
        /// <param name="from">自己位置</param>
        /// <param name="to">看向的位置</param>
        /// <param name="up">自己的Up轴</param>
        /// <param name="result">返回一个矩阵</param>
        public static void CreateLookAt(ref Vector3 from, ref Vector3 to, ref Vector3 up,
            out Matrix3x3 result) {
            Vector3 dir = (from - to).normalized;
            CreateLookAt(ref dir, ref up, out result);
        }

        /// <summary>
        /// 创建一个方向的矩阵
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Matrix3x3 CreateLookAt(Vector3 direction, Vector3 up) {
            CreateLookAt(ref direction, ref up, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个方向的矩阵
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="up"></param>
        /// <param name="result"></param>
        public static void CreateLookAt(ref Vector3 direction, ref Vector3 up, out Matrix3x3 result) {
            Vector3.Cross(ref up, ref direction, out var u);
            u.Normalize();
            Vector3.Cross(ref direction, ref u, out var v);

            result.m11 = u.x;
            result.m21 = u.y;
            result.m31 = u.z;
            result.m12 = v.x;
            result.m22 = v.y;
            result.m32 = v.z;
            result.m13 = direction.x;
            result.m23 = direction.y;
            result.m33 = direction.z;
        }

        public static bool LookRotationToMatrix(ref Vector3 view, ref Vector3 up, out Matrix3x3 matrix) {
            // 跟 Matrix4x4中的 LookAt一样的道理
            matrix = Identity;

            var z = view;
            // compute u0
            var mag = z.magnitude;
            if (mag < Math.Epsilon) {
                return false;
            }

            z /= mag;

            Vector3 x = Vector3.Cross(up, z);
            mag = x.magnitude;
            if (mag < Math.Epsilon) {
                return false;
            }

            x /= mag;

            Vector3 y = Vector3.Cross(z, x);

            if (!Math.CompareApproximate(y.magnitude, 1.0F)) {
                // Log.Error("<可能存在这种情况吗？>");
                return false;
            }

            matrix.SetOrthoNormalBasis(x, y, z);
            return true;
        }

        /// <summary>
        /// 创建一个旋转、缩放组合的矩阵
        /// </summary>
        /// <param name="rotation">旋转</param>
        /// <param name="scale">缩放</param>
        /// <returns>返回一个矩阵</returns>
        public static Matrix3x3 CreateRS(Quaternion rotation, Vector3 scale) {
            CreateRS(ref rotation, ref scale, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个旋转、缩放组合的矩阵
        /// </summary>
        /// <param name="rotation">旋转</param>
        /// <param name="scale">缩放</param>
        /// <param name="matrix">返回一个矩阵</param>
        public static void CreateRS(ref Quaternion rotation, ref Vector3 scale, out Matrix3x3 matrix) {
            matrix = CreateRotation(rotation) * CreateScale(scale);
        }

        #endregion

        #region -------------其他矩阵静态函数

        /// <summary>
        /// 绝对矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 Abs(Matrix3x3 matrix) {
            Abs(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// 绝对矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void Abs(ref Matrix3x3 matrix, out Matrix3x3 result) {
            var m11 = Math.Abs(matrix.m11);
            var m12 = Math.Abs(matrix.m12);
            var m13 = Math.Abs(matrix.m13);

            var m21 = Math.Abs(matrix.m21);
            var m22 = Math.Abs(matrix.m22);
            var m23 = Math.Abs(matrix.m23);

            var m31 = Math.Abs(matrix.m31);
            var m32 = Math.Abs(matrix.m32);
            var m33 = Math.Abs(matrix.m33);

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;
            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;
            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        /// <summary>
        /// 逆矩阵 - 当矩阵的平移为0、缩放为1时，矩阵的逆矩阵 = 矩阵的转置
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 Invert(Matrix3x3 matrix) {
            Invert(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// 逆矩阵 - 当矩阵的缩放为1时，矩阵的逆矩阵 = 矩阵的转置
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void Invert(ref Matrix3x3 matrix, out Matrix3x3 result) {
            var determinantInverse = 1 / matrix.determinant;
            var m11 = (matrix.m22 * matrix.m33 - matrix.m23 * matrix.m32) * determinantInverse;
            var m12 = (matrix.m13 * matrix.m32 - matrix.m33 * matrix.m12) * determinantInverse;
            var m13 = (matrix.m12 * matrix.m23 - matrix.m22 * matrix.m13) * determinantInverse;

            var m21 = (matrix.m23 * matrix.m31 - matrix.m21 * matrix.m33) * determinantInverse;
            var m22 = (matrix.m11 * matrix.m33 - matrix.m13 * matrix.m31) * determinantInverse;
            var m23 = (matrix.m13 * matrix.m21 - matrix.m11 * matrix.m23) * determinantInverse;

            var m31 = (matrix.m21 * matrix.m32 - matrix.m22 * matrix.m31) * determinantInverse;
            var m32 = (matrix.m12 * matrix.m31 - matrix.m11 * matrix.m32) * determinantInverse;
            var m33 = (matrix.m11 * matrix.m22 - matrix.m12 * matrix.m21) * determinantInverse;

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;

            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;

            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        /// <summary>
        /// 转置 - 当矩阵的平移为0、缩放为1时，矩阵的转置 = 矩阵的逆矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 Transpose(Matrix3x3 matrix) {
            Transpose(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// 转置 - 当矩阵的平移为0、缩放为1时，矩阵的转置 = 矩阵的逆矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void Transpose(ref Matrix3x3 matrix, out Matrix3x3 result) {
            var m11 = matrix.m11;
            var m12 = matrix.m21;
            var m13 = matrix.m31;
            var m21 = matrix.m12;
            var m22 = matrix.m22;
            var m23 = matrix.m32;
            var m31 = matrix.m13;
            var m32 = matrix.m23;
            var m33 = matrix.m33;

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;
            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;
            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Matrix3x3 Lerp(Matrix3x3 matrix1, Matrix3x3 matrix2, FLOAT amount) {
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
        public static void Lerp(ref Matrix3x3 matrix1, ref Matrix3x3 matrix2, FLOAT amount,
            out Matrix3x3 result) {
            var m11 = matrix1.m11 + (matrix2.m11 - matrix1.m11) * amount;
            var m12 = matrix1.m12 + (matrix2.m12 - matrix1.m12) * amount;
            var m13 = matrix1.m13 + (matrix2.m13 - matrix1.m13) * amount;
            var m21 = matrix1.m21 + (matrix2.m21 - matrix1.m21) * amount;
            var m22 = matrix1.m22 + (matrix2.m22 - matrix1.m22) * amount;
            var m23 = matrix1.m23 + (matrix2.m23 - matrix1.m23) * amount;
            var m31 = matrix1.m31 + (matrix2.m31 - matrix1.m31) * amount;
            var m32 = matrix1.m32 + (matrix2.m32 - matrix1.m32) * amount;
            var m33 = matrix1.m33 + (matrix2.m33 - matrix1.m33) * amount;

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;
            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;
            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        /// <summary>
        /// 根据矩阵变换一个方向
        /// 只计算方向，不计算平移
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Transform(Matrix3x3 value1, Vector3 vector) {
            Transform(ref value1, ref vector, out var result);
            return result;
        }

        /// <summary>
        /// 根据矩阵变换一个方向
        /// 只计算方向，不计算平移
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        public static void Transform(ref Matrix3x3 value1, ref Vector3 vector, out Vector3 result) {
            var x = value1.m11 * vector.x + value1.m12 * vector.y + value1.m13 * vector.z;
            var y = value1.m21 * vector.x + value1.m22 * vector.y + value1.m23 * vector.z;
            var z = value1.m31 * vector.x + value1.m32 * vector.y + value1.m33 * vector.z;

            result.x = x;
            result.y = y;
            result.z = z;
        }

        /// <summary>
        /// 根据矩阵的转置矩阵变换一个方向
        /// 只计算方向，不计算平移
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 TransposedTransform(Matrix3x3 value1, Vector3 vector) {
            TransposedTransform(ref value1, ref vector, out var result);
            return result;
        }

        /// <summary>
        /// 根据矩阵的转置矩阵变换一个方向
        /// 只计算方向，不计算平移
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        public static void TransposedTransform(ref Matrix3x3 value1, ref Vector3 vector, out Vector3 result) {
            var x = value1.m11 * vector.x + value1.m21 * vector.y + value1.m31 * vector.z;
            var y = value1.m12 * vector.x + value1.m22 * vector.y + value1.m32 * vector.z;
            var z = value1.m13 * vector.x + value1.m23 * vector.y + value1.m33 * vector.z;

            result.x = x;
            result.y = y;
            result.z = z;
        }

        #endregion

        #region -------------矩阵本地函数

        /// <summary>
        /// 获得行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetRow(int index) {
            Vector3 result;
            result.x = this[index, 0];
            result.y = this[index, 1];
            result.z = this[index, 2];
            return result;
        }

        /// <summary>
        /// 获得行
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetRow(int index, Vector3 value) {
            this[index, 0] = value.x;
            this[index, 1] = value.y;
            this[index, 2] = value.z;
        }

        /// <summary>
        /// 获得列
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetColumn(int index) {
            Vector3 result;
            result.x = this[0, index];
            result.y = this[1, index];
            result.z = this[2, index];
            return result;
        }

        /// <summary>
        /// 获得列
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

            var m121 = this.m12;
            // var m122 = this.m22;
            var m123 = this.m32;

            var m131 = this.m13;
            var m132 = this.m23;
            // var m133 = this.m33;

            // this.m11 = m111;
            this.m21 = m121;
            this.m31 = m131;

            this.m12 = m112;
            // this.m22 = m122;
            this.m32 = m132;

            this.m13 = m113;
            this.m23 = m123;
            // this.m33 = m133;
        }

        /// <summary>
        /// 取逆
        /// </summary>
        public void Invert() {
            var determinantInverse = 1 / this.determinant;
            var mm11 = (this.m22 * this.m33 - this.m23 * this.m32) * determinantInverse;
            var mm12 = (this.m13 * this.m32 - this.m33 * this.m12) * determinantInverse;
            var mm13 = (this.m12 * this.m23 - this.m22 * this.m13) * determinantInverse;

            var mm21 = (this.m23 * this.m31 - this.m21 * this.m33) * determinantInverse;
            var mm22 = (this.m11 * this.m33 - this.m13 * this.m31) * determinantInverse;
            var mm23 = (this.m13 * this.m21 - this.m11 * this.m23) * determinantInverse;

            var mm31 = (this.m21 * this.m32 - this.m22 * this.m31) * determinantInverse;
            var mm32 = (this.m12 * this.m31 - this.m11 * this.m32) * determinantInverse;
            var mm33 = (this.m11 * this.m22 - this.m12 * this.m21) * determinantInverse;

            this.m11 = mm11;
            this.m12 = mm12;
            this.m13 = mm13;

            this.m21 = mm21;
            this.m22 = mm22;
            this.m23 = mm23;

            this.m31 = mm31;
            this.m32 = mm32;
            this.m33 = mm33;
        }

        /// <summary>
        /// 分解
        /// </summary>
        /// <param name="qua">旋转</param>
        /// <param name="scl">缩放</param>
        public void Decompose(out Quaternion qua, out Vector3 scl) {
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

            var num1 = 1 / Math.Sqrt(num00 * num00 + num10 * num10 + num20 * num20);
            identity[0, 0] = num00 * num1;
            identity[1, 0] = num10 * num1;
            identity[2, 0] = num20 * num1;
            var num2 = (identity[0, 0] * num01 + identity[1, 0] * num11 + identity[2, 0] * num21);
            identity[0, 1] = num01 - num2 * identity[0, 0];
            identity[1, 1] = num11 - num2 * identity[1, 0];
            identity[2, 1] = num21 - num2 * identity[2, 0];
            var num3 = 1f / Math.Sqrt(identity[0, 1] * identity[0, 1] + identity[1, 1] * identity[1, 1] +
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
            var num6 = 1f / Math.Sqrt(identity[0, 2] * identity[0, 2] + identity[1, 2] * identity[1, 2] +
                                       identity[2, 2] * identity[2, 2]);
            identity[0, 2] *= num6;
            identity[1, 2] *= num6;
            identity[2, 2] *= num6;
            if (identity[0, 0] * identity[1, 1] * identity[2, 2] + identity[0, 1] * identity[1, 2] * identity[2, 0] +
                identity[0, 2] * identity[1, 0] * identity[2, 1] - identity[0, 2] * identity[1, 1] * identity[2, 0] -
                identity[0, 1] * identity[1, 0] * identity[2, 2] - identity[0, 0] * identity[1, 2] * identity[2, 1] <
                0.0) {
                for (int index1 = 0; index1 < 3; ++index1) {
                    for (int index2 = 0; index2 < 3; ++index2) {
                        identity[index1, index2] = -identity[index1, index2];
                    }
                }
            }

            scl = new Vector3((identity[0, 0] * num00 + identity[1, 0] * num10 + identity[2, 0] * num20),
                (identity[0, 1] * num01 + identity[1, 1] * num11 + identity[2, 1] * num21),
                (identity[0, 2] * num02 + identity[1, 2] * num12 + identity[2, 2] * num22));
            qua = Quaternion.CreateMatrix(identity);
        }

        /// <summary>
        /// 转成FMatrix4x4
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 ToFMatrix4x4() {
            Matrix4x4 result = default;

            for (int row = 0; row < 3; row++) {
                for (int column = 0; column < 3; column++) {
                    result[row, column] = this[row, column];
                }
            }

            result[15] = 1;

            return result;
        }

        public void SetOrthoNormalBasis(Vector3 inX, Vector3 inY, Vector3 inZ) {
            this[0, 0] = inX.x;
            this[0, 1] = inY.x;
            this[0, 2] = inZ.x;
            this[1, 0] = inX.y;
            this[1, 1] = inY.y;
            this[1, 2] = inZ.y;
            this[2, 0] = inX.z;
            this[2, 1] = inY.z;
            this[2, 2] = inZ.z;
        }

        #endregion

        #region -------------矩阵运算函数

        public bool Equals(Matrix3x3 other) {
            return Math.CompareApproximate(this.m11, other.m11) &&
                   Math.CompareApproximate(this.m12, other.m12) &&
                   Math.CompareApproximate(this.m13, other.m13) &&
                   Math.CompareApproximate(this.m21, other.m21) &&
                   Math.CompareApproximate(this.m22, other.m22) &&
                   Math.CompareApproximate(this.m23, other.m23) &&
                   Math.CompareApproximate(this.m31, other.m31) &&
                   Math.CompareApproximate(this.m32, other.m32) &&
                   Math.CompareApproximate(this.m33, other.m33);
        }

        public override bool Equals(object obj) {
            if (!(obj is Matrix3x3 other)) return false;

            return Math.CompareApproximate(this.m11, other.m11) &&
                   Math.CompareApproximate(this.m12, other.m12) &&
                   Math.CompareApproximate(this.m13, other.m13) &&
                   Math.CompareApproximate(this.m21, other.m21) &&
                   Math.CompareApproximate(this.m22, other.m22) &&
                   Math.CompareApproximate(this.m23, other.m23) &&
                   Math.CompareApproximate(this.m31, other.m31) &&
                   Math.CompareApproximate(this.m32, other.m32) &&
                   Math.CompareApproximate(this.m33, other.m33);
        }

        public override int GetHashCode() {
            return this.m11.GetHashCode() ^ this.m12.GetHashCode() ^ this.m13.GetHashCode() ^ this.m21.GetHashCode() ^ this.m22.GetHashCode() ^
                   this.m23.GetHashCode() ^ this.m31.GetHashCode() ^ this.m32.GetHashCode() ^ this.m33.GetHashCode();
        }

#if FIXED_MATH
        public override string ToString()
        {
            return $"{m11.AsFloat():F5}\t{m12.AsFloat():F5}\t{m13.AsFloat():F5}" + "\n" +
                    $"{m21.AsFloat():F5}\t{m22.AsFloat():F5}\t{m23.AsFloat():F5}" + "\n" +
                    $"{m31.AsFloat():F5}\t{m32.AsFloat():F5}\t{m33.AsFloat():F5}" + "\n";
        }
#else
        public override string ToString() {
            return $"{this.m11:F5}\t{this.m12:F5}\t{this.m13:F5}" + "\n" +
                   $"{this.m21:F5}\t{this.m22:F5}\t{this.m23:F5}" + "\n" +
                   $"{this.m31:F5}\t{this.m32:F5}\t{this.m33:F5}" + "\n";
        }
#endif

        public static void Add(ref Matrix3x3 value1, ref Matrix3x3 value2, out Matrix3x3 result) {
            var m11 = value1.m11 + value2.m11;
            var m12 = value1.m12 + value2.m12;
            var m13 = value1.m13 + value2.m13;

            var m21 = value1.m21 + value2.m21;
            var m22 = value1.m22 + value2.m22;
            var m23 = value1.m23 + value2.m23;

            var m31 = value1.m31 + value2.m31;
            var m32 = value1.m32 + value2.m32;
            var m33 = value1.m33 + value2.m33;

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;
            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;
            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        public static void Subtract(ref Matrix3x3 value1, ref Matrix3x3 value2, out Matrix3x3 result) {
            var m11 = value1.m11 - value2.m11;
            var m12 = value1.m12 - value2.m12;
            var m13 = value1.m13 - value2.m13;

            var m21 = value1.m21 - value2.m21;
            var m22 = value1.m22 - value2.m22;
            var m23 = value1.m23 - value2.m23;

            var m31 = value1.m31 - value2.m31;
            var m32 = value1.m32 - value2.m32;
            var m33 = value1.m33 - value2.m33;

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;
            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;
            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        public static void Multiply(ref Matrix3x3 value1, ref Matrix3x3 value2, out Matrix3x3 result) {
            var m11 = value1.m11 * value2.m11 + value1.m12 * value2.m21 + value1.m13 * value2.m31;
            var m12 = value1.m11 * value2.m12 + value1.m12 * value2.m22 + value1.m13 * value2.m32;
            var m13 = value1.m11 * value2.m13 + value1.m12 * value2.m23 + value1.m13 * value2.m33;

            // Second row
            var m21 = value1.m21 * value2.m11 + value1.m22 * value2.m21 + value1.m23 * value2.m31;
            var m22 = value1.m21 * value2.m12 + value1.m22 * value2.m22 + value1.m23 * value2.m32;
            var m23 = value1.m21 * value2.m13 + value1.m22 * value2.m23 + value1.m23 * value2.m33;

            // Third row
            var m31 = value1.m31 * value2.m11 + value1.m32 * value2.m21 + value1.m33 * value2.m31;
            var m32 = value1.m31 * value2.m12 + value1.m32 * value2.m22 + value1.m33 * value2.m32;
            var m33 = value1.m31 * value2.m13 + value1.m32 * value2.m23 + value1.m33 * value2.m33;

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;
            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;
            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        public static void Multiply(ref Matrix3x3 value1, ref Vector3 value2, out Vector3 result) {
            result.x = (value1.m11 * value2.x + value1.m12 * value2.y + value1.m13 * value2.z);
            result.y = (value1.m21 * value2.x + value1.m22 * value2.y + value1.m23 * value2.z);
            result.z = (value1.m31 * value2.x + value1.m32 * value2.y + value1.m33 * value2.z);
        }

        public static void Multiply(ref Matrix3x3 value1, FLOAT value2, out Matrix3x3 result) {
            var m11 = value1.m11 * value2;
            var m12 = value1.m12 * value2;
            var m13 = value1.m13 * value2;

            var m21 = value1.m21 * value2;
            var m22 = value1.m22 * value2;
            var m23 = value1.m23 * value2;

            var m31 = value1.m31 * value2;
            var m32 = value1.m32 * value2;
            var m33 = value1.m33 * value2;

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;
            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;
            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        public static void Negate(ref Matrix3x3 value, out Matrix3x3 result) {
            var m11 = -value.m11;
            var m12 = -value.m12;
            var m13 = -value.m13;
            var m21 = -value.m21;
            var m22 = -value.m22;
            var m23 = -value.m23;
            var m31 = -value.m31;
            var m32 = -value.m32;
            var m33 = -value.m33;

            result.m11 = m11;
            result.m12 = m12;
            result.m13 = m13;
            result.m21 = m21;
            result.m22 = m22;
            result.m23 = m23;
            result.m31 = m31;
            result.m32 = m32;
            result.m33 = m33;
        }

        public static bool operator ==(Matrix3x3 v1, Matrix3x3 v2) {
            return Math.CompareApproximate(v1.m11, v2.m11) &&
                   Math.CompareApproximate(v1.m12, v2.m12) &&
                   Math.CompareApproximate(v1.m13, v2.m13) &&
                   Math.CompareApproximate(v1.m21, v2.m21) &&
                   Math.CompareApproximate(v1.m22, v2.m22) &&
                   Math.CompareApproximate(v1.m23, v2.m23) &&
                   Math.CompareApproximate(v1.m31, v2.m31) &&
                   Math.CompareApproximate(v1.m32, v2.m32) &&
                   Math.CompareApproximate(v1.m33, v2.m33);
        }

        public static bool operator !=(Matrix3x3 v1, Matrix3x3 v2) {
            return !Math.CompareApproximate(v1.m11, v2.m11) ||
                   !Math.CompareApproximate(v1.m12, v2.m12) ||
                   !Math.CompareApproximate(v1.m13, v2.m13) ||
                   !Math.CompareApproximate(v1.m21, v2.m21) ||
                   !Math.CompareApproximate(v1.m22, v2.m22) ||
                   !Math.CompareApproximate(v1.m23, v2.m23) ||
                   !Math.CompareApproximate(v1.m31, v2.m31) ||
                   !Math.CompareApproximate(v1.m32, v2.m32) ||
                   !Math.CompareApproximate(v1.m33, v2.m33);
        }

        public static Matrix3x3 operator +(Matrix3x3 v1, Matrix3x3 v2) {
            Add(ref v1, ref v2, out var result);
            return result;
        }

        public static Matrix3x3 operator -(Matrix3x3 v) {
            Negate(ref v, out var result);
            return result;
        }

        public static Matrix3x3 operator -(Matrix3x3 v1, Matrix3x3 v2) {
            Subtract(ref v1, ref v2, out var result);
            return result;
        }

        public static Matrix3x3 operator *(Matrix3x3 v1, FLOAT v2) {
            Multiply(ref v1, v2, out var result);
            return result;
        }

        public static Matrix3x3 operator *(FLOAT v2, Matrix3x3 v1) {
            Multiply(ref v1, v2, out var result);
            return result;
        }

        public static Matrix3x3 operator *(Matrix3x3 v1, Matrix3x3 v2) {
            Multiply(ref v1, ref v2, out var result);
            return result;
        }

        public static Vector3 operator *(Matrix3x3 v1, Vector3 v2) {
            Multiply(ref v1, ref v2, out var result);
            return result;
        }

        public static Vector3 operator *(Vector3 v2, Matrix3x3 v1) {
            Multiply(ref v1, ref v2, out var result);
            return result;
        }

        #endregion
    }
}