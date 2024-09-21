using System;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public static class Integrator {
        private const int _degree = 5;

        private static FLOAT[] root = new FLOAT[5] { -0.906179845f, -0.5384693f, 0f, 0.5384693f, 0.906179845f };

        private static FLOAT[] coeff = new FLOAT[5] { 0.236926883f, 0.478628665f, 128f / 225f, 0.478628665f, 0.236926883f };

        /// <summary>
        /// Evaluates integral ∫f(x)dx on [a,b] interval using trapezoidal rule.
        /// sampleCount must be greater or equal to 2.
        /// </summary>
        public static FLOAT TrapezoidRule(Func<FLOAT, FLOAT> function, FLOAT a, FLOAT b, int sampleCount) {
            if (sampleCount < 2) {
                return FLOAT.NaN;
            }

            FLOAT num = (b - a) / (FLOAT)(sampleCount - 1);
            FLOAT num2 = 0.5f * (function(a) + function(b));
            for (int i = 1; i <= sampleCount - 2; i++) {
                num2 += function(a + (FLOAT)i * num);
            }

            return num2 * num;
        }

        /// <summary>
        /// Evaluates integral ∫f(x)dx on [a,b] interval using Romberg's method.
        /// Integration order must be positive (order &gt; 0).
        /// </summary>
        public static FLOAT RombergIntegral(Func<FLOAT, FLOAT> function, FLOAT a, FLOAT b, int order) {
            if (order <= 0) {
                return FLOAT.NaN;
            }

            FLOAT[,] array = new FLOAT[2, order];
            FLOAT num = b - a;
            array[0, 0] = 0.5f * num * (function(a) + function(b));
            int num2 = 2;
            int num3 = 1;
            while (num2 <= order) {
                FLOAT num4 = 0f;
                for (int i = 1; i <= num3; i++) {
                    num4 += function(a + num * ((FLOAT)i - 0.5f));
                }

                array[1, 0] = 0.5f * (array[0, 0] + num * num4);
                int num5 = 1;
                int num6 = 4;
                while (num5 < num2) {
                    array[1, num5] = ((FLOAT)num6 * array[1, num5 - 1] - array[0, num5 - 1]) / (FLOAT)(num6 - 1);
                    num5++;
                    num6 *= 4;
                }

                for (int i = 0; i < num2; i++) {
                    array[0, i] = array[1, i];
                }

                num2++;
                num3 *= 2;
                num *= 0.5f;
            }

            return array[0, order - 1];
        }

        /// <summary>
        /// Evaluates integral ∫f(x)dx on [a,b] interval using Gaussian quadrature rule (five Legendre polynomials).
        /// </summary>
        public static FLOAT GaussianQuadrature(Func<FLOAT, FLOAT> function, FLOAT a, FLOAT b) {
            FLOAT num = 0.5f * (b - a);
            FLOAT num2 = 0.5f * (b + a);
            FLOAT num3 = 0f;
            for (int i = 0; i < 5; i++) {
                num3 += coeff[i] * function(num * root[i] + num2);
            }

            return num3 * num;
        }
    }
}