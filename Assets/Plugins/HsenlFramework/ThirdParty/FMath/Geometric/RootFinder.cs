using System;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public static class RootFinder {
        private class PolyRootFinder {
            private int _count;

            private int _maxRoot;

            private FLOAT[] _roots;

            private FLOAT _epsilon;

            public FLOAT[] roots => this._roots;

            public PolyRootFinder(FLOAT epsilon) {
                this._count = 0;
                this._maxRoot = 4;
                this._roots = new FLOAT[this._maxRoot];
                this._epsilon = epsilon;
            }

            public bool Bisection(Polynomial poly, FLOAT xMin, FLOAT xMax, int digits, out FLOAT root) {
                FLOAT num = poly.Eval(xMin);
                if (FMath.Abs(num) <= 1E-05f) {
                    root = xMin;
                    return true;
                }

                FLOAT num2 = poly.Eval(xMax);
                if (FMath.Abs(num2) <= 1E-05f) {
                    root = xMax;
                    return true;
                }

                root = FLOAT.NaN;
                if (num * num2 > 0f) {
                    return false;
                }

                FLOAT num3 = FMath.Log(xMax - xMin);
                FLOAT num4 = (FLOAT)digits * FMath.Log(10f);
                FLOAT num5 = (num3 + num4) / FMath.Log(2f);
                int num6 = (int)(num5 + 0.5f);
                for (int i = 0; i < num6; i++) {
                    root = 0.5f * (xMin + xMax);
                    FLOAT num7 = poly.Eval(root);
                    FLOAT num8 = num7 * num;
                    if (num8 < 0f) {
                        xMax = root;
                        num2 = num7;
                        continue;
                    }

                    if (!(num8 > 0f)) {
                        break;
                    }

                    xMin = root;
                    num = num7;
                }

                return true;
            }

            public bool Find(Polynomial poly, FLOAT xMin, FLOAT xMax, int digits) {
                if (poly.degree > this._maxRoot) {
                    this._maxRoot = poly.degree;
                    this._roots = new FLOAT[this._maxRoot];
                }

                FLOAT root;
                if (poly.degree == 1) {
                    if (this.Bisection(poly, xMin, xMax, digits, out root) && root != FLOAT.NaN) {
                        this._count = 1;
                        this._roots[0] = root;
                        return true;
                    }

                    this._count = 0;
                    return false;
                }

                Polynomial poly2 = poly.CalcDerivative();
                this.Find(poly2, xMin, xMax, digits);
                int num = 0;
                FLOAT[] array = new FLOAT[this._count + 1];
                if (this._count > 0) {
                    if (this.Bisection(poly, xMin, this._roots[0], digits, out root)) {
                        array[num++] = root;
                    }

                    for (int i = 0; i <= this._count - 2; i++) {
                        if (this.Bisection(poly, this._roots[i], this._roots[i + 1], digits, out root)) {
                            array[num++] = root;
                        }
                    }

                    if (this.Bisection(poly, this._roots[this._count - 1], xMax, digits, out root)) {
                        array[num++] = root;
                    }
                }
                else if (this.Bisection(poly, xMin, xMax, digits, out root)) {
                    array[num++] = root;
                }

                if (num > 0) {
                    this._count = 1;
                    this._roots[0] = array[0];
                    for (int i = 1; i < num; i++) {
                        FLOAT f = array[i] - array[i - 1];
                        if (FMath.Abs(f) > this._epsilon) {
                            this._roots[this._count++] = array[i];
                        }
                    }
                }
                else {
                    this._count = 0;
                }

                return this._count > 0;
            }
        }

        // private static readonly FLOAT third = 0.333333343f;
        //
        // private static readonly FLOAT twentySeventh = 0.0370370373f;

        private static FLOAT sqrt3 = FMath.Sqrt(3f);

        /// <summary>
        /// This is an implementation of Brent's
        /// Method for computing a root of a function on an interval [x0,x1] for
        /// which f(x0)*f(x1) &lt; 0 (i.e. values of the function must have
        /// different signs on interval ends).  The method uses inverse quadratic interpolation
        /// to generate a root estimate but falls back to inverse linear
        /// interpolation (secant method) if necessary.  Moreover, based on
        /// previous iterates, the method will fall back to bisection when it
        /// appears the interpolated estimate is not of sufficient quality.
        ///
        /// This will compute a root, if any, on the interval [x0,x1].  The function returns
        /// 'true' when the root is found, in which case 'BrentsRoot.X' is the root. The function
        /// returns 'false' when the interval is invalid (x1 &lt;= x0) or when the interval
        /// does not bound a root (f(x0)*f(x1) &gt; 0).
        /// </summary>
        /// <param name="function">The function whose root is desired. The function accepts one real number and returns real number.</param>
        /// <param name="x0">Interval left border</param>
        /// <param name="x1">Interval right border</param>
        /// <param name="root">Out parameter containing root of the function.</param>
        /// <param name="maxIterations">The maximum number of iterations used to locate a root. Should be positive number.</param>
        /// <param name="negativeTolerance">The root estimate x is accepted when the function value f(x)
        /// satisfies negativeTolerance &lt;= f(x) &lt;= positiveTolerance. negativeTolerance must be non-positive.</param>
        /// <param name="positiveTolerance">The root estimate x is accepted when the function value f(x)
        /// satisfies negativeTolerance &lt;= f(x) &lt;= positiveTolerance. positiveTolerance must be non-negative.</param>
        /// <param name="stepTolerance">Brent's Method requires additional tests before an interpolated
        /// x-value is accepted as the next root estimate.  One of these tests
        /// compares the difference of consecutive iterates and requires it
        /// to be larger than a user-specified x-tolerance (to ensure progress
        /// is made).  This parameter is that tolerance.</param>
        /// <param name="segmentTolerance">The root search is allowed to terminate when the current
        /// subinterval [xsub0,xsub1] is sufficiently small, say,
        /// |xsub1 - xsub0| &lt;= tolerance.  This parameter is that tolerance.</param>
        /// <returns>True if root is found, false otherwise.</returns>
        public static bool BrentsMethod(Func<FLOAT, FLOAT> function, FLOAT x0, FLOAT x1, out BrentsRoot root,
            int maxIterations = 128) {
            FLOAT negativeTolerance = -1E-05f;
            FLOAT positiveTolerance = 1E-05f;
            FLOAT stepTolerance = 1E-05f;
            FLOAT segmentTolerance = 1E-05f;
            root.iterations = 0;
            root.exceededMaxIterations = false;
            if (x1 <= x0) {
                root.x = FLOAT.NaN;
                return false;
            }

            FLOAT num = function(x0);
            if (negativeTolerance <= num && num <= positiveTolerance) {
                root.x = x0;
                return true;
            }

            FLOAT num2 = function(x1);
            if (negativeTolerance <= num2 && num2 <= positiveTolerance) {
                root.x = x1;
                return true;
            }

            if (num * num2 >= 0f) {
                root.x = FLOAT.NaN;
                return false;
            }

            if (FMath.Abs(num) < FMath.Abs(num2)) {
                FLOAT num3 = x0;
                x0 = x1;
                x1 = num3;
                num3 = num;
                num = num2;
                num2 = num3;
            }

            FLOAT num4 = x0;
            FLOAT num5 = x0;
            FLOAT num6 = num;
            bool flag = true;
            int i;
            for (i = 0; i < maxIterations; i++) {
                FLOAT num7 = num - num2;
                FLOAT num8 = num - num6;
                FLOAT num9 = num2 - num6;
                FLOAT num10 = 1f / num7;
                FLOAT num13;
                if (num8 != 0f && num9 != 0f) {
                    FLOAT num11 = 1f / num8;
                    FLOAT num12 = 1f / num9;
                    num13 = x0 * num2 * num6 * num10 * num11 - x1 * num * num6 * num10 * num12 +
                            num4 * num * num2 * num11 * num12;
                }
                else {
                    num13 = (x1 * num - x0 * num2) * num10;
                }

                FLOAT num14 = num13 - 0.75f * x0 - 0.25f * x1;
                FLOAT num15 = num13 - x1;
                FLOAT num16 = FMath.Abs(num15);
                FLOAT num17 = FMath.Abs(x1 - num4);
                FLOAT num18 = FMath.Abs(num4 - num5);
                // bool flag2 = false;
                if (num14 * num15 > 0f ||
                    ((!flag)
                        ? (num16 >= 0.5f * num18 || num18 <= stepTolerance)
                        : (num16 >= 0.5f * num17 || num17 <= stepTolerance))) {
                    num13 = 0.5f * (x0 + x1);
                    flag = true;
                }
                else {
                    flag = false;
                }

                FLOAT num19 = function(num13);
                if (negativeTolerance <= num19 && num19 <= positiveTolerance) {
                    root.x = num13;
                    root.iterations = i;
                    return true;
                }

                num5 = num4;
                num4 = x1;
                num6 = num2;
                if (num * num19 < 0f) {
                    x1 = num13;
                    num2 = num19;
                }
                else {
                    x0 = num13;
                    num = num19;
                }

                if (FMath.Abs(x1 - x0) <= segmentTolerance) {
                    root.x = x1;
                    root.iterations = i;
                    return true;
                }

                if (FMath.Abs(num) < FMath.Abs(num2)) {
                    FLOAT num20 = x0;
                    x0 = x1;
                    x1 = num20;
                    num20 = num;
                    num = num2;
                    num2 = num20;
                }
            }

            root.x = x1;
            root.iterations = i;
            root.exceededMaxIterations = true;
            return true;
        }

        /// <summary>
        /// This is an implementation of Brent's
        /// Method for computing a root of a function on an interval [x0,x1] for
        /// which f(x0)*f(x1) &lt; 0 (i.e. values of the function must have
        /// different signs on interval ends).  The method uses inverse quadratic interpolation
        /// to generate a root estimate but falls back to inverse linear
        /// interpolation (secant method) if necessary.  Moreover, based on
        /// previous iterates, the method will fall back to bisection when it
        /// appears the interpolated estimate is not of sufficient quality.
        ///
        /// This will compute a root, if any, on the interval [x0,x1].  The function returns
        /// 'true' when the root is found, in which case 'BrentsRoot.X' is the root. The function
        /// returns 'false' when the interval is invalid (x1 &lt;= x0) or when the interval
        /// does not bound a root (f(x0)*f(x1) &gt; 0).
        /// </summary>
        /// <param name="function">The function whose root is desired. The function accepts one real number and returns real number.</param>
        /// <param name="x0">Interval left border</param>
        /// <param name="x1">Interval right border</param>
        /// <param name="root">Out parameter containing root of the function.</param>
        /// <param name="maxIterations">The maximum number of iterations used to locate a root. Should be positive number.</param>
        /// <param name="negativeTolerance">The root estimate x is accepted when the function value f(x)
        /// satisfies negativeTolerance &lt;= f(x) &lt;= positiveTolerance. negativeTolerance must be non-positive.</param>
        /// <param name="positiveTolerance">The root estimate x is accepted when the function value f(x)
        /// satisfies negativeTolerance &lt;= f(x) &lt;= positiveTolerance. positiveTolerance must be non-negative.</param>
        /// <param name="stepTolerance">Brent's Method requires additional tests before an interpolated
        /// x-value is accepted as the next root estimate.  One of these tests
        /// compares the difference of consecutive iterates and requires it
        /// to be larger than a user-specified x-tolerance (to ensure progress
        /// is made).  This parameter is that tolerance.</param>
        /// <param name="segmentTolerance">The root search is allowed to terminate when the current
        /// subinterval [xsub0,xsub1] is sufficiently small, say,
        /// |xsub1 - xsub0| &lt;= tolerance.  This parameter is that tolerance.</param>
        /// <returns>True if root is found, false otherwise.</returns>
        public static bool BrentsMethod(Func<FLOAT, FLOAT> function, FLOAT x0, FLOAT x1, out BrentsRoot root,
            FLOAT negativeTolerance, FLOAT positiveTolerance, FLOAT stepTolerance, FLOAT segmentTolerance,
            int maxIterations = 128) {
            root.iterations = 0;
            root.exceededMaxIterations = false;
            if (x1 <= x0) {
                root.x = FLOAT.NaN;
                return false;
            }

            FLOAT num = function(x0);
            if (negativeTolerance <= num && num <= positiveTolerance) {
                root.x = x0;
                return true;
            }

            FLOAT num2 = function(x1);
            if (negativeTolerance <= num2 && num2 <= positiveTolerance) {
                root.x = x1;
                return true;
            }

            if (num * num2 >= 0f) {
                root.x = FLOAT.NaN;
                return false;
            }

            if (FMath.Abs(num) < FMath.Abs(num2)) {
                FLOAT num3 = x0;
                x0 = x1;
                x1 = num3;
                num3 = num;
                num = num2;
                num2 = num3;
            }

            FLOAT num4 = x0;
            FLOAT num5 = x0;
            FLOAT num6 = num;
            bool flag = true;
            int i;
            for (i = 0; i < maxIterations; i++) {
                FLOAT num7 = num - num2;
                FLOAT num8 = num - num6;
                FLOAT num9 = num2 - num6;
                FLOAT num10 = 1f / num7;
                FLOAT num13;
                if (num8 != 0f && num9 != 0f) {
                    FLOAT num11 = 1f / num8;
                    FLOAT num12 = 1f / num9;
                    num13 = x0 * num2 * num6 * num10 * num11 - x1 * num * num6 * num10 * num12 +
                            num4 * num * num2 * num11 * num12;
                }
                else {
                    num13 = (x1 * num - x0 * num2) * num10;
                }

                FLOAT num14 = num13 - 0.75f * x0 - 0.25f * x1;
                FLOAT num15 = num13 - x1;
                FLOAT num16 = FMath.Abs(num15);
                FLOAT num17 = FMath.Abs(x1 - num4);
                FLOAT num18 = FMath.Abs(num4 - num5);
                // bool flag2 = false;
                if (num14 * num15 > 0f ||
                    ((!flag)
                        ? (num16 >= 0.5f * num18 || num18 <= stepTolerance)
                        : (num16 >= 0.5f * num17 || num17 <= stepTolerance))) {
                    num13 = 0.5f * (x0 + x1);
                    flag = true;
                }
                else {
                    flag = false;
                }

                FLOAT num19 = function(num13);
                if (negativeTolerance <= num19 && num19 <= positiveTolerance) {
                    root.x = num13;
                    root.iterations = i;
                    return true;
                }

                num5 = num4;
                num4 = x1;
                num6 = num2;
                if (num * num19 < 0f) {
                    x1 = num13;
                    num2 = num19;
                }
                else {
                    x0 = num13;
                    num = num19;
                }

                if (FMath.Abs(x1 - x0) <= segmentTolerance) {
                    root.x = x1;
                    root.iterations = i;
                    return true;
                }

                if (FMath.Abs(num) < FMath.Abs(num2)) {
                    FLOAT num20 = x0;
                    x0 = x1;
                    x1 = num20;
                    num20 = num;
                    num = num2;
                    num2 = num20;
                }
            }

            root.x = x1;
            root.iterations = i;
            root.exceededMaxIterations = true;
            return true;
        }

        /// <summary>
        /// Linear equations: c1*x+c0 = 0
        /// </summary>
        public static bool Linear(FLOAT c0, FLOAT c1, out FLOAT root) {
            FLOAT epsilon = 1E-05f;
            if (FMath.Abs(c1) >= epsilon) {
                root = (0f - c0) / c1;
                return true;
            }

            root = FLOAT.NaN;
            return false;
        }

        /// <summary>
        /// Linear equations: c1*x+c0 = 0
        /// </summary>
        public static bool Linear(FLOAT c0, FLOAT c1, out FLOAT root, FLOAT epsilon) {
            if (FMath.Abs(c1) >= epsilon) {
                root = (0f - c0) / c1;
                return true;
            }

            root = FLOAT.NaN;
            return false;
        }

        /// <summary>
        /// Quadratic equations: c2*x^2+c1*x+c0 = 0
        /// </summary>
        public static bool Quadratic(FLOAT c0, FLOAT c1, FLOAT c2, out QuadraticRoots roots) {
            FLOAT epsilon = 1E-05f;
            if (FMath.Abs(c2) <= epsilon) {
                FLOAT root;
                bool flag = Linear(c0, c1, out root, epsilon);
                if (flag) {
                    roots.x0 = root;
                    roots.x1 = FLOAT.NaN;
                    roots.rootCount = 1;
                }
                else {
                    roots.x0 = FLOAT.NaN;
                    roots.x1 = FLOAT.NaN;
                    roots.rootCount = 0;
                }

                return flag;
            }

            FLOAT num = c1 * c1 - 4f * c0 * c2;
            if (FMath.Abs(num) <= epsilon) {
                num = 0f;
            }

            if (num < 0f) {
                roots.x0 = FLOAT.NaN;
                roots.x1 = FLOAT.NaN;
                roots.rootCount = 0;
                return false;
            }

            FLOAT num2 = 0.5f / c2;
            if (num > 0f) {
                num = FMath.Sqrt(num);
                roots.x0 = num2 * (0f - c1 - num);
                roots.x1 = num2 * (0f - c1 + num);
                roots.rootCount = 2;
            }
            else {
                roots.x0 = (0f - num2) * c1;
                roots.x1 = FLOAT.NaN;
                roots.rootCount = 1;
            }

            return true;
        }

        /// <summary>
        /// Quadratic equations: c2*x^2+c1*x+c0 = 0
        /// </summary>
        public static bool Quadratic(FLOAT c0, FLOAT c1, FLOAT c2, out QuadraticRoots roots, FLOAT epsilon) {
            if (FMath.Abs(c2) <= epsilon) {
                FLOAT root;
                bool flag = Linear(c0, c1, out root, epsilon);
                if (flag) {
                    roots.x0 = root;
                    roots.x1 = FLOAT.NaN;
                    roots.rootCount = 1;
                }
                else {
                    roots.x0 = FLOAT.NaN;
                    roots.x1 = FLOAT.NaN;
                    roots.rootCount = 0;
                }

                return flag;
            }

            FLOAT num = c1 * c1 - 4f * c0 * c2;
            if (FMath.Abs(num) <= epsilon) {
                num = 0f;
            }

            if (num < 0f) {
                roots.x0 = FLOAT.NaN;
                roots.x1 = FLOAT.NaN;
                roots.rootCount = 0;
                return false;
            }

            FLOAT num2 = 0.5f / c2;
            if (num > 0f) {
                num = FMath.Sqrt(num);
                roots.x0 = num2 * (0f - c1 - num);
                roots.x1 = num2 * (0f - c1 + num);
                roots.rootCount = 2;
            }
            else {
                roots.x0 = (0f - num2) * c1;
                roots.x1 = FLOAT.NaN;
                roots.rootCount = 1;
            }

            return true;
        }

        /// <summary>
        /// Cubic equations: c3*x^3+c2*x^2+c1*x+c0 = 0
        /// </summary>
        public static bool Cubic(FLOAT c0, FLOAT c1, FLOAT c2, FLOAT c3, out CubicRoots roots) {
            FLOAT epsilon = 1E-05f;
            if (FMath.Abs(c3) <= epsilon) {
                QuadraticRoots roots2;
                bool flag = Quadratic(c0, c1, c2, out roots2, epsilon);
                if (flag) {
                    roots.x0 = roots2.x0;
                    roots.x1 = roots2.x1;
                    roots.x2 = FLOAT.NaN;
                    roots.rootCount = roots2.rootCount;
                }
                else {
                    roots.x0 = FLOAT.NaN;
                    roots.x1 = FLOAT.NaN;
                    roots.x2 = FLOAT.NaN;
                    roots.rootCount = 0;
                }

                return flag;
            }

            FLOAT num = 1f / c3;
            c2 *= num;
            c1 *= num;
            c0 *= num;
            FLOAT num2 = 0.333333343f * c2;
            FLOAT num3 = c1 - c2 * num2;
            FLOAT num4 = c0 + c2 * (2f * c2 * c2 - 9f * c1) * 0.0370370373f;
            FLOAT num5 = 0.5f * num4;
            FLOAT num6 = num5 * num5 + num3 * num3 * num3 * 0.0370370373f;
            if (FMath.Abs(num6) <= epsilon) {
                num6 = 0f;
            }

            if (num6 > 0f) {
                num6 = FMath.Sqrt(num6);
                FLOAT num7 = 0f - num5 + num6;
                if (num7 >= 0f) {
                    roots.x0 = FMath.Pow(num7, 0.333333343f);
                }
                else {
                    roots.x0 = 0f - FMath.Pow(0f - num7, 0.333333343f);
                }

                num7 = 0f - num5 - num6;
                if (num7 >= 0f) {
                    roots.x0 += FMath.Pow(num7, 0.333333343f);
                }
                else {
                    roots.x0 -= FMath.Pow(0f - num7, 0.333333343f);
                }

                roots.x0 -= num2;
                roots.x1 = FLOAT.NaN;
                roots.x2 = FLOAT.NaN;
                roots.rootCount = 1;
            }
            else if (num6 < 0f) {
                FLOAT num8 = FMath.Sqrt(-0.333333343f * num3);
                FLOAT f = 0.333333343f * FMath.Atan2(FMath.Sqrt(0f - num6), 0f - num5);
                FLOAT num9 = FMath.Cos(f);
                FLOAT num10 = FMath.Sin(f);
                roots.x0 = 2f * num8 * num9 - num2;
                roots.x1 = (0f - num8) * (num9 + sqrt3 * num10) - num2;
                roots.x2 = (0f - num8) * (num9 - sqrt3 * num10) - num2;
                roots.rootCount = 3;
            }
            else {
                FLOAT num11 = ((!(num5 >= 0f))
                    ? FMath.Pow(0f - num5, 0.333333343f)
                    : (0f - FMath.Pow(num5, 0.333333343f)));
                roots.x0 = 2f * num11 - num2;
                roots.x1 = 0f - num11 - num2;
                roots.x2 = roots.x1;
                roots.rootCount = 3;
            }

            return true;
        }

        /// <summary>
        /// Cubic equations: c3*x^3+c2*x^2+c1*x+c0 = 0
        /// </summary>
        public static bool Cubic(FLOAT c0, FLOAT c1, FLOAT c2, FLOAT c3, out CubicRoots roots, FLOAT epsilon) {
            if (FMath.Abs(c3) <= epsilon) {
                QuadraticRoots roots2;
                bool flag = Quadratic(c0, c1, c2, out roots2, epsilon);
                if (flag) {
                    roots.x0 = roots2.x0;
                    roots.x1 = roots2.x1;
                    roots.x2 = FLOAT.NaN;
                    roots.rootCount = roots2.rootCount;
                }
                else {
                    roots.x0 = FLOAT.NaN;
                    roots.x1 = FLOAT.NaN;
                    roots.x2 = FLOAT.NaN;
                    roots.rootCount = 0;
                }

                return flag;
            }

            FLOAT num = 1f / c3;
            c2 *= num;
            c1 *= num;
            c0 *= num;
            FLOAT num2 = 0.333333343f * c2;
            FLOAT num3 = c1 - c2 * num2;
            FLOAT num4 = c0 + c2 * (2f * c2 * c2 - 9f * c1) * 0.0370370373f;
            FLOAT num5 = 0.5f * num4;
            FLOAT num6 = num5 * num5 + num3 * num3 * num3 * 0.0370370373f;
            if (FMath.Abs(num6) <= epsilon) {
                num6 = 0f;
            }

            if (num6 > 0f) {
                num6 = FMath.Sqrt(num6);
                FLOAT num7 = 0f - num5 + num6;
                if (num7 >= 0f) {
                    roots.x0 = FMath.Pow(num7, 0.333333343f);
                }
                else {
                    roots.x0 = 0f - FMath.Pow(0f - num7, 0.333333343f);
                }

                num7 = 0f - num5 - num6;
                if (num7 >= 0f) {
                    roots.x0 += FMath.Pow(num7, 0.333333343f);
                }
                else {
                    roots.x0 -= FMath.Pow(0f - num7, 0.333333343f);
                }

                roots.x0 -= num2;
                roots.x1 = FLOAT.NaN;
                roots.x2 = FLOAT.NaN;
                roots.rootCount = 1;
            }
            else if (num6 < 0f) {
                FLOAT num8 = FMath.Sqrt(-0.333333343f * num3);
                FLOAT f = 0.333333343f * FMath.Atan2(FMath.Sqrt(0f - num6), 0f - num5);
                FLOAT num9 = FMath.Cos(f);
                FLOAT num10 = FMath.Sin(f);
                roots.x0 = 2f * num8 * num9 - num2;
                roots.x1 = (0f - num8) * (num9 + sqrt3 * num10) - num2;
                roots.x2 = (0f - num8) * (num9 - sqrt3 * num10) - num2;
                roots.rootCount = 3;
            }
            else {
                FLOAT num11 = ((!(num5 >= 0f))
                    ? FMath.Pow(0f - num5, 0.333333343f)
                    : (0f - FMath.Pow(num5, 0.333333343f)));
                roots.x0 = 2f * num11 - num2;
                roots.x1 = 0f - num11 - num2;
                roots.x2 = roots.x1;
                roots.rootCount = 3;
            }

            return true;
        }

        /// <summary>
        /// Quartic equations: c4*x^4+c3*x^3+c2*x^2+c1*x+c0 = 0
        /// </summary>
        public static bool Quartic(FLOAT c0, FLOAT c1, FLOAT c2, FLOAT c3, FLOAT c4, out QuarticRoots roots) {
            FLOAT epsilon = 1E-05f;
            roots.x0 = FLOAT.NaN;
            roots.x1 = FLOAT.NaN;
            roots.x2 = FLOAT.NaN;
            roots.x3 = FLOAT.NaN;
            if (FMath.Abs(c4) <= epsilon) {
                CubicRoots roots2;
                bool flag = Cubic(c0, c1, c2, c3, out roots2, epsilon);
                if (flag) {
                    roots.x0 = roots2.x0;
                    roots.x1 = roots2.x1;
                    roots.x2 = roots2.x2;
                    roots.rootCount = roots2.rootCount;
                }
                else {
                    roots.rootCount = 0;
                }

                return flag;
            }

            FLOAT num = 1f / c4;
            c0 *= num;
            c1 *= num;
            c2 *= num;
            c3 *= num;
            FLOAT c5 = (0f - c3) * c3 * c0 + 4f * c2 * c0 - c1 * c1;
            FLOAT c6 = c3 * c1 - 4f * c0;
            FLOAT c7 = 0f - c2;
            Cubic(c5, c6, c7, 1f, out var roots3, epsilon);
            FLOAT x = roots3.x0;
            roots.rootCount = 0;
            FLOAT num2 = 0.25f * c3 * c3 - c2 + x;
            if (FMath.Abs(num2) <= epsilon) {
                num2 = 0f;
            }

            if (num2 > 0f) {
                FLOAT num3 = FMath.Sqrt(num2);
                FLOAT num4 = 0.75f * c3 * c3 - num3 * num3 - 2f * c2;
                FLOAT num5 = (4f * c3 * c2 - 8f * c1 - c3 * c3 * c3) / (4f * num3);
                FLOAT num6 = num4 + num5;
                FLOAT num7 = num4 - num5;
                if (FMath.Abs(num6) <= epsilon) {
                    num6 = 0f;
                }

                if (FMath.Abs(num7) <= epsilon) {
                    num7 = 0f;
                }

                if (num6 >= 0f) {
                    FLOAT num8 = FMath.Sqrt(num6);
                    roots.x0 = -0.25f * c3 + 0.5f * (num3 + num8);
                    roots.x1 = -0.25f * c3 + 0.5f * (num3 - num8);
                    roots.rootCount += 2;
                }

                if (num7 >= 0f) {
                    FLOAT num9 = FMath.Sqrt(num7);
                    if (roots.rootCount == 0) {
                        roots.x0 = -0.25f * c3 + 0.5f * (num9 - num3);
                        roots.x1 = -0.25f * c3 - 0.5f * (num9 + num3);
                    }
                    else {
                        roots.x2 = -0.25f * c3 + 0.5f * (num9 - num3);
                        roots.x3 = -0.25f * c3 - 0.5f * (num9 + num3);
                    }

                    roots.rootCount += 2;
                }
            }
            else if (num2 < 0f) {
                roots.rootCount = 0;
            }
            else {
                FLOAT num10 = x * x - 4f * c0;
                if (num10 >= 0f - epsilon) {
                    if (num10 < 0f) {
                        num10 = 0f;
                    }

                    num10 = 2f * FMath.Sqrt(num10);
                    FLOAT num11 = 0.75f * c3 * c3 - 2f * c2;
                    FLOAT num12 = num11 + num10;
                    if (num12 >= epsilon) {
                        FLOAT num13 = FMath.Sqrt(num12);
                        roots.x0 = -0.25f * c3 + 0.5f * num13;
                        roots.x1 = -0.25f * c3 - 0.5f * num13;
                        roots.rootCount += 2;
                    }

                    FLOAT num14 = num11 - num10;
                    if (num14 >= epsilon) {
                        FLOAT num15 = FMath.Sqrt(num14);
                        if (roots.rootCount == 0) {
                            roots.x0 = -0.25f * c3 + 0.5f * num15;
                            roots.x1 = -0.25f * c3 - 0.5f * num15;
                        }
                        else {
                            roots.x2 = -0.25f * c3 + 0.5f * num15;
                            roots.x3 = -0.25f * c3 - 0.5f * num15;
                        }

                        roots.rootCount += 2;
                    }
                }
            }

            return roots.rootCount > 0;
        }

        /// <summary>
        /// Quartic equations: c4*x^4+c3*x^3+c2*x^2+c1*x+c0 = 0
        /// </summary>
        public static bool Quartic(FLOAT c0, FLOAT c1, FLOAT c2, FLOAT c3, FLOAT c4, out QuarticRoots roots, FLOAT epsilon) {
            roots.x0 = FLOAT.NaN;
            roots.x1 = FLOAT.NaN;
            roots.x2 = FLOAT.NaN;
            roots.x3 = FLOAT.NaN;
            if (FMath.Abs(c4) <= epsilon) {
                CubicRoots roots2;
                bool flag = Cubic(c0, c1, c2, c3, out roots2, epsilon);
                if (flag) {
                    roots.x0 = roots2.x0;
                    roots.x1 = roots2.x1;
                    roots.x2 = roots2.x2;
                    roots.rootCount = roots2.rootCount;
                }
                else {
                    roots.rootCount = 0;
                }

                return flag;
            }

            FLOAT num = 1f / c4;
            c0 *= num;
            c1 *= num;
            c2 *= num;
            c3 *= num;
            FLOAT c5 = (0f - c3) * c3 * c0 + 4f * c2 * c0 - c1 * c1;
            FLOAT c6 = c3 * c1 - 4f * c0;
            FLOAT c7 = 0f - c2;
            Cubic(c5, c6, c7, 1f, out var roots3, epsilon);
            FLOAT x = roots3.x0;
            roots.rootCount = 0;
            FLOAT num2 = 0.25f * c3 * c3 - c2 + x;
            if (FMath.Abs(num2) <= epsilon) {
                num2 = 0f;
            }

            if (num2 > 0f) {
                FLOAT num3 = FMath.Sqrt(num2);
                FLOAT num4 = 0.75f * c3 * c3 - num3 * num3 - 2f * c2;
                FLOAT num5 = (4f * c3 * c2 - 8f * c1 - c3 * c3 * c3) / (4f * num3);
                FLOAT num6 = num4 + num5;
                FLOAT num7 = num4 - num5;
                if (FMath.Abs(num6) <= epsilon) {
                    num6 = 0f;
                }

                if (FMath.Abs(num7) <= epsilon) {
                    num7 = 0f;
                }

                if (num6 >= 0f) {
                    FLOAT num8 = FMath.Sqrt(num6);
                    roots.x0 = -0.25f * c3 + 0.5f * (num3 + num8);
                    roots.x1 = -0.25f * c3 + 0.5f * (num3 - num8);
                    roots.rootCount += 2;
                }

                if (num7 >= 0f) {
                    FLOAT num9 = FMath.Sqrt(num7);
                    if (roots.rootCount == 0) {
                        roots.x0 = -0.25f * c3 + 0.5f * (num9 - num3);
                        roots.x1 = -0.25f * c3 - 0.5f * (num9 + num3);
                    }
                    else {
                        roots.x2 = -0.25f * c3 + 0.5f * (num9 - num3);
                        roots.x3 = -0.25f * c3 - 0.5f * (num9 + num3);
                    }

                    roots.rootCount += 2;
                }
            }
            else if (num2 < 0f) {
                roots.rootCount = 0;
            }
            else {
                FLOAT num10 = x * x - 4f * c0;
                if (num10 >= 0f - epsilon) {
                    if (num10 < 0f) {
                        num10 = 0f;
                    }

                    num10 = 2f * FMath.Sqrt(num10);
                    FLOAT num11 = 0.75f * c3 * c3 - 2f * c2;
                    FLOAT num12 = num11 + num10;
                    if (num12 >= epsilon) {
                        FLOAT num13 = FMath.Sqrt(num12);
                        roots.x0 = -0.25f * c3 + 0.5f * num13;
                        roots.x1 = -0.25f * c3 - 0.5f * num13;
                        roots.rootCount += 2;
                    }

                    FLOAT num14 = num11 - num10;
                    if (num14 >= epsilon) {
                        FLOAT num15 = FMath.Sqrt(num14);
                        if (roots.rootCount == 0) {
                            roots.x0 = -0.25f * c3 + 0.5f * num15;
                            roots.x1 = -0.25f * c3 - 0.5f * num15;
                        }
                        else {
                            roots.x2 = -0.25f * c3 + 0.5f * num15;
                            roots.x3 = -0.25f * c3 - 0.5f * num15;
                        }

                        roots.rootCount += 2;
                    }
                }
            }

            return roots.rootCount > 0;
        }

        /// <summary>
        /// Gets roots bound of the given polynomial or -1 if polynomial is constant.
        /// </summary>
        public static FLOAT PolynomialBound(Polynomial poly) {
            FLOAT epsilon = 1E-05f;
            Polynomial polynomial = poly.DeepCopy();
            polynomial.Compress(epsilon);
            int degree = polynomial.degree;
            if (degree < 1) {
                return -1f;
            }

            FLOAT num = 1f / polynomial[degree];
            FLOAT num2 = 0f;
            for (int i = 0; i < degree; i++) {
                FLOAT num3 = FMath.Abs(polynomial[i]) * num;
                if (num3 > num2) {
                    num2 = num3;
                }
            }

            return 1f + num2;
        }

        /// <summary>
        /// Gets roots bound of the given polynomial or -1 if polynomial is constant.
        /// </summary>
        public static FLOAT PolynomialBound(Polynomial poly, FLOAT epsilon) {
            Polynomial polynomial = poly.DeepCopy();
            polynomial.Compress(epsilon);
            int degree = polynomial.degree;
            if (degree < 1) {
                return -1f;
            }

            FLOAT num = 1f / polynomial[degree];
            FLOAT num2 = 0f;
            for (int i = 0; i < degree; i++) {
                FLOAT num3 = FMath.Abs(polynomial[i]) * num;
                if (num3 > num2) {
                    num2 = num3;
                }
            }

            return 1f + num2;
        }

        /// <summary>
        /// General polynomial equation: Σ(c_i * x^i), where i=[0..degree]. Finds roots in the interval [xMin..xMax].
        /// </summary>
        /// <param name="poly">Polynomial whose roots to be found</param>
        /// <param name="xMin">Interval left border</param>
        /// <param name="xMax">Interval right border</param>
        /// <param name="roots">Roots of the polynomial</param>
        /// <param name="digits">Accuracy</param>
        /// <param name="epsilon">Small positive number</param>
        public static bool Polynomial(Polynomial poly, FLOAT xMin, FLOAT xMax, out FLOAT[] roots, int digits = 6) {
            FLOAT epsilon = 1E-05f;
            PolyRootFinder polyRootFinder = new PolyRootFinder(epsilon);
            if (polyRootFinder.Find(poly, xMin, xMax, digits)) {
                roots = polyRootFinder.roots;
                return true;
            }

            roots = new FLOAT[0];
            return false;
        }

        /// <summary>
        /// General polynomial equation: Σ(c_i * x^i), where i=[0..degree]. Finds roots in the interval [xMin..xMax].
        /// </summary>
        /// <param name="poly">Polynomial whose roots to be found</param>
        /// <param name="xMin">Interval left border</param>
        /// <param name="xMax">Interval right border</param>
        /// <param name="roots">Roots of the polynomial</param>
        /// <param name="digits">Accuracy</param>
        /// <param name="epsilon">Small positive number</param>
        public static bool Polynomial(Polynomial poly, FLOAT xMin, FLOAT xMax, out FLOAT[] roots, FLOAT epsilon,
            int digits = 6) {
            PolyRootFinder polyRootFinder = new PolyRootFinder(epsilon);
            if (polyRootFinder.Find(poly, xMin, xMax, digits)) {
                roots = polyRootFinder.roots;
                return true;
            }

            roots = new FLOAT[0];
            return false;
        }

        /// <summary>
        /// General polynomial equation: Σ(c_i * x^i), where i=[0..degree].
        /// </summary>
        /// <param name="poly">Polynomial whose roots to be found</param>
        /// <param name="roots">Roots of the polynomial</param>
        /// <param name="digits">Accuracy</param>
        /// <param name="epsilon">Small positive number</param>
        public static bool Polynomial(Polynomial poly, out FLOAT[] roots, int digits = 6) {
            FLOAT epsilon = 1E-05f;
            FLOAT num = PolynomialBound(poly);
            if (num == -1f) {
                roots = new FLOAT[0];
                return false;
            }

            return Polynomial(poly, 0f - num, num, out roots, epsilon, digits);
        }

        /// <summary>
        /// General polynomial equation: Σ(c_i * x^i), where i=[0..degree].
        /// </summary>
        /// <param name="poly">Polynomial whose roots to be found</param>
        /// <param name="roots">Roots of the polynomial</param>
        /// <param name="digits">Accuracy</param>
        /// <param name="epsilon">Small positive number</param>
        public static bool Polynomial(Polynomial poly, out FLOAT[] roots, FLOAT epsilon, int digits = 6) {
            FLOAT num = PolynomialBound(poly);
            if (num == -1f) {
                roots = new FLOAT[0];
                return false;
            }

            return Polynomial(poly, 0f - num, num, out roots, epsilon, digits);
        }
    }
}