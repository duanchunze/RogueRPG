using System;
using System.Collections;
using System.Collections.Generic;
using Hsenl.bolt;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 例如ez的q
    [MemoryPackable]
    public partial class TpHarmOfDirectionBolt : TpHarm<timeline.HarmOfDirectionBoltInfo> {
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
                    var constrainsTags = this._faction?.GetTagsOfFactionTypes(ability.factionTypes);
                    var bolt = BoltManager.Instance.Rent(this._boltConfig, false);
                    var tspd = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tspd);
                    float speed = tspd + this.info.Speed;
                    if (speed <= 0) {
                        throw new Exception($"bolt speed is 0 or less of 0 '{speed}'");
                    }

                    var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
                    var dir = this.manager.Bodied.MainBodied.transform.Forward;
                    Coroutine.Start(this.FireOfDirection(
                        bolt,
                        this.harmable.transform.Position,
                        dir,
                        this.info.Distance,
                        speed,
                        tsize,
                        constrainsTags)
                    );

                    break;
                }
            }
        }

        private IEnumerator FireOfDirection(Bolt bolt, Vector3 origin, Vector3 dir, float distance, float speed, float tsize, IReadOnlyBitlist constrainsTags) {
            var run = true;

            var config = bolt.Config;
            var size = config.Size.ToVector3();
            bolt.transform.Position = origin;
            bolt.transform.LocalScale = size * tsize;
            var maxDisSq = distance * distance;
            bolt.Entity.Active = true;

            var collider = bolt.GetComponent<Collider>(polymorphic: true);
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
                run = false;
            };
            
            bolt.Entity.Active = true;

            yield return new WhenBreak(() => {
                BoltManager.Instance.Return(bolt);
            });

            while (run) {
                bolt.transform.Translate(dir * (speed * TimeInfo.DeltaTime));
                bolt.transform.LocalScale = size * tsize;
                if (Vector3.DistanceSquared(origin, bolt.transform.Position) > maxDisSq) {
                    yield break;
                }

                yield return null;
            }
        }
    }
}