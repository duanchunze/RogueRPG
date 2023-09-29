using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    public class EventSystemProxy : MonoBehaviour {
        [SerializeField, HideLabel]
        private EventSystemManager _event;

        public List<string> assembliesFilter = new() {
            "HsenlFramework.Runtime",
            "Assembly-CSharp",
        };

        private void Awake() {
            if (!SingletonManager.IsDisposed<EventSystemManager>()) {
                SingletonManager.Unregister<EventSystemManager>();
            }

            SingletonManager.Register(ref this._event);
            this._event.RegisterAttributeType(typeof(MemoryPackableAttribute));
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => this.assembliesFilter.Contains(assembly.GetName().Name)).ToArray();
            this._event.AddAssembles(assemblies);
        }
    }
}