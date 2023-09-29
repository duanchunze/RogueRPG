namespace Hsenl {
    public enum DropMode {
        /// <summary>
        /// 个数不固定
        /// </summary>
        UnfixedCount,

        /// <summary>
        /// 个数固定
        /// </summary>
        FixedCount,

        /// <summary>
        /// 个数固定, 且每个候选人只会掉落一次
        /// </summary>
        FixedCountNotRepeat,
    }
}