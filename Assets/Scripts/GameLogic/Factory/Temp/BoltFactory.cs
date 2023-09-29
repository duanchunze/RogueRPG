using UnityEngine;

namespace Hsenl {
    public static class BoltFactory {
        public static Bolt Create(string boltName) {
            var prefab = ResourcesHelper.GetAsset<GameObject>(Constant.BoltBundleName, boltName);
            var go = UnityEngine.Object.Instantiate(prefab);
            go.name = boltName;
            var entity = Entity.Create(go);
            var bolt = entity.AddComponent<Bolt>();
            bolt.boltName = boltName;

            entity.Active = false;
            return bolt;
        }
    }
}