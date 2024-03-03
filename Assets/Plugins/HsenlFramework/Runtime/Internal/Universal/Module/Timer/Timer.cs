namespace Hsenl {
    public static class Timer {
        public static async HTask WaitFrame() => await TimerManager.Instance.WaitFrame();

        public static async HTask WaitTime(long ms) =>
            await TimerManager.Instance.WaitTime(ms);

        public static async HTask WaitTillTime(long tillMs) =>
            await TimerManager.Instance.WaitTillTime(tillMs);
    }
}