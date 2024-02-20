using System.Collections.Generic;
using FixedMath;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public class CollisionDetectionTemp {
        public class Pool<T> where T : class, new() {
            private readonly Queue<T> pool = new Queue<T>();

            public T Fetch() {
                if (pool.Count == 0) {
                    return new T();
                }

                return pool.Dequeue();
            }

            public void Recycle(T t) {
                pool.Enqueue(t);
            }

            public void Clear() {
                pool.Clear();
            }
        }

        /// <summary>
        /// GJK（Gilbert-Johnson-Keerthi）碰撞检测算法，该算法是由这三个人最先提出的
        /// <para>虽然GJK数学模型的算法比较复杂，难以理解，但GJK的算法有快速、易实施且适用于多种凸体的优点。传统的GJK是用来计算物体的间距的，
        /// 这里实施了一种新的GJK算法，不仅能够进行物体间的距离查询，还能返回互相穿刺物体间的穿刺深度，并在性能上做出了改进</para>>
        /// </summary>
        public static class GJKCollide {
            private static FLOAT CollideEpsilon = Fixp.Epsilon;
            private const int MaxIterations = 15;
            private static Pool<VoronoiSimplexSolver> simplexSolverPool = new();

            /// <summary>
            /// 转换SupportPoint
            /// </summary>
            /// <param name="support">该形状</param>
            /// <param name="orientation"></param>
            /// <param name="invOrientation">该形状的矩阵</param>
            /// <param name="position">该形状的位置</param>
            /// <param name="direction">指定的方向</param>
            /// <param name="result">返回一个形状指定方向上，最远的点（世界坐标系）</param>
            internal static void SupportMapTransformed(INarrowPhase support, ref FMatrix3x3 orientation,
                ref FMatrix3x3 invOrientation, ref FVector3 position, ref FVector3 direction, out FVector3 result) {
                // 【该方法是计算碰撞部分的高频代码】

                /* 注释掉的这部分代码是只需要orientation不需要invOrientation的方案，在缩放为1时，逆矩阵=转置矩阵，但缩放不为1时，
                 * 第一步的世界转局部坐标的结果就不一样了，虽然方向不一样，但测试的box Collision时却不受影响，猜测可能是因为
                 * 即使方向不对，但不会差到连象限都不一样的程度，而box的SupportMapping又是只通过象限来决定结果，所以造成了，虽然方向计算不一致，
                 * 但结果却是一样的情况
                 *
                // 乘转置矩阵，把世界坐标的方向，转成support坐标的方向
                result.x = ((direction.x * orientation.m11) + (direction.y * orientation.m21)) +
                           (direction.z * orientation.m31);
                result.y = ((direction.x * orientation.m12) + (direction.y * orientation.m22)) +
                           (direction.z * orientation.m32);
                result.z = ((direction.x * orientation.m13) + (direction.y * orientation.m23)) +
                           (direction.z * orientation.m33);

                // 求出该形状的该方向的最远的点（形状坐标系）
                support.SupportMapping(ref result, out result);

                // 再把direction转成世界坐标
                var x = ((result.x * orientation.m11) + (result.y * orientation.m12)) + (result.z * orientation.m13);
                var y = ((result.x * orientation.m21) + (result.y * orientation.m22)) + (result.z * orientation.m23);
                var z = ((result.x * orientation.m31) + (result.y * orientation.m32)) + (result.z * orientation.m33);

                result.x = position.x + x;
                result.y = position.y + y;
                result.z = position.z + z;
                 */

                // 使用逆矩阵，把世界方向，转成support坐标系的局部方向
                FMatrix3x3.Transform(ref invOrientation, ref direction, out result);

                // 求出该形状的该方向的最远的点（形状坐标系）
                support.SupportPoint(ref result, out result);

                // 再把direction转成世界方向
                FMatrix3x3.Transform(ref orientation, ref result, out result);

                // 得到该形状，指定方向上，最远点的世界坐标
                result.x = position.x + result.x;
                result.y = position.y + result.y;
                result.z = position.z + result.z;
            }

            /// <summary>
            /// Checks if given point is within a shape.
            /// </summary>
            /// <param name="support">The support map implementation representing the shape.</param>
            /// <param name="orientation">The orientation of the shape.</param>
            /// <param name="invOrientation"></param>
            /// <param name="position">The position of the shape.</param>
            /// <param name="point">The point to check.</param>
            /// <returns>Returns true if the point is within the shape, otherwise false.</returns>
            public static bool Pointcast(INarrowPhase support, ref FMatrix3x3 orientation,
                ref FMatrix3x3 invOrientation, ref FVector3 position, ref FVector3 point) {
                SupportMapTransformed(support, ref orientation, ref invOrientation, ref position, ref point,
                    out var arbitraryPoint);
                FVector3.Subtract(ref point, ref arbitraryPoint, out arbitraryPoint);

                support.SupportCenter(out var r);
                FMatrix3x3.Transform(ref orientation, ref r, out r);
                FVector3.Add(ref position, ref r, out r);
                FVector3.Subtract(ref point, ref r, out r);

                FVector3 x = point;
                FVector3 w, p;
                FLOAT VdotR;

                FVector3 v;
                FVector3.Subtract(ref x, ref arbitraryPoint, out v);
                FLOAT dist = v.sqrMagnitude;
                FLOAT epsilon = CollideEpsilon;

                int maxIter = MaxIterations;

                VoronoiSimplexSolver simplexSolver = simplexSolverPool.Fetch();

                simplexSolver.Reset();

                while ((dist > epsilon) && (maxIter-- != 0)) {
                    SupportMapTransformed(support, ref orientation, ref invOrientation, ref position, ref v, out p);
                    FVector3.Subtract(ref x, ref p, out w);

                    FVector3.Dot(ref v, ref w, out var VdotW);

                    if (VdotW > Fixp.Zero) {
                        FVector3.Dot(ref v, ref r, out VdotR);

                        if (VdotR >= -(FMath.Epsilon * FMath.Epsilon)) {
                            simplexSolverPool.Recycle(simplexSolver);
                            return false;
                        }
                        else
                            simplexSolver.Reset();
                    }

                    if (!simplexSolver.InSimplex(w)) simplexSolver.AddVertex(ref w, ref x, ref p);

                    if (simplexSolver.Closest(out v))
                        dist = v.sqrMagnitude;
                    else
                        dist = Fixp.Zero;
                }

                simplexSolverPool.Recycle(simplexSolver);
                return true;
            }


            /// <summary>
            /// 两个形状的最近点。值得一提的是，GJK算法最初就是用来算多边形最近距离的
            /// </summary>
            /// <param name="support1"></param>
            /// <param name="support2"></param>
            /// <param name="orientation1"></param>
            /// <param name="orientation2"></param>
            /// <param name="invOrientation1"></param>
            /// <param name="invOrientation2"></param>
            /// <param name="position1"></param>
            /// <param name="position2"></param>
            /// <param name="p1"></param>
            /// <param name="p2"></param>
            /// <param name="normal"></param>
            /// <returns></returns>
            public static bool ClosestPoints(INarrowPhase support1, INarrowPhase support2,
                ref FMatrix3x3 orientation1, ref FMatrix3x3 orientation2, ref FMatrix3x3 invOrientation1,
                ref FMatrix3x3 invOrientation2, ref FVector3 position1, ref FVector3 position2, out FVector3 p1,
                out FVector3 p2, out FVector3 normal) {
                VoronoiSimplexSolver simplexSolver = simplexSolverPool.Fetch();
                simplexSolver.Reset();

                p1 = p2 = FVector3.Zero;

                FVector3 r = position1 - position2;
                FVector3 w, v;

                FVector3 supVertexA;
                FVector3 rn, vn;

                rn = -r;

                SupportMapTransformed(support1, ref orientation1, ref invOrientation1, ref position1, ref rn,
                    out supVertexA);

                FVector3 supVertexB;
                SupportMapTransformed(support2, ref orientation2, ref invOrientation2, ref position2, ref r,
                    out supVertexB);

                v = supVertexA - supVertexB;

                normal = FVector3.Zero;

                int maxIter = MaxIterations;

                FLOAT distSq = v.sqrMagnitude;
                FLOAT epsilon = CollideEpsilon;

                while ((distSq > epsilon) && (maxIter-- != 0)) {
                    vn = -v;
                    SupportMapTransformed(support1, ref orientation1, ref invOrientation1, ref position1, ref vn,
                        out supVertexA);
                    SupportMapTransformed(support2, ref orientation2, ref invOrientation2, ref position2, ref v,
                        out supVertexB);
                    w = supVertexA - supVertexB;

                    if (!simplexSolver.InSimplex(w)) simplexSolver.AddVertex(ref w, ref supVertexA, ref supVertexB);
                    if (simplexSolver.Closest(out v)) {
                        distSq = v.sqrMagnitude;
                        normal = v;
                    }
                    else
                        distSq = Fixp.Zero;
                }


                simplexSolver.ComputePoints(out p1, out p2);

                if (normal.sqrMagnitude > FMath.Epsilon * FMath.Epsilon) normal.Normalize();

                simplexSolverPool.Recycle(simplexSolver);

                return true;
            }

            // see: btSubSimplexConvexCast.cpp

            /// <summary>
            /// Checks if a ray definied through it's origin and direction collides
            /// with a shape.
            /// </summary>
            /// <param name="support">The supportmap implementation representing the shape.</param>
            /// <param name="orientation">The orientation of the shape.</param>
            /// <param name="invOrientation">The inverse orientation of the shape.</param>
            /// <param name="position">The position of the shape.</param>
            /// <param name="origin">The origin of the ray.</param>
            /// <param name="direction">The direction of the ray.</param>
            /// <param name="fraction">The fraction which gives information where at the 
            /// ray the collision occured. The hitPoint is calculated by: origin+friction*direction.</param>
            /// <param name="normal">The normal from the ray collision.</param>
            /// <returns>Returns true if the ray hit the shape, false otherwise.</returns>
            public static bool Raycast(INarrowPhase support, ref FMatrix3x3 orientation,
                ref FMatrix3x3 invOrientation, ref FVector3 position, ref FVector3 origin,
                ref FVector3 direction, out FLOAT fraction, out FVector3 normal) {
                VoronoiSimplexSolver simplexSolver = simplexSolverPool.Fetch();
                simplexSolver.Reset();

                normal = FVector3.Zero;
                fraction = Fixp.MaxValue;

                FLOAT lambda = Fixp.Zero;

                FVector3 r = direction;
                FVector3 x = origin;
                FVector3 w, p, v;

                // 获得形状指定方向上最远的点
                SupportMapTransformed(support, ref orientation, ref invOrientation, ref position, ref r,
                    out var arbitraryPoint);
                // 射线起点 - 射线方向上，形状的最远点 = 形状离射线起点最远的距离
                FVector3.Subtract(ref x, ref arbitraryPoint, out v);

                int maxIter = MaxIterations;

                FLOAT distSq = v.sqrMagnitude;
                FLOAT epsilon = Fixp.Epsilon;

                while ((distSq > epsilon) && (maxIter-- != 0)) {
                    SupportMapTransformed(support, ref orientation, ref invOrientation, ref position, ref v, out p);
                    FVector3.Subtract(ref x, ref p, out w);

                    FVector3.Dot(ref v, ref w, out var VdotW);

                    if (VdotW > Fixp.Zero) {
                        FVector3.Dot(ref v, ref r, out var VdotR);

                        if (VdotR >= -FMath.Epsilon) {
                            simplexSolverPool.Recycle(simplexSolver);
                            return false;
                        }
                        else {
                            lambda = lambda - VdotW / VdotR;
                            FVector3.Multiply(ref r, lambda, out x);
                            FVector3.Add(ref origin, ref x, out x);
                            FVector3.Subtract(ref x, ref p, out w);
                            normal = v;
                        }
                    }

                    if (!simplexSolver.InSimplex(w)) simplexSolver.AddVertex(ref w, ref x, ref p);
                    distSq = simplexSolver.Closest(out v) ? v.sqrMagnitude : Fixp.Zero;
                }

                #region Retrieving hitPoint

                // Giving back the fraction like this *should* work
                // but is inaccurate against large objects:
                // fraction = lambda;

                simplexSolver.ComputePoints(out _, out var p2);

                p2 = p2 - origin;
                fraction = p2.magnitude / direction.magnitude;

                #endregion

                if (normal.sqrMagnitude > FMath.Epsilon * FMath.Epsilon) normal.Normalize();

                simplexSolverPool.Recycle(simplexSolver);

                return true;
            }

            // Bullet has problems with raycasting large objects - so does jitter
            // hope to fix that in the next versions.

            /*
              Bullet for XNA Copyright (c) 2003-2007 Vsevolod Klementjev http://www.codeplex.com/xnadevru
              Bullet original C++ version Copyright (c) 2003-2007 Erwin Coumans http://bulletphysics.com

              This software is provided 'as-is', without any express or implied
              warranty.  In no event will the authors be held liable for any damages
              arising from the use of this software.

              Permission is granted to anyone to use this software for any purpose,
              including commercial applications, and to alter it and redistribute it
              freely, subject to the following restrictions:

              1. The origin of this software must not be misrepresented; you must not
                 claim that you wrote the original software. If you use this software
                 in a product, an acknowledgment in the product documentation would be
                 appreciated but is not required.
              2. Altered source versions must be plainly marked as such, and must not be
                 misrepresented as being the original software.
              3. This notice may not be removed or altered from any source distribution.
            */

            /// VoronoiSimplexSolver is an implementation of the closest point distance
            /// algorithm from a 1-4 points simplex to the origin.
            /// Can be used with GJK, as an alternative to Johnson distance algorithm. 
            internal class VoronoiSimplexSolver {
                private const int VertexA = 0, VertexB = 1, VertexC = 2, VertexD = 3;

                private const int VoronoiSimplexMaxVerts = 5;
                private const bool CatchDegenerateTetrahedron = true;

                private int _numVertices;

                private FVector3[] _simplexVectorW = new FVector3[VoronoiSimplexMaxVerts];
                private FVector3[] _simplexPointsP = new FVector3[VoronoiSimplexMaxVerts];
                private FVector3[] _simplexPointsQ = new FVector3[VoronoiSimplexMaxVerts];

                private FVector3 _cachedPA;
                private FVector3 _cachedPB;
                private FVector3 _cachedV;
                private FVector3 _lastW;
                private bool _cachedValidClosest;

                private SubSimplexClosestResult _cachedBC = new SubSimplexClosestResult();

                // Note that this assumes ray-casts and point-casts will always be called from the
                // same thread which I assume is true from the _cachedBC member
                // If this needs to made multi-threaded a resource pool will be needed
                private SubSimplexClosestResult tempResult = new SubSimplexClosestResult();

                private bool _needsUpdate;

                #region ISimplexSolver Members

                public bool FullSimplex {
                    get { return _numVertices == 4; }
                }

                public int NumVertices {
                    get { return _numVertices; }
                }

                public void Reset() {
                    _cachedValidClosest = false;
                    _numVertices = 0;
                    _needsUpdate = true;
                    _lastW = new FVector3(Fixp.MaxValue, Fixp.MaxValue, Fixp.MaxValue);
                    _cachedBC.Reset();
                }

                public void AddVertex(ref FVector3 w, ref FVector3 p, ref FVector3 q) {
                    _lastW = w;
                    _needsUpdate = true;

                    _simplexVectorW[_numVertices] = w;
                    _simplexPointsP[_numVertices] = p;
                    _simplexPointsQ[_numVertices] = q;

                    _numVertices++;
                }

                //return/calculate the closest vertex
                public bool Closest(out FVector3 v) {
                    bool succes = UpdateClosestVectorAndPoints();
                    v = _cachedV;
                    return succes;
                }

                public FLOAT MaxVertex {
                    get {
                        int numverts = NumVertices;
                        FLOAT maxV = Fixp.Zero, curLen2;
                        for (int i = 0; i < numverts; i++) {
                            curLen2 = _simplexVectorW[i].sqrMagnitude;
                            if (maxV < curLen2) maxV = curLen2;
                        }

                        return maxV;
                    }
                }

                //return the current simplex
                public int GetSimplex(out FVector3[] pBuf, out FVector3[] qBuf, out FVector3[] yBuf) {
                    int numverts = NumVertices;
                    pBuf = new FVector3[numverts];
                    qBuf = new FVector3[numverts];
                    yBuf = new FVector3[numverts];
                    for (int i = 0; i < numverts; i++) {
                        yBuf[i] = _simplexVectorW[i];
                        pBuf[i] = _simplexPointsP[i];
                        qBuf[i] = _simplexPointsQ[i];
                    }

                    return numverts;
                }

                public bool InSimplex(FVector3 w) {
                    //check in case lastW is already removed
                    if (w == _lastW) return true;

                    //w is in the current (reduced) simplex
                    int numverts = NumVertices;
                    for (int i = 0; i < numverts; i++)
                        if (_simplexVectorW[i] == w)
                            return true;

                    return false;
                }

                public void BackupClosest(out FVector3 v) {
                    v = _cachedV;
                }

                public bool EmptySimplex {
                    get { return NumVertices == 0; }
                }

                public void ComputePoints(out FVector3 p1, out FVector3 p2) {
                    UpdateClosestVectorAndPoints();
                    p1 = _cachedPA;
                    p2 = _cachedPB;
                }

                #endregion

                public void RemoveVertex(int index) {
                    _numVertices--;
                    _simplexVectorW[index] = _simplexVectorW[_numVertices];
                    _simplexPointsP[index] = _simplexPointsP[_numVertices];
                    _simplexPointsQ[index] = _simplexPointsQ[_numVertices];
                }

                public void ReduceVertices(UsageBitfield usedVerts) {
                    if ((NumVertices >= 4) && (!usedVerts.UsedVertexD)) RemoveVertex(3);
                    if ((NumVertices >= 3) && (!usedVerts.UsedVertexC)) RemoveVertex(2);
                    if ((NumVertices >= 2) && (!usedVerts.UsedVertexB)) RemoveVertex(1);
                    if ((NumVertices >= 1) && (!usedVerts.UsedVertexA)) RemoveVertex(0);
                }

                public bool UpdateClosestVectorAndPoints() {
                    if (_needsUpdate) {
                        _cachedBC.Reset();
                        _needsUpdate = false;

                        FVector3 p, a, b, c, d;
                        switch (NumVertices) {
                            case 0:
                                _cachedValidClosest = false;
                                break;
                            case 1:
                                _cachedPA = _simplexPointsP[0];
                                _cachedPB = _simplexPointsQ[0];
                                _cachedV = _cachedPA - _cachedPB;
                                _cachedBC.Reset();
                                _cachedBC.SetBarycentricCoordinates(1f, Fixp.Zero, Fixp.Zero, Fixp.Zero);
                                _cachedValidClosest = _cachedBC.IsValid;
                                break;
                            case 2:
                                //closest point origin from line segment
                                FVector3 from = _simplexVectorW[0];
                                FVector3 to = _simplexVectorW[1];
                                //FixVector3 nearest;

                                FVector3 diff = from * (-1);
                                FVector3 v = to - from;
                                FVector3.Dot(ref v, ref diff, out var t);

                                if (t > 0) {
                                    FLOAT dotVV = v.sqrMagnitude;
                                    if (t < dotVV) {
                                        t /= dotVV;
                                        diff -= t * v;
                                        _cachedBC.UsedVertices.UsedVertexA = true;
                                        _cachedBC.UsedVertices.UsedVertexB = true;
                                    }
                                    else {
                                        t = 1;
                                        diff -= v;
                                        //reduce to 1 point
                                        _cachedBC.UsedVertices.UsedVertexB = true;
                                    }
                                }
                                else {
                                    t = 0;
                                    //reduce to 1 point
                                    _cachedBC.UsedVertices.UsedVertexA = true;
                                }

                                _cachedBC.SetBarycentricCoordinates(1 - t, t, 0, 0);
                                //nearest = from + t * v;

                                _cachedPA = _simplexPointsP[0] + t * (_simplexPointsP[1] - _simplexPointsP[0]);
                                _cachedPB = _simplexPointsQ[0] + t * (_simplexPointsQ[1] - _simplexPointsQ[0]);
                                _cachedV = _cachedPA - _cachedPB;

                                ReduceVertices(_cachedBC.UsedVertices);

                                _cachedValidClosest = _cachedBC.IsValid;
                                break;
                            case 3:
                                //closest point origin from triangle
                                p = new FVector3();
                                a = _simplexVectorW[0];
                                b = _simplexVectorW[1];
                                c = _simplexVectorW[2];

                                ClosestPtPointTriangle(ref p, ref a, ref b, ref c, ref _cachedBC);
                                _cachedPA = _simplexPointsP[0] * _cachedBC.BarycentricCoords[0] +
                                            _simplexPointsP[1] * _cachedBC.BarycentricCoords[1] +
                                            _simplexPointsP[2] * _cachedBC.BarycentricCoords[2] +
                                            _simplexPointsP[3] * _cachedBC.BarycentricCoords[3];

                                _cachedPB = _simplexPointsQ[0] * _cachedBC.BarycentricCoords[0] +
                                            _simplexPointsQ[1] * _cachedBC.BarycentricCoords[1] +
                                            _simplexPointsQ[2] * _cachedBC.BarycentricCoords[2] +
                                            _simplexPointsQ[3] * _cachedBC.BarycentricCoords[3];

                                _cachedV = _cachedPA - _cachedPB;

                                ReduceVertices(_cachedBC.UsedVertices);
                                _cachedValidClosest = _cachedBC.IsValid;
                                break;
                            case 4:
                                p = new FVector3();
                                a = _simplexVectorW[0];
                                b = _simplexVectorW[1];
                                c = _simplexVectorW[2];
                                d = _simplexVectorW[3];

                                bool hasSeperation =
                                    ClosestPtPointTetrahedron(ref p, ref a, ref b, ref c, ref d, ref _cachedBC);

                                if (hasSeperation) {
                                    _cachedPA = _simplexPointsP[0] * _cachedBC.BarycentricCoords[0] +
                                                _simplexPointsP[1] * _cachedBC.BarycentricCoords[1] +
                                                _simplexPointsP[2] * _cachedBC.BarycentricCoords[2] +
                                                _simplexPointsP[3] * _cachedBC.BarycentricCoords[3];

                                    _cachedPB = _simplexPointsQ[0] * _cachedBC.BarycentricCoords[0] +
                                                _simplexPointsQ[1] * _cachedBC.BarycentricCoords[1] +
                                                _simplexPointsQ[2] * _cachedBC.BarycentricCoords[2] +
                                                _simplexPointsQ[3] * _cachedBC.BarycentricCoords[3];

                                    _cachedV = _cachedPA - _cachedPB;
                                    ReduceVertices(_cachedBC.UsedVertices);
                                }
                                else {
                                    if (_cachedBC.Degenerate) {
                                        _cachedValidClosest = false;
                                    }
                                    else {
                                        _cachedValidClosest = true;
                                        //degenerate case == false, penetration = true + Zero
                                        _cachedV.x = _cachedV.y = _cachedV.z = Fixp.Zero;
                                    }

                                    break; // !!!!!!!!!!!! proverit na vsakiy sluchai
                                }

                                _cachedValidClosest = _cachedBC.IsValid;

                                //closest point origin from tetrahedron
                                break;
                            default:
                                _cachedValidClosest = false;
                                break;
                        }
                    }

                    return _cachedValidClosest;
                }

                public bool ClosestPtPointTriangle(ref FVector3 p, ref FVector3 a, ref FVector3 b,
                    ref FVector3 c, ref SubSimplexClosestResult result) {
                    result.UsedVertices.Reset();

                    FLOAT v, w;

                    // Check if P in vertex region outside A
                    FVector3 ab = b - a;
                    FVector3 ac = c - a;
                    FVector3 ap = p - a;
                    FVector3.Dot(ref ab, ref ap, out var d1);
                    FVector3.Dot(ref ac, ref ap, out var d2);
                    if (d1 <= Fixp.Zero && d2 <= Fixp.Zero) {
                        result.ClosestPointOnSimplex = a;
                        result.UsedVertices.UsedVertexA = true;
                        result.SetBarycentricCoordinates(1, 0, 0, 0);
                        return true; // a; // barycentric coordinates (1,0,0)
                    }

                    // Check if P in vertex region outside B
                    FVector3 bp = p - b;
                    FVector3.Dot(ref ab, ref bp, out var d3);
                    FVector3.Dot(ref ac, ref bp, out var d4);
                    if (d3 >= Fixp.Zero && d4 <= d3) {
                        result.ClosestPointOnSimplex = b;
                        result.UsedVertices.UsedVertexB = true;
                        result.SetBarycentricCoordinates(0, 1, 0, 0);

                        return true; // b; // barycentric coordinates (0,1,0)
                    }

                    // Check if P in edge region of AB, if so return projection of P onto AB
                    FLOAT vc = d1 * d4 - d3 * d2;
                    if (vc <= Fixp.Zero && d1 >= Fixp.Zero && d3 <= Fixp.Zero) {
                        v = d1 / (d1 - d3);
                        result.ClosestPointOnSimplex = a + v * ab;
                        result.UsedVertices.UsedVertexA = true;
                        result.UsedVertices.UsedVertexB = true;
                        result.SetBarycentricCoordinates(1 - v, v, 0, 0);
                        return true;
                        //return a + v * ab; // barycentric coordinates (1-v,v,0)
                    }

                    // Check if P in vertex region outside C
                    FVector3 cp = p - c;
                    FVector3.Dot(ref ab, ref cp, out var d5);
                    FVector3.Dot(ref ac, ref cp, out var d6);
                    if (d6 >= Fixp.Zero && d5 <= d6) {
                        result.ClosestPointOnSimplex = c;
                        result.UsedVertices.UsedVertexC = true;
                        result.SetBarycentricCoordinates(0, 0, 1, 0);
                        return true; //c; // barycentric coordinates (0,0,1)
                    }

                    // Check if P in edge region of AC, if so return projection of P onto AC
                    FLOAT vb = d5 * d2 - d1 * d6;
                    if (vb <= Fixp.Zero && d2 >= Fixp.Zero && d6 <= Fixp.Zero) {
                        w = d2 / (d2 - d6);
                        result.ClosestPointOnSimplex = a + w * ac;
                        result.UsedVertices.UsedVertexA = true;
                        result.UsedVertices.UsedVertexC = true;
                        result.SetBarycentricCoordinates(1 - w, 0, w, 0);
                        return true;
                        //return a + w * ac; // barycentric coordinates (1-w,0,w)
                    }

                    // Check if P in edge region of BC, if so return projection of P onto BC
                    FLOAT va = d3 * d6 - d5 * d4;
                    if (va <= Fixp.Zero && (d4 - d3) >= Fixp.Zero && (d5 - d6) >= Fixp.Zero) {
                        w = (d4 - d3) / ((d4 - d3) + (d5 - d6));

                        result.ClosestPointOnSimplex = b + w * (c - b);
                        result.UsedVertices.UsedVertexB = true;
                        result.UsedVertices.UsedVertexC = true;
                        result.SetBarycentricCoordinates(0, 1 - w, w, 0);
                        return true;
                        // return b + w * (c - b); // barycentric coordinates (0,1-w,w)
                    }

                    // P inside face region. Compute Q through its barycentric coordinates (u,v,w)
                    FLOAT denom = Fixp.One / (va + vb + vc);
                    v = vb * denom;
                    w = vc * denom;

                    result.ClosestPointOnSimplex = a + ab * v + ac * w;
                    result.UsedVertices.UsedVertexA = true;
                    result.UsedVertices.UsedVertexB = true;
                    result.UsedVertices.UsedVertexC = true;
                    result.SetBarycentricCoordinates(1 - v - w, v, w, 0);

                    return true;
                }

                /// Test if point p and d lie on opposite sides of plane through abc
                public int PointOutsideOfPlane(ref FVector3 p, ref FVector3 a, ref FVector3 b, ref FVector3 c,
                    ref FVector3 d) {
                    FVector3 normal = FVector3.Cross(b - a, c - a);

                    var pa = p - a;
                    var da = d - a;
                    FVector3.Dot(ref pa, ref normal, out var signp); // [AP AB AC]
                    FVector3.Dot(ref da, ref normal, out var signd); // [AD AB AC]

                    //if (CatchDegenerateTetrahedron)
                    if (signd * signd < (Fixp.EN8)) return -1;

                    // Points on opposite sides if expression signs are opposite
                    return signp * signd < Fixp.Zero ? 1 : 0;
                }

                public bool ClosestPtPointTetrahedron(ref FVector3 p, ref FVector3 a, ref FVector3 b,
                    ref FVector3 c, ref FVector3 d, ref SubSimplexClosestResult finalResult) {
                    tempResult.Reset();

                    // Start out assuming point inside all halfspaces, so closest to itself
                    finalResult.ClosestPointOnSimplex = p;
                    finalResult.UsedVertices.Reset();
                    finalResult.UsedVertices.UsedVertexA = true;
                    finalResult.UsedVertices.UsedVertexB = true;
                    finalResult.UsedVertices.UsedVertexC = true;
                    finalResult.UsedVertices.UsedVertexD = true;

                    int pointOutsideABC = PointOutsideOfPlane(ref p, ref a, ref b, ref c, ref d);
                    int pointOutsideACD = PointOutsideOfPlane(ref p, ref a, ref c, ref d, ref b);
                    int pointOutsideADB = PointOutsideOfPlane(ref p, ref a, ref d, ref b, ref c);
                    int pointOutsideBDC = PointOutsideOfPlane(ref p, ref b, ref d, ref c, ref a);

                    if (pointOutsideABC < 0 || pointOutsideACD < 0 || pointOutsideADB < 0 || pointOutsideBDC < 0) {
                        finalResult.Degenerate = true;
                        return false;
                    }

                    if (pointOutsideABC == 0 && pointOutsideACD == 0 && pointOutsideADB == 0 && pointOutsideBDC == 0)
                        return false;

                    FLOAT bestSqDist = Fixp.MaxValue;
                    // If point outside face abc then compute closest point on abc
                    if (pointOutsideABC != 0) {
                        ClosestPtPointTriangle(ref p, ref a, ref b, ref c, ref tempResult);
                        FVector3 q = tempResult.ClosestPointOnSimplex;

                        FLOAT sqDist = ((q - p)).sqrMagnitude;
                        // Update best closest point if (squared) distance is less than current best
                        if (sqDist < bestSqDist) {
                            bestSqDist = sqDist;
                            finalResult.ClosestPointOnSimplex = q;
                            //convert result bitmask!
                            finalResult.UsedVertices.Reset();
                            finalResult.UsedVertices.UsedVertexA = tempResult.UsedVertices.UsedVertexA;
                            finalResult.UsedVertices.UsedVertexB = tempResult.UsedVertices.UsedVertexB;
                            finalResult.UsedVertices.UsedVertexC = tempResult.UsedVertices.UsedVertexC;
                            finalResult.SetBarycentricCoordinates(tempResult.BarycentricCoords[VertexA],
                                tempResult.BarycentricCoords[VertexB], tempResult.BarycentricCoords[VertexC], 0);
                        }
                    }

                    // Repeat test for face acd
                    if (pointOutsideACD != 0) {
                        ClosestPtPointTriangle(ref p, ref a, ref c, ref d, ref tempResult);
                        FVector3 q = tempResult.ClosestPointOnSimplex;
                        //convert result bitmask!

                        FLOAT sqDist = ((q - p)).sqrMagnitude;
                        if (sqDist < bestSqDist) {
                            bestSqDist = sqDist;
                            finalResult.ClosestPointOnSimplex = q;
                            finalResult.UsedVertices.Reset();
                            finalResult.UsedVertices.UsedVertexA = tempResult.UsedVertices.UsedVertexA;
                            finalResult.UsedVertices.UsedVertexC = tempResult.UsedVertices.UsedVertexB;
                            finalResult.UsedVertices.UsedVertexD = tempResult.UsedVertices.UsedVertexC;
                            finalResult.SetBarycentricCoordinates(tempResult.BarycentricCoords[VertexA], 0,
                                tempResult.BarycentricCoords[VertexB], tempResult.BarycentricCoords[VertexC]);
                        }
                    }
                    // Repeat test for face adb

                    if (pointOutsideADB != 0) {
                        ClosestPtPointTriangle(ref p, ref a, ref d, ref b, ref tempResult);
                        FVector3 q = tempResult.ClosestPointOnSimplex;
                        //convert result bitmask!

                        FLOAT sqDist = ((q - p)).sqrMagnitude;
                        if (sqDist < bestSqDist) {
                            bestSqDist = sqDist;
                            finalResult.ClosestPointOnSimplex = q;
                            finalResult.UsedVertices.Reset();
                            finalResult.UsedVertices.UsedVertexA = tempResult.UsedVertices.UsedVertexA;
                            finalResult.UsedVertices.UsedVertexD = tempResult.UsedVertices.UsedVertexB;
                            finalResult.UsedVertices.UsedVertexB = tempResult.UsedVertices.UsedVertexC;
                            finalResult.SetBarycentricCoordinates(tempResult.BarycentricCoords[VertexA],
                                tempResult.BarycentricCoords[VertexC], 0, tempResult.BarycentricCoords[VertexB]);
                        }
                    }
                    // Repeat test for face bdc

                    if (pointOutsideBDC != 0) {
                        ClosestPtPointTriangle(ref p, ref b, ref d, ref c, ref tempResult);
                        FVector3 q = tempResult.ClosestPointOnSimplex;
                        //convert result bitmask!
                        FLOAT sqDist = ((FVector3)(q - p)).sqrMagnitude;
                        if (sqDist < bestSqDist) {
                            bestSqDist = sqDist;
                            finalResult.ClosestPointOnSimplex = q;
                            finalResult.UsedVertices.Reset();
                            finalResult.UsedVertices.UsedVertexB = tempResult.UsedVertices.UsedVertexA;
                            finalResult.UsedVertices.UsedVertexD = tempResult.UsedVertices.UsedVertexB;
                            finalResult.UsedVertices.UsedVertexC = tempResult.UsedVertices.UsedVertexC;

                            finalResult.SetBarycentricCoordinates(0, tempResult.BarycentricCoords[VertexA],
                                tempResult.BarycentricCoords[VertexC], tempResult.BarycentricCoords[VertexB]);
                        }
                    }

                    //help! we ended up full !

                    if (finalResult.UsedVertices.UsedVertexA && finalResult.UsedVertices.UsedVertexB &&
                        finalResult.UsedVertices.UsedVertexC && finalResult.UsedVertices.UsedVertexD) {
                        return true;
                    }

                    return true;
                }
            }

            internal class UsageBitfield {
                private bool _usedVertexA, _usedVertexB, _usedVertexC, _usedVertexD;

                public bool UsedVertexA {
                    get => _usedVertexA;
                    set => _usedVertexA = value;
                }

                public bool UsedVertexB {
                    get => _usedVertexB;
                    set => _usedVertexB = value;
                }

                public bool UsedVertexC {
                    get => _usedVertexC;
                    set => _usedVertexC = value;
                }

                public bool UsedVertexD {
                    get => _usedVertexD;
                    set => _usedVertexD = value;
                }

                public void Reset() {
                    _usedVertexA = _usedVertexB = _usedVertexC = _usedVertexD = false;
                }
            }

            internal class SubSimplexClosestResult {
                private FVector3 _closestPointOnSimplex;

                //MASK for m_usedVertices
                //stores the simplex vertex-usage, using the MASK, 
                // if m_usedVertices & MASK then the related vertex is used
                private UsageBitfield _usedVertices = new UsageBitfield();
                private FLOAT[] _barycentricCoords = new FLOAT[4];
                private bool _degenerate;

                public FVector3 ClosestPointOnSimplex {
                    get => _closestPointOnSimplex;
                    set => _closestPointOnSimplex = value;
                }

                public UsageBitfield UsedVertices {
                    get => _usedVertices;
                    set => _usedVertices = value;
                }

                public FLOAT[] BarycentricCoords {
                    get => _barycentricCoords;
                    set => _barycentricCoords = value;
                }

                public bool Degenerate {
                    get => _degenerate;
                    set => _degenerate = value;
                }

                public void Reset() {
                    _degenerate = false;
                    SetBarycentricCoordinates();
                    _usedVertices.Reset();
                }

                public bool IsValid =>
                    (_barycentricCoords[0] >= Fixp.Zero) && (_barycentricCoords[1] >= Fixp.Zero) &&
                    (_barycentricCoords[2] >= Fixp.Zero) && (_barycentricCoords[3] >= Fixp.Zero);

                public void SetBarycentricCoordinates() {
                    SetBarycentricCoordinates(Fixp.Zero, Fixp.Zero, Fixp.Zero, Fixp.Zero);
                }

                public void SetBarycentricCoordinates(FLOAT a, FLOAT b, FLOAT c, FLOAT d) {
                    _barycentricCoords[0] = a;
                    _barycentricCoords[1] = b;
                    _barycentricCoords[2] = c;
                    _barycentricCoords[3] = d;
                }
            }
        }
    }
}