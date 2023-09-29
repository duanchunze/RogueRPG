namespace Hsenl {
    public static class NumericConst {
        public const int NumericTypeOffset = 16;
        public const int NumericTypeMax = (1 << NumericTypeOffset) - 1;
        public const int NodeLayerOffset = 2;
        // 同时层数的最大值也不能大于数值类型的最大值, 即 NodeLayerMax < NumericTypeMax, 不过也几乎不可能有这么多层
        public const int NodeLayerMax = (1 << NodeLayerOffset) - 1;
    }
}