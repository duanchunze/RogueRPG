using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Hsenl {
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class UnityOdinSingleton<T> : UnitySingleton<T>, ISerializationCallbackReceiver, ISupportsPrefabSerialization
        where T : UnityOdinSingleton<T> {
        [SerializeField]
        [HideInInspector]
        private SerializationData serializationData;

        SerializationData ISupportsPrefabSerialization.SerializationData {
            get => this.serializationData;
            set => this.serializationData = value;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            this.OnBeforeSerialize();
            UnitySerializationUtility.SerializeUnityObject((UnityEngine.Object)this, ref this.serializationData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            UnitySerializationUtility.DeserializeUnityObject((UnityEngine.Object)this, ref this.serializationData);
            this.OnAfterDeserialize();
        }

        /// <summary>Invoked before serialization has taken place.</summary>
        protected virtual void OnBeforeSerialize() { }

        /// <summary>Invoked after deserialization has taken place.</summary>
        protected virtual void OnAfterDeserialize() { }

#if UNITY_EDITOR
        [HideInTables]
        [OnInspectorGUI]
        [PropertyOrder(-2.147484E+09f)]
        private void InternalOnInspectorGUI() => EditorOnlyModeConfigUtility.InternalOnInspectorGUI((UnityEngine.Object)this);
#endif
    }
}