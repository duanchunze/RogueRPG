using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class SelectionTargetDefault : SelectionTarget {
        private List<SelectorDefault> _selectors = new();

        [MemoryPackIgnore]
        public IReadOnlyList<SelectorDefault> Selectors => this._selectors;

        public void AddSelector(SelectorDefault selector) {
            if (this._selectors.Contains(selector))
                return;

            this._selectors.Add(selector);
        }

        public void RemoveSelector(SelectorDefault selector) {
            this._selectors.Remove(selector);
        }

        public void ClearAllSelectors() {
            for (int i = this._selectors.Count - 1; i >= 0; i--) {
                var selector = this._selectors[i];
                if (selector.PrimaryTarget == this) {
                    selector.PrimaryTarget = null;
                }
            }
            
            this._selectors.Clear();
        }

        protected override void OnDestroy() {
            this._selectors.Clear();
        }
    }
}