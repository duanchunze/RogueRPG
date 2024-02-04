using System;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

namespace Hsenl {
    public class FrameworkProxy : MonoBehaviour {
        public Framework framework;
        public int targetFrameRate = 75;
        public float timeScale = 1;

        private void Awake() {
            Framework.OnAppStart();

            if (!SingletonManager.IsDisposed<Framework>()) {
                SingletonManager.Unregister<Framework>();
            }

            SingletonManager.Register(ref this.framework);

            Application.targetFrameRate = this.targetFrameRate;

            DontDestroyOnLoad(this.gameObject);
        }

        private void Update() {
            this.framework.Update();
        }

        private void LateUpdate() {
            this.framework.LateUpdate();
        }

        private void OnApplicationQuit() {
            Framework.OnAppQuit();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FrameworkProxy))]
    public class FrameworkProxyEditor : OdinEditor {
        private FrameworkProxy _t;

        protected override void OnEnable() {
            base.OnEnable();
            this._t = (FrameworkProxy)this.target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            Application.targetFrameRate = this._t.targetFrameRate;

            if (TimeInfoManager.Instance != null) {
                TimeInfo.TimeScale = this._t.timeScale;
            }
        }
    }
#endif
}