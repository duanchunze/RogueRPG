using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Hsenl {
    [ProtoContract]
    public class AssetsManifest {
        [ProtoContract]
        private class AssetBundleInfo {
            [ProtoMember(1)] public readonly string bundleName;
            [ProtoMember(2)] public readonly string assetName;

            // protobuf反序列化时，要求类必须有一个默认构造函数
            public AssetBundleInfo() { }

            public AssetBundleInfo(string bundleName, string assetName) {
                this.bundleName = bundleName;
                this.assetName = assetName;
            }
        }

        public const string AssetBundleSpaceMark = "/";

        public static AssetsManifest Instance { get; set; }

        private static readonly StringBuilder _stringBuilderCache = new(); // 临时缓存用
        private static readonly StringBuilder _stringBuilderResult = new(); // 存储最优路径用

        [ProtoMember(1)] private readonly Dictionary<string, AssetBundleInfo> _manifest = new(); // 清单

        /// <summary>
        /// 清单长度
        /// </summary>
        public int Length => this._manifest.Count;

        /// <summary>
        /// 登记包信息
        /// <para></para>>
        /// </summary>
        /// <param name="uniquePath">该路径必须是唯一的，相当于每个资源独一无二的key，用来确保我们不会找错资源</param>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        public void Register(string uniquePath, string bundleName, string assetName) {
            this._manifest.Add(uniquePath.ToLower(), new AssetBundleInfo(bundleName.ToLower(), assetName));
        }

        /// <summary>
        /// 尽可能获取最合适的资源包（针对复杂的资源加载情况，一般的简单加载则没必要使用这个，比如UI、场景、配置文件等）
        /// 例如, 现在有个叫跑步的动作资源run, 其资源路径可能是这样的
        /// 1、motion/hero/run
        /// 2、motion/hero/weapon/run
        /// 3、motion/hero/weapon/skin/run
        /// 可以看到每个英雄都有每个英雄不同的跑步动作, 同时, 根据各个英雄所佩戴的武器不同, 也有不同的跑步动作, 再然后, 如果装备了不同的皮肤时装, 跑步动作可能进一步定制化
        /// 但问题是, 比如有些皮肤比较贵, 我们给他制作的定制化的跑步动作, 而多数的皮肤比较便宜, 所以使用的还是通用的跑步动作
        /// 而我们想要的效果就是, 一个英雄a, 装备了武器b, 换了皮肤c, 假如有该组合下定制的跑步动作, 则加载, 没有, 就退而求其次, 加载该英雄武器下的通用跑步动作, 如果武器也没有定制化动作, 则
        /// 加载该英雄的通用动作
        /// 而我们每次获取资源名时, 只需要把自己的英雄名、武器名、皮肤名 + 最终资源名, 传入即可进行最优匹配
        /// 后续, 我们需要给某个人物或者某个皮肤制作一些定制化的资源的时候, 我们只需要增加该资源就行了, 而无需关心其他东西
        /// </summary>
        /// <param name="list">最后一位必须是资源名</param>
        /// <param name="bundleName"></param>
        /// <param name="lowest">能检测的最低位置</param>
        /// <returns></returns>
        public bool GetOptimalBundle(IList<string> list, out string bundleName, int lowest = 0) {
            bundleName = null;
            if (list.Count == 1) {
                Log.Error("if length is 1, it is not necessary to use AssetManifest");
                return false;
            }
            
            _stringBuilderCache.Clear();
            _stringBuilderResult.Clear();
            var assetName = list[^1].ToLower(); // ^1代表最后一位索引，等效于list.count - 1;
            var assetNameLength = assetName.Length;
            for (int i = 0, len = list.Count - 1; i < len; i++) {
                var part = list[i];
                if (string.IsNullOrEmpty(part)) {
                    continue;
                }

                _stringBuilderCache.Append(part.ToLower());
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

                if (!this._manifest.ContainsKey(_stringBuilderCache.ToString())) {
                    _stringBuilderCache.Remove(removeIndex, assetNameLength);
                    continue;
                }

                // 到这一步，说明这个stringBuilderCache中的组合是一个有效的组合，但还是会继续轮询下去，如果后面还有更合适的，就会顶替掉这个，如果没有，最终就会以这个为
                // 最优结果
                _stringBuilderResult.Clear();
                _stringBuilderResult.Append(_stringBuilderCache);
            }

            if (_stringBuilderResult.Length == 0) {
                return false;
            }

            var assetBundleInfo = this._manifest[_stringBuilderResult.ToString()];
            bundleName = assetBundleInfo.bundleName;
            return true;
        }
    }
}