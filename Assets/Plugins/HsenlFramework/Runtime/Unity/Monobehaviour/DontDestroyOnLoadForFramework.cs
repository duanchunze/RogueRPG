using System;
using UnityEngine;

namespace Hsenl {
    public class DontDestroyOnLoadForFramework : MonoBehaviour {
        private void Awake() {
            var go = this.gameObject;
            go.GetOrCreateEntityReference().Entity.DontDestroyOnLoadWithUnity();
        }
    }
}