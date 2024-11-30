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
            if (!SingletonManager.IsDisposed<Framework>()) {
                SingletonManager.Unregister<Framework>();
            }

            SingletonManager.Register(ref this.framework);
            
            this.framework.Start();

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
            this.framework.Destroy();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FrameworkProxy))]
    public class FrameworkProxyEditor : Editor {
        private FrameworkProxy _t;

        protected void OnEnable() {
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