namespace Hsenl {
    public interface ISupportPoint {
        /// <summary>
        /// 获得指定方向的最远点
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="point"></param>
        void SupportPoint(ref Vector3 direction, out Vector3 point);

        /// <summary>
        /// 获得几何中心
        /// </summary>
        /// <param name="center"></param>
        void SupportCenter(out Vector3 center);
    }
}