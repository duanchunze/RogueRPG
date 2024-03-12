using Hsenl.bolt;
using UnityEngine;
using YooAsset;

namespace Hsenl.View {
    [ShadowFunction(typeof(BoltFactory))]
    public static partial class BoltFactory_Shadow {
        [ShadowFunction]
        private static void CreateBolt(Bolt bolt) {
            var config = bolt.Config;
            if (!string.IsNullOrEmpty(config.ColliderName)) {
                if (YooAssets.LoadAssetAsync(config.ColliderName)?.AssetObject is GameObject prefab) {
                    UnityEngine.Object.Instantiate(prefab, bolt.UnityTransform, false);
                    bolt.AddComponent<CollisionEventListener>();
                }
            }

            bolt.AddComponent<Model>();
        }
    }
}