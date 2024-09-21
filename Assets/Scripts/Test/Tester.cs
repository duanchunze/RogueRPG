using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    public class Tester : MonoBehaviour {
        private static LogStopwatch _logStopwatch;

        [Button("Click1")]
        public void Click1() { }

        [Button("Click2")]
        public void Click2() { }

        private void Start() {
            SceneManager.LoadScene("main", LoadSceneMode.Single);
            Numerator.InitNumerator(3);
            _logStopwatch = new("start");

            ShadowFunctionExampleInvocation.Invoke();
        }

        private static Action a;
        public int i;

        private void Update() {
            a += OnAction;
            a.Invoke();
            return;

            void OnAction() {
                a -= OnAction;
                this.i++;
            }
        }

        private void OnDestroy() {
            
        }
    }
}