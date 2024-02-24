using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    [FrameworkMember]
    public static class MonoComponentManager {
        private static readonly Dictionary<Type, Type> _caches = new(); // key: hsenl component, value: mono component

        [OnEventSystemInitialized]
        private static void Cache() {
            _caches.Clear();

            var monoComponentTypes = AssemblyHelper.GetSubTypes(typeof(IHsenlComponentReference));
            foreach (var monoComponentType in monoComponentTypes) {
                var iface = AssemblyHelper.GetBaseInterface(monoComponentType, typeof(IHsenlComponentReference<>));
                if (iface == null) {
                    continue;
                }

                var genericArguments = iface.GetGenericArguments();
                var hsenlComponentType = genericArguments[0];
                _caches.Add(hsenlComponentType, monoComponentType);
            }
        }

        public static Type GetMonoComponentType(Type frameworkComponentType) {
            if (!_caches.TryGetValue(frameworkComponentType, out var result)) {
                return null;
                // throw new Exception($"cant find mono component by '{frameworkComponentType.Name}'");
            }

            return result;
        }
    }
}