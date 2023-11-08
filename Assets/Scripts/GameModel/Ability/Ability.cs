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
        public int configId;

        [MemoryPackIgnore]
        public AbilityConfig Config => Tables.Instance.TbAbilityConfig.GetById(this.configId);

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
    }
}