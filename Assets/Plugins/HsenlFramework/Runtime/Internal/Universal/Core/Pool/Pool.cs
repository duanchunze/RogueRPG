namespace Hsenl {
    public static class Pool {
        public static Object Rent(PoolKey poolKey, Entity parent = null, bool active = true) => PoolManager.Instance.Rent(poolKey, parent, active);
        
        public static T Rent<T>(PoolKey key, Entity parent = null, bool active = true) where T : Object, IPoolable =>
            PoolManager.Instance.Rent<T>(key, parent, active);

        public static void Return(PoolKey key, Object obj) => PoolManager.Instance.Return(key, obj);

        public static void Return(IPoolable poolable) => PoolManager.Instance.Return(poolable);
    }
}