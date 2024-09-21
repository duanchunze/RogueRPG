using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TsHarmOfBoxCollider : TsHarm<timeline.HarmOfBoxColliderInfo> {
        private IReadOnlyBitlist _containsTags;
        private BoxCollider _boxCollider;

        protected override void OnReset() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.MainBodied;
                    if (owner == null)
                        break;

                    this._containsTags = owner.GetComponent<Faction>().GetTagsOfFactionTypes(ability.factionTypes);
                    break;
                }
            }
        }

        protected override void OnTimeSegmentOrigin() {
            this._boxCollider = ColliderManager.Instance.Rent<BoxCollider>(active: false);
            this._boxCollider.IsTrigger = true;
            this._boxCollider.Center = this.info.Center.ToVector3();
            this._boxCollider.Size = this.info.Size.ToVector3();
            this._boxCollider.SetUsage(GameColliderPurpose.Detection);
            var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
            this._boxCollider.transform.LocalScale = Vector3.One * tsize;

            var eventListener = CollisionEventListener.Get(this._boxCollider.Entity);
            eventListener.onTriggerEnter = col => {
                var bod = col.Bodied;
                if (bod == null)
                    return;

                if (bod == this.manager.Bodied.MainBodied)
                    return;

                if (!bod.Tags.ContainsAny(this._containsTags))
                    return;

                var hurtable = bod.GetComponent<Hurtable>();
                if (hurtable == null)
                    return;

                this.Harm(hurtable, this.info.HarmFormula);
            };

            this._boxCollider.Entity.Active = true;
        }

        protected override void OnTimeSegmentRunning() {
            if (this._boxCollider != null) {
                var tran = this.manager.Bodied.transform;
                this._boxCollider.transform.Position = tran.Position;
                this._boxCollider.transform.Quaternion = tran.Quaternion;
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            if (this._boxCollider != null) {
                ColliderManager.Instance.Return(this._boxCollider);
            }
        }
    }
}