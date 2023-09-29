namespace Hsenl {
    public enum CombinerType {
        SingleCombiner, // 只在单个域内做单组件匹配
        MultiCombiner, // 只在单个域内做多组合匹配
        CrossCombiner, // 在父域与子域之间做混合的多组合匹配
    }
}