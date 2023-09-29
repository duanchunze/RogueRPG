namespace Hsenl {
    public static class TimeInfo {
        public static float DeltaTime => TimeInfoManager.Instance?.DeltaTime ?? 0;
        public static int FrameCount => TimeInfoManager.Instance?.FrameCount ?? 0;
        public static long Now => TimeInfoManager.Instance?.Now ?? 0;
        public static float Time => TimeInfoManager.Instance?.GameTime ?? 0;
    }
}