using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 例如剑魔的q
    [Serializable]
    [MemoryPackable()]
    public partial class TsHarmOfCollider : TsHarm<timeline.HarmOfColliderInfo> {
        private Faction _faction;
        private CollisionEventListener _collisionEventListener;
        private Hurtable _targetHurtable;

        protected override void OnNodeOpen() {
            base.OnNodeOpen();
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._faction = ability.Owner?.GetComponent<Faction>();

                    if (this._collisionEventListener == null) {
                        this._collisionEventListener = ability.FindChild<CollisionEventListener>();
                        if (this._collisionEventListener == null) {
                            this._collisionEventListener = ColliderManager.Instance.Rent(this.info.ColliderName, false);
                            this._collisionEventListener.SetParent(this.manager.Bodied.Entity);
                        }

                        this._collisionEventListener.SetUsage(GameColliderPurpose.Detection);

                        this._collisionEventListener.onTriggerEnter = this.OnTriggerEnter;
                    }

                    break;
                }
            }
        }

        private void OnTriggerEnter(Collider collider) {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var bod = collider.Bodied;
                    if (bod == null) return;
                    if (bod == this.manager.Owner) return;
                    var tags = this._faction.GetTagsOfFactionTypes(ability.factionTypes);
                    if (!bod.Tags.ContainsAny(tags)) {
                        return;
                    }

                    var hurtable = bod.GetComponent<Hurtable>();
                    if (hurtable == null) return;

                    this.Harm(hurtable, this.info.HarmFormula);

                    this._targetHurtable ??= hurtable;
                    FxManager.Instance.Play(this.info.HitFx, hurtable.transform.Position);
                    break;
                }
            }
        }

        protected override void OnTimeSegmentOrigin() {
            if (this._collisionEventListener == null) return;
            var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
            if (tsize == 0)
                tsize = 1;

            this._collisionEventListener.transform.LocalScale = Vector3.one * tsize;
            this._targetHurtable = null;
            // 将物理模拟临时改为脚本调用模式, 然后手动进行一次模拟运行, 这样哪怕collider只被开启一帧, 也能检测出来
            var simulationMode = Physics.simulationMode;
            Physics.simulationMode = SimulationMode.Script;
            this._collisionEventListener.Entity.Active = true;
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.simulationMode = simulationMode;

            // 只播放一次, 要不然声音太大了
            this._targetHurtable?.GetComponent<Sound>()?.Play(this.info.HitSound);
        }

        protected override void OnTimeSegmentRunning() { }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            if (this._collisionEventListener.IsInvalid())
                return;

            this._collisionEventListener.Entity.Active = false;
        }
    }
}