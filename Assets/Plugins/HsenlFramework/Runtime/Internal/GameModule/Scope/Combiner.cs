using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hsenl {
    // 什么逻辑写在组合里, 什么逻辑写在组件本身里
    // 只和自己有关系的逻辑写在本身里
    // 牵扯到其他的组件的逻辑, 则写在组合里
    // 组合的触发不受Active影响, 无论实体是否激活, 只要组合条件符合, 就会激活组合.

    [FrameworkMember]
    public abstract class Combiner {
        private static readonly Dictionary<Type, Combiner> _allCombiners = new();
        private static ComponentTypeCacher _singleCombinerCacher; // 所有的单例组合, 以位列表的形式缓存
        private static readonly Dictionary<int, Combiner> _singleCombiners = new(); // key: component index
        private static readonly List<Combiner> _multiCombiners = new();
        private static readonly List<Combiner> _crossCombiners = new();

        private static readonly MultiList<Combiner, Combiner> _overrides = new(); // key: 覆盖者, value: 被覆盖者
        private static readonly MultiList<Combiner, Combiner> _inverseOverrides = new(); // key: 被覆盖者, value: 覆盖者

        public static ComponentTypeCacher SingleCombinerCacher => _singleCombinerCacher;
        public static Dictionary<int, Combiner> SingleCombiners => _singleCombiners;
        public static List<Combiner> MultiCombiners => _multiCombiners;
        public static List<Combiner> CrossCombiners => _crossCombiners;

        public static Dictionary<Combiner, List<Combiner>> Overrides => _overrides;
        public static Dictionary<Combiner, List<Combiner>> InverseOverrides => _inverseOverrides;

        [OnEventSystemInitialized]
        private static void CacheCombiner() {
            _allCombiners.Clear();
            _singleCombinerCacher = null;
            _singleCombiners.Clear();
            _multiCombiners.Clear();
            _crossCombiners.Clear();
            _overrides.Clear();
            _inverseOverrides.Clear();
            var singleIndex = 0;
            var multiIndex = 0;
            var crossIndex = 0;
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(CombinerAttribute))) {
                var att = type.GetCustomAttribute<CombinerAttribute>(true);
                var combiner = (Combiner)Activator.CreateInstance(type);
                _allCombiners.Add(type, combiner);
                var genericType = AssemblyHelper.GetGenericBaseClass(type);
                if (genericType == null) throw new NullReferenceException($"combiner must inherit from a generic combiner '{type.Name}'");
                var genericParas = genericType.GetGenericArguments();
                switch (att.combinerType) {
                    case CombinerType.SingleCombiner: {
                        combiner.id = singleIndex++;
                        _singleCombinerCacher ??= ComponentTypeCacher.CreateNull();
                        Entity.CombineComponentType(_singleCombinerCacher, genericParas);
                        var componentIndex = Entity.GetComponentIndex(genericParas[0]);
                        _singleCombiners.Add(componentIndex, combiner);
                        break;
                    }
                    case CombinerType.MultiCombiner: {
                        combiner.id = multiIndex++;
                        combiner.allTypeCacher = Entity.CombineComponentType(genericParas);
                        _multiCombiners.Add(combiner);
                        break;
                    }
                    case CombinerType.CrossCombiner: {
                        combiner.childTypeCacher = ComponentTypeCacher.CreateNull();
                        combiner.parentTypeCacher = ComponentTypeCacher.CreateNull();
                        combiner.id = crossIndex++;
                        if (att.splitPosition != -1) {
                            // 情况1, 用户指定了主次类型的拆分位置
                            var headArgType = CombinerArgType.Main;
                            for (int i = 0, len = genericParas.Length; i < len; i++) {
                                var paraType = genericParas[i];
                                if (i == att.splitPosition) headArgType = CombinerArgType.Sub;
                                switch (headArgType) {
                                    case CombinerArgType.Main:
                                        Entity.CombineComponentType(combiner.childTypeCacher, paraType);
                                        break;
                                    case CombinerArgType.Sub:
                                        Entity.CombineComponentType(combiner.parentTypeCacher, paraType);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                        else {
                            // 情况2, 默认第一位为主, 其他为次来处理
                            combiner.childTypeCacher = Entity.CombineComponentType(genericParas, 0, 1);
                            combiner.parentTypeCacher = Entity.CombineComponentType(genericParas, 1);
                        }

                        _crossCombiners.Add(combiner);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(CombinerOverrideAttribute))) {
                var att = type.GetCustomAttribute<CombinerOverrideAttribute>(false);
                if (att != null) {
                    var combienr = _allCombiners[type];
                    if (type.GetCustomAttribute<CombinerAttribute>(true).combinerType != CombinerType.MultiCombiner) {
                        throw new Exception($"CombinerOverrideAttribute only use for multi combiner '{type}'");
                    }

                    foreach (var overrideType in att.overrides) {
                        var overrideCombiner = _allCombiners[overrideType];
                        _overrides.Add(combienr, overrideCombiner);
                    }
                }
            }

            foreach (var @override in _overrides) {
                foreach (var overrideCombiner in @override.Value) {
                    if (_overrides.TryGetValue(overrideCombiner, out var list)) {
                        if (list.Contains(@override.Key)) {
                            throw new Exception($"<override combiner conflict exists, '{@override.Key}' '{overrideCombiner}'>");
                        }
                    }

                    _inverseOverrides.Add(overrideCombiner, @override.Key);
                }
            }
        }

        public int id; // combiner独有的id, 并非component index
        public ComponentTypeCacher allTypeCacher;
        public ComponentTypeCacher childTypeCacher;
        public ComponentTypeCacher parentTypeCacher;
        internal int actionCounter; // 动作计数器

        private readonly Dictionary<int, Delegate> _actions = new();

        internal virtual void OnCombin(Component component) {
            this.actionCounter = 0;
        }

        internal virtual void OnDecombin(Component component) {
            this.actionCounter = 0;
        }

        internal virtual void OnCombin(IList<Component> components) {
            this.actionCounter = 0;
        }

        internal virtual void OnDecombin(IList<Component> components) {
            this.actionCounter = 0;
        }

        protected abstract int GetComponentCombineHashCode();

        protected T EnqueueAction<T>(T action) where T : Delegate {
            var hashcode = this.GetComponentCombineHashCode();
            hashcode = HashCode.Combine(hashcode, this.actionCounter++);
            if (this._actions.ContainsKey(hashcode)) throw new Exception($"action is already has '{typeof(T)}' '{this.GetType()}'");
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