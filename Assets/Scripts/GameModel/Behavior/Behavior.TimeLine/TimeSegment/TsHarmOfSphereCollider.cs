using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TsHarmOfSphereCollider : TsHarm<timeline.HarmOfSphereColliderInfo> {
        private IReadOnlyBitlist _containsTags;
        private SphereCollider _sphereCollider;

        protected override void OnReset() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.AttachedBodied;
                    if (owner == null)
                        break;

                    this._containsTags = owner.GetComponent<Faction>().GetTagsOfFactionTypes(ability.factionTypes);
                    break;
                }
            }
        }

        protected override void OnTimeSegmentOrigin() {
            this._sphereCollider = ColliderManager.Instance.Rent<SphereCollider>();
            this._sphereCollider.IsTrigger = true;
            this._sphereCollider.Center = this.info.Center.ToUnityVector3();
            this._sphereCollider.Radius = this.info.Radius;
            this._sphereCollider.SetUsage(GameColliderPurpose.Detection);
            var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
            this._sphereCollider.transform.LocalScale = Vector3.one * tsize;

            var eventListener = CollisionEventListener.Get(this._sphereCollider.Entity);
            eventListener.onTriggerEnter = col => {
                var bod = col.Bodied;
                if (bod == null)
                    return;

                if (bod == this.manager.Bodied.AttachedBodied)
                    return;

                if (!bod.Tags.ContainsAny(this._containsTags))
                    return;

                var hurtable = bod.GetComponent<Hurtable>();
                if (hurtable == null)
                    return;

                this.Harm(hurtable, this.info.HarmFormula);
            };

            this._sphereCollider.Entity.Active = true;
        }

        protected override void OnTimeSegmentRunning() {
            if (this._sphereCollider != null) {
                var tran = this.manager.Bodied.transform;
                this._sphereCollider.transform.Position = tran.Position;
                this._sphereCollider.transform.Quaternion = tran.Quaternion;
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            if (this._sphereCollider != null) {
                ColliderManager.Instance.Return(this._sphereCollider);
            }
        }
    }
}