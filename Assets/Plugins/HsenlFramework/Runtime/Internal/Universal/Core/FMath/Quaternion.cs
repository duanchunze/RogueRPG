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
    public struct Quaternion {
        public static Quaternion Identity = new Quaternion(0, 0, 0, 1);
        public static Quaternion X90du = Euler(90, 0, 0);
        public static Quaternion Y90du = Euler(0, 90, 0);
        public static Quaternion Z90du = Euler(0, 0, 90);

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

        public Vector3 eulerAngles {
            get {
                Matrix3x3.CreateRotation(ref this, out var mt);
                Internal_MatrixToEuler(ref mt, out var result);
                return result;
            }
        }

        public FLOAT lengthSquared => this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;

        public FLOAT length => Math.Sqrt(this.lengthSquared);

        public Quaternion normalize {
            get {
                Normalize(ref this, out var result);
                return result;
            }
        }

        public Quaternion inverse {
            get {
                Inverse(ref this, out var result);
                return result;
            }
        }

        public Quaternion(FLOAT x, FLOAT y, FLOAT z, FLOAT w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis) {
            var identity = Matrix3x3.Identity;

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

        public Quaternion(float angle, Vector3 rkAxis) {
            float num1 = angle * 0.5f;
            float num2 = Math.Sin(num1);
            float num3 = Math.Cos(num1);
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
        public static Quaternion Euler(Vector3 eulerAngles) {
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
        public static Quaternion Euler(FLOAT x, FLOAT y, FLOAT z) {
            CreateYawPitchRoll(y, x, z, out var rotation);
            return rotation;
        }

        /// <summary>
        /// 根据欧拉角创建一个四元数
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <param name="result"></param>
        public static void Euler(ref Vector3 eulerAngles, out Quaternion result) {
            CreateYawPitchRoll(eulerAngles.y, eulerAngles.x, eulerAngles.z, out result);
        }

        /// <summary>
        /// 根据XYZ三轴的角度，创建一个四元数
        /// </summary>
        /// <param name="yaw">偏航角，绕Y轴的角度</param>
        /// <param name="pitch">俯仰角，绕X轴的角度</param>
        /// <param name="roll">翻滚角，绕Z轴的角度</param>
        /// <param name="result"></param>
        public static void CreateYawPitchRoll(FLOAT yaw, FLOAT pitch, FLOAT roll, out Quaternion result) {
            /*
             * 原理就是利用四元数相乘，yaw pitch roll三个角度，分别创建三个四元数
             * 1、(0,            sin(yaw/2),  0,           cos(yaw/2))
             * 2、(sin(pitch/2), 0,           0,           cos(pitch/2))
             * 3、(0,            0,           sin(roll/2), cos(roll/2))
             * 然后相乘，就是最后的结果
             * 下面是简化后的算式
             */
            var p = pitch * 0.5f * Math.Deg2Rad;
            var y = yaw * 0.5f * Math.Deg2Rad;
            var r = roll * 0.5f * Math.Deg2Rad;

            var sp = Math.Sin(p);
            var cp = Math.Cos(p);
            var sy = Math.Sin(y);
            var cy = Math.Cos(y);
            var sr = Math.Sin(r);
            var cr = Math.Cos(r);

            result.x = ((cy * sp) * cr) + ((sy * cp) * sr);
            result.y = ((sy * cp) * cr) - ((cy * sp) * sr);
            result.z = ((cy * cp) * sr) - ((sy * sp) * cr);
            result.w = ((cy * cp) * cr) + ((sy * sp) * sr);
        }

        public static Quaternion CreateYawPitchRoll(FLOAT yaw, FLOAT pitch, FLOAT roll) {
            CreateYawPitchRoll(yaw, pitch, roll, out var result);
            return result;
        }

        /// <summary>
        /// 根据3x3矩阵创建一个四元数
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        internal static Quaternion CreateMatrix(Matrix3x3 matrix) {
            CreateMatrix(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// 根据3x3矩阵创建一个四元数
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        internal static void CreateMatrix(ref Matrix3x3 matrix, out Quaternion result) {
            var num8 = (matrix.m11 + matrix.m22) + matrix.m33;
            if (num8 > 0) {
                var num = Math.Sqrt(num8 + 1);
                result.w = num * 0.5f;
                num = 0.5f / num;
                result.x = (matrix.m23 - matrix.m32) * num;
                result.y = (matrix.m31 - matrix.m13) * num;
                result.z = (matrix.m12 - matrix.m21) * num;
            }
            else if (matrix.m11 >= matrix.m22 && matrix.m11 >= matrix.m33) {
                var num7 = Math.Sqrt(1 + matrix.m11 - matrix.m22 - matrix.m33);
                var num4 = 0.5f / num7;
                result.x = 0.5f * num7;
                result.y = (matrix.m12 + matrix.m21) * num4;
                result.z = (matrix.m13 + matrix.m31) * num4;
                result.w = (matrix.m23 - matrix.m32) * num4;
            }
            else if (matrix.m22 > matrix.m33) {
                var num6 = Math.Sqrt(1 + matrix.m22 - matrix.m11 - matrix.m33);
                var num3 = 0.5f / num6;
                result.x = (matrix.m21 + matrix.m12) * num3;
                result.y = 0.5f * num6;
                result.z = (matrix.m32 + matrix.m23) * num3;
                result.w = (matrix.m31 - matrix.m13) * num3;
            }
            else {
                var num5 = Math.Sqrt(1 + matrix.m33 - matrix.m11 - matrix.m22);
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
        public static Quaternion CreateMatrix(Matrix4x4 matrix) {
            var m = Matrix4x4.Transpose(matrix);
            CreateMatrix(ref m, out var result);
            return result;
        }

        /// <summary>
        /// 根据4x4矩阵创建一个四元数
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void CreateMatrix(ref Matrix4x4 matrix, out Quaternion result) {
            var num1 = (matrix.m11 + matrix.m22) + matrix.m33;
            if (num1 > 0) {
                var num = Math.Sqrt(num1 + 1);
                result.w = num * 0.5f;
                num = 0.5f / num;
                result.x = (matrix.m23 - matrix.m32) * num;
                result.y = (matrix.m31 - matrix.m13) * num;
                result.z = (matrix.m12 - matrix.m21) * num;
            }
            else if (matrix.m11 >= matrix.m22 && matrix.m11 >= matrix.m33) {
                var num2 = Math.Sqrt(1 + matrix.m11 - matrix.m22 - matrix.m33);
                var num3 = 0.5f / num2;
                result.x = 0.5f * num2;
                result.y = (matrix.m12 + matrix.m21) * num3;
                result.z = (matrix.m13 + matrix.m31) * num3;
                result.w = (matrix.m23 - matrix.m32) * num3;
            }
            else if (matrix.m22 > matrix.m33) {
                var num4 = Math.Sqrt(1 + matrix.m22 - matrix.m11 - matrix.m33);
                var num5 = 0.5f / num4;
                result.x = (matrix.m21 + matrix.m12) * num5;
                result.y = 0.5f * num4;
                result.z = (matrix.m32 + matrix.m23) * num5;
                result.w = (matrix.m31 - matrix.m13) * num5;
            }
            else {
                var num6 = Math.Sqrt(1 + matrix.m33 - matrix.m11 - matrix.m22);
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
        /// <param name="point"></param>
        /// <returns></returns>
        public static Quaternion CreateLookAt(Vector3 point) {
            var up = Vector3.Up;
            Matrix3x3.CreateLookAt(ref point, ref up, out var mt);
            CreateMatrix(ref mt, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个LookAt四元数
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public static void CreateLookAt(ref Vector3 point, out Quaternion result) {
            var up = Vector3.Up;
            Matrix3x3.CreateLookAt(ref point, ref up, out var mt);
            CreateMatrix(ref mt, out result);
        }

        /// <summary>
        /// 创建一个LookAt四元数
        /// </summary>
        /// <param name="point"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Quaternion CreateLookAt(Vector3 point, Vector3 up) {
            return CreateMatrix(Matrix3x3.CreateLookAt(point, up));
        }

        /// <summary>
        /// 创建一个LookAt四元数
        /// </summary>
        /// <param name="point"></param>
        /// <param name="up"></param>
        /// <param name="result"></param>
        public static void CreateLookAt(ref Vector3 point, ref Vector3 up, out Quaternion result) {
            Matrix3x3.CreateLookAt(ref point, ref up, out var mt);
            CreateMatrix(ref mt, out result);
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <returns></returns>
        public static Quaternion CreateLookRotation(Vector3 forward) {
            CreateLookRotation(ref forward, out var result);
            return result;
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="result"></param>
        public static void CreateLookRotation(ref Vector3 forward, out Quaternion result) {
            var up = Vector3.Up;
            CreateLookRotation(ref forward, ref up, out result);
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Quaternion CreateLookRotation(Vector3 forward, Vector3 up) {
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
        public static bool CreateLookRotation(ref Vector3 forward, ref Vector3 up, out Quaternion result) {
            result = Identity;

            // Generates a Right handed Quaternion from a look rotation. Returns if conversion was successful.
            if (!Matrix3x3.LookRotationToMatrix(ref forward, ref up, out var matrix)) return false;

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
        public static Quaternion CreateRotateTowards(Quaternion from, Quaternion to, FLOAT maxDegreesDelta) {
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
        public static void CreateRotateTowards(ref Quaternion from, ref Quaternion to, FLOAT maxDegreesDelta,
            out Quaternion result) {
            Dot(ref from, ref to, out var dot);

            if (dot < 0.0f) {
                result = to * -1;
                dot = -dot;
            }

            var halfTheta = Math.Acos(dot);
            var theta = halfTheta * 2;

            maxDegreesDelta *= Math.Deg2Rad;

            if (maxDegreesDelta >= theta) {
                result = default;
                return;
            }

            maxDegreesDelta /= theta;

            result = (from * Math.Sin((1 - maxDegreesDelta) * halfTheta) +
                      to * Math.Sin(maxDegreesDelta * halfTheta)) * (1 / Math.Sin(halfTheta));
        }

        /// <summary>
        /// 创建一个按轴的旋转四元数
        /// </summary>
        /// <param name="axis">轴方向</param>
        /// <param name="angle">角度 degrees</param>
        /// <returns></returns>
        public static Quaternion CreateAngleAxis(Vector3 axis, FLOAT angle) {
            CreateAngleAxis(ref axis, angle, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个按轴的旋转四元数
        /// </summary>
        /// <param name="axis">轴方向</param>
        /// <param name="angle">角度 degrees</param>
        /// <param name="result"></param>
        public static void CreateAngleAxis(ref Vector3 axis, FLOAT angle, out Quaternion result) {
            axis.Normalize();

            var halfAngle = angle * Math.Deg2Rad * 0.5f;

            var sin = Math.Sin(halfAngle);

            result.x = axis.x * sin;
            result.y = axis.y * sin;
            result.z = axis.z * sin;
            result.w = Math.Cos(halfAngle);
        }

        /// <summary>
        /// 创建一个从from旋转到to的旋转，有问题，当 form和 to平行时，结果和 unity的不一致
        /// </summary>
        /// <param name="from">from旋转</param>
        /// <param name="to">to旋转</param>
        /// <returns></returns>
        private static Quaternion CreateFromToRotation(Vector3 from, Vector3 to) {
            CreateFromToRotation(ref from, ref to, out var result);
            return result;
        }

        /// <summary>
        /// 创建一个从from旋转到to的旋转
        /// </summary>
        /// <param name="from">from旋转</param>
        /// <param name="to">to旋转</param>
        /// <param name="result"></param>
        private static void CreateFromToRotation(ref Vector3 from, ref Vector3 to, out Quaternion result) {
            Vector3 start = from.normalized;
            Vector3 dest = to.normalized;
            float cosTheta = Vector3.Dot(start, dest);
            Vector3 rotationAxis;
            Quaternion quaternion;
            if (cosTheta < -1 + Math.Epsilon) {
                rotationAxis = Vector3.Cross(new Vector3(0.0f, 0.0f, 1.0f), start);
                if (rotationAxis.sqrMagnitude < 0.01f) {
                    rotationAxis = Vector3.Cross(new Vector3(1.0f, 0.0f, 0.0f), start);
                }

                rotationAxis.Normalize();
                quaternion = new Quaternion((FLOAT)System.Math.PI, rotationAxis);
                quaternion.Normalize();
                result = quaternion;
            }

            rotationAxis = Vector3.Cross(start, dest);
            float s = (float)System.Math.Sqrt((1 + cosTheta) * 2);
            float invs = 1 / s;

            quaternion = new Quaternion(rotationAxis.x * invs, rotationAxis.y * invs, rotationAxis.z * invs, s * 0.5f);
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
        public static Quaternion Normalize(Quaternion quaternion) {
            Normalize(ref quaternion, out var result);
            return result;
        }

        /// <summary>
        /// 标准化一个四元数
        /// </summary>
        /// <param name="quaternion"></param>
        /// <param name="result"></param>
        public static void Normalize(ref Quaternion quaternion, out Quaternion result) {
            var sqrtLen = quaternion.x * quaternion.x + quaternion.y * quaternion.y +
                          quaternion.z * quaternion.z + quaternion.w * quaternion.w;

            if (!Math.CompareApproximateZero(sqrtLen)) {
                var num = 1f / Math.Sqrt(sqrtLen);
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
        public static FLOAT Angle(Quaternion v1, Quaternion v2) {
            Angle(ref v1, ref v2, out var result);
            return result;
        }

        /// <summary>
        /// 角度
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="result"></param>
        public static void Angle(ref Quaternion v1, ref Quaternion v2, out FLOAT result) {
            Inverse(ref v1, out var v1Inv);
            var q = v1Inv * v2;

            result = Math.Acos(q.w) * 2 * Math.Rad2Deg;

            if (result > 180) {
                result = 360 - result;
            }
        }

        /// <summary>
        /// 共轭
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Quaternion Conjugate(Quaternion value) {
            Conjugate(ref value, out var result);
            return result;
        }

        /// <summary>
        /// 共轭
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public static void Conjugate(ref Quaternion value, out Quaternion result) {
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
        public static FLOAT Dot(Quaternion a, Quaternion b) {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// 点积
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void Dot(ref Quaternion a, ref Quaternion b, out FLOAT result) {
            result = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// 取逆
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static Quaternion Inverse(Quaternion quaternion) {
            Inverse(ref quaternion, out var result);
            return result;
        }

        /// <summary>
        /// 取逆
        /// </summary>
        /// <param name="quaternion"></param>
        /// <param name="result"></param>
        public static void Inverse(ref Quaternion quaternion, out Quaternion result) {
            var nor = quaternion.x * quaternion.x + quaternion.y * quaternion.y +
                      quaternion.z * quaternion.z + quaternion.w * quaternion.w;

            if (Math.CompareApproximateZero(nor)) {
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
        public static Quaternion Lerp(Quaternion a, Quaternion b, FLOAT amount) {
            amount = Math.Clamp(amount, 0, 1);
            return LerpUnclamped(a, b, amount);
        }

        /// <summary>
        /// 插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="amount"></param>
        /// <param name="result"></param>
        public static void Lerp(ref Quaternion a, ref Quaternion b, FLOAT amount, out Quaternion result) {
            amount = Math.Clamp(amount, 0, 1);
            LerpUnclamped(ref a, ref b, amount, out result);
        }

        /// <summary>
        /// 插值(无限制)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, FLOAT amount) {
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
        public static void LerpUnclamped(ref Quaternion a, ref Quaternion b, FLOAT amount,
            out Quaternion result) {
            Dot(ref a, ref b, out var dt);
            if (dt < 0.0f) {
                b = -b;
            }

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
        public static Quaternion Slerp(Quaternion from, Quaternion to, FLOAT amount) {
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
        public static void Slerp(ref Quaternion from, ref Quaternion to, FLOAT amount, out Quaternion result) {
            amount = Math.Clamp(amount, 0, 1);

            Dot(ref from, ref to, out var dot);

            if (dot < 0.0f) {
                to = to * -1;
                dot = -dot;
            }

            var halfTheta = Math.Acos(dot);

            result = (from * Math.Sin((1 - amount) * halfTheta) + to * Math.Sin(amount * halfTheta)) *
                     (1 / Math.Sin(halfTheta));
        }

        /// <summary>
        /// 球形插值(无限制)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Quaternion SlerpUnclamped(Quaternion from, Quaternion to, FLOAT amount) {
            Dot(ref from, ref to, out var dot);

            Quaternion result = default;
            if (dot < 0) {
                dot = -dot;
                result.Set(-to.x, -to.y, -to.z, -to.w);
            }
            else
                result = to;

            if (dot < 1) {
                var angle = Math.Acos(dot);
                FLOAT sinadiv, sinat, sinaomt;
                sinadiv = 1 / Math.Sin(angle);
                sinat = Math.Sin(angle * amount);
                sinaomt = Math.Sin(angle * (1 - amount));
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
        public static void SlerpUnclamped(ref Quaternion from, ref Quaternion to, FLOAT amount,
            out Quaternion result) {
            Dot(ref from, ref to, out var dot);

            result = default;
            if (dot < 0) {
                dot = -dot;
                result.Set(-to.x, -to.y, -to.z, -to.w);
            }
            else
                result = to;

            if (dot < 1) {
                var angle = Math.Acos(dot);
                FLOAT sinadiv, sinat, sinaomt;
                sinadiv = 1 / Math.Sin(angle);
                sinat = Math.Sin(angle * amount);
                sinaomt = Math.Sin(angle * (1 - amount));
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
        public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection) {
            CreateFromToRotation(ref fromDirection, ref toDirection, out var targetRotation);
            this.Set(targetRotation.x, targetRotation.y, targetRotation.z, targetRotation.w);
        }

        /// <summary>
        /// 设置注视旋转
        /// </summary>
        /// <param name="forward"></param>
        public void SetLookRotation(Vector3 forward) {
            CreateLookRotation(ref forward, out this);
        }

        /// <summary>
        /// 设置注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        public void SetLookRotation(Vector3 forward, Vector3 up) {
            CreateLookRotation(ref forward, ref up, out this);
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
        public void ToAngleAxis(out FLOAT angle, out Vector3 axis) {
            angle = 2 * Math.Acos(this.w);
            if (angle == 0) {
                axis = Vector3.Right;
                return;
            }

            FLOAT div = 1 / Math.Sqrt(1 - this.w * this.w);
            axis = new Vector3(this.x * div, this.y * div, this.z * div);
            angle = angle * 180 / Math.Pi;
        }

        #endregion

        #region ---------运算函数

        public bool Equals(Quaternion other) {
            return Math.CompareApproximate(this.x, other.x) &&
                   Math.CompareApproximate(this.y, other.y) &&
                   Math.CompareApproximate(this.z, other.z) &&
                   Math.CompareApproximate(this.w, other.w);
        }

        public override bool Equals(object obj) {
            if (!(obj is Quaternion other)) {
                return false;
            }

            return Math.CompareApproximate(this.x, other.x) &&
                   Math.CompareApproximate(this.y, other.y) &&
                   Math.CompareApproximate(this.z, other.z) &&
                   Math.CompareApproximate(this.w, other.w);
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

        public static void Add(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result) {
            var x = quaternion1.x + quaternion2.x;
            var y = quaternion1.y + quaternion2.y;
            var z = quaternion1.z + quaternion2.z;
            var w = quaternion1.w + quaternion2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Subtract(ref Quaternion quaternion1, ref Quaternion quaternion2,
            out Quaternion result) {
            var x = quaternion1.x - quaternion2.x;
            var y = quaternion1.y - quaternion2.y;
            var z = quaternion1.z - quaternion2.z;
            var w = quaternion1.w - quaternion2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Multiply(ref Quaternion quaternion, ref Quaternion quaternion2,
            out Quaternion result) {
            var x1 = quaternion.x;
            var y1 = quaternion.y;
            var z1 = quaternion.z;
            var w1 = quaternion.w;

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

        public static void Multiply(ref Quaternion quaternion, ref Vector3 value, out Vector3 result) {
            var x = quaternion.x;
            var y = quaternion.y;
            var z = quaternion.z;
            var w = quaternion.w;

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

            result.x = (1f - (yyy + zzz)) * value.x + (xyy - wzz) * value.y + (xzz + wyy) * value.z;
            result.y = (xyy + wzz) * value.x + (1f - (xxx + zzz)) * value.y + (yzz - wxx) * value.z;
            result.z = (xzz - wyy) * value.x + (yzz + wxx) * value.y + (1f - (xxx + yyy)) * value.z;
        }

        public static void Multiply(ref Quaternion quaternion, ref FLOAT value, out Quaternion result) {
            result.x = quaternion.x * value;
            result.y = quaternion.y * value;
            result.z = quaternion.z * value;
            result.w = quaternion.w * value;
        }

        public static void Negate(ref Quaternion quaternion, out Quaternion result) {
            result.x = -quaternion.x;
            result.y = -quaternion.y;
            result.z = -quaternion.z;
            result.w = -quaternion.w;
        }

        public static bool operator ==(Quaternion value1, Quaternion value2) {
            if (Math.CompareApproximate(value1.x, value2.x) &&
                Math.CompareApproximate(value1.y, value2.y) &&
                Math.CompareApproximate(value1.z, value2.z)) {
                return Math.CompareApproximate(value1.w, value2.w);
            }

            return false;
        }

        public static bool operator !=(Quaternion value1, Quaternion value2) {
            if (Math.CompareApproximate(value1.x, value2.x) &&
                Math.CompareApproximate(value1.y, value2.y) &&
                Math.CompareApproximate(value1.z, value2.z)) {
                return !Math.CompareApproximate(value1.w, value2.w);
            }

            return true;
        }

        public static Quaternion operator +(Quaternion value1, Quaternion value2) {
            Add(ref value1, ref value2, out var result);
            return result;
        }

        public static Quaternion operator -(Quaternion value1, Quaternion value2) {
            Subtract(ref value1, ref value2, out var result);
            return result;
        }

        public static Quaternion operator -(Quaternion value1) {
            Negate(ref value1, out var result);
            return result;
        }

        public static Quaternion operator *(Quaternion value1, Quaternion value2) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

        public static Vector3 operator *(Quaternion value1, Vector3 value2) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

        public static Vector3 operator *(Vector3 value2, Quaternion value1) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

        public static Quaternion operator *(Quaternion value1, FLOAT value2) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

        public static Quaternion operator *(FLOAT value2, Quaternion value1) {
            Multiply(ref value1, ref value2, out var result);
            return result;
        }

#if UNITY_5_3_OR_NEWER
        public static implicit operator UnityEngine.Quaternion(Quaternion q)
        {
            return new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
        }

        public static implicit operator Quaternion(UnityEngine.Quaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }
#endif

        #endregion

        #region ---------私有函数

        // 3x3矩阵转成向量。不能转复合矩阵，就是同时包含[旋转]和[非1缩放]的矩阵。如果缩放始终为1的话，可以使用该方法获得欧拉角，因为速度比较快
        private static void Internal_MatrixToEuler(ref Matrix3x3 matrix, out Vector3 result) {
            // from http://www.geometrictools.com/Documentation/EulerAngles.pdf
            // YXZ order
            if (matrix.m23 < 0.999F) // some fudge for imprecision
            {
                if (matrix.m23 > -0.999F) // some fudge for imprecision
                {
                    result.x = Math.Asin(-matrix.m23);
                    result.y = Math.Atan2(matrix.m13, matrix.m33);
                    result.z = Math.Atan2(matrix.m21, matrix.m22);
                    result *= Math.Rad2Deg;
                    Vector3.MakePositive(ref result);
                }
                else {
                    // WARNING.  Not unique.  YA - ZA = atan2(r01,r00)
                    result.x = Math.Pi * 0.5F;
                    result.y = Math.Atan2(matrix.m12, matrix.m11);
                    result.z = 0.0F;
                    result *= Math.Rad2Deg;
                    Vector3.MakePositive(ref result);
                }
            }
            else {
                // WARNING.  Not unique.  YA + ZA = atan2(-r01,r00)
                result.x = -Math.Pi * 0.5F;
                result.y = Math.Atan2(-matrix.m12, matrix.m11);
                result.z = 0.0F;
                result *= Math.Rad2Deg;
                Vector3.MakePositive(ref result);
            }
        }

        // 3x3矩阵转成四元数
        private static void Internal_MatrixToQuaternion(ref Matrix3x3 matrix, out Quaternion result) {
            // Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
            // article "Quaternionf Calculus and Fast Animation".

            result = default;
            var fTrace = matrix.trace;
            FLOAT root;

            if (fTrace > 0.0f) {
                // |w| > 1/2, may as well choose w > 1/2
                root = Math.Sqrt(fTrace + 1.0f); // 2w
                result.w = 0.5f * root;
                root = 0.5f / root; // 1/(4w)
                result.x = (matrix[2, 1] - matrix[1, 2]) * root;
                result.y = (matrix[0, 2] - matrix[2, 0]) * root;
                result.z = (matrix[1, 0] - matrix[0, 1]) * root;
            }
            else {
                // |w| <= 1/2
                // var s_iNext = new int[] { 1, 2, 0 };
                var s_iNext = new Int3(1, 2, 0);
                int i = 0;
                if (matrix[1, 1] > matrix[0, 0]) i = 1;
                if (matrix[2, 2] > matrix[i, i]) i = 2;
                int j = s_iNext[i];
                int k = s_iNext[j];

                root = Math.Sqrt(matrix[i, i] - matrix[j, j] - matrix[k, k] + 1.0f);
                // var apkQuat = new FLOAT[] { result.x, result.y, result.z };
                var apkQuat = new Vector3(result.x, result.y, result.z);

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