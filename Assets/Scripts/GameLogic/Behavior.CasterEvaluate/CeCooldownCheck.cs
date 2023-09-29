using Hsenl.casterevaluate;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class CeCooldownCheck : CeInfo<CooldownCheckInfo> {
        protected override NodeStatus OnNodeTick() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    if (!ability.IsCooldown) {
                        this.manager.status = CastEvaluateStatus.Cooldown;
                        return NodeStatus.Failure;
                    }

                    return NodeStatus.Success;
                }
            }

            return NodeStatus.Success;
        }
    }
}