namespace Hsenl.Network {
    // 提供所有与包相关的支持
    public interface IPacketHandler {
        public void Init();
        public void Init<T>(T t);
        public void Dispose();
    }
}