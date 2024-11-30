namespace Hsenl {
    public static class Timer {
        public static async HTask WaitFrame(int frameCount = 1) => await TimerManager.Instance.WaitFrame(frameCount);

        public static async HTask WaitTime(long ms) =>
            await TimerManager.Instance.WaitTime(ms);

        public static async HTask WaitTimeWithScale(float ms) =>
            await TimerManager.Instance.WaitTimeWithScale(ms);

        public static async HTask WaitTillTime(long tillMs) =>
            await TimerManager.Instance.WaitTillTime(tillMs);

        public static async HTask WaitTillTimeWithScale(float tillMs) =>
            await TimerManager.Instance.WaitTillTimeWithScale(tillMs);

        public static bool ClockTick(float timeInternal) => TimerManager.Instance.ClockTick(timeInternal);
    }
}