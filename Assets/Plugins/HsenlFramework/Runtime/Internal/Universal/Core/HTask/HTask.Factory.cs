namespace Hsenl {
    public partial struct HTask {
        // 返回一个空壳子, 在Completed判断时, 总是返回true
        public static HTask Completed => default;

        public static HTask Create() {
            return new HTask(HTaskPool.Rent<NormalHTaskBody>());
        }

        public static HTask Create<T>() where T : IHTaskBody, new() {
            return new HTask(HTaskPool.Rent<T>());
        }

        public static SwitchToMainThreadPoolAwaitable Returner() => SwitchToMainThreadPoolAwaitable.Create();
    }

    public partial struct HTask<T> {
        public static HTask<T> Create() {
            return new HTask<T>(HTaskPool<T>.Rent<NormalHTaskBody<T>>());
        }

        public static HTask<T> Create<TH>() where TH : IHTaskBody<T>, new() {
            return new HTask<T>(HTaskPool<T>.Rent<TH>());
        }
    }
}