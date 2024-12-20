﻿#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Points are R(s,t) = C+s0*U0+s1*U1, where C is the center of the
    /// rectangle, U0 and U1 are unit-length and perpendicular axes.  The
    /// parameters s0 and s1 are constrained by |s0| &lt;= e0 and |s1| &lt;= e1,
    /// where e0 &gt; 0 and e1 &gt; 0 are called the extents of the rectangle.
    /// </summary>
    public struct Rectangle {
        /// <summary>
        /// Rectangle center
        /// </summary>
        public Vector3 center;

        /// <summary>
        /// First rectangle axis. Must be unit length!
        /// </summary>
        public Vector3 axis0;

        /// <summary>
        /// Second rectangle axis. Must be unit length!
        /// </summary>
        public Vector3 axis1;

        /// <summary>
        /// Rectangle normal which is Cross(Axis0, Axis1). Must be unit length!
        /// </summary>
        public Vector3 normal;

        /// <summary>
        /// Extents (half sizes) along Axis0 and Axis1. Must be non-negative!
        /// </summary>
        public Vector2 extents;

        /// <summary>
        /// Creates new Rectangle3 instance.
        /// </summary>
        /// <param name="center">Rectangle center</param>
        /// <param name="axis0">First box axis. Must be unit length!</param>
        /// <param name="axis1">Second box axis. Must be unit length!</param>
        /// <param name="extents">Extents (half sizes) along Axis0 and Axis1. Must be non-negative!</param>
        public Rectangle(ref Vector3 center, ref Vector3 axis0, ref Vector3 axis1, ref Vector2 extents) {
            this.center = center;
            this.axis0 = axis0;
            this.axis1 = axis1;
            this.normal = axis0.Cross(axis1);
            this.extents = extents;
        }

        /// <summary>
        /// Creates new Rectangle3 instance.
        /// </summary>
        /// <param name="center">Rectangle center</param>
        /// <param name="axis0">First rectangle axis. Must be unit length!</param>
        /// <param name="axis1">Second rectangle axis. Must be unit length!</param>
        /// <param name="extents">Extents (half sizes) along Axis0 and Axis1. Must be non-negative!</param>
        public Rectangle(Vector3 center, Vector3 axis0, Vector3 axis1, Vector2 extents) {
            this.center = center;
            this.axis0 = axis0;
            this.axis1 = axis1;
            this.normal = axis0.Cross(axis1);
            this.extents = extents;
        }

        /// <summary>
        /// Creates rectangle from 4 counter clockwise ordered ordered points. Center=(p0+p2)/2, Axis0=Normalized(p1-p0), Axis1=Normalized(p2-p1).
        /// The user therefore must ensure that the points are indeed represent rectangle to obtain meaningful result.
        /// </summary>
        public static Rectangle CreateFromCCWPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            Vector3 vector = p1 - p0;
            Vector3 vector2 = p2 - p1;
            Rectangle result = default(Rectangle);
            result.center = (p0 + p2) * 0.5f;
            result.extents.x = Vector3.Normalize(ref vector) * 0.5f;
            result.extents.y = Vector3.Normalize(ref vector2) * 0.5f;
            result.axis0 = vector;
            result.axis1 = vector2;
            result.normal = vector.Cross(vector2);
            return result;
        }

        /// <summary>
        /// Creates rectangle from 4 clockwise ordered points. Center=(p0+p2)/2, Axis0=Normalized(p2-p1), Axis1=Normalized(p1-p0).
        /// The user therefore must ensure that the points are indeed represent rectangle to obtain meaningful result.
        /// </summary>
        public static Rectangle CreateFromCWPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            Vector3 vector = p2 - p1;
            Vector3 vector2 = p1 - p0;
            Rectangle result = default(Rectangle);
            result.center = (p0 + p2) * 0.5f;
            result.extents.x = Vector3.Normalize(ref vector) * 0.5f;
            result.extents.y = Vector3.Normalize(ref vector2) * 0.5f;
            result.axis0 = vector;
            result.axis1 = vector2;
            result.normal = vector.Cross(vector2);
            return result;
        }

        /// <summary>
        /// Calculates 4 box corners. extAxis[i] is Axis[i]*Extent[i], i=0,1.
        /// </summary>
        /// <param name="vertex0">Center - extAxis0 - extAxis1</param>
        /// <param name="vertex1">Center + extAxis0 - extAxis1</param>
        /// <param name="vertex2">Center + extAxis0 + extAxis1</param>
        /// <param name="vertex3">Center - extAxis0 + extAxis1</param>
        public void CalcVertices(out Vector3 vertex0, out Vector3 vertex1, out Vector3 vertex2,
            out Vector3 vertex3) {
            Vector3 vector = this.axis0 * this.extents.x;
            Vector3 vector2 = this.axis1 * this.extents.y;
            vertex0 = this.center - vector - vector2;
            vertex1 = this.center + vector - vector2;
            vertex2 = this.center + vector + vector2;
            vertex3 = this.center - vector + vector2;
        }

        /// <summary>
        /// Calculates 4 box corners and returns them in an allocated array.
        /// Look array-less method for the description.
        /// </summary>
        public Vector3[] CalcVertices() {
            Vector3 vector = this.axis0 * this.extents.x;
            Vector3 vector2 = this.axis1 * this.extents.y;
            return new Vector3[4]
                { this.center - vector - vector2, this.center + vector - vector2, this.center + vector + vector2, this.center - vector + vector2 };
        }

        /// <summary>
        /// Calculates 4 box corners and fills the input array with them (array length must be 4).
        /// Look array-less method for the description.
        /// </summary>
        public void CalcVertices(Vector3[] array) {
            Vector3 vector = this.axis0 * this.extents.x;
            Vector3 vector2 = this.axis1 * this.extents.y;
            ref Vector3 reference = ref array[0];
            reference = this.center - vector - vector2;
            ref Vector3 reference2 = ref array[1];
            reference2 = this.center + vector - vector2;
            ref Vector3 reference3 = ref array[2];
            reference3 = this.center + vector + vector2;
            ref Vector3 reference4 = ref array[3];
            reference4 = this.center - vector + vector2;
        }

        /// <summary>
        /// Returns area of the box as Extent.x*Extent.y*4
        /// </summary>
        public FLOAT CalcArea() {
            return 4f * this.extents.x * this.extents.y;
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(Vector3 point) {
            return Distance.Point3Rectangle3(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public Vector3 Project(Vector3 point) {
            Distance.SqrPoint3Rectangle3(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return
                $"[Center: {this.center.ToString()}  Axis0: {this.axis0.ToString()} Axis1: {this.axis1.ToString()} Normal: {this.normal.ToString()} Extents: {this.extents.ToString()}]";
        }
    }
}