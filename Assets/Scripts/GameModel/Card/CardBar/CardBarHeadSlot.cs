using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class CardBarHeadSlot : CardBarSlot {
        public int order;
        private ControlCode _controlCode;

        public ControlCode ControlCode {
            get => this._controlCode;
            set {
                this._controlCode = value;
            }
        }
    }
}