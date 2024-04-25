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
        private float _cooldownTillTime;

        [MemoryPackIgnore]
        public bool IsCooldown => TimeInfo.Time >= this._cooldownTillTime;

        [MemoryPackOrder(52)]
        [MemoryPackInclude]
        public int manaCost;

        [MemoryPackIgnore]
        public float casterCompensate; // 施法补偿

        [MemoryPackIgnore]
        public float cooldownCompensate; // 冷却补偿

        [MemoryPackIgnore]
        public Action onAbilityEnter;

        [MemoryPackIgnore]
        public Action onAbilityLeave;

        public void SetConfigId(int configId) {
            this._configId = configId;
        }

        public void ResetCooldown(float coolTime) {
            this._cooldownTillTime = coolTime;
        }

        public void OnAbilityEnter() {
            try {
                this.onAbilityEnter?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnAbilityLeave() {
            try {
                this.onAbilityLeave?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override void OnAwake() {
            this.MultiCombin(null);
            this.CrossCombinForParent(this.FindScopeInParent<AbilityBar>(), null);
            this.CrossCombinForParent(this.FindScopeInParent<Actor>(), null);
        }

        protected override void OnDestroy() {
            this.DecombinAll();
        }

        protected override void OnComponentAdd(Component component) {
            if (component is not Element element)
                return;

            this.MultiCombin(element);
            this.CrossCombinForParent(this.FindScopeInParent<AbilityBar>(), element);
            this.CrossCombinForParent(this.FindScopeInParent<Actor>(), element);
        }

        protected override void OnComponentRemove(Component component) {
            if (component is not Element element)
                return;

            this.MultiDecombin(element);
            this.CrossDecombinByComponent(element);
        }

        protected override void OnParentChanged(Entity previousParent) {
            this.CrossDecombinForParents(previousParent?.GetComponentInParent<Scope>(true));
            this.CrossCombinForParent(this.FindScopeInParent<AbilityBar>(), null);
            this.CrossCombinForParent(this.FindScopeInParent<Actor>(), null);
        }
    }
}