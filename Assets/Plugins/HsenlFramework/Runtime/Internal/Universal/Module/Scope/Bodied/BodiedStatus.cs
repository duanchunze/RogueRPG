namespace Hsenl {
    public enum BodiedStatus {
        // Individual同时被称为所有者(owner)
        // Dependent不能被称为所有者, 他有owner的能力, 却没有owner的身份
        // 但Dependent可以通过升级变成Individual, 同样的, Individual也可以降级为Dependent
        
        /// 独立个体
        Individual,
        /// 一个需要依赖于独立个体的依赖者
        Dependent,
    }
}