using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hsenl {
    // 什么逻辑写在组合里, 什么逻辑写在组件本身里
    // 只和自己有关系的逻辑写在本身里
    // 牵扯到其他的组件的逻辑, 则写在组合里
    // 组合的触发不受Active影响, 无论实体是否激活, 只要组合条件符合, 就会激活组合.
    internal class MultiCombinInfo {
        public ComponentTypeCacher totalTypeCacher; // total是所有combiner中的cacher做或运算的结果
        public readonly List<Combiner> combiners = new();
    }

    internal class CrossCombinInfo {
        public ComponentTypeCacher totalChildTypeCacher;
        public readonly List<Combiner> parentCombiners = new(); // 主域作为父域的有哪些组合
        public readonly List<Combiner> childCombiners = new(); // 主域作为子域的有哪些组合
    }

    [FrameworkMember]
    public abstract class Combiner {
        internal static Dictionary<Type, Combiner> TypeCombinerMap { get; } = new();

        internal static List<Combiner> MultiCombiners { get; } = new();
        internal static ComponentTypeCacher TotalMultiCombinerTypeCacher;
        internal static List<Combiner> CrossCombiners { get; } = new();
        internal static ComponentTypeCacher TotalCrossCombinerChildTypeCacher;

        internal static MultiList<int, int> Overrides { get; } = new(); // key: 覆盖者, value: 被覆盖者
        internal static MultiList<int, int> InverseOverrides { get; } = new(); // key: 被覆盖者, value: 覆盖者

        // key: componentTypeIndex, value: 与该组件有关的所有组合, 且按照组合成员数有小到大排序
        internal static Dictionary<int, MultiCombinInfo> MultiCombinLookupTable { get; } = new();
        internal static Dictionary<int, CrossCombinInfo> CrossCombinLookupTable { get; } = new();

        [OnEventSystemInitialized]
        private static void CacheCombiner() {
            TypeCombinerMap.Clear();
            MultiCombiners.Clear();
            CrossCombiners.Clear();
            Overrides.Clear();
            InverseOverrides.Clear();
            var multiId = 0;
            var crossId = 0;
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
                        combiner.id = multiId++;
                        combiner.multiTypeCacher = Entity.CombineComponentType(combiner.memberTypes);
                        MultiCombiners.Add(combiner);
                        break;
                    }
                    case CombinerType.CrossCombiner: {
                        var optionsAtt = type.GetCustomAttribute<CombinerOptionsAttribute>(true);
                        combiner.crossChildTypeCacher = ComponentTypeCacher.CreateNull();
                        combiner.crossParentTypeCacher = ComponentTypeCacher.CreateNull();
                        combiner.id = crossId++;
                        combiner.crossChildTypeCacher = Entity.CombineComponentType(combiner.memberTypes, 0, optionsAtt.crossSplitPosition);
                        combiner.crossParentTypeCacher = Entity.CombineComponentType(combiner.memberTypes, optionsAtt.crossSplitPosition);
                        combiner.crossMaximumLayer = optionsAtt.crossMaximumLayer;
                        CrossCombiners.Add(combiner);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                TypeCombinerMap.Add(type, combiner);
            }

            TotalMultiCombinerTypeCacher = ComponentTypeCacher.CreateNull();
            foreach (var combiner in MultiCombiners) {
                TotalMultiCombinerTypeCacher.Add(combiner.multiTypeCacher);
            }

            TotalCrossCombinerChildTypeCacher = ComponentTypeCacher.CreateNull();
            foreach (var combiner in CrossCombiners) {
                TotalCrossCombinerChildTypeCacher.Add(combiner.crossChildTypeCacher);
            }

            // 缓存所有CombinerOverride
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(CombinerOverrideAttribute))) {
                var att = type.GetCustomAttribute<CombinerOverrideAttribute>(false);
                if (att != null) {
                    if (type.GetCustomAttribute<CombinerAttribute>(true).combinerType != CombinerType.MultiCombiner) {
                        // 暂时只允许multi作为覆盖者
                        throw new Exception($"CombinerOverrideAttribute only use for multi combiner '{type}'");
                    }

                    var combienr = TypeCombinerMap[type];
                    foreach (var overrideType in att.overrides) {
                        var overrideCombiner = TypeCombinerMap[overrideType];
                        Overrides.Add(combienr.id, overrideCombiner.id);
                    }
                }
            }

            // 检查CombinerOverride是否存在冲突问题(两个组合相互覆盖)
            foreach (var @override in Overrides) {
                foreach (var overrideCombiner in @override.Value) {
                    if (Overrides.TryGetValue(overrideCombiner, out var list)) {
                        if (list.Contains(@override.Key)) {
                            throw new Exception($"<override combiner conflict exists, '{@override.Key}' '{overrideCombiner}'>");
                        }
                    }

                    InverseOverrides.Add(overrideCombiner, @override.Key);
                }
            }

            // 把MultiCombiners按照规律缓存起来
            MultiCombinLookupTable.Clear();
            foreach (var multiCombiner in MultiCombiners) {
                foreach (var memberType in multiCombiner.memberTypes) {
                    var componentTypeIndex = Entity.GetComponentIndex(memberType);
                    if (!MultiCombinLookupTable.TryGetValue(componentTypeIndex, out var info)) {
                        info = new MultiCombinInfo();
                        MultiCombinLookupTable[componentTypeIndex] = info;
                    }

                    info.combiners.Add(multiCombiner);
                }
            }

            // 整理MultiCombiners
            foreach (var kv in MultiCombinLookupTable) {
                var combinInfo = kv.Value;

                combinInfo.combiners.Sort((combiner1, combiner2) => combiner1.memberTypes.Length.CompareTo(combiner2.memberTypes.Length));
                combinInfo.totalTypeCacher = ComponentTypeCacher.CreateNull();
                foreach (var combiner in combinInfo.combiners) {
                    combinInfo.totalTypeCacher.Add(combiner.multiTypeCacher);
                }
            }

            // 把CrossCombiners按照规律缓存起来
            CrossCombinLookupTable.Clear();
            foreach (var combiner in CrossCombiners) {
                foreach (var memberType in combiner.memberTypes) {
                    var componentTypeIndex = Entity.GetComponentIndex(memberType);
                    if (!CrossCombinLookupTable.TryGetValue(componentTypeIndex, out var info)) {
                        info = new CrossCombinInfo();
                        CrossCombinLookupTable[componentTypeIndex] = info;
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
            foreach (var kv in CrossCombinLookupTable) {
                var combinInfo = kv.Value;

                combinInfo.totalChildTypeCacher = ComponentTypeCacher.CreateNull();
                foreach (var combiner in combinInfo.childCombiners) {
                    combinInfo.totalChildTypeCacher.Add(combiner.crossChildTypeCacher);
                }
            }
        }

        internal CombinerType combinerType;
        internal int id; // combiner独有的id, 并非component index
        internal Type[] memberTypes;
        internal ComponentTypeCacher multiTypeCacher;
        internal ComponentTypeCacher crossChildTypeCacher;
        internal ComponentTypeCacher crossParentTypeCacher;
        internal int crossMaximumLayer;

        internal int actionCounter; // 动作计数器
        private readonly Dictionary<int, Delegate> _actions = new();
        protected readonly Dictionary<int, int> actionCounters = new(); // 记录每个形成的组合的事件数, 用以判断每个组合是不是忘了 -= action

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
            if (this._actions.ContainsKey(hashcode)) throw new Exception($"combiner is already has combined'{typeof(T)}' '{this.GetType()}'");
            this._actions.Add(hashcode, action);
            return action;
        }

        protected T DequeueAction<T>() where T : Delegate {
            var hashcode = this.GetComponentCombineHashCode();
            hashcode = HashCode.Combine(hashcode, this.actionCounter++);
            if (!this._actions.TryGetValue(hashcode, out var action)) {
                // enqueue和dequeue必须一一对应, 顺序也不能错
                throw new NullReferenceException("action is null, check whether EnqueueAction and DequeueAction one-to-one correspondenceto each other");
            }

            this._actions.Remove(hashcode);
            return (T)action;
        }
    }
}