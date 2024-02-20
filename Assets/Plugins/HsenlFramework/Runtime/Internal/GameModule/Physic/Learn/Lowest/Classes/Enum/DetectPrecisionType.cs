namespace Hsenl {
    public enum DetectPrecisionType {
        /// <summary>
        /// 大概检测下，一般是AABB检测
        /// </summary>
        Rough,

        /// <summary>
        /// 尽可能快，如无必要，则不是要高性能消耗的检测
        /// </summary>
        TryFast,

        /// <summary>
        /// 精确，必定使用高性能消耗，且精确的检测
        /// </summary>
        Accurate,
    }
}