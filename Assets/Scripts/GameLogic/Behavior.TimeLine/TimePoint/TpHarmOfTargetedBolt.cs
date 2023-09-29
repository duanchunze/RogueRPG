using System;
using MemoryPack;

namespace Hsenl {
    // 例如ez的平a
    [Serializable]
    [MemoryPackable()]
    public partial class TpHarmOfTargetedBolt : TpHarm<timeline.HarmOfTargetedBoltInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    foreach (var selectionTarget in ability.targets) {
                        var hurtable = selectionTarget.Substantive.GetComponent<Hurtable>();
                        if (hurtable == null) continue;

                        var bolt = BoltManager.Instance.Rent(this.info.ModelName, false);
                        var tspd = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tspd);
                        float speed = tspd + this.info.Speed;
                        if (speed <= 0) {
                            throw new Exception($"bolt speed is 0 or less of 0 '{speed}'");
                        }

                        bolt.FireOfTarget(this.harmable.transform.Position, hurtable.transform, speed, () => {
                            this.Harm(hurtable);
                            FxManager.Instance.Play(this.info.HitFx, hurtable.transform.Position);
                            hurtable.GetComponent<Sound>()?.Play(this.info.HitSound);
                        });
                    }

                    break;
                }
            }
        }
    }
}