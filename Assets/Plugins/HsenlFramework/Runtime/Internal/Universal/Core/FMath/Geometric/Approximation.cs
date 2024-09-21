using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public static class Approximation {
        /// <summary>
        /// Fits points with a Gaussian distribution. Produces box as the result.
        /// Box center is average of a point set. Box axes are eigenvectors of the
        /// covariance matrix, box extents are eigenvalues.
        /// A set must contain at least one point!
        /// </summary>
        public static Box2D GaussPointsFit2(IList<Vector2> points) {
            Box2D result = new Box2D(Vector2.Zero, Vector2.Right, Vector2.Up, Vector2.One);
            int count = points.Count;
            result.center = points[0];
            for (int i = 1; i < count; i++) {
                result.center += points[i];
            }

            FLOAT num = 1f / (FLOAT)count;
            result.center *= num;
            FLOAT num2 = 0f;
            FLOAT num3 = 0f;
            FLOAT num4 = 0f;
            for (int j = 0; j < count; j++) {
                Vector2 vector = points[j] - result.center;
                num2 += vector.x * vector.x;
                num3 += vector.x * vector.y;
                num4 += vector.y * vector.y;
            }

            num2 *= num;
            num3 *= num;
            num4 *= num;
            FLOAT[,] symmetricSquareMatrix = new FLOAT[2, 2] { { num2, num3 }, { num3, num4 } };
            EigenData eigenData = EigenDecomposition.Solve(symmetricSquareMatrix, increasingSort: true);
            result.extents.x = eigenData.GetEigenvalue(0);
            result.extents.y = eigenData.GetEigenvalue(1);
            result.axis0 = eigenData.GetEigenvector2(0);
            result.axis1 = eigenData.GetEigenvector2(1);
            return result;
        }

        /// <summary>
        /// Fitting to a line using least-squares method and using distance
        /// measurements in the y-direction. The result is a line represented by
        /// y = A*x + B. If a line cannot be constructed method returns false and
        /// A and B are returned as FLOAT.MaxValue.
        /// </summary>
        internal static bool HeightLineFit2(IList<Vector2> points, out FLOAT a, out FLOAT b) {
            FLOAT num = 0f;
            FLOAT num2 = 0f;
            FLOAT num3 = 0f;
            FLOAT num4 = 0f;
            int count = points.Count;
            for (int i = 0; i < count; i++) {
                num += points[i].x;
                num2 += points[i].y;
                num3 += points[i].x * points[i].x;
                num4 += points[i].x * points[i].y;
            }

            FLOAT[,] a2 = new FLOAT[2, 2] { { num3, num }, { num, count } };
            FLOAT[] b2 = new FLOAT[2] { num4, num2 };
            FLOAT[] X;
            bool flag = LinearSystem.Solve2(a2, b2, out X, 1E-05f);
            if (flag) {
                a = X[0];
                b = X[1];
            }
            else {
                a = FLOAT.MaxValue;
                b = FLOAT.MaxValue;
            }

            return flag;
        }

        /// <summary>
        /// Producing a line using least-squares fitting. A set must contain at least one point!
        /// </summary>
        public static Line2D LeastSquaresLineFit2(IList<Vector2> points) {
            Line2D result = default(Line2D);
            int count = points.Count;
            result.origin = points[0];
            for (int i = 1; i < count; i++) {
                result.origin += points[i];
            }

            FLOAT num = 1f / (FLOAT)count;
            result.origin *= num;
            FLOAT num2 = 0f;
            FLOAT num3 = 0f;
            FLOAT num4 = 0f;
            for (int i = 0; i < count; i++) {
                Vector2 vector = points[i] - result.origin;
                num2 += vector.x * vector.x;
                num3 += vector.x * vector.y;
                num4 += vector.y * vector.y;
            }

            num2 *= num;
            num3 *= num;
            num4 *= num;
            FLOAT[,] symmetricSquareMatrix = new FLOAT[2, 2] { { num4, 0f - num3 }, { num3, num2 } };
            EigenData eigenData = EigenDecomposition.Solve(symmetricSquareMatrix, increasingSort: false);
            result.direction = eigenData.GetEigenvector2(1);
            return result;
        }

        /// <summary>
        /// Fits points with a Gaussian distribution. Produces box as the result.
        /// Box center is average of a point set. Box axes are eigenvectors of the
        /// covariance matrix, box extents are eigenvalues.
        /// A set must contain at least one point!
        /// </summary>
        public static Box GaussPointsFit3(IList<Vector3> points) {
            Box result = new Box(Vector3.Zero, Vector3.Right, Vector3.Up, Vector3.Forward, Vector3.One);
            int count = points.Count;
            result.center = points[0];
            for (int i = 1; i < count; i++) {
                result.center += points[i];
            }

            FLOAT num = 1f / (FLOAT)count;
            result.center *= num;
            FLOAT num2 = 0f;
            FLOAT num3 = 0f;
            FLOAT num4 = 0f;
            FLOAT num5 = 0f;
            FLOAT num6 = 0f;
            FLOAT num7 = 0f;
            for (int j = 0; j < count; j++) {
                Vector3 vector = points[j] - result.center;
                num2 += vector.x * vector.x;
                num3 += vector.x * vector.y;
                num4 += vector.x * vector.z;
                num5 += vector.y * vector.y;
                num6 += vector.y * vector.z;
                num7 += vector.z * vector.z;
            }

            num2 *= num;
            num3 *= num;
            num4 *= num;
            num5 *= num;
            num6 *= num;
            num7 *= num;
            FLOAT[,] symmetricSquareMatrix = new FLOAT[3, 3] { { num2, num3, num4 }, { num3, num5, num6 }, { num4, num6, num7 } };
            EigenData eigenData = EigenDecomposition.Solve(symmetricSquareMatrix, increasingSort: true);
            result.extents.x = eigenData.GetEigenvalue(0);
            result.axis0 = eigenData.GetEigenvector3(0);
            result.extents.y = eigenData.GetEigenvalue(1);
            result.axis1 = eigenData.GetEigenvector3(1);
            result.extents.z = eigenData.GetEigenvalue(2);
            result.axis2 = eigenData.GetEigenvector3(2);
            return result;
        }

        /// <summary>
        /// Producing a line using least-squares fitting. A set must contain at least one point!
        /// </summary>
        public static Line LeastsSquaresLineFit3(IList<Vector3> points) {
            Line result = default(Line);
            int count = points.Count;
            result.origin = points[0];
            for (int i = 1; i < count; i++) {
                result.origin += points[i];
            }

            FLOAT num = 1f / (FLOAT)count;
            result.origin *= num;
            FLOAT num2 = 0f;
            FLOAT num3 = 0f;
            FLOAT num4 = 0f;
            FLOAT num5 = 0f;
            FLOAT num6 = 0f;
            FLOAT num7 = 0f;
            for (int i = 0; i < count; i++) {
                Vector3 vector = points[i] - result.origin;
                num2 += vector.x * vector.x;
                num3 += vector.x * vector.y;
                num4 += vector.x * vector.z;
                num5 += vector.y * vector.y;
                num6 += vector.y * vector.z;
                num7 += vector.z * vector.z;
            }

            num2 *= num;
            num3 *= num;
            num4 *= num;
            num5 *= num;
            num6 *= num;
            num7 *= num;
            FLOAT[,] array = new FLOAT[3, 3];
            array[0, 0] = num5 + num7;
            array[0, 1] = 0f - num3;
            array[0, 2] = 0f - num4;
            array[1, 0] = array[0, 1];
            array[1, 1] = num2 + num7;
            array[1, 2] = 0f - num6;
            array[2, 0] = array[0, 2];
            array[2, 1] = array[1, 2];
            array[2, 2] = num2 + num5;
            EigenData eigenData = EigenDecomposition.Solve(array, increasingSort: false);
            result.direction = eigenData.GetEigenvector3(2);
            return result;
        }

        /// <summary>
        /// Least-squares fit of a plane to (x,y,f(x,y)) data by using distance
        /// measurements in the z-direction.  The resulting plane is represented by
        /// z = A*x + B*y + C.  The return value is 'false' if the 3x3 coefficient
        /// matrix in the linear system that defines A, B, and C is (nearly) singular.
        /// In this case, A, B, and C are returned as FLOAT.MaxValue.
        /// </summary>
        internal static bool HeightPlaneFit3(IList<Vector3> points, out FLOAT a, out FLOAT b, out FLOAT c) {
            FLOAT num = 0f;
            FLOAT num2 = 0f;
            FLOAT num3 = 0f;
            FLOAT num4 = 0f;
            FLOAT num5 = 0f;
            FLOAT num6 = 0f;
            FLOAT num7 = 0f;
            FLOAT num8 = 0f;
            int count = points.Count;
            for (int i = 0; i < count; i++) {
                num += points[i].x;
                num2 += points[i].y;
                num3 += points[i].z;
                num4 += points[i].x * points[i].x;
                num5 += points[i].x * points[i].y;
                num6 += points[i].x * points[i].z;
                num7 += points[i].y * points[i].y;
                num8 += points[i].y * points[i].z;
            }

            FLOAT[,] a2 = new FLOAT[3, 3] { { num4, num5, num }, { num5, num7, num2 }, { num, num2, count } };
            FLOAT[] b2 = new FLOAT[3] { num6, num8, num3 };
            FLOAT[] X;
            bool flag = LinearSystem.Solve3(a2, b2, out X, 1E-05f);
            if (flag) {
                a = X[0];
                b = X[1];
                c = X[2];
            }
            else {
                a = FLOAT.MaxValue;
                b = FLOAT.MaxValue;
                c = FLOAT.MaxValue;
            }

            return flag;
        }

        /// <summary>
        /// Producing a plane using least-squares fitting. A set must contain at least one point!
        /// </summary>
        public static Plane LeastSquaresPlaneFit3(IList<Vector3> points) {
            Vector3 point = Vector3.Zero;
            int count = points.Count;
            for (int i = 0; i < count; i++) {
                point += points[i];
            }

            FLOAT num = 1f / (FLOAT)count;
            point *= num;
            FLOAT num2 = 0f;
            FLOAT num3 = 0f;
            FLOAT num4 = 0f;
            FLOAT num5 = 0f;
            FLOAT num6 = 0f;
            FLOAT num7 = 0f;
            for (int i = 0; i < count; i++) {
                Vector3 vector = points[i] - point;
                num2 += vector.x * vector.x;
                num3 += vector.x * vector.y;
                num4 += vector.x * vector.z;
                num5 += vector.y * vector.y;
                num6 += vector.y * vector.z;
                num7 += vector.z * vector.z;
            }

            num2 *= num;
            num3 *= num;
            num4 *= num;
            num5 *= num;
            num6 *= num;
            num7 *= num;
            EigenData eigenData = EigenDecomposition.Solve(new FLOAT[3, 3] { { num2, num3, num4 }, { num3, num5, num6 }, { num4, num6, num7 } },
                increasingSort: false);
            Vector3 normal = eigenData.GetEigenvector3(2);
            return new Plane(ref normal, ref point);
        }
    }
}