using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class AIBehaviorTree : BehaviorTree<AICompositeNode<AIBehaviorTree>>, IUpdate {
        public void Update() {
            this.DeltaTime = TimeInfo.DeltaTime;
            this.Tick();
        }
    }
}