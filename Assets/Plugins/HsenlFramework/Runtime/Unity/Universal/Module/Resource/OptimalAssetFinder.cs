using System;
using System.Collections.Generic;

namespace Hsenl {
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
    public class OptimalAssetFinder : IDisposable {
        internal readonly List<string> strs = new(); // 资源所有前级的列表
        internal string assetName; // 目标资源的名字
        internal string tag; // 标签
        public int lowest = 0; // 最低找到哪集目录

        public static OptimalAssetFinder Create(string assetName, string tag) {
            var finder = ObjectPool.Rent<OptimalAssetFinder>();
            finder.assetName = assetName;
            finder.tag = tag;

            return finder;
        }

        public static void Return(OptimalAssetFinder finder) {
            finder.Clear();
            ObjectPool.Return(finder);
        }

        public void Append(string str) {
            this.strs.Add(str);
        }

        public void Clear() {
            this.strs.Clear();
            this.assetName = null;
            this.tag = null;
            this.lowest = 0;
        }

        public void Dispose() {
            Return(this);
        }
    }
}