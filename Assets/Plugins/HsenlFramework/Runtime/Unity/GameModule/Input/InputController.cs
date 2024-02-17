using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    public struct InputHappen {
        public InputCode inputCode;
        public bool isButtonDown;
        public bool isButton;
        public bool isButtonUp;
        public bool hasValue;
        public float3 value;
    }
    
    [DisallowMultipleComponent, DefaultExecutionOrder(-5)] // 让InputController的Update顺序在框架Update顺序的前面
    public class InputController : UnitySingleton<InputController> {
        protected override void Awake() {
            base.Awake();
            foreach (var inputGroup in this.GetComponentsInChildren<InputGroup>()) {
                inputGroup.onAnyInputEvent += OnInputGroupTrigger;
            }
        }

        private void OnDestroy() {
            foreach (var inputGroup in this.GetComponentsInChildren<InputGroup>()) {
                inputGroup.onAnyInputEvent -= OnInputGroupTrigger;
            }
        }

        private static void OnInputGroupTrigger(InputGroup inputGroup) {
            // Debug.Log(inputGroup.name);
            InputGroup.current = inputGroup;
        }

        private void Update() {
            if (InputGroup.current == null) {
                return;
            }

            foreach (var inputUnit in InputGroup.current.inputs) {
                if (inputUnit.WasPressedThisFrame) {
                    InputHappen inputHappen = new() {
                        inputCode = inputUnit.InputCode,
                        isButtonDown = true
                    };
                    if (inputUnit.GetValue() is Vector2 value) {
                        inputHappen.hasValue = true;
                        inputHappen.value = new float3(value.x, 0, value.y);
                    }

                    this.OnInputHappen(inputHappen);
                }

                if (inputUnit.IsPressed) {
                    InputHappen inputHappen = new() {
                        inputCode = inputUnit.InputCode,
                        isButton = true
                    };
                    if (inputUnit.GetValue() is Vector2 value) {
                        inputHappen.hasValue = true;
                        inputHappen.value = new float3(value.x, 0, value.y);
                    }

                    this.OnInputHappen(inputHappen);
                }

                if (inputUnit.WasReleasedThisFrame) {
                    InputHappen inputHappen = new() {
                        inputCode = inputUnit.InputCode,
                        isButtonUp = true
                    };
                    if (inputUnit.GetValue() is Vector2 value) {
                        inputHappen.hasValue = true;
                        inputHappen.value = new float3(value.x, 0, value.y);
                    }

                    this.OnInputHappen(inputHappen);
                }
            }
        }

        private void OnInputHappen(InputHappen inputHappen) {
            var codes = InputMaptable.QueryControlCode(inputHappen.inputCode);

            if (codes == null) {
                return;
            }

            foreach (var code in codes) {
                EventType.ControlMessage inputMessage = new() {
                    controlCode = code,
                    isButtonDown = inputHappen.isButtonDown,
                    isButton = inputHappen.isButton,
                    isButtonUp = inputHappen.isButtonUp,
                    hasValue = inputHappen.hasValue,
                    value = inputHappen.value,
                };

                EventSystem.Publish(inputMessage);
            }
        }
        
        public static bool GetButtonDown(InputCode inputCode) {
            if (InputGroup.current == null)
                return false;

            if (!InputGroup.current.TryGet(inputCode, out var inputUnit))
                return false;

            return inputUnit.WasPressedThisFrame;
        }

        public static bool GetButton(InputCode inputCode) {
            if (InputGroup.current == null)
                return false;

            if (!InputGroup.current.TryGet(inputCode, out var inputUnit))
                return false;

            return inputUnit.IsPressed;
        }

        public static bool GetButtonUp(InputCode inputCode) {
            if (InputGroup.current == null)
                return false;

            if (!InputGroup.current.TryGet(inputCode, out var inputUnit))
                return false;

            return inputUnit.WasReleasedThisFrame;
        }

        public static Vector2 GetVector2(InputCode inputCode) {
            if (InputGroup.current == null)
                return Vector2.zero;

            if (!InputGroup.current.TryGet(inputCode, out var inputUnit))
                return Vector2.zero;

            return inputUnit.GetVector2();
        }

        public static float GetFloat(InputCode inputCode) {
            if (InputGroup.current == null)
                return 0;

            if (!InputGroup.current.TryGet(inputCode, out var inputUnit))
                return 0;

            return inputUnit.GetFloat();
        }

        public static float3 GetMousePosition() {
            if (InputGroup.current == null) {
                return default;
            }

            var o = InputGroup.current[InputCode.MousePosition].GetValue();
            var v = (Vector2)o;
            return new float3(v.x, v.y, 0);
        }
    }
}