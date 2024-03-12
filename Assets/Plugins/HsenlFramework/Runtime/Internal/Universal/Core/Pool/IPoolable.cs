namespace Hsenl {
    public interface IPoolable {
        public PoolKey PoolKey { get; }
        public void SetPoolKey(PoolKey poolKey);
    }
}