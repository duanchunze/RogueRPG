
namespace Hsenl {
    // 需要的时候, 自行调用, 因为比较耗性能, 所以基类不自动调用
    public interface IUpdate {
        int InstanceId { get; }
        bool RealEnable { get; }
        void Update();
    }
}