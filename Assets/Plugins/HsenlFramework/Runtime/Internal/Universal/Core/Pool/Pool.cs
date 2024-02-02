namespace Hsenl {
    public static class Pool {
        public static Object Rent(PoolKey key, Entity parent = null, bool active = true) => PoolManager.Instance.Rent(key, parent, active);

        public static T Rent<T>(PoolKey key, Entity parent = null, bool active = true) where T : Object =>
            PoolManager.Instance.Rent<T>(key, parent, active);

        public static void Return(PoolKey poolKey, Object obj) => PoolManager.Instance.Return(poolKey, obj);
    }
}