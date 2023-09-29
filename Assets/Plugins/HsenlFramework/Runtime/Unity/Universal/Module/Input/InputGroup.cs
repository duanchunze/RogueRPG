using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hsenl {
    public enum InputGroupType {
        Joystick,
        Keycode,
    }

    public class InputGroup : MonoBehaviour {
        public static InputGroup current;

        public InputGroupType inputGroupType;

        private readonly Dictionary<InputCode, InputUnit> _map = new Dictionary<InputCode, InputUnit>();

        public readonly List<InputUnit> inputs = new List<InputUnit>();

        public Action<InputGroup> onAnyInputEvent;

        public InputUnit this[InputCode inputCode] {
            get {
                this._map.TryGetValue(inputCode, out var result);
                return result;
            }
        }

        public bool TryGet(InputCode inputCode, out InputUnit inputUtil) {
            return this._map.TryGetValue(inputCode, out inputUtil);
        }

        private void OnEnable() {
            this.UpdateInputUnit();
        }

        private void OnDisable() {
            foreach (var inputUnit in this.inputs) {
                inputUnit.OnStarted -= this.OnAnyInputEvent;
            }

            this._map.Clear();
            this.inputs.Clear();
            this.onAnyInputEvent = null;
        }

        public void UpdateInputUnit() {
            this._map.Clear();
            this.inputs.Clear();
            for (var i = 0; i < this.transform.childCount; i++) {
                var inputUnit = this.transform.GetChild(i).GetComponent<InputUnit>();
                if (inputUnit == null) {
                    continue;
                }

                if (inputUnit.InputCode == InputCode.None) {
                    Debug.LogError($"<InputCode 没有指定> {inputUnit.name}");
                    continue;
                }

                this._map.Add(inputUnit.InputCode, inputUnit);
                this.inputs.Add(inputUnit);
                inputUnit.OnStarted += this.OnAnyInputEvent;
            }
        }

        private void OnAnyInputEvent(InputAction.CallbackContext context) {
            this.onAnyInputEvent?.Invoke(this);
        }

        [Button("自动配置所有InputUnit"), ButtonGroup()]
        private void MatchAll() {
            foreach (var inputUnit in this.GetComponentsInChildren<InputUnit>()) {
                inputUnit.MatchOfName();
            }
        }

        [Button("清理所有InputUnit"), ButtonGroup()]
        private void ClearAll() {
            foreach (var inputUnit in this.GetComponentsInChildren<InputUnit>()) {
                inputUnit.Clear();
            }
        }
    }
}