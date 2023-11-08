namespace Hsenl {
    // ConfigInfo不同于Config, Config只要知道索引, 就可以明确的拿到该Config, 但ConfigInfo不是一个表, 而是一个Bean, 内容是不确定的, 所以必须拿到其实例
    // 应用: 当我希望某些类中的字段交由配置表去决定的时候
    public interface IConfigInfoInitializer<in T> {
        void InitInfo(T configInfo);
    }
}