namespace Hsenl {
    public static class Timer {
        public static async ETTask WaitFrame(ETCancellationToken cancellationToken = null) => await TimerManager.Instance.WaitFrame(cancellationToken);

        public static async ETTask WaitTime(long time, ETCancellationToken cancellationToken = null) =>
            await TimerManager.Instance.WaitTime(time, cancellationToken);

        public static async ETTask WaitTillTime(long tillTime, ETCancellationToken cancellationToken = null) =>
            await TimerManager.Instance.WaitTillTime(tillTime, cancellationToken);
    }
}