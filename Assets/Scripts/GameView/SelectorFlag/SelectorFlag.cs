using UnityEngine;

namespace Hsenl.View {
    public class SelectorFlag : Unbodied, IUpdate {
        private GameObject flag;
        private SelectorDefault _selector;

        protected override void OnAwake() {
            this.flag = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.DontDestroyOnLoad(this.flag);
            var localScale = Vector3.One * 0.1f;
            localScale.y = 9f;
            this.flag.transform.localScale = localScale;
        }

        public void Update() {
            if (this.flag == null)
                return;

            this._selector ??= this.GetComponent<SelectorDefault>();
            if (this._selector == null) {
                this.flag.SetActive(false);
                return;
            }

            if (this._selector.PrimaryTarget == null) {
                this.flag.SetActive(false);
                return;
            }

            this.flag.SetActive(true);
            this.flag.transform.position = this._selector.PrimaryTarget.transform.Position;
        }

        protected override void OnDestroy() {
            this._selector = null;
            UnityEngine.Object.Destroy(this.flag);
        }
    }
}