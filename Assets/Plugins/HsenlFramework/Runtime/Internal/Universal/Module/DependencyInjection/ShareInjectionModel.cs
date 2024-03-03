namespace Hsenl {
    public enum ShareInjectionModel {
        Global, // 当前容器内全局共享
        SpecifiedType, // 只在针对指定类型共享
        OnlyInstance, // 只在实例内部共享
    }
}