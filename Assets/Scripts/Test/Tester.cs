using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Hsenl {
    public class Tester : MonoBehaviour {
        private static LogStopwatch _logStopwatch;

        [Button("Click1")]
        public void Click1() { }

        [Button("Click2")]
        public void Click2() { }

        [Button("Click3")]
        public void Click3() { }

        private void Start() {
            SceneManager.LoadScene("main", LoadSceneMode.Single);
            _logStopwatch = new("start");

            ShadowFunctionExampleInvocation.Invoke();
        }

        private void Update() { }

        private void OnDestroy() { }
    }
}