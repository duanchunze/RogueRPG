namespace Hsenl {
    public static class Timer {
        public static async HTask WaitFrame() => await TimerManager.Instance.WaitFrame();

        public static async HTask WaitTime(long time) =>
            await TimerManager.Instance.WaitTime(time);

        public static async HTask WaitTillTime(long tillTime) =>
            await TimerManager.Instance.WaitTillTime(tillTime);
    }
}