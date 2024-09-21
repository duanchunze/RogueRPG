using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Hsenl {
    // 输入个体，核心为 InputAction
    // 可以直接获得按键状态，也可以注册回调事件
    public class InputUnit : MonoBehaviour {
        private static List<InputUnit> _allInputUnits;

        // 遥感需要偏移5成以上，才算是触发按下事件
        public float stickIgnoreError = 0.5f;

        [FormerlySerializedAs("m_inputAction")]
        [SerializeField]
        private InputAction _inputAction; // 输入行为

        private InputControl _control; // 指的是控制器，即设备

        [FormerlySerializedAs("m_InputCode")]
        [SerializeField]
        private InputCode _inputCode;

        [SerializeField]
        private int _controlIndex;

        private int _controlMissProtection;

        public event Action<InputAction.CallbackContext> OnStarted;
        public event Action<InputAction.CallbackContext> OnPerformed;
        public event Action<InputAction.CallbackContext> OnCanceled;

        private object _value;

        public InputAction InputAction => this._inputAction;

        public InputCode InputCode => this._inputCode;

        /// <summary>
        /// 效果等同于 OnButtonEnter，对于非按钮式的控制器，比如遥感，则有单独的触发规则。
        /// </summary>
        [ShowInInspector]
        public bool WasPressedThisFrame {
            get {
                if (this.enabled == false) {
                    return false;
                }

                return this._inputAction.WasPressedThisFrame();
                // switch (m_control)
                // {
                //     case null:
                //         return false;
                //     case ButtonControl buttonControl:
                //     {
                //         return buttonControl.wasPressedThisFrame;
                //     }
                //     case StickControl stickControl:
                //     {
                //         if (stickControl.device == null)
                //         {
                //             return false;
                //         }
                //
                //         bool b1 = this.IsValueConsideredPressed_Vector2(stickControl.ReadValue(), Vector2.zero);
                //         bool b2 = this.IsValueConsideredPressed_Vector2(stickControl.ReadValueFromPreviousFrame(), Vector2.zero);
                //
                //         return stickControl.device.wasUpdatedThisFrame && b1 && !b2;
                //     }
                // }
                //
                // return false;
            }
        }

        /// <summary>
        /// 效果等同于 OnButton，对于非按钮式的控制器，比如遥感，则有单独的触发规则。
        /// </summary>
        [ShowInInspector]
        public bool IsPressed {
            get {
                if (this.enabled == false) {
                    return false;
                }

                return this._inputAction.IsPressed();
                // switch (m_control)
                // {
                //     case null:
                //         return false;
                //     case ButtonControl buttonControl:
                //     {
                //         if (this.m_control is KeyControl keyControl)
                //         {
                //             return keyControl.isPressed;
                //         }
                //
                //         return buttonControl.isPressed;
                //     }
                //     case StickControl stickControl:
                //     {
                //         if (stickControl.device == null)
                //         {
                //             return false;
                //         }
                //
                //         bool b = this.IsValueConsideredPressed_Vector2(stickControl.ReadValue(), Vector2.zero);
                //
                //         return b;
                //     }
                //     case AxisControl axisControl:
                //     {
                //         if (axisControl.device == null)
                //         {
                //             return false;
                //         }
                //
                //         bool b1 = IsValueConsideredPressed_Float(axisControl.ReadValue(), 0);
                //
                //         return b1;
                //     }
                //     case Vector2Control vector2Control:
                //     {
                //         if (vector2Control.device == null)
                //         {
                //             return false;
                //         }
                //
                //         bool b1 = IsValueConsideredPressed_Vector2(vector2Control.ReadValue(), Vector2.zero);
                //
                //         return b1;
                //     }
                // }
                //
                // return false;
            }
        }

        /// <summary>
        /// 效果等同于 OnButtonUp，对于非按钮式的控制器，比如遥感，则有单独的触发规则。
        /// </summary>
        [ShowInInspector]
        public bool WasReleasedThisFrame {
            get {
                if (this.enabled == false) {
                    return false;
                }

                return this._inputAction.WasReleasedThisFrame();
                // switch (m_control)
                // {
                //     case null:
                //         return this.m_inputAction.phase == InputActionPhase.Canceled; // TODO 这里的设备丢失保护 不知道效果对不对，还未测试
                //     case ButtonControl buttonControl:
                //     {
                //         return buttonControl.wasReleasedThisFrame;
                //     }
                //     case StickControl stickControl:
                //     {
                //         if (stickControl.device == null)
                //         {
                //             return false;
                //         }
                //
                //         bool b1 = this.IsValueConsideredPressed_Vector2(stickControl.ReadValue(), Vector2.zero);
                //         bool b2 = this.IsValueConsideredPressed_Vector2(stickControl.ReadValueFromPreviousFrame(), Vector2.zero);
                //
                //         return stickControl.device.wasUpdatedThisFrame && !b1 && b2;
                //     }
                // }
                //
                // return false;
            }
        }

        /// <summary>
        /// 获取控制器的值
        /// </summary>
        /// <returns></returns>
        public object GetValue() {
            return this._inputAction.ReadValueAsObject();
        }

        /// <summary>
        /// 获取遥感或复合型输入的向量值，对于 Button Trigger 类按钮无效
        /// </summary>
        /// <returns></returns>
        public UnityEngine.Vector2 GetVector2() {
            return this._inputAction.ReadValue<UnityEngine.Vector2>();
        }

        /// <summary>
        /// 获取 Trigger 的按下程度值
        /// </summary>
        /// <returns></returns>
        public float GetFloat() {
            return this._inputAction.ReadValue<float>();
        }

        private void Awake() {
            this._inputAction.started += this.OnInputActionOnstarted;
            this._inputAction.performed += this.OnInputActionOnperformed;
            this._inputAction.canceled += this.OnInputActionOncanceled;
        }

        private void OnDestroy() {
            this._inputAction.started -= this.OnInputActionOnstarted;
            this._inputAction.performed -= this.OnInputActionOnperformed;
            this._inputAction.canceled -= this.OnInputActionOncanceled;
        }

        private void OnInputActionOncanceled(InputAction.CallbackContext context) {
            try {
                if (this.enabled == false) {
                    return;
                }

                this.OnCanceled?.Invoke(context);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private void OnInputActionOnperformed(InputAction.CallbackContext context) {
            try {
                if (this.enabled == false) {
                    return;
                }

                this.OnPerformed?.Invoke(context);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private void OnInputActionOnstarted(InputAction.CallbackContext context) {
            try {
                if (this.enabled == false) {
                    return;
                }

                this.OnStarted?.Invoke(context);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private void OnEnable() {
            if (_allInputUnits == null) {
                _allInputUnits = new List<InputUnit>();
            }

            if (_allInputUnits.Count == 0) {
                InputSystem.onDeviceChange += this.OnDeviceChange;
            }

            _allInputUnits.Add(this);

            this.ResolveControl();

            this._inputAction.Enable();
        }

        private void OnDisable() {
            this._inputAction.Disable();

            _allInputUnits.Remove(this);

            if (_allInputUnits.Count == 0) {
                _allInputUnits = null;
                InputSystem.onDeviceChange -= this.OnDeviceChange;
            }
        }

        /// <summary>
        /// 当设备发生改变
        /// </summary>
        /// <param name="device"></param>
        /// <param name="change"></param>
        private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
            // 如果改变的是当前使用的设备，则需要重新确定设备
            if (change != InputDeviceChange.Added && change != InputDeviceChange.Removed)
                return;

            if (_allInputUnits == null)
                return;

            foreach (var component in _allInputUnits) {
                if (change == InputDeviceChange.Removed && component._control != null &&
                    component._control.device == device) {
                    component.ResolveControl();
                }
                else if (change == InputDeviceChange.Added) {
                    component.ResolveControl();
                }
            }
        }

        /// <summary>
        /// 确定一个控制器
        /// </summary>
        private void ResolveControl() {
            this._control = null;

            var controls = this._inputAction.controls;
            var candidateCount = controls.Count;

            if (this._inputAction.controls.Count == 0) {
                // 说明没有接入对应的设备
                return;
            }

            if (candidateCount > 1 && this._controlIndex < candidateCount && this._controlIndex >= 0) {
                this._control = controls[this._controlIndex];
            }
            else {
                this._control = controls[0];
            }
        }

        private bool IsValueConsideredPressed_Vector2(UnityEngine.Vector2 v1, UnityEngine.Vector2 v2) {
            var diff = v1 - v2;
            return diff.sqrMagnitude > this.stickIgnoreError;
        }

        private bool IsValueConsideredPressed_Float(float v1, float v2) {
            var diff = v1 - v2;
            return diff > this.stickIgnoreError;
        }

        [Button("自动匹配"), ButtonGroup()]
        public void MatchOfName() {
            if (!Enum.TryParse(this.name, out this._inputCode)) return;

            switch (this._inputCode) {
                // 键盘
                case InputCode.None:
                    break;
                case InputCode.Backspace:
                case InputCode.Tab:
                case InputCode.Clear:
                case InputCode.Return:
                case InputCode.Pause:
                case InputCode.Escape:
                case InputCode.Space:
                case InputCode.Exclaim:
                case InputCode.DoubleQuote:
                case InputCode.Hash:
                case InputCode.Dollar:
                case InputCode.Percent:
                case InputCode.Ampersand:
                case InputCode.Quote:
                case InputCode.LeftParen:
                case InputCode.RightParen:
                case InputCode.Asterisk:
                case InputCode.Plus:
                case InputCode.Comma:
                case InputCode.Minus:
                case InputCode.Period:
                case InputCode.Slash:
                case InputCode.Alpha0:
                case InputCode.Alpha1:
                case InputCode.Alpha2:
                case InputCode.Alpha3:
                case InputCode.Alpha4:
                case InputCode.Alpha5:
                case InputCode.Alpha6:
                case InputCode.Alpha7:
                case InputCode.Alpha8:
                case InputCode.Alpha9:
                    this._inputAction.AddBinding($"<Keyboard>/{this._inputCode.ToString()[^1]}");
                    break;
                case InputCode.Colon:
                case InputCode.Semicolon:
                case InputCode.Less:
                case InputCode.Equals:
                case InputCode.Greater:
                case InputCode.Question:
                case InputCode.At:
                case InputCode.LeftBracket:
                case InputCode.Backslash:
                case InputCode.RightBracket:
                case InputCode.Caret:
                case InputCode.Underscore:
                case InputCode.BackQuote:
                case InputCode.A:
                case InputCode.B:
                case InputCode.C:
                case InputCode.D:
                case InputCode.E:
                case InputCode.F:
                case InputCode.G:
                case InputCode.H:
                case InputCode.I:
                case InputCode.J:
                case InputCode.K:
                case InputCode.L:
                case InputCode.M:
                case InputCode.N:
                case InputCode.O:
                case InputCode.P:
                case InputCode.Q:
                case InputCode.R:
                case InputCode.S:
                case InputCode.T:
                case InputCode.U:
                case InputCode.V:
                case InputCode.W:
                case InputCode.X:
                case InputCode.Y:
                case InputCode.Z:
                case InputCode.LeftCurlyBracket:
                case InputCode.Pipe:
                case InputCode.RightCurlyBracket:
                case InputCode.Tilde:
                case InputCode.Delete:
                case InputCode.Keypad0:
                case InputCode.Keypad1:
                case InputCode.Keypad2:
                case InputCode.Keypad3:
                case InputCode.Keypad4:
                case InputCode.Keypad5:
                case InputCode.Keypad6:
                case InputCode.Keypad7:
                case InputCode.Keypad8:
                case InputCode.Keypad9:
                    this._inputAction.AddBinding($"<Keyboard>/numpad{this._inputCode.ToString()[^1]}");
                    break;
                case InputCode.KeypadPeriod:
                case InputCode.KeypadDivide:
                case InputCode.KeypadMultiply:
                case InputCode.KeypadMinus:
                case InputCode.KeypadPlus:
                case InputCode.KeypadEnter:
                case InputCode.KeypadEquals:
                case InputCode.UpArrow:
                case InputCode.DownArrow:
                case InputCode.RightArrow:
                case InputCode.LeftArrow:
                case InputCode.Insert:
                case InputCode.Home:
                case InputCode.End:
                case InputCode.PageUp:
                case InputCode.PageDown:
                case InputCode.F1:
                case InputCode.F2:
                case InputCode.F3:
                case InputCode.F4:
                case InputCode.F5:
                case InputCode.F6:
                case InputCode.F7:
                case InputCode.F8:
                case InputCode.F9:
                case InputCode.F10:
                case InputCode.F11:
                case InputCode.F12:
                case InputCode.F13:
                case InputCode.F14:
                case InputCode.F15:
                case InputCode.Numlock:
                case InputCode.CapsLock:
                case InputCode.ScrollLock:
                case InputCode.RightShift:
                case InputCode.LeftShift:
                case InputCode.RightAlt:
                case InputCode.LeftAlt:
                case InputCode.RightApple:
                case InputCode.LeftApple:
                case InputCode.LeftWindows:
                case InputCode.RightWindows:
                case InputCode.AltGr:
                case InputCode.Help:
                case InputCode.Print:
                case InputCode.SysReq:
                case InputCode.Break:
                case InputCode.Menu:
                    this._inputAction.AddBinding($"<Keyboard>/{this._inputCode.ToString().ToLower()}");
                    break;

                case InputCode.RightControl:
                    this._inputAction.AddBinding($"<Keyboard>/rightCtrl");
                    break;
                case InputCode.LeftControl:
                    this._inputAction.AddBinding($"<Keyboard>/leftCtrl");
                    break;

                // 特殊
                case InputCode.WASD:
                    break;

                // 鼠标
                case InputCode.MouseLeft:
                    this._inputAction.AddBinding("<Mouse>/leftButton");
                    break;
                case InputCode.MouseRight:
                    this._inputAction.AddBinding("<Mouse>/rightButton");
                    break;
                case InputCode.MouseMiddle:
                    this._inputAction.AddBinding("<Mouse>/middleButton");
                    break;
                case InputCode.MousePosition:
                    this._inputAction.AddBinding("<Mouse>/position");
                    break;

                // 手柄
                case InputCode.JoystickNone:
                    break;
                case InputCode.LeftStick:
                case InputCode.RightStick:
                case InputCode.LeftBumper:
                case InputCode.RightBumper:
                case InputCode.LeftTrigger:
                case InputCode.RightTrigger:
                case InputCode.LeftStickPress:
                case InputCode.RightStickPress:
                case InputCode.ButtonSouth:
                case InputCode.ButtonEast:
                case InputCode.ButtonWest:
                case InputCode.ButtonNorth:
                case InputCode.Start:
                case InputCode.Select:
                    this._inputAction.AddBinding($"<Gamepad>/{this._inputCode.ToString().ToLower()}");
                    break;
                case InputCode.Dpad_Up:
                case InputCode.Dpad_Down:
                case InputCode.Dpad_Left:
                case InputCode.Dpad_Right:
                    var str = this._inputCode.ToString().ToLower();
                    str = str.Replace("_", "/");
                    this._inputAction.AddBinding($"<Gamepad>/{str}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Button("清理"), ButtonGroup()]
        public void Clear() {
            this._inputAction = new InputAction();
        }
    }

// #if UNITY_EDITOR
//     [CustomEditor(typeof(InputUnit))]
//     public class InputUnitEditor : Editor {
//         private InputUnit _t;
//
//         private void OnEnable() {
//             this._t = (InputUnit)this.target;
//         }
//
//         public override void OnInspectorGUI() {
//             base.OnInspectorGUI();
//
//             if (GUILayout.Button("清空")) {
//                 this._t.Clear();
//                 this.serializedObject.Update();
//             }
//
//             if (GUILayout.Button("根据名称匹配")) {
//                 this._t.MatchOfName();
//                 this.serializedObject.Update();
//             }
//         }
//     }
// #endif
}