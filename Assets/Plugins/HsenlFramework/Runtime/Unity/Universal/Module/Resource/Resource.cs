using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YooAsset;

namespace Hsenl {
    public static class Resource {
        private static readonly StringBuilder _stringBuilderCache = new(); // 临时缓存用
        private static readonly StringBuilder _stringBuilderResult = new(); // 存储最优路径用
        private const string AssetBundleSpaceMark = "_";

        public static AssetInfo GetOptimalAssetInfo(OptimalAssetFinder finder) {
            var assetInfos = AssetManifest.GetAssetInfosOfTag(finder.tag);

            var list = finder.strs;
            var assetName = finder.assetName;
            var lowest = finder.lowest;

            _stringBuilderCache.Clear();
            _stringBuilderResult.Clear();
            var assetNameLength = assetName.Length;
            for (int i = 0, len = list.Count; i < len; i++) {
                var part = list[i];
                if (string.IsNullOrEmpty(part)) {
                    continue;
                }

                _stringBuilderCache.Append(part);
                _stringBuilderCache.Append(AssetBundleSpaceMark);
                if (i < lowest) {
                    // 自 startIndex的位置，才开始正式干活
                    // 例如一个路径：fx_a_b，如果startIndex = 1，则代表，最多查询到a的位置，没有的话就没有了，如果startIndex = 0，则会查到 fx
                    continue;
                }

                // 查询的过程就是, 先查fx_name, 没有, 则删除资源名, 然后继续下一级, fx_a_name, 没有则重复该步骤, 如果有, 则算找出来一个, 添加到stringBuilderResult中,
                // 然后继续下一级, 看有没有更合适的
                var removeIndex = _stringBuilderCache.Length;
                _stringBuilderCache.Append(assetName);
                if (!assetInfos.ContainsKey(_stringBuilderCache.ToString())) {
                    _stringBuilderCache.Remove(removeIndex, assetNameLength);
                    continue;
                }

                // 到这一步，说明这个stringBuilderCache中的组合是一个有效的组合，但还是会继续轮询下去，如果后面还有更合适的，就会顶替掉这个，如果没有，最终就会以这个为
                // 最优结果
                _stringBuilderResult.Clear();
                _stringBuilderResult.Append(_stringBuilderCache);
            }

            if (_stringBuilderResult.Length == 0) {
                return null;
            }

            var result = assetInfos[_stringBuilderResult.ToString()];
            return result;
        }

        public static UnityEngine.Object GetOptimalAsset(OptimalAssetFinder finder) {
            var assetInfo = GetOptimalAssetInfo(finder);
            if (assetInfo == null)
                return null;

            return YooAssets.LoadAssetSync(assetInfo)?.AssetObject;
        }

        public static T GetOptimalAsset<T>(OptimalAssetFinder finder) where T : UnityEngine.Object {
            return (T)GetOptimalAsset(finder);
        }
    }
}