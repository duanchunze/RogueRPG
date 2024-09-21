/*
            XenoCollide Collision Detection and Physics Library
            Copyright (c) 2007-2014 Gary Snethen http://xenocollide.com

            This software is provided 'as-is', without any express or implied warranty.
            In no event will the authors be held liable for any damages arising
            from the use of this software.
            Permission is granted to anyone to use this software for any purpose,
            including commercial applications, and to alter it and redistribute it freely,
            subject to the following restrictions:

            1. The origin of this software must not be misrepresented; you must
            not claim that you wrote the original software. If you use this
            software in a product, an acknowledgment in the product documentation
            would be appreciated but is not required.
            2. Altered source versions must be plainly marked as such, and must
            not be misrepresented as being the original software.
            3. This notice may not be removed or altered from any source distribution.
*/

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

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public static class CollisionDetection {
        public static int testStep; // 测试用

        private static readonly FLOAT s_collideEpsilon = Fixp.Epsilon;
        private const int k_maximumIterations = 7;

        /// <summary>
        /// 包围盒和包围盒碰撞检测（基于AABB检测）
        /// 虽然这种检测的速度，非常的快
        /// </summary>
        /// <param name="box1">处理过的AABB1（旋转、位移、缩放过的）</param>
        /// <param name="box2">处理过的AABB2（旋转、位移、缩放过的）</param>
        /// <returns></returns>
        public static bool BoundingBoxCollision(ref AABB box1, ref AABB box2) {
            return box1.max.x >= box2.min.x && box1.min.x <= box2.max.x && // x轴
                   box1.max.y >= box2.min.x && box1.min.y <= box2.max.y && // y轴
                   box1.max.z >= box2.min.x && box1.min.z <= box2.max.z; // z
        }

        /// <summary>
        /// 检测射线和包围盒碰撞检测
        /// <para>该方法用于粗略检测射线和物体是否碰撞</para>
        /// <para>该方法提供AABB的转向及缩放支持</para>
        /// </summary>
        /// <param name="origin">射线起点</param>
        /// <param name="direction">射线方向</param>
        /// <param name="boundingBox">包围盒</param>
        /// <param name="invOrientation">包围盒的旋转缩放矩阵的逆矩阵</param>
        /// <param name="position"></param>
        /// <returns>是否碰撞</returns>
        public static bool RayBoundingBoxCollision(ref Vector3 origin, ref Vector3 direction, ref AABB boundingBox,
            ref Matrix3x3 invOrientation, ref Vector3 position) {
            /*
             * 检测射线和包围盒碰撞的基础条件是：包围盒必须是轴对齐的(axis aligned)，也就是包围盒不能旋转。
             * 包围盒不能旋转，但要实现包围盒旋转的检测，就需要旋转射线，把射线按照包围盒旋转的逆旋转，就可以达到射线和旋转的包围盒检测的效果。
             */

            var tmpEnter = Fixp.Zero;
            var tmpExit = Fixp.MaxValue;

            // 把射线的位置转为以包围盒为原点的位置
            Vector3.Subtract(ref origin, ref position, out var tmpOrigin);
            // 把射线的位置转为以包围盒为坐标系的位置
            Matrix3x3.Transform(ref invOrientation, ref tmpOrigin, out tmpOrigin);
            Matrix3x3.Transform(ref invOrientation, ref direction, out var tmpDirection);

            /* 该段注释可以帮助在unity绘图
             *
            Debug.DrawLine(tmpOrigin.ToVector3(), (tmpOrigin + tmpDirection * 10).ToVector3());

            var vertices = boundingBox.CalcVertices();
            for (int i = 0; i < vertices.Length; i++) {
                if (i == vertices.Length - 1) {
                    Debug.DrawLine(vertices[i].ToVector3(), vertices[0].ToVector3());
                }
                else {
                    Debug.DrawLine(vertices[i].ToVector3(), vertices[i + 1].ToVector3());
                }
            }
             */

            if (!Intersect1D(tmpOrigin.x, tmpDirection.x, boundingBox.min.x, boundingBox.max.x, ref tmpEnter, ref tmpExit)) {
                return false;
            }

            if (!Intersect1D(tmpOrigin.y, tmpDirection.y, boundingBox.min.y, boundingBox.max.y, ref tmpEnter, ref tmpExit)) {
                return false;
            }

            if (!Intersect1D(tmpOrigin.z, tmpDirection.z, boundingBox.min.z, boundingBox.max.z, ref tmpEnter, ref tmpExit)) {
                return false;
            }

            // enter代表区间起点，exit代表区间结束点
            bool Intersect1D(FLOAT org, FLOAT dir, FLOAT min, FLOAT max, ref FLOAT enter, ref FLOAT exit) {
                // 如果方向和该轴平行，则直接根据起点位置判断是否重叠
                if (dir * dir < Math.Epsilon * Math.Epsilon) return (org >= min && org <= max);

                /* 理解：
                 * 只判断一个轴是无法确定射线是否与包围盒碰撞的，除非射线的方向和包围盒为钝角，所以，至少要做两个轴判断
                 * 举例：min （1,1）、max（3,3）、origin（0,0）、direction（0.5，1）
                 * 则x轴的tmpMin为2 tmpMax为6
                 * 而y轴的tmpMin为1 tmpMax为3
                 * 3还是大于2的，如果3小于2了，就说明射线偏离包围盒了，没射到。1小于6同理，代表射线和包围盒的另一边碰撞
                 * 此时如果把0.5减少到0.2
                 * 则x轴的tmpMin为5 tmpMax为15
                 * 而y轴的tmpMin大概为0.2 tmpMax大概为1.2
                 * 此时1.2小于5，说明射线已经偏离包围盒了，不会碰撞了
                 * 但又不能让一个轴过大，因为一个轴过大就会导致另一个轴无法弥补，导致射线偏离
                 * 所以，可以看出，direction的x是有个区间的，太大太小都不行。direction的y同理，z也同理
                 * 所以，判断射线与包围盒碰撞就是三个轴都必须在这个区间之内。
                 */

                FLOAT tmpMin = (min - org) / dir;
                FLOAT tmpMax = (max - org) / dir;

                // 确保min是二者较小的那个数，max是二者较大的那个数
                if (tmpMin > tmpMax) {
                    (tmpMin, tmpMax) = (tmpMax, tmpMin);
                }

                if (tmpMin > exit || tmpMax < enter) return false;

                if (tmpMin > enter) enter = tmpMin;
                if (tmpMax < exit) exit = tmpMax;

                return true;
            }

            return true;
        }

        /// <summary>
        /// 形状与形状碰撞检测（基于GJK碰撞算法），因为计算碰撞信息方面不比Xeno，所以改成了只检测碰撞。如果不需要获得碰撞信息，使用该方法效率会稍微高一些
        /// <para>知识点参考 https://blog.hamaluik.ca/posts/building-a-collision-engine-part-1-2d-gjk-collision-detection/</para>>
        /// </summary>
        /// <param name="support1"></param>
        /// <param name="support2"></param>
        /// <returns></returns>
        public static bool ShapeCollision(ISupportPoint support1, ISupportPoint support2) {
            // 注：下文中，所有牵扯到形状1和形状2做差运算时，均是用2减1

            var result = DetectCollision(out var ver1, out var ver2, out var ver3, out var ver4, //
                out var axisDir, out var rAxisDir, //
                out var supportPt, out var rSupportPt, //
                out var ver2Ver, out var ori2Ver, // 
                out var temp1, out var temp2, out var temp3, out var temp4);

            return result;

            // 检测碰撞
            // tip：isCalCollisionInfo - 是否计算出了碰撞信息。该方法主要检测碰撞，不计算碰撞信息，但如果顺便就可以算出的话，也会计算
            // tip：经过测试，其检测速度比Xeno检测法更快速，但问题是，他无法计算碰撞信息，需要单独的方法去计算，通常是用EPA算法，但通常这相当于增加1倍还多的工作量，效率不行
            // Xeno之所以好的原因是，他在检测碰撞的同时，也获得了用来计算碰撞信息的必要数据，碰撞检测完毕的同时，碰撞信息顺手也算出来了。而GJK检测完碰撞后留下的数据，却不足以计算出碰撞信息
            // 核心逻辑：构建单纯形（四面体），试图让单纯形包含原点。构建过程是可以描述为，一个点如果不在一个四面体内，说明它至少和某个顶点，以该顶点的对边为分水岭，呈对立关系。
            // 我们就是不断找出那个对立的顶点，然后把他取反，继续构建单纯形，并检测是否包含原点。如此循环直到包含原点，或证明无论如何都不可能包含原点为止。
            bool DetectCollision(out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3,
                out Vector3 vertex4, // 四面体的四个顶点
                out Vector3 axisDirection, out Vector3 rAxisDirection, // 每次用来获得Support Point的正反方向
                out Vector3 supportPoint, out Vector3 rSupportPoint, // 通过正反方向获得形状1、2的各自的最远点
                out Vector3 vertex2Vertex, out Vector3 origin2Vertex, // 顶点到顶点（边），原点到顶点
                out Vector3 tmp1, out Vector3 tmp2, out Vector3 tmp3, out Vector3 tmp4) // 临时变量
            {
                vertex1 = default;
                vertex2 = default;
                vertex3 = default;
                vertex4 = default;

                axisDirection = default;
                rAxisDirection = default;
                supportPoint = default;
                rSupportPoint = default;
                vertex2Vertex = default;
                origin2Vertex = default;

                tmp1 = default;
                tmp2 = default;
                tmp3 = default;
                tmp4 = default;

                bool isCollide;

                // 顶点1 ------------------- 只有首次使用中心轴来作为起始轴方向
                support1.SupportCenter(out supportPoint);
                support2.SupportCenter(out rSupportPoint);
                Vector3.Subtract(ref rSupportPoint, ref supportPoint, out vertex1);

                // 提前判断相交 ------ 如果两个形状中心重叠，我们就认为他是相交的
                if (vertex1.IsZero()) {
                    return true;
                }

                // 得到顶点2 ---------------------
                Vector3.Copy(ref vertex1, out axisDirection);
                if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
                        out vertex2, out isCollide)) {
                    return isCollide;
                }

                Vector3.Negate(ref vertex1, out origin2Vertex);
                Vector3.Subtract(ref vertex2, ref vertex1, out vertex2Vertex);

                // 确定顶点3方向，顶点3的方向为原点对于v2v1线段的另一侧，这样取得的支持点才是原点这一侧的
                // 比如v2v1直直朝前，原点在线段左侧，tmp1就是下侧，方向就是线段右侧（其中下侧和右侧都是以原点和线段组成的平面为参考）
                Vector3.Cross(ref vertex2Vertex, ref origin2Vertex, out tmp1);
                Vector3.Cross(ref vertex2Vertex, ref tmp1, out axisDirection);

                // 得到顶点3 ---------------------
                if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
                        out vertex3, out isCollide)) {
                    return isCollide;
                }

                // 确定顶点4方向。结合顶点3方向，如果顶点3在面的下方，那么说明原点是在面的上方，所以反着取方向，就是下方，就是tmp1
                Vector3.Subtract(ref vertex3, ref vertex1, out vertex2Vertex);
                if (Vector3.Dot(ref vertex2Vertex, ref tmp1) > 0) {
                    Vector3.Copy(ref tmp1, out axisDirection);
                }
                else {
                    Vector3.NegateCopy(ref tmp1, out axisDirection);
                }

                // 得到顶点4 ---------------------
                if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
                        out vertex4, out isCollide)) {
                    return isCollide;
                }

                // ------------------------到这里，四面体的四个顶点都找好了，找这四个顶点的逻辑为
                // 当只有两个顶点的线段时，找原点那一侧的支持点为第三个顶点，形成面
                // 当有三个顶点时的面时，找原点那一侧的支持点为第四个顶点，形成体
                // 这个四面体的特性是：1、要么直接包含原点 2、要么不包含，但原点在以顶点1向顶点234形成的面的放射范围内，不会出这个范围

                // 下面就开始检测原点是否在四面体内，同时到这里也是GJK检测法和Xeno检测法的分水岭

                #region GJK检测法

                // 下面就用GJK检测法来做
                // 第一步：判断原点是否在四面体内
                // 是：退出
                // 否：向v2v3v4面的法线获得支持点，用该支持点顶替v1
                // 使用v1作为main点，检测原点是否在v2、v3、v4的另一侧，是，则使用新的支持点顶替掉那个点
                // 直到检测到没有v2、v3、v4点在原点的另一侧

                // 四面体的包含检测逻辑是：确认一个顶点，该点是指向原点方向、在明可夫斯基差中距离远的那个点。该点的特性就在于，如果四面体不包含原点，那肯定不是我的锅（至少暂时不是），
                // 因为原点肯定在我两侧，不会在我对面。这个点，我们称它为main顶点，而这个点就是main点对立面的法线支持点，第一个main点是中心点差
                // 明确该点后，我们只需要连续判断另外三个顶点，就可以不断找出导致四面体不包含的罪魁祸首，然后把他取反，它就变成了新的main顶点，如此循环

                // 第一次是特殊的，因为第一个四面体的第一个点是我们直接指定的中心点差，所以这个四面体，如果不包含原点，那么只有一种可能，就是这第一个点反了，把它取反后，再进入正常检测环节
                if (!EliminateVertex(ref vertex4, ref vertex3, ref vertex2, ref vertex1, ref axisDirection, out tmp1,
                        out tmp2, out tmp3, out tmp4)) {
                    if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
                            out vertex1, out isCollide)) {
                        return isCollide;
                    }
                }
                else {
                    // 相交
                    return true;
                }

                // 对于正方体来说，最多3次就检测出来了，一般都是1次检出，越是复杂的形状，可能用到的循环越多
                // 测试时，可以把 i 限制在0、1、2……来一步步查看变化过程
                // vertex1永远作为 main 点，其他点轮番着上来被检测剔除。
                for (int i = 0; i < k_maximumIterations; i++) {
                    if (!EliminateVertex(ref vertex1, ref vertex4, ref vertex3, ref vertex2, ref axisDirection,
                            out tmp1, out tmp2, out tmp3, out tmp4)) {
                        Vector3.Copy(ref vertex1, out vertex2);
                        if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
                                out vertex1, out isCollide)) {
                            return isCollide;
                        }

                        continue;
                    }

                    if (!EliminateVertex(ref vertex1, ref vertex3, ref vertex2, ref vertex4, ref axisDirection,
                            out tmp1, out tmp2, out tmp3, out tmp4)) {
                        Vector3.Copy(ref vertex1, out vertex4);
                        if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
                                out vertex1, out isCollide)) {
                            return isCollide;
                        }

                        continue;
                    }

                    if (!EliminateVertex(ref vertex1, ref vertex2, ref vertex4, ref vertex3, ref axisDirection,
                            out tmp1, out tmp2, out tmp3, out tmp4)) {
                        Vector3.Copy(ref vertex1, out vertex3);
                        if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
                                out vertex1, out isCollide)) {
                            return isCollide;
                        }

                        continue;
                    }

                    // Debug.Log($"第{i + 1}次循环，得出结论：相交；   是否顺便计算了渗透信息：{calCollisionInfo}");
                    isCollide = true;
                    break;
                }

                return isCollide;

                #endregion
            }

            // 通过SupportPoint得到顶点，同时，每次得到顶点时，都可以做分离轴判断，因为有可能快速结束检测
            bool GetVertex(ref Vector3 direction, out Vector3 reverseDirection, out Vector3 supportPoint,
                out Vector3 rSupportPoint, out Vector3 vertex, out bool isCollide) {
                isCollide = false;

                // 反方向
                Vector3.Negate(ref direction, out reverseDirection);

                // 分别得到正方向和反方向上的 support point
                support1.SupportPoint(ref direction, out supportPoint);
                support2.SupportPoint(ref reverseDirection, out rSupportPoint);

                // 相减，得到明可夫斯基差中的一个点，也是我们需要的三角体顶点
                Vector3.Subtract(ref rSupportPoint, ref supportPoint, out vertex);

                // 下面的就是做分离轴检测
                // direction：用来获得SupportPoint的方向，也是轴方向
                // vertex：【形状2在 -axisDirection方向的最远点】 - 【形状1在 axisDirection方向的最远点】
                var dot = Vector3.Dot(ref direction, ref vertex);

                // 如果根据某轴得到的两个最远点的差，和该轴方向是同方向，则可以直接判定为两形状不相交（分离轴定律）；但即使是反方向，也无法确定是否相交，除非……
                if (dot > 0) {
                    // 不相交
                    return true;
                }
                // 除非该轴和该方向平行
                else if (Math.Abs(dot) < s_collideEpsilon) {
                    // 相交
                    isCollide = true;
                    return true;
                }

                // 无法确切判断
                return false;
            }

            // 排除顶点，然后得到一个最可能使四面体包含原点的顶点，目标是剔除vertex4
            bool EliminateVertex(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3,
                ref Vector3 vertex4, ref Vector3 newDirection, //
                out Vector3 tmp1, out Vector3 tmp2, out Vector3 tmp3, out Vector3 tmp4) {
                // vertex1为主点，vertex4为要考察的点，
                Vector3.Subtract(ref vertex2, ref vertex1, out tmp1);
                Vector3.Subtract(ref vertex3, ref vertex1, out tmp2);
                Vector3.Subtract(ref vertex4, ref vertex1, out tmp3);
                Vector3.Negate(ref vertex1, out tmp4);

                Vector3.Cross(ref tmp1, ref tmp2, out tmp1);

                var dot1 = Vector3.Dot(ref tmp3, ref tmp1);
                var dot2 = Vector3.Dot(ref tmp4, ref tmp1);

                // 说明原点在边上，那就说明相交
                if (Math.Abs(dot2) < Fixp.Epsilon) {
                    return true;
                }

                // 说明原点在vertex4的对边的另一侧，vertex4应当被剔除
                if (Math.Sign(dot1) != Math.Sign(dot2)) {
                    if (dot1 > 0) {
                        newDirection = tmp1;
                    }
                    else {
                        newDirection = -tmp1;
                    }

                    return false;
                }

                // 说明原点在vertex4的对边的同一侧，说明包含原点，相交
                return true;
            }
        }

        /// <summary>
        /// 形状与形状碰撞检测（基于Xeno算法，Xeno是一种综合来说更好的碰撞算法，相比GJK）
        /// <para>GJK：只能算碰撞，不能计算碰撞信息，需要搭配EPA渗透算法一起使用。单使用GJK计算碰撞的话，确实比Xeno要快一点点，但那是不计算渗透的前提下，EPA很耗时的，特别是3D中的EPA，即便做了特殊优化，也是无法赶上Xeno</para>>
        /// <para>Xeno：计算碰撞的同时，就把用于计算渗透所需要的参数得到了，且效率和单纯的计算GJK差不多。如果不要求计算碰撞信息的话，还能提前退出，效率会比单纯的GJK还快</para>>
        /// <para>为了更快，检测中没有使用列表之类的数据结构，以及Simplex，且用了很多局部方法</para>>
        /// </summary>
        /// <param name="support1"></param>
        /// <param name="support2"></param>
        /// <param name="collisionNormal"></param>
        /// <param name="collisionPoint"></param>
        /// <param name="collisionPenetration"></param>
        /// <returns></returns>
        public static bool ShapeCollision(ISupportPoint support1, ISupportPoint support2,
            out Vector3 collisionNormal, out Vector3 collisionPoint, out FLOAT collisionPenetration) {
            var result = DetectCollision(out var ver1, out var ver2, out var ver3, out var ver4, //
                out var axisDir, out var rAxisDir, //
                out collisionNormal, out collisionPoint, out collisionPenetration, //
                out var temp1, out var temp2, out var temp3, out var tempVertex);

            return result;

            // 检测碰撞
            bool DetectCollision(out Vertex vertex1, out Vertex vertex2, out Vertex vertex3,
                out Vertex vertex4, // 四面体的四个顶点
                out Vector3 axisDirection, out Vector3 rAxisDirection, // 每次用来获得Support Point的正反方向
                out Vector3 normal, out Vector3 point, out FLOAT penetration, // 碰撞信息
                out Vector3 tmp1, out Vector3 tmp2, out Vector3 tmp3, out Vertex tmpVertex) // 临时变量
            {
                normal = default;
                point = default;
                penetration = default;

                vertex1 = default;
                vertex2 = default;
                vertex3 = default;
                vertex4 = default;

                axisDirection = default;
                rAxisDirection = default;

                tmp1 = default;
                tmp2 = default;
                tmp3 = default;
                tmpVertex = default;

                var isCollide = false;
                var swap = false;
                var cycleCount = 0;

                // 顶点1 ------------------- 只有首次使用中心轴来作为起始轴方向
                support1.SupportCenter(out vertex1.supportPoint1);
                support2.SupportCenter(out vertex1.supportPoint2);
                Vector3.Subtract(ref vertex1.supportPoint2, ref vertex1.supportPoint1, out vertex1.value);

                // 提前判断相交 ------ 如果两个形状中心重叠，我们就认为他是相交的
                if (vertex1.value.IsZero()) {
                    point = (vertex1.supportPoint1 + vertex1.supportPoint2) * Fixp.Half;
                    normal = vertex1.value * Fixp.Epsilon;
                    return true;
                }

                // 得到顶点2 ---------------------
                Vector3.Copy(ref vertex1.value, out axisDirection);
                if (GetVertex(ref axisDirection, out rAxisDirection, out vertex2, //
                        ref normal, ref point, ref penetration, ref isCollide)) {
                    return isCollide;
                }

                // 0 - vertex1   和   v2 - v1
                Vector3.Negate(ref vertex1.value, out tmp1);
                Vector3.Subtract(ref vertex2.value, ref vertex1.value, out tmp2);

                // 确定顶点3方向，顶点3的方向为原点对于v2v1线段的另一侧，这样取得的支持点才是原点这一侧的
                // 比如v2v1直直朝前，原点在线段左侧，tmp1就是下侧，方向就是线段右侧（其中下侧和右侧都是以原点和线段组成的平面为参考）
                Vector3.Cross(ref tmp2, ref tmp1, out tmp3);
                Vector3.Cross(ref tmp2, ref tmp3, out axisDirection);

                // 得到顶点3 ---------------------
                if (GetVertex(ref axisDirection, out rAxisDirection, out vertex3, //
                        ref normal, ref point, ref penetration, ref isCollide)) {
                    return isCollide;
                }

                // 确定顶点4方向。结合顶点3方向，如果顶点3在面的下方，那么说明原点是在面的上方，所以反着取方向，就是下方，就是tmp1
                Vector3.Subtract(ref vertex3.value, ref vertex1.value, out tmp1);
                if (Vector3.Dot(ref tmp1, ref tmp3) > 0) {
                    Vector3.Copy(ref tmp3, out axisDirection);
                }
                else {
                    Vector3.NegateCopy(ref tmp3, out axisDirection);
                    swap = true;
                }

                // 得到顶点4 ---------------------
                if (GetVertex(ref axisDirection, out rAxisDirection, out vertex4, //
                        ref normal, ref point, ref penetration, ref isCollide)) {
                    return isCollide;
                }

                // 该步骤的目的是确保，后续使用 （v4 - v2）和（v3 - v2）做叉乘时，结果一定指向明差内部
                if (swap) {
                    SwapVertex(ref vertex3, ref vertex4);
                }

                // ------------------------到这里，四面体的四个顶点都找好了，找这四个顶点的逻辑为
                // 当只有两个顶点的线段时，找原点那一侧的支持点为第三个顶点，形成面
                // 当有三个顶点时的面时，找原点那一侧的支持点为第四个顶点，形成体
                // 这个四面体的特性是：1、要么直接包含原点 2、要么不包含，但原点在以顶点1向顶点234形成的面的放射范围内，不会出这个范围

                // 下面就开始检测原点是否在四面体内，同时到这里也是GJK检测法和Xeno检测法思想的分水岭

                // if (testStep == 1) {
                //     Gizmos.color = Color.red;
                //     Gizmos.DrawSphere(vertex1.ToVector3(), 0.075f);
                //     Gizmos.color = Color.yellow;
                //     Gizmos.DrawSphere(vertex2.ToVector3(), 0.075f);
                //     Gizmos.color = Color.blue;
                //     Gizmos.DrawSphere(vertex3.ToVector3(), 0.075f);
                //     Gizmos.color = Color.green;
                //     Gizmos.DrawSphere(vertex4.ToVector3(), 0.075f);
                //     
                //     Debug.DrawLine(vertex1.ToVector3(), vertex2.ToVector3(), Color.red);
                //     Debug.DrawLine(vertex1.ToVector3(), vertex3.ToVector3(), Color.red);
                //     Debug.DrawLine(vertex1.ToVector3(), vertex4.ToVector3(), Color.red);
                //     Debug.DrawLine(vertex2.ToVector3(), vertex3.ToVector3(), Color.yellow);
                //     Debug.DrawLine(vertex2.ToVector3(), vertex4.ToVector3(), Color.yellow);
                //     Debug.DrawLine(vertex3.ToVector3(), vertex4.ToVector3(), Color.blue);
                // }

                #region Xeno检测法

                // Xeno检测简要步骤
                // 第一步：向v2v3v4面的法线获得支持点，用该支持点顶替v2v3v4中的一个，然后组成新的v2v3v4（顶替的是无法使雷达继续照射到原点的那个顶点）
                // 第二步：检测v2v3v4是否是边缘面（如果该点和任意点的连线与v2v3v4的法线乘垂直关系，则说明是边缘面）
                // 是：退出
                // 否：循环第一步

                while (true) {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogWarning($"{cycleCount + 1}");
#endif
                    // Debug.DrawLine(Vector3.zero, normal.ToVector3(), Color.black);
                    // if (cycleCount == testStep - 2) {
                    //     Gizmos.color = Color.red;
                    //     Gizmos.DrawSphere(vertex1.value.ToVector3(), 0.075f);
                    //     Gizmos.color = Color.yellow;
                    //     Gizmos.DrawSphere(vertex2.value.ToVector3(), 0.075f);
                    //     Gizmos.color = Color.blue;
                    //     Gizmos.DrawSphere(vertex3.value.ToVector3(), 0.075f);
                    //     Gizmos.color = Color.green;
                    //     Gizmos.DrawSphere(vertex4.value.ToVector3(), 0.075f);
                    //
                    //     Debug.DrawLine(vertex1.value.ToVector3(), vertex2.value.ToVector3(), Color.red);
                    //     Debug.DrawLine(vertex1.value.ToVector3(), vertex3.value.ToVector3(), Color.red);
                    //     Debug.DrawLine(vertex1.value.ToVector3(), vertex4.value.ToVector3(), Color.red);
                    //     Debug.DrawLine(vertex2.value.ToVector3(), vertex3.value.ToVector3(), Color.yellow);
                    //     Debug.DrawLine(vertex2.value.ToVector3(), vertex4.value.ToVector3(), Color.yellow);
                    //     Debug.DrawLine(vertex3.value.ToVector3(), vertex4.value.ToVector3(), Color.blue);
                    // }

                    // 获得新支持点的方向，也就是顶点234组成的面的法线，需要确保，该法线必须是指向v1的方向，而上面的swap保证了这一点
                    Vector3.Subtract(ref vertex3.value, ref vertex2.value, out tmp1);
                    Vector3.Subtract(ref vertex4.value, ref vertex2.value, out tmp2);
                    Vector3.Cross(ref tmp1, ref tmp2, out axisDirection);

                    // 首先，在还没有确定是否碰撞前，每次都判断一下是否发生碰撞，只要证明原点和顶点1在顶点234组成的面的同一侧就可以证明相交
                    // 我们每次得到的axisDirection都是指向明差内部的，所以只要证明（原点 - vertex2）和 axisDirection 是同向或平行的，就可以证明相交
                    if (isCollide == false) {
                        if (Vector3.Dot(ref vertex2.value, ref axisDirection) <= 0) {
                            isCollide = true;
                            // 如果不需要计算碰撞信息，那么到这里就可以提前结束了
                        }
                    }

                    // 现在，获得新的顶点，先临时用 tmpVertex 代替，后面在决定用这个顶点去顶替哪个顶点
                    GetVertexOnly(ref axisDirection, out rAxisDirection, out tmpVertex);

                    // if (cycleCount == testStep - 2) {
                    //     Gizmos.color = Color.black;
                    //     Gizmos.DrawSphere(tmpVertex.value.ToVector3(), 0.075f);
                    // }

                    // 继续，检测该顶点是否无效，如果该点在顶点234组成的面上，说明该顶点无效，自然顶点234组成的面就是边缘面
                    Vector3.Subtract(ref tmpVertex.value, ref vertex2.value, out tmp3);
                    var dot = Vector3.Dot(ref tmp3, ref axisDirection);
                    if (dot >= 0 || cycleCount++ > k_maximumIterations) {
                        // 渗透法线就是顶点234组成的面的法线的反向，但该法线的长度并不是渗透深度
                        Vector3.NegateCopy(ref axisDirection, out normal);
                        normal.Normalize();

                        // 渗透深度 = 明差点 在 法线上投影的长度。只有边缘面的法线，得到的才是真的渗透深度
                        penetration = Vector3.Dot(ref tmpVertex.value, ref normal);

                        // 碰撞点就是两个形状在碰撞法线上的“支持点”的中点
                        // 现在，知道渗透法线了。直接得到两个支持点，就可以得出碰撞点了，但有个问题，对于正方体，获得的支持点经常不是我们认为的正确的点
                        // 到不是支持点出问题了，因为对于正方体，比如（1,0,0）方向的最远点，整个右面都是符合的支持点，但显然不是每个点都可以用来求碰撞点的
                        // 对于不是正好是（1,0,0）（0,1,0）之类的方向，取的支持点是可用的，反之，取的点就不能使用。
                        // 同时，也不能使用明差点在法线上的投影的中点来作为碰撞点，因为法线只是方向，没有坐标，不能用来确定点的位置
                        // 以下摘抄 Xeno方法，为什么能算出来，我也不清楚
                        if (isCollide) {
                            // 计算原点的重心坐标
                            Vector3.Cross(ref vertex2.value, ref vertex4.value, out tmp1);
                            Vector3.Dot(ref tmp1, ref vertex3.value, out var b0);
                            Vector3.Cross(ref vertex3.value, ref vertex4.value, out tmp1);
                            Vector3.Dot(ref tmp1, ref vertex1.value, out var b1);
                            Vector3.Cross(ref vertex1.value, ref vertex2.value, out tmp1);
                            Vector3.Dot(ref tmp1, ref vertex3.value, out var b2);
                            Vector3.Cross(ref vertex4.value, ref vertex2.value, out tmp1);
                            Vector3.Dot(ref tmp1, ref vertex1.value, out var b3);

                            var sum = b0 + b1 + b2 + b3;

                            if (sum <= 0) {
                                b0 = 0;
                                Vector3.Cross(ref vertex4.value, ref vertex3.value, out tmp1);
                                Vector3.Dot(ref tmp1, ref normal, out b1);
                                Vector3.Cross(ref vertex3.value, ref vertex2.value, out tmp1);
                                Vector3.Dot(ref tmp1, ref normal, out b2);
                                Vector3.Cross(ref vertex2.value, ref vertex4.value, out tmp1);
                                Vector3.Dot(ref tmp1, ref normal, out b3);

                                sum = b1 + b2 + b3;
                            }

                            var inv = Fixp.One / sum;

                            Vector3.Multiply(ref vertex1.supportPoint1, b0, out point);
                            Vector3.Multiply(ref vertex2.supportPoint1, b1, out tmp1);
                            Vector3.Add(ref point, ref tmp1, out point);
                            Vector3.Multiply(ref vertex4.supportPoint1, b2, out tmp1);
                            Vector3.Add(ref point, ref tmp1, out point);
                            Vector3.Multiply(ref vertex3.supportPoint1, b3, out tmp1);
                            Vector3.Add(ref point, ref tmp1, out point);

                            Vector3.Multiply(ref vertex1.supportPoint2, b0, out tmp1);
                            Vector3.Add(ref tmp1, ref point, out point);
                            Vector3.Multiply(ref vertex2.supportPoint2, b1, out tmp1);
                            Vector3.Add(ref point, ref tmp1, out point);
                            Vector3.Multiply(ref vertex4.supportPoint2, b2, out tmp1);
                            Vector3.Add(ref point, ref tmp1, out point);
                            Vector3.Multiply(ref vertex3.supportPoint2, b3, out tmp1);
                            Vector3.Add(ref point, ref tmp1, out point);

                            Vector3.Multiply(ref point, inv * Fixp.Half, out point);
                        }

                        // Gizmos.color = Color.red;
                        // Gizmos.DrawSphere(vertex1.value.ToVector3(), 0.075f);
                        // Gizmos.color = Color.yellow;
                        // Gizmos.DrawSphere(vertex2.value.ToVector3(), 0.075f);
                        // Gizmos.color = Color.blue;
                        // Gizmos.DrawSphere(vertex3.value.ToVector3(), 0.075f);
                        // Gizmos.color = Color.green;
                        // Gizmos.DrawSphere(vertex4.value.ToVector3(), 0.075f);
                        //
                        // Debug.DrawLine(vertex1.value.ToVector3(), vertex2.value.ToVector3(), Color.red);
                        // Debug.DrawLine(vertex1.value.ToVector3(), vertex3.value.ToVector3(), Color.red);
                        // Debug.DrawLine(vertex1.value.ToVector3(), vertex4.value.ToVector3(), Color.red);
                        // Debug.DrawLine(vertex2.value.ToVector3(), vertex3.value.ToVector3(), Color.yellow);
                        // Debug.DrawLine(vertex2.value.ToVector3(), vertex4.value.ToVector3(), Color.yellow);
                        // Debug.DrawLine(vertex3.value.ToVector3(), vertex4.value.ToVector3(), Color.blue);

                        break;
                    }

                    // 得到新顶点和顶点1的法线
                    // 这里思路是，如果原点相对于四面体的某个面和剩下的顶点呈对立关系，那么该点就是要被取缔的点。同时，Xeno特性，v1点永不被取缔
                    Vector3.Cross(ref tmpVertex.value, ref vertex1.value, out tmp1);
                    dot = Vector3.Dot(ref tmp1, ref vertex2.value);

                    // if (cycleCount == testStep - 2) {
                    //     Debug.DrawLine(Vector3.zero, tmp3.ToVector3(), Color.magenta);
                    //     Debug.DrawLine(Vector3.zero, vertex1.ToVector3(), Color.magenta);
                    //     Debug.DrawLine(Vector3.zero, temp1.ToVector3(), Color.magenta);
                    //     Debug.DrawLine(Vector3.zero, vertex2.ToVector3(), Color.yellow);
                    //     Debug.DrawLine(Vector3.zero, vertex3.ToVector3(), Color.blue);
                    //     Debug.DrawLine(Vector3.zero, vertex4.ToVector3(), Color.green);
                    // }

                    if (dot <= Fixp.Zero) {
                        dot = Vector3.Dot(ref tmp1, ref vertex3.value);

                        if (dot <= Fixp.Zero) {
                            CopyVertex(ref tmpVertex, out vertex2);
                        }
                        else {
                            CopyVertex(ref tmpVertex, out vertex4);
                        }
                    }
                    else {
                        dot = Vector3.Dot(ref tmp1, ref vertex4.value);

                        if (dot <= Fixp.Zero) {
                            CopyVertex(ref tmpVertex, out vertex3);
                        }
                        else {
                            CopyVertex(ref tmpVertex, out vertex2);
                        }
                    }
                }

                return isCollide;

                #endregion
            }

            // 通过SupportPoint得到顶点，同时，每次得到顶点时，都可以做分离轴判断，因为有可能快速结束检测
            bool GetVertex(ref Vector3 direction, out Vector3 reverseDirection, out Vertex vertex,
                ref Vector3 normal, ref Vector3 point, ref FLOAT penetration, ref bool isCollide) {
                // 反方向
                Vector3.Negate(ref direction, out reverseDirection);

                // 分别得到正方向和反方向上的 support point
                support1.SupportPoint(ref direction, out vertex.supportPoint1);
                support2.SupportPoint(ref reverseDirection, out vertex.supportPoint2);

                // 相减，得到明可夫斯基差中的一个点，也是我们需要的三角体顶点
                Vector3.Subtract(ref vertex.supportPoint2, ref vertex.supportPoint1, out vertex.value);

                // 下面的就是做分离轴检测
                // direction：用来获得SupportPoint的方向，也是轴方向
                // vertex：【形状2在 -axisDirection方向的最远点】 - 【形状1在 axisDirection方向的最远点】
                var dot = Vector3.Dot(ref direction, ref vertex.value);

                // 除非该轴和该方向平行
                if (Math.Abs(dot) < s_collideEpsilon) {
                    // 相交
                    // 计算渗透。在这种情况下，渗透很容易计算
                    Vector3.Copy(ref vertex.value, out normal);
                    normal.Normalize();
                    Vector3.Add(ref vertex.supportPoint1, ref vertex.supportPoint2, out var sum);
                    Vector3.Multiply(ref sum, Fixp.Half, out point);
                    penetration = dot;
                    isCollide = true;
                    return true;
                }

                // 如果根据某轴得到的两个最远点的差，和该轴方向是同方向，则可以直接判定为两形状不相交（分离轴定律）；但即使是反方向，也无法确定是否相交，除非……
                if (dot > 0) {
                    // 不相交
                    penetration = dot;
                    isCollide = false;
                    return true;
                }

                // 无法确切判断
                return false;
            }

            // 只得到顶点
            void GetVertexOnly(ref Vector3 direction, out Vector3 reverseDirection, out Vertex vertex) {
                // 反方向
                Vector3.Negate(ref direction, out reverseDirection);

                // 分别得到正方向和反方向上的 support point
                support1.SupportPoint(ref direction, out vertex.supportPoint1);
                support2.SupportPoint(ref reverseDirection, out vertex.supportPoint2);

                // 相减，得到明可夫斯基差中的一个点，也是我们需要的三角体顶点
                Vector3.Subtract(ref vertex.supportPoint2, ref vertex.supportPoint1, out vertex.value);
            }

            // 拷贝顶点
            void CopyVertex(ref Vertex copier, out Vertex target) {
                Vector3.Copy(ref copier.value, out target.value);
                Vector3.Copy(ref copier.supportPoint1, out target.supportPoint1);
                Vector3.Copy(ref copier.supportPoint2, out target.supportPoint2);
            }

            // 交换顶点
            void SwapVertex(ref Vertex v1, ref Vertex v2) {
                Vector3.Swap(ref v1.value, ref v2.value);
                Vector3.Swap(ref v1.supportPoint1, ref v2.supportPoint1);
                Vector3.Swap(ref v1.supportPoint2, ref v2.supportPoint2);
            }
        }

        private struct Vertex {
            public Vector3 value;
            public Vector3 supportPoint1;
            public Vector3 supportPoint2;
        }

        #region 废弃：（GJK算法，废弃是因为其中EPA渗透算法没有Xeno方式快）

        // private static Queue<TrianglePlane> trianglePlanes1 = new Queue<TrianglePlane>();
        // private static Queue<TrianglePlane> trianglePlanes2 = new Queue<TrianglePlane>();

        // /// <summary>
        // /// 形状与形状碰撞检测（基于GJK碰撞算法和EPA渗透算法），相比于Xeno算法，该算法有些失色
        // /// <para>知识点参考 https://blog.hamaluik.ca/posts/building-a-collision-engine-part-1-2d-gjk-collision-detection/</para>>
        // /// </summary>
        // /// <param name="support1"></param>
        // /// <param name="support2"></param>
        // /// <param name="collisionPenetration"></param>
        // /// <param name="collisionPoint"></param>
        // /// <returns></returns>
        // public static bool ShapeCollision(ISupportPoint support1, ISupportPoint support2,
        //     out FixVector3 collisionPenetration, out FixVector3 collisionPoint) {
        //     // 注：下文中，所有牵扯到形状1和形状2做差运算时，均是用2减1
        //
        //     var result = DetectCollision(out var ver1, out var ver2, out var ver3, out var ver4, //
        //         out var axisDir, out var rAxisDir, //
        //         out var supportPt, out var rSupportPt, //
        //         out var ver2Ver, out var ori2Ver, // 
        //         out collisionPenetration, out collisionPoint, out var calCollisionInfo, //
        //         out var temp1, out var temp2, out var temp3, out var temp4);
        //
        //     // 如果碰撞了，且碰撞检测时，没有计算出碰撞信息，则进行信息计算
        //     if (result && !calCollisionInfo) {
        //         // 用下面的方法（EPA）来求渗透比较标准，但也更耗时。目前看，即便计算比较标准的渗透值意义也不大，不如直接用中心点差来的爽快
        //         support1.SupportCenter(out temp1);
        //         support2.SupportCenter(out temp2);
        //         FixVector3.Subtract(ref temp1, ref temp2, out collisionPenetration);
        //
        //         // CalculatePenetration(ref ver1, ref ver2, ref ver3, ref ver4, out collisionPenetration,
        //         //     out collisionPoint);
        //     }
        //
        //     return result;
        //
        //     // 检测碰撞
        //     // tip：isCalCollisionInfo - 是否计算出了碰撞信息。该方法主要检测碰撞，不计算碰撞信息，但如果顺便就可以算出的话，也会计算
        //     // tip：经过测试，其检测速度比Xeno检测法更快速，但问题是，他无法计算碰撞信息，需要单独的方法去计算，通常是用EPA算法，但通常这相当于增加1倍还多的工作量，效率不行
        //     // Xeno之所以好的原因是，他在检测碰撞的同时，也获得了用来计算碰撞信息的必要数据，碰撞检测完毕的同时，碰撞信息顺手也算出来了。而GJK检测完碰撞后留下的数据，却不足以计算出碰撞信息
        //     // 核心逻辑：构建单纯形（四面体），试图让单纯形包含原点。构建过程是可以描述为，一个点如果不在一个四面体内，说明它至少和某个顶点，以该顶点的对边为分水岭，呈对立关系。
        //     // 我们就是不断找出那个对立的顶点，然后把他取反，继续构建单纯形，并检测是否包含原点。如此循环直到包含原点，或证明无论如何都不可能包含原点为止。
        //     bool DetectCollision(out FixVector3 vertex1, out FixVector3 vertex2, out FixVector3 vertex3,
        //         out FixVector3 vertex4, // 四面体的四个顶点
        //         out FixVector3 axisDirection, out FixVector3 rAxisDirection, // 每次用来获得Support Point的正反方向
        //         out FixVector3 supportPoint, out FixVector3 rSupportPoint, // 通过正反方向获得形状1、2的各自的最远点
        //         out FixVector3 vertex2Vertex, out FixVector3 origin2Vertex, // 顶点到顶点（边），原点到顶点
        //         out FixVector3 penetration, out FixVector3 point, out bool isCalCollisionInfo, // 碰撞信息
        //         out FixVector3 tmp1, out FixVector3 tmp2, out FixVector3 tmp3, out FixVector3 tmp4) // 临时变量
        //     {
        //         penetration = default;
        //         point = default;
        //         isCalCollisionInfo = false;
        //
        //         vertex1 = default;
        //         vertex2 = default;
        //         vertex3 = default;
        //         vertex4 = default;
        //
        //         axisDirection = default;
        //         rAxisDirection = default;
        //         supportPoint = default;
        //         rSupportPoint = default;
        //         vertex2Vertex = default;
        //         origin2Vertex = default;
        //
        //         tmp1 = default;
        //         tmp2 = default;
        //         tmp3 = default;
        //         tmp4 = default;
        //
        //         bool isCollide;
        //
        //         // 顶点1 ------------------- 只有首次使用中心轴来作为起始轴方向
        //         support1.SupportCenter(out supportPoint);
        //         support2.SupportCenter(out rSupportPoint);
        //         FixVector3.Subtract(ref rSupportPoint, ref supportPoint, out vertex1);
        //
        //         // Debug.DrawLine(supportPoint.ToVector3(), rSupportPoint.ToVector3(), Color.red);
        //
        //         // 提前判断相交 ------ 如果两个形状中心重叠，我们就认为他是相交的
        //         if (vertex1.IsNearlyZero()) {
        //             point = supportPoint + rSupportPoint * Fixp.Half;
        //             penetration = vertex1 * Fixp.Epsilon;
        //             isCalCollisionInfo = true;
        //             return true;
        //         }
        //
        //         // 得到顶点2 ---------------------
        //         FixVector3.Copy(ref vertex1, out axisDirection);
        //         if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //             out vertex2, //
        //             ref penetration, ref point, out isCollide)) {
        //             isCalCollisionInfo = isCollide;
        //             return isCollide;
        //         }
        //
        //         // 画出用来得到顶点的support point
        //         // Debug.DrawLine(supportPoint.ToVector3(), rSupportPoint.ToVector3(), Color.yellow);
        //
        //         FixVector3.Negate(ref vertex1, out origin2Vertex);
        //         FixVector3.Subtract(ref vertex2, ref vertex1, out vertex2Vertex);
        //
        //         // 确定顶点3方向，顶点3的方向为原点对于v2v1线段的另一侧，这样取得的支持点才是原点这一侧的
        //         // 比如v2v1直直朝前，原点在线段左侧，tmp1就是下侧，方向就是线段右侧（其中下侧和右侧都是以原点和线段组成的平面为参考）
        //         FixVector3.Cross(ref vertex2Vertex, ref origin2Vertex, out tmp1);
        //         FixVector3.Cross(ref vertex2Vertex, ref tmp1, out axisDirection);
        //
        //         // 得到顶点3 ---------------------
        //         if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //             out vertex3, //
        //             ref penetration, ref point, out isCollide)) {
        //             isCalCollisionInfo = isCollide;
        //             return isCollide;
        //         }
        //
        //         // 画出用来得到顶点的方向、以及support point
        //         // Debug.DrawLine(vertex1.ToVector3(), (axisDirection + vertex1).ToVector3(), Color.blue);
        //         // Debug.DrawLine(supportPoint.ToVector3(), rSupportPoint.ToVector3(), Color.blue);
        //
        //         // 确定顶点4方向。结合顶点3方向，如果顶点3在面的下方，那么说明原点是在面的上方，所以反着取方向，就是下方，就是tmp1
        //         FixVector3.Subtract(ref vertex3, ref vertex1, out vertex2Vertex);
        //         if (FixVector3.Dot(ref vertex2Vertex, ref tmp1) > 0) {
        //             FixVector3.Copy(ref tmp1, out axisDirection);
        //         }
        //         else {
        //             FixVector3.NegateCopy(ref tmp1, out axisDirection);
        //         }
        //
        //         // 得到顶点4 ---------------------
        //         if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //             out vertex4, //
        //             ref penetration, ref point, out isCollide)) {
        //             isCalCollisionInfo = isCollide;
        //             return isCollide;
        //         }
        //
        //         // 画出用来得到顶点的方向、以及support point
        //         // Debug.DrawLine(vertex1.ToVector3(), (axisDirection + vertex1).ToVector3(), Color.green);
        //         // Debug.DrawLine(supportPoint.ToVector3(), rSupportPoint.ToVector3(), Color.green);
        //
        //         // ------------------------到这里，四面体的四个顶点都找好了，找这四个顶点的逻辑为
        //         // 当只有两个顶点的线段时，找原点那一侧的支持点为第三个顶点，形成面
        //         // 当有三个顶点时的面时，找原点那一侧的支持点为第四个顶点，形成体
        //         // 这个四面体的特性是：1、要么直接包含原点 2、要么不包含，但原点在以顶点1向顶点234形成的面的放射范围内，不会出这个范围
        //
        //         // 下面就开始检测原点是否在四面体内，同时到这里也是GJK检测法和Xeno检测法的分水岭
        //
        //         #region GJK检测法
        //
        //         // 下面就用GJK检测法来做
        //         // 第一步：判断原点是否在四面体内
        //         // 是：退出
        //         // 否：向v2v3v4面的法线获得支持点，用该支持点顶替v1
        //         // 使用v1作为main点，检测原点是否在v2、v3、v4的另一侧，是，则使用新的支持点顶替掉那个点
        //         // 直到检测到没有v2、v3、v4点在原点的另一侧
        //
        //         // 四面体的包含检测逻辑是：确认一个顶点，该点是指向原点方向、在明可夫斯基差中距离远的那个点。该点的特性就在于，如果四面体不包含原点，那肯定不是我的锅（至少暂时不是），
        //         // 因为原点肯定在我两侧，不会在我对面。这个点，我们称它为main顶点，而这个点就是main点对立面的法线支持点，第一个main点是中心点差
        //         // 明确该点后，我们只需要连续判断另外三个顶点，就可以不断找出导致四面体不包含的罪魁祸首，然后把他取反，它就变成了新的main顶点，如此循环
        //
        //         // 第一次是特殊的，因为第一个四面体的第一个点是我们直接指定的中心点差，所以这个四面体，如果不包含原点，那么只有一种可能，就是这第一个点反了，把它取反后，再进入正常检测环节
        //         if (!EliminateVertex(ref vertex4, ref vertex3, ref vertex2, ref vertex1, ref axisDirection, out tmp1,
        //             out tmp2, out tmp3, out tmp4)) {
        //             if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //                 out vertex1, //
        //                 ref penetration, ref point, out isCollide)) {
        //                 calCollisionInfo = isCollide;
        //                 return isCollide;
        //             }
        //         }
        //         else {
        //             // 相交
        //             return true;
        //         }
        //
        //         // 对于正方体来说，最多3次就检测出来了，一般都是1次检出，越是复杂的形状，可能用到的循环越多
        //         // 测试时，可以把 i 限制在0、1、2……来一步步查看变化过程
        //         // vertex1永远作为 main 点，其他点轮番着上来被检测剔除。
        //         for (int i = 0; i < 7; i++) {
        //             if (!EliminateVertex(ref vertex1, ref vertex4, ref vertex3, ref vertex2, ref axisDirection,
        //                 out tmp1, out tmp2, out tmp3, out tmp4)) {
        //                 FixVector3.Copy(ref vertex1, out vertex2);
        //                 if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //                     out vertex1, //
        //                     ref penetration, ref point, out isCollide)) {
        //                     calCollisionInfo = isCollide;
        //                     return isCollide;
        //                 }
        //
        //                 continue;
        //             }
        //
        //             if (!EliminateVertex(ref vertex1, ref vertex3, ref vertex2, ref vertex4, ref axisDirection,
        //                 out tmp1, out tmp2, out tmp3, out tmp4)) {
        //                 FixVector3.Copy(ref vertex1, out vertex4);
        //                 if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //                     out vertex1, //
        //                     ref penetration, ref point, out isCollide)) {
        //                     calCollisionInfo = isCollide;
        //                     return isCollide;
        //                 }
        //
        //                 continue;
        //             }
        //
        //             if (!EliminateVertex(ref vertex1, ref vertex2, ref vertex4, ref vertex3, ref axisDirection,
        //                 out tmp1, out tmp2, out tmp3, out tmp4)) {
        //                 FixVector3.Copy(ref vertex1, out vertex3);
        //                 if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //                     out vertex1, //
        //                     ref penetration, ref point, out isCollide)) {
        //                     calCollisionInfo = isCollide;
        //                     return isCollide;
        //                 }
        //
        //                 continue;
        //             }
        //
        //             // Debug.Log($"第{i + 1}次循环，得出结论：相交；   是否顺便计算了渗透信息：{calCollisionInfo}");
        //             isCollide = true;
        //             break;
        //         }
        //
        //         // 画出包围原点的四面体
        //         // Gizmos.color = Color.red;
        //         // Gizmos.DrawSphere(vertex1.ToVector3(), 0.075f);
        //         // Gizmos.color = Color.yellow;
        //         // Gizmos.DrawSphere(vertex2.ToVector3(), 0.075f);
        //         // Gizmos.color = Color.blue;
        //         // Gizmos.DrawSphere(vertex3.ToVector3(), 0.075f);
        //         // Gizmos.color = Color.green;
        //         // Gizmos.DrawSphere(vertex4.ToVector3(), 0.075f);
        //         //
        //         // // Debug.DrawLine(FixVector3.Zero.ToVector3(), vertex1.ToVector3(), Color.black);
        //         //
        //         // Debug.DrawLine(vertex1.ToVector3(), vertex2.ToVector3(), Color.red);
        //         // Debug.DrawLine(vertex1.ToVector3(), vertex3.ToVector3(), Color.red);
        //         // Debug.DrawLine(vertex1.ToVector3(), vertex4.ToVector3(), Color.red);
        //         // Debug.DrawLine(vertex2.ToVector3(), vertex3.ToVector3(), Color.yellow);
        //         // Debug.DrawLine(vertex2.ToVector3(), vertex4.ToVector3(), Color.yellow);
        //         // Debug.DrawLine(vertex3.ToVector3(), vertex4.ToVector3(), Color.blue);
        //
        //         // 到这里，碰撞检测部分就完成了。接下来，要进入渗透深度的计算采用的是EPA渗透算法，不同于四面体的顶点数是固定的，EPA里的拓展多面体顶点数是不确定的,
        //         // 所以需要用到列表来计算
        //
        //         return isCollide;
        //
        //         #endregion
        //     }
        //
        //     // 老实人检测法，由于该方法的效率不行，所以也没有做进一步优化，处于废弃状态
        //     // 计算穿透，给定4个初始顶点。核心逻辑，通过不断扩张四面体的四个面，直到找到某个不能扩张的面，且他的距离最小时，结束
        //     // 实际计算中，不需要留住每个面，对于那么既不是最外层的、且距离不比已知最外层的面近的时候，可以直接剔除
        //     void CalculatePenetration(ref FixVector3 vertex1, ref FixVector3 vertex2, ref FixVector3 vertex3,
        //         ref FixVector3 vertex4, out FixVector3 penetration, out FixVector3 point) {
        //         penetration = default;
        //         point = default;
        //
        //         trianglePlanes1.Clear();
        //         trianglePlanes2.Clear();
        //
        //         // 暂时最近的三角面
        //         TrianglePlane minTrianglePlaneMoment = null;
        //         // 最终最近的三角面
        //         TrianglePlane minTrianglePlaneFinal = null;
        //         int circleCount = 0;
        //
        //         // 添加起始三角面。这里添加起始面的逻辑是：因为我们的检测方式包围原点，所以这四个点是必然包围着原点的
        //         support1.SupportCenter(out var center1);
        //         support2.SupportCenter(out var center2);
        //         FixVector3.Subtract(ref center2, ref center1, out var tmp);
        //
        //         if ((vertex1 - tmp).IsNearlyZero()) {
        //             // 对立面
        //             var startTrianglePlane = new TrianglePlane();
        //             startTrianglePlane.Init(support1, support2, ref vertex2, ref vertex3, ref vertex4);
        //             startTrianglePlane.CalculateClosestPoint();
        //             startTrianglePlane.Expanding();
        //             trianglePlanes1.Enqueue(startTrianglePlane);
        //         }
        //         else {
        //             // 除了对立面的其他三个面
        //             var startTrianglePlane = new TrianglePlane();
        //             startTrianglePlane.Init(support1, support2, ref vertex1, ref vertex2, ref vertex3);
        //             startTrianglePlane.CalculateClosestPoint();
        //             startTrianglePlane.Expanding();
        //             trianglePlanes1.Enqueue(startTrianglePlane);
        //             startTrianglePlane = new TrianglePlane();
        //             startTrianglePlane.Init(support1, support2, ref vertex1, ref vertex3, ref vertex4);
        //             startTrianglePlane.CalculateClosestPoint();
        //             startTrianglePlane.Expanding();
        //             trianglePlanes1.Enqueue(startTrianglePlane);
        //             startTrianglePlane = new TrianglePlane();
        //             startTrianglePlane.Init(support1, support2, ref vertex1, ref vertex4, ref vertex2);
        //             startTrianglePlane.CalculateClosestPoint();
        //             startTrianglePlane.Expanding();
        //             trianglePlanes1.Enqueue(startTrianglePlane);
        //         }
        //
        //         // 用来绘图，注意使用会修改 trianglePlanes1 等变量，导致后面的计算不正确，所以注意重置下变量
        //         if (tmpNumber2 == 1) {
        //             while (circleCount++ < tmpNumber) {
        //                 Color color = default;
        //                 switch (circleCount) {
        //                     case 1:
        //                         color = Color.red;
        //                         break;
        //                     case 2:
        //                         color = Color.yellow;
        //                         break;
        //                     case 3:
        //                         color = Color.blue;
        //                         break;
        //                     case 4:
        //                         color = Color.green;
        //                         break;
        //                 }
        //
        //                 foreach (var trianglePlane in trianglePlanes1) {
        //                     trianglePlane.Expanding();
        //                     if (trianglePlane.canExpanding) {
        //                         trianglePlane.DrawBounding(color);
        //                         trianglePlane.DrawPoints(color);
        //                         trianglePlane.DrawVertex(color);
        //                     }
        //                     else {
        //                         trianglePlane.DrawBounding(Color.black);
        //                         trianglePlane.DrawPoints(Color.black);
        //                         trianglePlane.DrawVertex(Color.black, true);
        //                     }
        //
        //                     if (trianglePlane.canExpanding) {
        //                         trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane1);
        //                         trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane2);
        //                         trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane3);
        //                     }
        //                 }
        //
        //                 trianglePlanes1.Clear();
        //                 ObjectHelper.Swap(ref trianglePlanes2, ref trianglePlanes1);
        //             }
        //         }
        //
        //         // 循环计算渗透深度
        //         if (tmpNumber2 == 0) {
        //             while (circleCount++ < 7) {
        //                 foreach (var trianglePlane in trianglePlanes1) {
        //                     trianglePlane.Expanding();
        //                     if (!trianglePlane.canExpanding) {
        //                         trianglePlane.CalculateClosestPoint();
        //                         if (!trianglePlane.outside) {
        //                             // 找到了一个暂时的最近的三角面，但还不能确定他是最近的，要确定，还需要满足两个条件，就是没有竞争者和潜在竞争者
        //                             // 竞争者：不可拓展面，且距离比他近的
        //                             // 潜在竞争者：虽然有拓展面，但距离比他近
        //                             // 一旦我们确定了没有竞争者和潜在竞争者，那么就能确定这个面是最近的三角面
        //                             if (minTrianglePlaneMoment == null) {
        //                                 minTrianglePlaneMoment = trianglePlane;
        //                             }
        //                             else {
        //                                 if (trianglePlane.closestDistance < minTrianglePlaneMoment.closestDistance) {
        //                                     minTrianglePlaneMoment = trianglePlane;
        //                                 }
        //                             }
        //                         }
        //                     }
        //                     else {
        //                         if (minTrianglePlaneMoment == null) {
        //                             trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane1);
        //                             trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane2);
        //                             trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane3);
        //                         }
        //                         else {
        //                             trianglePlane.expendingTrianglePlane1.CalculateClosestPoint();
        //                             if (trianglePlane.closestDistance < minTrianglePlaneMoment.closestDistance) {
        //                                 trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane1);
        //                             }
        //
        //                             trianglePlane.expendingTrianglePlane2.CalculateClosestPoint();
        //                             if (trianglePlane.closestDistance < minTrianglePlaneMoment.closestDistance) {
        //                                 trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane2);
        //                             }
        //
        //                             trianglePlane.expendingTrianglePlane3.CalculateClosestPoint();
        //                             if (trianglePlane.closestDistance < minTrianglePlaneMoment.closestDistance) {
        //                                 trianglePlanes2.Enqueue(trianglePlane.expendingTrianglePlane3);
        //                             }
        //                         }
        //                     }
        //                 }
        //
        //                 Debug.Log(trianglePlanes2.Count);
        //
        //                 // 循环完之后，做个检测，看是否找到了最近面，两个条件：在已经找到一个暂时的最近面的前提下，没有潜在竞争者
        //                 if (minTrianglePlaneMoment != null) {
        //                     if (trianglePlanes2.Count == 0) {
        //                         minTrianglePlaneFinal = minTrianglePlaneMoment;
        //                         Debug.Log($"第{circleCount}次循环，找到了渗透向量");
        //                         break;
        //                     }
        //                 }
        //
        //                 trianglePlanes1.Clear();
        //                 ObjectHelper.Swap(ref trianglePlanes2, ref trianglePlanes1);
        //             }
        //         }
        //
        //         if (minTrianglePlaneFinal != null) {
        //             minTrianglePlaneFinal.DrawLine(Color.white);
        //             minTrianglePlaneFinal.DrawBounding(Color.cyan);
        //             FixVector3.Copy(ref minTrianglePlaneFinal.closestPoint, out penetration);
        //             FixVector3.Add(ref minTrianglePlaneFinal.supportPoint, ref minTrianglePlaneFinal.rSupportPoint,
        //                 out var sum);
        //             FixVector3.Multiply(ref sum, Fixp.Half, out point);
        //         }
        //     }
        //
        //     // 通过SupportPoint得到顶点，同时，每次得到顶点时，都可以做分离轴判断，因为有可能快速结束检测
        //     bool GetVertex(ref FixVector3 direction, out FixVector3 reverseDirection, out FixVector3 supportPoint,
        //         out FixVector3 rSupportPoint, out FixVector3 vertex, ref FixVector3 penetration, ref FixVector3 point,
        //         out bool isCollide) {
        //         isCollide = false;
        //
        //         // 反方向
        //         FixVector3.Negate(ref direction, out reverseDirection);
        //
        //         // 分别得到正方向和反方向上的 support point
        //         support1.SupportPoint(ref direction, out supportPoint);
        //         support2.SupportPoint(ref reverseDirection, out rSupportPoint);
        //
        //         // 相减，得到明可夫斯基差中的一个点，也是我们需要的三角体顶点
        //         FixVector3.Subtract(ref rSupportPoint, ref supportPoint, out vertex);
        //
        //         // 下面的就是做分离轴检测
        //         // direction：用来获得SupportPoint的方向，也是轴方向
        //         // vertex：【形状2在 -axisDirection方向的最远点】 - 【形状1在 axisDirection方向的最远点】
        //         var dot = FixVector3.Dot(ref direction, ref vertex);
        //
        //         // 如果根据某轴得到的两个最远点的差，和该轴方向是同方向，则可以直接判定为两形状不相交（分离轴定律）；但即使是反方向，也无法确定是否相交，除非……
        //         if (dot > 0) {
        //             // 不相交
        //             return true;
        //         }
        //         // 除非该轴和该方向平行
        //         else if (FixMath.Abs(dot) < Fixp.Epsilon) {
        //             // 相交
        //             // 计算渗透。在这种情况下，渗透很容易计算
        //             FixVector3.Copy(ref vertex, out penetration);
        //             FixVector3.Add(ref supportPoint, ref rSupportPoint, out var sum);
        //             FixVector3.Multiply(ref sum, Fixp.Half, out point);
        //
        //             isCollide = true;
        //             return true;
        //         }
        //
        //         // 无法确切判断
        //         return false;
        //     }
        //
        //     // 排除顶点，然后得到一个最可能使四面体包含原点的顶点，目标是剔除vertex4
        //     bool EliminateVertex(ref FixVector3 vertex1, ref FixVector3 vertex2, ref FixVector3 vertex3,
        //         ref FixVector3 vertex4, ref FixVector3 newDirection, //
        //         out FixVector3 tmp1, out FixVector3 tmp2, out FixVector3 tmp3, out FixVector3 tmp4) {
        //         // vertex1为主点，vertex4为要考察的点，
        //         FixVector3.Subtract(ref vertex2, ref vertex1, out tmp1);
        //         FixVector3.Subtract(ref vertex3, ref vertex1, out tmp2);
        //         FixVector3.Subtract(ref vertex4, ref vertex1, out tmp3);
        //         FixVector3.Negate(ref vertex1, out tmp4);
        //
        //         FixVector3.Cross(ref tmp1, ref tmp2, out tmp1);
        //
        //         var dot1 = FixVector3.Dot(ref tmp3, ref tmp1);
        //         var dot2 = FixVector3.Dot(ref tmp4, ref tmp1);
        //
        //         // 说明原点在边上，那就说明相交
        //         if (FixMath.Abs(dot2) < Fixp.Epsilon) {
        //             return true;
        //         }
        //
        //         // 说明原点在vertex4的对边的另一侧，vertex4应当被剔除
        //         if (FixMath.Sign(dot1) != FixMath.Sign(dot2)) {
        //             if (dot1 > 0) {
        //                 newDirection = tmp1;
        //             }
        //             else {
        //                 newDirection = -tmp1;
        //             }
        //
        //             return false;
        //         }
        //
        //         // 说明原点在vertex4的对边的同一侧，说明包含原点，相交
        //         return true;
        //     }
        // }
        //
        // /// <summary>
        // /// 形状与形状碰撞检测（基于Xeno算法，Xeno是一种综合来说更好的碰撞算法，相比GJK）
        // /// <para>GJK：只能算碰撞，不能计算碰撞信息，需要搭配EPA渗透算法一起使用。单使用GJK计算碰撞的话，确实比Xeno要快一点点，但那是不计算渗透的前提下，EPA很耗时的，特别是3D中的EPA，即便做了特殊优化，也是无法赶上Xeno</para>>
        // /// <para>Xeno：计算碰撞的同时，就把用于计算渗透所需要的参数得到了，且效率和单纯的计算GJK差不多。如果不要求计算碰撞信息的话，还能提前退出，效率会比单纯的GJK还快</para>>
        // /// <para>为了更快，检测中没有使用列表之类的数据结构，以及Simplex，且用了很多局部方法</para>>
        // /// </summary>
        // /// <param name="support1"></param>
        // /// <param name="support2"></param>
        // /// <param name="collisionNormal"></param>
        // /// <param name="collisionPoint"></param>
        // /// <param name="collisionPenetration"></param>
        // /// <returns></returns>
        // public static bool ShapeCollision2(ISupportPoint support1, ISupportPoint support2,
        //     out FixVector3 collisionNormal, out FixVector3 collisionPoint, out FLOAT collisionPenetration) {
        //     // 思路解析：
        //     // GJK和Xeno最大的区别是对于明差是否包含原点的判断不同，虽然他俩本质上都是使用明差来判断相交
        //     // GJK采用都是在明差内部不断的获得支持点，组成新的四面体并检测是否包含原点，形式上就像是一个不断移动的吞噬细胞，去捕捉原点
        //     // Xeno采用的是探照灯模式，在确定了一个main点之后，该点就永不改变，转而改变其他三个点，形式上就像一个捕捉原点的探照灯
        //     // 这二者在捕捉原点的速度上是差不多的，当原点离明差边界近的时候，GJK可能会比Xeno少一两次循环（实际测试中，以正方体碰撞为例，最多差距1次循环，且是在少数情况下）
        //     // 但核心区别在于，单纯以捕捉原点为目的的GJK，在捕捉完成后，留下的那个四面体并不能提供更多的作用，而Xeno在捕捉完成后，留下的四面体
        //     // 却可以直接用来求渗透信息，这是因为，该四面体（探照灯）照出的面，一定是明差的边界面。这是由于Xeno的检测条件是，不断缩小探照范围，
        //     // 直到最后无法缩小，无法缩小，代表该面上已经不存在更多的支持点了，那该面一定是边界面
        //
        //     // 注：下文中，所有牵扯到形状1和形状2做差运算时，均是用2减1
        //
        //     var result = DetectCollision(out var ver1, out var ver2, out var ver3, out var ver4, //
        //         out var axisDir, out var rAxisDir, //
        //         out var supportPt, out var rSupportPt, //
        //         // out var ver2Ver, out var ori2Ver, // 
        //         out collisionNormal, out collisionPoint, out collisionPenetration, //
        //         out var temp1, out var temp2, out var temp3, out var temp4);
        //
        //     return result;
        //
        //     // 检测碰撞
        //     bool DetectCollision(out FixVector3 vertex1, out FixVector3 vertex2, out FixVector3 vertex3,
        //         out FixVector3 vertex4, // 四面体的四个顶点
        //         out FixVector3 axisDirection, out FixVector3 rAxisDirection, // 每次用来获得Support Point的正反方向
        //         out FixVector3 supportPoint, out FixVector3 rSupportPoint, // 通过正反方向获得形状1、2的各自的最远点
        //         // out FixVector3 vertex2Vertex, out FixVector3 origin2Vertex, // 顶点到顶点（边），原点到顶点
        //         out FixVector3 normal, out FixVector3 point, out FLOAT penetration, // 碰撞信息
        //         out FixVector3 tmp1, out FixVector3 tmp2, out FixVector3 tmp3, out FixVector3 tmp4) // 临时变量
        //     {
        //         normal = default;
        //         point = default;
        //         penetration = default;
        //
        //         vertex1 = default;
        //         vertex2 = default;
        //         vertex3 = default;
        //         vertex4 = default;
        //
        //         axisDirection = default;
        //         rAxisDirection = default;
        //         supportPoint = default;
        //         rSupportPoint = default;
        //         // vertex2Vertex = default;
        //         // origin2Vertex = default;
        //
        //         tmp1 = default;
        //         tmp2 = default;
        //         tmp3 = default;
        //         tmp4 = default;
        //
        //         bool isCollide = false;
        //         bool swap = false;
        //
        //         // 顶点1 ------------------- 只有首次使用中心轴来作为起始轴方向
        //         support1.SupportCenter(out supportPoint);
        //         support2.SupportCenter(out rSupportPoint);
        //         FixVector3.Subtract(ref rSupportPoint, ref supportPoint, out vertex1);
        //
        //         // 提前判断相交 ------ 如果两个形状中心重叠，我们就认为他是相交的
        //         if (vertex1.IsNearlyZero()) {
        //             point = supportPoint + rSupportPoint * Fixp.Half;
        //             normal = vertex1 * Fixp.Epsilon;
        //             return true;
        //         }
        //
        //         // 得到顶点2 ---------------------
        //         FixVector3.Copy(ref vertex1, out axisDirection);
        //         if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //             out vertex2, //
        //             ref normal, ref point, ref penetration, ref isCollide)) {
        //             return isCollide;
        //         }
        //
        //         // 0 - vertex1   和   v2 - v1
        //         FixVector3.Negate(ref vertex1, out tmp1);
        //         FixVector3.Subtract(ref vertex2, ref vertex1, out tmp2);
        //
        //         // 确定顶点3方向，顶点3的方向为原点对于v2v1线段的另一侧，这样取得的支持点才是原点这一侧的
        //         // 比如v2v1直直朝前，原点在线段左侧，tmp1就是下侧，方向就是线段右侧（其中下侧和右侧都是以原点和线段组成的平面为参考）
        //         FixVector3.Cross(ref tmp2, ref tmp1, out tmp3);
        //         FixVector3.Cross(ref tmp2, ref tmp3, out axisDirection);
        //
        //         // 得到顶点3 ---------------------
        //         if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //             out vertex3, //
        //             ref normal, ref point, ref penetration, ref isCollide)) {
        //             return isCollide;
        //         }
        //
        //         // 确定顶点4方向。结合顶点3方向，如果顶点3在面的下方，那么说明原点是在面的上方，所以反着取方向，就是下方，就是tmp1
        //         FixVector3.Subtract(ref vertex3, ref vertex1, out tmp1);
        //         if (FixVector3.Dot(ref tmp1, ref tmp3) > 0) {
        //             FixVector3.Copy(ref tmp3, out axisDirection);
        //         }
        //         else {
        //             FixVector3.NegateCopy(ref tmp3, out axisDirection);
        //             swap = true;
        //         }
        //
        //         // 得到顶点4 ---------------------
        //         if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //             out vertex4, //
        //             ref normal, ref point, ref penetration, ref isCollide)) {
        //             return isCollide;
        //         }
        //
        //         // 该步骤的目的是确保，后续使用 （v4 - v2）和（v3 - v2）做叉乘时，结果一定指向明差内部
        //         if (swap) {
        //             FixVector3.Swap(ref vertex3, ref vertex4);
        //         }
        //
        //         // ------------------------到这里，四面体的四个顶点都找好了，找这四个顶点的逻辑为
        //         // 当只有两个顶点的线段时，找原点那一侧的支持点为第三个顶点，形成面
        //         // 当有三个顶点时的面时，找原点那一侧的支持点为第四个顶点，形成体
        //         // 这个四面体的特性是：1、要么直接包含原点 2、要么不包含，但原点在以顶点1向顶点234形成的面的放射范围内，不会出这个范围
        //
        //         // 下面就开始检测原点是否在四面体内，同时到这里也是GJK检测法和Xeno检测法的分水岭
        //
        //         // if (tmpNumber3 == 1) {
        //         //     Gizmos.color = Color.red;
        //         //     Gizmos.DrawSphere(vertex1.ToVector3(), 0.075f);
        //         //     Gizmos.color = Color.yellow;
        //         //     Gizmos.DrawSphere(vertex2.ToVector3(), 0.075f);
        //         //     Gizmos.color = Color.blue;
        //         //     Gizmos.DrawSphere(vertex3.ToVector3(), 0.075f);
        //         //     Gizmos.color = Color.green;
        //         //     Gizmos.DrawSphere(vertex4.ToVector3(), 0.075f);
        //         //     
        //         //     Debug.DrawLine(vertex1.ToVector3(), vertex2.ToVector3(), Color.red);
        //         //     Debug.DrawLine(vertex1.ToVector3(), vertex3.ToVector3(), Color.red);
        //         //     Debug.DrawLine(vertex1.ToVector3(), vertex4.ToVector3(), Color.red);
        //         //     Debug.DrawLine(vertex2.ToVector3(), vertex3.ToVector3(), Color.yellow);
        //         //     Debug.DrawLine(vertex2.ToVector3(), vertex4.ToVector3(), Color.yellow);
        //         //     Debug.DrawLine(vertex3.ToVector3(), vertex4.ToVector3(), Color.blue);
        //         // }
        //
        //         #region Xeno检测法
        //
        //         // Xeno检测简要步骤
        //         // 第一步：向v2v3v4面的法线获得支持点，用该支持点顶替v2v3v4中的一个，然后组成新的v2v3v4（顶替的是无法使雷达继续照射到原点的那个顶点）
        //         // 第二步：检测v2v3v4是否是边缘面（如果该点和任意点的连线与v2v3v4的法线乘垂直关系，则说明是边缘面）
        //         // 是：退出
        //         // 否：循环第一步
        //
        //         int cycleCount = 0;
        //         while (true) {
        //             // Debug.LogWarning($"{cycleCount + 1}");
        //             // Debug.DrawLine(Vector3.zero, normal.ToVector3(), Color.black);
        //             // if (cycleCount == tmpNumber3 - 2) {
        //             //     Gizmos.color = Color.red;
        //             //     Gizmos.DrawSphere(vertex1.ToVector3(), 0.075f);
        //             //     Gizmos.color = Color.yellow;
        //             //     Gizmos.DrawSphere(vertex2.ToVector3(), 0.075f);
        //             //     Gizmos.color = Color.blue;
        //             //     Gizmos.DrawSphere(vertex3.ToVector3(), 0.075f);
        //             //     Gizmos.color = Color.green;
        //             //     Gizmos.DrawSphere(vertex4.ToVector3(), 0.075f);
        //             //
        //             //     Debug.DrawLine(vertex1.ToVector3(), vertex2.ToVector3(), Color.red);
        //             //     Debug.DrawLine(vertex1.ToVector3(), vertex3.ToVector3(), Color.red);
        //             //     Debug.DrawLine(vertex1.ToVector3(), vertex4.ToVector3(), Color.red);
        //             //     Debug.DrawLine(vertex2.ToVector3(), vertex3.ToVector3(), Color.yellow);
        //             //     Debug.DrawLine(vertex2.ToVector3(), vertex4.ToVector3(), Color.yellow);
        //             //     Debug.DrawLine(vertex3.ToVector3(), vertex4.ToVector3(), Color.blue);
        //             // }
        //
        //             // 获得新支持点的方向，也就是顶点234组成的面的法线，需要确保，该法线不能和顶点1是反方向
        //             FixVector3.Subtract(ref vertex3, ref vertex2, out tmp1);
        //             FixVector3.Subtract(ref vertex4, ref vertex2, out tmp2);
        //             FixVector3.Cross(ref tmp1, ref tmp2, out axisDirection);
        //
        //             // 首先，在还没有确定是否碰撞前，每次都判断一下是否发生碰撞，只要证明原点和顶点1在顶点234组成的面的同一侧就可以证明相交
        //             // 我们每次得到的axisDirection都是指向明差内部的，所以只要证明（原点 - vertex2）和 axisDirection 是同向或平行的，就可以证明相交
        //             if (isCollide == false) {
        //                 if (FixVector3.Dot(ref vertex2, ref axisDirection) <= 0) {
        //                     isCollide = true;
        //                     // 如果不需要计算碰撞信息，那么到这里就可以提前结束了
        //                 }
        //             }
        //
        //             // 现在，获得新的顶点，先临时用tmp3代替，后面在决定用这个顶点去顶替哪个顶点
        //             if (GetVertex(ref axisDirection, out rAxisDirection, out supportPoint, out rSupportPoint, //
        //                 out tmp3, //
        //                 ref normal, ref point, ref penetration, ref isCollide)) {
        //                 return isCollide;
        //             }
        //
        //             // if (cycleCount == tmpNumber3 - 2) {
        //             //     Gizmos.color = Color.black;
        //             //     Gizmos.DrawSphere(tmp3.ToVector3(), 0.075f);
        //             // }
        //
        //             // 继续，检测该顶点是否无效，如果该点在顶点234组成的面上，说明该顶点无效，自然顶点234组成的面就是边缘面
        //             FixVector3.Subtract(ref tmp3, ref vertex2, out tmp4);
        //             var dot = FixVector3.Dot(ref tmp4, ref axisDirection);
        //             if (dot >= 0 || cycleCount++ > 10) {
        //                 // 渗透法线就是顶点234组成的面的法线的反向，但该法线的长度并不是渗透深度
        //                 FixVector3.NegateCopy(ref axisDirection, out normal);
        //                 normal.Normalize();
        //
        //                 // 渗透深度 = 明差点 在 法线上投影的长度。只有边缘面的法线，得到的才是真的渗透深度
        //                 penetration = FixVector3.Dot(ref tmp3, ref normal);
        //                 
        //                 if (isCollide) {
        //                 }
        //
        //                 break;
        //             }
        //
        //             // 得到新顶点和顶点1的法线
        //             // 这里思路是，如果原点相对于四面体的某个面和剩下的顶点呈对立关系，那么该点就是要被取缔的点。同时，Xeno特性，v1点永不被取缔
        //             // 简化公式可得
        //             FixVector3.Cross(ref tmp3, ref vertex1, out temp1);
        //             dot = FixVector3.Dot(ref temp1, ref vertex2);
        //
        //             // if (cycleCount == tmpNumber3 - 2) {
        //             //     Debug.DrawLine(Vector3.zero, tmp3.ToVector3(), Color.magenta);
        //             //     Debug.DrawLine(Vector3.zero, vertex1.ToVector3(), Color.magenta);
        //             //     Debug.DrawLine(Vector3.zero, temp1.ToVector3(), Color.magenta);
        //             //     Debug.DrawLine(Vector3.zero, vertex2.ToVector3(), Color.yellow);
        //             //     Debug.DrawLine(Vector3.zero, vertex3.ToVector3(), Color.blue);
        //             //     Debug.DrawLine(Vector3.zero, vertex4.ToVector3(), Color.green);
        //             // }
        //
        //             if (dot <= Fixp.Zero) {
        //                 dot = FixVector3.Dot(ref temp1, ref vertex3);
        //
        //                 if (dot <= Fixp.Zero) {
        //                     FixVector3.Copy(ref tmp3, out vertex2);
        //                 }
        //                 else {
        //                     FixVector3.Copy(ref tmp3, out vertex4);
        //                 }
        //             }
        //             else {
        //                 dot = FixVector3.Dot(ref temp1, ref vertex4);
        //
        //                 if (dot <= Fixp.Zero) {
        //                     FixVector3.Copy(ref tmp3, out vertex3);
        //                 }
        //                 else {
        //                     FixVector3.Copy(ref tmp3, out vertex2);
        //                 }
        //             }
        //         }
        //
        //         return isCollide;
        //
        //         #endregion
        //     }
        //
        //     // 通过SupportPoint得到顶点，同时，每次得到顶点时，都可以做分离轴判断，因为有可能快速结束检测
        //     bool GetVertex(ref FixVector3 direction, out FixVector3 reverseDirection, out FixVector3 supportPoint,
        //         out FixVector3 rSupportPoint, out FixVector3 vertex, ref FixVector3 normal, ref FixVector3 point,
        //         ref FLOAT penetration, ref bool isCollide) {
        //         // 反方向
        //         FixVector3.Negate(ref direction, out reverseDirection);
        //
        //         // 分别得到正方向和反方向上的 support point
        //         support1.SupportPoint(ref direction, out supportPoint);
        //         support2.SupportPoint(ref reverseDirection, out rSupportPoint);
        //
        //         // 相减，得到明可夫斯基差中的一个点，也是我们需要的三角体顶点
        //         FixVector3.Subtract(ref rSupportPoint, ref supportPoint, out vertex);
        //
        //         // 下面的就是做分离轴检测
        //         // direction：用来获得SupportPoint的方向，也是轴方向
        //         // vertex：【形状2在 -axisDirection方向的最远点】 - 【形状1在 axisDirection方向的最远点】
        //         var dot = FixVector3.Dot(ref direction, ref vertex);
        //
        //         // 除非该轴和该方向平行
        //         if (FixMath.Abs(dot) < Fixp.Epsilon) {
        //             // 相交
        //             // 计算渗透。在这种情况下，渗透很容易计算
        //             FixVector3.Copy(ref vertex, out normal);
        //             normal.Normalize();
        //             FixVector3.Add(ref supportPoint, ref rSupportPoint, out var sum);
        //             FixVector3.Multiply(ref sum, Fixp.Half, out point);
        //             penetration = dot;
        //             isCollide = true;
        //             return true;
        //         }
        //
        //         // 如果根据某轴得到的两个最远点的差，和该轴方向是同方向，则可以直接判定为两形状不相交（分离轴定律）；但即使是反方向，也无法确定是否相交，除非……
        //         if (dot > 0) {
        //             // 不相交
        //             penetration = dot;
        //             isCollide = false;
        //             return true;
        //         }
        //
        //         // 无法确切判断
        //         return false;
        //     }
        // }
        //
        // /// <summary>
        // /// 三角面 - 用于计算碰撞渗透
        // /// </summary>
        // internal class TrianglePlane {
        //     public FixVector3 point0;
        //     public FixVector3 point1;
        //     public FixVector3 point2;
        //     public FixVector3 normal;
        //
        //     public bool canExpanding;
        //     public FLOAT closestDistance;
        //     public FixVector3 closestPoint;
        //
        //     public TrianglePlane expendingTrianglePlane1;
        //     public TrianglePlane expendingTrianglePlane2;
        //     public TrianglePlane expendingTrianglePlane3;
        //
        //     public FixVector3 segment1;
        //     public FixVector3 segment2;
        //     public FixVector3 segment3;
        //
        //     public ISupportPoint support1;
        //     public ISupportPoint support2;
        //
        //     public FixVector3 supportPoint;
        //     public FixVector3 rSupportPoint;
        //
        //     public bool outside;
        //     public bool isExcuteExpanded;
        //     public bool isExcuteClosestPoint;
        //
        //     public FixVector3 direction;
        //     public FixVector3 vertex;
        //
        //     public TrianglePlane parent;
        //
        //     public void Init(ISupportPoint supportPoint1, ISupportPoint supportPoint2, ref FixVector3 p0,
        //         ref FixVector3 p1, ref FixVector3 p2) {
        //         canExpanding = false;
        //         closestDistance = 0;
        //         closestPoint = default;
        //
        //         expendingTrianglePlane1 = null;
        //         expendingTrianglePlane2 = null;
        //         expendingTrianglePlane3 = null;
        //
        //         supportPoint = default;
        //         rSupportPoint = default;
        //
        //         outside = false;
        //         isExcuteExpanded = false;
        //         isExcuteClosestPoint = false;
        //
        //         direction = default;
        //         vertex = default;
        //
        //         parent = null;
        //
        //         this.support1 = supportPoint1;
        //         this.support2 = supportPoint2;
        //
        //         FixVector3.Copy(ref p0, out point0);
        //         FixVector3.Copy(ref p1, out point1);
        //         FixVector3.Copy(ref p2, out point2);
        //
        //         FixVector3.Subtract(ref p1, ref p0, out segment1);
        //         FixVector3.Subtract(ref p2, ref p0, out segment2);
        //         FixVector3.Cross(ref segment1, ref segment2, out normal);
        //         FixVector3.Normalize(ref normal);
        //         FixVector3.NegateCopy(ref p0, out segment3);
        //     }
        //
        //     /// <summary>
        //     /// 计算最近距离
        //     /// </summary>
        //     /// <returns></returns>
        //     public void CalculateClosestPoint() {
        //         isExcuteClosestPoint = true;
        //         var constant = normal.Dot(point0);
        //
        //         var num = normal.Dot(FixVector3.Zero) - constant;
        //         closestPoint = FixVector3.Zero - num * normal;
        //         closestDistance = FixMath.Abs(num);
        //
        //         var cross1 = FixVector3.Cross(ref segment1, ref segment2);
        //         var cross2 = FixVector3.Cross(segment1, closestPoint - point0);
        //         if (FixVector3.Dot(cross1.normalized, cross2.normalized) + 1 < Fixp.Epsilon) {
        //             outside = true;
        //         }
        //
        //         cross1 = FixVector3.Cross(ref segment2, ref segment1);
        //         cross2 = FixVector3.Cross(segment2, closestPoint - point0);
        //         if (FixVector3.Dot(cross1.normalized, cross2.normalized) + 1 < Fixp.Epsilon) {
        //             outside = true;
        //         }
        //
        //         cross1 = FixVector3.Cross(point2 - point1, point0 - point1);
        //         cross2 = FixVector3.Cross(point2 - point1, closestPoint - point1);
        //         if (FixVector3.Dot(cross1.normalized, cross2.normalized) + 1 < Fixp.Epsilon) {
        //             outside = true;
        //         }
        //
        //         if (outside) {
        //             closestDistance = Fixp.MaxValue;
        //             closestPoint = default;
        //         }
        //     }
        //
        //     /// <summary>
        //     /// 拓展三角面
        //     /// </summary>
        //     /// <returns>true代表可以拓展，并返回三个三角面，false代表不能拓展</returns>
        //     public void Expanding() {
        //         isExcuteExpanded = true;
        //         FixVector3.Cross(ref segment1, ref segment2, out direction);
        //         if (FixVector3.Dot(ref segment3, ref direction) < 0) {
        //             FixVector3.Negate(ref direction, out direction);
        //         }
        //
        //         // 反方向
        //         FixVector3.Negate(ref direction, out var reverseDirection);
        //
        //         // 分别得到正方向和反方向上的 support point
        //         support1.SupportPoint(ref direction, out supportPoint);
        //         support2.SupportPoint(ref reverseDirection, out rSupportPoint);
        //
        //         // 相减，得到明可夫斯基差中的一个点，也是我们需要的三角体顶点
        //         FixVector3.Subtract(ref rSupportPoint, ref supportPoint, out vertex);
        //
        //         FixVector3.Subtract(ref vertex, ref point0, out var diff);
        //         var dot = FixVector3.Dot(ref diff, ref direction);
        //         if (FixMath.Abs(dot) < Fixp.Epsilon) {
        //             // 这是边界，不可拓展
        //             canExpanding = false;
        //         }
        //         else {
        //             // 得到拓展的那三个面
        //             expendingTrianglePlane1 = new TrianglePlane();
        //             expendingTrianglePlane1.Init(support1, support2, ref vertex, ref point0, ref point1);
        //             expendingTrianglePlane1.parent = this;
        //             expendingTrianglePlane2 = new TrianglePlane();
        //             expendingTrianglePlane2.Init(support1, support2, ref vertex, ref point1, ref point2);
        //             expendingTrianglePlane2.parent = this;
        //             expendingTrianglePlane3 = new TrianglePlane();
        //             expendingTrianglePlane3.Init(support1, support2, ref vertex, ref point2, ref point0);
        //             expendingTrianglePlane3.parent = this;
        //             // 这不是边界，可拓展
        //             canExpanding = true;
        //         }
        //     }
        //
        //     public void DrawBounding(Color color) {
        //         Debug.DrawLine(point0.ToVector3(), point1.ToVector3(), color);
        //         Debug.DrawLine(point1.ToVector3(), point2.ToVector3(), color);
        //         Debug.DrawLine(point2.ToVector3(), point0.ToVector3(), color);
        //     }
        //
        //     public void DrawPoints(Color color) {
        //         Gizmos.color = color;
        //         Gizmos.DrawSphere(point0.ToVector3(), 0.055f);
        //         Gizmos.DrawSphere(point1.ToVector3(), 0.055f);
        //         Gizmos.DrawSphere(point2.ToVector3(), 0.055f);
        //     }
        //
        //     public void DrawVertex(Color color, bool line = false) {
        //         if (!isExcuteExpanded) {
        //             Debug.LogError("错误，还未执行拓展");
        //         }
        //
        //         Gizmos.color = color;
        //         Gizmos.DrawCube(vertex.ToVector3(), Vector3.one * 0.075f);
        //         if (line) {
        //             Debug.DrawLine(vertex.ToVector3(), point0.ToVector3(), color);
        //         }
        //     }
        //
        //     public void DrawLine(Color color) {
        //         if (!isExcuteClosestPoint) {
        //             Debug.LogError("错误，还未执行最近点");
        //         }
        //
        //         Gizmos.color = color;
        //         Gizmos.DrawCube(closestPoint.ToVector3(), Vector3.one * 0.025f);
        //         Debug.DrawLine(Vector3.zero, closestPoint.ToVector3(), color);
        //     }
        //
        //     public override bool Equals(object obj) {
        //         if (obj is TrianglePlane trianglePlane) {
        //             var v1 = trianglePlane.point0 + trianglePlane.point1 + trianglePlane.point2;
        //             var v2 = point0 + point1 + point2;
        //             if (v1 == v2) {
        //                 return true;
        //             }
        //         }
        //
        //         return false;
        //     }
        // }
        //
        // /// <summary>
        // /// 单纯形。
        // /// 单纯形的由来：我们计算碰撞时，使用的是【明可夫斯基差】来判断，即，两个形状的【明可夫斯基差】如果包含原点，那么两者肯定相交，
        // /// 但为了性能考虑，我们又不想每次都把【明可夫斯基差】完全算出来，再去判断是否包含原点，因为【明可夫斯基差】是一个多边形，所以我们
        // /// 在计算【明可夫斯基】的过程中，就不停的去根据已知的点来组成形状，去判断是否包含原点，如果包含了，那就不需要继续计算【明可夫斯基差】
        // /// 了。而这个可能包含原点的四面体就是单纯体
        // /// </summary>
        // internal class Simplex {
        //     internal FixVector3 vertex1;
        //     internal FixVector3 vertex2;
        //     internal FixVector3 vertex3;
        //     internal FixVector3 vertex4;
        //
        //     public void GetVertex(int index) { }
        // }
        //
        // /// <summary>
        // /// 明可夫斯基和。
        // /// </summary>
        // public static void MinkowskiSum(FixVector3[] vertices1, FixVector3[] vertices2, out FixVector3[] result,
        //     out int[] indices, out int dimension) {
        //     result = new FixVector3[vertices1.Length * vertices2.Length];
        //     int index = 0;
        //     for (int i = 0; i < vertices1.Length; i++) {
        //         for (int j = 0; j < vertices2.Length; j++) {
        //             FixVector3.Add(ref vertices1[i], ref vertices2[j], out result[index++]);
        //         }
        //     }
        //
        //     ConvexHull.Create3D(result, out indices, out dimension);
        // }
        //
        // /// <summary>
        // /// 明可夫斯基差。
        // /// <para>改变两个形状的位置，不影响明可夫斯基差的形状</para>>
        // /// <para>两个形状的中心点的差，一定也是他们明可夫斯基差的中心点</para>>
        // /// </summary>
        // public static void MinkowskiDifference(FixVector3[] vertices1, FixVector3[] vertices2, out FixVector3[] result,
        //     out int[] indices, out int dimension) {
        //     result = new FixVector3[vertices1.Length * vertices2.Length];
        //     int index = 0;
        //
        //     // 明可夫斯基差，就是一个形状的每个顶点和另一个形状的每个顶点的差的合集组成的凸包
        //     for (int i = 0; i < vertices1.Length; i++) {
        //         for (int j = 0; j < vertices2.Length; j++) {
        //             FixVector3.Subtract(ref vertices1[i], ref vertices2[j], out result[index++]);
        //         }
        //     }
        //
        //     ConvexHull.Create3D(result, out indices, out dimension);
        // }

        #endregion
    }
}