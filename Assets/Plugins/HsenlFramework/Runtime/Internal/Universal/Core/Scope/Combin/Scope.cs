﻿using System;
using System.Collections.Generic;
using MemoryPack;

// ReSharper disable MemberCanBePrivate.Global

namespace Hsenl {
    // 一个域, 一个具体的逻辑范围
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Scope : Component {
        private static readonly List<Component> _componentCache = new();

        // 父域与子域之间也可以看做是一种<御统和被御统的关系>
        [MemoryPackIgnore]
        internal Scope parentScope;

        [MemoryPackIgnore]
        internal List<Scope> childrenScopes;

        [MemoryPackIgnore]
        internal readonly Dictionary<int, Element> elements = new(); // 单领域下, 组件不可以重复, 所以, 单领域下每种组合最多存在一个. 如果有需要多组件的情况, 可以使用子域来解决

        [MemoryPackIgnore]
        internal readonly Queue<int> multiMatchs = new();

        [MemoryPackIgnore]
        internal readonly CrossMatchQueue parentCrossMatchQueue = new();

        [MemoryPackIgnore]
        internal readonly CrossMatchQueue childCrossMatchQueue = new();

        [MemoryPackIgnore]
        internal int maximumFormatterCrossLayer = 1; // 每当父级发生改变时(包括父级以上发生改变时), 会算出一个在当前父子链下, 最大跨域层数, 默认为1

        [MemoryPackIgnore]
        public virtual Scope ParentScope {
            get => this.parentScope;
            internal set {
                if (this.parentScope == value)
                    return;

                this.OnBeforeParentScopeChangeInternal(value);

                // 确定父子关系
                var prevParent = this.parentScope;
                this.parentScope = value;

                if (prevParent != null) {
                    if (!prevParent.childrenScopes.Remove(this)) throw new Exception("???");
                    if (prevParent.childrenScopes.Count == 0)
                        prevParent.childrenScopes = null;
                }

                if (value != null) {
                    value.childrenScopes ??= new();
                    value.childrenScopes.Add(this);
                }

                if (prevParent != null) {
                    CrossDecombinMatchForParent(this, prevParent);
                    this.ForeachChildrenScope((childScope, _) => {
                        CrossDecombinMatchForParent(childScope, prevParent); // 
                    });
                }

                // 确立好父子关系后再进行跨域匹配, 保证形成组合的时候, 父子关系是正确的.
                if (value != null) {
                    this.CalcMaximumCrossLayerInTheory();
                    CrossCombinMatchForParent(this, value, 1);

                    this.ForeachChildrenScope((childScope, layer) => {
                        childScope.CalcMaximumCrossLayerInTheory();
                        CrossCombinMatchForParent(childScope, value, layer + 1); //
                    });
                }

                // 先匹配再触发事件, 可以保证当用户使用该事件时, 所有该做的组合都已经完成了, 保证了组合的优先性(注: 组合的优先级只比添加组件时的initializeInvoke和序列化的OnDeserialized低)
                this.OnParentScopeChangedInternal(prevParent);
                prevParent?.OnChildScopeRemoveInternal(this);
                value?.OnChildScopeAddInternal(this);
            }
        }

        internal override void OnDeserializedInternal() {
            this.Entity.ForeachComponents(component => {
                if (component is not Element element)
                    return;

                this.elements.Add(component.ComponentIndex, element);
            });
        }

        internal override void OnConstructionInternal() {
            // 创建的第一时间进行multi匹配, 因为这种组合不依赖父子关系
            MultiCombinMatch(this, null);

            // 确立上下的父子关系, 并进行匹配及事件触发
            this.ParentScope = this.Parent?.GetComponentInParent<Scope>(true);

            // 域被创建有两种种情况, 一种是通过new创建, 第二种是通过instantiate实例化
            // 第一种情况: 由于通过new创建一个域的时候, 我们不能确定该域具体是什么状况, 比如只有父域, 或者只有子域, 或者有子域也有父域, 自己属于是插入者的情况, 所以这种情况, 我们要把父域和
            //      子域通通设置一番
            // 第二种情况: 由于实例化一个实体事件回调是自上而下的, 所以, 每个域都只需向上做匹配组合就行了, 最后自然都做一次匹配了
            if (!this.IsDeserialized) {
                this.SearchChildScopes();
            }
        }

        internal override void OnDestroyInternal() {
            // 牵扯到跨域的销毁有两种情况, 一种是整个域被销毁(连同该域及其所有子域), 第二种是单单该域被销毁(仅仅该域Scope组件被Destroy)
            // 第一种情况: 由于销毁的顺序是自下而上的, 所以, 每个子域在被销毁时, 都只需要向父域断开组合就行了, 最后所有的跨域组合自然都断开了
            // 第二种情况: 由于单单自己被销毁了, 所以相当于自己从原来的父子域链中被抽出了, 所以自己可能是存在子域的, 这时, 不仅自己要和所有子域断开组合, 还需要把原本自己的子域交给自己的父域
            // 可以根据自己实体的 imminentDispose 状态来判断是哪种情况, 如果自己的实体没有被即将销毁, 那就说明只有自己被销毁了
            if (!this.Entity.imminentDispose) {
                // 把本来自己的子域, 移交给自己的父域, 让他们重新做组合
                if (this.childrenScopes != null) {
                    for (int i = this.childrenScopes.Count - 1; i >= 0; i--) {
                        var childScope = this.childrenScopes[i];
                        childScope.ParentScope = this.parentScope;
                    }
                }
            }

            // 域被销毁的时候, 只断开组合就行了, 其他的都不用做
            MultiDecombinMatch(this, null);
            if (this.parentScope != null) {
                CrossDecombinMatchForParent(this, this.parentScope);
                this.ForeachChildrenScope((childScope, _) => {
                    CrossDecombinMatchForParent(childScope, this.parentScope); // 
                });
            }
        }

        internal override void OnComponentAddInternal(Component component) {
            if (component is Scope) throw new Exception($"one entity only one scope. '{this.Name}' '{component.GetType().Name}'");
            if (component is not Element element)
                return;

            if (!this.elements.TryAdd(component.ComponentIndex, element)) {
                throw new Exception($"this component is alrealy has in scope. '{this.GetType().Name}' '{component.GetType().Name}'");
            }

            MultiCombinMatch(this, component);
            CrossCombinMatch(this, component);
        }

        internal override void OnComponentRemoveInternal(Component component) {
            if (component is not Element)
                return;

            this.elements.Remove(component.ComponentIndex);

            MultiDecombinMatch(this, component);
            CrossDecombinMatch(this, component);
        }

        internal override void OnParentChangedInternal(Entity previousParent) {
            this.ParentScope = this.Parent?.GetComponentInParent<Scope>(true);
        }

        #region internal combin match

        internal static void MultiCombinMatch(Scope scope, Component added) {
            List<Combiner> combiners;
            var matchCount = int.MaxValue;
            if (added != null) {
                // 如果该组件没有组合, 则直接退出
                if (!Combiner.MultiCombinLookupTable.TryGetValue(added.ComponentIndex, out var combinInfo))
                    return;

                // 获取该组件的组合信息, 然后与total cacher做一次匹配, 可以得到一个理论上能匹配到的最大成员数
                matchCount = scope.Entity.componentTypeCacher.ContainsCount(combinInfo.totalTypeCacher);

                // 组合最少要两个才能形成组合, 如果和total的匹配都不足两个, 就没必要挨个尝试了
                if (matchCount < 2)
                    return;

                combiners = combinInfo.combiners;
            }
            else {
                matchCount = scope.Entity.componentTypeCacher.ContainsCount(Combiner.TotalMultiCombinerTypeCacher);
                if (matchCount < 2)
                    return;

                combiners = Combiner.MultiCombiners;
            }

            for (int i = 0, len = combiners.Count; i < len; i++) {
                var combiner = combiners[i];
                // 如果成员数已经超出了当前可能出现的最大数, 就可以直接跳出, 剩下的也不需要尝试了
                if (combiner.memberTypes.Length > matchCount)
                    break;

                if (scope.HasComponentsAll(combiner.multiTypeCacher)) {
                    if (scope.multiMatchs.Contains(combiner.id))
                        goto CONTINUE;

                    // 如果符合, 先判断该组合有没有覆盖者, 如果有, 则不能形成组合
                    if (Combiner.InverseOverrides.TryGetValue(combiner.id, out var ids)) {
                        foreach (var overrideCombinerId in ids) {
                            if (scope.multiMatchs.Contains(overrideCombinerId)) {
                                goto CONTINUE;
                            }
                        }
                    }

                    // 再判断该组合有没有需要覆盖的, 如果有, 就断开其组合
                    if (Combiner.Overrides.TryGetValue(combiner.id, out ids)) {
                        foreach (var overrideCombinerId in ids) {
                            var matchcount = scope.multiMatchs.Count;
                            while (matchcount-- > 0) {
                                var id = scope.multiMatchs.Dequeue();
                                if (id == overrideCombinerId) {
                                    var decombiner = Combiner.MultiCombiners[overrideCombinerId];
                                    _componentCache.Clear();
                                    scope.GetComponentsOfTypeCacher(decombiner.multiTypeCacher, _componentCache);
                                    decombiner.Decombin(_componentCache);
                                    continue;
                                }

                                scope.multiMatchs.Enqueue(id);
                            }

                            scope.parentCrossMatchQueue.Select((parentScope, crossCombiner) => {
                                if (crossCombiner.id == overrideCombinerId) {
                                    parentScope.childCrossMatchQueue.Dequeue(scope, crossCombiner);
                                    _componentCache.Clear();
                                    scope.GetComponentsOfTypeCacher(crossCombiner.crossChildTypeCacher, _componentCache);
                                    parentScope.GetComponentsOfTypeCacher(crossCombiner.crossParentTypeCacher, _componentCache);
                                    crossCombiner.Decombin(_componentCache);
                                    return false;
                                }

                                return true;
                            });
                        }
                    }

                    scope.multiMatchs.Enqueue(combiner.id);
                    _componentCache.Clear();
                    scope.GetComponentsOfTypeCacher(combiner.multiTypeCacher, _componentCache);
                    combiner.Combin(_componentCache);
                }

                CONTINUE: ;
            }
        }

        internal static void MultiDecombinMatch(Scope scope, Component removed) {
            // component可以为空, 如果为空, 则代表断开所有组合
            // 当有组件移除时, 说明有可能存在组合被断开了, 遍历已经所有已经组合的组合, 挨个匹配, 看看断掉的是哪个组合
            var componentIndex = removed?.ComponentIndex ?? -1;
            var combiners = Combiner.MultiCombiners;
            var count = scope.multiMatchs.Count;
            while (count-- > 0) {
                var combinerId = scope.multiMatchs.Dequeue();
                var combiner = combiners[combinerId];
                // 如果组合包含这个组件, 则删除该组件编号, 并触发事件
                if (componentIndex == -1 || combiner.multiTypeCacher.Contains(componentIndex)) {
                    _componentCache.Clear();
                    scope.GetComponentsOfTypeCacher(combiner.multiTypeCacher, _componentCache);
                    if (componentIndex != -1) _componentCache.Add(removed);
                    combiner.Decombin(_componentCache);
                    continue;
                }

                scope.multiMatchs.Enqueue(combinerId);
            }
        }

        internal void CalcMaximumCrossLayerInTheory() {
            if (CrossCombinFormatter.CrossCombinFormatterInfoCollects.TryGetValue(this.ComponentIndex, out var collect)) {
                if (collect.infiniteMatching) {
                    this.maximumFormatterCrossLayer = int.MaxValue;
                    return;
                }

                var parent = this.parentScope;
                var layer = 1;
                while (parent != null) {
                    var evaluate = false;
                    foreach (var formatterInfo in collect.formatterInfos) {
                        if (!formatterInfo.succ && layer != 1)
                            continue;

                        var index = layer - 1;
                        if (index < formatterInfo.layerInfos.Count) {
                            var layerInfo = formatterInfo.layerInfos[index];
                            if (layerInfo.parentTypeCacher == null) {
                                formatterInfo.succ = true;
                            }
                            else {
                                formatterInfo.succ = parent.HasComponentsAny(layerInfo.parentTypeCacher);
                            }
                        }
                        else {
                            formatterInfo.succ = false;
                        }

                        if (formatterInfo.succ) {
                            evaluate = formatterInfo.succ;
                        }
                    }

                    if (!evaluate) {
                        break;
                    }

                    parent = parent.parentScope;
                    layer++;
                }

                this.maximumFormatterCrossLayer = layer - 1;
                if (this.maximumFormatterCrossLayer < 1)
                    this.maximumFormatterCrossLayer = 1;
            }
            else {
                this.maximumFormatterCrossLayer = 1;
            }
        }

        // 跨域组合
        internal static void CrossCombinMatch(Scope scope, Component added) {
            if (Combiner.CrossCombinLookupTable.TryGetValue(added.ComponentIndex, out var crossCombinInfo)) {
                if (crossCombinInfo.childCombiners.Count != 0) {
                    if (scope.HasComponentsAny(crossCombinInfo.totalChildTypeCacher)) {
                        CrossCombinMatchForParent(scope, scope.parentScope, 1, crossCombinInfo.childCombiners);
                    }
                }

                if (crossCombinInfo.parentCombiners.Count != 0) {
                    foreach (var combiner in crossCombinInfo.parentCombiners) {
                        if (scope.HasComponentsAll(combiner.crossParentTypeCacher)) {
                            scope.ForeachChildrenScope((childScope, layer) => {
                                if (childScope.HasComponentsAny(crossCombinInfo.totalChildTypeCacher)) {
                                    CrossCombinMatchForParent(childScope, scope, layer, combiner);
                                }
                            });
                        }
                    }
                }
            }
        }

        // 尝试向父域及父域的父域跨域组合
        internal static void CrossCombinMatchForParent(Scope child, Scope parent, int layer, List<Combiner> crossCombiners = null) {
            // 方案1
            if (crossCombiners == null) {
                if (!child.HasComponentsAny(Combiner.TotalCrossCombinerChildTypeCacher))
                    return;

                crossCombiners = Combiner.CrossCombiners;
            }

            foreach (var combiner in crossCombiners) {
                CrossCombinMatchForParent(child, parent, layer, combiner);
            }

            // 方案2
            // 下面这种是第二种思路, 能更精确的针对性的做判断, 而不用把所有的cross combiners都遍历判断一遍
            // 但问题就是, 如果cross combiner的childCacher有两个或更多, 就会出现重复判断的情况, 且多几个就会重复判断几次. 虽然目前cross combiner基本上child都是单个的, 
            // 但始终是有局限性.
            // 而且, foreach Contains每次都要for循环64*n次, 循环的也挺多的, 并不一定就比上面那种快
            // if (crossCombiners == null) {
            //     foreach (var componentIndex in child.Entity.componentTypeCacher.Contains(Combiner.TotalCrossCombinerChildTypeCacher)) {
            //         foreach (var combiner in Combiner.CrossCombinLookupTable[componentIndex].childCombiners) {
            //             CrossCombinMatchForParent(child, parent, layer, combiner);
            //         }
            //     }
            // }
            // else {
            //     foreach (var combiner in crossCombiners) {
            //         CrossCombinMatchForParent(child, parent, layer, combiner);
            //     }
            // }
        }

        internal static void CrossCombinMatchForParent(Scope child, Scope parent, int layer, Combiner crossCombiner) {
            if (child.HasComponentsAll(crossCombiner.crossChildTypeCacher)) {
                while (parent != null) {
                    var evaluateOfCombin = layer <= crossCombiner.crossMaximumLayer;
                    var eveluateOfFormatter = layer <= child.maximumFormatterCrossLayer;

                    // cross 组合, 与cross formatter都不支持该层, 则跳出
                    if (!evaluateOfCombin && !eveluateOfFormatter) {
                        break;
                    }

                    // 先判断该父域能否形成组合, 因为这个判断速度最快, 所以先判断他
                    if (!parent.HasComponentsAll(crossCombiner.crossParentTypeCacher))
                        goto CONTINUE;

                    // 再判断该组合是不是已经组合过了
                    if (child.parentCrossMatchQueue.Contains(parent, crossCombiner))
                        goto CONTINUE;

                    // 再判断该组合有没有覆盖者, 如果有, 则不能形成组合
                    if (Combiner.InverseOverrides.TryGetValue(crossCombiner.id, out var ids)) {
                        foreach (var overrideCombinerId in ids) {
                            if (child.multiMatchs.Contains(overrideCombinerId)) {
                                goto CONTINUE;
                            }
                        }
                    }

                    // 到这里, 就说明可以形成组合了
                    child.parentCrossMatchQueue.Enqueue(parent, crossCombiner);
                    parent.childCrossMatchQueue.Enqueue(child, crossCombiner);
                    _componentCache.Clear();
                    child.GetComponentsOfTypeCacher(crossCombiner.crossChildTypeCacher, _componentCache);
                    parent.GetComponentsOfTypeCacher(crossCombiner.crossParentTypeCacher, _componentCache);
                    crossCombiner.Combin(_componentCache);

                    // 到这里说明组合已经成功了, 而如果不是formatter还要继续的话, 就可以跳出, 不再继续向上了
                    // 这里的逻辑之所以这么写, 是因为 cross formatter 和 单纯的cross 的规则是不同的
                    // 单纯的cross: 比如, 某个cross combiner规定了某个组合最高向上匹配 3 层, 但无论最高有多少层, 一旦匹配成功, 就不会在继续往上匹配了.
                    // cross formatter: 他是以scope作为参考的, 且不拘泥于某个具体的组合, 例如, 现在有一个formatter: scopeA、scopeB、scopeC,
                    //   当scopeA的父域是scopeB时, 他才会与其进行匹配(不局限于某个具体的cross combiner, 而是所有的cross combiner), 只有父域的父域是scopeC时, 才会与其进行匹配,
                    //   而且不会终止, 也就是说, 不会因为scopeA与scopeB匹配成功了, 就不再与scopeC匹配了.
                    if (!eveluateOfFormatter) {
                        break;
                    }

                    CONTINUE:
                    parent = parent.parentScope;
                    layer++;
                }
            }
        }

        // 断开跨域组合
        internal static void CrossDecombinMatch(Scope scope, Component removed) {
            var componentIndex = removed.ComponentIndex;
            scope.parentCrossMatchQueue.Select((parentScope, combiner) => {
                if (combiner.crossChildTypeCacher.Contains(componentIndex)) {
                    parentScope.childCrossMatchQueue.Dequeue(scope, combiner);
                    _componentCache.Clear();
                    scope.GetComponentsOfTypeCacher(combiner.crossChildTypeCacher, _componentCache);
                    parentScope.GetComponentsOfTypeCacher(combiner.crossParentTypeCacher, _componentCache);
                    _componentCache.Add(removed);
                    combiner.Decombin(_componentCache);
                    return false;
                }

                return true;
            });

            scope.childCrossMatchQueue.Select((childScope, combiner) => {
                if (combiner.crossParentTypeCacher.Contains(componentIndex)) {
                    childScope.parentCrossMatchQueue.Dequeue(scope, combiner);
                    _componentCache.Clear();
                    childScope.GetComponentsOfTypeCacher(combiner.crossChildTypeCacher, _componentCache);
                    scope.GetComponentsOfTypeCacher(combiner.crossParentTypeCacher, _componentCache);
                    _componentCache.Add(removed);
                    combiner.Decombin(_componentCache);
                    return false;
                }

                return true;
            });
        }

        // 尝试向父域及父域的父域断开跨域组合
        internal static void CrossDecombinMatchForParent(Scope scope, Scope parent) {
            scope.parentCrossMatchQueue.SelectMatchs(match => {
                var parentScope = match.scope;
                if (parentScope == parent || parentScope.Entity.IsParentOf(parent.Entity)) {
                    foreach (var combiner in match.combiners) {
                        parentScope.childCrossMatchQueue.Dequeue(scope, combiner);
                        _componentCache.Clear();
                        parentScope.GetComponentsOfTypeCacher(combiner.crossParentTypeCacher, _componentCache);
                        scope.GetComponentsOfTypeCacher(combiner.crossChildTypeCacher, _componentCache);
                        combiner.Decombin(_componentCache);
                    }

                    return false;
                }

                return true;
            });
        }

        #endregion

        #region function

        /// 从所有父域中寻找指定域
        public T FindScopeInParent<T>() where T : class {
            var curr = this.parentScope;
            while (curr != null) {
                if (curr is T t)
                    return t;

                curr = curr.parentScope;
            }

            return default;
        }
        
        /// 从所有子域中寻找指定域
        public T FindScopeInChildren<T>() where T : class {
            T GetByChildren(List<Scope> children) {
                if (children != null) {
                    for (int i = 0, len = children.Count; i < len; i++) {
                        if (children[i] is T t)
                            return t;
                    }

                    for (int i = 0, len = children.Count; i < len; i++) {
                        var t = GetByChildren(children[i].childrenScopes);
                        if (t != null)
                            return t;
                    }
                }

                return null;
            }

            var t = GetByChildren(this.childrenScopes);
            return t;
        }

        protected void ForeachChildrenScope(Action<Scope, int> callback, int maxLayer = int.MaxValue) {
            void Foreach(Action<Scope, int> _callback, Scope scope, int layer) {
                if (layer > maxLayer)
                    return;

                if (scope.childrenScopes == null)
                    return;

                for (int i = 0, len = scope.childrenScopes.Count; i < len; i++) {
                    _callback.Invoke(scope.childrenScopes[i], layer);
                }

                ++layer;
                for (int i = 0, len = scope.childrenScopes.Count; i < len; i++) {
                    Foreach(_callback, scope.childrenScopes[i], layer);
                }
            }

            Foreach(callback, this, 1);
        }

        internal void SearchChildScopes() {
            void Search(Entity e) {
                if (e.children == null)
                    return;

                for (int i = 0, len = e.children.Count; i < len; i++) {
                    var child = e.children[i];
                    var childScope = child.GetComponent<Scope>();
                    if (childScope != null) {
                        childScope.ParentScope = this;
                        continue;
                    }

                    Search(child);
                }
            }

            Search(this.Entity);
        }

        #endregion

        #region protected events

        internal virtual void OnBeforeParentScopeChangeInternal(Scope previous) {
            try {
                this.OnBeforeParentScopeChanged(previous);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal virtual void OnParentScopeChangedInternal(Scope previous) {
            try {
                this.OnParentScopeChanged(previous);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal virtual void OnChildScopeAddInternal(Scope child) {
            try {
                this.OnChildScopeAdd(child);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal virtual void OnChildScopeRemoveInternal(Scope child) {
            try {
                this.OnChildScopeRemove(child);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnBeforeParentScopeChanged(Scope previous) { }

        protected virtual void OnParentScopeChanged(Scope previous) { }

        protected virtual void OnChildScopeAdd(Scope child) { }

        protected virtual void OnChildScopeRemove(Scope child) { }

        #endregion

        internal class CrossMatchQueue {
            private Queue<CrossMatch> _matches;

            public bool Contains(Scope scope, Combiner combiner) {
                if (this._matches == null)
                    return false;

                foreach (var match in this._matches) {
                    if (match.scope == scope && match.combiners.Contains(combiner)) {
                        return true;
                    }
                }

                return false;
            }

            public bool Contains(Scope scope) {
                if (this._matches == null)
                    return false;

                foreach (var match in this._matches) {
                    if (match.scope == scope) {
                        return true;
                    }
                }

                return false;
            }

            public void Enqueue(Scope scope, Combiner combiner) {
                if (this._matches == null) {
                    this._matches = new();
                }
                else {
                    foreach (var match in this._matches) {
                        if (match.scope == scope) {
                            match.combiners.Enqueue(combiner);
                            return;
                        }
                    }
                }

                CrossMatch newMatch = new() {
                    scope = scope,
                    combiners = new()
                };
                newMatch.combiners.Enqueue(combiner);
                this._matches.Enqueue(newMatch);
            }

            public void Dequeue(Scope scope, Combiner combiner) {
                if (this._matches == null)
                    return;

                var matchCount = this._matches.Count;
                while (matchCount-- > 0) {
                    var match = this._matches.Dequeue();
                    if (match.scope == scope) {
                        var combinerCount = match.combiners.Count;
                        while (combinerCount-- > 0) {
                            var tempCombiner = match.combiners.Dequeue();
                            if (tempCombiner == combiner) {
                                if (match.combiners.Count == 0) {
                                    match.combiners = null;
                                }
                                else {
                                    this._matches.Enqueue(match);
                                }

                                return;
                            }

                            match.combiners.Enqueue(tempCombiner);
                        }
                    }

                    this._matches.Enqueue(match);
                }

                if (this._matches.Count == 0)
                    this._matches = null;
            }

            public void Select(Func<Scope, Combiner, bool> func) {
                if (this._matches == null)
                    return;

                var matchCount = this._matches.Count;
                while (matchCount-- > 0) {
                    var match = this._matches.Dequeue();
                    var combinerCount = match.combiners.Count;
                    while (combinerCount-- > 0) {
                        var tempCombiner = match.combiners.Dequeue();
                        var ret = func.Invoke(match.scope, tempCombiner);
                        if (!ret) {
                            continue;
                        }

                        match.combiners.Enqueue(tempCombiner);
                    }

                    if (match.combiners.Count != 0)
                        this._matches.Enqueue(match);
                }

                if (this._matches.Count == 0)
                    this._matches = null;
            }

            public void SelectMatchs(Func<CrossMatch, bool> func) {
                if (this._matches == null)
                    return;

                var matchCount = this._matches.Count;
                while (matchCount-- > 0) {
                    var match = this._matches.Dequeue();
                    var ret = func.Invoke(match);
                    if (!ret)
                        continue;

                    this._matches.Enqueue(match);
                }

                if (this._matches.Count == 0)
                    this._matches = null;
            }

            public void Foreach(Action<Scope, Combiner> action) {
                if (this._matches == null)
                    return;

                var matchCount = this._matches.Count;
                while (matchCount-- > 0) {
                    var match = this._matches.Dequeue();
                    var combinerCount = match.combiners.Count;
                    while (combinerCount-- > 0) {
                        var tempCombiner = match.combiners.Dequeue();
                        action.Invoke(match.scope, tempCombiner);

                        match.combiners.Enqueue(tempCombiner);
                    }

                    this._matches.Enqueue(match);
                }
            }

            public void Clear() {
                this._matches?.Clear();
                this._matches = null;
            }
        }

        internal class CrossMatch {
            public Scope scope;
            public Queue<Combiner> combiners;
        }
    }
}