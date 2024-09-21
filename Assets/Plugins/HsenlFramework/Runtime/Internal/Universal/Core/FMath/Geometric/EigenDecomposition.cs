using System;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public static class EigenDecomposition {
        private static void Tridiagonal2(FLOAT[] diagonal, FLOAT[] subdiagonal, FLOAT[,] matrix, out bool isRotation) {
            diagonal[0] = matrix[0, 0];
            diagonal[1] = matrix[1, 1];
            subdiagonal[0] = matrix[0, 1];
            subdiagonal[1] = 0f;
            matrix[0, 0] = 1f;
            matrix[0, 1] = 0f;
            matrix[1, 0] = 0f;
            matrix[1, 1] = 1f;
            isRotation = true;
        }

        private static void Tridiagonal3(FLOAT[] diagonal, FLOAT[] subdiagonal, FLOAT[,] matrix, out bool isRotation) {
            FLOAT num = matrix[0, 0];
            FLOAT num2 = matrix[0, 1];
            FLOAT num3 = matrix[0, 2];
            FLOAT num4 = matrix[1, 1];
            FLOAT num5 = matrix[1, 2];
            FLOAT num6 = matrix[2, 2];
            diagonal[0] = num;
            subdiagonal[2] = 0f;
            if (Math.Abs(num3) > 1E-05f) {
                FLOAT num7 = Math.Sqrt(num2 * num2 + num3 * num3);
                FLOAT num8 = 1f / num7;
                num2 *= num8;
                num3 *= num8;
                FLOAT num9 = 2f * num2 * num5 + num3 * (num6 - num4);
                diagonal[1] = num4 + num3 * num9;
                diagonal[2] = num6 - num3 * num9;
                subdiagonal[0] = num7;
                subdiagonal[1] = num5 - num2 * num9;
                matrix[0, 0] = 1f;
                matrix[0, 1] = 0f;
                matrix[0, 2] = 0f;
                matrix[1, 0] = 0f;
                matrix[1, 1] = num2;
                matrix[1, 2] = num3;
                matrix[2, 0] = 0f;
                matrix[2, 1] = num3;
                matrix[2, 2] = 0f - num2;
                isRotation = false;
            }
            else {
                diagonal[1] = num4;
                diagonal[2] = num6;
                subdiagonal[0] = num2;
                subdiagonal[1] = num5;
                matrix[0, 0] = 1f;
                matrix[0, 1] = 0f;
                matrix[0, 2] = 0f;
                matrix[1, 0] = 0f;
                matrix[1, 1] = 1f;
                matrix[1, 2] = 0f;
                matrix[2, 0] = 0f;
                matrix[2, 1] = 0f;
                matrix[2, 2] = 1f;
                isRotation = true;
            }
        }

        private static void TridiagonalN(FLOAT[] diagonal, FLOAT[] subdiagonal, FLOAT[,] matrix, out bool isRotation) {
            int num = diagonal.Length;
            int num2 = num - 1;
            int num3 = num - 2;
            while (num2 >= 1) {
                FLOAT num4 = 0f;
                FLOAT num5 = 0f;
                if (num3 > 0) {
                    for (int i = 0; i <= num3; i++) {
                        num5 += Math.Abs(matrix[num2, i]);
                    }

                    if (num5 == 0f) {
                        subdiagonal[num2] = matrix[num2, num3];
                    }
                    else {
                        FLOAT num6 = 1f / num5;
                        for (int i = 0; i <= num3; i++) {
                            matrix[num2, i] *= num6;
                            num4 += matrix[num2, i] * matrix[num2, i];
                        }

                        FLOAT num7 = matrix[num2, num3];
                        FLOAT num8 = Math.Sqrt(num4);
                        if (num7 > 0f) {
                            num8 = 0f - num8;
                        }

                        subdiagonal[num2] = num5 * num8;
                        num4 -= num7 * num8;
                        matrix[num2, num3] = num7 - num8;
                        num7 = 0f;
                        FLOAT num9 = 1f / num4;
                        for (int j = 0; j <= num3; j++) {
                            matrix[j, num2] = matrix[num2, j] * num9;
                            num8 = 0f;
                            for (int i = 0; i <= j; i++) {
                                num8 += matrix[j, i] * matrix[num2, i];
                            }

                            for (int i = j + 1; i <= num3; i++) {
                                num8 += matrix[i, j] * matrix[num2, i];
                            }

                            subdiagonal[j] = num8 * num9;
                            num7 += subdiagonal[j] * matrix[num2, j];
                        }

                        FLOAT num10 = 0.5f * num7 * num9;
                        for (int j = 0; j <= num3; j++) {
                            num7 = matrix[num2, j];
                            num8 = (subdiagonal[j] -= num10 * num7);
                            for (int i = 0; i <= j; i++) {
                                matrix[j, i] -= num7 * subdiagonal[i] + num8 * matrix[num2, i];
                            }
                        }
                    }
                }
                else {
                    subdiagonal[num2] = matrix[num2, num3];
                }

                diagonal[num2] = num4;
                num2--;
                num3--;
            }

            diagonal[0] = 0f;
            subdiagonal[0] = 0f;
            num2 = 0;
            num3 = -1;
            while (num2 <= num - 1) {
                if (diagonal[num2] != 0f) {
                    for (int j = 0; j <= num3; j++) {
                        FLOAT num11 = 0f;
                        for (int i = 0; i <= num3; i++) {
                            num11 += matrix[num2, i] * matrix[i, j];
                        }

                        for (int i = 0; i <= num3; i++) {
                            matrix[i, j] -= num11 * matrix[i, num2];
                        }
                    }
                }

                diagonal[num2] = matrix[num2, num2];
                matrix[num2, num2] = 1f;
                for (int j = 0; j <= num3; j++) {
                    matrix[j, num2] = 0f;
                    matrix[num2, j] = 0f;
                }

                num2++;
                num3++;
            }

            num2 = 1;
            num3 = 0;
            while (num2 < num) {
                subdiagonal[num3] = subdiagonal[num2];
                num2++;
                num3++;
            }

            subdiagonal[num - 1] = 0f;
            isRotation = num % 2 == 0;
        }

        /// <summary>
        /// QL algorithm with implicit shifting.  This function is called for tridiagonal matrices.
        /// </summary>
        private static bool QLAlgorithm(FLOAT[] diagonal, FLOAT[] subdiagonal, FLOAT[,] matrix) {
            int num = 32;
            int num2 = diagonal.Length;
            for (int i = 0; i < num2; i++) {
                int j;
                for (j = 0; j < num; j++) {
                    int k;
                    for (k = i; k <= num2 - 2; k++) {
                        FLOAT num3 = Math.Abs(diagonal[k]) + Math.Abs(diagonal[k + 1]);
                        if (Math.Abs(subdiagonal[k]) + num3 == num3) {
                            break;
                        }
                    }

                    if (k == i) {
                        break;
                    }

                    FLOAT num4 = (diagonal[i + 1] - diagonal[i]) / (2f * subdiagonal[i]);
                    FLOAT num5 = Math.Sqrt(num4 * num4 + 1f);
                    num4 = ((!(num4 < 0f))
                        ? (diagonal[k] - diagonal[i] + subdiagonal[i] / (num4 + num5))
                        : (diagonal[k] - diagonal[i] + subdiagonal[i] / (num4 - num5)));
                    FLOAT num6 = 1f;
                    FLOAT num7 = 1f;
                    FLOAT num8 = 0f;
                    for (int num9 = k - 1; num9 >= i; num9--) {
                        FLOAT num10 = num6 * subdiagonal[num9];
                        FLOAT num11 = num7 * subdiagonal[num9];
                        if (Math.Abs(num10) >= Math.Abs(num4)) {
                            num7 = num4 / num10;
                            num5 = Math.Sqrt(num7 * num7 + 1f);
                            subdiagonal[num9 + 1] = num10 * num5;
                            num6 = 1f / num5;
                            num7 *= num6;
                        }
                        else {
                            num6 = num10 / num4;
                            num5 = Math.Sqrt(num6 * num6 + 1f);
                            subdiagonal[num9 + 1] = num4 * num5;
                            num7 = 1f / num5;
                            num6 *= num7;
                        }

                        num4 = diagonal[num9 + 1] - num8;
                        num5 = (diagonal[num9] - num4) * num6 + 2f * num11 * num7;
                        num8 = num6 * num5;
                        diagonal[num9 + 1] = num4 + num8;
                        num4 = num7 * num5 - num11;
                        for (int l = 0; l < num2; l++) {
                            num10 = matrix[l, num9 + 1];
                            matrix[l, num9 + 1] = num6 * matrix[l, num9] + num7 * num10;
                            matrix[l, num9] = num7 * matrix[l, num9] - num6 * num10;
                        }
                    }

                    diagonal[i] -= num8;
                    subdiagonal[i] = num4;
                    subdiagonal[k] = 0f;
                }

                if (j == num) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sort eigenvalues from smallest to largest.
        /// </summary>
        private static void IncreasingSort(FLOAT[] diagonal, FLOAT[] subdiagonal, FLOAT[,] matrix, ref bool isRotation) {
            int num = diagonal.Length;
            for (int i = 0; i <= num - 2; i++) {
                int num2 = i;
                FLOAT num3 = diagonal[num2];
                for (int j = i + 1; j < num; j++) {
                    if (diagonal[j] < num3) {
                        num2 = j;
                        num3 = diagonal[num2];
                    }
                }

                if (num2 != i) {
                    diagonal[num2] = diagonal[i];
                    diagonal[i] = num3;
                    for (int j = 0; j < num; j++) {
                        FLOAT num4 = matrix[j, i];
                        matrix[j, i] = matrix[j, num2];
                        matrix[j, num2] = num4;
                        isRotation = !isRotation;
                    }
                }
            }
        }

        /// <summary>
        /// Sort eigenvalues from largest to smallest.
        /// </summary>
        private static void DecreasingSort(FLOAT[] diagonal, FLOAT[] subdiagonal, FLOAT[,] matrix, ref bool isRotation) {
            int num = diagonal.Length;
            for (int i = 0; i <= num - 2; i++) {
                int num2 = i;
                FLOAT num3 = diagonal[num2];
                for (int j = i + 1; j < num; j++) {
                    if (diagonal[j] > num3) {
                        num2 = j;
                        num3 = diagonal[num2];
                    }
                }

                if (num2 != i) {
                    diagonal[num2] = diagonal[i];
                    diagonal[i] = num3;
                    for (int j = 0; j < num; j++) {
                        FLOAT num4 = matrix[j, i];
                        matrix[j, i] = matrix[j, num2];
                        matrix[j, num2] = num4;
                        isRotation = !isRotation;
                    }
                }
            }
        }

        private static void GuaranteeRotation(FLOAT[,] matrix, bool isRotation) {
            if (!isRotation) {
                int length = matrix.GetLength(0);
                for (int i = 0; i < length; i++) {
                    matrix[i, 0] = 0f - matrix[i, 0];
                }
            }
        }

        /// <summary>
        /// Solve the eigensystem. Set increasingSort to true when you want
        /// the eigenvalues to be sorted in increasing order (from smallest to largest);
        /// otherwise, the eigenvalues are sorted in decreasing order (from largest to smallest).
        /// </summary>
        /// <param name="symmetricSquareMatrix">Matrix must be square and symmetric. Matrix size must be &gt;= 2.</param>
        /// <param name="increasingSort">true for increasing sort, false for decreasing sort.</param>
        /// <returns>Data containing eigenvalues and eigenvectors or null if matrix is non-square or size is &lt; 2.</returns>
        public static EigenData Solve(FLOAT[,] symmetricSquareMatrix, bool increasingSort) {
            int length;
            if ((length = symmetricSquareMatrix.GetLength(0)) != symmetricSquareMatrix.GetLength(1)) {
                return null;
            }

            if (length < 2) {
                return null;
            }

            FLOAT[,] array = new FLOAT[length, length];
            Buffer.BlockCopy(symmetricSquareMatrix, 0, array, 0, symmetricSquareMatrix.Length * 4);
            FLOAT[] diagonal = new FLOAT[length];
            FLOAT[] subdiagonal = new FLOAT[length];
            bool isRotation;
            switch (length) {
                case 2:
                    Tridiagonal2(diagonal, subdiagonal, array, out isRotation);
                    break;
                case 3:
                    Tridiagonal3(diagonal, subdiagonal, array, out isRotation);
                    break;
                default:
                    TridiagonalN(diagonal, subdiagonal, array, out isRotation);
                    break;
            }

            QLAlgorithm(diagonal, subdiagonal, array);
            if (increasingSort) {
                IncreasingSort(diagonal, subdiagonal, array, ref isRotation);
            }
            else {
                DecreasingSort(diagonal, subdiagonal, array, ref isRotation);
            }

            GuaranteeRotation(array, isRotation);
            return new EigenData(diagonal, array);
        }
    }
}