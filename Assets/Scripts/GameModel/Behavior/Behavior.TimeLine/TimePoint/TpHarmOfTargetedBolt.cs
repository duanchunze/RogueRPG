using System;
using System.Collections;
using Hsenl.bolt;
using MemoryPack;

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

        protected override async void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    foreach (var selectionTarget in ability.targets) {
                        var hurtable = selectionTarget.Bodied.GetComponent<Hurtable>();
                        if (hurtable == null) continue;

                        var bolt = BoltManager.Instance.Rent(this._boltConfig, false);
                        var tspd = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tspd);
                        var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
                        bolt.transform.LocalScale = new Vector3(tsize);
                        float speed = tspd + this.info.Speed;
                        if (speed <= 0) {
                            throw new Exception($"bolt speed is 0 or less of 0 '{speed}'");
                        }

                        // Coroutine.Start(FireOfTargeted(bolt, this.harmable, hurtable, speed));
                        this.Fire(bolt, hurtable, speed);
                    }

                    break;
                }
            }
        }

        private async void Fire(Bolt bolt, Hurtable hurtable, float speed) {
            await FireToTarget(bolt, hurtable, this.harmable.transform.Position, speed);
            this.Harm(hurtable, this.info.HarmFormula);
        }

        public static async HTask FireToTarget(Bolt bolt, Hurtable hurtable, Vector3 origin, float speed) {
            bolt.transform.Position = origin;
            bolt.Entity.Active = true;
            var dis = Vector3.Distance(bolt.transform.Position, hurtable.transform.Position);
            var time = dis / speed;
            if (time < 0.5f) {
                speed = dis / 0.5f;
            }

            while (true) {
                var dir = hurtable.transform.Position - bolt.transform.Position;
                var norDir = dir.normalized;
                bolt.transform.Translate(norDir * speed * TimeInfo.DeltaTime);
                if (Vector3.DistanceSquared(bolt.transform.Position, hurtable.transform.Position) < 0.1f) {
                    break;
                }

                await Timer.WaitFrame();
            }

            BoltManager.Instance.Return(bolt);
        }

        private static IEnumerator FireOfTargeted(Bolt bolt, Harmable harmable, Hurtable hurtable, float speed) {
            bolt.transform.Position = harmable.transform.Position;
            bolt.Entity.Active = true;

            yield return new WhenBreak(() => {
                BoltManager.Instance.Return(bolt);

                // this.Harm(hurtable, this.info.HarmFormula);
            });

            while (true) {
                var dir = Vector3.Normalize(hurtable.transform.Position - bolt.transform.Position);
                bolt.transform.Translate(dir * speed * TimeInfo.DeltaTime);
                if (Vector3.DistanceSquared(bolt.transform.Position, hurtable.transform.Position) < 0.1f) {
                    break;
                }

                yield return 0;
            }
        }
    }
}