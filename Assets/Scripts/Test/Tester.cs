using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    public class Tester : MonoBehaviour {
        [Button("Click")]
        public void Click() { }

        private LogStopwatch _logStopwatch;

        private void Start() {
            SceneManager.LoadScene("main", LoadSceneMode.Single);
            Numerator.InitNumerator(3);
            this._logStopwatch = new("start");
        }

        private async void Update() {
            
        }
    }
}