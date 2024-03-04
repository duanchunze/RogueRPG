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
            "HsenlFramework.Editor"
        };

        private const string basicAssemblyName = "HsenlFramework.Runtime";

        private void Awake() {
            if (!SingletonManager.IsDisposed<EventSystemManager>()) {
                SingletonManager.Unregister<EventSystemManager>();
            }

            SingletonManager.Register(ref this._event);
            this._event.RegisterAttributeType(typeof(MemoryPackableAttribute));

            var basicAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == basicAssemblyName);
            if (basicAssembly == null)
                throw new Exception("Can't find HsenlFramework.Runtime assembly!");
                
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => { return x.FullName == basicAssembly.FullName || x.GetReferencedAssemblies()
                    .Any(xx => xx.FullName == basicAssembly.FullName); })
                .Where(x => !this.assembliesFilter.Contains(x.GetName().Name))
                .ToArray();
            this._event.AddAssembles(assemblies);
        }
    }
}