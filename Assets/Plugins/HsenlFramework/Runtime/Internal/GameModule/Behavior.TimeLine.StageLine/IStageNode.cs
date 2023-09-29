using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface IStageNode : INode<ITimeLine> {
        int StageType { get; }
        float Duration { get; }
    }
}