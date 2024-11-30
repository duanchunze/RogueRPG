using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class PropBar : Bodied {
        private readonly List<Prop> _props = new();

        public event Action<Prop> OnPropAdd;
        public event Action<Prop> OnPropRemove;
        public event Action OnPropBarChanged;
        
        [MemoryPackIgnore]
        public IReadOnlyList<Prop> Props => this._props;

        public void EquipProp(Prop prop) {
            prop.SetParent(this.Entity, false);
        }

        public void UnequipProp(Prop prop) {
            prop.SetParent(null);
        }

        public Prop FindProp(string name) {
            for (int i = 0; i < this._props.Count; i++) {
                var prop = this._props[i];
                if (prop.Name == name)
                    return prop;
            }

            return null;
        }

        public void SwapProps(Prop prop1, Prop prop2) {
            if (prop1.FindScopeInParent<PropBar>() != this
                || prop2.FindScopeInParent<PropBar>() != this)
                return;

            this.SwapChildrenSeat(prop1.Entity, prop2.Entity);
            this.Changed();
        }

        protected override void OnChildScopeAdd(Scope child) {
            if (child is not Prop prop)
                return;

            this._props.Add(prop);
            prop.Reactivation();
            this.OnPropAdd?.Invoke(prop);
            this.Changed();
        }

        protected override void OnChildScopeRemove(Scope child) {
            if (child is not Prop prop)
                return;

            this._props.Remove(prop);
            this.OnPropRemove?.Invoke(prop);
            this.Changed();
        }

        public void Changed() {
            try {
                this.OnPropBarChanged?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            EventStation.OnPropBarChanged(this);
        }
    }
}