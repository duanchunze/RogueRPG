using System;
using System.Collections;
using System.Collections.Generic;
using Hsenl.bolt;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine;
using YooAsset;

namespace Hsenl {
    // 例如ez的q
    [MemoryPackable()]
    public partial class TpHarmOfDirectionBolt : TpHarm<timeline.HarmOfDirectionBoltInfo> {
        private Faction _faction;
        private string _boltBundleName;

        private BoltConfig _boltConfig;

        protected override void OnEnable() {
            base.OnEnable();
            var owner = this.manager.Bodied.AttachedBodied;
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
                    var directions = this.manager.Blackboard.GetOrCreateData<List<Vector3>>("AbilityCastDirections");
                    foreach (var direction in directions) {
                        var bolt = BoltManager.Instance.Rent(this._boltConfig, false);
                        var tspd = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tspd);
                        float speed = tspd + this.info.Speed;
                        if (speed <= 0) {
                            throw new Exception($"bolt speed is 0 or less of 0 '{speed}'");
                        }

                        var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);

                        Coroutine.Start(this.FireOfDirection(
                            bolt,
                            this.harmable.transform.Position,
                            direction,
                            this.info.Distance,
                            speed,
                            tsize,
                            constrainsTags)
                        );
                    }

                    break;
                }
            }
        }

        private IEnumerator FireOfDirection(Bolt bolt, Vector3 origin, Vector3 dir, float distance, float speed, float size, IReadOnlyBitlist constrainsTags) {
            bolt.transform.Position = origin;
            bolt.transform.LocalScale = new Vector3(size, size, size);
            var maxDisSq = distance * distance;
            bolt.Entity.Active = true;

            var collider = ColliderManager.Instance.Rent<SphereCollider>(active: false);
            collider.transform.Position = origin;
            collider.transform.LocalScale = new Vector3(size, size, size);
            collider.IsTrigger = true;
            collider.SetUsage(GameColliderPurpose.Detection);
            var listener = CollisionEventListener.Get(collider.Entity);
            listener.onTriggerEnter = col => {
                if (col.Bodied.AttachedBodied == this.harmable.Bodied.AttachedBodied)
                    return;

                if (!col.Tags.ContainsAny(constrainsTags))
                    return;

                var hurtable = col.GetComponent<Hurtable>();
                if (hurtable == null)
                    return;

                this.Harm(hurtable, this.info.HarmFormula);
            };
            collider.Entity.Active = true;

            yield return new WhenBreak(() => {
                ColliderManager.Instance.Return(collider);
                BoltManager.Instance.Return(bolt);
            });

            while (true) {
                collider.transform.Translate(dir * (speed * TimeInfo.DeltaTime));
                bolt.transform.Position = collider.transform.Position;
                if (math.distancesq(origin, collider.transform.Position) > maxDisSq) {
                    yield break;
                }

                yield return null;
            }
        }
    }
}