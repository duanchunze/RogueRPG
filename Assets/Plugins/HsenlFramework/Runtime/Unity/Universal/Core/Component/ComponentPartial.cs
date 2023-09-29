using MemoryPack;
using Sirenix.OdinInspector;
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
        
        partial void PartialOnEnableSelfChanged(bool enab) {
            if (this.Entity.GameObject == null) return;
            if (!Framework.Instance.displayMono) return;
            this.MonoBehaviour.enabled = enab;
        }
    }
}