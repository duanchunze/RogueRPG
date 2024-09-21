using System;
using System.Text;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Represents 2d polygon (vertex count must be &gt;= 3).
    /// </summary>
    public class Polygon2D {
        private Vector2[] _vertices;

        private Edge2D[] _edges;

        /// <summary>
        /// Gets vertices array (do not change the data, use only for traversal)
        /// </summary>
        public Vector2[] vertices => this._vertices;

        /// <summary>
        /// Gets edges array (do not change the data, use only for traversal)
        /// </summary>
        public Edge2D[] edges => this._edges;

        /// <summary>
        /// Polygon vertex count
        /// </summary>
        public int vertexCount => this._vertices.Length;

        /// <summary>
        /// Gets or sets polygon vertex
        /// </summary>
        public Vector2 this[int vertexIndex] {
            get { return this._vertices[vertexIndex]; }
            set { this._vertices[vertexIndex] = value; }
        }

        private Polygon2D() { }

        /// <summary>
        /// Creates polygon from an array of vertices (array is copied)
        /// </summary>
        public Polygon2D(Vector2[] vertices) {
            this._vertices = new Vector2[vertices.Length];
            this._edges = new Edge2D[vertices.Length];
            Array.Copy(vertices, this._vertices, vertices.Length);
            this.UpdateEdges();
        }

        /// <summary>
        /// Creates polygon setting number of vertices. Vertices then
        /// can be filled using indexer.
        /// </summary>
        public Polygon2D(int vertexCount) {
            this._vertices = new Vector2[vertexCount];
            this._edges = new Edge2D[vertexCount];
        }

        /// <summary>
        /// Creates Polygon2 instance from Polygon3 instance by projecting
        /// Polygon3 vertices onto one of three base planes (on practice just
        /// dropping one of the coordinates).
        /// </summary>
        public static Polygon2D CreateProjected(Polygon polygon, ProjectionPlanes projectionPlane) {
            Polygon2D polygon2D = new Polygon2D(polygon.vertexCount);
            switch (projectionPlane) {
                case ProjectionPlanes.XY: {
                    int j = 0;
                    for (int vertexCount2 = polygon.vertexCount; j < vertexCount2; j++) {
                        ref Vector2 reference2 = ref polygon2D._vertices[j];
                        reference2 = polygon[j].ToFVector2XY();
                    }

                    break;
                }
                case ProjectionPlanes.XZ: {
                    int k = 0;
                    for (int vertexCount3 = polygon.vertexCount; k < vertexCount3; k++) {
                        ref Vector2 reference3 = ref polygon2D._vertices[k];
                        reference3 = polygon[k].ToFVector2XZ();
                    }

                    break;
                }
                default: {
                    int i = 0;
                    for (int vertexCount = polygon.vertexCount; i < vertexCount; i++) {
                        ref Vector2 reference = ref polygon2D._vertices[i];
                        reference = polygon[i].ToFVector2YZ();
                    }

                    break;
                }
            }

            polygon2D.UpdateEdges();
            return polygon2D;
        }

        /// <summary>
        /// Returns polygon edge
        /// </summary>
        public Edge2D GetEdge(int edgeIndex) {
            return this._edges[edgeIndex];
        }

        /// <summary>
        /// Updates all polygon edges. Use after vertex change.
        /// </summary>
        public void UpdateEdges() {
            int num = this._vertices.Length;
            int num2 = num - 1;
            for (int i = 0; i < num; i++) {
                Vector2 vector = (this._edges[num2].point1 = this._vertices[i]) - (this._edges[num2].point0 = this._vertices[num2]);
                this._edges[num2].length = Vector2.Normalize(ref vector);
                this._edges[num2].direction = vector;
                this._edges[num2].normal = vector.Perpendicular();
                num2 = i;
            }
        }

        /// <summary>
        /// Updates certain polygon edge. Use after vertex change.
        /// </summary>
        public void UpdateEdge(int edgeIndex) {
            Vector2 vector = (this._edges[edgeIndex].point1 = this._vertices[(edgeIndex + 1) % this._vertices.Length]) -
                              (this._edges[edgeIndex].point0 = this._vertices[edgeIndex]);
            this._edges[edgeIndex].length = Vector2.Normalize(ref vector);
            this._edges[edgeIndex].direction = vector;
            this._edges[edgeIndex].normal = vector.Perpendicular();
        }

        /// <summary>
        /// Returns polygon center
        /// </summary>
        public Vector2 CalcCenter() {
            Vector2 vector = this._vertices[0];
            int num = this._vertices.Length;
            for (int i = 1; i < num; i++) {
                vector += this._vertices[i];
            }

            return vector / num;
        }

        /// <summary>
        /// Returns polygon perimeter length
        /// </summary>
        public FLOAT CalcPerimeter() {
            FLOAT num = 0f;
            int i = 0;
            for (int num2 = this._edges.Length; i < num2; i++) {
                num += this._edges[i].length;
            }

            return num;
        }

        /// <summary>
        /// Returns polygon area (polygon must be simple, i.e. without self-intersections).
        /// </summary>
        public FLOAT CalcArea() {
            int num = this._vertices.Length - 1;
            FLOAT num2 = this._vertices[0][0] * (this._vertices[1][1] - this._vertices[num][1]) +
                         this._vertices[num][0] * (this._vertices[0][1] - this._vertices[num - 1][1]);
            int num3 = 0;
            int num4 = 1;
            int num5 = 2;
            while (num4 < num) {
                num2 += this._vertices[num4][0] * (this._vertices[num5][1] - this._vertices[num3][1]);
                num3++;
                num4++;
                num5++;
            }

            num2 *= 0.5f;
            return Math.Abs(num2);
        }

        /// <summary>
        /// Tests if the polygon is convex and returns orientation
        /// </summary>
        public bool IsConvex(out Orientations orientation) {
            FLOAT threshold = 1E-05f;
            orientation = Orientations.None;
            int num = this._edges.Length;
            int num2 = 0;
            int num3 = num - 1;
            for (int i = 0; i < num; i++) {
                Vector2 vector = -this._edges[num3].direction;
                Vector2 direction = this._edges[i].direction;
                FLOAT num4 = vector.DotPerpendicular(direction);
                int num5 = ((num4 < 0f - threshold || num4 > threshold) ? ((num4 > 0f) ? 1 : (-1)) : 0);
                if (num5 != 0) {
                    if (num2 != 0) {
                        if ((num2 > 0 && num5 < 0) || (num2 < 0 && num5 > 0)) {
                            return false;
                        }
                    }
                    else {
                        num2 += num5;
                    }
                }

                num3 = i;
            }

            orientation = ((num2 == 0) ? Orientations.None : ((num2 <= 0) ? Orientations.CCW : Orientations.CW));
            return orientation != Orientations.None;
        }

        /// <summary>
        /// Tests if the polygon is convex and returns orientation
        /// </summary>
        public bool IsConvex(out Orientations orientation, FLOAT threshold) {
            orientation = Orientations.None;
            int num = this._edges.Length;
            int num2 = 0;
            int num3 = num - 1;
            for (int i = 0; i < num; i++) {
                Vector2 vector = -this._edges[num3].direction;
                Vector2 direction = this._edges[i].direction;
                FLOAT num4 = vector.DotPerpendicular(direction);
                int num5 = ((num4 < 0f - threshold || num4 > threshold) ? ((num4 > 0f) ? 1 : (-1)) : 0);
                if (num5 != 0) {
                    if (num2 != 0) {
                        if ((num2 > 0 && num5 < 0) || (num2 < 0 && num5 > 0)) {
                            return false;
                        }
                    }
                    else {
                        num2 += num5;
                    }
                }

                num3 = i;
            }

            orientation = ((num2 == 0) ? Orientations.None : ((num2 <= 0) ? Orientations.CCW : Orientations.CW));
            return orientation != Orientations.None;
        }

        /// <summary>
        /// Tests if the polygon is convex
        /// </summary>
        public bool IsConvex() {
            // FLOAT threshold = 1E-05f;
            // Orientations orientation;
            return this.IsConvex(out var orientation);
        }

        /// <summary>
        /// Tests if the polygon is convex
        /// </summary>
        public bool IsConvex(FLOAT threshold) {
            // Orientations orientation;
            return this.IsConvex(out var orientation);
        }

        /// <summary>
        /// Returns true if polygon contains some edges which have zero angle between them.
        /// </summary>
        public bool HasZeroCorners() {
            FLOAT threshold = 1E-05f;
            int num = this._edges.Length;
            FLOAT num2 = 1f - threshold;
            int num3 = num - 1;
            for (int i = 0; i < num; i++) {
                Vector2 lhs = -this._edges[num3].direction;
                Vector2 direction = this._edges[i].direction;
                Vector2.Dot(ref lhs, ref direction, out var num4);
                if (num4 >= num2) {
                    return true;
                }

                num3 = i;
            }

            return false;
        }

        /// <summary>
        /// Returns true if polygon contains some edges which have zero angle between them.
        /// </summary>
        public bool HasZeroCorners(FLOAT threshold) {
            int num = this._edges.Length;
            FLOAT num2 = 1f - threshold;
            int num3 = num - 1;
            for (int i = 0; i < num; i++) {
                Vector2 lhs = -this._edges[num3].direction;
                Vector2 direction = this._edges[i].direction;
                Vector2.Dot(ref lhs, ref direction, out var num4);
                if (num4 >= num2) {
                    return true;
                }

                num3 = i;
            }

            return false;
        }

        /// <summary>
        /// Reverses polygon vertex order
        /// </summary>
        public void ReverseVertices() {
            int num = this._vertices.Length;
            int num2 = num / 2;
            num--;
            for (int i = 0; i < num2; i++) {
                Vector2 vector = this._vertices[i];
                int num3 = num - i;
                ref Vector2 reference = ref this._vertices[i];
                reference = this._vertices[num3];
                this._vertices[num3] = vector;
            }

            this.UpdateEdges();
        }

        /// <summary>
        /// Tests whether a point is contained by the convex CCW 4-sided polygon (the caller must ensure that polygon is indeed CCW ordered)
        /// </summary>
        public bool ContainsConvexQuadCCW(ref Vector2 point) {
            if (this._vertices.Length != 4) {
                return false;
            }

            FLOAT num = this._vertices[2].y - this._vertices[0].y;
            FLOAT num2 = this._vertices[0].x - this._vertices[2].x;
            FLOAT num3 = point.x - this._vertices[0].x;
            FLOAT num4 = point.y - this._vertices[0].y;
            if (num * num3 + num2 * num4 > 0f) {
                num = this._vertices[1].y - this._vertices[0].y;
                num2 = this._vertices[0].x - this._vertices[1].x;
                if (num * num3 + num2 * num4 > 0f) {
                    return false;
                }

                num = this._vertices[2].y - this._vertices[1].y;
                num2 = this._vertices[1].x - this._vertices[2].x;
                num3 = point.x - this._vertices[1].x;
                num4 = point.y - this._vertices[1].y;
                if (num * num3 + num2 * num4 > 0f) {
                    return false;
                }
            }
            else {
                num = this._vertices[0].y - this._vertices[3].y;
                num2 = this._vertices[3].x - this._vertices[0].x;
                if (num * num3 + num2 * num4 > 0f) {
                    return false;
                }

                num = this._vertices[3].y - this._vertices[2].y;
                num2 = this._vertices[2].x - this._vertices[3].x;
                num3 = point.x - this._vertices[3].x;
                num4 = point.y - this._vertices[3].y;
                if (num * num3 + num2 * num4 > 0f) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tests whether a point is contained by the convex CCW 4-sided polygon (the caller must ensure that polygon is indeed CCW ordered)
        /// </summary>
        public bool ContainsConvexQuadCCW(Vector2 point) {
            return this.ContainsConvexQuadCCW(ref point);
        }

        /// <summary>
        /// Tests whether a point is contained by the convex CW 4-sided polygon (the caller must ensure that polygon is indeed CW ordered)
        /// </summary>
        public bool ContainsConvexQuadCW(ref Vector2 point) {
            if (this._vertices.Length != 4) {
                return false;
            }

            FLOAT num = this._vertices[2].y - this._vertices[0].y;
            FLOAT num2 = this._vertices[0].x - this._vertices[2].x;
            FLOAT num3 = point.x - this._vertices[0].x;
            FLOAT num4 = point.y - this._vertices[0].y;
            if (num * num3 + num2 * num4 < 0f) {
                num = this._vertices[1].y - this._vertices[0].y;
                num2 = this._vertices[0].x - this._vertices[1].x;
                if (num * num3 + num2 * num4 < 0f) {
                    return false;
                }

                num = this._vertices[2].y - this._vertices[1].y;
                num2 = this._vertices[1].x - this._vertices[2].x;
                num3 = point.x - this._vertices[1].x;
                num4 = point.y - this._vertices[1].y;
                if (num * num3 + num2 * num4 < 0f) {
                    return false;
                }
            }
            else {
                num = this._vertices[0].y - this._vertices[3].y;
                num2 = this._vertices[3].x - this._vertices[0].x;
                if (num * num3 + num2 * num4 < 0f) {
                    return false;
                }

                num = this._vertices[3].y - this._vertices[2].y;
                num2 = this._vertices[2].x - this._vertices[3].x;
                num3 = point.x - this._vertices[3].x;
                num4 = point.y - this._vertices[3].y;
                if (num * num3 + num2 * num4 < 0f) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tests whether a point is contained by the convex CW 4-sided polygon (the caller must ensure that polygon is indeed CW ordered)
        /// </summary>
        public bool ContainsConvexQuadCW(Vector2 point) {
            return this.ContainsConvexQuadCW(ref point);
        }

        /// <summary>
        /// Tests whether a point is contained by the convex CCW polygon (the caller must ensure that polygon is indeed CCW ordered)
        /// </summary>
        public bool ContainsConvexCCW(ref Vector2 point) {
            return this.SubContainsPointCCW(ref point, 0, 0);
        }

        /// <summary>
        /// Tests whether a point is contained by the convex CCW polygon (the caller must ensure that polygon is indeed CCW ordered)
        /// </summary>
        public bool ContainsConvexCCW(Vector2 point) {
            return this.ContainsConvexCCW(ref point);
        }

        private bool SubContainsPointCCW(ref Vector2 p, int i0, int i1) {
            int num = this._vertices.Length;
            int num2 = i1 - i0;
            FLOAT num3;
            FLOAT num4;
            FLOAT num5;
            FLOAT num6;
            if (num2 == 1 || (num2 < 0 && num2 + num == 1)) {
                num3 = this._vertices[i1].y - this._vertices[i0].y;
                num4 = this._vertices[i0].x - this._vertices[i1].x;
                num5 = p.x - this._vertices[i0].x;
                num6 = p.y - this._vertices[i0].y;
                return num3 * num5 + num4 * num6 <= 0f;
            }

            int num7;
            if (i0 < i1) {
                num7 = i0 + i1 >> 1;
            }
            else {
                num7 = i0 + i1 + num >> 1;
                if (num7 >= num) {
                    num7 -= num;
                }
            }

            num3 = this._vertices[num7].y - this._vertices[i0].y;
            num4 = this._vertices[i0].x - this._vertices[num7].x;
            num5 = p.x - this._vertices[i0].x;
            num6 = p.y - this._vertices[i0].y;
            if (num3 * num5 + num4 * num6 > 0f) {
                return this.SubContainsPointCCW(ref p, i0, num7);
            }

            return this.SubContainsPointCCW(ref p, num7, i1);
        }

        /// <summary>
        /// Tests whether a point is contained by the convex CW polygon (the caller must ensure that polygon is indeed CW ordered)
        /// </summary>
        public bool ContainsConvexCW(ref Vector2 point) {
            return this.SubContainsPointCW(ref point, 0, 0);
        }

        /// <summary>
        /// Tests whether a point is contained by the convex CW polygon (the caller must ensure that polygon is indeed CW ordered)
        /// </summary>
        public bool ContainsConvexCW(Vector2 point) {
            return this.ContainsConvexCW(ref point);
        }

        private bool SubContainsPointCW(ref Vector2 p, int i0, int i1) {
            int num = this._vertices.Length;
            int num2 = i1 - i0;
            FLOAT num3;
            FLOAT num4;
            FLOAT num5;
            FLOAT num6;
            if (num2 == 1 || (num2 < 0 && num2 + num == 1)) {
                num3 = this._vertices[i1].y - this._vertices[i0].y;
                num4 = this._vertices[i0].x - this._vertices[i1].x;
                num5 = p.x - this._vertices[i0].x;
                num6 = p.y - this._vertices[i0].y;
                return num3 * num5 + num4 * num6 >= 0f;
            }

            int num7;
            if (i0 < i1) {
                num7 = i0 + i1 >> 1;
            }
            else {
                num7 = i0 + i1 + num >> 1;
                if (num7 >= num) {
                    num7 -= num;
                }
            }

            num3 = this._vertices[num7].y - this._vertices[i0].y;
            num4 = this._vertices[i0].x - this._vertices[num7].x;
            num5 = p.x - this._vertices[i0].x;
            num6 = p.y - this._vertices[i0].y;
            if (num3 * num5 + num4 * num6 < 0f) {
                return this.SubContainsPointCW(ref p, i0, num7);
            }

            return this.SubContainsPointCW(ref p, num7, i1);
        }

        /// <summary>
        /// Tests whether a point is contained by the simple polygon (i.e. without self intersection). Non-convex polygons are allowed, orientation is irrelevant.
        /// Note that points which are on border may be classified differently depending on the point position.
        /// </summary>
        public bool ContainsSimple(ref Vector2 point) {
            bool flag = false;
            int num = this._vertices.Length;
            int i = 0;
            int num2 = num - 1;
            for (; i < num; i++) {
                Vector2 vector = this._vertices[i];
                Vector2 vector2 = this._vertices[num2];
                if (point.y < vector2.y) {
                    if (vector.y <= point.y) {
                        FLOAT num3 = (point.y - vector.y) * (vector2.x - vector.x);
                        FLOAT num4 = (point.x - vector.x) * (vector2.y - vector.y);
                        if (num3 > num4) {
                            flag = !flag;
                        }
                    }
                }
                else if (point.y < vector.y) {
                    FLOAT num3 = (point.y - vector.y) * (vector2.x - vector.x);
                    FLOAT num4 = (point.x - vector.x) * (vector2.y - vector.y);
                    if (num3 < num4) {
                        flag = !flag;
                    }
                }

                num2 = i;
            }

            return flag;
        }

        /// <summary>
        /// Tests whether a point is contained by the simple polygon (i.e. without self intersection). Non-convex polygons are allowed, orientation is irrelevant.
        /// Note that points which are on border may be classified differently depending on the point position.
        /// </summary>
        public bool ContainsSimple(Vector2 point) {
            return this.ContainsSimple(ref point);
        }

        /// <summary>
        /// Converts the polygon to segment array
        /// </summary>
        public Segment2D[] ToSegmentArray() {
            Segment2D[] array = new Segment2D[this._edges.Length];
            int i = 0;
            for (int num = array.Length; i < num; i++) {
                ref Segment2D reference = ref array[i];
                reference = new Segment2D(this._edges[i].point0, this._edges[i].point1);
            }

            return array;
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[VertexCount: " + this._vertices.Length);
            int i = 0;
            for (int num = this._vertices.Length; i < num; i++) {
                stringBuilder.Append($" V{i.ToString()}: {this._vertices[i].ToString()}");
            }

            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
    }
}