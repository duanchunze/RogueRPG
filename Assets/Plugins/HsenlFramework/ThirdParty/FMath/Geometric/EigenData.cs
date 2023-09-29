#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public class EigenData {
        private int _size;

        private FLOAT[] _diagonal;

        private FLOAT[,] _matrix;

        /// <summary>
        /// Eigen system size
        /// </summary>
        public int size => this._size;

        internal EigenData(FLOAT[] diagonal, FLOAT[,] matrix) {
            this._size = diagonal.Length;
            this._diagonal = diagonal;
            this._matrix = matrix;
        }

        /// <summary>
        /// Gets eigenvalue. Index must be 0&lt;=index&lt;Size
        /// </summary>
        public FLOAT GetEigenvalue(int index) {
            return this._diagonal[index];
        }

        /// <summary>
        /// Gets eigenvector. Use this only if eigen system was of 2x2 size. Index must be 0&lt;=index&lt;Size
        /// </summary>
        public FVector2 GetEigenvector2(int index) {
            if (this._size == 2) {
                FVector2 result = default(FVector2);
                for (int i = 0; i < this._size; i++) {
                    result[i] = this._matrix[i, index];
                }

                return result;
            }

            return FVector2.Zero;
        }

        /// <summary>
        /// Gets eigenvector. Use this only if eigen system was of 3x3 size. Index must be 0&lt;=index&lt;Size
        /// </summary>
        public FVector3 GetEigenvector3(int index) {
            if (this._size == 3) {
                FVector3 result = default(FVector3);
                for (int i = 0; i < this._size; i++) {
                    result[i] = this._matrix[i, index];
                }

                return result;
            }

            return FVector3.Zero;
        }

        /// <summary>
        /// Gets eigenvector. Size of the resulting array is equal to eigen system size. Index must be 0&lt;=index&lt;Size
        /// </summary>
        public FLOAT[] GetEigenvector(int index) {
            FLOAT[] array = new FLOAT[this._size];
            for (int i = 0; i < this._size; i++) {
                array[i] = this._matrix[i, index];
            }

            return array;
        }

        /// <summary>
        /// Gets eigenvector. Size of the array must match eigen system size. Method will fill in components of eigenvector into the array.
        /// Index must be 0&lt;=index&lt;Size
        /// </summary>
        public void GetEigenvector(int index, FLOAT[] out_eigenvector) {
            for (int i = 0; i < this._size; i++) {
                out_eigenvector[i] = this._matrix[i, index];
            }
        }
    }
}