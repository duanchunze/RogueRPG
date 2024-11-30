using System;
using System.Collections;
using Hsenl.bolt;
using MemoryPack;

namespace Hsenl {
    // 例如奶妈的q
    [MemoryPackable]
    public partial class TpHarmOfPointBolt : TpHarm<timeline.HarmOfPointBoltInfo> {
        private Faction _faction;
        private string _boltBundleName;

        private BoltConfig _boltConfig;

        protected override void OnEnable() {
            base.OnEnable();
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._faction = owner?.GetComponent<Faction>();
                    this._boltConfig = Tables.Instance.TbBoltConfig.GetByAlias(this.info.BoltConfigAlias);
                    break;
                }
            }
        }

        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    if (ability.targets.Count == 0)
                        break;

                    var target = ability.targets[0];

                    var constrainsTags = this._faction?.GetTagsOfFactionTypes(ability.factionTypes);
                    var bolt = BoltManager.Instance.Rent(this._boltConfig, false);
                    var tspd = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tspd);
                    float speed = tspd + this.info.Speed;
                    if (speed <= 0) {
                        throw new Exception($"bolt speed is 0 or less of 0 '{speed}'");
                    }

                    var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
                    Coroutine.Start(this.FireToPoint(
                        bolt,
                        this.harmable.transform.Position,
                        target.transform.Position,
                        speed,
                        tsize,
                        constrainsTags)
                    );

                    break;
                }
            }
        }

        private IEnumerator FireToPoint(Bolt bolt, Vector3 origin, Vector3 dst, float speed, float tsize, IReadOnlyBitlist constrainsTags) {
            var dis = Vector3.Distance(origin, dst);
            var duration = dis / speed;
            var elapsedTime = 0f;
            var stayTime = 0.1f;

            var config = bolt.Config;
            var size = config.Size.ToVector3();
            bolt.transform.Position = origin;
            bolt.transform.LocalScale = size * tsize;
            bolt.Entity.Active = true;

            var collider = bolt.GetComponent<Collider>(polymorphic: true);
            collider.Enable = false;
            collider.transform.Position = origin;
            collider.transform.LocalScale = new Vector3(tsize, tsize, tsize);
            collider.IsTrigger = true;
            collider.SetUsage(GameColliderPurpose.Detection);
            var listener = CollisionEventListener.Get(collider.Entity);
            listener.onTriggerEnter = col => {
                if (col.Bodied.MainBodied == this.harmable.Bodied.MainBodied)
                    return;

                if (!col.Tags.ContainsAny(constrainsTags))
                    return;

                var hurtable = col.GetComponent<Hurtable>();
                if (hurtable == null)
                    return;

                this.Harm(hurtable, this.info.HarmFormula);
            };

            bolt.Entity.Active = true;

            yield return new WhenBreak(() => { BoltManager.Instance.Return(bolt); });

            while (true) {
                if (elapsedTime < duration) {
                    elapsedTime += TimeInfo.DeltaTime;
                    var progress = elapsedTime / duration;

                    bolt.transform.Position = Vector3.Lerp(origin, dst, progress);
                }
                else {
                    // 到达目标点后，将位置设置为目标点
                    bolt.transform.Position = dst;
                    collider.Enable = true;
                    if (stayTime <= 0) {
                        yield break;
                    }
                    else {
                        stayTime -= TimeInfo.DeltaTime;
                    }
                }

                bolt.transform.LocalScale = size * tsize;
                yield return null;
            }
        }
    }
}