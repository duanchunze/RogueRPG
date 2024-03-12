using System.Collections;
using Hsenl.bolt;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpHarmOfDibozhanBolt : TpHarm<timeline.HarmOfDibozhanBoltInfo> {
        private Faction _faction;
        private BoltConfig _boltConfig;

        protected override void OnEnable() {
            base.OnEnable();
            var owner = this.manager.Bodied.AttachedBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._faction = owner?.GetComponent<Faction>();
                    var boltConfig = Tables.Instance.TbBoltConfig.GetByAlias(this.info.BoltConfigAlias);
                    break;
                }
            }
        }

        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var constrainsTags = this._faction?.GetTagsOfFactionTypes(ability.factionTypes);
                    var dir = ability.targets[0].transform.Position - this.harmable.transform.Position;
                    dir.Normalize();

                    Coroutine.Start(this.Fire(dir, constrainsTags));
                    break;
                }
            }
        }

        private IEnumerator Fire(Vector3 dir, IReadOnlyBitlist constrainsTags) {
            // // var bolt = BoltManager.Instance.Rent(this._boltBundleName, this.info.BoltName, false);
            // var collider = ColliderManager.Instance.Rent(this.info.ColliderName, autoActive: false);
            // collider.SetUsage(GameColliderPurpose.Detection);
            // var listener = CollisionEventListener.Get(collider.Entity);
            // var counter = 0;
            // var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
            //
            // listener.onTriggerEnter = col => {
            //     if (col.AttachedBodied == this.harmable.AttachedBodied)
            //         return;
            //
            //     if (!col.Tags.ContainsAny(constrainsTags))
            //         return;
            //
            //     var hurtable = col.GetComponent<Hurtable>();
            //     if (hurtable == null)
            //         return;
            //
            //     this.Harm(hurtable, this.info.HarmFormula);
            //
            //     Shortcut.InflictionStatus(this.harmable.Bodied, hurtable.Bodied, StatusAlias.Jitui, 0.1f);
            //
            //     FxManager.Instance.Play(this.info.HitFx, hurtable.transform.Position);
            //     SourceEventStation.PlaySound(hurtable.Entity, this.info.HitSound);
            // };
            //
            // yield return new WhenBreak(() => {
            //     ColliderManager.Instance.Return(collider);
            //     // BoltManager.Instance.Return(bolt);
            // });
            //
            // while (counter < this.info.Num) {
            //     var pos = this.harmable.transform.Position + dir * (counter + 1) * this.info.InternalDistance;
            //     collider.transform.Position = pos;
            //     collider.transform.LocalScale = Vector3.one * tsize;
            //     collider.transform.Forward = dir;
            //     // bolt.transform.Position = pos;
            //     // bolt.transform.LocalScale = collider.transform.LocalScale;
            //     // bolt.transform.Forward = dir;
            //     collider.Entity.Active = true;
            //     // bolt.Entity.Active = true;
            //
            //     yield return new WaitSeconds((int)(this.info.InternalTime * 1000));
            //
            //     collider.Entity.Active = false;
            //     // bolt.Entity.Active = false;
            //     counter++;
            // }
            
            yield break;
        }
    }
}