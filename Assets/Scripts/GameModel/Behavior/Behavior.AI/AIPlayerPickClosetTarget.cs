using System.Collections.Generic;
using Hsenl.ai;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class AIPlayerPickClosetTarget : AIInfo<PlayerPickClosestTargetInfo> {
        protected override bool Check() {
            return true;
        }

        protected override void Enter() { }

        protected override void Running() { }

        protected override void Exit() { }
    }
}