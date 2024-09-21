using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class SelectionTarget : Unbodied {
        private List<Selector> _selectors = new();

        public IReadOnlyList<Selector> Selectors => this._selectors;

        public void AddSelector(Selector selector) {
            if (this._selectors.Contains(selector))
                return;

            this._selectors.Add(selector);
        }

        public void RemoveSelector(Selector selector) {
            this._selectors.Remove(selector);
        }

        protected override void OnDestroy() {
            this._selectors.Clear();
        }
    }
}