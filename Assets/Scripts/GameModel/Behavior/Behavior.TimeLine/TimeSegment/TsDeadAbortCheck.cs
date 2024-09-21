using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TsDeadAbortCheck : TsInfo<timeline.DeadAbortCheckInfo> {
        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    if (ability.targets.Count == 0)
                        break; // targets为空代表该技能不是需要目标施法的技能, todo 或者可以用tags来判断, 缺点是容易忘, 以后可以改成用tags来判断
                    
                    var allDead = true;
                    foreach (var target in ability.targets) {
                        var dead = Shortcut.IsDead(target.Bodied);
                        if (!dead) {
                            allDead = false;
                            break;
                        }
                    }

                    if (allDead) {
                        this.manager.Abort();
                    }

                    break;
                }
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) { }
    }
}