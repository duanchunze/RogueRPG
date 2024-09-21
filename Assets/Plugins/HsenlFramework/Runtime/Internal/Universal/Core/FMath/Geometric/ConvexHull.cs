using System;
using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// 凸包
    /// </summary>
    public static class ConvexHull {
        /// <summary>
        /// Generates 2D convex hull of the input point set. Resulting convex hull is defined by the indices parameter. Its behavior depends on the dimension parameter.
        /// If dimension is 2, then convex hull is 2D polygon and indices should be accessed as Edge=(points[indices[i]], points[indices[(i+1)%indices.Length]), for i=[0,indices.Length-1].
        /// If dimension is 1, then input point set lie on the line and covex hull is a segment, use (points[indices[0]], points[indices[1]) to access the segment.
        /// If dimension is 0, then all points in the input set are practically the same.
        /// </summary>
        /// <param name="points">Input point set whose convex hull should be calculated.</param>
        /// <param name="indices">Contains indices into point set (null if construction has failed).</param>
        /// <param name="dimension">Resulting dimension of the input set: 2, 1 or 0.</param>
        /// <param name="epsilon">Small positive number used to determine dimension of the input set.</param>
        /// <returns>True if convex hull is created, false otherwise (in case if input point set is null, contains no points or if some error has occured during construction)</returns>
        public static bool Create2D(IList<Vector2> points, out int[] indices, out int dimension, FLOAT epsilon) {
            if (points == null || points.Count == 0) {
                indices = null;
                dimension = -1;
                return false;
            }

            epsilon = ((epsilon >= 0f) ? epsilon : 0f);
            return ConvexHull2.Create(points, epsilon, out dimension, out indices);
        }

        /// <summary>
        /// Generates 3D convex hull of the input point set. Resulting convex hull is defined fo the indices parameter. Its behavior depends on the dimension parameter.
        /// If dimension is 3, then convex hull is 3D polyhedron and indices define triangles, Triangle=(points[indices[i]], points[indices[i+1]], points[indices[i+2]]), for i=[0,indices.Length-1], i+=3.
        /// If dimension is 2, then convex hull is 2D polygon and indices should be accessed as Edge=(points[indices[i]], points[indices[(i+1)%indices.Length]), for i=[0,indices.Length-1].
        /// If dimension is 1, then input point set lie on the line and covex hull is a segment, use (points[indices[0]], points[indices[1]) to access the segment.
        /// If dimension is 0, then all points in the input set are practically the same.
        /// </summary>
        /// <param name="points">Input point set whose convex hull should be calculated.</param>
        /// <param name="indices">Contains indices into point set (null if construction has failed).</param>
        /// <param name="dimension">Resulting dimension of the input set: 3, 2, 1 or 0.</param>
        /// <param name="epsilon">Small positive number used to determine dimension of the input set.</param>
        /// <returns>True if convex hull is created, false otherwise (in case if input point set is null, contains no points or if some error has occured during construction)</returns>
        public static bool Create3D(IList<Vector3> points, out int[] indices, out int dimension, float epsilon = 1E-05f) {
            if (points == null || points.Count == 0) {
                indices = null;
                dimension = -1;
                return false;
            }

            epsilon = ((epsilon >= 0f) ? epsilon : 0f);
            return ConvexHull3.Create(points, epsilon, out dimension, out indices);
        }
    }

    internal class ConvexHull1 {
        private class SortedVertex {
            public FLOAT Value;

            public int Index;
        }

        public static void Create(FLOAT[] vertices, FLOAT epsilon, out int dimension, out int[] indices) {
            int num = vertices.Length;
            SortedVertex[] array = new SortedVertex[num];
            for (int i = 0; i < num; i++) {
                array[i] = new SortedVertex { Value = vertices[i], Index = i };
            }

            Array.Sort(array,
                (SortedVertex e1, SortedVertex e2) => Comparer<FLOAT>.Default.Compare(e1.Value, e2.Value));
            FLOAT num2 = array[num - 1].Value - array[0].Value;
            if (num2 >= epsilon) {
                dimension = 1;
                indices = new int[2] { array[0].Index, array[num - 1].Index };
            }
            else {
                dimension = 0;
                int[] array2 = (indices = new int[1]);
            }
        }
    }

    internal class ConvexHull2 {
        private class Edge {
            public int V0;

            public int V1;

            public Edge E0;

            public Edge E1;

            public int Sign;

            public int Time;

            public Edge(int v0, int v1) {
                this.V0 = v0;
                this.V1 = v1;
                this.Time = -1;
            }

            public int GetSign(int i, Query2 query) {
                if (i != this.Time) {
                    this.Time = i;
                    this.Sign = query.ToLine(i, this.V0, this.V1);
                }

                return this.Sign;
            }

            public void Insert(Edge adj0, Edge adj1) {
                adj0.E1 = this;
                adj1.E0 = this;
                this.E0 = adj0;
                this.E1 = adj1;
            }

            public void DeleteSelf() {
                if (this.E0 != null) {
                    this.E0.E1 = null;
                }

                if (this.E1 != null) {
                    this.E1.E0 = null;
                }
            }

            public void GetIndices(out int[] indices) {
                int num = 0;
                Edge edge = this;
                do {
                    num++;
                    edge = edge.E1;
                } while (edge != this);

                indices = new int[num];
                num = 0;
                edge = this;
                do {
                    indices[num] = edge.V0;
                    num++;
                    edge = edge.E1;
                } while (edge != this);
            }
        }

        public static bool Create(IList<Vector2> vertices, FLOAT epsilon, out int dimension, out int[] indices) {
            FVector2Ex.Information information = FVector2Ex.GetInformation(vertices, epsilon);
            if (information == null) {
                dimension = -1;
                indices = null;
                return false;
            }

            int count = vertices.Count;
            if (information.Dimension == 0) {
                dimension = 0;
                int[] array = (indices = new int[1]);
                return true;
            }

            if (information.Dimension == 1) {
                FLOAT[] array2 = new FLOAT[count];
                Vector2 origin = information.Origin;
                Vector2 vector = information.Direction[0];
                for (int i = 0; i < count; i++) {
                    Vector2 value = vertices[i] - origin;
                    array2[i] = vector.Dot(value);
                }

                ConvexHull1.Create(array2, epsilon, out dimension, out indices);
                return true;
            }

            dimension = 2;
            Vector2[] array3 = new Vector2[count];
            Vector2 min = information.Min;
            FLOAT num = 1f / information.MaxRange;
            for (int j = 0; j < count; j++) {
                ref Vector2 reference = ref array3[j];
                reference = (vertices[j] - min) * num;
            }

            Query2 query = new Query2(array3);
            int num2 = information.Extreme[0];
            int num3 = information.Extreme[1];
            int num4 = information.Extreme[2];
            Edge edge;
            Edge edge2;
            Edge edge3;
            if (information.ExtremeCCW) {
                edge = new Edge(num2, num3);
                edge2 = new Edge(num3, num4);
                edge3 = new Edge(num4, num2);
            }
            else {
                edge = new Edge(num2, num4);
                edge2 = new Edge(num4, num3);
                edge3 = new Edge(num3, num2);
            }

            edge.Insert(edge3, edge2);
            edge2.Insert(edge, edge3);
            edge3.Insert(edge2, edge);
            Edge hull = edge;
            for (int k = 0; k < count; k++) {
                if (!Update(ref hull, k, query)) {
                    dimension = -1;
                    indices = null;
                    return false;
                }
            }

            hull.GetIndices(out indices);
            return true;
        }

        private static bool Update(ref Edge hull, int i, Query2 query) {
            Edge edge = null;
            Edge edge2 = hull;
            do {
                if (edge2.GetSign(i, query) > 0) {
                    edge = edge2;
                    break;
                }

                edge2 = edge2.E1;
            } while (edge2 != hull);

            if (edge == null) {
                return true;
            }

            Edge e = edge.E0;
            if (e == null) {
                throw new Exception("Expecting nonnull adjacent");
            }

            Edge e2 = edge.E1;
            if (e2 == null) {
                throw new Exception("Expecting nonnull adjacent");
            }

            edge.DeleteSelf();
            while (e.GetSign(i, query) > 0) {
                hull = e;
                e = e.E0;
                if (e == null) {
                    throw new Exception("Expecting nonnull adjacent");
                }

                e.E1.DeleteSelf();
            }

            while (e2.GetSign(i, query) > 0) {
                hull = e2;
                e2 = e2.E1;
                if (e2 == null) {
                    throw new Exception("Expecting nonnull adjacent");
                }

                e2.E0.DeleteSelf();
            }

            Edge edge3 = new Edge(e.V1, i);
            Edge edge4 = new Edge(i, e2.V0);
            edge3.Insert(e, edge4);
            edge4.Insert(edge3, e2);
            hull = edge3;
            return true;
        }
    }

    internal class ConvexHull3 {
        private class Triangle {
            public int V0;

            public int V1;

            public int V2;

            public Triangle Adj0;

            public Triangle Adj1;

            public Triangle Adj2;

            public int Sign;

            public int Time;

            public bool OnStack;

            public Triangle(int v0, int v1, int v2) {
                this.V0 = v0;
                this.V1 = v1;
                this.V2 = v2;
                this.Time = -1;
            }

            public Triangle GetAdj(int index) {
                return index switch {
                    0 => this.Adj0,
                    1 => this.Adj1,
                    _ => this.Adj2,
                };
            }

            public void SetAdj(int index, Triangle value) {
                switch (index) {
                    case 0:
                        this.Adj0 = value;
                        break;
                    case 1:
                        this.Adj1 = value;
                        break;
                    default:
                        this.Adj2 = value;
                        break;
                }
            }

            public int GetV(int index) {
                return index switch {
                    0 => this.V0,
                    1 => this.V1,
                    _ => this.V2,
                };
            }

            public int GetSign(int i, Query3 query) {
                if (i != this.Time) {
                    this.Time = i;
                    this.Sign = query.ToPlane(i, this.V0, this.V1, this.V2);
                }

                return this.Sign;
            }

            public void AttachTo(Triangle adj0, Triangle adj1, Triangle adj2) {
                this.Adj0 = adj0;
                this.Adj1 = adj1;
                this.Adj2 = adj2;
            }

            public int DetachFrom(int adjIndex, Triangle adj) {
                switch (adjIndex) {
                    case 0:
                        this.Adj0 = null;
                        break;
                    case 1:
                        this.Adj1 = null;
                        break;
                    default:
                        this.Adj2 = null;
                        break;
                }

                if (adj.Adj0 == this) {
                    adj.Adj0 = null;
                    return 0;
                }

                if (adj.Adj1 == this) {
                    adj.Adj1 = null;
                    return 1;
                }

                if (adj.Adj2 == this) {
                    adj.Adj2 = null;
                    return 2;
                }

                return -1;
            }
        }

        private class TerminatorData {
            public int V0;

            public int V1;

            public int NullIndex;

            public Triangle T;

            public TerminatorData(int v0 = -1, int v1 = -1, int nullIndex = -1, Triangle tri = null) {
                this.NullIndex = nullIndex;
                this.T = tri;
                this.V0 = v0;
                this.V1 = v1;
            }
        }

        public static bool Create(IList<Vector3> vertices, FLOAT epsilon, out int dimension, out int[] indices) {
            FVector3Ex.Information information = FVector3Ex.GetInformation(vertices, epsilon);

            if (information == null) {
                dimension = -1;
                indices = null;
                return false;
            }

            int count = vertices.Count;
            if (information.Dimension == 0) {
                dimension = 0;
                int[] array = (indices = new int[1]);
                return true;
            }

            if (information.Dimension == 1) {
                FLOAT[] array2 = new FLOAT[count];
                Vector3 origin = information.Origin;
                Vector3 vector = information.Direction[0];
                for (int i = 0; i < count; i++) {
                    Vector3 value = vertices[i] - origin;
                    array2[i] = vector.Dot(value);
                }

                ConvexHull1.Create(array2, epsilon, out dimension, out indices);
                return true;
            }

            if (information.Dimension == 2) {
                Vector2[] array3 = new Vector2[count];
                Vector3 origin2 = information.Origin;
                Vector3 vector2 = information.Direction[0];
                Vector3 vector3 = information.Direction[1];
                for (int j = 0; j < count; j++) {
                    Vector3 value2 = vertices[j] - origin2;
                    ref Vector2 reference = ref array3[j];
                    reference = new Vector2(vector2.Dot(value2), vector3.Dot(value2));
                }

                return ConvexHull2.Create(array3, epsilon, out dimension, out indices);
            }

            dimension = 3;
            Vector3[] array4 = new Vector3[count];
            Vector3 min = information.Min;
            FLOAT num = 1f / information.MaxRange;
            for (int k = 0; k < count; k++) {
                ref Vector3 reference2 = ref array4[k];
                reference2 = (vertices[k] - min) * num;
            }

            Query3 query = new Query3(array4);
            int v = information.Extreme[0];
            int num2 = information.Extreme[1];
            int num3 = information.Extreme[2];
            int num4 = information.Extreme[3];
            Triangle triangle;
            Triangle triangle2;
            Triangle triangle3;
            Triangle triangle4;
            if (information.ExtremeCCW) {
                triangle = new Triangle(v, num2, num4);
                triangle2 = new Triangle(v, num3, num2);
                triangle3 = new Triangle(v, num4, num3);
                triangle4 = new Triangle(num2, num3, num4);
                triangle.AttachTo(triangle2, triangle4, triangle3);
                triangle2.AttachTo(triangle3, triangle4, triangle);
                triangle3.AttachTo(triangle, triangle4, triangle2);
                triangle4.AttachTo(triangle2, triangle3, triangle);
            }
            else {
                triangle = new Triangle(v, num4, num2);
                triangle2 = new Triangle(v, num2, num3);
                triangle3 = new Triangle(v, num3, num4);
                triangle4 = new Triangle(num2, num4, num3);
                triangle.AttachTo(triangle3, triangle4, triangle2);
                triangle2.AttachTo(triangle, triangle4, triangle3);
                triangle3.AttachTo(triangle2, triangle4, triangle);
                triangle4.AttachTo(triangle, triangle3, triangle2);
            }

            HashSet<Triangle> hashSet = new HashSet<Triangle>();
            hashSet.Add(triangle);
            hashSet.Add(triangle2);
            hashSet.Add(triangle3);
            hashSet.Add(triangle4);
            for (int l = 0; l < count; l++) {
                if (!Update(hashSet, l, query)) {
                    dimension = -1;
                    indices = null;
                    return false;
                }
            }

            ExtractIndices(hashSet, out indices);
            return true;
        }

        private static bool Update(HashSet<Triangle> hull, int i, Query3 query) {
            Triangle triangle = null;
            foreach (Triangle item in hull) {
                if (item.GetSign(i, query) > 0) {
                    triangle = item;
                    break;
                }
            }

            if (triangle == null) {
                return true;
            }

            Stack<Triangle> stack = new Stack<Triangle>();
            stack.Push(triangle);
            triangle.OnStack = true;
            Dictionary<int, TerminatorData> dictionary = new Dictionary<int, TerminatorData>();
            int v2;
            Triangle triangle2;
            int v;
            while (stack.Count != 0) {
                triangle2 = stack.Pop();
                triangle2.OnStack = false;
                for (int j = 0; j < 3; j++) {
                    Triangle adj = triangle2.GetAdj(j);
                    if (adj == null) {
                        continue;
                    }

                    int nullIndex = triangle2.DetachFrom(j, adj);
                    if (adj.GetSign(i, query) > 0) {
                        if (!adj.OnStack) {
                            stack.Push(adj);
                            adj.OnStack = true;
                        }
                    }
                    else {
                        v = triangle2.GetV(j);
                        v2 = triangle2.GetV((j + 1) % 3);
                        dictionary[v] = new TerminatorData(v, v2, nullIndex, adj);
                    }
                }

                hull.Remove(triangle2);
            }

            int count = dictionary.Count;
            if (count < 3) {
                throw new Exception("Terminator must be at least a triangle");
            }

            Dictionary<int, TerminatorData>.Enumerator enumerator2 = dictionary.GetEnumerator();
            enumerator2.MoveNext();
            KeyValuePair<int, TerminatorData> current2 = enumerator2.Current;
            v = current2.Value.V0;
            v2 = current2.Value.V1;
            triangle2 = new Triangle(i, v, v2);
            hull.Add(triangle2);
            int v3 = current2.Value.V0;
            Triangle triangle3 = triangle2;
            triangle2.Adj1 = current2.Value.T;
            current2.Value.T.SetAdj(current2.Value.NullIndex, triangle2);
            for (int j = 1; j < count; j++) {
                if (!dictionary.TryGetValue(v2, out var value)) {
                    throw new Exception("Unexpected condition");
                }

                v = v2;
                v2 = value.V1;
                Triangle triangle4 = new Triangle(i, v, v2);
                hull.Add(triangle4);
                triangle4.Adj1 = value.T;
                value.T.SetAdj(value.NullIndex, triangle4);
                triangle4.Adj0 = triangle2;
                triangle2.Adj2 = triangle4;
                triangle2 = triangle4;
            }

            if (v2 != v3) {
                throw new Exception("Expecting initial vertex");
            }

            triangle3.Adj0 = triangle2;
            triangle2.Adj2 = triangle3;
            return true;
        }

        private static void ExtractIndices(HashSet<Triangle> hull, out int[] indices) {
            int count = hull.Count;
            indices = new int[3 * count];
            int num = 0;
            foreach (Triangle item in hull) {
                indices[num] = item.V0;
                num++;
                indices[num] = item.V1;
                num++;
                indices[num] = item.V2;
                num++;
            }

            hull.Clear();
        }
    }
}