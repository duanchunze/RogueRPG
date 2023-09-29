using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface ITimeNode : INode<ITimeLine> { }
}