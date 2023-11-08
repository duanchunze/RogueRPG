namespace Hsenl {
    public static class Pool {
        public static Object Rent(PoolKey key, Entity parent = null, bool autoActive = true) => PoolManager.Instance.Rent(key, parent, autoActive);

        public static T Rent<T>(PoolKey key, Entity parent = null, bool autoActive = true) where T : Object =>
            PoolManager.Instance.Rent<T>(key, parent, autoActive);

        public static void Return(PoolKey poolKey, Object obj) => PoolManager.Instance.Return(poolKey, obj);
    }
}