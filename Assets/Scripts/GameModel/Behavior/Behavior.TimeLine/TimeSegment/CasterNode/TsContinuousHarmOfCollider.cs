using Hsenl.timeline;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TsContinuousHarmOfCollider : TsHarmOfColliderBase<ContinuousHarmOfColliderInfo> {
        private IReadOnlyBitlist _containsTags;

        private float _timer;
        private float _timer2;

        protected override void OnEnable() {
            base.OnEnable();
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.MainBodied;
                    if (owner == null)
                        break;

                    this._containsTags = owner.GetComponent<Faction>()?.GetTagsOfFactionTypes(ability.factionTypes);
                    break;
                }
            }
        }

        protected override string GetColliderName() {
            return this.info.ColliderName;
        }

        protected override Vector3 GetCenter() {
            return new Vector3(this.info.Center.X, this.info.Center.Y, this.info.Center.Z);
        }

        protected override Vector3 GetSize() {
            return new Vector3(this.info.Size.X, this.info.Size.Y, this.info.Size.Z);
        }

        protected override void OnTimeSegmentOrigin() {
            base.OnTimeSegmentOrigin();
            this._timer = this.info.InternalTime;
        }

        protected override void OnTimeSegmentRunning() {
            var deltaTime = this.manager.DeltaTime;
            this._timer += deltaTime;
            if (this._timer > this.info.InternalTime && this.collider.Entity.Active == false) {
                this._timer = 0;
                this._timer2 = 0;

                if (this.collider != null) {
                    this.UpdateTransform();
                    this.collider.Entity.Active = true;
                }
            }
            else {
                if (this._timer2 > 0.016f) {
                    this.collider.Entity.Active = false;
                }
                else {
                    this._timer2 += deltaTime;
                }
            }
        }

        protected override void OnTriggerEnter(Collider other) {
            var bod = other.Bodied;
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
        }
    }
}