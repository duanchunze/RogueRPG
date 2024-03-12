using System;
using System.Collections;
using Hsenl.bolt;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    // 例如ez的平a
    [Serializable]
    [MemoryPackable()]
    public partial class TpHarmOfTargetedBolt : TpHarm<timeline.HarmOfTargetedBoltInfo> {
        private string _boltBundleName;

        private BoltConfig _boltConfig;

        protected override void OnEnable() {
            base.OnEnable();
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._boltConfig = Tables.Instance.TbBoltConfig.GetByAlias(this.info.BoltConfigAlias);
                    break;
                }
            }
        }

        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    foreach (var selectionTarget in ability.targets) {
                        var hurtable = selectionTarget.Bodied.GetComponent<Hurtable>();
                        if (hurtable == null) continue;

                        var bolt = BoltManager.Instance.Rent(this._boltConfig, false);
                        var tspd = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tspd);
                        float speed = tspd + this.info.Speed;
                        if (speed <= 0) {
                            throw new Exception($"bolt speed is 0 or less of 0 '{speed}'");
                        }

                        Coroutine.Start(this.FireOfTargeted(bolt, hurtable, speed));
                    }

                    break;
                }
            }
        }

        private IEnumerator FireOfTargeted(Bolt bolt, Hurtable hurtable, float speed) {
            bolt.transform.Position = this.harmable.transform.Position;
            bolt.Entity.Active = true;

            yield return new WhenBreak(() => {
                BoltManager.Instance.Return(bolt);

                this.Harm(hurtable, this.info.HarmFormula);
            });

            while (true) {
                var dir = math.normalize(hurtable.transform.Position - bolt.transform.Position);
                bolt.transform.Translate(dir * speed * TimeInfo.DeltaTime);
                if (math.distancesq(bolt.transform.Position, hurtable.transform.Position) < 0.1f) {
                    break;
                }

                yield return 0;
            }
        }
    }
}