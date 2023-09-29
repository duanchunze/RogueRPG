using System;
using MemoryPack;

namespace Hsenl {
    // 例如ez的q
    [MemoryPackable()]
    public partial class TpHarmOfDirectionBolt : TpHarm<timeline.HarmOfDirectionBoltInfo> {
        private Faction _selfFaction;

        protected override void OnNodeOpen() {
            base.OnNodeOpen();
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this._selfFaction = ability.GetHolder()?.GetComponent<Faction>();
                    break;
                }
            }
        }

        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var constrainsTags = this._selfFaction?.GetTagsOfFactionTypes(ability.factionTypes);

                    for (int i = 0, len = ability.targets.Count; i < len; i++) {
                        var target = ability.targets[i];
                        var hurtable = target.Substantive.GetComponent<Hurtable>();
                        if (hurtable == null) continue;

                        var bolt = BoltManager.Instance.Rent(this.info.ModelName, false);
                        var tspd = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tspd);
                        float speed = tspd + this.info.Speed;
                        if (speed <= 0) {
                            throw new Exception($"bolt speed is 0 or less of 0 '{speed}'");
                        }

                        bolt.FireOfDirection(this.harmable.transform.Position, this.harmable.transform.Forward, this.info.Distance, speed, (sub) => {
                            if (sub == this.harmable.Substantive) return;
                            if (!sub.Tags.ContainsAny(constrainsTags)) {
                                return;
                            }

                            var hurt = sub.GetComponent<Hurtable>();
                            if (hurt == null) return;

                            this.Harm(hurt);

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