#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public static class FMatrix4x4Ex {
        /// <summary>
        /// Copies source matrix into destination matrix
        /// </summary>
        public static void CopyMatrix(ref Matrix4x4 source, out Matrix4x4 destination) {
            destination.m11 = source.m11;
            destination.m12 = source.m12;
            destination.m13 = source.m13;
            destination.m14 = source.m14;
            destination.m21 = source.m21;
            destination.m22 = source.m22;
            destination.m23 = source.m23;
            destination.m24 = source.m24;
            destination.m31 = source.m31;
            destination.m32 = source.m32;
            destination.m33 = source.m33;
            destination.m34 = source.m34;
            destination.m41 = source.m41;
            destination.m42 = source.m42;
            destination.m43 = source.m43;
            destination.m44 = source.m44;
        }

        /// <summary>
        /// Creates rotation matrix from 3 vectors (vectors are columns of the matrix)
        /// </summary>
        public static void CreateRotationFromColumns(Vector3 column0, Vector3 column1, Vector3 column2,
            out Matrix4x4 matrix) {
            matrix = Matrix4x4.Identity;
            matrix.SetColumn(0, column0);
            matrix.SetColumn(1, column1);
            matrix.SetColumn(2, column2);
        }

        /// <summary>
        /// Creates rotation matrix from 3 vectors (vectors are columns of the matrix)
        /// </summary>
        public static void CreateRotationFromColumns(ref Vector3 column0, ref Vector3 column1,
            ref Vector3 column2, out Matrix4x4 matrix) {
            matrix = Matrix4x4.Identity;
            matrix.SetColumn(0, column0);
            matrix.SetColumn(1, column1);
            matrix.SetColumn(2, column2);
        }
    }
}