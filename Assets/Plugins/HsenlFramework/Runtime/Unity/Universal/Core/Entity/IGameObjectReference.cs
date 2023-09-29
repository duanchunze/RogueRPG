using UnityEngine;

namespace Hsenl {
    public interface IGameObjectReference {
        public GameObject GameObject { get; }
        internal void SetUnityReference(GameObject reference);
    }
}