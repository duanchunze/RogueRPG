using System;
using System.Collections.Generic;
using MemoryPack;

// ReSharper disable MemberCanBePrivate.Global

namespace Hsenl {
    public enum CombinMatchMode {
        Auto, // 纯自动, 自动进行multi和cross匹配
        Manual, // 纯手动, 不进行匹配, 需要自己手动匹配
    }

    // 一个域, 一个具体的逻辑范围, 框架内部没有实际组件去继承他, 他是由用户来继承使用
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Scope : Component {
        private static readonly List<Component> _componentCache = new();

        // 父域与子域之间也可以看做是一种<御统和被御统的关系>
        [MemoryPackIgnore]
        internal Scope parentScope;

        [MemoryPackIgnore]
        internal List<Scope> childrenScopes;

        [MemoryPackIgnore]
        internal Queue<int> multiMatchs;

        [MemoryPackIgnore]
        internal CrossMatchQueue crossMatchQueue1;

        [MemoryPackIgnore]
        internal CrossMatchQueue crossMatchQueue2;

        // 每当父级发生改变时(包括父级以上发生改变时), 会算出一个在当前父子链下, 最大跨域层数, 默认为1
        // 指的注意的是, CombinerOptionsAttribute中有个crossMaximumLayer参数, 虽然看起来类似, 但二者本质并不同, 那个参数是用来决定某个具体的组合的最大匹配层数,
        // 而这个参数是决定某个scope像父域做整体匹配时的最大层数. 二者是相互独立, 互不影响的.
        [MemoryPackIgnore]
        internal int maximumFormatterCrossLayer = 1;

        [MemoryPackOrder(2)]
        [MemoryPackInclude]
        internal CombinMatchMode combinMatchMode;

        /// <summary>
        /// <para>组合匹配模式.</para>
        /// <para>自动模式: 全由系统自动匹配.</para>
        /// <para>手动模式: 不自动进行任何跨域组合行为. 全由用户决定.(不推荐, 麻烦且容易出错, 用半自动就好了) 该模式下, 需要在添加删除组件时、销毁时以及父子关系改时, 手动调用函数</para>
        /// <para>模式只会影响自己, 而不会影响自己的子域, 比如自己为手动模式, 而自己的子域为自动模式, 那么自己的子域依然会正常的自动匹配, 不受影响</para>
        /// ps: 跨域组合变化可能发生在以下时刻: 被创建或被反序列化时, 添加删除组件时, 被销毁时, 父子关系发生改变时.
        /// </summary>
        [MemoryPackIgnore]
        public CombinMatchMode CombinMatchMode {
            get => this.combinMatchMode;
            set {
                if (this.combinMatchMode == value)
                    return;

                var v = this.combinMatchMode;
                this.combinMatchMode = value;

                if (v == CombinMatchMode.Auto) {
                    this.DecombinAll();
                }
                else {
                    this.CombinAll();
                }
            }
        }

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
                    if (this.CombinMatchMode == CombinMatchMode.Auto) {
                        CrossDecombinMatchForParents(this, prevParent);
                    }

                    this.ForeachChildrenScope((childScope, _, p) => {
                        if (childScope.CombinMatchMode == CombinMatchMode.Auto) {
                            CrossDecombinMatchForParents(childScope, p); //
                        }
                    }, data: prevParent);
                }

                // 确立好父子关系后再进行跨域匹配, 保证形成组合的时候, 父子关系是正确的.
                if (value != null) {
                    if (this.CombinMatchMode == CombinMatchMode.Auto) {
                        this.CalcMaximumCrossLayerInTheory();
                        CrossCombinsMatchForParents(this, value, 1, null);
                    }

                    this.ForeachChildrenScope((childScope, layer, p) => {
                        if (childScope.CombinMatchMode == CombinMatchMode.Auto) {
                            childScope.CalcMaximumCrossLayerInTheory();
                            CrossCombinsMatchForParents(childScope, p, layer + 1, null); // 
                        }
                    }, data: value);
                }

                // 先匹配再触发事件, 可以保证当用户使用该事件时, 所有该做的组合都已经完成了, 保证了组合的优先性(注: 组合的优先级只比添加组件时的initializeInvoke和序列化的OnDeserialized低)
                this.OnParentScopeChangedInternal(prevParent);
                prevParent?.OnChildScopeRemoveInternal(this);
                value?.OnChildScopeAddInternal(this);
            }
        }

        internal override void OnAwakeInternal() {
            // 创建的第一时间进行multi匹配, 因为这种组合不依赖父子关系
            if (this.CombinMatchMode != CombinMatchMode.Manual) {
                MultiCombinMatch(this, null);
            }

            // 域被创建有两种种情况, 一种是通过new创建, 第二种是通过instantiate实例化
            // 第一种情况: 由于通过new创建一个域的时候, 我们不能确定该域具体是什么状况, 比如只有父域, 或者只有子域, 或者有子域也有父域, 自己属于是插入者的情况, 所以这种情况, 我们要把父域和
            //      子域通通设置一番
            // 第二种情况: 由于实例化一个实体事件回调是自上而下的, 所以, 每个域都只需向上做匹配组合就行了, 最后自然都做一次匹配了
            if (!this.IsDeserialized) {
                this.SearchChildScopes();
            }

            // 确立上下的父子关系, 并进行匹配及事件触发
            this.ParentScope = this.Parent?.GetComponentInParent<Scope>(true, true);
        }

        internal override void OnDestroyInternal() {
            // 域被销毁的时候, 只断开组合就行了, 其他的都不用做
            if (this.CombinMatchMode != CombinMatchMode.Manual) {
                MultiDecombinMatch(this, null);
            }

            // 牵扯到跨域的销毁有两种情况, 一种是整个域被销毁(连同该域及其所有子域), 第二种是单单该域被销毁(仅仅该域Scope组件被Destroy)
            // 第一种情况: 由于销毁的顺序是自下而上的, 所以, 每个子域在被销毁时, 都只需要向父域断开组合就行了, 最后所有的跨域组合自然都断开了
            // 第二种情况: 由于单单自己被销毁了, 所以相当于自己从原来的父子域链中被抽出了, 所以自己可能是存在子域的, 这时, 不仅自己要和所有子域断开组合, 还需要把原本自己的子域交给自己的父域
            // 可以根据自己实体的 imminentDispose 状态来判断是哪种情况, 如果自己的实体没有被即将销毁, 那就说明只有自己被销毁了
            if (!this.Entity.disposing) {
                // 把本来自己的子域, 移交给自己的父域, 让他们重新做组合
                if (this.childrenScopes != null) {
                    for (int i = this.childrenScopes.Count - 1; i >= 0; i--) {
                        var childScope = this.childrenScopes[i];
                        childScope.ParentScope = this.ParentScope;
                    }
                }
            }

            if (this.ParentScope != null) {
                if (this.CombinMatchMode == CombinMatchMode.Auto) {
                    CrossDecombinMatchForParents(this, this.ParentScope);
                }
            }
        }

        internal override void OnDisposedInternal() {
            this.parentScope = null;
            this.childrenScopes?.Clear();
            this.childrenScopes = null;
            this.multiMatchs?.Clear();
            this.multiMatchs = null;
            this.crossMatchQueue2?.Clear();
            this.crossMatchQueue2 = null;
            this.crossMatchQueue1?.Clear();
            this.crossMatchQueue1 = null;
            this.maximumFormatterCrossLayer = 1;
            this.combinMatchMode = default;
        }

        internal override void OnComponentAddInternal(Component component) {
            if (component is Scope)
                throw new Exception($"one entity only one scope. '{this.Name}' '{component.GetType().Name}'");

            if (component is not Element element)
                return;

            if (this.CombinMatchMode != CombinMatchMode.Manual) {
                MultiCombinMatch(this, element);
            }

            if (Combiner.CombinerCache.CrossCombinLookupTable.TryGetValue(element.ComponentIndex, out var combinInfo)) {
                if (combinInfo.combiners1.Count != 0) {
                    if (this.CombinMatchMode == CombinMatchMode.Auto) {
                        if (this.ParentScope != null) {
                            if (this.HasComponentsAny(combinInfo.totalTypeCacher1)) {
                                foreach (var combiner in combinInfo.combiners1) {
                                    CrossCombinMatchForParent(this, this.ParentScope, 1, combiner);
                                }
                            }
                        }
                    }
                }

                if (combinInfo.combiners2.Count != 0) {
                    if (this.HasComponentsAny(combinInfo.totalTypeCacher2)) {
                        this.ForeachChildrenScope<(CrossCombinInfo ci, Scope p)>((child, layer, data) => {
                            if (child.CombinMatchMode == CombinMatchMode.Auto) {
                                foreach (var combiner in data.ci.combiners2) {
                                    CrossCombinMatchForParent(child, data.p, layer, combiner);
                                }
                            }
                        }, data: (combinInfo, this));
                    }
                }
            }
        }

        internal override void OnComponentRemoveInternal(Component component) {
            if (component is not Element element)
                return;

            if (this.CombinMatchMode != CombinMatchMode.Manual) {
                MultiDecombinMatch(this, element);
            }

            if (this.CombinMatchMode == CombinMatchMode.Auto) {
                CrossDecombinMatchByComponent(this, element);
            }
        }

        internal override void OnParentChangedInternal(Entity previousParent) {
            this.ParentScope = this.Parent?.GetComponentInParent<Scope>(true, true);
        }

        #region core functions

        // multi组合的组合和解组, 相对简单一些, 就是当当前scope发生组件变化的时候, 去检测一次.
        internal static void MultiCombinMatch(Scope scope, Element added) {
            List<Combiner> combiners;
            int matchCount;
            if (added != null) {
                // 如果该组件连一个组合都没有, 则直接退出, 没必要继续尝试形成新的组合了
                if (!Combiner.CombinerCache.MultiCombinLookupTable.TryGetValue(added.ComponentIndex, out var combinInfo))
                    return;

                // 获取该组件的组合信息, 然后与total cacher做一次匹配, 可以得到一个理论上能匹配到的最大成员数
                // 因为一个组件可能形成2个、3个、4个组合, 先提前判断当前实体有可能形成的最大成员的组合, 这样在遇到高于该成员数的组合时, 就可以直接跳过, 没必要尝试了
                matchCount = scope.Entity.componentTypeCacher.ContainsCount(combinInfo.totalTypeCacher);

                // 组合最少要两个才能形成组合, 如果和total的匹配都不足两个, 那也没必要挨个尝试了
                if (matchCount < 2)
                    return;

                // 获取候选组合列表
                combiners = combinInfo.combiners;
            }
            else {
                // 如果added为空, 则表示尝试所有multi组合
                // 先与total cacher判断一次, 可能会避免不必要的循环, 比如该scope并没有形成组合的条件, 但依然要遍历所有combiner去试, 所以先与total cacher判存一次, 
                // 如果匹配的数还不到2个, 那就说明该scope能匹配的最大数都不足2个, 自然也就不用遍历所有combiner去试了.
                matchCount = scope.Entity.componentTypeCacher.ContainsCount(Combiner.CombinerCache.TotalMultiCombinerTypeCacher);
                if (matchCount < 2)
                    return;

                combiners = Combiner.CombinerCache.MultiCombiners;
            }

            for (int i = 0, len = combiners.Count; i < len; i++) {
                var combiner = combiners[i];
                // 如果成员数已经超出了当前可能出现的最大数, 就可以直接跳出, 剩下的也不需要尝试了
                if (combiner.memberTypes.Length > matchCount)
                    break;

                // 判断能否形成组合
                if (scope.HasComponentsAll(combiner.multiTypeCacher)) {
                    if (scope.multiMatchs != null) {
                        if (scope.multiMatchs.Contains(combiner.id))
                            goto CONTINUE;

                        // 如果符合, 先判断该组合有没有覆盖者, 如果有, 则不能形成组合
                        if (Combiner.CombinerCache.InverseOverrides.TryGetValue(combiner.id, out var ids)) {
                            foreach (var overrideCombinerId in ids) {
                                if (scope.multiMatchs.Contains(overrideCombinerId)) {
                                    goto CONTINUE;
                                }
                            }
                        }

                        // 再判断该组合有没有需要覆盖的, 如果有, 就断开其组合
                        if (Combiner.CombinerCache.Overrides.TryGetValue(combiner.id, out ids)) {
                            foreach (var overrideCombinerId in ids) {
                                if (scope.multiMatchs == null) break;

                                // 断开被覆盖的multi组合
                                var matchcount = scope.multiMatchs.Count;
                                while (matchcount-- > 0) {
                                    var id = scope.multiMatchs.Dequeue();
                                    if (id == overrideCombinerId) {
                                        var decombiner = Combiner.CombinerCache.TotalCombiners[overrideCombinerId];
                                        _componentCache.Clear();
                                        scope.GetComponentsOfTypeCacher(decombiner.multiTypeCacher, _componentCache);
                                        decombiner.Decombin(_componentCache);
                                        continue;
                                    }

                                    scope.multiMatchs.Enqueue(id);
                                }

                                if (scope.multiMatchs.Count == 0)
                                    scope.multiMatchs = null;

                                // 断开被覆盖的cross组合
                                scope.crossMatchQueue2?.Select<(List<Component> cache, Scope scope, int overrideId)>(
                                    (parentScope, crossCombiner, data) => {
                                        if (crossCombiner.id == data.overrideId) {
                                            parentScope.crossMatchQueue1.Dequeue(data.scope, crossCombiner);
                                            if (parentScope.crossMatchQueue1.Count == 0)
                                                parentScope.crossMatchQueue1 = null;
                                            data.cache.Clear();
                                            data.scope.GetComponentsOfTypeCacher(crossCombiner.crossTypeCacher1, data.cache);
                                            parentScope.GetComponentsOfTypeCacher(crossCombiner.crossTypeCacher2, data.cache);
                                            crossCombiner.Decombin(data.cache);
                                            return false; // 返回false, 代表该combiner在代码域执行完后, 该combiner不会再重新被添加到queue中(也就是删除了)
                                        }

                                        return true;
                                    }, (_componentCache, scope, overrideCombinerId));
                            }
                        }
                    }

                    // 正式形成新的multi组合
                    scope.multiMatchs ??= new();
                    scope.multiMatchs.Enqueue(combiner.id);
                    _componentCache.Clear();
                    scope.GetComponentsOfTypeCacher(combiner.multiTypeCacher, _componentCache);
                    combiner.Combin(_componentCache);
                }

                CONTINUE: ;
            }
        }

        internal static void MultiDecombinMatch(Scope scope, Element removed) {
            if (scope.multiMatchs == null) return;
            // component可以为空, 如果为空, 则代表断开所有组合
            // 当有组件移除时, 说明有可能存在组合被断开了, 遍历已经所有已经组合的组合, 挨个匹配, 看看断掉的是哪个组合
            var componentIndex = -1;
            if (removed != null) {
                componentIndex = removed.ComponentIndex;
            }

            var combiners = Combiner.CombinerCache.TotalCombiners;
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

            if (scope.multiMatchs.Count == 0)
                scope.multiMatchs = null;
        }

        // 每当scope的父子关系发生变化时, 都重新计算这整条父子链上每个节点上的最大formatter layer, 该数据后续的跨域组合中需要用到. 该值在每次发生父级变化时只需要计算一次即可.
        internal void CalcMaximumCrossLayerInTheory() {
            if (ScopeCombinFormatter.CrossCombinFormatterInfoCollects.TryGetValue(this.ComponentIndex, out var collect)) {
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
                            if (layerInfo.typeCacher == null) {
                                formatterInfo.succ = true;
                            }
                            else {
                                formatterInfo.succ = parent.HasComponentsAny(layerInfo.typeCacher);
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

        // 尝试向父域们做跨域组合
        internal static void CrossCombinsMatchForParents(Scope self, Scope parent, int layer, List<Combiner> combiners) {
            if (combiners == null) {
                if (!self.HasComponentsAny(Combiner.CombinerCache.TotalCrossCombiner1TypeCacher))
                    return;

                combiners = Combiner.CombinerCache.CrossCombiners;
            }

            foreach (var combiner in combiners) {
                CrossCombinMatchForParents(self, parent, layer, combiner);
            }
        }

        // 尝试向父域们做跨域组合
        internal static void CrossCombinMatchForParents(Scope self, Scope parent, int layer, Combiner crossCombiner) {
            if (!self.HasComponentsAll(crossCombiner.crossTypeCacher1))
                return;

            while (parent != null) {
                if (!CrossCombinMatchForParent(self, parent, layer, crossCombiner))
                    break;

                parent = parent.parentScope;
                layer++;
            }
        }

        /// 向父级做一次跨域匹配, 会受到来自layer的规则的约束
        /// <returns>是否还能继续匹配下一层</returns>
        internal static bool CrossCombinMatchForParent(Scope self, Scope parent, int layer, Combiner crossCombiner) {
            var evaluateOfCombin = layer <= crossCombiner.crossMaximumLayer;
            var eveluateOfFormatter = layer <= self.maximumFormatterCrossLayer;

            // cross 组合, 与cross formatter都不支持该层, 则跳出
            if (!evaluateOfCombin && !eveluateOfFormatter) {
                return false;
            }

            if (CrossCombinMatch(self, parent, crossCombiner) != 3) {
                goto FLAG;
            }

            // 到这里说明组合已经成功了, 而如果不是formatter还要继续的话, 就可以跳出, 不再继续向上了
            // 这里的逻辑之所以这么写, 是因为 cross formatter 和 单纯的cross 的规则是不同的
            // 单纯的cross: 比如, 某个cross combiner规定了某个组合最高向上匹配 3 层, 但无论最高有多少层, 一旦匹配成功, 就不会在继续往上匹配了.
            // cross formatter: 他是以scope作为参考的, 且不拘泥于某个具体的组合, 例如, 现在有一个formatter: scopeA、scopeB、scopeC,
            //   当scopeA的父域是scopeB时, 他才会与其进行匹配(不局限于某个具体的cross combiner, 而是所有的cross combiner), 只有父域的父域是scopeC时, 才会与其进行匹配,
            //   而且不会终止, 也就是说, 不会因为scopeA与scopeB匹配成功了, 就不再与scopeC匹配了.
            if (!eveluateOfFormatter) {
                return false;
            }

            FLAG:
            return true;
        }

        /// 让两个域做一次跨域匹配, 不受layer的约束
        /// <returns>0: 组件不满足组合条件 1: 该组合已经组合过了 2: 该组合已经被覆盖了 3: 组合成功</returns>
        internal static int CrossCombinMatch(Scope scope1, Scope scope2, Combiner crossCombiner) {
            // 先判断所含组件是否满足组合条件, 因为这个判断速度最快, 所以先判断他
            if (!scope1.HasComponentsAll(crossCombiner.crossTypeCacher1))
                return 0;

            if (!scope2.HasComponentsAll(crossCombiner.crossTypeCacher2))
                return 0;

            // 再判断该组合是不是已经组合过了
            if (scope1.crossMatchQueue2 != null)
                if (scope1.crossMatchQueue2.Contains(scope2, crossCombiner))
                    return 1;

            // 再判断该组合有没有覆盖者, 如果有, 则不能形成组合
            if (scope1.multiMatchs != null)
                if (Combiner.CombinerCache.InverseOverrides.TryGetValue(crossCombiner.id, out var ids)) {
                    foreach (var overrideCombinerId in ids) {
                        if (scope1.multiMatchs.Contains(overrideCombinerId)) {
                            return 2;
                        }
                    }
                }

            // 到这里, 就说明可以形成组合了
            scope1.crossMatchQueue2 ??= new CrossMatchQueue();
            scope2.crossMatchQueue1 ??= new CrossMatchQueue();
            scope1.crossMatchQueue2.Enqueue(scope2, crossCombiner);
            scope2.crossMatchQueue1.Enqueue(scope1, crossCombiner);
            _componentCache.Clear();
            scope1.GetComponentsOfTypeCacher(crossCombiner.crossTypeCacher1, _componentCache);
            scope2.GetComponentsOfTypeCacher(crossCombiner.crossTypeCacher2, _componentCache);
            crossCombiner.Combin(_componentCache);

            return 3;
        }

        // 断开跨域组合
        internal static void CrossDecombinMatchByComponent(Scope scope, Element removed) {
            if (removed == null)
                throw new ArgumentNullException(nameof(removed));

            var componentIndex = removed.ComponentIndex;
            scope.crossMatchQueue2?.Select<(Scope scope, Element removed, int index, List<Component> cache)>((parentScope, combiner, data) => {
                if (combiner.crossTypeCacher1.Contains(data.index)) {
                    parentScope.crossMatchQueue1.Dequeue(data.scope, combiner);
                    if (parentScope.crossMatchQueue1.Count == 0)
                        parentScope.crossMatchQueue1 = null;
                    data.cache.Clear();
                    data.scope.GetComponentsOfTypeCacher(combiner.crossTypeCacher1, data.cache);
                    parentScope.GetComponentsOfTypeCacher(combiner.crossTypeCacher2, data.cache);
                    data.cache.Add(data.removed);
                    combiner.Decombin(data.cache);
                    return false;
                }

                return true;
            }, (scope, removed, componentIndex, _componentCache));

            scope.crossMatchQueue1.Select<(Scope scope, Element removed, int index, List<Component> cache)>((childScope, combiner, data) => {
                if (combiner.crossTypeCacher2.Contains(data.index)) {
                    childScope.crossMatchQueue2.Dequeue(data.scope, combiner);
                    if (childScope.crossMatchQueue2.Count == 0)
                        childScope.crossMatchQueue2 = null;
                    data.cache.Clear();
                    childScope.GetComponentsOfTypeCacher(combiner.crossTypeCacher1, data.cache);
                    data.scope.GetComponentsOfTypeCacher(combiner.crossTypeCacher2, data.cache);
                    data.cache.Add(data.removed);
                    combiner.Decombin(data.cache);
                    return false;
                }

                return true;
            }, (scope, removed, componentIndex, _componentCache));
        }

        // 尝试向父域断开所有当前存在的跨域组合
        internal static void CrossDecombinMatchForParents(Scope child, Scope parent) {
            // 遍历child所有的父级跨域组合, 然后判断每个组合中的parent scope是否是目标parent, 或者是目标parent的父级, 这两种情况都说明该组合符合断开条件.
            child.crossMatchQueue2?.SelectMatchs<(Scope child, Scope parent, List<Component> cache)>((match, data) => {
                var parentScope = match.scope;
                if (parentScope.Entity.IsParentOf(data.parent.Entity)) {
                    foreach (var combiner in match.combiners) {
                        if (parentScope.crossMatchQueue1 == null) break;
                        parentScope.crossMatchQueue1.Dequeue(data.child, combiner);
                        if (parentScope.crossMatchQueue1.Count == 0)
                            parentScope.crossMatchQueue1 = null;
                        data.cache.Clear();
                        parentScope.GetComponentsOfTypeCacher(combiner.crossTypeCacher2, data.cache);
                        data.child.GetComponentsOfTypeCacher(combiner.crossTypeCacher1, data.cache);
                        combiner.Decombin(data.cache);
                    }

                    return false;
                }

                return true;
            }, (child, parent, _componentCache));
        }

        // 将两个scope断开
        internal static void CrossDecombinMatch(Scope scope1, Scope scope2) {
            // 遍历child所有的父级跨域组合, 然后判断每个组合中的parent scope是否是目标parent, 或者是目标parent的父级, 这两种情况都说明该组合符合断开条件.
            scope1.crossMatchQueue2?.SelectMatchs<(Scope scope1, Scope scope2, List<Component> cache)>((match, data) => {
                if (match.scope == data.scope2) {
                    var s1 = data.scope1;
                    var s2 = data.scope2;
                    foreach (var combiner in match.combiners) {
                        if (s2.crossMatchQueue1 == null) break;
                        s2.crossMatchQueue1.Dequeue(s1, combiner);
                        if (s2.crossMatchQueue1.Count == 0)
                            s2.crossMatchQueue1 = null;
                        data.cache.Clear();
                        s2.GetComponentsOfTypeCacher(combiner.crossTypeCacher2, data.cache);
                        s1.GetComponentsOfTypeCacher(combiner.crossTypeCacher1, data.cache);
                        combiner.Decombin(data.cache);
                    }

                    return false;
                }

                return true;
            }, (scope1, scope2, _componentCache));
        }

        #endregion

        #region interface functions

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

        internal void ForeachChildrenScope<T>(Action<Scope, int, T> callback, int maxLayer = int.MaxValue, T data = default) {
            Foreach(callback, this, 1, data);

            return;

            void Foreach(Action<Scope, int, T> _callback, Scope scope, int layer, T __data) {
                if (layer > maxLayer)
                    return;

                if (scope.childrenScopes == null)
                    return;

                for (int i = 0, len = scope.childrenScopes.Count; i < len; i++) {
                    _callback.Invoke(scope.childrenScopes[i], layer, __data);
                }

                ++layer;
                for (int i = 0, len = scope.childrenScopes.Count; i < len; i++) {
                    Foreach(_callback, scope.childrenScopes[i], layer, __data);
                }
            }
        }

        internal void SearchChildScopes() {
            void Search(Entity e) {
                if (e.children == null)
                    return;

                for (int i = 0, len = e.children.Count; i < len; i++) {
                    var child = e.children[i];
                    var childScope = child.GetComponent<Scope>(true);
                    if (childScope != null) {
                        childScope.ParentScope = this;
                        continue;
                    }

                    Search(child);
                }
            }

            Search(this.Entity);
        }

        /// 手动进行一次multi组合
        /// <param name="added">为空则代表对所有组件进行一次匹配, 否则只针对指定组件进行一次匹配</param>
        protected void MultiCombin(Element added) {
            MultiCombinMatch(this, added);
        }

        /// 手动进行一次multi断开组合
        /// /// <param name="removed">同added</param>
        protected void MultiDecombin(Element removed) {
            MultiDecombinMatch(this, removed);
        }

        /// 手动进行一次跨域匹配
        public void CrossCombinForOther(Scope other, Element added) {
            if (other == null)
                return;

            List<Combiner> combiners;
            if (added != null) {
                if (!Combiner.CombinerCache.CrossCombinLookupTable.TryGetValue(added.ComponentIndex, out var combinInfo))
                    return;

                // 判断是否有该组件作为child的跨域组合
                if (combinInfo.combiners1.Count == 0)
                    return;

                if (!this.HasComponentsAny(combinInfo.totalTypeCacher1))
                    return;

                combiners = combinInfo.combiners1;
            }
            else {
                if (!this.HasComponentsAny(Combiner.CombinerCache.TotalCrossCombiner1TypeCacher))
                    return;

                combiners = Combiner.CombinerCache.CrossCombiners;
            }

            foreach (var combiner in combiners) {
                CrossCombinMatch(this, other, combiner);
            }
        }

        /// 手动进行一次跨域断开
        public void CrossDecombinByComponent(Element removed) {
            CrossDecombinMatchByComponent(this, removed);
        }

        /// 手动进行一次跨域断开
        public void CrossDecombin(Scope other) {
            if (other == null)
                return;

            CrossDecombinMatch(this, other);
        }

        /// 手动尝试断开指定父级且向上的所有跨域组合
        public void CrossDecombinForParents(Scope parent) {
            if (parent == null)
                return;

            CrossDecombinMatchForParents(this, parent);
        }

        /// 手动进行一次全部组合
        protected void CombinAll() {
            this.MultiCombin(null);
            if (this.ParentScope != null) {
                CrossCombinsMatchForParents(this, this.ParentScope, 1, null);
            }
        }

        /// 手动进行一次全部断开
        protected void DecombinAll() {
            this.MultiDecombin(null);
            if (this.ParentScope != null) {
                CrossDecombinMatchForParents(this, this.ParentScope);
            }
        }

        private int GetLayerNum(Scope parent) {
            var layer = 0;
            var p = this.parentScope;
            while (p != null) {
                layer++;

                if (p == parent) {
                    return layer;
                }

                p = p.parentScope;
            }

            return -1;
        }

        #endregion

        #region protected events

        internal virtual void OnBeforeParentScopeChangeInternal(Scope future) {
            try {
                this.OnBeforeParentScopeChanged(future);
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

        protected virtual void OnBeforeParentScopeChanged(Scope future) { }

        protected virtual void OnParentScopeChanged(Scope previous) { }

        protected virtual void OnChildScopeAdd(Scope child) { }

        protected virtual void OnChildScopeRemove(Scope child) { }

        #endregion

        internal class CrossMatchQueue {
            private Queue<CrossMatch> _matches;

            public int Count => this._matches?.Count ?? 0;

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

            // func的返回值为false时, 代表顺便移除该combiner
            public void Select<T>(Func<Scope, Combiner, T, bool> func, T data = default) {
                if (this._matches == null)
                    return;

                var matchCount = this._matches.Count;
                while (matchCount-- > 0) {
                    var match = this._matches.Dequeue();
                    var combinerCount = match.combiners.Count;
                    while (combinerCount-- > 0) {
                        var tempCombiner = match.combiners.Dequeue();
                        var ret = func.Invoke(match.scope, tempCombiner, data);
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

            // func的返回值为false时, 代表顺便移除该match
            public void SelectMatchs<T>(Func<CrossMatch, T, bool> func, T data = default) {
                if (this._matches == null)
                    return;

                var matchCount = this._matches.Count;
                while (matchCount-- > 0) {
                    var match = this._matches.Dequeue();
                    var ret = func.Invoke(match, data);
                    if (!ret)
                        continue;

                    this._matches.Enqueue(match);
                }

                if (this._matches.Count == 0)
                    this._matches = null;
            }

            public void Foreach<T>(Action<Scope, Combiner, T> action, T data = default) {
                if (this._matches == null)
                    return;

                var matchCount = this._matches.Count;
                while (matchCount-- > 0) {
                    var match = this._matches.Dequeue();
                    var combinerCount = match.combiners.Count;
                    while (combinerCount-- > 0) {
                        var tempCombiner = match.combiners.Dequeue();
                        action.Invoke(match.scope, tempCombiner, data);

                        match.combiners.Enqueue(tempCombiner);
                    }

                    this._matches.Enqueue(match);
                }
            }

            public void Clear() {
                if (this._matches == null)
                    return;

                var count = this._matches.Count;
                while (count-- > 0) {
                    var match = this._matches.Dequeue();
                    match.combiners?.Clear();
                    match.combiners = null;
                }

                this._matches = null;
            }
        }

        internal class CrossMatch {
            public Scope scope;
            public Queue<Combiner> combiners;
        }
    }
}