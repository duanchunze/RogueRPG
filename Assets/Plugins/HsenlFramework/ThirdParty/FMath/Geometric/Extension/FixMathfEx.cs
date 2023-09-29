using System;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public static class FMathfEx {
        /// <summary>
        /// Evaluates quadratic equation a*x^2 + b*x + c
        /// </summary>
        public static FLOAT EvalQuadratic(FLOAT x, FLOAT a, FLOAT b, FLOAT c) {
            return a * x * x + b * x + c;
        }

        /// <summary>
        /// Evaluates sigmoid function (used for smoothing values).
        /// Formula: x^2 * (3 - 2*x)
        /// </summary>
        public static FLOAT EvalSigmoid(FLOAT x) {
            return x * x * (3f - 2f * x);
        }

        /// <summary>
        /// Evaluates overlapped step function. Useful for animating several objects
        /// (stepIndex parameter is number of the objects), where animations follow one after
        /// another with some overlapping in time (overlap parameter).
        /// </summary>
        /// <param name="x">Evaluation parameter, makes sence in [0,1] range</param>
        /// <param name="overlap">Overlapping between animations (must be greater or equal to zero),
        /// where 0 means that animations do not overlap and follow one after another.</param>
        /// <param name="objectIndex">Index of object beeing animated</param>
        /// <param name="objectCount">Number of objects beeing animated</param>
        public static FLOAT EvalOverlappedStep(FLOAT x, FLOAT overlap, int objectIndex, int objectCount) {
            FLOAT num = (x - (1f - overlap) * (FLOAT)objectIndex / ((FLOAT)objectCount - 1f)) / overlap;
            if (num < 0f) {
                num = 0f;
            }
            else if (num > 1f) {
                num = 1f;
            }

            return num;
        }

        /// <summary>
        /// Evaluates overlapped step function and applies sigmoid to smooth the result. Useful for animating several objects
        /// (stepIndex parameter is number of the objects), where animations follow one after
        /// another with some overlapping in time (overlap parameter).
        /// </summary>
        /// <param name="x">Evaluation parameter, makes sence in [0,1] range</param>
        /// <param name="overlap">Overlapping between animations (must be greater or equal to zero),
        /// where 0 means that animations do not overlap and follow one after another.</param>
        /// <param name="objectIndex">Index of object beeing animated</param>
        /// <param name="objectCount">Number of objects beeing animated</param>
        public static FLOAT EvalSmoothOverlappedStep(FLOAT x, FLOAT overlap, int objectIndex, int objectCount) {
            FLOAT num = (x - (1f - overlap) * (FLOAT)objectIndex / ((FLOAT)objectCount - 1f)) / overlap;
            if (num < 0f) {
                num = 0f;
            }
            else if (num > 1f) {
                num = 1f;
            }

            return num * num * (3f - 2f * num);
        }

        /// <summary>
        /// Evaluates scalar gaussian function. The formula is:
        /// a * e^(-(x-b)^2 / 2*c^2)
        /// </summary>
        /// <param name="x">Function parameter</param>
        public static FLOAT EvalGaussian(FLOAT x, FLOAT a, FLOAT b, FLOAT c) {
            FLOAT num = x - b;
            return a * FMath.Exp(num * num / (-2f * c * c));
        }

        /// <summary>
        /// Evaluates 2-dimensional gaussian function. The formula is:
        /// A * e^(-(a*(x - x0)^2 + 2*b*(x - x0)*(y - y0) + c*(y - y0)^2))
        /// </summary>
        /// <param name="x">First function parameter</param>
        /// <param name="y">Second function parameter</param>
        public static FLOAT EvalGaussian2D(FLOAT x, FLOAT y, FLOAT x0, FLOAT y0, FLOAT A, FLOAT a, FLOAT b, FLOAT c) {
            FLOAT num = x - x0;
            FLOAT num2 = y - y0;
            return A * FMath.Exp(0f - (a * num * num + 2f * b * num * num2 + c * num2 * num2));
        }

        /// <summary>
        /// Interpolates between 'value0' and 'value1' using sigmoid as interpolation function.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
        /// <param name="value0"></param>
        public static FLOAT SigmoidInterp(FLOAT value0, FLOAT value1, FLOAT factor) {
            if (factor < 0f) {
                factor = 0f;
            }
            else if (factor > 1f) {
                factor = 1f;
            }

            factor = factor * factor * (3f - 2f * factor);
            return value0 + (value1 - value0) * factor;
        }

        /// <summary>
        /// Interpolates between 'value0' and 'value1' using sine function easing at the end.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
        /// <param name="value0"></param>
        public static FLOAT SinInterp(FLOAT value0, FLOAT value1, FLOAT factor) {
            if (factor < 0f) {
                factor = 0f;
            }
            else if (factor > 1f) {
                factor = 1f;
            }

            factor = FMath.Sin(factor * ((FLOAT)FMath.Pi / 2f));
            return value0 + (value1 - value0) * factor;
        }

        /// <summary>
        /// Interpolates between 'value0' and 'value1' using cosine function easing in the start.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
        /// <param name="value0"></param>
        public static FLOAT CosInterp(FLOAT value0, FLOAT value1, FLOAT factor) {
            if (factor < 0f) {
                factor = 0f;
            }
            else if (factor > 1f) {
                factor = 1f;
            }

            factor = 1f - FMath.Cos(factor * ((FLOAT)FMath.Pi / 2f));
            return value0 + (value1 - value0) * factor;
        }

        /// <summary>
        /// Interpolates between 'value0' and 'value1' in using special function which overshoots first, then waves back and forth gradually declining towards the end.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
        /// <param name="value0"></param>
        public static FLOAT WobbleInterp(FLOAT value0, FLOAT value1, FLOAT factor) {
            if (factor < 0f) {
                factor = 0f;
            }
            else if (factor > 1f) {
                factor = 1f;
            }

            factor = (FMath.Sin(factor * (FLOAT)FMath.Pi * (0.2f + 2.5f * factor * factor * factor)) *
                FMath.Pow(1f - factor, 2.2f) + factor) * (1f + 1.2f * (1f - factor));
            return value0 + (value1 - value0) * factor;
        }

        /// <summary>
        /// Interpolates between 'value0' and 'value1' using provided function (function will be sampled in [0..1] range]).
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
        /// <param name="value0"></param>
        /// <param name="func"></param>
        public static FLOAT FuncInterp(FLOAT value0, FLOAT value1, FLOAT factor, Func<FLOAT, FLOAT> func) {
            if (factor < 0f) {
                factor = 0f;
            }
            else if (factor > 1f) {
                factor = 1f;
            }

            FLOAT num = func(factor);
            return value0 * (1f - num) + value1 * num;
        }

        /// <summary>
        /// Returns 1/Sqrt(value) if value != 0, otherwise returns 0.
        /// </summary>
        public static FLOAT InvSqrt(FLOAT value) {
            if (value != 0f) {
                return 1f / FMath.Sqrt(value);
            }

            return 0f;
        }

        /// <summary>
        /// Converts cartesian coordinates to polar coordinates.
        /// Resulting vector contains rho (length) in x coordinate and phi (angle) in y coordinate; rho &gt;= 0, 0 &lt;= phi &lt; 2pi.
        /// If cartesian coordinates are (0,0) resulting coordinates are (0,0).
        /// </summary>
        public static FVector2 CartesianToPolar(FVector2 cartesianCoordinates) {
            FLOAT x = cartesianCoordinates.x;
            FLOAT y = cartesianCoordinates.y;
            FVector2 result = default(FVector2);
            result.x = FMath.Sqrt(x * x + y * y);
            if (x > 0f) {
                if (y >= 0f) {
                    result.y = FMath.Atan(y / x);
                }
                else {
                    result.y = FMath.Atan(y / x) + (FLOAT)FMath.Pi * 2f;
                }
            }
            else if (x < 0f) {
                result.y = FMath.Atan(y / x) + (FLOAT)FMath.Pi;
            }
            else if (y > 0f) {
                result.y = (FLOAT)FMath.Pi / 2f;
            }
            else if (y < 0f) {
                result.y = 4.712389f;
            }
            else {
                result.x = 0f;
                result.y = 0f;
            }

            return result;
        }

        /// <summary>
        /// Converts polar coordinates to cartesian coordinates.
        /// Input vector contains rho (length) in x coordinate and phi (angle) in y coordinate; rho &gt;= 0, 0 &lt;= phi &lt; 2pi.
        /// </summary>
        public static FVector2 PolarToCartesian(FVector2 polarCoordinates) {
            FVector2 result = default(FVector2);
            result.x = polarCoordinates.x * FMath.Cos(polarCoordinates.y);
            result.y = polarCoordinates.x * FMath.Sin(polarCoordinates.y);
            return result;
        }

        /// <summary>
        /// Converts cartesian coordinates to spherical coordinates.
        /// Resulting vector contains rho (length) in x coordinate, theta (azimutal angle in XZ plane from X axis) in y coordinate,
        /// phi (zenith angle from positive Y axis) in z coordinate; rho &gt;= 0, 0 &lt;= theta &lt; 2pi, 0 &lt;= phi &lt; pi.
        /// If cartesian coordinates are (0,0,0) resulting coordinates are (0,0,0).
        /// </summary>
        /// <param name="cartesianCoordinates"></param>
        /// <returns></returns>
        public static FVector3 CartesianToSpherical(FVector3 cartesianCoordinates) {
            FLOAT x = cartesianCoordinates.x;
            FLOAT y = cartesianCoordinates.y;
            FLOAT z = cartesianCoordinates.z;
            FLOAT num = FMath.Sqrt(x * x + y * y + z * z);
            FLOAT z2;
            FLOAT y2;
            if (num != 0f) {
                z2 = FMath.Acos(y / num);
                y2 = ((x > 0f)
                    ? ((!(z >= 0f)) ? (FMath.Atan(z / x) + (FLOAT)FMath.Pi * 2f) : FMath.Atan(z / x))
                    : ((x < 0f)
                        ? (FMath.Atan(z / x) + (FLOAT)FMath.Pi)
                        : ((z > 0f) ? ((FLOAT)FMath.Pi / 2f) : ((!(z < 0f)) ? 0f : 4.712389f))));
            }
            else {
                num = 0f;
                y2 = 0f;
                z2 = 0f;
            }

            FVector3 result = default(FVector3);
            result.x = num;
            result.y = y2;
            result.z = z2;
            return result;
        }

        /// <summary>
        /// Converts spherical coordinates to cartesian coordinates.
        /// Input vector contains rho (length) in x coordinate, theta (azimutal angle in XZ plane from X axis) in y coordinate,
        /// phi (zenith angle from positive Y axis) in z coordinate; rho &gt;= 0, 0 &lt;= theta &lt; 2pi, 0 &lt;= phi &lt; pi.
        /// </summary>
        /// <param name="sphericalCoordinates"></param>
        /// <returns></returns>
        public static FVector3 SphericalToCartesian(FVector3 sphericalCoordinates) {
            FLOAT x = sphericalCoordinates.x;
            FLOAT y = sphericalCoordinates.y;
            FLOAT z = sphericalCoordinates.z;
            FLOAT num = FMath.Sin(z);
            FVector3 result = default(FVector3);
            result.x = x * FMath.Cos(y) * num;
            result.y = x * FMath.Cos(z);
            result.z = x * FMath.Sin(y) * num;
            return result;
        }

        /// <summary>
        /// Converts cartesian coordinates to cylindrical coordinates.
        /// Resulting vector contains rho (length) in x coordinate, phi (polar angle in XZ plane) in y coordinate,
        /// height (height from XZ plane to the point) in z coordinate.
        /// </summary>
        public static FVector3 CartesianToCylindrical(FVector3 cartesianCoordinates) {
            FLOAT x = cartesianCoordinates.x;
            FLOAT z = cartesianCoordinates.z;
            FLOAT x2 = FMath.Sqrt(x * x + z * z);
            FLOAT y = ((x > 0f)
                ? ((!(z >= 0f)) ? (FMath.Atan(z / x) + (FLOAT)FMath.Pi * 2f) : FMath.Atan(z / x))
                : ((x < 0f)
                    ? (FMath.Atan(z / x) + (FLOAT)FMath.Pi)
                    : ((z > 0f) ? ((FLOAT)FMath.Pi / 2f) : ((!(z < 0f)) ? 0f : 4.712389f))));
            FVector3 result = default(FVector3);
            result.x = x2;
            result.y = y;
            result.z = cartesianCoordinates.y;
            return result;
        }

        /// <summary>
        /// Converts cylindrical coordinates to cartesian coordinates.
        /// Input vector contains rho (length) in x coordinate, phi (polar angle in XZ plane) in y coordinate,
        /// height (height from XZ plane to the point) in z coordinate.
        /// </summary>
        public static FVector3 CylindricalToCartesian(FVector3 cylindricalCoordinates) {
            FVector3 result = default(FVector3);
            result.x = cylindricalCoordinates.x * FMath.Cos(cylindricalCoordinates.y);
            result.y = cylindricalCoordinates.z;
            result.z = cylindricalCoordinates.x * FMath.Sin(cylindricalCoordinates.y);
            return result;
        }
    }
}