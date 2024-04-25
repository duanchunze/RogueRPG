using Hsenl.bolt;
using UnityEngine;
using YooAsset;

namespace Hsenl.View {
    [ShadowFunction(typeof(BoltFactory))]
    public static partial class BoltFactory_Shadow {
        [ShadowFunction]
        private static void CreateBolt(Bolt bolt) {
            bolt.AddComponent<Model>();
        }
    }
}