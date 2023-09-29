using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface ITimeLine : IBehaviorTree {
        public float Time { get; set; }
        public float TillTime { get; set; }
        public float Speed { get; set; }
    }

    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface ITimeLine<out T> : ITimeLine, IBehaviorTree<T> where T : ITimeLine<T> { }
}