using System;
using MemoryPack;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class ControlTrigger : Unbodied {
#if UNITY_EDITOR
        [SerializeField]
#endif
        [MemoryPackInclude]
        protected int controlCode = 0;

        [MemoryPackInclude]
        public bool supportContinue;

        [MemoryPackIgnore]
        private Control _control;

        [MemoryPackIgnore]
        public Action onBegin;

        [MemoryPackIgnore]
        public Action onFinish;

        [MemoryPackIgnore]
        public int ControlCode {
            get => this.controlCode;
            set {
                if (this.controlCode == value) return;
                this.UnregisterControlListening(this._control, this.controlCode);
                this.controlCode = value;
                this.RegisterControlListening(this._control, this.controlCode);
            }
        }

        protected internal override void OnDestroyFinish() {
            this._control = null;
            this.onBegin = null;
            this.onFinish = null;
        }

        public void SetControl(Control control, bool onlySet = false) {
            if (this._control == control) return;
            if (!onlySet) this.UnregisterControlListening(this._control, this.controlCode);
            this._control = control;
            if (!onlySet) this.RegisterControlListening(this._control, this.controlCode);
        }

        private void RegisterControlListening(Control ctrl, int code) {
            if (ctrl == null) return;
            if (code == 0) return;
            ctrl.Register(code);
            ctrl.AddStartListener(code, this.Start);
            ctrl.AddSustainedListener(code, this.Sustain);
            ctrl.AddEndListener(code, this.End);
        }

        private void UnregisterControlListening(Control ctrl, int code) {
            if (ctrl == null) return;
            if (code == 0) return;
            ctrl.RemoveStartListener(code, this.Start);
            ctrl.RemoveSustainedListener(code, this.Sustain);
            ctrl.RemoveEndListener(code, this.End);
            ctrl.UnRegister(code);
        }

        protected virtual void Start() {
            this.onBegin?.Invoke();
        }

        protected virtual void Sustain() {
            if (!this.supportContinue) return;
            this.onBegin?.Invoke();
        }

        protected virtual void End() {
            this.onFinish?.Invoke();
        }

        public bool GetValue(out float3 value) {
            if (this._control == null) {
                value = default;
                return false;
            }

            return this._control.GetValue(this.controlCode, out value);
        }
    }
}