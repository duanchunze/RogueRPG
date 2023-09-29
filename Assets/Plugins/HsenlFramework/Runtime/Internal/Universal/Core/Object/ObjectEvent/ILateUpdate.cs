namespace Hsenl {
    public interface ILateUpdate {
        int InstanceId { get; }
        bool RealEnable { get; }
        void LateUpdate();
    }
}