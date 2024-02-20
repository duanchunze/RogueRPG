using FixedMath;

namespace Hsenl {
    /// <summary>
    /// 形状
    /// <para>仅包含几何图形相关的信息</para>>
    /// <para>都是最原始的几何数据</para>>
    /// <para>形象比喻shape和collider的关系为：shape为无生命体，仅代表几何，没有旋转、位移；而collider代表一个有生命的个体，有旋转、位移等参数</para>>
    /// </summary>
    public abstract class FixShape : IBroadPhase, INarrowPhase {
        /// <summary>
        /// 未被处理过的包围盒
        /// </summary>
        internal AABB _boundingBox;

        /// <summary>
        /// 未被处理过的盒体
        /// </summary>
        internal Box _box;

        /// <summary>
        /// 几何中心
        /// </summary>
        internal FVector3 _geometricCenter;

        public abstract FittingDegreeType fittingDegreeType { get; }
        public abstract void SupportPoint(ref FVector3 direction, out FVector3 point);

        public virtual void SupportCenter(out FVector3 center) {
            center = this._geometricCenter;
        }

        public virtual AABB boundingBox => _boundingBox;
        public virtual Box box => _box;
    }
}