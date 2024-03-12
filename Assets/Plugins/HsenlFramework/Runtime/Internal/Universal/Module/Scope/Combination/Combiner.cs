using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hsenl {
    /* 什么逻辑写在组合里, 什么逻辑写在组件本身里
     * 只和自己有关系的逻辑写在本身里
     * 牵扯到其他的组件的逻辑, 则写在组合里
     * 组合的触发不受Active影响, 无论实体是否激活, 只要组合条件符合, 就会激活组合.
     */
    internal class MultiCombinInfo {
        public ComponentTypeCacher totalTypeCacher; // total是所有combiner中的cacher做或运算的结果
        public readonly List<Combiner> combiners = new();
    }

    internal class CrossCombinInfo {
        public ComponentTypeCacher totalChildTypeCacher;
        public readonly List<Combiner> parentCombiners = new(); // 主域作为父域的有哪些组合
        public readonly List<Combiner> childCombiners = new(); // 主域作为子域的有哪些组合
    }

    /* 组合器
     * 定义一个组合器, 那么当组件满足该条件时, 就会自动形成一个组合, 同样, 当组件不再满足该条件时, 组合也会被自动断开.
     * 示例: 比如你定义了一个组合MultiCombiner<A, B>, 那么当A和B组件同时存在的时候, 该组合就形成了, 如果此时B被移除了, 那该组合也自然被断开了.
     *
     * 条件:
     *  - 所有组合必须发生在scope下(也就是说, 该实体上至少有且只能有一个scope)
     *  - 参与组合的组件要么是scope, 要么是一个element, 否则不会有效
     *  - 之所以要限制scope和element为的是不污染原本的entity系统, 用户依然可以选择纯净的entity系统.
     *
     * 心路历程:
     * 编程中, 为了实现更好的模块化, 我们经常需要把功能尽可能的进行细化然后放到不同的组件中, 而不是统统塞在一个组件中. 如此就出现了一个问题, 组件与组件之间的相互引用问题
     * 变的非常严重, 虽然是不同的组件, 但却常常需要互相引用. 这就与我们的初衷背道而驰了, 我们细化组件的目的就是为了尽量的解耦, 现在因为相互引用的问题, 又导致我们的逻辑写的粘黏到了一起.
     * 于是为了解决这个问题, 诞生了这种以组合的形式来构建组件与组件之间的新关系. 形成组合时, 双方有联系, 断开组合, 双方就毫无关系, 双方都干干净净, 不需要声明对方组件的引用.
     * 一开始, 只有MultiCombiner, 也就是在单个scope下进行的组合, 后来有拓展出了CrossCombiner, 也就是在scope与scope之间父子关系中, 进行跨域组合.
     *
     * 以一个功能为例, 技能模块, 技能能释放, 于是我们要写一个施法功能, 但不是只有技能可以释放, 道具也能释放, 那现在有几种解决方案
     *  - 写一个释放基类, 显然这不是一个好选择, 实际上, 任何时候, 我们都应该尽量避免使用继承
     *  - 写一个释放接口, 这是一个不错的方案, 但实际上, 接口只是更灵活的继承罢了, 依然不能实现组件与组件之间的干干净净, 特别在功能复杂后, 一个组件可能要继承一大堆接口, 这些都是明码写在脚本里的,
     *    接口合适, 但不是最合适的
     *  - 把所有的能释放的统一定为技能, 道具释放的叫道具技能, 装备释放的叫装备技能. 显然这个方案虽然可行, 但同时也相当于给自己锁死了, 且这也需要看策划的实际需求.
     * 同时, 技能既然能释放, 代表他还需要和控制器组件配合, 所以一个技能需要和技能栏有联系, 需要和控制器有联系, 那问题就来了, 这个和控制器的联系由谁去做? 我们可以在技能栏里写一个方法,
     * EquipAbility(Ability abi). 那谁把abi再添加到控制器的控制列表里? 写在技能栏里显然不合适, 那只能在添加到技能栏的时候, 同时也添加到控制器里, 或者让技能栏和控制器做绑定. 但无论怎么做,
     * 都有一个问题, 就是我们必须在一开始的时候, 就把这些东西都处理好, 且不要去乱动他们, 比如你在运行过程中删除一个控制器组件, 这是危险的, 因为你可能不清楚这个控制器组件和多少组件有关联, 你
     * 必修保证这些关联你都处理好了, 你才能安全的删除控制器组件. 而组合器系统就完美的解决了这个问题, 你只需要定义好各种组合的条件, 在形成组合与断开组合时, 需要做什么事, 剩下的事情, 都不需要多
     * 过问.
     *
     * 优点:
     *  - 最大程度的组件解耦, 让组件之间的联系清晰明了
     *  - 大大增加工程的扩展能力, 写在combiner中的逻辑可以随时注释替换, 非常方便, 逻辑也可以很方便的复用, 全都集中在combiner的定义里.
     * 缺点:
     *  - 需要大量的委托实现组件与组件之间的联动, 也就存在内存泄露的风险, 不过组合器系统已经写了比较完善的检测机制, 能帮助使用者避免委托带来的内存泄露问题.
     *  - cross combiner
     *
     */
    [FrameworkMember]
    public abstract class Combiner {
        private static CombinerCache _cache;

        internal static CombinerCache CombinerCache => _cache;

        [OnEventSystemInitialized]
        private static void CacheCombiner() {
            _cache = new CombinerCache();
            var uniqueId = 0;
            // 缓存所有Combiner
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(CombinerAttribute))) {
                var att = type.GetCustomAttribute<CombinerAttribute>(true);
                var combiner = (Combiner)Activator.CreateInstance(type);
                var genericType = AssemblyHelper.GetGenericBaseClass(type);
                if (genericType == null)
                    throw new NullReferenceException($"combiner must inherit from a generic combiner '{type.Name}'");
                combiner.memberTypes = genericType.GetGenericArguments();
                foreach (var memberType in combiner.memberTypes) {
                    if (!memberType.IsSubclassOf(typeof(Scope)) && !memberType.IsSubclassOf(typeof(Unbodied))) {
                        throw new Exception($"combiner member must inherit from <Scope> or <Unbodied> '{memberType.Name}'");
                    }
                }

                combiner.combinerType = att.combinerType;
                switch (combiner.combinerType) {
                    case CombinerType.MultiCombiner: {
                        combiner.id = uniqueId++;
                        combiner.multiTypeCacher = Entity.CombineComponentType(combiner.memberTypes);
                        _cache.MultiCombiners.Add(combiner);
                        break;
                    }
                    case CombinerType.CrossCombiner: {
                        var optionsAtt = type.GetCustomAttribute<CombinerOptionsAttribute>(true);
                        combiner.crossChildTypeCacher = ComponentTypeCacher.CreateNull();
                        combiner.crossParentTypeCacher = ComponentTypeCacher.CreateNull();
                        combiner.id = uniqueId++;
                        combiner.crossChildTypeCacher = Entity.CombineComponentType(combiner.memberTypes, 0, optionsAtt.crossSplitPosition);
                        combiner.crossParentTypeCacher = Entity.CombineComponentType(combiner.memberTypes, optionsAtt.crossSplitPosition);
                        combiner.crossMaximumLayer = optionsAtt.crossMaximumLayer;
                        _cache.CrossCombiners.Add(combiner);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _cache.TypeCombinerMap.TryAdd(type, combiner);
                _cache.TotalCombiners.Add(combiner);
            }

            _cache.TotalMultiCombinerTypeCacher = ComponentTypeCacher.CreateNull();
            foreach (var combiner in _cache.MultiCombiners) {
                _cache.TotalMultiCombinerTypeCacher.Add(combiner.multiTypeCacher);
            }

            _cache.TotalCrossCombinerChildTypeCacher = ComponentTypeCacher.CreateNull();
            foreach (var combiner in _cache.CrossCombiners) {
                _cache.TotalCrossCombinerChildTypeCacher.Add(combiner.crossChildTypeCacher);
            }

            // 缓存所有CombinerOverride
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(CombinerOverrideAttribute))) {
                var att = type.GetCustomAttribute<CombinerOverrideAttribute>(false);
                if (att != null) {
                    if (type.GetCustomAttribute<CombinerAttribute>(true).combinerType != CombinerType.MultiCombiner) {
                        // 暂时只允许multi作为覆盖者
                        throw new Exception($"CombinerOverrideAttribute only use for multi combiner '{type}'");
                    }

                    var combienr = _cache.TypeCombinerMap[type];
                    foreach (var overrideType in att.overrides) {
                        var overrideCombiner = _cache.TypeCombinerMap[overrideType];
                        _cache.Overrides.Add(combienr.id, overrideCombiner.id);
                    }
                }
            }

            // 检查CombinerOverride是否存在冲突问题(两个组合相互覆盖)
            foreach (var kv in _cache.Overrides) {
                foreach (var overrideUniqueId in kv.Value) {
                    if (_cache.Overrides.TryGetValue(overrideUniqueId, out var list)) {
                        if (list.Contains(kv.Key)) {
                            var combiner1 = _cache.TotalCombiners[kv.Key];
                            var combiner2 = _cache.TotalCombiners[overrideUniqueId];
                            throw new Exception(
                                $"<override combiner conflict exists, '{combiner1.GetType().Name} - {combiner1.id}' '{combiner1.GetType().Name} - {combiner2.id}'>");
                        }
                    }

                    _cache.InverseOverrides.Add(overrideUniqueId, kv.Key);
                }
            }

            // 把MultiCombiners按照规律缓存起来
            _cache.MultiCombinLookupTable.Clear();
            foreach (var multiCombiner in _cache.MultiCombiners) {
                foreach (var memberType in multiCombiner.memberTypes) {
                    var componentTypeIndex = Entity.GetComponentIndex(memberType);
                    if (!_cache.MultiCombinLookupTable.TryGetValue(componentTypeIndex, out var info)) {
                        info = new MultiCombinInfo();
                        _cache.MultiCombinLookupTable[componentTypeIndex] = info;
                    }

                    info.combiners.Add(multiCombiner);
                }
            }

            // 整理MultiCombiners
            foreach (var kv in _cache.MultiCombinLookupTable) {
                var combinInfo = kv.Value;

                combinInfo.combiners.Sort((combiner1, combiner2) => combiner1.memberTypes.Length.CompareTo(combiner2.memberTypes.Length));
                combinInfo.totalTypeCacher = ComponentTypeCacher.CreateNull();
                foreach (var combiner in combinInfo.combiners) {
                    combinInfo.totalTypeCacher.Add(combiner.multiTypeCacher);
                }
            }

            // 把CrossCombiners按照规律缓存起来
            _cache.CrossCombinLookupTable.Clear();
            foreach (var combiner in _cache.CrossCombiners) {
                foreach (var memberType in combiner.memberTypes) {
                    var componentTypeIndex = Entity.GetComponentIndex(memberType);
                    if (!_cache.CrossCombinLookupTable.TryGetValue(componentTypeIndex, out var info)) {
                        info = new CrossCombinInfo();
                        _cache.CrossCombinLookupTable[componentTypeIndex] = info;
                    }

                    if (combiner.crossParentTypeCacher.Contains(componentTypeIndex)) {
                        info.parentCombiners.Add(combiner);
                    }

                    if (combiner.crossChildTypeCacher.Contains(componentTypeIndex)) {
                        info.childCombiners.Add(combiner);
                    }
                }
            }

            // 整理CrossCombiners
            foreach (var kv in _cache.CrossCombinLookupTable) {
                var combinInfo = kv.Value;

                combinInfo.totalChildTypeCacher = ComponentTypeCacher.CreateNull();
                foreach (var combiner in combinInfo.childCombiners) {
                    combinInfo.totalChildTypeCacher.Add(combiner.crossChildTypeCacher);
                }
            }

            ScopeCombinFormatter.Init();
        }

        internal CombinerType combinerType;
        internal int id; // 唯一id, multi和cross之间也不会重复
        internal Type[] memberTypes;
        internal ComponentTypeCacher multiTypeCacher;
        internal ComponentTypeCacher crossChildTypeCacher;
        internal ComponentTypeCacher crossParentTypeCacher;
        internal int crossMaximumLayer;

        internal int actionCounter; // 事件计数器
        private readonly Dictionary<int, Delegate> _actions = new();
        protected readonly Dictionary<int, int> actionCounters = new(); // 记录每个形成的组合的事件数, 用以判断每个组合是不是忘了 -= action

        private readonly Dictionary<int, object> _userTokens = new();

        internal virtual void Combin(IList<Component> components) {
            this.actionCounter = 0;
        }

        internal virtual void Decombin(IList<Component> components) {
            this.actionCounter = 0;
        }

        protected abstract int GetComponentCombineHashCode();

        protected T EnqueueAction<T>(T action) where T : Delegate {
            var hashcode = this.GetComponentCombineHashCode();
            hashcode = HashCode.Combine(hashcode, this.actionCounter++);
            if (!this._actions.TryAdd(hashcode, action))
                throw new Exception($"combiner is already has combined'{typeof(T)}' '{this.GetType()}'");
            return action;
        }

        protected T DequeueAction<T>() where T : Delegate {
            var hashcode = this.GetComponentCombineHashCode();
            hashcode = HashCode.Combine(hashcode, this.actionCounter++);
            if (!this._actions.Remove(hashcode, out var action)) {
                // enqueue和dequeue必须一一对应, 顺序也不能错
                throw new NullReferenceException("action is null, check whether EnqueueAction and DequeueAction one-to-one correspondenceto each other");
            }

            return (T)action;
        }

        protected void SetUserToken(object o) {
            var hashcode = this.GetComponentCombineHashCode();
            this._userTokens[hashcode] = o;
        }

        protected object GetUserToken() {
            var hashcode = this.GetComponentCombineHashCode();
            this._userTokens.Remove(hashcode, out var value);
            return value;
        }

        protected T GetUserToken<T>() {
            return (T)this.GetUserToken();
        }
    }

    internal class CombinerCache {
        internal Dictionary<Type, Combiner> TypeCombinerMap { get; } = new();

        internal List<Combiner> MultiCombiners { get; } = new();
        internal ComponentTypeCacher TotalMultiCombinerTypeCacher;
        internal List<Combiner> CrossCombiners { get; } = new();
        internal ComponentTypeCacher TotalCrossCombinerChildTypeCacher;
        internal List<Combiner> TotalCombiners { get; } = new(); // combiner的id就是index, 所以可以直接通过 TotalCombiners[combiner.id] 来获取到该combiner

        internal MultiList<int, int> Overrides { get; } = new(); // key: 覆盖者id, value: 被覆盖者id
        internal MultiList<int, int> InverseOverrides { get; } = new(); // key: 被覆盖者id, value: 覆盖者id

        // key: componentTypeIndex, value: 与该组件有关的所有组合, 且按照组合成员数有小到大排序
        internal Dictionary<int, MultiCombinInfo> MultiCombinLookupTable { get; } = new();
        internal Dictionary<int, CrossCombinInfo> CrossCombinLookupTable { get; } = new();

        internal void Clear() {
            this.TypeCombinerMap.Clear();
            this.MultiCombiners.Clear();
            this.CrossCombiners.Clear();
            this.TotalCombiners.Clear();
            this.Overrides.Clear();
            this.InverseOverrides.Clear();
        }
    }
}