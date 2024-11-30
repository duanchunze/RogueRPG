using System;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class AbilitesBar : Bodied {
        [MemoryPackInclude]
        private int _explicitAbilityCapacity; // 显示技能的容量
        private readonly List<Ability> _abilities = new();
        private readonly List<Ability> _explicitAbilities = new();

        [MemoryPackIgnore]
        public List<Ability> currentCastingAbilities = new(); // 当前释放中的技能

        [MemoryPackIgnore]
        public Action<Ability> onAbilityAdd;

        [MemoryPackIgnore]
        public Action<Ability> onAbilityRemove;

        private Action _onAbilityChanged;

        [MemoryPackIgnore]
        public int ExplicitAbilityCapacity {
            get => this._explicitAbilityCapacity;
            set => this._explicitAbilityCapacity = value;
        }

        [MemoryPackIgnore]
        public IReadOnlyList<Ability> Abilities => this._abilities;

        [MemoryPackIgnore]
        public IReadOnlyList<Ability> ExplicitAbilies => this._explicitAbilities;

        public event Action OnAbilityChanged {
            add => this._onAbilityChanged += value;
            remove => this._onAbilityChanged -= value;
        }

        public void EquipAbility(Ability ability) {
            ability.SetParent(this.Entity);
        }

        public void UnequipAbility(Ability ability) {
            ability.SetParent(null);
        }

        public Ability FindAbility(string name) {
            for (int i = 0; i < this._abilities.Count; i++) {
                var abi = this._abilities[i];
                if (abi.Name == name)
                    return abi;
            }

            return null;
        }

        public void SwapAbilites(Ability ability1, Ability ability2) {
            if (ability1.FindScopeInParent<AbilitesBar>() != this
                || ability2.FindScopeInParent<AbilitesBar>() != this)
                return;

            if (!ability1.Tags.Contains(TagType.AbilityExplicit)
                || !ability2.Tags.Contains(TagType.AbilityExplicit))
                return;

            this.SwapChildrenSeat(ability1.Entity, ability2.Entity);
            this._explicitAbilities.SwapElements(ability1, ability2);
            this.Changed();
        }

        protected override void OnChildScopeAdd(Scope child) {
            if (child is not Ability ability)
                return;

            ability.transform.NormalizeValue();
            ability.Reactivation();
            this._abilities.Add(ability);
            if (ability.Tags.Contains(TagType.AbilityExplicit))
                this._explicitAbilities.Add(ability);
            this.onAbilityAdd?.Invoke(ability);
            this.Changed();
        }

        protected override void OnChildScopeRemove(Scope child) {
            if (child is not Ability ability)
                return;

            this._abilities.Remove(ability);
            this._explicitAbilities.Remove(ability);
            this.onAbilityRemove?.Invoke(ability);
            this.Changed();
        }

        public void Changed() {
            try {
                this._onAbilityChanged?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            EventStation.OnAbilitesBarChanged(this);
        }
    }
}