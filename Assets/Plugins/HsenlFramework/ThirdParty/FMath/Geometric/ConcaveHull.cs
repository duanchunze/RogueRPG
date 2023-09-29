using System;
using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// 凹包
    /// </summary>
    public static class ConcaveHull {
        public static bool Create2D(FVector2[] points, out int[] concaveHull, out int[] convexHull,
            FLOAT algorithmThreshold) {
            FLOAT epsilon = 1E-05f;
            if (algorithmThreshold <= 0f) {
                throw new Exception("algorithmThreshold must be positive number");
            }

            if (!ConvexHull.Create2D(points, out convexHull, out var dimension, epsilon)) {
                throw new Exception("Convex hull creation failed, can't create concave hull");
            }

            if (dimension != 2) {
                throw new Exception("Convex hull dimension is less than 2, can't create concave hull");
            }

            bool flag = ConcaveHull2.Create(points, out concaveHull, convexHull, algorithmThreshold, epsilon);
            if (!flag) {
                convexHull = null;
            }

            return flag;
        }

        public static bool Create2D(FVector2[] points, out int[] concaveHull, out int[] convexHull,
            FLOAT algorithmThreshold, FLOAT epsilon) {
            if (algorithmThreshold <= 0f) {
                throw new Exception("algorithmThreshold must be positive number");
            }

            if (!ConvexHull.Create2D(points, out convexHull, out var dimension, epsilon)) {
                throw new Exception("Convex hull creation failed, can't create concave hull");
            }

            if (dimension != 2) {
                throw new Exception("Convex hull dimension is less than 2, can't create concave hull");
            }

            bool flag = ConcaveHull2.Create(points, out concaveHull, convexHull, algorithmThreshold, epsilon);
            if (!flag) {
                convexHull = null;
            }

            return flag;
        }

        public static bool Create2D(FVector2[] points, out int[] concaveHull, FLOAT algorithmThreshold) {
            FLOAT epsilon = 1E-05f;
            return Create2D(points, out concaveHull, out _, algorithmThreshold, epsilon);
        }

        public static bool Create2D(FVector2[] points, out int[] concaveHull, FLOAT algorithmThreshold, FLOAT epsilon) {
            return Create2D(points, out concaveHull, out _, algorithmThreshold, epsilon);
        }
    }

    internal class ConcaveHull2 {
        private struct Edge {
            public int V0;

            public int V1;

            public Edge(int v0, int v1) {
                this.V0 = v0;
                this.V1 = v1;
            }
        }

        private struct InnerPoint {
            public FLOAT AverageDistance;

            public FLOAT Distance0;

            public FLOAT Distance1;

            public int Index;
        }

        private static void Quicksort(InnerPoint[] x, int first, int last) {
            if (first >= last) {
                return;
            }

            int i = first;
            int num = last;
            InnerPoint innerPoint;
            while (i < num) {
                for (; x[i].AverageDistance <= x[first].AverageDistance && i < last; i++) { }

                while (x[num].AverageDistance > x[first].AverageDistance) {
                    num--;
                }

                if (i < num) {
                    innerPoint = x[i];
                    ref InnerPoint reference = ref x[i];
                    reference = x[num];
                    x[num] = innerPoint;
                }
            }

            innerPoint = x[first];
            ref InnerPoint reference2 = ref x[first];
            reference2 = x[num];
            x[num] = innerPoint;
            Quicksort(x, first, num - 1);
            Quicksort(x, num + 1, last);
        }

        private static FLOAT CalcDistanceFromPointToEdge(ref FVector2 pointA, ref FVector2 v0, ref FVector2 v1) {
            FLOAT num = v0.x - pointA.x;
            FLOAT num2 = v0.y - pointA.y;
            FLOAT num3 = num * num + num2 * num2;
            num = v1.x - pointA.x;
            num2 = v1.y - pointA.y;
            FLOAT num4 = num * num + num2 * num2;
            num = v0.x - v1.x;
            num2 = v0.y - v1.y;
            FLOAT num5 = num * num + num2 * num2;
            if (num3 < num4) {
                (num3, num4) = (num4, num3);
            }

            if (num3 > num4 + num5 || num5 < 1E-05f) {
                return FMath.Sqrt(num4);
            }

            FLOAT f = v0.x * v1.y - v1.x * v0.y + v1.x * pointA.y - pointA.x * v1.y + pointA.x * v0.y - v0.x * pointA.y;
            return FMath.Abs(f) / FMath.Sqrt(num5);
        }

        public static bool Create(FVector2[] points, out int[] concaveHull, int[] convexHull, FLOAT N) {
            // FLOAT epsilon = 1E-05f;
            LinkedList<Edge> linkedList = new LinkedList<Edge>();
            int num = convexHull.Length;
            HashSet<int> hashSet = new HashSet<int>();
            int num2 = 0;
            int num3 = points.Length;
            for (num2 = 0; num2 < num3; num2++) {
                hashSet.Add(num2);
            }

            int num4 = num - 1;
            for (int i = 0; i < num; i++) {
                int num5 = convexHull[i];
                linkedList.AddLast(new Edge(convexHull[num4], num5));
                hashSet.Remove(num5);
                num4 = i;
            }

            InnerPoint[] array = new InnerPoint[hashSet.Count];
            LinkedListNode<Edge> linkedListNode = linkedList.First;
            while (linkedListNode != null && hashSet.Count != 0) {
                int v = linkedListNode.Value.V0;
                int v2 = linkedListNode.Value.V1;
                FVector2 v3 = points[v];
                FVector2 v4 = points[v2];
                int num6 = 0;
                foreach (int item in hashSet) {
                    FVector2 vector = points[item];
                    FLOAT num7 = vector.x - v3.x;
                    FLOAT num8 = vector.y - v3.y;
                    FLOAT num9 = FMath.Sqrt(num7 * num7 + num8 * num8);
                    num7 = vector.x - v4.x;
                    num8 = vector.y - v4.y;
                    FLOAT num10 = FMath.Sqrt(num7 * num7 + num8 * num8);
                    FLOAT averageDistance = (num9 + num10) * 0.5f;
                    InnerPoint innerPoint = default(InnerPoint);
                    innerPoint.Distance0 = num9;
                    innerPoint.Distance1 = num10;
                    innerPoint.AverageDistance = averageDistance;
                    innerPoint.Index = item;
                    array[num6] = innerPoint;
                    num6++;
                }

                Quicksort(array, 0, num6 - 1);
                InnerPoint innerPoint2 = default(InnerPoint);
                bool flag = false;
                int j = 0;
                for (int num11 = num6; j < num11; j++) {
                    InnerPoint innerPoint3 = array[j];
                    FVector2 pointA = points[innerPoint3.Index];
                    int num12 = ((innerPoint3.Distance0 < innerPoint3.Distance1) ? v : v2);
                    LinkedListNode<Edge> linkedListNode2 = linkedList.First;
                    LinkedListNode<Edge> linkedListNode3 = null;
                    while (linkedListNode2 != null) {
                        if (linkedListNode2 != linkedListNode &&
                            (linkedListNode2.Value.V0 == num12 || linkedListNode2.Value.V1 == num12)) {
                            linkedListNode3 = linkedListNode2;
                            break;
                        }

                        linkedListNode2 = linkedListNode2.Next;
                    }

                    FLOAT num13 = CalcDistanceFromPointToEdge(ref pointA, ref v3, ref v4);
                    FLOAT num14 = CalcDistanceFromPointToEdge(ref pointA, ref points[linkedListNode3.Value.V0],
                        ref points[linkedListNode3.Value.V1]);
                    if (num13 < num14) {
                        innerPoint2 = innerPoint3;
                        flag = true;
                        break;
                    }
                }

                if (!flag) {
                    linkedListNode = linkedListNode.Next;
                    continue;
                }

                FLOAT num15 = ((innerPoint2.Distance0 < innerPoint2.Distance1)
                    ? innerPoint2.Distance0
                    : innerPoint2.Distance1);
                FLOAT magnitude = (v3 - v4).magnitude;
                if (num15 > 0f && magnitude / num15 > N) {
                    LinkedListNode<Edge> node = linkedListNode;
                    linkedListNode = linkedListNode.Next;
                    linkedList.Remove(node);
                    int index = innerPoint2.Index;
                    linkedList.AddLast(new Edge(v, index));
                    linkedList.AddLast(new Edge(index, v2));
                    hashSet.Remove(index);
                }
                else {
                    linkedListNode = linkedListNode.Next;
                }
            }

            LinkedListNode<Edge> linkedListNode4 = linkedList.First;
            bool flag2;
            do {
                flag2 = false;
                for (LinkedListNode<Edge> next = linkedListNode4.Next; next != null; next = next.Next) {
                    if (linkedListNode4.Value.V1 == next.Value.V0) {
                        linkedList.Remove(next);
                        linkedList.AddAfter(linkedListNode4, next);
                        linkedListNode4 = next;
                        flag2 = true;
                        break;
                    }
                }
            } while (flag2);

            concaveHull = new int[linkedList.Count];
            num2 = 0;
            foreach (Edge item2 in linkedList) {
                concaveHull[num2] = item2.V0;
                num2++;
            }

            return true;
        }

        public static bool Create(FVector2[] points, out int[] concaveHull, int[] convexHull, FLOAT N, FLOAT epsilon) {
            LinkedList<Edge> linkedList = new LinkedList<Edge>();
            int num = convexHull.Length;
            HashSet<int> hashSet = new HashSet<int>();
            int num2 = 0;
            int num3 = points.Length;
            for (num2 = 0; num2 < num3; num2++) {
                hashSet.Add(num2);
            }

            int num4 = num - 1;
            for (int i = 0; i < num; i++) {
                int num5 = convexHull[i];
                linkedList.AddLast(new Edge(convexHull[num4], num5));
                hashSet.Remove(num5);
                num4 = i;
            }

            InnerPoint[] array = new InnerPoint[hashSet.Count];
            LinkedListNode<Edge> linkedListNode = linkedList.First;
            while (linkedListNode != null && hashSet.Count != 0) {
                int v = linkedListNode.Value.V0;
                int v2 = linkedListNode.Value.V1;
                FVector2 v3 = points[v];
                FVector2 v4 = points[v2];
                int num6 = 0;
                foreach (int item in hashSet) {
                    FVector2 vector = points[item];
                    FLOAT num7 = vector.x - v3.x;
                    FLOAT num8 = vector.y - v3.y;
                    FLOAT num9 = FMath.Sqrt(num7 * num7 + num8 * num8);
                    num7 = vector.x - v4.x;
                    num8 = vector.y - v4.y;
                    FLOAT num10 = FMath.Sqrt(num7 * num7 + num8 * num8);
                    FLOAT averageDistance = (num9 + num10) * 0.5f;
                    InnerPoint innerPoint = default(InnerPoint);
                    innerPoint.Distance0 = num9;
                    innerPoint.Distance1 = num10;
                    innerPoint.AverageDistance = averageDistance;
                    innerPoint.Index = item;
                    array[num6] = innerPoint;
                    num6++;
                }

                Quicksort(array, 0, num6 - 1);
                InnerPoint innerPoint2 = default(InnerPoint);
                bool flag = false;
                int j = 0;
                for (int num11 = num6; j < num11; j++) {
                    InnerPoint innerPoint3 = array[j];
                    FVector2 pointA = points[innerPoint3.Index];
                    int num12 = ((innerPoint3.Distance0 < innerPoint3.Distance1) ? v : v2);
                    LinkedListNode<Edge> linkedListNode2 = linkedList.First;
                    LinkedListNode<Edge> linkedListNode3 = null;
                    while (linkedListNode2 != null) {
                        if (linkedListNode2 != linkedListNode &&
                            (linkedListNode2.Value.V0 == num12 || linkedListNode2.Value.V1 == num12)) {
                            linkedListNode3 = linkedListNode2;
                            break;
                        }

                        linkedListNode2 = linkedListNode2.Next;
                    }

                    FLOAT num13 = CalcDistanceFromPointToEdge(ref pointA, ref v3, ref v4);
                    FLOAT num14 = CalcDistanceFromPointToEdge(ref pointA, ref points[linkedListNode3.Value.V0],
                        ref points[linkedListNode3.Value.V1]);
                    if (num13 < num14) {
                        innerPoint2 = innerPoint3;
                        flag = true;
                        break;
                    }
                }

                if (!flag) {
                    linkedListNode = linkedListNode.Next;
                    continue;
                }

                FLOAT num15 = ((innerPoint2.Distance0 < innerPoint2.Distance1)
                    ? innerPoint2.Distance0
                    : innerPoint2.Distance1);
                FLOAT magnitude = (v3 - v4).magnitude;
                if (num15 > 0f && magnitude / num15 > N) {
                    LinkedListNode<Edge> node = linkedListNode;
                    linkedListNode = linkedListNode.Next;
                    linkedList.Remove(node);
                    int index = innerPoint2.Index;
                    linkedList.AddLast(new Edge(v, index));
                    linkedList.AddLast(new Edge(index, v2));
                    hashSet.Remove(index);
                }
                else {
                    linkedListNode = linkedListNode.Next;
                }
            }

            LinkedListNode<Edge> linkedListNode4 = linkedList.First;
            bool flag2;
            do {
                flag2 = false;
                for (LinkedListNode<Edge> next = linkedListNode4.Next; next != null; next = next.Next) {
                    if (linkedListNode4.Value.V1 == next.Value.V0) {
                        linkedList.Remove(next);
                        linkedList.AddAfter(linkedListNode4, next);
                        linkedListNode4 = next;
                        flag2 = true;
                        break;
                    }
                }
            } while (flag2);

            concaveHull = new int[linkedList.Count];
            num2 = 0;
            foreach (Edge item2 in linkedList) {
                concaveHull[num2] = item2.V0;
                num2++;
            }

            return true;
        }
    }
}