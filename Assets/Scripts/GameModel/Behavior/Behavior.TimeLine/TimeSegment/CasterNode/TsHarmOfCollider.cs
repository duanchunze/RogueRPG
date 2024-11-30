using Hsenl.timeline;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TsHarmOfCollider : TsHarmOfColliderBase<HarmOfColliderInfo> {
        private IReadOnlyBitlist _containsTags;

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