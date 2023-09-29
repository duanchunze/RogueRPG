using System;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public static class LinearSystem {
        /// <summary>
        /// Solves linear system A*X=B with two equations and two unknowns.
        /// </summary>
        /// <param name="A">FLOAT[2,2] array containing equations coefficients</param>
        /// <param name="B">FLOAT[2] array containing constants</param>
        /// <param name="X">Out FLOAT[2] array contaning the solution or null if system has no solution</param>
        /// <param name="zeroTolerance">Small positive number</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve2(FLOAT[,] A, FLOAT[] B, out FLOAT[] X) {
            FLOAT zeroTolerance = 1E-05f;
            FLOAT num = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (FMath.Abs(num) < zeroTolerance) {
                X = null;
                return false;
            }

            FLOAT num2 = 1f / num;
            X = new FLOAT[2];
            X[0] = (A[1, 1] * B[0] - A[0, 1] * B[1]) * num2;
            X[1] = (A[0, 0] * B[1] - A[1, 0] * B[0]) * num2;
            return true;
        }

        /// <summary>
        /// Solves linear system A*X=B with two equations and two unknowns.
        /// </summary>
        /// <param name="A">FLOAT[2,2] array containing equations coefficients</param>
        /// <param name="B">FLOAT[2] array containing constants</param>
        /// <param name="X">Out FLOAT[2] array contaning the solution or null if system has no solution</param>
        /// <param name="zeroTolerance">Small positive number</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve2(FLOAT[,] A, FLOAT[] B, out FLOAT[] X, FLOAT zeroTolerance) {
            FLOAT num = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (FMath.Abs(num) < zeroTolerance) {
                X = null;
                return false;
            }

            FLOAT num2 = 1f / num;
            X = new FLOAT[2];
            X[0] = (A[1, 1] * B[0] - A[0, 1] * B[1]) * num2;
            X[1] = (A[0, 0] * B[1] - A[1, 0] * B[0]) * num2;
            return true;
        }

        /// <summary>
        /// Solves linear system A*X=B with two equations and two unknowns.
        /// </summary>
        /// <param name="A">FLOAT[2,2] array containing equations coefficients</param>
        /// <param name="B">FLOAT[2] array containing constants</param>
        /// <param name="X">Out vector contaning the solution or zero vector if system has no solution</param>
        /// <param name="zeroTolerance">Small positive number</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve2(FLOAT[,] A, FLOAT[] B, out FVector2 X) {
            FLOAT zeroTolerance = 1E-05f;
            FLOAT[] X2;
            bool flag = Solve2(A, B, out X2, zeroTolerance);
            if (flag) {
                X.x = X2[0];
                X.y = X2[1];
            }
            else {
                X = FVector2.Zero;
            }

            return flag;
        }

        /// <summary>
        /// Solves linear system A*X=B with two equations and two unknowns.
        /// </summary>
        /// <param name="A">FLOAT[2,2] array containing equations coefficients</param>
        /// <param name="B">FLOAT[2] array containing constants</param>
        /// <param name="X">Out vector contaning the solution or zero vector if system has no solution</param>
        /// <param name="zeroTolerance">Small positive number</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve2(FLOAT[,] A, FLOAT[] B, out FVector2 X, FLOAT zeroTolerance) {
            FLOAT[] X2;
            bool flag = Solve2(A, B, out X2, zeroTolerance);
            if (flag) {
                X.x = X2[0];
                X.y = X2[1];
            }
            else {
                X = FVector2.Zero;
            }

            return flag;
        }

        /// <summary>
        /// Solves linear system A*X=B with three equations and three unknowns.
        /// </summary>
        /// <param name="A">FLOAT[3,3] array containing equations coefficients</param>
        /// <param name="B">FLOAT[3] array containing constants</param>
        /// <param name="X">Out FLOAT[3] array contaning the solution or null if system has no solution</param>
        /// <param name="zeroTolerance">Small positive number</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve3(FLOAT[,] A, FLOAT[] B, out FLOAT[] X) {
            FLOAT zeroTolerance = 1E-05f;
            FLOAT[,] array = new FLOAT[3, 3] {
                { A[1, 1] * A[2, 2] - A[1, 2] * A[2, 1], A[0, 2] * A[2, 1] - A[0, 1] * A[2, 2], A[0, 1] * A[1, 2] - A[0, 2] * A[1, 1] },
                { A[1, 2] * A[2, 0] - A[1, 0] * A[2, 2], A[0, 0] * A[2, 2] - A[0, 2] * A[2, 0], A[0, 2] * A[1, 0] - A[0, 0] * A[1, 2] },
                { A[1, 0] * A[2, 1] - A[1, 1] * A[2, 0], A[0, 1] * A[2, 0] - A[0, 0] * A[2, 1], A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0] }
            };
            FLOAT num = A[0, 0] * array[0, 0] + A[0, 1] * array[1, 0] + A[0, 2] * array[2, 0];
            if (FMath.Abs(num) < zeroTolerance) {
                X = null;
                return false;
            }

            FLOAT num2 = 1f / num;
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    array[i, j] *= num2;
                }
            }

            X = new FLOAT[3];
            X[0] = array[0, 0] * B[0] + array[0, 1] * B[1] + array[0, 2] * B[2];
            X[1] = array[1, 0] * B[0] + array[1, 1] * B[1] + array[1, 2] * B[2];
            X[2] = array[2, 0] * B[0] + array[2, 1] * B[1] + array[2, 2] * B[2];
            return true;
        }

        /// <summary>
        /// Solves linear system A*X=B with three equations and three unknowns.
        /// </summary>
        /// <param name="A">FLOAT[3,3] array containing equations coefficients</param>
        /// <param name="B">FLOAT[3] array containing constants</param>
        /// <param name="X">Out FLOAT[3] array contaning the solution or null if system has no solution</param>
        /// <param name="zeroTolerance">Small positive number</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve3(FLOAT[,] A, FLOAT[] B, out FLOAT[] X, FLOAT zeroTolerance) {
            FLOAT[,] array = new FLOAT[3, 3] {
                { A[1, 1] * A[2, 2] - A[1, 2] * A[2, 1], A[0, 2] * A[2, 1] - A[0, 1] * A[2, 2], A[0, 1] * A[1, 2] - A[0, 2] * A[1, 1] },
                { A[1, 2] * A[2, 0] - A[1, 0] * A[2, 2], A[0, 0] * A[2, 2] - A[0, 2] * A[2, 0], A[0, 2] * A[1, 0] - A[0, 0] * A[1, 2] },
                { A[1, 0] * A[2, 1] - A[1, 1] * A[2, 0], A[0, 1] * A[2, 0] - A[0, 0] * A[2, 1], A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0] }
            };
            FLOAT num = A[0, 0] * array[0, 0] + A[0, 1] * array[1, 0] + A[0, 2] * array[2, 0];
            if (FMath.Abs(num) < zeroTolerance) {
                X = null;
                return false;
            }

            FLOAT num2 = 1f / num;
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    array[i, j] *= num2;
                }
            }

            X = new FLOAT[3];
            X[0] = array[0, 0] * B[0] + array[0, 1] * B[1] + array[0, 2] * B[2];
            X[1] = array[1, 0] * B[0] + array[1, 1] * B[1] + array[1, 2] * B[2];
            X[2] = array[2, 0] * B[0] + array[2, 1] * B[1] + array[2, 2] * B[2];
            return true;
        }

        /// <summary>
        /// Solves linear system A*X=B with three equations and three unknowns.
        /// </summary>
        /// <param name="A">FLOAT[3,3] array containing equations coefficients</param>
        /// <param name="B">FLOAT[3] array containing constants</param>
        /// <param name="X">Out vector contaning the solution or zero vector if system has no solution</param>
        /// <param name="zeroTolerance">Small positive number</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve3(FLOAT[,] A, FLOAT[] B, out FVector3 X) {
            FLOAT zeroTolerance = 1E-05f;
            FLOAT[] X2;
            bool flag = Solve3(A, B, out X2, zeroTolerance);
            if (flag) {
                X.x = X2[0];
                X.y = X2[1];
                X.z = X2[2];
            }
            else {
                X = FVector3.Zero;
            }

            return flag;
        }

        /// <summary>
        /// Solves linear system A*X=B with three equations and three unknowns.
        /// </summary>
        /// <param name="A">FLOAT[3,3] array containing equations coefficients</param>
        /// <param name="B">FLOAT[3] array containing constants</param>
        /// <param name="X">Out vector contaning the solution or zero vector if system has no solution</param>
        /// <param name="zeroTolerance">Small positive number</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve3(FLOAT[,] A, FLOAT[] B, out FVector3 X, FLOAT zeroTolerance) {
            FLOAT[] X2;
            bool flag = Solve3(A, B, out X2, zeroTolerance);
            if (flag) {
                X.x = X2[0];
                X.y = X2[1];
                X.z = X2[2];
            }
            else {
                X = FVector3.Zero;
            }

            return flag;
        }

        private static void SwapRows(FLOAT[,] matrix, int row0, int row1, int columnCount) {
            if (row0 != row1) {
                for (int i = 0; i < columnCount; i++) {
                    (matrix[row0, i], matrix[row1, i]) = (matrix[row1, i], matrix[row0, i]);
                }
            }
        }

        /// <summary>
        /// Solves linear system A*X=B with N equations and N unknowns.
        /// </summary>
        /// <param name="A">FLOAT[N,N] array containing equations coefficients</param>
        /// <param name="B">FLOAT[N] array containing constants</param>
        /// <param name="X">Out FLOAT[N] array contaning the solution or null if system has no solution</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool Solve(FLOAT[,] A, FLOAT[] B, out FLOAT[] X) {
            if (A.GetLength(0) != A.GetLength(1) || A.GetLength(0) != B.Length) {
                X = null;
                return false;
            }

            int length = A.GetLength(1);
            FLOAT[,] array = new FLOAT[A.GetLength(0), A.GetLength(1)];
            Buffer.BlockCopy(A, 0, array, 0, A.Length * 4);
            X = new FLOAT[length];
            Buffer.BlockCopy(B, 0, X, 0, length * 4);
            int[] array2 = new int[length];
            int[] array3 = new int[length];
            bool[] array4 = new bool[length];
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < length; i++) {
                FLOAT num3 = 0f;
                for (int j = 0; j < length; j++) {
                    if (array4[j]) {
                        continue;
                    }

                    for (int k = 0; k < length; k++) {
                        if (!array4[k]) {
                            FLOAT num4 = FMath.Abs(array[j, k]);
                            if (num4 > num3) {
                                num3 = num4;
                                num = j;
                                num2 = k;
                            }
                        }
                    }
                }

                if (num3 == 0f) {
                    X = null;
                    return false;
                }

                array4[num2] = true;
                if (num != num2) {
                    SwapRows(array, num, num2, length);
                    (X[num], X[num2]) = (X[num2], X[num]);
                }

                array3[i] = num;
                array2[i] = num2;
                FLOAT num6 = 1f / array[num2, num2];
                array[num2, num2] = 1f;
                for (int k = 0; k < length; k++) {
                    array[num2, k] *= num6;
                }

                X[num2] *= num6;
                for (int j = 0; j < length; j++) {
                    if (j != num2) {
                        FLOAT num5 = array[j, num2];
                        array[j, num2] = 0f;
                        for (int k = 0; k < length; k++) {
                            array[j, k] -= array[num2, k] * num5;
                        }

                        X[j] -= X[num2] * num5;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Solves linear system A*X=B, where A is tridiagonal matrix.
        /// </summary>
        /// <param name="A">Lower diagonal FLOAT[N-1]</param>
        /// <param name="B">Main  diagonal FLOAT[N]</param>
        /// <param name="C">Upper diagonal FLOAT[N-1]</param>
        /// <param name="R">Right-hand side FLOAT[N]</param>
        /// <param name="U">Out FLOAT[N] containing the solution or null if system has no solution</param>
        /// <returns>True if solution is found, false otherwise</returns>
        public static bool SolveTridiagonal(FLOAT[] A, FLOAT[] B, FLOAT[] C, FLOAT[] R, out FLOAT[] U) {
            int num = B.Length;
            if (B[0] == 0f) {
                U = null;
                return false;
            }

            FLOAT[] array = new FLOAT[num - 1];
            FLOAT num2 = B[0];
            FLOAT num3 = 1f / num2;
            U = new FLOAT[num];
            U[0] = R[0] * num3;
            int num4 = 0;
            for (int i = 1; i < num; i++) {
                array[num4] = C[num4] * num3;
                num2 = B[i] - A[num4] * array[num4];
                if (num2 == 0f) {
                    U = null;
                    return false;
                }

                num3 = 1f / num2;
                U[i] = (R[i] - A[num4] * U[num4]) * num3;
                num4++;
            }

            num4 = num - 1;
            for (int i = num - 2; i >= 0; i--) {
                U[i] -= array[i] * U[num4];
                num4--;
            }

            return true;
        }

        /// <summary>
        /// Inverses square matrix A. Returns inversed matrix in invA parameter (invA is null if A has no inverse).
        /// </summary>
        public static bool Inverse(FLOAT[,] A, out FLOAT[,] invA) {
            if (A.GetLength(0) != A.GetLength(1)) {
                invA = null;
                return false;
            }

            int length = A.GetLength(0);
            invA = new FLOAT[length, length];
            Buffer.BlockCopy(A, 0, invA, 0, A.Length * 4);
            int[] array = new int[length];
            int[] array2 = new int[length];
            bool[] array3 = new bool[length];
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < length; i++) {
                FLOAT num3 = 0f;
                for (int j = 0; j < length; j++) {
                    if (array3[j]) {
                        continue;
                    }

                    for (int k = 0; k < length; k++) {
                        if (!array3[k]) {
                            FLOAT num4 = FMath.Abs(invA[j, k]);
                            if (num4 > num3) {
                                num3 = num4;
                                num = j;
                                num2 = k;
                            }
                        }
                    }
                }

                if (num3 == 0f) {
                    invA = null;
                    return false;
                }

                array3[num2] = true;
                if (num != num2) {
                    SwapRows(invA, num, num2, length);
                }

                array2[i] = num;
                array[i] = num2;
                FLOAT num5 = 1f / invA[num2, num2];
                invA[num2, num2] = 1f;
                for (int k = 0; k < length; k++) {
                    invA[num2, k] *= num5;
                }

                for (int j = 0; j < length; j++) {
                    if (j != num2) {
                        FLOAT num6 = invA[j, num2];
                        invA[j, num2] = 0f;
                        for (int k = 0; k < length; k++) {
                            invA[j, k] -= invA[num2, k] * num6;
                        }
                    }
                }
            }

            for (int j = length - 1; j >= 0; j--) {
                if (array2[j] != array[j]) {
                    for (int k = 0; k < length; k++) {
                        (invA[k, array2[j]], invA[k, array[j]]) = (invA[k, array[j]], invA[k, array2[j]]);
                    }
                }
            }

            return true;
        }
    }
}