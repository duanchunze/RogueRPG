using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public static class BoltFactory {
        public static Bolt Create(string bundleName, string boltName) {
            ResourcesHelper.Clear();
            var prefab = ResourcesHelper.GetAsset<GameObject>(bundleName, boltName);
            var go = UnityEngine.Object.Instantiate(prefab);
            go.name = boltName;
            var entity = Entity.Create(go);
            var bolt = entity.AddComponent<Bolt>();

            entity.Active = false;
            return bolt;
        }
    }
}