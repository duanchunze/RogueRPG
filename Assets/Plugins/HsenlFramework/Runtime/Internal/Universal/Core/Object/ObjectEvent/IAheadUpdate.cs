namespace Hsenl {
    public interface IAheadUpdate {
        int InstanceId { get; }
        bool RealEnable { get; }
        void AheadUpdate();
    }
}