using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class AbilityBar : Substantive {
        private readonly List<Ability> _abilities = new();
        public List<Ability> currentCastingAbilities = new(); // 当前释放中的技能
        
        public Action<Ability> onAbilityAdd;
        public Action<Ability> onAbilityRemove;
        
        public IReadOnlyList<Ability> Abilities => this._abilities;

        public void EquipAbility(Ability ability) {
            ability.SetParent(this.Entity);
        }

        public void UnequipAbility(Ability ability) {
            ability.SetParent(null);
        }

        protected override void OnChildSubstantiveAdd(Substantive childSubs) {
            if (childSubs is not Ability ability)
                return;
            
            ability.transform.NormalTransfrom();
            ability.Reactivation();
            this._abilities.Add(ability);
            this.onAbilityAdd?.Invoke(ability);
        }

        protected override void OnChildSubstantiveRemove(Substantive childSubs) {
            if (childSubs is not Ability ability)
                return;
            
            this._abilities.Remove(ability);
            this.onAbilityRemove?.Invoke(ability);
        }
    }
}