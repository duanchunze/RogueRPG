using System;
using System.Text;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Represents 3d planar polygon (vertex count must be &gt;= 3).
    /// </summary>
    public class Polygon {
        private FVector3[] _vertices;

        private Edge[] _edges;

        private Plane _plane;

        /// <summary>
        /// Gets vertices array (do not change the data, use only for traversal)
        /// </summary>
        public FVector3[] vertices => this._vertices;

        /// <summary>
        /// Gets edges array (do not change the data, use only for traversal)
        /// </summary>
        public Edge[] edges => this._edges;

        /// <summary>
        /// Polygon vertex count
        /// </summary>
        public int vertexCount => this._vertices.Length;

        /// <summary>
        /// Gets or sets polygon vertex. The caller is responsible for supplying the points which lie in the polygon's plane.
        /// </summary>
        public FVector3 this[int vertexIndex] {
            get { return this._vertices[vertexIndex]; }
            set { this._vertices[vertexIndex] = value; }
        }

        /// <summary>
        /// Gets or sets polygon plane. After plane change reset all vertices manually or call ProjectVertices() to project all vertices automatically.
        /// </summary>
        public Plane Plane {
            get { return this._plane; }
            set { this._plane = value; }
        }

        private Polygon() { }

        /// <summary>
        /// Creates polygon from an array of vertices (array is copied).
        /// The caller is responsible for supplying the points which lie in the polygon's plane.
        /// </summary>
        public Polygon(FVector3[] vertices, Plane plane) {
            this._vertices = new FVector3[vertices.Length];
            this._edges = new Edge[vertices.Length];
            Array.Copy(vertices, this._vertices, vertices.Length);
            this._plane = plane;
            this.UpdateEdges();
        }

        /// <summary>
        /// Creates polygon setting number of vertices. Vertices then
        /// can be filled using indexer.
        /// </summary>
        public Polygon(int vertexCount, Plane plane) {
            this._vertices = new FVector3[vertexCount];
            this._edges = new Edge[vertexCount];
            this._plane = plane;
        }

        /// <summary>
        /// Sets polygon vertex and ensures that it will lie in the plane by projecting it.
        /// </summary>
        public void SetVertexProjected(int vertexIndex, FVector3 vertex) {
            FLOAT num = this._plane.normal.Dot(vertex) - this._plane.constant;
            ref FVector3 reference = ref this._vertices[vertexIndex];
            reference = vertex - num * this._plane.normal;
        }

        /// <summary>
        /// Projects polygon vertices onto polygon plane.
        /// </summary>
        public void ProjectVertices() {
            int i = 0;
            for (int num = this._vertices.Length; i < num; i++) {
                FLOAT num2 = this._plane.normal.Dot(this._vertices[i]) - this._plane.constant;
                this._vertices[i] -= num2 * this._plane.normal;
            }
        }

        /// <summary>
        /// Returns polygon edge
        /// </summary>
        public Edge GetEdge(int edgeIndex) {
            return this._edges[edgeIndex];
        }

        /// <summary>
        /// Updates all polygon edges. Use after vertex change.
        /// </summary>
        public void UpdateEdges() {
            int num = this._vertices.Length;
            int num2 = num - 1;
            for (int i = 0; i < num; i++) {
                FVector3 vector = (this._edges[num2].point1 = this._vertices[i]) - (this._edges[num2].point0 = this._vertices[num2]);
                this._edges[num2].length = FVector3.Normalize(ref vector);
                this._edges[num2].direction = vector;
                this._edges[num2].normal = this._plane.normal.Cross(vector);
                num2 = i;
            }
        }

        /// <summary>
        /// Updates certain polygon edge. Use after vertex change.
        /// </summary>
        public void UpdateEdge(int edgeIndex) {
            FVector3 vector = (this._edges[edgeIndex].point1 = this._vertices[(edgeIndex + 1) % this._vertices.Length]) -
                              (this._edges[edgeIndex].point0 = this._vertices[edgeIndex]);
            this._edges[edgeIndex].length = FVector3.Normalize(ref vector);
            this._edges[edgeIndex].direction = vector;
            this._edges[edgeIndex].normal = this._plane.normal.Cross(vector);
        }

        /// <summary>
        /// Returns polygon center
        /// </summary>
        public FVector3 CalcCenter() {
            FVector3 vector = this._vertices[0];
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
        /// Returns true if polygon contains some edges which have zero angle between them.
        /// </summary>
        public bool HasZeroCorners() {
            FLOAT threshold = 1E-05f;
            int num = this._edges.Length;
            FLOAT num2 = 1f - threshold;
            int num3 = num - 1;
            for (int i = 0; i < num; i++) {
                FVector3 lhs = -this._edges[num3].direction;
                FVector3 direction = this._edges[i].direction;
                FVector3.Dot(ref lhs, ref direction, out var num4);
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
                FVector3 lhs = -this._edges[num3].direction;
                FVector3 direction = this._edges[i].direction;
                FVector3.Dot(ref lhs, ref direction, out var num4);
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
                FVector3 vector = this._vertices[i];
                int num3 = num - i;
                ref FVector3 reference = ref this._vertices[i];
                reference = this._vertices[num3];
                this._vertices[num3] = vector;
            }

            this.UpdateEdges();
        }

        /// <summary>
        /// Converts the polygon to segment array
        /// </summary>
        public Segment[] ToSegmentArray() {
            Segment[] array = new Segment[this._edges.Length];
            int i = 0;
            for (int num = array.Length; i < num; i++) {
                ref Segment reference = ref array[i];
                reference = new Segment(this._edges[i].point0, this._edges[i].point1);
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