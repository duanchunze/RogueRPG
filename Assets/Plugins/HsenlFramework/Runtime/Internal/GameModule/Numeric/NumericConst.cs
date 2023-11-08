namespace Hsenl {
    public static class NumericConst {
        public const int NumericTypeOffset = 16;
        public const int NumericTypeMaxInTheory = (1 << NumericTypeOffset) - 1; // 理论上的最大数值类型数
        public const int NodeLayerOffset = 3;
        public const int NodeLayerMaxInTheory = (1 << NodeLayerOffset) - 1; // 理论上的最大层数
    }
}