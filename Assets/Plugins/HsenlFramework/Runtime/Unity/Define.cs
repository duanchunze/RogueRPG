namespace Hsenl
{
	public static class Define
	{
#if UNITY_EDITOR
		public static bool IsEditor = true;
#else
        public static bool IsEditor = false;
#endif
	}
}