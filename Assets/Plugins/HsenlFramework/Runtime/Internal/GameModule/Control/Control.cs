using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MemoryPack;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    public interface IControlElement {
        bool GetStart();
        bool GetSustained();
        bool GetEnd();
        bool GetValue(out float3 result);
    }

    namespace EventType {
        public struct ControlMessage {
            public int controlCode;
            public bool isButtonDown;
            public bool isButton;
            public bool isButtonUp;
            public bool hasValue;
            public float3 value;
        }
    }

    [Serializable]
    [MemoryPackable()]
    public partial class Control : Unbodied, ILateUpdate {
        [ShowInInspector, ReadOnly]
        [MemoryPackIgnore]
        private Dictionary<int, Element> _elementDict = new();

        protected override void OnDestroy() {
            foreach (var element in this._elementDict.Values) {
                element.Dispose();
            }

            this._elementDict.Clear();
        }

        public void LateUpdate() {
            // 执行流程: Set -> 我们要做的xxx事 -> Reset
            this.ResetControl();
        }

        public void Register(int controlCode) {
            if (this._elementDict.TryGetValue(controlCode, out var element)) {
                element.refCount++;
                return;
            }

            this._elementDict.Add(controlCode, new Element(controlCode));
        }

        public bool UnRegister(int controlCode) {
            if (this._elementDict.TryGetValue(controlCode, out var element)) {
                element.refCount--;
                if (element.refCount == 0) {
                    return this._elementDict.Remove(controlCode);
                }
            }

            return false;
        }

        public bool Contains(int controlCode) {
            return this._elementDict.ContainsKey(controlCode);
        }

        public void SetStart(int controlCode) {
            if (!this._elementDict.TryGetValue(controlCode, out var element)) return;
            element.SetStart();
        }

        public void SetSustained(int controlCode) {
            if (!this._elementDict.TryGetValue(controlCode, out var element)) return;
            element.SetSustained();
        }

        public void SetEnd(int controlCode) {
            if (!this._elementDict.TryGetValue(controlCode, out var element)) return;
            element.SetEnd();
        }

        public void SetValue(int controlCode, float3 val) {
            if (!this._elementDict.TryGetValue(controlCode, out var element)) return;
            element.SetValue(val);
        }

        public bool GetStart(int controlCode) {
            if (!this._elementDict.TryGetValue(controlCode, out var element)) return false;
            return element.GetStart();
        }

        public bool GetSustained(int controlCode) {
            if (!this._elementDict.TryGetValue(controlCode, out var element)) return false;
            return element.GetSustained();
        }

        public bool GetEnd(int controlCode) {
            if (!this._elementDict.TryGetValue(controlCode, out var element)) return false;
            return element.GetEnd();
        }

        public bool GetValue(int controlCode, out float3 result) {
            result = default;
            if (!this._elementDict.TryGetValue(controlCode, out var element)) return false;
            return element.GetValue(out result);
        }

        public void AddStartListener(int controlCode, Action onStart) {
            this.ExceptionOfNotExistElement(controlCode, out var element);
            element.onStart += onStart;
        }

        public void AddSustainedListener(int controlCode, Action onSustained) {
            this.ExceptionOfNotExistElement(controlCode, out var element);
            element.onSustained += onSustained;
        }

        public void AddEndListener(int controlCode, Action onEnd) {
            this.ExceptionOfNotExistElement(controlCode, out var element);
            element.onEnd += onEnd;
        }

        public void RemoveStartListener(int controlCode, Action onStart) {
            this.ExceptionOfNotExistElement(controlCode, out var element);
            element.onStart -= onStart;
        }

        public void RemoveSustainedListener(int controlCode, Action onSustained) {
            this.ExceptionOfNotExistElement(controlCode, out var element);
            element.onSustained -= onSustained;
        }

        public void RemoveEndListener(int controlCode, Action onEnd) {
            this.ExceptionOfNotExistElement(controlCode, out var element);
            element.onEnd -= onEnd;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExceptionOfNotExistElement(int controlCode, out Element element) {
            if (!this._elementDict.TryGetValue(controlCode, out element)) {
                throw new Exception($"{controlCode} element not exist in control");
            }
        }

        private void ResetControl() {
            foreach (var element in this._elementDict.Values) {
                element.Reset(TimeInfo.FrameCount);
            }
        }

        [Serializable]
        private class Element : IControlElement {
            public int refCount = 1;
            public int controlCode;
            public bool isStart;
            public bool isSustained;
            public bool isEnd;
            public long startFrameCount;
            public long sustainFrameCount;
            public long endFrameCount;
            public float3? value;

            public event Action onStart;
            public event Action onSustained;
            public event Action onEnd;

            public Element(int controlCode) {
                this.controlCode = controlCode;
            }

            public void Dispose() {
                this.refCount = 0;
                this.controlCode = 0;
                this.isStart = false;
                this.isSustained = false;
                this.isEnd = false;
                this.value = float3.zero;

                this.onStart = null;
                this.onSustained = null;
                this.onEnd = null;
            }

            public void SetStart() {
                this.isStart = true;
                this.startFrameCount = TimeInfo.FrameCount;
                try {
                    this.onStart?.Invoke();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            public void SetSustained() {
                this.isSustained = true;
                this.sustainFrameCount = TimeInfo.FrameCount;
                try {
                    this.onSustained?.Invoke();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            public void SetEnd() {
                this.isEnd = true;
                this.endFrameCount = TimeInfo.FrameCount;
                this.isSustained = false;
                this.value = null;
                try {
                    this.onEnd?.Invoke();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            public void SetValue(float3 val) {
                this.value = val;
                this.SetSustained();
            }

            public bool GetStart() => this.isStart;

            public bool GetSustained() => this.isSustained;

            public bool GetEnd() => this.isEnd;

            public bool GetValue(out float3 result) {
                if (this.value != null) {
                    result = this.value.Value;
                    return true;
                }

                result = default;
                return false;
            }

            public void Reset(int frameCount) {
                if (frameCount > this.startFrameCount) {
                    this.isStart = false;
                }

                if (frameCount > this.sustainFrameCount) {
                    this.isSustained = false;
                    this.value = null;
                }

                if (frameCount > this.endFrameCount) {
                    this.isEnd = false;
                }
            }
        }
    }
}