using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public class GizmosDebug : MonoBehaviour {
        public static GizmosDebug Instance { get; private set; }

        public List<UnityEngine.Vector3> Path;

        private void Awake() {
            Instance = this;
        }

        private void OnDrawGizmos() {
            if (this.Path.Count < 2) {
                return;
            }

            for (var i = 0; i < Path.Count - 1; ++i) {
                Gizmos.DrawLine(Path[i], Path[i + 1]);
            }
        }
    }
}