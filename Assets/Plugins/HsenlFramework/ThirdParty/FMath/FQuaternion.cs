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
    public struct FQuaternion {
        public static FQuaternion Identity = new FQuaternion(0, 0, 0, 1);
        public static FQuaternion X90du = Euler(90, 0, 0);
        public static FQuaternion Y90du = Euler(0, 90, 0);
        public static FQuaternion Z90du = Euler(0, 0, 90);

        /*
         * 1、四元数在3D图形学中，就是代表旋转的，因为欧拉角表示旋转会出现万向节死锁，所以才出现的四元数。它是由一个三维向量（xyz）和一个标量（w）组成的
         * 2、设旋转轴为v，旋转弧度为θ，则四元数表示为：
         * x = sin(θ/2) * v.x;
         * y = sin(θ/2) * v.y;
         * z = sin(θ/2) * v.z;
         * w = cos(θ/2);
         * 其中x y z w的取值范围均为-1到1
         */

        public FLOAT x;
        public FLOAT y;
        public FLOAT z;
        public FLOAT w;

        public FVector3 eulerAngles {
            get {
                FMatrix3x3.CreateRotation(ref this, out var mt);
                Internal_MatrixToEuler(ref mt, out var result);
                return result;
            }
        }

        public FLOAT lengthSquared => this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;

        public FLOAT length => FMath.Sqrt(this.lengthSquared);

        public FQuaternion normalize {
            get {
                Normalize(ref this, out var result);
                return result;
            }
        }

        public FQuaternion inverse {
            get {
                Inverse(ref this, out var result);
                return result;
            }
        }

        public FQuaternion(FLOAT x, FLOAT y, FLOAT z, FLOAT w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public FQuaternion(FVector3 xAxis, FVector3 yAxis, FVector3 zAxis) {
            var identity = FMatrix3x3.Identity;

            identity[0, 0] = xAxis.x;
            identity[1, 0] = xAxis.y;
            identity[2, 0] = xAxis.z;
            identity[0, 1] = yAxis.x;
            identity[1, 1] = yAxis.y;
            identity[2, 1] = yAxis.z;
            identity[0, 2] = zAxis.x;
            identity[1, 2] = zAxis.y;
            identity[2, 2] = zAxis.z;

            CreateMatrix(ref identity, out this);
        }

        public FQuaternion(float angle, FVector3 rkAxis) {
            float num1 = angle * 0.5f;
            float num2 = FMath.Sin(num1);
            float num3 = FMath.Cos(num1);
            this.x = rkAxis.x * num2;
            this.y = rkAxis.y * num2;
            this.z = rkAxis.z * num2;
            this.w = num3;
        }

        #region ---------创建静态函数

        /// <summary>
        /// 根据欧拉角创建一个四元数
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public static FQuaternion Euler(FVector3 eulerAngles) {
            CreateYawPitchRoll(eulerAngles.y, eulerAngles.x, eulerAngles.z, out var rotation);
            return rotation;
        }

        /// <summary>
        /// 根据欧拉角创建一个四元数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static FQuaternion Euler(FLOAT x, FLOAT y, FLOAT z) {
            CreateYawPitchRoll(y, x, z, out var rotation);
            return rotation;
        }

        /// <summary>
        /// 根据欧拉角创建一个四元数
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <param name="result"></param>
        public static void Euler(ref FVector3 eulerAngles, out FQuaternion result) {
            CreateYawPitchRoll(eulerAngles.y, eulerAngles.x, eulerAngles.z, out result);
        }

        /// <summary>
        /// 根据XYZ三轴的角度，创建一个四元数
        /// </summary>
        /// <param name="yaw">偏航角，绕Y轴的角度</param>
        /// <param name="pitch">俯仰角，绕X轴的角度</param>
        /// <param name="roll">翻滚角，绕Z轴的角度</param>
        /// <param name="result"></param>
        public static void CreateYawPitchRoll(FLOAT yaw, FLOAT pitch, FLOAT roll, out FQuaternion result) {
            /*
             * 原理就是利用四元数相乘，yaw pitch roll三个角度，分别创建三个四元数
             * 1、(0,            sin(yaw/2),  0,           cos(yaw/2))
             * 2、(sin(pitch/2), 0,           0,           cos(pitch/2))
             * 3、(0,            0,           sin(roll/2), cos(roll/2))
             * 然后相乘，就是最后的结果
             * 下面是简化后的算式
             */
            var p = pitch * 0.5f * FMath.Deg2Rad;
            var y = yaw * 0.5f * FMath.Deg2Rad;
            var r = roll * 0.5f * FMath.Deg2Rad;

            var sp = FMath.Sin(p);
            var cp = FMath.Cos(p);
            var sy = FMath.Sin(y);
            var cy = FMath.Cos(y);
            var sr = FMath.Sin(r);
            var cr = FMath.Cos(r);

            result.x = ((cy * sp) * cr) + ((sy * cp) * sr);
            result.y = ((sy * cp) * cr) - ((cy * sp) * sr);
            result.z = ((cy * cp) * sr) - ((sy * sp) * cr);
            result.w = ((cy * cp) * cr) + ((sy * sp) * sr);
        }

        /// <summary>
        /// 根据3x3矩阵创建一个四元数
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        internal static FQuaternion CreateMatrix(FMatrix3x3 matrix) {
            CreateMatrix(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// 根据3x3矩阵创建一个四元数
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        internal static void CreateMatrix(ref FMatrix3x3 matrix, out FQuaternion result) {
            var num8 = (matrix.m11 + matrix.m22) + matrix.m33;
            if (num8 > 0) {
                var num = FMath.Sqrt(num8 + 1);
                result.w = num * 0.5f;
                num = 0.5f / num;
                result.x = (matrix.m23 - matrix.m32) * num;
                result.y = (matrix.m31 - matrix.m13) * num;
                result.z = (matrix.m12 - matrix.m21) * num;
            }
            else if (matrix.m11 >= matrix.m22 && matrix.m11 >= matrix.m33) {
                var num7 = FMath.Sqrt(1 + matrix.m11 - matrix.m22 - matrix.m33);
                var num4 = 0.5f / num7;
                result.x = 0.5f * num7;
                result.y = (matrix.m12 + matrix.m21) * num4;
                result.z = (matrix.m13 + matrix.m31) * num4;
                result.w = (matrix.m23 - matrix.m32) * num4;
            }
            else if (matrix.m22 > matrix.m33) {
                var num6 = FMath.Sqrt(1 + matrix.m22 - matrix.m11 - matrix.m33);
                var num3 = 0.5f / num6;
                result.x = (matrix.m21 + matrix.m12) * num3;
                result.y = 0.5f * num6;
                result.z = (matrix.m32 + matrix.m23) * num3;
                result.w = (matrix.m31 - matrix.m13) * num3;
            }
            else {
                var num5 = FMath.Sqrt(1 + matrix.m33 - matrix.m11 - matrix.m22);
                var num2 = 0.5f / num5;
                result.x = (matrix.m31 + matrix.m13) * num2;
                result.y = (matrix.m32 + matrix.m23) * num2;
                result.z = 0.5f * num5;
                result.w = (matrix.m12 - matrix.m21) * num2;
            }
        }

        /// <summary>
        /// 根据4x4矩阵创建一个四元数
        /// <para>不能通过该方式来把一个矩阵转成四元数，然后妄图获得欧拉角，虽然当矩阵是纯旋转矩阵时，得到的结果是正确的</para>
        /// <para>知识点扩充：</para>
        /// <para>当你想把一个矩阵里的旋转拆解出来时，就需要先把旋转里掺杂的缩放给剔除掉，Decompose方法里就是这么做的</para>
        /// <para>网上很多矩阵转四元数，欧拉角的公式代码，多数都是指的纯旋转矩阵，对复合矩阵不适用</para>
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static FQuaternion CreateMatrix(FMatrix4x4 matrix) {
            var m = FMatrix4x4.Transpose(matrix);
            CreateMatrix(ref m, out var result);
            return result;
        }

        /// <summary>
        /// 根据4x4矩阵创建一个四元数
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void CreateMatrix(ref FMatrix4x4 matrix, out FQuaternion result) {
            var num1 = (matrix.m11 + matrix.m22) + matrix.m33;
            if (num1 > 0) {
                var num = FMath.Sqrt(num1 + 1);
                result.w = num * 0.5f;
                num = 0.5f / num;
                result.x = (matrix.m23 - matrix.m32) * num;
                result.y = (matrix.m31 - matrix.m13) * num;
                result.z = (matrix.m12 - matrix.m21) * num;
            }
            else if (matrix.m11 >= matrix.m22 && matrix.m11 >= matrix.m33) {
                var num2 = FMath.Sqrt(1 + matrix.m11 - matrix.m22 - matrix.m33);
                var num3 = 0.5f / num2;
                result.x = 0.5f * num2;
                result.y = (matrix.m12 + matrix.m21) * num3;
                result.z = (matrix.m13 + matrix.m31) * num3;
                result.w = (matrix.m23 - matrix.m32) * num3;
            }
            else if (matrix.m22 > matrix.m33) {
                var num4 = FMath.Sqrt(1 + matrix.m22 - matrix.m11 - matrix.m33);
                var num5 = 0.5f / num4;
                result.x = (matrix.m21 + matrix.m12) * num5;
                result.y = 0.5f * num4;
                result.z = (matrix.m32 + matrix.m23) * num5;
                result.w = (matrix.m31 - matrix.m13) * num5;
            }
            else {
                var num6 = FMath.Sqrt(1 + matrix.m33 - matrix.m11 - matrix.m22);
                var num7 = 0.5f / num6;
                result.x = (matrix.m31 + matrix.m13) * num7;
                result.y = (matrix.m32 + matrix.m23) * num7;
                result.z = 0.5f * num6;
                result.w = (matrix.m12 - matrix.m21) * num7;
            }
        }

        /// <summary>
        /// 创建一个LookAt四元数
        /// </summary>
        /// <param name="forward"></param>
        /// <returns></returns>
        public static FQuaternion CreateLookAt(FVector3 forward) {
            var up = FVector3.Up;
            FMatrix3x3.CreateLookAt(ref forward, ref up, out var mt);
            CreateMatrix(ref mt, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个LookAt四元数
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="result"></param>
        public static void CreateLookAt(ref FVector3 forward, out FQuaternion result) {
            var up = FVector3.Up;
            FMatrix3x3.CreateLookAt(ref forward, ref up, out var mt);
            CreateMatrix(ref mt, out result);
        }

        /// <summary>
        /// 创建一个LookAt四元数
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static FQuaternion CreateLookAt(FVector3 forward, FVector3 up) {
            return CreateMatrix(FMatrix3x3.CreateLookAt(forward, up));
        }

        /// <summary>
        /// 创建一个LookAt四元数
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <param name="result"></param>
        public static void CreateLookAt(ref FVector3 forward, ref FVector3 up, out FQuaternion result) {
            FMatrix3x3.CreateLookAt(ref forward, ref up, out var mt);
            CreateMatrix(ref mt, out result);
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <returns></returns>
        public static FQuaternion CreateLookRotation(FVector3 forward) {
            CreateLookRotation(ref forward, out var result);
            return result;
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="result"></param>
        public static void CreateLookRotation(ref FVector3 forward, out FQuaternion result) {
            var up = FVector3.Up;
            CreateLookRotation(ref forward, ref up, out result);
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static FQuaternion CreateLookRotation(FVector3 forward, FVector3 up) {
            CreateLookRotation(ref forward, ref up, out var result);
            return result;
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CreateLookRotation(ref FVector3 forward, ref FVector3 up, out FQuaternion result) {
            result = Identity;

            // Generates a Right handed Quaternion from a look rotation. Returns if conversion was successful.
            if (!FMatrix3x3.LookRotationToMatrix(ref forward, ref up, out var matrix)) return false;

            Internal_MatrixToQuaternion(ref matrix, out result);

            return true;
        }

        /// <summary>
        /// 创建一个向目标角度旋转的四元数
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        /// <returns></returns>
        public static FQuaternion CreateRotateTowards(FQuaternion from, FQuaternion to, FLOAT maxDegreesDelta) {
            CreateRotateTowards(ref from, ref to, maxDegreesDelta, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个向目标角度旋转的四元数
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        /// <param name="result"></param>
        public static void CreateRotateTowards(ref FQuaternion from, ref FQuaternion to, FLOAT maxDegreesDelta,
            out FQuaternion result) {
            Dot(ref from, ref to, out var dot);

            if (dot < 0.0f) {
                result = to * -1;
                dot = -dot;
            }

            var halfTheta = FMath.Acos(dot);
            var theta = halfTheta * 2;

            maxDegreesDelta *= FMath.Deg2Rad;

            if (maxDegreesDelta >= theta) {
                result = default;
                return;
            }

            maxDegreesDelta /= theta;

            result = (from * FMath.Sin((1 - maxDegreesDelta) * halfTheta) +
                      to * FMath.Sin(maxDegreesDelta * halfTheta)) * (1 / FMath.Sin(halfTheta));
        }

        /// <summary>
        /// 创建一个按轴的旋转四元数
        /// </summary>
        /// <param name="axis">轴方向</param>
        /// <param name="angle">角度 degrees</param>
        /// <returns></returns>
        public static FQuaternion CreateAngleAxis(FVector3 axis, FLOAT angle) {
            CreateAngleAxis(ref axis, angle, out var result);
            return result;
        } //

        /// <summary>
        /// 创建一个按轴的旋转四元数
        /// </summary>
        /// <param name="axis">轴方向</param>
        /// <param name="angle">角度 degrees</param>
        /// <param name="result"></param>
        public static void CreateAngleAxis(ref FVector3 axis, FLOAT angle, out FQuaternion result) {
            axis.Normalize();

            var halfAngle = angle * FMath.Deg2Rad * 0.5f;

            var sin = FMath.Sin(halfAngle);

            result.x = axis.x * sin;
            result.y = axis.y * sin;
            result.z = axis.z * sin;
            result.w = FMath.Cos(halfAngle);
        } //

        /// <summary>
        /// 创建一个从from旋转到to的旋转，有问题，当 form和 to平行时，结果和 unity的不一致
        /// </summary>
        /// <param name="from">from旋转</param>
        /// <param name="to">to旋转</param>
        /// <returns></returns>
        private static FQuaternion CreateFromToRotation(FVector3 from, FVector3 to) {
            CreateFromToRotation(ref from, ref to, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个从from旋转到to的旋转
        /// </summary>
        /// <param name="from">from旋转</param>
        /// <param name="to">to旋转</param>
        /// <param name="result"></param>
        private static void CreateFromToRotation(ref FVector3 from, ref FVector3 to, out FQuaternion result) {
            FVector3 start = from.normalized;
            FVector3 dest = to.normalized;
            float cosTheta = FVector3.Dot(start, dest);
            FVector3 rotationAxis;
            FQuaternion quaternion;
            if (cosTheta < -1 + FMath.Epsilon) {
                rotationAxis = FVector3.Cross(new FVector3(0.0f, 0.0f, 1.0f), start);
                if (rotationAxis.sqrMagnitude < 0.01f) {
                    rotationAxis = FVector3.Cross(new FVector3(1.0f, 0.0f, 0.0f), start);
                }

                rotationAxis.Normalize();
                quaternion = new FQuaternion((FLOAT)Math.PI, rotationAxis);
                quaternion.Normalize();
                result = quaternion;
            }

            rotationAxis = FVector3.Cross(start, dest);
            float s = (float)Math.Sqrt((1 + cosTheta) * 2);
            float invs = 1 / s;

            quaternion = new FQuaternion(rotationAxis.x * invs, rotationAxis.y * invs, rotationAxis.z * invs, s * 0.5f);
            quaternion.Normalize();
            result = quaternion;
        }

        #endregion

        #region ---------其他静态函数

        /// <summary>
        /// 标准化一个四元数
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static FQuaternion Normalize(FQuaternion quaternion) {
            Normalize(ref quaternion, out var result);
            return result;
        }

        /// <summary>
        /// 标准化一个四元数
        /// </summary>
        /// <param name="quaternion"></param>
        /// <param name="result"></param>
        public static void Normalize(ref FQuaternion quaternion, out FQuaternion result) {
            var sqrtLen = quaternion.x * quaternion.x + quaternion.y * quaternion.y +
                          quaternion.z * quaternion.z + quaternion.w * quaternion.w;

            if (!FMath.CompareApproximateZero(sqrtLen)) {
                var num = 1f / FMath.Sqrt(sqrtLen);
                result.x = quaternion.x * num;
                result.y = quaternion.y * num;
                result.z = quaternion.z * num;
                result.w = quaternion.w * num;
                return;
            }

            result = Identity;
        }

        /// <summary>
        /// 角度
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static FLOAT Angle(FQuaternion v1, FQuaternion v2) {
            Angle(ref v1, ref v2, out var result);
            return result;
        }

        /// <summary>
        /// 角度
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="result"></param>
        public static void Angle(ref FQuaternion v1, ref FQuaternion v2, out FLOAT result) {
            Inverse(ref v1, out var v1Inv);
            var q = v1Inv * v2;

            result = FMath.Acos(q.w) * 2 * FMath.Rad2Deg;

            if (result > 180) {
                result = 360 - result;
            }
        }

        /// <summary>
        /// 共轭
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FQuaternion Conjugate(FQuaternion value) {
            Conjugate(ref value, out var result);
            return result;
        }

        /// <summary>
        /// 共轭
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public static void Conjugate(ref FQuaternion value, out FQuaternion result) {
            var x = -value.x;
            var y = -value.y;
            var z = -value.z;
            var w = value.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        /// <summary>
        /// 点积
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FLOAT Dot(FQuaternion a, FQuaternion b) {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// 点积
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void Dot(ref FQuaternion a, ref FQuaternion b, out FLOAT result) {
            result = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// 取逆
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static FQuaternion Inverse(FQuaternion quaternion) {
            Inverse(ref quaternion, out var result);
            return result;
        }

        /// <summary>
        /// 取逆
        /// </summary>
        /// <param name="quaternion"></param>
        /// <param name="result"></param>
        public static void Inverse(ref FQuaternion quaternion, out FQuaternion result) {
            var nor = quaternion.x * quaternion.x + quaternion.y * quaternion.y +
                      quaternion.z * quaternion.z + quaternion.w * quaternion.w;

            if (FMath.CompareApproximateZero(nor)) {
                result = Identity;
                return;
            }

            var invNorm = 1 / (nor);
            // 共轭 * invNorm
            var x = -quaternion.x * invNorm;
            var y = -quaternion.y * invNorm;
            var z = -quaternion.z * invNorm;
            var w = quaternion.w * invNorm;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        /// <summary>
        /// 插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static FQuaternion Lerp(FQuaternion a, FQuaternion b, FLOAT amount) {
            amount = FMath.Clamp(amount, 0, 1);
            return LerpUnclamped(a, b, amount);
        }

        /// <summary>
        /// 插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="amount"></param>
        /// <param name="result"></param>
        public static void Lerp(ref FQuaternion a, ref FQuaternion b, FLOAT amount, out FQuaternion result) {
            amount = FMath.Clamp(amount, 0, 1);
            LerpUnclamped(ref a, ref b, amount, out result);
        }

        /// <summary>
        /// 插值(无限制)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static FQuaternion LerpUnclamped(FQuaternion a, FQuaternion b, FLOAT amount) {
            LerpUnclamped(ref a, ref b, amount, out var result);
            return result;
        }

        /// <summary>
        /// 插值(无限制)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="amount"></param>
        /// <param name="result"></param>
        public static void LerpUnclamped(ref FQuaternion a, ref FQuaternion b, FLOAT amount,
            out FQuaternion result) {
            result = a * (1 - amount) + b * amount;
            result.Normalize();
        }

        /// <summary>
        /// 球形插值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static FQuaternion Slerp(FQuaternion from, FQuaternion to, FLOAT amount) {
            Slerp(ref from, ref to, amount, out var result);
            return result;
        }

        /// <summary>
        /// 球形插值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="result"></param>
        public static void Slerp(ref FQuaternion from, ref FQuaternion to, FLOAT amount, out FQuaternion result) {
            amount = FMath.Clamp(amount, 0, 1);

            Dot(ref from, ref to, out var dot);

            if (dot < 0.0f) {
                to = to * -1;
                dot = -dot;
            }

            var halfTheta = FMath.Acos(dot);

            result = (from * FMath.Sin((1 - amount) * halfTheta) + to * FMath.Sin(amount * halfTheta)) *
                     (1 / FMath.Sin(halfTheta));
        }

        /// <summary>
        /// 球形插值(无限制)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static FQuaternion SlerpUnclamped(FQuaternion from, FQuaternion to, FLOAT amount) {
            Dot(ref from, ref to, out var dot);

            FQuaternion result = default;
            if (dot < 0) {
                dot = -dot;
                result.Set(-to.x, -to.y, -to.z, -to.w);
            }
            else
                result = to;

            if (dot < 1) {
                var angle = FMath.Acos(dot);
                FLOAT sinadiv, sinat, sinaomt;
                sinadiv = 1 / FMath.Sin(angle);
                sinat = FMath.Sin(angle * amount);
                sinaomt = FMath.Sin(angle * (1 - amount));
                result.Set((from.x * sinaomt + result.x * sinat) * sinadiv,
                    (from.y * sinaomt + result.y * sinat) * sinadiv, (from.z * sinaomt + result.z * sinat) * sinadiv,
                    (from.w * sinaomt + result.w * sinat) * sinadiv);
                return result;
            }
            else {
                return Lerp(from, result, amount);
            }
        }

        /// <summary>
        /// 球形插值(无限制)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="result"></param>
        public static void SlerpUnclamped(ref FQuaternion from, ref FQuaternion to, FLOAT amount,
            out FQuaternion result) {
            Dot(ref from, ref to, out var dot);

            result = default;
            if (dot < 0) {
                dot = -dot;
                result.Set(-to.x, -to.y, -to.z, -to.w);
            }
            else
                result = to;

            if (dot < 1) {
                var angle = FMath.Acos(dot);
                FLOAT sinadiv, sinat, sinaomt;
                sinadiv = 1 / FMath.Sin(angle);
                sinat = FMath.Sin(angle * amount);
                sinaomt = FMath.Sin(angle * (1 - amount));
                result.Set((from.x * sinaomt + result.x * sinat) * sinadiv,
                    (from.y * sinaomt + result.y * sinat) * sinadiv, (from.z * sinaomt + result.z * sinat) * sinadiv,
                    (from.w * sinaomt + result.w * sinat) * sinadiv);
            }
            else {
                Lerp(ref from, ref result, amount, out result);
            }
        }

        #endregion

        #region ---------本地函数

        public void Set(FLOAT x, FLOAT y, FLOAT z, FLOAT w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        public void SetFromToRotation(FVector3 fromDirection, FVector3 toDirection) {
            CreateFromToRotation(ref fromDirection, ref toDirection, out var targetRotation);
            this.Set(targetRotation.x, targetRotation.y, targetRotation.z, targetRotation.w);
        }

        /// <summary>
        /// 设置注视旋转
        /// </summary>
        /// <param name="view"></param>
        public void SetLookRotation(FVector3 view) {
            CreateLookRotation(ref view, out this);
        }

        /// <summary>
        /// 设置注视旋转
        /// </summary>
        /// <param name="view"></param>
        /// <param name="up"></param>
        public void SetLookRotation(FVector3 view, FVector3 up) {
            CreateLookRotation(ref view, ref up, out this);
        }

        /// <summary>
        /// 标准化
        /// </summary>
        public void Normalize() {
            Normalize(ref this, out this);
        }

        /// <summary>
        /// 转换为角轴
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        public void ToAngleAxis(out FLOAT angle, out FVector3 axis) {
            angle = 2 * FMath.Acos(this.w);
            if (angle == 0) {
                axis = FVector3.Right;
                return;
            }

            FLOAT div = 1 / FMath.Sqrt(1 - this.w * this.w);
            axis = new FVector3(this.x * div, this.y * div, this.z * div);
            angle = angle * 180 / FMath.Pi;
        }

        #endregion

        #region ---------运算函数

        public bool Equals(FQuaternion other) {
            return FMath.CompareApproximate(this.x, other.x) &&
                   FMath.CompareApproximate(this.y, other.y) &&
                   FMath.CompareApproximate(this.z, other.z) &&
                   FMath.CompareApproximate(this.w, other.w);
        }

        public override bool Equals(object obj) {
            if (!(obj is FQuaternion other)) {
                return false;
            }

            return FMath.CompareApproximate(this.x, other.x) &&
                   FMath.CompareApproximate(this.y, other.y) &&
                   FMath.CompareApproximate(this.z, other.z) &&
                   FMath.CompareApproximate(this.w, other.w);
        }

        public override int GetHashCode() {
            return this.x.GetHashCode() + this.y.GetHashCode() + this.z.GetHashCode() + this.w.GetHashCode();
        }

#if FIXED_MATH
        public override string ToString() {
            return $"({x.AsFloat():f1}, {y.AsFloat():f1}, {z.AsFloat():f1}, {w.AsFloat():f1})";
        }
#else
        public override string ToString() {
            return $"({this.x:f1}, {this.y:f1}, {this.z:f1}, {this.w:f1})";
        }
#endif

        public static void Add(ref FQuaternion quaternion1, ref FQuaternion quaternion2, out FQuaternion result) {
            var x = quaternion1.x + quaternion2.x;
            var y = quaternion1.y + quaternion2.y;
            var z = quaternion1.z + quaternion2.z;
            var w = quaternion1.w + quaternion2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Subtract(ref FQuaternion quaternion1, ref FQuaternion quaternion2,
            out FQuaternion result) {
            var x = quaternion1.x - quaternion2.x;
            var y = quaternion1.y - quaternion2.y;
            var z = quaternion1.z - quaternion2.z;
            var w = quaternion1.w - quaternion2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Multiply(ref FQuaternion quaternion1, ref FQuaternion quaternion2,
            out FQuaternion result) {
            var x1 = quaternion1.x;
            var y1 = quaternion1.y;
            var z1 = quaternion1.z;
            var w1 = quaternion1.w;

            var x2 = quaternion2.x;
            var y2 = quaternion2.y;
            var z2 = quaternion2.z;
            var w2 = quaternion2.w;

            var num1 = (y1 * z2) - (z1 * y2);
            var num2 = (z1 * x2) - (x1 * z2);
            var num3 = (x1 * y2) - (y1 * x2);
            var num4 = ((x1 * x2) + (y1 * y2)) + (z1 * z2);

            result.x = ((x1 * w2) + (x2 * w1)) + num1;
            result.y = ((y1 * w2) + (y2 * w1)) + num2;
            result.z = ((z1 * w2) + (z2 * w1)) + num3;
            result.w = (w1 * w2) - num4;
        }

        public static void Multiply(ref FQuaternion quaternion1, ref FVector3 value2, out FVector3 result) {
            var x = quaternion1.x;
            var y = quaternion1.y;
            var z = quaternion1.z;
            var w = quaternion1.w;

            var xx = x * 2f;
            var yy = y * 2f;
            var zz = z * 2f;

            var xxx = x * xx;
            var yyy = y * yy;
            var zzz = z * zz;
            var xyy = x * yy;
            var xzz = x * zz;
            var yzz = y * zz;
            var wxx = w * xx;
            var wyy = w * yy;
            var wzz = w * zz;

            result.x = (1f - (yyy + zzz)) * value2.x + (xyy - wzz) * value2.y + (xzz + wyy) * value2.z;
            result.y = (xyy + wzz) * value2.x + (1f - (xxx + zzz)) * value2.y + (yzz - wxx) * value2.z;
            result.z = (xzz - wyy) * value2.x + (yzz + wxx) * value2.y + (1f - (xxx + yyy)) * value2.z;
        }

        public static void Multiply(ref FQuaternion quaternion1, ref FLOAT value2, out FQuaternion result) {
            result.x = quaternion1.x * value2;
            result.y = quaternion1.y * value2;
            result.z = quaternion1.z * value2;
            result.w = quaternion1.w * value2;
        }

        public static void Negate(ref FQuaternion quaternion, out FQuaternion result) {
            result.x = -quaternion.x;
            result.y = -quaternion.y;
            result.z = -quaternion.z;
            result.w = -quaternion.w;
        }

        public static bool operator ==(FQuaternion value1, FQuaternion value2) {
            if (FMath.CompareApproximate(value1.x, value2.x) &&
                FMath.CompareApproximate(value1.y, value2.y) &&
                FMath.CompareApproximate(value1.z, value2.z)) {
                return FMath.CompareApproximate(value1.w, value2.w);
            }

            return false;
        }

        public static bool operator !=(FQuaternion value1, FQuaternion value2) {
            if (FMath.CompareApproximate(value1.x, value2.x) &&
                FMath.CompareApproximate(value1.y, value2.y) &&
                FMath.CompareApproximate(value1.z, value2.z)) {
                return !FMath.CompareApproximate(value1.w, value2.w);
            }

            return true;
        }

        public static FQuaternion operator +(FQuaternion value1, FQuaternion value2) {
            Add(ref value1, ref value2, out var result);
            return result;
        }

        public static FQuaternion operator -(FQuaternion value1, FQuaternion value2) {
            Subtract(ref value1, ref value2, out var result);
            return result;
        }

        public static FQuaternion operator -(FQuaternion value1) {
            Negate(ref value1, out var result);
            return result;
        }

        public static FQuaternion operator *(FQuaternion value1, FQuaternion value2) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

        public static FVector3 operator *(FQuaternion value1, FVector3 value2) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

        public static FVector3 operator *(FVector3 value2, FQuaternion value1) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

        public static FQuaternion operator *(FQuaternion value1, FLOAT value2) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

        public static FQuaternion operator *(FLOAT value2, FQuaternion value1) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

#if UNITY
        public static implicit operator UnityEngine.Quaternion(FQuaternion q)
        {
            return new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
        }

        public static implicit operator FQuaternion(UnityEngine.Quaternion q)
        {
            return new FQuaternion(q.x, q.y, q.z, q.w);
        }
#endif

        #endregion

        #region ---------私有函数

        // 3x3矩阵转成向量。不能转复合矩阵，就是同时包含[旋转]和[非1缩放]的矩阵。如果缩放始终为1的话，可以使用该方法获得欧拉角，因为速度比较快
        private static void Internal_MatrixToEuler(ref FMatrix3x3 matrix, out FVector3 result) {
            // from http://www.geometrictools.com/Documentation/EulerAngles.pdf
            // YXZ order
            if (matrix.m23 < 0.999F) // some fudge for imprecision
            {
                if (matrix.m23 > -0.999F) // some fudge for imprecision
                {
                    result.x = FMath.Asin(-matrix.m23);
                    result.y = FMath.Atan2(matrix.m13, matrix.m33);
                    result.z = FMath.Atan2(matrix.m21, matrix.m22);
                    result *= FMath.Rad2Deg;
                    FVector3.MakePositive(ref result);
                }
                else {
                    // WARNING.  Not unique.  YA - ZA = atan2(r01,r00)
                    result.x = FMath.Pi * 0.5F;
                    result.y = FMath.Atan2(matrix.m12, matrix.m11);
                    result.z = 0.0F;
                    result *= FMath.Rad2Deg;
                    FVector3.MakePositive(ref result);
                }
            }
            else {
                // WARNING.  Not unique.  YA + ZA = atan2(-r01,r00)
                result.x = -FMath.Pi * 0.5F;
                result.y = FMath.Atan2(-matrix.m12, matrix.m11);
                result.z = 0.0F;
                result *= FMath.Rad2Deg;
                FVector3.MakePositive(ref result);
            }
        }

        // 3x3矩阵转成四元数
        private static void Internal_MatrixToQuaternion(ref FMatrix3x3 matrix, out FQuaternion result) {
            // Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
            // article "Quaternionf Calculus and Fast Animation".

            result = default;
            var fTrace = matrix.trace;
            FLOAT root;

            if (fTrace > 0.0f) {
                // |w| > 1/2, may as well choose w > 1/2
                root = FMath.Sqrt(fTrace + 1.0f); // 2w
                result.w = 0.5f * root;
                root = 0.5f / root; // 1/(4w)
                result.x = (matrix[2, 1] - matrix[1, 2]) * root;
                result.y = (matrix[0, 2] - matrix[2, 0]) * root;
                result.z = (matrix[1, 0] - matrix[0, 1]) * root;
            }
            else {
                // |w| <= 1/2
                // var s_iNext = new int[] { 1, 2, 0 };
                var s_iNext = new FInt3(1, 2, 0);
                int i = 0;
                if (matrix[1, 1] > matrix[0, 0]) i = 1;
                if (matrix[2, 2] > matrix[i, i]) i = 2;
                int j = s_iNext[i];
                int k = s_iNext[j];

                root = FMath.Sqrt(matrix[i, i] - matrix[j, j] - matrix[k, k] + 1.0f);
                // var apkQuat = new FLOAT[] { result.x, result.y, result.z };
                var apkQuat = new FVector3(result.x, result.y, result.z);

                apkQuat[i] = 0.5f * root;
                root = 0.5f / root;
                result.w = (matrix[k, j] - matrix[j, k]) * root;
                apkQuat[j] = (matrix[j, i] + matrix[i, j]) * root;
                apkQuat[k] = (matrix[k, i] + matrix[i, k]) * root;

                result.x = apkQuat[0];
                result.y = apkQuat[1];
                result.z = apkQuat[2];
            }

            result.Normalize();
        }

        #endregion
    }
}