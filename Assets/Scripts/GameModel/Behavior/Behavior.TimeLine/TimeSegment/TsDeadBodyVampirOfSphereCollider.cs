using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TsDeadBodyVampirOfSphereCollider : TsInfo<timeline.DeadBodyVampirOfSphereColliderInfo> {
        protected List<Numerator> numerators;
        private IReadOnlyBitlist _containsTags;
        private SphereCollider _sphereCollider;
        private ProcedureLine _procedureLine;

        protected override void OnReset() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.MainBodied;
                    if (owner == null)
                        break;

                    this.numerators ??= new(2);
                    this.numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    numerator = owner.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);

                    this._containsTags = owner.GetComponent<Faction>().GetTagsOfFactionTypes(ability.factionTypes);
                    this._procedureLine = owner.GetComponent<ProcedureLine>();
                    break;
                }
            }
        }

        protected override void OnTimeSegmentOrigin() {
            this._sphereCollider = ColliderManager.Instance.Rent<SphereCollider>();
            this._sphereCollider.IsTrigger = true;
            this._sphereCollider.Center = Vector3.Zero;
            this._sphereCollider.Radius = this.info.Radius;
            this._sphereCollider.SetUsage(GameColliderPurpose.Detection);
            var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
            this._sphereCollider.transform.LocalScale = Vector3.One * tsize;

            var eventListener = CollisionEventListener.Get(this._sphereCollider.Entity);
            eventListener.onTriggerEnter = col => {
                var bod = col.Bodied;
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