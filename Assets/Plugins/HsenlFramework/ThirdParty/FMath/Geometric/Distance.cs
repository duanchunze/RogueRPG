#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public static class Distance {
        /// <summary>
        /// Returns distance between two lines.
        /// </summary>
        public static FLOAT Line2Line2(ref Line2D line0, ref Line2D line1) {
            return FMath.Sqrt(SqrLine2Line2(ref line0, ref line1));
        }

        /// <summary>
        /// Returns distance between two lines.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="closestPoint0">Point on line0 closest to line1</param>
        /// <param name="closestPoint1">Point on line1 closest to line0</param>
        /// <param name="line0"></param>
        public static FLOAT Line2Line2(ref Line2D line0, ref Line2D line1, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            return FMath.Sqrt(SqrLine2Line2(ref line0, ref line1, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between two lines.
        /// </summary>
        public static FLOAT SqrLine2Line2(ref Line2D line0, ref Line2D line1) {
            FVector2 vector = line0.origin - line1.origin;
            FLOAT num = 0f - line0.direction.Dot(line1.direction);
            FLOAT num2 = vector.Dot(line0.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num4;
            if (num3 >= 1E-05f) {
                num4 = 0f;
            }
            else {
                FLOAT num5 = 0f - num2;
                num4 = num2 * num5 + sqrMagnitude;
                if (num4 < 0f) {
                    num4 = 0f;
                }
            }

            return num4;
        }

        /// <summary>
        /// Returns squared distance between two lines.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="closestPoint0">Point on line0 closest to line1</param>
        /// <param name="closestPoint1">Point on line1 closest to line0</param>
        /// <param name="line0"></param>
        public static FLOAT SqrLine2Line2(ref Line2D line0, ref Line2D line1, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            FVector2 vector = line0.origin - line1.origin;
            FLOAT num = 0f - line0.direction.Dot(line1.direction);
            FLOAT num2 = vector.Dot(line0.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num6;
            FLOAT num7;
            FLOAT num8;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(line1.direction);
                FLOAT num5 = 1f / num3;
                num6 = (num * num4 - num2) * num5;
                num7 = (num * num2 - num4) * num5;
                num8 = 0f;
            }
            else {
                num6 = 0f - num2;
                num7 = 0f;
                num8 = num2 * num6 + sqrMagnitude;
                if (num8 < 0f) {
                    num8 = 0f;
                }
            }

            closestPoint0 = line0.origin + num6 * line0.direction;
            closestPoint1 = line1.origin + num7 * line1.direction;
            return num8;
        }

        /// <summary>
        /// Returns distance between a line and a ray
        /// </summary>
        public static FLOAT Line2Ray2(ref Line2D line, ref Ray2D ray) {
            return FMath.Sqrt(SqrLine2Ray2(ref line, ref ray));
        }

        /// <summary>
        /// Returns distance between a line and a ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="closestPoint0">Point on line closest to ray</param>
        /// <param name="closestPoint1">Point on ray closest to line</param>
        /// <param name="line"></param>
        public static FLOAT Line2Ray2(ref Line2D line, ref Ray2D ray, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            return FMath.Sqrt(SqrLine2Ray2(ref line, ref ray, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between a line and a ray
        /// </summary>
        public static FLOAT SqrLine2Ray2(ref Line2D line, ref Ray2D ray) {
            FVector2 vector = line.origin - ray.origin;
            FLOAT num = 0f - line.direction.Dot(ray.direction);
            FLOAT num2 = vector.Dot(line.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num6;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(ray.direction);
                FLOAT num5 = num * num2 - num4;
                if (num5 >= 0f) {
                    num6 = 0f;
                }
                else {
                    FLOAT num7 = 0f - num2;
                    num5 = 0f;
                    num6 = num2 * num7 + sqrMagnitude;
                    if (num6 < 0f) {
                        num6 = 0f;
                    }
                }
            }
            else {
                FLOAT num7 = 0f - num2;
                // FLOAT num5 = 0f;
                num6 = num2 * num7 + sqrMagnitude;
                if (num6 < 0f) {
                    num6 = 0f;
                }
            }

            return num6;
        }

        /// <summary>
        /// Returns squared distance between a line and a ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="closestPoint0">Point on line closest to ray</param>
        /// <param name="closestPoint1">Point on ray closest to line</param>
        /// <param name="line"></param>
        public static FLOAT SqrLine2Ray2(ref Line2D line, ref Ray2D ray, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            FVector2 vector = line.origin - ray.origin;
            FLOAT num = 0f - line.direction.Dot(ray.direction);
            FLOAT num2 = vector.Dot(line.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num7;
            FLOAT num5;
            FLOAT num8;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(ray.direction);
                num5 = num * num2 - num4;
                if (num5 >= 0f) {
                    FLOAT num6 = 1f / num3;
                    num7 = (num * num4 - num2) * num6;
                    num5 *= num6;
                    num8 = 0f;
                }
                else {
                    num7 = 0f - num2;
                    num5 = 0f;
                    num8 = num2 * num7 + sqrMagnitude;
                    if (num8 < 0f) {
                        num8 = 0f;
                    }
                }
            }
            else {
                num7 = 0f - num2;
                num5 = 0f;
                num8 = num2 * num7 + sqrMagnitude;
                if (num8 < 0f) {
                    num8 = 0f;
                }
            }

            closestPoint0 = line.origin + num7 * line.direction;
            closestPoint1 = ray.origin + num5 * ray.direction;
            return num8;
        }

        /// <summary>
        /// Returns distance between a line and a segment
        /// </summary>
        public static FLOAT Line2Segment2(ref Line2D line, ref Segment2D segment) {
            return FMath.Sqrt(SqrLine2Segment2(ref line, ref segment));
        }

        /// <summary>
        /// Returns distance between a line and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint0">Point on line closest to segment</param>
        /// <param name="closestPoint1">Point on segment closest to line</param>
        /// <param name="line"></param>
        public static FLOAT Line2Segment2(ref Line2D line, ref Segment2D segment, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            return FMath.Sqrt(SqrLine2Segment2(ref line, ref segment, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between a line and a segment
        /// </summary>
        public static FLOAT SqrLine2Segment2(ref Line2D line, ref Segment2D segment) {
            FVector2 vector = line.origin - segment.center;
            FLOAT num = 0f - line.direction.Dot(segment.direction);
            FLOAT num2 = vector.Dot(line.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num7;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(segment.direction);
                FLOAT num5 = num * num2 - num4;
                FLOAT num6 = segment.extent * num3;
                if (num5 >= 0f - num6) {
                    if (num5 <= num6) {
                        num7 = 0f;
                    }
                    else {
                        num5 = segment.extent;
                        FLOAT num8 = 0f - (num * num5 + num2);
                        num7 = (0f - num8) * num8 + num5 * (num5 + 2f * num4) + sqrMagnitude;
                    }
                }
                else {
                    num5 = 0f - segment.extent;
                    FLOAT num8 = 0f - (num * num5 + num2);
                    num7 = (0f - num8) * num8 + num5 * (num5 + 2f * num4) + sqrMagnitude;
                }
            }
            else {
                // FLOAT num5 = 0f;
                FLOAT num8 = 0f - num2;
                num7 = num2 * num8 + sqrMagnitude;
            }

            if (num7 < 0f) {
                num7 = 0f;
            }

            return num7;
        }

        /// <summary>
        /// Returns squared distance between a line and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint0">Point on line closest to segment</param>
        /// <param name="closestPoint1">Point on segment closest to line</param>
        /// <param name="line"></param>
        public static FLOAT SqrLine2Segment2(ref Line2D line, ref Segment2D segment, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            FVector2 vector = line.origin - segment.center;
            FLOAT num = 0f - line.direction.Dot(segment.direction);
            FLOAT num2 = vector.Dot(line.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num8;
            FLOAT num5;
            FLOAT num9;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(segment.direction);
                num5 = num * num2 - num4;
                FLOAT num6 = segment.extent * num3;
                if (num5 >= 0f - num6) {
                    if (num5 <= num6) {
                        FLOAT num7 = 1f / num3;
                        num8 = (num * num4 - num2) * num7;
                        num5 *= num7;
                        num9 = 0f;
                    }
                    else {
                        num5 = segment.extent;
                        num8 = 0f - (num * num5 + num2);
                        num9 = (0f - num8) * num8 + num5 * (num5 + 2f * num4) + sqrMagnitude;
                    }
                }
                else {
                    num5 = 0f - segment.extent;
                    num8 = 0f - (num * num5 + num2);
                    num9 = (0f - num8) * num8 + num5 * (num5 + 2f * num4) + sqrMagnitude;
                }
            }
            else {
                num5 = 0f;
                num8 = 0f - num2;
                num9 = num2 * num8 + sqrMagnitude;
            }

            closestPoint0 = line.origin + num8 * line.direction;
            closestPoint1 = segment.center + num5 * segment.direction;
            if (num9 < 0f) {
                num9 = 0f;
            }

            return num9;
        }

        /// <summary>
        /// Returns distance between a point and an abb
        /// </summary>
        public static FLOAT Point2AAB2(ref FVector2 point, ref AABB2D box) {
            FLOAT num = 0f;
            FLOAT x = point.x;
            if (x < box.min.x) {
                FLOAT num2 = box.min.x - x;
                num += num2 * num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
            }

            x = point.y;
            if (x < box.min.y) {
                FLOAT num2 = box.min.y - x;
                num += num2 * num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
            }

            return FMath.Sqrt(num);
        }

        /// <summary>
        /// Returns distance between a point and an abb
        /// </summary>
        /// <param name="box"></param>
        /// <param name="closestPoint">Point projected on an aab</param>
        /// <param name="point"></param>
        public static FLOAT Point2AAB2(ref FVector2 point, ref AABB2D box, out FVector2 closestPoint) {
            FLOAT num = 0f;
            closestPoint = point;
            FLOAT x = point.x;
            if (x < box.min.x) {
                FLOAT num2 = box.min.x - x;
                num += num2 * num2;
                closestPoint.x += num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
                closestPoint.x -= num2;
            }

            x = point.y;
            if (x < box.min.y) {
                FLOAT num2 = box.min.y - x;
                num += num2 * num2;
                closestPoint.y += num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
                closestPoint.y -= num2;
            }

            return FMath.Sqrt(num);
        }

        /// <summary>
        /// Returns squared distance between a point and an abb
        /// </summary>
        public static FLOAT SqrPoint2AAB2(ref FVector2 point, ref AABB2D box) {
            FLOAT num = 0f;
            FLOAT x = point.x;
            if (x < box.min.x) {
                FLOAT num2 = box.min.x - x;
                num += num2 * num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
            }

            x = point.y;
            if (x < box.min.y) {
                FLOAT num2 = box.min.y - x;
                num += num2 * num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
            }

            return num;
        }

        /// <summary>
        /// Returns squared distance between a point and an abb
        /// </summary>
        /// <param name="box"></param>
        /// <param name="closestPoint">Point projected on an aab</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint2AAB2(ref FVector2 point, ref AABB2D box, out FVector2 closestPoint) {
            FLOAT num = 0f;
            closestPoint = point;
            FLOAT x = point.x;
            if (x < box.min.x) {
                FLOAT num2 = box.min.x - x;
                num += num2 * num2;
                closestPoint.x += num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
                closestPoint.x -= num2;
            }

            x = point.y;
            if (x < box.min.y) {
                FLOAT num2 = box.min.y - x;
                num += num2 * num2;
                closestPoint.y += num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
                closestPoint.y -= num2;
            }

            return num;
        }

        /// <summary>
        /// Returns distance between a point and a box
        /// </summary>
        public static FLOAT Point2Box2(ref FVector2 point, ref Box2D box) {
            return FMath.Sqrt(SqrPoint2Box2(ref point, ref box));
        }

        /// <summary>
        /// Returns distance between a point and a box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="closestPoint">Point projected on a box</param>
        /// <param name="point"></param>
        public static FLOAT Point2Box2(ref FVector2 point, ref Box2D box, out FVector2 closestPoint) {
            return FMath.Sqrt(SqrPoint2Box2(ref point, ref box, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a box
        /// </summary>
        public static FLOAT SqrPoint2Box2(ref FVector2 point, ref Box2D box) {
            FVector2 vector = point - box.center;
            FLOAT num = 0f;
            FLOAT num2 = vector.Dot(box.axis0);
            FLOAT x = box.extents.x;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
            }

            FLOAT num4 = vector.Dot(box.axis1);
            x = box.extents.y;
            if (num4 < 0f - x) {
                FLOAT num3 = num4 + x;
                num += num3 * num3;
            }
            else if (num4 > x) {
                FLOAT num3 = num4 - x;
                num += num3 * num3;
            }

            return num;
        }

        /// <summary>
        /// Returns squared distance between a point and a box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="closestPoint">Point projected on a box</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint2Box2(ref FVector2 point, ref Box2D box, out FVector2 closestPoint) {
            FVector2 vector = point - box.center;
            FLOAT num = 0f;
            FLOAT num2 = vector.Dot(box.axis0);
            FLOAT x = box.extents.x;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
                num2 = 0f - x;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
                num2 = x;
            }

            FLOAT num4 = vector.Dot(box.axis1);
            x = box.extents.y;
            if (num4 < 0f - x) {
                FLOAT num3 = num4 + x;
                num += num3 * num3;
                num4 = 0f - x;
            }
            else if (num4 > x) {
                FLOAT num3 = num4 - x;
                num += num3 * num3;
                num4 = x;
            }

            closestPoint = box.center + num2 * box.axis0 + num4 * box.axis1;
            return num;
        }

        /// <summary>
        /// Returns distance between a point and a circle
        /// </summary>
        public static FLOAT Point2Circle2(ref FVector2 point, ref Circle2D circle) {
            FLOAT num = (point - circle.center).magnitude - circle.radius;
            if (!(num > 0f)) {
                return 0f;
            }

            return num;
        }

        /// <summary>
        /// Returns distance between a point and a circle
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="closestPoint">Point projected on a circle</param>
        /// <param name="point"></param>
        public static FLOAT Point2Circle2(ref FVector2 point, ref Circle2D circle, out FVector2 closestPoint) {
            FVector2 vector = point - circle.center;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > circle.radius * circle.radius) {
                FLOAT num = FMath.Sqrt(sqrMagnitude);
                closestPoint = circle.center + vector * (circle.radius / num);
                return num - circle.radius;
            }

            closestPoint = point;
            return 0f;
        }

        /// <summary>
        /// Returns squared distance between a point and a circle
        /// </summary>
        public static FLOAT SqrPoint2Circle2(ref FVector2 point, ref Circle2D circle) {
            FLOAT num = (point - circle.center).magnitude - circle.radius;
            if (!(num > 0f)) {
                return 0f;
            }

            return num * num;
        }

        /// <summary>
        /// Returns squared distance between a point and a circle
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="closestPoint">Point projected on a circle</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint2Circle2(ref FVector2 point, ref Circle2D circle, out FVector2 closestPoint) {
            FVector2 vector = point - circle.center;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > circle.radius * circle.radius) {
                FLOAT num = FMath.Sqrt(sqrMagnitude);
                closestPoint = circle.center + vector * (circle.radius / num);
                FLOAT num2 = num - circle.radius;
                return num2 * num2;
            }

            closestPoint = point;
            return 0f;
        }

        /// <summary>
        /// Returns distance between a point and a line
        /// </summary>
        public static FLOAT Point2Line2(ref FVector2 point, ref Line2D line) {
            return FMath.Sqrt(SqrPoint2Line2(ref point, ref line));
        }

        /// <summary>
        /// Returns distance between a point and a line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="closestPoint">Point projected on a line</param>
        /// <param name="point"></param>
        public static FLOAT Point2Line2(ref FVector2 point, ref Line2D line, out FVector2 closestPoint) {
            return FMath.Sqrt(SqrPoint2Line2(ref point, ref line, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a line
        /// </summary>
        public static FLOAT SqrPoint2Line2(ref FVector2 point, ref Line2D line) {
            FVector2 value = point - line.origin;
            FLOAT num = line.direction.Dot(value);
            FVector2 vector = line.origin + num * line.direction;
            return (vector - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns squared distance between a point and a line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="closestPoint">Point projected on a line</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint2Line2(ref FVector2 point, ref Line2D line, out FVector2 closestPoint) {
            FVector2 value = point - line.origin;
            FLOAT num = line.direction.Dot(value);
            closestPoint = line.origin + num * line.direction;
            return (closestPoint - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns distance between a point and a ray
        /// </summary>
        public static FLOAT Point2Ray2(ref FVector2 point, ref Ray2D ray) {
            return FMath.Sqrt(SqrPoint2Ray2(ref point, ref ray));
        }

        /// <summary>
        /// Returns distance between a point and a ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="closestPoint">Point projected on a ray and clamped by ray origin</param>
        /// <param name="point"></param>
        public static FLOAT Point2Ray2(ref FVector2 point, ref Ray2D ray, out FVector2 closestPoint) {
            return FMath.Sqrt(SqrPoint2Ray2(ref point, ref ray, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a ray
        /// </summary>
        public static FLOAT SqrPoint2Ray2(ref FVector2 point, ref Ray2D ray) {
            FVector2 value = point - ray.origin;
            FLOAT num = ray.direction.Dot(value);
            FVector2 vector = ((!(num > 0f)) ? ray.origin : (ray.origin + num * ray.direction));
            return (vector - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns squared distance between a point and a ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="closestPoint">Point projected on a ray and clamped by ray origin</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint2Ray2(ref FVector2 point, ref Ray2D ray, out FVector2 closestPoint) {
            FVector2 value = point - ray.origin;
            FLOAT num = ray.direction.Dot(value);
            if (num > 0f) {
                closestPoint = ray.origin + num * ray.direction;
            }
            else {
                closestPoint = ray.origin;
            }

            return (closestPoint - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns distance between a point and a segment
        /// </summary>
        public static FLOAT Point2Segment2(ref FVector2 point, ref Segment2D segment) {
            return FMath.Sqrt(SqrPoint2Segment2(ref point, ref segment));
        }

        /// <summary>
        /// Returns distance between a point and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint">Point projected on a segment and clamped by segment endpoints</param>
        /// <param name="point"></param>
        public static FLOAT Point2Segment2(ref FVector2 point, ref Segment2D segment, out FVector2 closestPoint) {
            return FMath.Sqrt(SqrPoint2Segment2(ref point, ref segment, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a segment
        /// </summary>
        public static FLOAT SqrPoint2Segment2(ref FVector2 point, ref Segment2D segment) {
            FVector2 value = point - segment.center;
            FLOAT num = segment.direction.Dot(value);
            FVector2 vector = ((!(0f - segment.extent < num))
                ? segment.p0
                : ((!(num < segment.extent)) ? segment.p1 : (segment.center + num * segment.direction)));
            return (vector - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns squared distance between a point and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint">Point projected on a segment and clamped by segment endpoints</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint2Segment2(ref FVector2 point, ref Segment2D segment, out FVector2 closestPoint) {
            FVector2 value = point - segment.center;
            FLOAT num = segment.direction.Dot(value);
            if (0f - segment.extent < num) {
                if (num < segment.extent) {
                    closestPoint = segment.center + num * segment.direction;
                }
                else {
                    closestPoint = segment.p1;
                }
            }
            else {
                closestPoint = segment.p0;
            }

            return (closestPoint - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns distance between a point and a triangle
        /// </summary>
        public static FLOAT Point2Triangle2(ref FVector2 point, ref Triangle2D triangle) {
            if (triangle.Contains(point)) {
                return 0f;
            }

            Segment2D segment = new Segment2D(ref triangle.v0, ref triangle.v1);
            FLOAT num = Point2Segment2(ref point, ref segment);
            Segment2D segment2D = new Segment2D(ref triangle.v1, ref triangle.v2);
            FLOAT num2 = Point2Segment2(ref point, ref segment2D);
            Segment2D segment3 = new Segment2D(ref triangle.v2, ref triangle.v0);
            FLOAT num3 = Point2Segment2(ref point, ref segment3);
            if (num < num2) {
                if (num < num3) {
                    return num;
                }

                if (num2 < num3) {
                    return num2;
                }

                return num3;
            }

            if (num2 < num3) {
                return num2;
            }

            if (num < num3) {
                return num;
            }

            return num3;
        }

        /// <summary>
        /// Returns distance between a point and a triangle
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="closestPoint">Point projected on a triangle</param>
        /// <param name="point"></param>
        public static FLOAT Point2Triangle2(ref FVector2 point, ref Triangle2D triangle, out FVector2 closestPoint) {
            if (triangle.Contains(point)) {
                closestPoint = point;
                return 0f;
            }

            Segment2D segment = new Segment2D(ref triangle.v0, ref triangle.v1);
            FVector2 closestPoint2;
            FLOAT num = Point2Segment2(ref point, ref segment, out closestPoint2);
            Segment2D segment2D = new Segment2D(ref triangle.v1, ref triangle.v2);
            FVector2 closestPoint3;
            FLOAT num2 = Point2Segment2(ref point, ref segment2D, out closestPoint3);
            Segment2D segment3 = new Segment2D(ref triangle.v2, ref triangle.v0);
            FVector2 closestPoint4;
            FLOAT num3 = Point2Segment2(ref point, ref segment3, out closestPoint4);
            if (num < num2) {
                if (num < num3) {
                    closestPoint = closestPoint2;
                    return num;
                }

                if (num2 < num3) {
                    closestPoint = closestPoint3;
                    return num2;
                }

                closestPoint = closestPoint4;
                return num3;
            }

            if (num2 < num3) {
                closestPoint = closestPoint3;
                return num2;
            }

            if (num < num3) {
                closestPoint = closestPoint2;
                return num;
            }

            closestPoint = closestPoint4;
            return num3;
        }

        /// <summary>
        /// Returns squared distance between a point and a triangle
        /// </summary>
        public static FLOAT SqrPoint2Triangle2(ref FVector2 point, ref Triangle2D triangle) {
            if (triangle.Contains(point)) {
                return 0f;
            }

            Segment2D segment = new Segment2D(ref triangle.v0, ref triangle.v1);
            FLOAT num = SqrPoint2Segment2(ref point, ref segment);
            Segment2D segment2D = new Segment2D(ref triangle.v1, ref triangle.v2);
            FLOAT num2 = SqrPoint2Segment2(ref point, ref segment2D);
            Segment2D segment3 = new Segment2D(ref triangle.v2, ref triangle.v0);
            FLOAT num3 = SqrPoint2Segment2(ref point, ref segment3);
            if (num < num2) {
                if (num < num3) {
                    return num;
                }

                if (num2 < num3) {
                    return num2;
                }

                return num3;
            }

            if (num2 < num3) {
                return num2;
            }

            if (num < num3) {
                return num;
            }

            return num3;
        }

        /// <summary>
        /// Returns squared distance between a point and a triangle
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="closestPoint">Point projected on a triangle</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint2Triangle2(ref FVector2 point, ref Triangle2D triangle,
            out FVector2 closestPoint) {
            if (triangle.Contains(point)) {
                closestPoint = point;
                return 0f;
            }

            Segment2D segment = new Segment2D(ref triangle.v0, ref triangle.v1);
            FVector2 closestPoint2;
            FLOAT num = SqrPoint2Segment2(ref point, ref segment, out closestPoint2);
            Segment2D segment2D = new Segment2D(ref triangle.v1, ref triangle.v2);
            FVector2 closestPoint3;
            FLOAT num2 = SqrPoint2Segment2(ref point, ref segment2D, out closestPoint3);
            Segment2D segment3 = new Segment2D(ref triangle.v2, ref triangle.v0);
            FVector2 closestPoint4;
            FLOAT num3 = SqrPoint2Segment2(ref point, ref segment3, out closestPoint4);
            if (num < num2) {
                if (num < num3) {
                    closestPoint = closestPoint2;
                    return num;
                }

                if (num2 < num3) {
                    closestPoint = closestPoint3;
                    return num2;
                }

                closestPoint = closestPoint4;
                return num3;
            }

            if (num2 < num3) {
                closestPoint = closestPoint3;
                return num2;
            }

            if (num < num3) {
                closestPoint = closestPoint2;
                return num;
            }

            closestPoint = closestPoint4;
            return num3;
        }

        /// <summary>
        /// Returns distance between two rays
        /// </summary>
        public static FLOAT Ray2Ray2(ref Ray2D ray0, ref Ray2D ray1) {
            return FMath.Sqrt(SqrRay2Ray2(ref ray0, ref ray1, out _, out _));
        }

        /// <summary>
        /// Returns distance between two rays
        /// </summary>
        /// <param name="closestPoint0">Point on ray0 closest to ray1</param>
        /// <param name="closestPoint1">Point on ray1 closest to ray0</param>
        public static FLOAT Ray2Ray2(ref Ray2D ray0, ref Ray2D ray1, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            return FMath.Sqrt(SqrRay2Ray2(ref ray0, ref ray1, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between two rays
        /// </summary>
        public static FLOAT SqrRay2Ray2(ref Ray2D ray0, ref Ray2D ray1) {
            return SqrRay2Ray2(ref ray0, ref ray1, out _, out _);
        }

        /// <summary>
        /// Returns squared distance between two rays
        /// </summary>
        /// <param name="ray1"></param>
        /// <param name="closestPoint0">Point on ray0 closest to ray1</param>
        /// <param name="closestPoint1">Point on ray1 closest to ray0</param>
        /// <param name="ray0"></param>
        public static FLOAT SqrRay2Ray2(ref Ray2D ray0, ref Ray2D ray1, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            FVector2 vector = ray0.origin - ray1.origin;
            FLOAT num = 0f - ray0.direction.Dot(ray1.direction);
            FLOAT num2 = vector.Dot(ray0.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num5;
            FLOAT num6;
            FLOAT num8;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(ray1.direction);
                num5 = num * num4 - num2;
                num6 = num * num2 - num4;
                if (num5 >= 0f) {
                    if (num6 >= 0f) {
                        FLOAT num7 = 1f / num3;
                        num5 *= num7;
                        num6 *= num7;
                        num8 = 0f;
                    }
                    else {
                        num6 = 0f;
                        if (num2 >= 0f) {
                            num5 = 0f;
                            num8 = sqrMagnitude;
                        }
                        else {
                            num5 = 0f - num2;
                            num8 = num2 * num5 + sqrMagnitude;
                        }
                    }
                }
                else if (num6 >= 0f) {
                    num5 = 0f;
                    if (num4 >= 0f) {
                        num6 = 0f;
                        num8 = sqrMagnitude;
                    }
                    else {
                        num6 = 0f - num4;
                        num8 = num4 * num6 + sqrMagnitude;
                    }
                }
                else if (num2 < 0f) {
                    num5 = 0f - num2;
                    num6 = 0f;
                    num8 = num2 * num5 + sqrMagnitude;
                }
                else {
                    num5 = 0f;
                    if (num4 >= 0f) {
                        num6 = 0f;
                        num8 = sqrMagnitude;
                    }
                    else {
                        num6 = 0f - num4;
                        num8 = num4 * num6 + sqrMagnitude;
                    }
                }
            }
            else if (num > 0f) {
                num6 = 0f;
                if (num2 >= 0f) {
                    num5 = 0f;
                    num8 = sqrMagnitude;
                }
                else {
                    num5 = 0f - num2;
                    num8 = num2 * num5 + sqrMagnitude;
                }
            }
            else if (num2 >= 0f) {
                FLOAT num4 = 0f - vector.Dot(ray1.direction);
                num5 = 0f;
                num6 = 0f - num4;
                num8 = num4 * num6 + sqrMagnitude;
            }
            else {
                num5 = 0f - num2;
                num6 = 0f;
                num8 = num2 * num5 + sqrMagnitude;
            }

            closestPoint0 = ray0.origin + num5 * ray0.direction;
            closestPoint1 = ray1.origin + num6 * ray1.direction;
            if (num8 < 0f) {
                num8 = 0f;
            }

            return num8;
        }

        /// <summary>
        /// Returns distance between a ray and a segment
        /// </summary>
        public static FLOAT Ray2Segment2(ref Ray2D ray, ref Segment2D segment) {
            return FMath.Sqrt(SqrRay2Segment2(ref ray, ref segment, out _, out _));
        }

        /// <summary>
        /// Returns distance between a ray and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint0">Point on ray closest to segment</param>
        /// <param name="closestPoint1">Point on segment closest to ray</param>
        /// <param name="ray"></param>
        public static FLOAT Ray2Segment2(ref Ray2D ray, ref Segment2D segment, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            return FMath.Sqrt(SqrRay2Segment2(ref ray, ref segment, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between a ray and a segment
        /// </summary>
        public static FLOAT SqrRay2Segment2(ref Ray2D ray, ref Segment2D segment) {
            return SqrRay2Segment2(ref ray, ref segment, out _, out _);
        }

        /// <summary>
        /// Returns squared distance between a ray and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint0">Point on ray closest to segment</param>
        /// <param name="closestPoint1">Point on segment closest to ray</param>
        /// <param name="ray"></param>
        public static FLOAT SqrRay2Segment2(ref Ray2D ray, ref Segment2D segment, out FVector2 closestPoint0,
            out FVector2 closestPoint1) {
            FVector2 vector = ray.origin - segment.center;
            FLOAT num = 0f - ray.direction.Dot(segment.direction);
            FLOAT num2 = vector.Dot(ray.direction);
            FLOAT num3 = 0f - vector.Dot(segment.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num4 = FMath.Abs(1f - num * num);
            FLOAT num5;
            FLOAT num6;
            FLOAT num9;
            if (num4 >= 1E-05f) {
                num5 = num * num3 - num2;
                num6 = num * num2 - num3;
                FLOAT num7 = segment.extent * num4;
                if (num5 >= 0f) {
                    if (num6 >= 0f - num7) {
                        if (num6 <= num7) {
                            FLOAT num8 = 1f / num4;
                            num5 *= num8;
                            num6 *= num8;
                            num9 = 0f;
                        }
                        else {
                            num6 = segment.extent;
                            num5 = 0f - (num * num6 + num2);
                            if (num5 > 0f) {
                                num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else {
                                num5 = 0f;
                                num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                        }
                    }
                    else {
                        num6 = 0f - segment.extent;
                        num5 = 0f - (num * num6 + num2);
                        if (num5 > 0f) {
                            num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else {
                            num5 = 0f;
                            num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                    }
                }
                else if (num6 <= 0f - num7) {
                    num5 = 0f - ((0f - num) * segment.extent + num2);
                    if (num5 > 0f) {
                        num6 = 0f - segment.extent;
                        num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                    else {
                        num5 = 0f;
                        num6 = 0f - num3;
                        if (num6 < 0f - segment.extent) {
                            num6 = 0f - segment.extent;
                        }
                        else if (num6 > segment.extent) {
                            num6 = segment.extent;
                        }

                        num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                }
                else if (num6 <= num7) {
                    num5 = 0f;
                    num6 = 0f - num3;
                    if (num6 < 0f - segment.extent) {
                        num6 = 0f - segment.extent;
                    }
                    else if (num6 > segment.extent) {
                        num6 = segment.extent;
                    }

                    num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                }
                else {
                    num5 = 0f - (num * segment.extent + num2);
                    if (num5 > 0f) {
                        num6 = segment.extent;
                        num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                    else {
                        num5 = 0f;
                        num6 = 0f - num3;
                        if (num6 < 0f - segment.extent) {
                            num6 = 0f - segment.extent;
                        }
                        else if (num6 > segment.extent) {
                            num6 = segment.extent;
                        }

                        num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                }
            }
            else {
                num6 = ((!(num > 0f)) ? segment.extent : (0f - segment.extent));
                num5 = 0f - (num * num6 + num2);
                if (num5 > 0f) {
                    num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                }
                else {
                    num5 = 0f;
                    num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                }
            }

            closestPoint0 = ray.origin + num5 * ray.direction;
            closestPoint1 = segment.center + num6 * segment.direction;
            if (num9 < 0f) {
                num9 = 0f;
            }

            return num9;
        }

        /// <summary>
        /// Returns distance between two segments
        /// </summary>
        public static FLOAT Segment2Segment2(ref Segment2D segment0, ref Segment2D segment1) {
            return FMath.Sqrt(SqrSegment2Segment2(ref segment0, ref segment1, out _, out _));
        }

        /// <summary>
        /// Returns distance between two segments
        /// </summary>
        /// <param name="closestPoint0">Point on segment0 closest to segment1</param>
        /// <param name="closestPoint1">Point on segment1 closest to segment0</param>
        public static FLOAT Segment2Segment2(ref Segment2D segment0, ref Segment2D segment1,
            out FVector2 closestPoint0, out FVector2 closestPoint1) {
            return FMath.Sqrt(SqrSegment2Segment2(ref segment0, ref segment1, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between two segments
        /// </summary>
        public static FLOAT SqrSegment2Segment2(ref Segment2D segment0, ref Segment2D segment1) {
            return SqrSegment2Segment2(ref segment0, ref segment1, out _, out _);
        }

        /// <summary>
        /// Returns squared distance between two segments
        /// </summary>
        /// <param name="segment1"></param>
        /// <param name="closestPoint0">Point on segment0 closest to segment1</param>
        /// <param name="closestPoint1">Point on segment1 closest to segment0</param>
        /// <param name="segment0"></param>
        public static FLOAT SqrSegment2Segment2(ref Segment2D segment0, ref Segment2D segment1,
            out FVector2 closestPoint0, out FVector2 closestPoint1) {
            FVector2 vector = segment0.center - segment1.center;
            FLOAT num = 0f - segment0.direction.Dot(segment1.direction);
            FLOAT num2 = vector.Dot(segment0.direction);
            FLOAT num3 = 0f - vector.Dot(segment1.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num4 = FMath.Abs(1f - num * num);
            FLOAT num5;
            FLOAT num6;
            FLOAT num10;
            if (num4 >= 1E-05f) {
                num5 = num * num3 - num2;
                num6 = num * num2 - num3;
                FLOAT num7 = segment0.extent * num4;
                FLOAT num8 = segment1.extent * num4;
                if (num5 >= 0f - num7) {
                    if (num5 <= num7) {
                        if (num6 >= 0f - num8) {
                            if (num6 <= num8) {
                                FLOAT num9 = 1f / num4;
                                num5 *= num9;
                                num6 *= num9;
                                num10 = 0f;
                            }
                            else {
                                num6 = segment1.extent;
                                FLOAT num11 = 0f - (num * num6 + num2);
                                if (num11 < 0f - segment0.extent) {
                                    num5 = 0f - segment0.extent;
                                    num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                                }
                                else if (num11 <= segment0.extent) {
                                    num5 = num11;
                                    num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                                }
                                else {
                                    num5 = segment0.extent;
                                    num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                                }
                            }
                        }
                        else {
                            num6 = 0f - segment1.extent;
                            FLOAT num11 = 0f - (num * num6 + num2);
                            if (num11 < 0f - segment0.extent) {
                                num5 = 0f - segment0.extent;
                                num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else if (num11 <= segment0.extent) {
                                num5 = num11;
                                num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else {
                                num5 = segment0.extent;
                                num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                        }
                    }
                    else if (num6 >= 0f - num8) {
                        if (num6 <= num8) {
                            num5 = segment0.extent;
                            FLOAT num12 = 0f - (num * num5 + num3);
                            if (num12 < 0f - segment1.extent) {
                                num6 = 0f - segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else if (num12 <= segment1.extent) {
                                num6 = num12;
                                num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else {
                                num6 = segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                        }
                        else {
                            num6 = segment1.extent;
                            FLOAT num11 = 0f - (num * num6 + num2);
                            if (num11 < 0f - segment0.extent) {
                                num5 = 0f - segment0.extent;
                                num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else if (num11 <= segment0.extent) {
                                num5 = num11;
                                num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else {
                                num5 = segment0.extent;
                                FLOAT num12 = 0f - (num * num5 + num3);
                                if (num12 < 0f - segment1.extent) {
                                    num6 = 0f - segment1.extent;
                                    num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                                }
                                else if (num12 <= segment1.extent) {
                                    num6 = num12;
                                    num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                                }
                                else {
                                    num6 = segment1.extent;
                                    num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                                }
                            }
                        }
                    }
                    else {
                        num6 = 0f - segment1.extent;
                        FLOAT num11 = 0f - (num * num6 + num2);
                        if (num11 < 0f - segment0.extent) {
                            num5 = 0f - segment0.extent;
                            num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else if (num11 <= segment0.extent) {
                            num5 = num11;
                            num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else {
                            num5 = segment0.extent;
                            FLOAT num12 = 0f - (num * num5 + num3);
                            if (num12 > segment1.extent) {
                                num6 = segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else if (num12 >= 0f - segment1.extent) {
                                num6 = num12;
                                num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else {
                                num6 = 0f - segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                        }
                    }
                }
                else if (num6 >= 0f - num8) {
                    if (num6 <= num8) {
                        num5 = 0f - segment0.extent;
                        FLOAT num12 = 0f - (num * num5 + num3);
                        if (num12 < 0f - segment1.extent) {
                            num6 = 0f - segment1.extent;
                            num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                        else if (num12 <= segment1.extent) {
                            num6 = num12;
                            num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                        else {
                            num6 = segment1.extent;
                            num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                    }
                    else {
                        num6 = segment1.extent;
                        FLOAT num11 = 0f - (num * num6 + num2);
                        if (num11 > segment0.extent) {
                            num5 = segment0.extent;
                            num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else if (num11 >= 0f - segment0.extent) {
                            num5 = num11;
                            num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else {
                            num5 = 0f - segment0.extent;
                            FLOAT num12 = 0f - (num * num5 + num3);
                            if (num12 < 0f - segment1.extent) {
                                num6 = 0f - segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else if (num12 <= segment1.extent) {
                                num6 = num12;
                                num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else {
                                num6 = segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                        }
                    }
                }
                else {
                    num6 = 0f - segment1.extent;
                    FLOAT num11 = 0f - (num * num6 + num2);
                    if (num11 > segment0.extent) {
                        num5 = segment0.extent;
                        num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                    else if (num11 >= 0f - segment0.extent) {
                        num5 = num11;
                        num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                    else {
                        num5 = 0f - segment0.extent;
                        FLOAT num12 = 0f - (num * num5 + num3);
                        if (num12 < 0f - segment1.extent) {
                            num6 = 0f - segment1.extent;
                            num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                        else if (num12 <= segment1.extent) {
                            num6 = num12;
                            num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                        else {
                            num6 = segment1.extent;
                            num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                    }
                }
            }
            else {
                FLOAT num13 = segment0.extent + segment1.extent;
                FLOAT num14 = ((num > 0f) ? (-1f) : 1f);
                FLOAT num15 = 0.5f * (num2 - num14 * num3);
                FLOAT num16 = 0f - num15;
                if (num16 < 0f - num13) {
                    num16 = 0f - num13;
                }
                else if (num16 > num13) {
                    num16 = num13;
                }

                num6 = (0f - num14) * num16 * segment1.extent / num13;
                num5 = num16 + num14 * num6;
                num10 = num16 * (num16 + 2f * num15) + sqrMagnitude;
            }

            closestPoint0 = segment0.center + num5 * segment0.direction;
            closestPoint1 = segment1.center + num6 * segment1.direction;
            if (num10 < 0f) {
                num10 = 0f;
            }

            return num10;
        }

        private static void Face(ref Box mBox, ref FLOAT mLineParameter, int i0, int i1, int i2, ref FVector3 pnt,
            ref FVector3 dir, ref FVector3 PmE, ref FLOAT sqrDistance) {
            FVector3 vector = default(FVector3);
            vector[i1] = pnt[i1] + mBox.extents[i1];
            vector[i2] = pnt[i2] + mBox.extents[i2];
            FLOAT num3;
            FLOAT num2;
            if (dir[i0] * vector[i1] >= dir[i1] * PmE[i0]) {
                if (dir[i0] * vector[i2] >= dir[i2] * PmE[i0]) {
                    pnt[i0] = mBox.extents[i0];
                    FLOAT num = 1f / dir[i0];
                    pnt[i1] -= dir[i1] * PmE[i0] * num;
                    pnt[i2] -= dir[i2] * PmE[i0] * num;
                    mLineParameter = (0f - PmE[i0]) * num;
                    return;
                }

                num2 = dir[i0] * dir[i0] + dir[i2] * dir[i2];
                num3 = num2 * vector[i1] - dir[i1] * (dir[i0] * PmE[i0] + dir[i2] * vector[i2]);
                if (num3 <= 2f * num2 * mBox.extents[i1]) {
                    FLOAT num4 = num3 / num2;
                    num2 += dir[i1] * dir[i1];
                    num3 = vector[i1] - num4;
                    FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * num3 + dir[i2] * vector[i2];
                    FLOAT num6 = (0f - num5) / num2;
                    sqrDistance += PmE[i0] * PmE[i0] + num3 * num3 + vector[i2] * vector[i2] + num5 * num6;
                    mLineParameter = num6;
                    pnt[i0] = mBox.extents[i0];
                    pnt[i1] = num4 - mBox.extents[i1];
                    pnt[i2] = 0f - mBox.extents[i2];
                }
                else {
                    num2 += dir[i1] * dir[i1];
                    FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * PmE[i1] + dir[i2] * vector[i2];
                    FLOAT num6 = (0f - num5) / num2;
                    sqrDistance += PmE[i0] * PmE[i0] + PmE[i1] * PmE[i1] + vector[i2] * vector[i2] + num5 * num6;
                    mLineParameter = num6;
                    pnt[i0] = mBox.extents[i0];
                    pnt[i1] = mBox.extents[i1];
                    pnt[i2] = 0f - mBox.extents[i2];
                }

                return;
            }

            if (dir[i0] * vector[i2] >= dir[i2] * PmE[i0]) {
                num2 = dir[i0] * dir[i0] + dir[i1] * dir[i1];
                num3 = num2 * vector[i2] - dir[i2] * (dir[i0] * PmE[i0] + dir[i1] * vector[i1]);
                if (num3 <= 2f * num2 * mBox.extents[i2]) {
                    FLOAT num4 = num3 / num2;
                    num2 += dir[i2] * dir[i2];
                    num3 = vector[i2] - num4;
                    FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * vector[i1] + dir[i2] * num3;
                    FLOAT num6 = (0f - num5) / num2;
                    sqrDistance += PmE[i0] * PmE[i0] + vector[i1] * vector[i1] + num3 * num3 + num5 * num6;
                    mLineParameter = num6;
                    pnt[i0] = mBox.extents[i0];
                    pnt[i1] = 0f - mBox.extents[i1];
                    pnt[i2] = num4 - mBox.extents[i2];
                }
                else {
                    num2 += dir[i2] * dir[i2];
                    FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * vector[i1] + dir[i2] * PmE[i2];
                    FLOAT num6 = (0f - num5) / num2;
                    sqrDistance += PmE[i0] * PmE[i0] + vector[i1] * vector[i1] + PmE[i2] * PmE[i2] + num5 * num6;
                    mLineParameter = num6;
                    pnt[i0] = mBox.extents[i0];
                    pnt[i1] = 0f - mBox.extents[i1];
                    pnt[i2] = mBox.extents[i2];
                }

                return;
            }

            num2 = dir[i0] * dir[i0] + dir[i2] * dir[i2];
            num3 = num2 * vector[i1] - dir[i1] * (dir[i0] * PmE[i0] + dir[i2] * vector[i2]);
            if (num3 >= 0f) {
                if (num3 <= 2f * num2 * mBox.extents[i1]) {
                    FLOAT num4 = num3 / num2;
                    num2 += dir[i1] * dir[i1];
                    num3 = vector[i1] - num4;
                    FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * num3 + dir[i2] * vector[i2];
                    FLOAT num6 = (0f - num5) / num2;
                    sqrDistance += PmE[i0] * PmE[i0] + num3 * num3 + vector[i2] * vector[i2] + num5 * num6;
                    mLineParameter = num6;
                    pnt[i0] = mBox.extents[i0];
                    pnt[i1] = num4 - mBox.extents[i1];
                    pnt[i2] = 0f - mBox.extents[i2];
                }
                else {
                    num2 += dir[i1] * dir[i1];
                    FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * PmE[i1] + dir[i2] * vector[i2];
                    FLOAT num6 = (0f - num5) / num2;
                    sqrDistance += PmE[i0] * PmE[i0] + PmE[i1] * PmE[i1] + vector[i2] * vector[i2] + num5 * num6;
                    mLineParameter = num6;
                    pnt[i0] = mBox.extents[i0];
                    pnt[i1] = mBox.extents[i1];
                    pnt[i2] = 0f - mBox.extents[i2];
                }

                return;
            }

            num2 = dir[i0] * dir[i0] + dir[i1] * dir[i1];
            num3 = num2 * vector[i2] - dir[i2] * (dir[i0] * PmE[i0] + dir[i1] * vector[i1]);
            if (num3 >= 0f) {
                if (num3 <= 2f * num2 * mBox.extents[i2]) {
                    FLOAT num4 = num3 / num2;
                    num2 += dir[i2] * dir[i2];
                    num3 = vector[i2] - num4;
                    FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * vector[i1] + dir[i2] * num3;
                    FLOAT num6 = (0f - num5) / num2;
                    sqrDistance += PmE[i0] * PmE[i0] + vector[i1] * vector[i1] + num3 * num3 + num5 * num6;
                    mLineParameter = num6;
                    pnt[i0] = mBox.extents[i0];
                    pnt[i1] = 0f - mBox.extents[i1];
                    pnt[i2] = num4 - mBox.extents[i2];
                }
                else {
                    num2 += dir[i2] * dir[i2];
                    FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * vector[i1] + dir[i2] * PmE[i2];
                    FLOAT num6 = (0f - num5) / num2;
                    sqrDistance += PmE[i0] * PmE[i0] + vector[i1] * vector[i1] + PmE[i2] * PmE[i2] + num5 * num6;
                    mLineParameter = num6;
                    pnt[i0] = mBox.extents[i0];
                    pnt[i1] = 0f - mBox.extents[i1];
                    pnt[i2] = mBox.extents[i2];
                }
            }
            else {
                num2 += dir[i2] * dir[i2];
                FLOAT num5 = dir[i0] * PmE[i0] + dir[i1] * vector[i1] + dir[i2] * vector[i2];
                FLOAT num6 = (0f - num5) / num2;
                sqrDistance += PmE[i0] * PmE[i0] + vector[i1] * vector[i1] + vector[i2] * vector[i2] + num5 * num6;
                mLineParameter = num6;
                pnt[i0] = mBox.extents[i0];
                pnt[i1] = 0f - mBox.extents[i1];
                pnt[i2] = 0f - mBox.extents[i2];
            }
        }

        private static void CaseNoZeros(ref Box mBox, ref FLOAT mLineParameter, ref FVector3 pnt, ref FVector3 dir,
            ref FLOAT sqrDistance) {
            FVector3 PmE = new FVector3(pnt.x - mBox.extents[0], pnt.y - mBox.extents[1], pnt.z - mBox.extents[2]);
            FLOAT num = dir.x * PmE.y;
            FLOAT num2 = dir.y * PmE.x;
            if (num2 >= num) {
                FLOAT num3 = dir.z * PmE.x;
                FLOAT num4 = dir.x * PmE.z;
                if (num3 >= num4) {
                    Face(ref mBox, ref mLineParameter, 0, 1, 2, ref pnt, ref dir, ref PmE, ref sqrDistance);
                }
                else {
                    Face(ref mBox, ref mLineParameter, 2, 0, 1, ref pnt, ref dir, ref PmE, ref sqrDistance);
                }
            }
            else {
                FLOAT num5 = dir.z * PmE.y;
                FLOAT num6 = dir.y * PmE.z;
                if (num5 >= num6) {
                    Face(ref mBox, ref mLineParameter, 1, 2, 0, ref pnt, ref dir, ref PmE, ref sqrDistance);
                }
                else {
                    Face(ref mBox, ref mLineParameter, 2, 0, 1, ref pnt, ref dir, ref PmE, ref sqrDistance);
                }
            }
        }

        private static void Case0(ref Box mBox, ref FLOAT mLineParameter, int i0, int i1, int i2, ref FVector3 pnt,
            ref FVector3 dir, ref FLOAT sqrDistance) {
            FLOAT num = pnt[i0] - mBox.extents[i0];
            FLOAT num2 = pnt[i1] - mBox.extents[i1];
            FLOAT num3 = dir[i1] * num;
            FLOAT num4 = dir[i0] * num2;
            if (num3 >= num4) {
                pnt[i0] = mBox.extents[i0];
                FLOAT num5 = pnt[i1] + mBox.extents[i1];
                FLOAT num6 = num3 - dir[i0] * num5;
                if (num6 >= 0f) {
                    FLOAT num7 = 1f / (dir[i0] * dir[i0] + dir[i1] * dir[i1]);
                    sqrDistance += num6 * num6 * num7;
                    pnt[i1] = 0f - mBox.extents[i1];
                    mLineParameter = (0f - (dir[i0] * num + dir[i1] * num5)) * num7;
                }
                else {
                    FLOAT num8 = 1f / dir[i0];
                    pnt[i1] -= num3 * num8;
                    mLineParameter = (0f - num) * num8;
                }
            }
            else {
                pnt[i1] = mBox.extents[i1];
                FLOAT num9 = pnt[i0] + mBox.extents[i0];
                FLOAT num6 = num4 - dir[i1] * num9;
                if (num6 >= 0f) {
                    FLOAT num7 = 1f / (dir[i0] * dir[i0] + dir[i1] * dir[i1]);
                    sqrDistance += num6 * num6 * num7;
                    pnt[i0] = 0f - mBox.extents[i0];
                    mLineParameter = (0f - (dir[i0] * num9 + dir[i1] * num2)) * num7;
                }
                else {
                    FLOAT num8 = 1f / dir[i1];
                    pnt[i0] -= num4 * num8;
                    mLineParameter = (0f - num2) * num8;
                }
            }

            if (pnt[i2] < 0f - mBox.extents[i2]) {
                FLOAT num6 = pnt[i2] + mBox.extents[i2];
                sqrDistance += num6 * num6;
                pnt[i2] = 0f - mBox.extents[i2];
            }
            else if (pnt[i2] > mBox.extents[i2]) {
                FLOAT num6 = pnt[i2] - mBox.extents[i2];
                sqrDistance += num6 * num6;
                pnt[i2] = mBox.extents[i2];
            }
        }

        private static void Case00(ref Box mBox, ref FLOAT mLineParameter, int i0, int i1, int i2, ref FVector3 pnt,
            ref FVector3 dir, ref FLOAT sqrDistance) {
            mLineParameter = (mBox.extents[i0] - pnt[i0]) / dir[i0];
            pnt[i0] = mBox.extents[i0];
            if (pnt[i1] < 0f - mBox.extents[i1]) {
                FLOAT num = pnt[i1] + mBox.extents[i1];
                sqrDistance += num * num;
                pnt[i1] = 0f - mBox.extents[i1];
            }
            else if (pnt[i1] > mBox.extents[i1]) {
                FLOAT num = pnt[i1] - mBox.extents[i1];
                sqrDistance += num * num;
                pnt[i1] = mBox.extents[i1];
            }

            if (pnt[i2] < 0f - mBox.extents[i2]) {
                FLOAT num = pnt[i2] + mBox.extents[i2];
                sqrDistance += num * num;
                pnt[i2] = 0f - mBox.extents[i2];
            }
            else if (pnt[i2] > mBox.extents[i2]) {
                FLOAT num = pnt[i2] - mBox.extents[i2];
                sqrDistance += num * num;
                pnt[i2] = mBox.extents[i2];
            }
        }

        private static void Case000(ref Box mBox, ref FLOAT mLineParameter, ref FVector3 pnt, ref FLOAT sqrDistance) {
            if (pnt.x < 0f - mBox.extents[0]) {
                FLOAT num = pnt.x + mBox.extents[0];
                sqrDistance += num * num;
                pnt.x = 0f - mBox.extents[0];
            }
            else if (pnt.x > mBox.extents[0]) {
                FLOAT num = pnt.x - mBox.extents[0];
                sqrDistance += num * num;
                pnt.x = mBox.extents[0];
            }

            if (pnt.y < 0f - mBox.extents[1]) {
                FLOAT num = pnt.y + mBox.extents[1];
                sqrDistance += num * num;
                pnt.y = 0f - mBox.extents[1];
            }
            else if (pnt.y > mBox.extents[1]) {
                FLOAT num = pnt.y - mBox.extents[1];
                sqrDistance += num * num;
                pnt.y = mBox.extents[1];
            }

            if (pnt.z < 0f - mBox.extents[2]) {
                FLOAT num = pnt.z + mBox.extents[2];
                sqrDistance += num * num;
                pnt.z = 0f - mBox.extents[2];
            }
            else if (pnt.z > mBox.extents[2]) {
                FLOAT num = pnt.z - mBox.extents[2];
                sqrDistance += num * num;
                pnt.z = mBox.extents[2];
            }
        }

        public static FLOAT Line3Box3(ref Line line, ref Box box, out Line3Box3Dist info) {
            return FMath.Sqrt(SqrLine3Box3(ref line, ref box, out info));
        }

        public static FLOAT Line3Box3(ref Line line, ref Box box) {
            return FMath.Sqrt(SqrLine3Box3(ref line, ref box, out _));
        }

        public static FLOAT SqrLine3Box3(ref Line line, ref Box box, out Line3Box3Dist info) {
            FVector3 vector = line.origin - box.center;
            FVector3 pnt = new FVector3(vector.Dot(box.axis0), vector.Dot(box.axis1), vector.Dot(box.axis2));
            FVector3 dir = new FVector3(line.direction.Dot(box.axis0), line.direction.Dot(box.axis1),
                line.direction.Dot(box.axis2));
            bool flag;
            if (dir.x < 0f) {
                pnt.x = 0f - pnt.x;
                dir.x = 0f - dir.x;
                flag = true;
            }
            else {
                flag = false;
            }

            bool flag2;
            if (dir.y < 0f) {
                pnt.y = 0f - pnt.y;
                dir.y = 0f - dir.y;
                flag2 = true;
            }
            else {
                flag2 = false;
            }

            bool flag3;
            if (dir.z < 0f) {
                pnt.z = 0f - pnt.z;
                dir.z = 0f - dir.z;
                flag3 = true;
            }
            else {
                flag3 = false;
            }

            FLOAT sqrDistance = 0f;
            FLOAT mLineParameter = 0f;
            if (dir.x > 0f) {
                if (dir.y > 0f) {
                    if (dir.z > 0f) {
                        CaseNoZeros(ref box, ref mLineParameter, ref pnt, ref dir, ref sqrDistance);
                    }
                    else {
                        Case0(ref box, ref mLineParameter, 0, 1, 2, ref pnt, ref dir, ref sqrDistance);
                    }
                }
                else if (dir.z > 0f) {
                    Case0(ref box, ref mLineParameter, 0, 2, 1, ref pnt, ref dir, ref sqrDistance);
                }
                else {
                    Case00(ref box, ref mLineParameter, 0, 1, 2, ref pnt, ref dir, ref sqrDistance);
                }
            }
            else if (dir.y > 0f) {
                if (dir.z > 0f) {
                    Case0(ref box, ref mLineParameter, 1, 2, 0, ref pnt, ref dir, ref sqrDistance);
                }
                else {
                    Case00(ref box, ref mLineParameter, 1, 0, 2, ref pnt, ref dir, ref sqrDistance);
                }
            }
            else if (dir.z > 0f) {
                Case00(ref box, ref mLineParameter, 2, 0, 1, ref pnt, ref dir, ref sqrDistance);
            }
            else {
                Case000(ref box, ref mLineParameter, ref pnt, ref sqrDistance);
            }

            FVector3 closestPoint = line.origin + mLineParameter * line.direction;
            FVector3 center = box.center;
            if (flag) {
                pnt.x = 0f - pnt.x;
            }

            center += pnt.x * box.axis0;
            if (flag2) {
                pnt.y = 0f - pnt.y;
            }

            center += pnt.y * box.axis1;
            if (flag3) {
                pnt.z = 0f - pnt.z;
            }

            center += pnt.z * box.axis2;
            info.ClosestPoint0 = closestPoint;
            info.ClosestPoint1 = center;
            info.LineParameter = mLineParameter;
            return sqrDistance;
        }

        public static FLOAT SqrLine3Box3(ref Line line, ref Box box) {
            return SqrLine3Box3(ref line, ref box, out _);
        }

        /// <summary>
        /// Returns distance between two lines.
        /// </summary>
        public static FLOAT Line3Line3(ref Line line0, ref Line line1) {
            return FMath.Sqrt(SqrLine3Line3(ref line0, ref line1, out _, out _));
        }

        /// <summary>
        /// Returns distance between two lines.
        /// </summary>
        /// <param name="closestPoint0">Point on line0 closest to line1</param>
        /// <param name="closestPoint1">Point on line1 closest to line0</param>
        public static FLOAT Line3Line3(ref Line line0, ref Line line1, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            return FMath.Sqrt(SqrLine3Line3(ref line0, ref line1, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between two lines.
        /// </summary>
        public static FLOAT SqrLine3Line3(ref Line line0, ref Line line1) {
            return SqrLine3Line3(ref line0, ref line1, out _, out _);
        }

        /// <summary>
        /// Returns squared distance between two lines.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="closestPoint0">Point on line0 closest to line1</param>
        /// <param name="closestPoint1">Point on line1 closest to line0</param>
        /// <param name="line0"></param>
        public static FLOAT SqrLine3Line3(ref Line line0, ref Line line1, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            FVector3 vector = line0.origin - line1.origin;
            FLOAT num = 0f - line0.direction.Dot(line1.direction);
            FLOAT num2 = vector.Dot(line0.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num6;
            FLOAT num7;
            FLOAT num8;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(line1.direction);
                FLOAT num5 = 1f / num3;
                num6 = (num * num4 - num2) * num5;
                num7 = (num * num2 - num4) * num5;
                num8 = num6 * (num6 + num * num7 + 2f * num2) + num7 * (num * num6 + num7 + 2f * num4) + sqrMagnitude;
            }
            else {
                num6 = 0f - num2;
                num7 = 0f;
                num8 = num2 * num6 + sqrMagnitude;
            }

            closestPoint0 = line0.origin + num6 * line0.direction;
            closestPoint1 = line1.origin + num7 * line1.direction;
            if (num8 < 0f) {
                num8 = 0f;
            }

            return num8;
        }

        /// <summary>
        /// Returns distance between a line and a ray
        /// </summary>
        public static FLOAT Line3Ray3(ref Line line, ref Ray ray) {
            return FMath.Sqrt(SqrLine3Ray3(ref line, ref ray, out _, out _));
        }

        /// <summary>
        /// Returns distance between a line and a ray
        /// </summary>
        /// <param name="closestPoint0">Point on line closest to ray</param>
        /// <param name="closestPoint1">Point on ray closest to line</param>
        public static FLOAT Line3Ray3(ref Line line, ref Ray ray, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            return FMath.Sqrt(SqrLine3Ray3(ref line, ref ray, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between a line and a ray
        /// </summary>
        public static FLOAT SqrLine3Ray3(ref Line line, ref Ray ray) {
            return SqrLine3Ray3(ref line, ref ray, out _, out _);
        }

        /// <summary>
        /// Returns squared distance between a line and a ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="closestPoint0">Point on line closest to ray</param>
        /// <param name="closestPoint1">Point on ray closest to line</param>
        /// <param name="line"></param>
        public static FLOAT SqrLine3Ray3(ref Line line, ref Ray ray, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            FVector3 vector = line.origin - ray.origin;
            FLOAT num = 0f - line.direction.Dot(ray.direction);
            FLOAT num2 = vector.Dot(line.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num7;
            FLOAT num5;
            FLOAT num8;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(ray.direction);
                num5 = num * num2 - num4;
                if (num5 >= 0f) {
                    FLOAT num6 = 1f / num3;
                    num7 = (num * num4 - num2) * num6;
                    num5 *= num6;
                    num8 = num7 * (num7 + num * num5 + 2f * num2) + num5 * (num * num7 + num5 + 2f * num4) +
                           sqrMagnitude;
                }
                else {
                    num7 = 0f - num2;
                    num5 = 0f;
                    num8 = num2 * num7 + sqrMagnitude;
                }
            }
            else {
                num7 = 0f - num2;
                num5 = 0f;
                num8 = num2 * num7 + sqrMagnitude;
            }

            closestPoint0 = line.origin + num7 * line.direction;
            closestPoint1 = ray.origin + num5 * ray.direction;
            if (num8 < 0f) {
                num8 = 0f;
            }

            return num8;
        }

        /// <summary>
        /// Returns distance between a line and a segment
        /// </summary>
        public static FLOAT Line3Segment3(ref Line line, ref Segment segment) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return FMath.Sqrt(SqrLine3Segment3(ref line, ref segment, out closestPoint, out closestPoint2));
        }

        /// <summary>
        /// Returns distance between a line and a segment
        /// </summary>
        /// <param name="closestPoint0">Point on line closest to segment</param>
        /// <param name="closestPoint1">Point on segment closest to line</param>
        public static FLOAT Line3Segment3(ref Line line, ref Segment segment, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            return FMath.Sqrt(SqrLine3Segment3(ref line, ref segment, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between a line and a segment
        /// </summary>
        public static FLOAT SqrLine3Segment3(ref Line line, ref Segment segment) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return SqrLine3Segment3(ref line, ref segment, out closestPoint, out closestPoint2);
        }

        /// <summary>
        /// Returns squared distance between a line and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint0">Point on line closest to segment</param>
        /// <param name="closestPoint1">Point on segment closest to line</param>
        /// <param name="line"></param>
        public static FLOAT SqrLine3Segment3(ref Line line, ref Segment segment, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            FVector3 vector = line.origin - segment.center;
            FLOAT num = 0f - line.direction.Dot(segment.direction);
            FLOAT num2 = vector.Dot(line.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num8;
            FLOAT num5;
            FLOAT num9;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(segment.direction);
                num5 = num * num2 - num4;
                FLOAT num6 = segment.extent * num3;
                if (num5 >= 0f - num6) {
                    if (num5 <= num6) {
                        FLOAT num7 = 1f / num3;
                        num8 = (num * num4 - num2) * num7;
                        num5 *= num7;
                        num9 = num8 * (num8 + num * num5 + 2f * num2) + num5 * (num * num8 + num5 + 2f * num4) +
                               sqrMagnitude;
                    }
                    else {
                        num5 = segment.extent;
                        num8 = 0f - (num * num5 + num2);
                        num9 = (0f - num8) * num8 + num5 * (num5 + 2f * num4) + sqrMagnitude;
                    }
                }
                else {
                    num5 = 0f - segment.extent;
                    num8 = 0f - (num * num5 + num2);
                    num9 = (0f - num8) * num8 + num5 * (num5 + 2f * num4) + sqrMagnitude;
                }
            }
            else {
                num5 = 0f;
                num8 = 0f - num2;
                num9 = num2 * num8 + sqrMagnitude;
            }

            closestPoint0 = line.origin + num8 * line.direction;
            closestPoint1 = segment.center + num5 * segment.direction;
            if (num9 < 0f) {
                num9 = 0f;
            }

            return num9;
        }

        /// <summary>
        /// Returns distance between a point and an abb
        /// </summary>
        public static FLOAT Point3AAB3(ref FVector3 point, ref AABB box) {
            FLOAT num = 0f;
            FLOAT x = point.x;
            if (x < box.min.x) {
                FLOAT num2 = box.min.x - x;
                num += num2 * num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
            }

            x = point.y;
            if (x < box.min.y) {
                FLOAT num2 = box.min.y - x;
                num += num2 * num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
            }

            x = point.z;
            if (x < box.min.z) {
                FLOAT num2 = box.min.z - x;
                num += num2 * num2;
            }
            else if (x > box.max.z) {
                FLOAT num2 = x - box.max.z;
                num += num2 * num2;
            }

            return FMath.Sqrt(num);
        }

        /// <summary>
        /// Returns distance between a point and an abb
        /// </summary>
        /// <param name="box"></param>
        /// <param name="closestPoint">Point projected on an aab</param>
        /// <param name="point"></param>
        public static FLOAT Point3AAB3(ref FVector3 point, ref AABB box, out FVector3 closestPoint) {
            FLOAT num = 0f;
            closestPoint = point;
            FLOAT x = point.x;
            if (x < box.min.x) {
                FLOAT num2 = box.min.x - x;
                num += num2 * num2;
                closestPoint.x += num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
                closestPoint.x -= num2;
            }

            x = point.y;
            if (x < box.min.y) {
                FLOAT num2 = box.min.y - x;
                num += num2 * num2;
                closestPoint.y += num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
                closestPoint.y -= num2;
            }

            x = point.z;
            if (x < box.min.z) {
                FLOAT num2 = box.min.z - x;
                num += num2 * num2;
                closestPoint.z += num2;
            }
            else if (x > box.max.z) {
                FLOAT num2 = x - box.max.z;
                num += num2 * num2;
                closestPoint.z -= num2;
            }

            return FMath.Sqrt(num);
        }

        /// <summary>
        /// Returns squared distance between a point and an abb
        /// </summary>
        public static FLOAT SqrPoint3AAB3(ref FVector3 point, ref AABB box) {
            FLOAT num = 0f;
            FLOAT x = point.x;
            if (x < box.min.x) {
                FLOAT num2 = box.min.x - x;
                num += num2 * num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
            }

            x = point.y;
            if (x < box.min.y) {
                FLOAT num2 = box.min.y - x;
                num += num2 * num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
            }

            x = point.z;
            if (x < box.min.z) {
                FLOAT num2 = box.min.z - x;
                num += num2 * num2;
            }
            else if (x > box.max.z) {
                FLOAT num2 = x - box.max.z;
                num += num2 * num2;
            }

            return num;
        }

        /// <summary>
        /// Returns squared distance between a point and an abb
        /// </summary>
        /// <param name="box"></param>
        /// <param name="closestPoint">Point projected on an aab</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint3AAB3(ref FVector3 point, ref AABB box, out FVector3 closestPoint) {
            FLOAT num = 0f;
            closestPoint = point;
            FLOAT x = point.x;
            if (x < box.min.x) {
                FLOAT num2 = box.min.x - x;
                num += num2 * num2;
                closestPoint.x += num2;
            }
            else if (x > box.max.x) {
                FLOAT num2 = x - box.max.x;
                num += num2 * num2;
                closestPoint.x -= num2;
            }

            x = point.y;
            if (x < box.min.y) {
                FLOAT num2 = box.min.y - x;
                num += num2 * num2;
                closestPoint.y += num2;
            }
            else if (x > box.max.y) {
                FLOAT num2 = x - box.max.y;
                num += num2 * num2;
                closestPoint.y -= num2;
            }

            x = point.z;
            if (x < box.min.z) {
                FLOAT num2 = box.min.z - x;
                num += num2 * num2;
                closestPoint.z += num2;
            }
            else if (x > box.max.z) {
                FLOAT num2 = x - box.max.z;
                num += num2 * num2;
                closestPoint.z -= num2;
            }

            return num;
        }

        /// <summary>
        /// Returns distance between a point and a box
        /// </summary>
        public static FLOAT Point3Box3(ref FVector3 point, ref Box box) {
            return FMath.Sqrt(SqrPoint3Box3(ref point, ref box));
        }

        /// <summary>
        /// Returns distance between a point and a box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="closestPoint">Point projected on a box</param>
        /// <param name="point"></param>
        public static FLOAT Point3Box3(ref FVector3 point, ref Box box, out FVector3 closestPoint) {
            return FMath.Sqrt(SqrPoint3Box3(ref point, ref box, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a box
        /// </summary>
        public static FLOAT SqrPoint3Box3(ref FVector3 point, ref Box box) {
            FVector3 vector = point - box.center;
            FLOAT num = 0f;
            FLOAT num2 = vector.Dot(box.axis0);
            FLOAT x = box.extents.x;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
            }

            FLOAT num4 = vector.Dot(box.axis1);
            x = box.extents.y;
            if (num4 < 0f - x) {
                FLOAT num3 = num4 + x;
                num += num3 * num3;
            }
            else if (num4 > x) {
                FLOAT num3 = num4 - x;
                num += num3 * num3;
            }

            FLOAT num5 = vector.Dot(box.axis2);
            x = box.extents.z;
            if (num5 < 0f - x) {
                FLOAT num3 = num5 + x;
                num += num3 * num3;
            }
            else if (num5 > x) {
                FLOAT num3 = num5 - x;
                num += num3 * num3;
            }

            return num;
        }

        /// <summary>
        /// Returns squared distance between a point and a box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="point"></param>
        /// <param name="closestPoint">Point projected on a box</param>
        public static FLOAT SqrPoint3Box3(ref FVector3 point, ref Box box, out FVector3 closestPoint) {
            /*
             * 以point-center为轴向，做投影，得出最长的距离
             * 用该距离 * （point - center）.Normal，即得到最近点
             */
            FVector3 vector = point - box.center;
            FLOAT num = 0f;
            FLOAT num2 = vector.Dot(box.axis0);
            FLOAT x = box.extents.x;
            if (num2 < 0f - x) {
                FLOAT num3 = num2 + x;
                num += num3 * num3;
                num2 = 0f - x;
            }
            else if (num2 > x) {
                FLOAT num3 = num2 - x;
                num += num3 * num3;
                num2 = x;
            }

            FLOAT num4 = vector.Dot(box.axis1);
            x = box.extents.y;
            if (num4 < 0f - x) {
                FLOAT num3 = num4 + x;
                num += num3 * num3;
                num4 = 0f - x;
            }
            else if (num4 > x) {
                FLOAT num3 = num4 - x;
                num += num3 * num3;
                num4 = x;
            }

            FLOAT num5 = vector.Dot(box.axis2);
            x = box.extents.z;
            if (num5 < 0f - x) {
                FLOAT num3 = num5 + x;
                num += num3 * num3;
                num5 = 0f - x;
            }
            else if (num5 > x) {
                FLOAT num3 = num5 - x;
                num += num3 * num3;
                num5 = x;
            }

            closestPoint = box.center + num2 * box.axis0 + num4 * box.axis1 + num5 * box.axis2;
            return num;
        }

        /// <summary>
        /// Returns distance between a point and a circle
        /// </summary>
        /// <param name="point"></param>
        /// <param name="circle"></param>
        /// <param name="solid">When true circle considered to be solid disk otherwise hollow circle.</param>
        public static FLOAT Point3Circle3(ref FVector3 point, ref Circle circle, bool solid = true) {
            return FMath.Sqrt(SqrPoint3Circle3(ref point, ref circle, out var closestPoint, solid));
        }

        /// <summary>
        /// Returns distance between a point and a circle
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="closestPoint">Point projected on a circle</param>
        /// <param name="solid">When true circle considered to be solid disk otherwise hollow circle.</param>
        /// <param name="point"></param>
        public static FLOAT Point3Circle3(ref FVector3 point, ref Circle circle, out FVector3 closestPoint,
            bool solid = true) {
            return FMath.Sqrt(SqrPoint3Circle3(ref point, ref circle, out closestPoint, solid));
        }

        /// <summary>
        /// Returns squared distance between a point and a circle
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="solid">When true circle considered to be solid disk otherwise hollow circle.</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint3Circle3(ref FVector3 point, ref Circle circle, bool solid = true) {
            return SqrPoint3Circle3(ref point, ref circle, out var closestPoint, solid);
        }

        /// <summary>
        /// Returns squared distance between a point and a circle
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="closestPoint">Point projected on a circle</param>
        /// <param name="solid">When true circle considered to be solid disk otherwise hollow circle.</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint3Circle3(ref FVector3 point, ref Circle circle, out FVector3 closestPoint,
            bool solid = true) {
            if (solid) {
                FVector3 vector = point - circle.center;
                FLOAT num = vector.Dot(circle.normal);
                FVector3 vector2 = vector - num * circle.normal;
                FLOAT sqrMagnitude = vector2.sqrMagnitude;
                if (sqrMagnitude > circle.radius) {
                    closestPoint = circle.center + circle.radius / FMath.Sqrt(sqrMagnitude) * vector2;
                    return (point - closestPoint).sqrMagnitude;
                }

                closestPoint = circle.center + vector2;
                return num * num;
            }

            FVector3 vector3 = point - circle.center;
            FLOAT num2 = vector3.Dot(circle.normal);
            FVector3 vector4 = vector3 - num2 * circle.normal;
            FLOAT sqrMagnitude2 = vector4.sqrMagnitude;
            if (sqrMagnitude2 >= 1E-05f) {
                closestPoint = circle.center + circle.radius / FMath.Sqrt(sqrMagnitude2) * vector4;
                return (point - closestPoint).sqrMagnitude;
            }

            closestPoint = circle.Eval(0f);
            return circle.radius * circle.radius + num2 * num2;
        }

        /// <summary>
        /// Returns distance between a point and a line
        /// </summary>
        public static FLOAT Point3Line3(ref FVector3 point, ref Line line) {
            return FMath.Sqrt(SqrPoint3Line3(ref point, ref line));
        }

        /// <summary>
        /// Returns distance between a point and a line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="closestPoint">Point projected on a line</param>
        /// <param name="point"></param>
        public static FLOAT Point3Line3(ref FVector3 point, ref Line line, out FVector3 closestPoint) {
            return FMath.Sqrt(SqrPoint3Line3(ref point, ref line, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a line
        /// </summary>
        public static FLOAT SqrPoint3Line3(ref FVector3 point, ref Line line) {
            FVector3 value = point - line.origin;
            FLOAT num = line.direction.Dot(value);
            FVector3 vector = line.origin + num * line.direction;
            return (vector - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns squared distance between a point and a line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="closestPoint">Point projected on a line</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint3Line3(ref FVector3 point, ref Line line, out FVector3 closestPoint) {
            FVector3 value = point - line.origin;
            FLOAT num = line.direction.Dot(value);
            closestPoint = line.origin + num * line.direction;
            return (closestPoint - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns distance between a point and a plane
        /// </summary>
        public static FLOAT Point3Plane3(ref FVector3 point, ref Plane plane) {
            FLOAT f = plane.normal.Dot(point) - plane.constant;
            return FMath.Abs(f);
        }

        /// <summary>
        /// Returns distance between a point and a plane
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="closestPoint">Point projected on a plane</param>
        /// <param name="point"></param>
        public static FLOAT Point3Plane3(ref FVector3 point, ref Plane plane, out FVector3 closestPoint) {
            FLOAT num = plane.normal.Dot(point) - plane.constant;
            closestPoint = point - num * plane.normal;
            return FMath.Abs(num);
        }

        public static FLOAT Point3Plane3(ref FVector3 point, ref FVector3 p0, ref FVector3 p1, ref FVector3 p2, out FVector3 closestPoint) {
            FVector3.Subtract(ref p1, ref p0, out var vector);
            FVector3.Subtract(ref p2, ref p0, out var value);
            var normal = vector.NormalizeCross(value);
            var constant = normal.Dot(p0);
            FLOAT num = normal.Dot(point) - constant;
            closestPoint = point - num * normal;
            return FMath.Abs(num);
        }

        /// <summary>
        /// Returns squared distance between a point and a plane
        /// </summary>
        public static FLOAT SqrPoint3Plane3(ref FVector3 point, ref Plane plane) {
            FLOAT num = plane.normal.Dot(point) - plane.constant;
            return num * num;
        }

        /// <summary>
        /// Returns squared distance between a point and a plane
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="closestPoint">Point projected on a plane</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint3Plane3(ref FVector3 point, ref Plane plane, out FVector3 closestPoint) {
            FLOAT num = plane.normal.Dot(point) - plane.constant;
            closestPoint = point - num * plane.normal;
            return num * num;
        }

        public static FLOAT SqrPoint3Plane3(ref FVector3 point, ref FVector3 p0, ref FVector3 p1, ref FVector3 p2, out FVector3 closestPoint) {
            FVector3.Subtract(ref p1, ref p0, out var vector);
            FVector3.Subtract(ref p2, ref p0, out var value);
            var normal = vector.NormalizeCross(value);
            var constant = normal.Dot(p0);
            FLOAT num = normal.Dot(point) - constant;
            closestPoint = point - num * normal;
            return num * num;
        }

        /// <summary>
        /// Returns distance between a point and a ray
        /// </summary>
        public static FLOAT Point3Ray3(ref FVector3 point, ref Ray ray) {
            return FMath.Sqrt(SqrPoint3Ray3(ref point, ref ray));
        }

        /// <summary>
        /// Returns distance between a point and a ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="closestPoint">Point projected on a ray and clamped by ray origin</param>
        /// <param name="point"></param>
        public static FLOAT Point3Ray3(ref FVector3 point, ref Ray ray, out FVector3 closestPoint) {
            return FMath.Sqrt(SqrPoint3Ray3(ref point, ref ray, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a ray
        /// </summary>
        public static FLOAT SqrPoint3Ray3(ref FVector3 point, ref Ray ray) {
            FVector3 value = point - ray.origin;
            FLOAT num = ray.direction.Dot(value);
            FVector3 vector = ((!(num > 0f)) ? ray.origin : (ray.origin + num * ray.direction));
            return (vector - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns squared distance between a point and a ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="closestPoint">Point projected on a ray and clamped by ray origin</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint3Ray3(ref FVector3 point, ref Ray ray, out FVector3 closestPoint) {
            FVector3 value = point - ray.origin;
            FLOAT num = ray.direction.Dot(value);
            if (num > 0f) {
                closestPoint = ray.origin + num * ray.direction;
            }
            else {
                closestPoint = ray.origin;
            }

            return (closestPoint - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns distance between a point and a rectangle
        /// </summary>
        public static FLOAT Point3Rectangle3(ref FVector3 point, ref Rectangle rectangle) {
            return FMath.Sqrt(SqrPoint3Rectangle3(ref point, ref rectangle));
        }

        /// <summary>
        /// Returns distance between a point and a rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="closestPoint">Point projected on a rectangle</param>
        /// <param name="point"></param>
        public static FLOAT
            Point3Rectangle3(ref FVector3 point, ref Rectangle rectangle, out FVector3 closestPoint) {
            return FMath.Sqrt(SqrPoint3Rectangle3(ref point, ref rectangle, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a rectangle
        /// </summary>
        public static FLOAT SqrPoint3Rectangle3(ref FVector3 point, ref Rectangle rectangle) {
            FVector3 vector = rectangle.center - point;
            FLOAT num = vector.Dot(rectangle.axis0);
            FLOAT num2 = vector.Dot(rectangle.axis1);
            FLOAT num3 = 0f - num;
            FLOAT num4 = 0f - num2;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT x = rectangle.extents.x;
            if (num3 < 0f - x) {
                num3 = 0f - x;
            }
            else if (num3 > x) {
                num3 = x;
            }

            sqrMagnitude += num3 * (num3 + 2f * num);
            x = rectangle.extents.y;
            if (num4 < 0f - x) {
                num4 = 0f - x;
            }
            else if (num4 > x) {
                num4 = x;
            }

            sqrMagnitude += num4 * (num4 + 2f * num2);
            if (sqrMagnitude < 0f) {
                sqrMagnitude = 0f;
            }

            return sqrMagnitude;
        }

        /// <summary>
        /// Returns squared distance between a point and a rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="closestPoint">Point projected on a rectangle</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint3Rectangle3(ref FVector3 point, ref Rectangle rectangle,
            out FVector3 closestPoint) {
            FVector3 vector = rectangle.center - point;
            FLOAT num = vector.Dot(rectangle.axis0);
            FLOAT num2 = vector.Dot(rectangle.axis1);
            FLOAT num3 = 0f - num;
            FLOAT num4 = 0f - num2;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT x = rectangle.extents.x;
            if (num3 < 0f - x) {
                num3 = 0f - x;
            }
            else if (num3 > x) {
                num3 = x;
            }

            sqrMagnitude += num3 * (num3 + 2f * num);
            x = rectangle.extents.y;
            if (num4 < 0f - x) {
                num4 = 0f - x;
            }
            else if (num4 > x) {
                num4 = x;
            }

            sqrMagnitude += num4 * (num4 + 2f * num2);
            if (sqrMagnitude < 0f) {
                sqrMagnitude = 0f;
            }

            closestPoint = rectangle.center + num3 * rectangle.axis0 + num4 * rectangle.axis1;
            return sqrMagnitude;
        }

        /// <summary>
        /// Returns distance between a point and a segment
        /// </summary>
        public static FLOAT Point3Segment3(ref FVector3 point, ref Segment segment) {
            return FMath.Sqrt(SqrPoint3Segment3(ref point, ref segment));
        }

        /// <summary>
        /// Returns distance between a point and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint">Point projected on a segment and clamped by segment endpoints</param>
        /// <param name="point"></param>
        public static FLOAT Point3Segment3(ref FVector3 point, ref Segment segment, out FVector3 closestPoint) {
            return FMath.Sqrt(SqrPoint3Segment3(ref point, ref segment, out closestPoint));
        }

        /// <summary>
        /// Returns squared distance between a point and a segment
        /// </summary>
        public static FLOAT SqrPoint3Segment3(ref FVector3 point, ref Segment segment) {
            FVector3 value = point - segment.center;
            FLOAT num = segment.direction.Dot(value);
            FVector3 vector = ((!(0f - segment.extent < num))
                ? segment.p0
                : ((!(num < segment.extent)) ? segment.p1 : (segment.center + num * segment.direction)));
            return (vector - point).sqrMagnitude;
        }

        /// <summary>
        /// Returns squared distance between a point and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="closestPoint">Point projected on a segment and clamped by segment endpoints</param>
        /// <param name="point"></param>
        public static FLOAT SqrPoint3Segment3(ref FVector3 point, ref Segment segment, out FVector3 closestPoint) {
            FVector3 value = point - segment.center;
            FLOAT num = segment.direction.Dot(value);
            if (0f - segment.extent < num) {
                if (num < segment.extent) {
                    closestPoint = segment.center + num * segment.direction;
                }
                else {
                    closestPoint = segment.p1;
                }
            }
            else {
                closestPoint = segment.p0;
            }

            return (closestPoint - point).sqrMagnitude;
        }

        /// <summary>
        /// 计算点到线段的最短距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="end"></param>
        /// <param name="closestPoint">最近点</param>
        /// <returns>最近距离</returns>
        public static FLOAT SqrPoint3Segment3(ref FVector3 point, ref FVector3 origin, ref FVector3 end,
            out FVector3 closestPoint) {
            FVector3.Subtract(ref end, ref origin, out var v1);
            FVector3.Subtract(ref point, ref origin, out var v2);

            FLOAT sqrLength = v1.sqrMagnitude;

            if (sqrLength < FLOAT.Epsilon) {
                closestPoint = origin;
                FVector3.DistanceSquared(ref point, ref closestPoint, out var result);
                return result;
            }

            FLOAT projection = FVector3.Dot(v1, v2) / sqrLength;
            if (projection < 0) {
                closestPoint = origin;
                FVector3.DistanceSquared(ref point, ref closestPoint, out var result);
                return result;
            }
            else if (projection > 1) {
                closestPoint = end;
                FVector3.DistanceSquared(ref point, ref closestPoint, out var result);
                return result;
            }
            else {
                FVector3.Multiply(ref v1, projection, out var v3);
                FVector3.Add(ref v3, ref origin, out closestPoint);
                FVector3.DistanceSquared(ref point, ref closestPoint, out var result);
                return result;
            }
        }

        /// <summary>
        /// Returns distance between a point and a sphere
        /// </summary>
        public static FLOAT Point3Sphere3(ref FVector3 point, ref Sphere sphere) {
            FLOAT num = (point - sphere.center).magnitude - sphere.radius;
            if (!(num > 0f)) {
                return 0f;
            }

            return num;
        }

        /// <summary>
        /// Returns distance between a point and a sphere
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="closestPoint">Point projected on a sphere</param>
        /// <param name="point"></param>
        public static FLOAT Point3Sphere3(ref FVector3 point, ref Sphere sphere, out FVector3 closestPoint) {
            FVector3 vector = point - sphere.center;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > sphere.radius * sphere.radius) {
                FLOAT num = FMath.Sqrt(sqrMagnitude);
                closestPoint = sphere.center + vector * (sphere.radius / num);
                return num - sphere.radius;
            }

            closestPoint = point;
            return 0f;
        }

        /// <summary>
        /// Returns squared distance between a point and a sphere
        /// </summary>
        public static FLOAT SqrPoint3Sphere3(ref FVector3 point, ref Sphere sphere) {
            FLOAT num = (point - sphere.center).magnitude - sphere.radius;
            if (!(num > 0f)) {
                return 0f;
            }

            return num * num;
        }

        /// <summary>
        /// Returns squared distance between a point and a sphere
        /// </summary>
        /// <param name="closestPoint">Point projected on a sphere</param>
        public static FLOAT SqrPoint3Sphere3(ref FVector3 point, ref Sphere sphere, out FVector3 closestPoint) {
            FVector3 vector = point - sphere.center;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > sphere.radius * sphere.radius) {
                FLOAT num = FMath.Sqrt(sqrMagnitude);
                closestPoint = sphere.center + vector * (sphere.radius / num);
                FLOAT num2 = num - sphere.radius;
                return num2 * num2;
            }

            closestPoint = point;
            return 0f;
        }

        /// <summary>
        /// Returns distance between two rays
        /// </summary>
        public static FLOAT Ray3Ray3(ref Ray ray0, ref Ray ray1) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return FMath.Sqrt(SqrRay3Ray3(ref ray0, ref ray1, out closestPoint, out closestPoint2));
        }

        /// <summary>
        /// Returns distance between two rays
        /// </summary>
        /// <param name="closestPoint0">Point on ray0 closest to ray1</param>
        /// <param name="closestPoint1">Point on ray1 closest to ray0</param>
        public static FLOAT Ray3Ray3(ref Ray ray0, ref Ray ray1, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            return FMath.Sqrt(SqrRay3Ray3(ref ray0, ref ray1, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between two rays
        /// </summary>
        public static FLOAT SqrRay3Ray3(ref Ray ray0, ref Ray ray1) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return SqrRay3Ray3(ref ray0, ref ray1, out closestPoint, out closestPoint2);
        }

        /// <summary>
        /// Returns squared distance between two rays
        /// </summary>
        /// <param name="closestPoint0">Point on ray0 closest to ray1</param>
        /// <param name="closestPoint1">Point on ray1 closest to ray0</param>
        public static FLOAT SqrRay3Ray3(ref Ray ray0, ref Ray ray1, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            FVector3 vector = ray0.origin - ray1.origin;
            FLOAT num = 0f - ray0.direction.Dot(ray1.direction);
            FLOAT num2 = vector.Dot(ray0.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num3 = FMath.Abs(1f - num * num);
            FLOAT num5;
            FLOAT num6;
            FLOAT num8;
            if (num3 >= 1E-05f) {
                FLOAT num4 = 0f - vector.Dot(ray1.direction);
                num5 = num * num4 - num2;
                num6 = num * num2 - num4;
                if (num5 >= 0f) {
                    if (num6 >= 0f) {
                        FLOAT num7 = 1f / num3;
                        num5 *= num7;
                        num6 *= num7;
                        num8 = num5 * (num5 + num * num6 + 2f * num2) + num6 * (num * num5 + num6 + 2f * num4) +
                               sqrMagnitude;
                    }
                    else {
                        num6 = 0f;
                        if (num2 >= 0f) {
                            num5 = 0f;
                            num8 = sqrMagnitude;
                        }
                        else {
                            num5 = 0f - num2;
                            num8 = num2 * num5 + sqrMagnitude;
                        }
                    }
                }
                else if (num6 >= 0f) {
                    num5 = 0f;
                    if (num4 >= 0f) {
                        num6 = 0f;
                        num8 = sqrMagnitude;
                    }
                    else {
                        num6 = 0f - num4;
                        num8 = num4 * num6 + sqrMagnitude;
                    }
                }
                else if (num2 < 0f) {
                    num5 = 0f - num2;
                    num6 = 0f;
                    num8 = num2 * num5 + sqrMagnitude;
                }
                else {
                    num5 = 0f;
                    if (num4 >= 0f) {
                        num6 = 0f;
                        num8 = sqrMagnitude;
                    }
                    else {
                        num6 = 0f - num4;
                        num8 = num4 * num6 + sqrMagnitude;
                    }
                }
            }
            else if (num > 0f) {
                num6 = 0f;
                if (num2 >= 0f) {
                    num5 = 0f;
                    num8 = sqrMagnitude;
                }
                else {
                    num5 = 0f - num2;
                    num8 = num2 * num5 + sqrMagnitude;
                }
            }
            else if (num2 >= 0f) {
                FLOAT num4 = 0f - vector.Dot(ray1.direction);
                num5 = 0f;
                num6 = 0f - num4;
                num8 = num4 * num6 + sqrMagnitude;
            }
            else {
                num5 = 0f - num2;
                num6 = 0f;
                num8 = num2 * num5 + sqrMagnitude;
            }

            closestPoint0 = ray0.origin + num5 * ray0.direction;
            closestPoint1 = ray1.origin + num6 * ray1.direction;
            if (num8 < 0f) {
                num8 = 0f;
            }

            return num8;
        }

        /// <summary>
        /// Returns distance between a ray and a segment
        /// </summary>
        public static FLOAT Ray3Segment3(ref Ray ray, ref Segment segment) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return FMath.Sqrt(SqrRay3Segment3(ref ray, ref segment, out closestPoint, out closestPoint2));
        }

        /// <summary>
        /// Returns distance between a ray and a segment
        /// </summary>
        /// <param name="closestPoint0">Point on ray closest to segment</param>
        /// <param name="closestPoint1">Point on segment closest to ray</param>
        public static FLOAT Ray3Segment3(ref Ray ray, ref Segment segment, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            return FMath.Sqrt(SqrRay3Segment3(ref ray, ref segment, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between a ray and a segment
        /// </summary>
        public static FLOAT SqrRay3Segment3(ref Ray ray, ref Segment segment) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return SqrRay3Segment3(ref ray, ref segment, out closestPoint, out closestPoint2);
        }

        /// <summary>
        /// Returns squared distance between a ray and a segment
        /// </summary>
        /// <param name="closestPoint0">Point on ray closest to segment</param>
        /// <param name="closestPoint1">Point on segment closest to ray</param>
        public static FLOAT SqrRay3Segment3(ref Ray ray, ref Segment segment, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            FVector3 vector = ray.origin - segment.center;
            FLOAT num = 0f - ray.direction.Dot(segment.direction);
            FLOAT num2 = vector.Dot(ray.direction);
            FLOAT num3 = 0f - vector.Dot(segment.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num4 = FMath.Abs(1f - num * num);
            FLOAT num5;
            FLOAT num6;
            FLOAT num9;
            if (num4 >= 1E-05f) {
                num5 = num * num3 - num2;
                num6 = num * num2 - num3;
                FLOAT num7 = segment.extent * num4;
                if (num5 >= 0f) {
                    if (num6 >= 0f - num7) {
                        if (num6 <= num7) {
                            FLOAT num8 = 1f / num4;
                            num5 *= num8;
                            num6 *= num8;
                            num9 = num5 * (num5 + num * num6 + 2f * num2) + num6 * (num * num5 + num6 + 2f * num3) +
                                   sqrMagnitude;
                        }
                        else {
                            num6 = segment.extent;
                            num5 = 0f - (num * num6 + num2);
                            if (num5 > 0f) {
                                num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else {
                                num5 = 0f;
                                num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                        }
                    }
                    else {
                        num6 = 0f - segment.extent;
                        num5 = 0f - (num * num6 + num2);
                        if (num5 > 0f) {
                            num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else {
                            num5 = 0f;
                            num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                    }
                }
                else if (num6 <= 0f - num7) {
                    num5 = 0f - ((0f - num) * segment.extent + num2);
                    if (num5 > 0f) {
                        num6 = 0f - segment.extent;
                        num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                    else {
                        num5 = 0f;
                        num6 = 0f - num3;
                        if (num6 < 0f - segment.extent) {
                            num6 = 0f - segment.extent;
                        }
                        else if (num6 > segment.extent) {
                            num6 = segment.extent;
                        }

                        num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                }
                else if (num6 <= num7) {
                    num5 = 0f;
                    num6 = 0f - num3;
                    if (num6 < 0f - segment.extent) {
                        num6 = 0f - segment.extent;
                    }
                    else if (num6 > segment.extent) {
                        num6 = segment.extent;
                    }

                    num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                }
                else {
                    num5 = 0f - (num * segment.extent + num2);
                    if (num5 > 0f) {
                        num6 = segment.extent;
                        num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                    else {
                        num5 = 0f;
                        num6 = 0f - num3;
                        if (num6 < 0f - segment.extent) {
                            num6 = 0f - segment.extent;
                        }
                        else if (num6 > segment.extent) {
                            num6 = segment.extent;
                        }

                        num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                }
            }
            else {
                num6 = ((!(num > 0f)) ? segment.extent : (0f - segment.extent));
                num5 = 0f - (num * num6 + num2);
                if (num5 > 0f) {
                    num9 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                }
                else {
                    num5 = 0f;
                    num9 = num6 * (num6 + 2f * num3) + sqrMagnitude;
                }
            }

            closestPoint0 = ray.origin + num5 * ray.direction;
            closestPoint1 = segment.center + num6 * segment.direction;
            if (num9 < 0f) {
                num9 = 0f;
            }

            return num9;
        }

        public static FLOAT Segment3Box3(ref Segment segment, ref Box box, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            return FMath.Sqrt(SqrSegment3Box3(ref segment, ref box, out closestPoint0, out closestPoint1));
        }

        public static FLOAT Segment3Box3(ref Segment segment, ref Box box) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return FMath.Sqrt(SqrSegment3Box3(ref segment, ref box, out closestPoint, out closestPoint2));
        }

        public static FLOAT SqrSegment3Box3(ref Segment segment, ref Box box, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            Line line = new Line(segment.center, segment.direction);
            Line3Box3Dist info;
            FLOAT result = SqrLine3Box3(ref line, ref box, out info);
            FLOAT lineParameter = info.LineParameter;
            if (lineParameter >= 0f - segment.extent) {
                if (lineParameter <= segment.extent) {
                    closestPoint0 = info.ClosestPoint0;
                    closestPoint1 = info.ClosestPoint1;
                }
                else {
                    closestPoint0 = segment.p1;
                    result = SqrPoint3Box3(ref segment.p1, ref box, out closestPoint1);
                }
            }
            else {
                closestPoint0 = segment.p0;
                result = SqrPoint3Box3(ref segment.p0, ref box, out closestPoint1);
            }

            return result;
        }

        public static FLOAT SqrSegment3Box3(ref Segment segment, ref Box box) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return SqrSegment3Box3(ref segment, ref box, out closestPoint, out closestPoint2);
        }

        /// <summary>
        /// Returns distance between two segments
        /// </summary>
        public static FLOAT Segment3Segment3(ref Segment segment0, ref Segment segment1) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return FMath.Sqrt(SqrSegment3Segment3(ref segment0, ref segment1, out closestPoint, out closestPoint2));
        }

        /// <summary>
        /// Returns distance between two segments
        /// </summary>
        /// <param name="closestPoint0">Point on segment0 closest to segment1</param>
        /// <param name="closestPoint1">Point on segment1 closest to segment0</param>
        public static FLOAT Segment3Segment3(ref Segment segment0, ref Segment segment1, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            return FMath.Sqrt(SqrSegment3Segment3(ref segment0, ref segment1, out closestPoint0, out closestPoint1));
        }

        /// <summary>
        /// Returns squared distance between two segments
        /// </summary>
        public static FLOAT SqrSegment3Segment3(ref Segment segment0, ref Segment segment1) {
            FVector3 closestPoint;
            FVector3 closestPoint2;
            return SqrSegment3Segment3(ref segment0, ref segment1, out closestPoint, out closestPoint2);
        }

        /// <summary>
        /// Returns squared distance between two segments
        /// </summary>
        /// <param name="closestPoint0">Point on segment0 closest to segment1</param>
        /// <param name="closestPoint1">Point on segment1 closest to segment0</param>
        public static FLOAT SqrSegment3Segment3(ref Segment segment0, ref Segment segment1, out FVector3 closestPoint0,
            out FVector3 closestPoint1) {
            FVector3 vector = segment0.center - segment1.center;
            FLOAT num = 0f - segment0.direction.Dot(segment1.direction);
            FLOAT num2 = vector.Dot(segment0.direction);
            FLOAT num3 = 0f - vector.Dot(segment1.direction);
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num4 = FMath.Abs(1f - num * num);
            FLOAT num5;
            FLOAT num6;
            FLOAT num10;
            if (num4 >= 1E-05f) {
                num5 = num * num3 - num2;
                num6 = num * num2 - num3;
                FLOAT num7 = segment0.extent * num4;
                FLOAT num8 = segment1.extent * num4;
                if (num5 >= 0f - num7) {
                    if (num5 <= num7) {
                        if (num6 >= 0f - num8) {
                            if (num6 <= num8) {
                                FLOAT num9 = 1f / num4;
                                num5 *= num9;
                                num6 *= num9;
                                num10 = num5 * (num5 + num * num6 + 2f * num2) +
                                        num6 * (num * num5 + num6 + 2f * num3) + sqrMagnitude;
                            }
                            else {
                                num6 = segment1.extent;
                                FLOAT num11 = 0f - (num * num6 + num2);
                                if (num11 < 0f - segment0.extent) {
                                    num5 = 0f - segment0.extent;
                                    num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                                }
                                else if (num11 <= segment0.extent) {
                                    num5 = num11;
                                    num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                                }
                                else {
                                    num5 = segment0.extent;
                                    num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                                }
                            }
                        }
                        else {
                            num6 = 0f - segment1.extent;
                            FLOAT num11 = 0f - (num * num6 + num2);
                            if (num11 < 0f - segment0.extent) {
                                num5 = 0f - segment0.extent;
                                num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else if (num11 <= segment0.extent) {
                                num5 = num11;
                                num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else {
                                num5 = segment0.extent;
                                num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                        }
                    }
                    else if (num6 >= 0f - num8) {
                        if (num6 <= num8) {
                            num5 = segment0.extent;
                            FLOAT num12 = 0f - (num * num5 + num3);
                            if (num12 < 0f - segment1.extent) {
                                num6 = 0f - segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else if (num12 <= segment1.extent) {
                                num6 = num12;
                                num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else {
                                num6 = segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                        }
                        else {
                            num6 = segment1.extent;
                            FLOAT num11 = 0f - (num * num6 + num2);
                            if (num11 < 0f - segment0.extent) {
                                num5 = 0f - segment0.extent;
                                num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else if (num11 <= segment0.extent) {
                                num5 = num11;
                                num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                            }
                            else {
                                num5 = segment0.extent;
                                FLOAT num12 = 0f - (num * num5 + num3);
                                if (num12 < 0f - segment1.extent) {
                                    num6 = 0f - segment1.extent;
                                    num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                                }
                                else if (num12 <= segment1.extent) {
                                    num6 = num12;
                                    num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                                }
                                else {
                                    num6 = segment1.extent;
                                    num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                                }
                            }
                        }
                    }
                    else {
                        num6 = 0f - segment1.extent;
                        FLOAT num11 = 0f - (num * num6 + num2);
                        if (num11 < 0f - segment0.extent) {
                            num5 = 0f - segment0.extent;
                            num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else if (num11 <= segment0.extent) {
                            num5 = num11;
                            num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else {
                            num5 = segment0.extent;
                            FLOAT num12 = 0f - (num * num5 + num3);
                            if (num12 > segment1.extent) {
                                num6 = segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else if (num12 >= 0f - segment1.extent) {
                                num6 = num12;
                                num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else {
                                num6 = 0f - segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                        }
                    }
                }
                else if (num6 >= 0f - num8) {
                    if (num6 <= num8) {
                        num5 = 0f - segment0.extent;
                        FLOAT num12 = 0f - (num * num5 + num3);
                        if (num12 < 0f - segment1.extent) {
                            num6 = 0f - segment1.extent;
                            num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                        else if (num12 <= segment1.extent) {
                            num6 = num12;
                            num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                        else {
                            num6 = segment1.extent;
                            num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                    }
                    else {
                        num6 = segment1.extent;
                        FLOAT num11 = 0f - (num * num6 + num2);
                        if (num11 > segment0.extent) {
                            num5 = segment0.extent;
                            num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else if (num11 >= 0f - segment0.extent) {
                            num5 = num11;
                            num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                        }
                        else {
                            num5 = 0f - segment0.extent;
                            FLOAT num12 = 0f - (num * num5 + num3);
                            if (num12 < 0f - segment1.extent) {
                                num6 = 0f - segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else if (num12 <= segment1.extent) {
                                num6 = num12;
                                num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                            else {
                                num6 = segment1.extent;
                                num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                            }
                        }
                    }
                }
                else {
                    num6 = 0f - segment1.extent;
                    FLOAT num11 = 0f - (num * num6 + num2);
                    if (num11 > segment0.extent) {
                        num5 = segment0.extent;
                        num10 = num5 * (num5 - 2f * num11) + num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                    else if (num11 >= 0f - segment0.extent) {
                        num5 = num11;
                        num10 = (0f - num5) * num5 + num6 * (num6 + 2f * num3) + sqrMagnitude;
                    }
                    else {
                        num5 = 0f - segment0.extent;
                        FLOAT num12 = 0f - (num * num5 + num3);
                        if (num12 < 0f - segment1.extent) {
                            num6 = 0f - segment1.extent;
                            num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                        else if (num12 <= segment1.extent) {
                            num6 = num12;
                            num10 = (0f - num6) * num6 + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                        else {
                            num6 = segment1.extent;
                            num10 = num6 * (num6 - 2f * num12) + num5 * (num5 + 2f * num2) + sqrMagnitude;
                        }
                    }
                }
            }
            else {
                FLOAT num13 = segment0.extent + segment1.extent;
                FLOAT num14 = ((num > 0f) ? (-1f) : 1f);
                FLOAT num15 = 0.5f * (num2 - num14 * num3);
                FLOAT num16 = 0f - num15;
                if (num16 < 0f - num13) {
                    num16 = 0f - num13;
                }
                else if (num16 > num13) {
                    num16 = num13;
                }

                num6 = (0f - num14) * num16 * segment1.extent / num13;
                num5 = num16 + num14 * num6;
                num10 = num16 * (num16 + 2f * num15) + sqrMagnitude;
            }

            closestPoint0 = segment0.center + num5 * segment0.direction;
            closestPoint1 = segment1.center + num6 * segment1.direction;
            if (num10 < 0f) {
                num10 = 0f;
            }

            return num10;
        }
    }
}