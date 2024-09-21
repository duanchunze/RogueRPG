using System.Collections;
using Hsenl.bolt;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpHarmOfDibozhanBolt : TpHarm<timeline.HarmOfDibozhanBoltInfo> {
        private Faction _faction;
        private BoltConfig _boltConfig;

        private UnityEngine.Collider[] _buffer = new UnityEngine.Collider[1];

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
                    var constrainsTags = this._faction?.GetTagsOfFactionTypes(ability.factionTypes);
                    var dir = ability.targets[0].transform.Position - this.harmable.transform.Position;
                    dir.Normalize();

                    Coroutine.Start(this.Fire(dir, constrainsTags));
                    break;
                }
            }
        }

        private IEnumerator Fire(Vector3 dir, IReadOnlyBitlist constrainsTags) {
            var bolt = BoltManager.Instance.Rent(this._boltConfig, false);
            var collider = bolt.GetComponent<Collider>(true);
            collider.SetUsage(GameColliderPurpose.Detection);
            var listener = CollisionEventListener.Get(collider.Entity);
            var counter = 0;
            var maxNum = this.info.Num;
            var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
            var origin = this.harmable.transform.Position;

            listener.onTriggerEnter = col => {
                if (col.Bodied.MainBodied == this.harmable.Bodied.MainBodied)
                    return;

                if (!col.Tags.ContainsAny(constrainsTags))
                    return;

                var hurtable = col.GetComponent<Hurtable>();
                if (hurtable == null)
                    return;

                this.Harm(hurtable, this.info.HarmFormula);

                Shortcut.InflictionStatus(this.harmable.Bodied, hurtable.Bodied, StatusAlias.Jitui, 0.1f);
            };

            yield return new WhenBreak(() => { BoltManager.Instance.Return(bolt); });

            while (counter < this.info.Num) {
                var num = counter + 1;
                if (num > maxNum)
                    num = maxNum;
                var pos = origin + dir * num * this.info.InternalDistance;
                bolt.transform.Position = pos;
                bolt.transform.LocalScale = collider.transform.LocalScale;
                bolt.transform.Forward = dir;
                bolt.Entity.Active = true;
                
                var c = Physics.OverlapSphereNonAlloc(pos, 0.1f, this._buffer, 1 << Constant.ObstaclesLayer);
                if (c > 0) {
                    maxNum = num;
                }

                yield return new WaitSeconds((int)(this.info.InternalTime * 1000));

                bolt.Entity.Active = false;
                counter++;
            }
        }
    }
}