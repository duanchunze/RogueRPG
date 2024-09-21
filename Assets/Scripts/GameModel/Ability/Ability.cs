using System;
using System.Collections.Generic;
using Hsenl.ability;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Ability : Bodied {
        [MemoryPackOrder(50)]
        [MemoryPackInclude]
        private int _configId;

        [MemoryPackIgnore]
        public AbilityConfig Config => Tables.Instance.TbAbilityConfig.GetById(this._configId);

        [MemoryPackOrder(51)]
        [MemoryPackInclude]
        public List<FactionType> factionTypes = new();

        [MemoryPackIgnore]
        public List<SelectionTarget> targets = new();

        [MemoryPackIgnore]
        private float _cooldownOrigin;

        [MemoryPackIgnore]
        private float _cooldownTillTime;

        [MemoryPackIgnore]
        public bool IsCooldown => TimeInfo.Time >= this._cooldownTillTime;

        [MemoryPackIgnore]
        public float casterCompensate; // 施法补偿

        [MemoryPackIgnore]
        public float cooldownCompensate; // 冷却补偿

        [MemoryPackIgnore]
        public Action onAbilityCastStart;

        [MemoryPackIgnore]
        public Action onAbilityCastEnd;

        public void SetConfigId(int configId) {
            this._configId = configId;
        }

        public void Cooldown(float tillTime) {
            this._cooldownOrigin = TimeInfo.Time;
            this._cooldownTillTime = tillTime;
        }

        public (float originTime, float tillTime) GetCooldownInfo() {
            return (this._cooldownOrigin, this._cooldownTillTime);
        }

        public float GetCooldownPct() {
            var cd = this._cooldownTillTime - this._cooldownOrigin;
            var cur = TimeInfo.Time - this._cooldownOrigin;
            var pct = cur / cd;
            return pct;
        }

        public void OnAbilityCastStart() {
            try {
                this.onAbilityCastStart?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnAbilityCastEnd() {
            try {
                this.onAbilityCastEnd?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void AddPatch(AbilityPatch patch) {
            patch.SetParent(this.Entity);
        }

        public void AddAssist(AbilityAssist assist) {
            assist.SetParent(this.Entity);
        }

        protected override void OnAwake() {
            this.MultiCombin(null);
            this.CrossCombinForOther(this.FindScopeInParent<AbilitesBar>(), null);
            this.CrossCombinForOther(this.FindScopeInParent<Actor>(), null);
        }

        protected override void OnDestroy() {
            this.DecombinAll();
        }

        protected override void OnComponentAdd(Component component) {
            if (component is not Element element)
                return;

            this.MultiCombin(element);
            this.CrossCombinForOther(this.FindScopeInParent<AbilitesBar>(), element);
            this.CrossCombinForOther(this.FindScopeInParent<Actor>(), element);
        }

        protected override void OnComponentRemove(Component component) {
            if (component is not Element element)
                return;

            this.MultiDecombin(element);
            this.CrossDecombinByComponent(element);
        }

        protected override void OnParentChanged(Entity previousParent) {
            this.CrossDecombinForParents(previousParent?.GetComponentInParent<Scope>(true, true));
            this.CrossCombinForOther(this.FindScopeInParent<AbilitesBar>(), null);
            this.CrossCombinForOther(this.FindScopeInParent<Actor>(), null);
        }
    }
}