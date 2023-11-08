using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class AbilityBar : Bodied {
        private readonly List<Ability> _abilities = new();
        public List<Ability> currentCastingAbilities = new(); // 当前释放中的技能
        
        public Action<Ability> onAbilityAdd;
        public Action<Ability> onAbilityRemove;
        public Action onAbilityChanged;
        
        public IReadOnlyList<Ability> Abilities => this._abilities;

        public void EquipAbility(Ability ability) {
            ability.SetParent(this.Entity);
        }

        public void UnequipAbility(Ability ability) {
            ability.SetParent(null);
        }

        protected override void OnChildScopeAdd(Scope child) {
            if (child is not Ability ability)
                return;
            
            ability.transform.NormalTransfrom();
            ability.Reactivation();
            this._abilities.Add(ability);
            this.onAbilityAdd?.Invoke(ability);
            this.onAbilityChanged?.Invoke();
        }

        protected override void OnChildScopeRemove(Scope child) {
            if (child is not Ability ability)
                return;
            
            this._abilities.Remove(ability);
            this.onAbilityRemove?.Invoke(ability);
            this.onAbilityChanged?.Invoke();
        }
    }
}