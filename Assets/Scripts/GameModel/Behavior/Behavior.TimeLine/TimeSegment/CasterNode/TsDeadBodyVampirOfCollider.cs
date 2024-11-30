using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TsDeadBodyVampirOfCollider : TsColliderBase<timeline.DeadBodyVampirOfColliderInfo> {
        private IReadOnlyBitlist _containsTags;
        private ProcedureLine _procedureLine;

        protected override void OnEnable() {
            base.OnEnable();
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.MainBodied;
                    if (owner == null)
                        break;

                    this._containsTags = owner.GetComponent<Faction>()?.GetTagsOfFactionTypes(ability.factionTypes);
                    this._procedureLine = owner.GetComponent<ProcedureLine>();
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

            if (!Shortcut.IsDead(bod))
                return;

            var targetNumerator = bod.GetComponent<Numerator>();
            var vampHp = targetNumerator.GetValue(NumericType.MaxHp) * this.info.Pct;
            if (vampHp < 1)
                vampHp = 1;

            this._procedureLine.StartLine(new PliRecoverForm() {
                inflictor = bod,
                target = this.manager.Bodied.MainBodied,
                source = this.manager.Bodied,
                recoverHp = vampHp,
            });
        }
    }
}