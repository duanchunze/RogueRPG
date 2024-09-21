using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public partial class Component : IMonoBehaviourReference {
        [MemoryPackIgnore]
        private MonoBehaviour MonoBehaviour { get; set; }

        void IMonoBehaviourReference.SetUnityReference(MonoBehaviour reference) {
            this.MonoBehaviour = reference;
        }

        [MemoryPackIgnore]
        public UnityEngine.Transform UnityTransform => this.Entity.UnityTransform;

#if UNITY_EDITOR
        partial void PartialOnEnableSelfChanged(bool enab) {
            if (this.Entity.GameObject == null) return;
            if (!Framework.Instance.DisplayMonoComponent) return;
            this.MonoBehaviour.enabled = enab;
        }

        partial void PartialOnDestroyFinish() {
            if (this.MonoBehaviour != null) {
                var mono = this.MonoBehaviour;
                mono.GetComponent<IHsenlComponentReference>().SetFrameworkReference(null);
                this.MonoBehaviour = null;
                UnityEngine.Object.Destroy(mono);
            }
        }
#endif
    }
}