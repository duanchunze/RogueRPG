using Hsenl.timeline;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TsHarmOfSphereCollider : TsHarm<HarmOfSphereColliderInfo> {
        private Substantive _self;
        private Collider _selfCollider;
        private Substantive _selfHolder;
        private Faction _selfFaction;

        private Hurtable _targetHurtable;

        protected override void OnNodeOpen() {
            base.OnNodeOpen();
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this._self = ability;
                    this._selfHolder = ability.GetHolder();
                    this._selfFaction = ability.GetHolder()?.GetComponent<Faction>();

                    if (this._selfCollider == null) {
                        this._selfCollider = ability.GetComponentInChildren<Collider>(true);
                        if (this._selfCollider == null) {
                            this._selfCollider = ColliderFactory.CreateCollider<SphereCollider>(GameColliderPurpose.BodyTrigger);
                            this._selfCollider.SetParent(this.manager.Substantive.Entity);
                            this._selfCollider.Entity.Active = false;
                        }

                        this._selfCollider.AddTriggerEnterListening(this.OnTriggerEnter);
                        this._selfCollider.AddTriggerExitListening(this.OnTriggerExit);
                    }

                    break;
                }
            }
        }

        private void OnTriggerEnter(Collider collider) {
            switch (this._self) {
                case Ability ability: {
                    var sub = collider.Substantive;
                    if (sub == null) return;
                    if (sub == this._selfHolder) return;
                    var tags = this._selfFaction.GetTagsOfFactionTypes(ability.factionTypes);
                    if (!sub.Tags.ContainsAny(tags)) {
                        return;
                    }

                    var hurtable = sub.GetComponent<Hurtable>();
                    if (hurtable == null) return;

                    this.Harm(hurtable);

                    this._targetHurtable ??= hurtable;
                    FxManager.Instance.Play(this.info.HitFx, hurtable.transform.Position);
                    hurtable.GetComponent<Sound>()?.Play(this.info.HitSound);
                    break;
                }
            }
        }

        private void OnTriggerExit(Collider collider) { }

        protected override void OnTimeSegmentOrigin() {
            if (this._selfCollider == null) return;
            var tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);
            if (tsize == 0) tsize = 1;
            this._selfCollider.transform.LocalScale = Vector3.one * tsize;
            this._targetHurtable = null;
            // 将物理模拟临时改为脚本调用模式, 然后手动进行一次模拟运行, 这样哪怕collider只被开启一帧, 也能检测出来
            var simulationMode = Physics.simulationMode;
            Physics.simulationMode = SimulationMode.Script;
            this._selfCollider.Entity.Active = true;
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.simulationMode = simulationMode;

            // 只播放一次, 要不然声音太大了
            this._targetHurtable?.GetComponent<Sound>()?.Play(this.info.HitSound);
        }

        protected override void OnTimeSegmentRunning() { }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            if (this._selfCollider.IsInvalid())
                return;

            this._selfCollider.Entity.Active = false;
        }
    }
}