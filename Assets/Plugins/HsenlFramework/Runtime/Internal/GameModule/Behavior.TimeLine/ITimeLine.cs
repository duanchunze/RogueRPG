using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface ITimeLine : IBehaviorTree {
        public float StageTime { get; set; }
        public float StageTillTime { get; set; }
        public float Speed { get; set; }
    }

    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface ITimeLine<out T> : ITimeLine, IBehaviorTree<T> where T : ITimeLine<T> { }
}