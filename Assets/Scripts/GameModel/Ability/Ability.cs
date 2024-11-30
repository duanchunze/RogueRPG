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
        public List<SelectionTargetDefault> targets = new();

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

        public void ResetCooldown() {
            this._cooldownOrigin = 0;
            this._cooldownTillTime = 0;
        }

        public (float originTime, float tillTime) GetCooldownInfo() {
            return (this._cooldownOrigin, this._cooldownTillTime);
        }

        public float GetCooldownPct() {
            var cd = this._cooldownTillTime - this._cooldownOrigin;
            if (cd <= 0)
                return 0;

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
            // var patchs = this.FindBodiedsInIndividual<AbilityPatch>();
            // if (patchs.Length >= 6)
            //     return;

            patch.SetParent(this.Entity);
        }

        public void AddAssist(Prop assist) {
            assist.SetParent(this.Entity);
        }

        protected override void OnChildAdd(Entity child) {
            var abiPatch = child.GetComponent<AbilityPatch>();
            if (abiPatch != null) { }
        }

        protected override void OnChildRemove(Entity child) {
            var abiPatch = child.GetComponent<AbilityPatch>();
            if (abiPatch != null) { }
        }
    }
}