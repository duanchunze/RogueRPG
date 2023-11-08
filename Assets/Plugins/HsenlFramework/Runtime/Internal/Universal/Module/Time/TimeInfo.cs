namespace Hsenl {
    public static class TimeInfo {
        public static float DeltaTime => TimeInfoManager.Instance.DeltaTime;
        public static int FrameCount => TimeInfoManager.Instance.FrameCount;
        public static long Now => TimeInfoManager.Instance.Now;
        public static float Time => TimeInfoManager.Instance.Time;

        public static float TimeScale {
            get => TimeInfoManager.Instance.TimeScale;
            set => TimeInfoManager.Instance.TimeScale = value;
        }
    }
}