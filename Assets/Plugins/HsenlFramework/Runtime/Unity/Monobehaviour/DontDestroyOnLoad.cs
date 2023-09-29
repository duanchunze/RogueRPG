using System;
using UnityEngine;

namespace Hsenl {
    public class DontDestroyOnLoad : MonoBehaviour {
        private void Start() {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}