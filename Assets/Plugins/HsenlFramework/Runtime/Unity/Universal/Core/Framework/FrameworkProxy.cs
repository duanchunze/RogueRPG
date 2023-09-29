using System;
using UnityEngine;

namespace Hsenl {
    public class FrameworkProxy : MonoBehaviour {
        public Framework framework;
        public int targetFrameRate = 75;

        private void Awake() {
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

        [RuntimeInitializeOnLoadMethod]
        private static void OnApplicationStart() {
            Framework.OnAppStart();
        }

        private void OnApplicationQuit() {
            Framework.OnAppQuit();
        }
    }
}