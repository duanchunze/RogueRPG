namespace Hsenl {
    public partial struct HTask {
        // 返回一个空壳子, 在Completed判断时, 总是返回true
        public static HTask Completed => default;

        public static HTask Create() {
            return new HTask(HTaskPool.Rent<NormalHTask>());
        }

        public static HTask Create<T>() where T : IHTask, new() {
            return new HTask(HTaskPool.Rent<T>());
        }

        public static HTask<bool> CreateCancelable() {
            return new HTask<bool>(HTaskPool<bool>.Rent<CancelableHTask>());
        }
    }

    public partial struct HTask<T> {
        public static HTask<T> Create() {
            return new HTask<T>(HTaskPool<T>.Rent<NormalHTask<T>>());
        }

        public static HTask<T> Create<TH>() where TH : IHTask<T>, new() {
            return new HTask<T>(HTaskPool<T>.Rent<TH>());
        }
    }
}