namespace Hsenl {
    public enum CastModel {
        Transient, // 转瞬即逝的
        FiniteTime = -1, // 有限时间
        InfiniteTime = -2, // 无限时间的
    }
}