namespace Hsenl {
    public static class NumericConst {
        public const int NumericTypeOffset = 16;
        public const int NumericMaxTypeNumInTheory = (1 << NumericTypeOffset) - 1; // 理论上的最大数值类型数
        public const int NumericLayerOffset = 3;
        public const int NumericMaxLayerNumInTheory = (1 << (NumericTypeOffset - NumericLayerOffset)) - 1; // 理论上的最大层数
        public const int NumericMaxModeNumlInTheory = (1 << NumericLayerOffset) - 1;
    }
}