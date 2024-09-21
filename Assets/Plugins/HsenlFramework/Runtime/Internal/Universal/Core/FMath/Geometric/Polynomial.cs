#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Represents n-degree polynomial of one variable
    /// </summary>
    public class Polynomial {
        private int _degree;

        private FLOAT[] _coeffs;

        /// <summary>
        /// Gets or sets polynomial degree (0 - constant, 1 - linear, 2 - quadratic, etc).
        /// When set, recreates coefficient array thus all coefficients become 0.
        /// </summary>
        public int degree {
            get { return this._degree; }
            set {
                this._degree = value;
                this._coeffs = new FLOAT[this._degree + 1];
            }
        }

        /// <summary>
        /// Gets or sets polynomial coefficient.
        /// </summary>
        /// <param name="index">Valid index is 0&lt;=index&lt;=Degree</param>
        public FLOAT this[int index] {
            get { return this._coeffs[index]; }
            set { this._coeffs[index] = value; }
        }

        /// <summary>
        /// Creates polynomial of specified degree. Use indexer to set coefficients.
        /// Coefficients order is from smallest order to highest order, e.g for quadratic equation it's:
        /// c0 + c1*x + c2*x^2, coefficients array will be [c0,c1,c2].
        /// </summary>
        /// <param name="degree">Must be &gt;= 0!</param>
        public Polynomial(int degree) {
            this.degree = degree;
        }

        /// <summary>
        /// Copies the polynomial
        /// </summary>
        public Polynomial DeepCopy() {
            Polynomial polynomial = new Polynomial(this._degree);
            for (int i = 0; i <= this._degree; i++) {
                polynomial._coeffs[i] = this._coeffs[i];
            }

            return polynomial;
        }

        /// <summary>
        /// Returns derivative of the current polynomial. Formula is:
        /// p (x) = c0 + c1*x + c2*x^2 + ... + cn*x^n
        /// p'(x) = c1 + 2*c2*x + 3*c3*x^2 + ... + n*cn*x^(n-1)
        /// </summary>
        public Polynomial CalcDerivative() {
            if (this._degree > 0) {
                Polynomial polynomial = new Polynomial(this._degree - 1);
                int num = 0;
                int num2 = 1;
                while (num < this._degree) {
                    polynomial._coeffs[num] = (FLOAT)num2 * this._coeffs[num2];
                    num++;
                    num2++;
                }

                return polynomial;
            }

            Polynomial polynomial2 = new Polynomial(0);
            polynomial2._coeffs[0] = 0f;
            return polynomial2;
        }

        /// <summary>
        /// Computes inversion of the current polynomial ( invpoly[i] = poly[degree-i] for 0 &lt;= i &lt;= degree ).
        /// </summary>
        public Polynomial CalcInversion() {
            Polynomial polynomial = new Polynomial(this._degree);
            for (int i = 0; i <= this._degree; i++) {
                polynomial._coeffs[i] = this._coeffs[this._degree - i];
            }

            return polynomial;
        }

        /// <summary>
        /// Reduce the degree by eliminating all (nearly) zero leading coefficients
        /// and by making the leading coefficient one.  The input parameter is
        /// the threshold for specifying that a coefficient is effectively zero.
        /// </summary>
        public void Compress() {
            FLOAT epsilon = 1E-05f;
            int num = this._degree;
            int num2 = num;
            while (num2 >= 0 && Math.Abs(this._coeffs[num2]) <= epsilon) {
                num--;
                num2--;
            }

            if (num >= 0) {
                this._degree = num;
                FLOAT num3 = 1f / this._coeffs[this._degree];
                this._coeffs[this._degree] = 1f;
                for (int i = 0; i < this._degree; i++) {
                    this._coeffs[i] *= num3;
                }
            }
        }

        /// <summary>
        /// Reduce the degree by eliminating all (nearly) zero leading coefficients
        /// and by making the leading coefficient one.  The input parameter is
        /// the threshold for specifying that a coefficient is effectively zero.
        /// </summary>
        public void Compress(FLOAT epsilon) {
            int num = this._degree;
            int num2 = num;
            while (num2 >= 0 && Math.Abs(this._coeffs[num2]) <= epsilon) {
                num--;
                num2--;
            }

            if (num >= 0) {
                this._degree = num;
                FLOAT num3 = 1f / this._coeffs[this._degree];
                this._coeffs[this._degree] = 1f;
                for (int i = 0; i < this._degree; i++) {
                    this._coeffs[i] *= num3;
                }
            }
        }

        /// <summary>
        /// Evaluates the polynomial
        /// </summary>
        public FLOAT Eval(FLOAT t) {
            FLOAT num = this._coeffs[this._degree];
            for (int num2 = this._degree - 1; num2 >= 0; num2--) {
                num *= t;
                num += this._coeffs[num2];
            }

            return num;
        }
    }
}