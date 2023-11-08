namespace Hsenl {
    public enum CombinerType {
        MultiCombiner, // 只在单个域内做多组合匹配
        CrossCombiner, // 在父域与子域之间做混合的多组合匹配
    }
}