namespace Hsenl.Network {
    public interface IPlug {
        public void Init(IPluggable pluggable);
        public void Dispose();
    }
}