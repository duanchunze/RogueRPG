using System.Collections.Generic;
using YooAsset;

namespace Hsenl {
    public class AssetManifest {
        private static readonly AssetManifest _instance = new();

        private readonly Dictionary<string, Dictionary<string, AssetInfo>> _manifest = new(); // key1: tag, key2: address

        public static Dictionary<string, AssetInfo> GetAssetInfosOfTag(string tag) {
            if (!_instance._manifest.TryGetValue(tag, out var dict)) {
                dict = new Dictionary<string, AssetInfo>();
                _instance._manifest[tag] = dict;
                foreach (var assetInfo in YooAssets.GetAssetInfos(tag)) {
                    dict[assetInfo.Address] = assetInfo;
                }
            }

            return dict;
        }
    }
}