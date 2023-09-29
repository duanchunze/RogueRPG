using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    public class ObjectPoolProxy : MonoBehaviour {
        [LabelText("对象池信息"), ReadOnly]
        public List<string> infos;

        private FieldInfo _fieldInfo;

        public bool detailInspector;

        [SerializeField, ShowIf("detailInspector"), HideLabel]
        private ObjectPoolManager objectPoolManager;

        private void Start() {
            BindingFlags flags = BindingFlags.Default;
            flags |= BindingFlags.Public;
            flags |= BindingFlags.NonPublic;
            flags |= BindingFlags.Instance;
            flags |= BindingFlags.DeclaredOnly;
            this._fieldInfo = typeof(ObjectPoolManager).GetField("_pool", flags);
        }

        private void Awake() {
            if (!SingletonManager.IsDisposed<ObjectPoolManager>()) {
                SingletonManager.Unregister<ObjectPoolManager>();
            }
            
            SingletonManager.Register(ref this.objectPoolManager);
        }

        [OnInspectorGUI]
        private void UpdateInspector() {
            if (!Application.isPlaying) {
                return;
            }

            if (this.objectPoolManager == null) {
                return;
            }

            var pool = (Dictionary<Type, Queue<object>>)this._fieldInfo.GetValue(this.objectPoolManager);
            this.infos.Clear();
            foreach (var kv in pool) {
                this.infos.Add($"{kv.Key.Name} -- {kv.Value.Count}");
            }
        }
    }
}