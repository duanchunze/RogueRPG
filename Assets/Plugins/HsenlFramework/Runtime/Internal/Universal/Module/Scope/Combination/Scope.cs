using System;
using System.Collections.Generic;
using MemoryPack;

// ReSharper disable MemberCanBePrivate.Global

namespace Hsenl {
    public enum CrossMatchMode {
        Auto, // 纯自动, 自动进行multi和cross匹配
        Semi_Automatic, // 半自动, 暂时和纯手动效果一样
        Manual, // 纯手动, 不自动进行cross匹配, 只进行multi匹配
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
        internal Dictionary<int, Element> elements = new(); // 单领域下, 组件不可以重复, 所以, 单领域下每种组合最多存在一个. 如果有需要多组件的情况, 可以使用子域来解决

        [MemoryPackIgnore]
        internal Queue<int> multiMatchs;

        [MemoryPackIgnore]
        internal CrossMatchQueue parentCrossMatchQueue;

        [MemoryPackIgnore]
        internal CrossMatchQueue childCrossMatchQueue;

        // 每当父级发生改变时(包括父级以上发生改变时), 会算出一个在当前父子链下, 最大跨域层数, 默认为1
        // 指的注意的是, CombinerOptionsAttribute中有个crossMaximumLayer参数, 虽然看起来类似, 但二者本质并不同, 那个参数是用来决定某个具体的组合的最大匹配层数,
        // 而这个参数是决定某个scope像父域做整体匹配时的最大层数. 二者是相互独立, 互不影响的.
        [MemoryPackIgnore]
        internal int maximumFormatterCrossLayer = 1;

        /// <summary>
        /// <para>跨域匹配模式.</para>
        /// <para>自动模式: 全由系统自动匹配.</para>
        /// <para>半自动模式: 屏蔽父子关系改变时进行的匹配组合行为, 其他的时候都不影响. 所以该模式需要用户在父域改变时, 自行做跨域匹配(重写OnParentScopeChanged函数,
        /// 在其中手动调用CrossDecombinMatch和CrossCombinMatch两个函数, 前者断开之前父级的组合, 后者匹配新父级的组合). 且由于该模式影响的是父子改变时的组合匹配, 所以要在父子改变前
        /// 修改该值(OnParentScopeChanged函数调用前), 可以在OnBeforeParentScopeChanged函数中修改.</para>
        /// <para>手动模式: 不自动进行任何跨域组合行为. 全由用户决定.(不推荐, 麻烦且容易出错, 用半自动就好了) 该模式下, 需要在添加删除组件时、销毁时以及父子关系改时, 手动调用函数</para>
        /// <para>模式只会影响自己, 而不会影响自己的子域, 比如自己为手动模式, 而自己的子域为自动模式, 那么自己的子域依然会正常的自动匹配, 不受影响</para>
        /// ps: 跨域组合变化可能发生在以下时刻: 被创建或被反序列化时, 添加删除组件时, 被销毁时, 父子关系发生改变时.
        /// </summary>
        [MemoryPackOrder(2)]
        [MemoryPackInclude]
        protected internal CrossMatchMode crossMatchMode;

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
                    if (this.crossMatchMode == CrossMatchMode.Auto) {
                        CrossDecombinMatchForParent(this, prevParent);
                    }

                    this.ForeachChildrenScope((childScope, _) => {
                        if (childScope.crossMatchMode == CrossMatchMode.Auto) {
                            CrossDecombinMatchForParent(childScope, prevParent); //
                        }
                    });
                }

                // 确立好父子关系后再进行跨域匹配, 保证形成组合的时候, 父子关系是正确的.
                if (value != null) {
                    this.CalcMaximumCrossLayerInTheory();

                    if (this.crossMatchMode == CrossMatchMode.Auto) {
                        CrossCombinMatchForParent(this, value, 1);
                    }

                    this.ForeachChildrenScope((childScope, layer) => {
                        childScope.CalcMaximumCrossLayerInTheory();
                        if (childScope.crossMatchMode == CrossMatchMode.Auto) {
                            CrossCombinMatchForParent(childScope, value, layer + 1); // 
                        }
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

                this.elements ??= new();
                this.elements.Add(component.ComponentIndex, element);
            });
        }

        internal override void OnAwakeInternal() {
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
                if (this.crossMatchMode != CrossMatchMode.Manual) {
                    CrossDecombinMatchForParent(this, this.parentScope);
                    this.ForeachChildrenScope((childScope, _) => {
                        CrossDecombinMatchForParent(childScope, this.parentScope); // 
                    });
                }
            }
        }

        protected internal override void OnDestroyFinish() {
            base.OnDestroyFinish();
            this.parentScope = null;
            this.childrenScopes?.Clear();
            this.childrenScopes = null;
            this.elements?.Clear();
            this.elements = null;
            this.multiMatchs?.Clear();
            this.multiMatchs = null;
            this.parentCrossMatchQueue?.Clear();
            this.parentCrossMatchQueue = null;
            this.childCrossMatchQueue?.Clear();
            this.childCrossMatchQueue = null;
            this.maximumFormatterCrossLayer = 1;
            this.crossMatchMode = default;
        }

        internal override void OnComponentAddInternal(Component component) {
            if (component is Scope) throw new Exception($"one entity only one scope. '{this.Name}' '{component.GetType().Name}'");
            if (component is not Element element)
                return;

            if (elements != null && !this.elements.TryAdd(component.ComponentIndex, element)) {
                throw new Exception($"this component is alrealy has in scope. '{this.GetType().Name}' '{component.GetType().Name}'");
            }

            MultiCombinMatch(this, component);
            if (this.crossMatchMode != CrossMatchMode.Manual) {
                CrossCombinMatchByComponent(this, component);
            }
        }

        internal override void OnComponentRemoveInternal(Component component) {
            if (component is not Element)
                return;

            if (this.elements != null) {
                this.elements.Remove(component.ComponentIndex);
                if (this.elements.Count == 0) this.elements = null;
            }

            MultiDecombinMatch(this, component);
            if (this.crossMatchMode != CrossMatchMode.Manual) {
                CrossDecombinMatchByComponent(this, component);
            }
        }

        internal override void OnParentChangedInternal(Entity previousParent) {
            this.ParentScope = this.Parent?.GetComponentInParent<Scope>(true);
        }

        #region internal combin match

        // multi组合的组合和解组, 相对简单一些, 就是当当前scope发生组件变化的时候, 去检测一次.
        internal static void MultiCombinMatch(Scope scope, Component added) {
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
                                scope.parentCrossMatchQueue?.Select((parentScope, crossCombiner) => {
                                    if (crossCombiner.id == overrideCombinerId) {
                                        parentScope.childCrossMatchQueue.Dequeue(scope, crossCombiner);
                                        if (parentScope.childCrossMatchQueue.Count == 0) parentScope.childCrossMatchQueue = null;
                                        _componentCache.Clear();
                                        scope.GetComponentsOfTypeCacher(crossCombiner.crossChildTypeCacher, _componentCache);
                                        parentScope.GetComponentsOfTypeCacher(crossCombiner.crossParentTypeCacher, _componentCache);
                                        crossCombiner.Decombin(_componentCache);
                                        return false; // 返回false, 代表该combiner在代码域执行完后, 该combiner不会再重新被添加到queue中(也就是删除了)
                                    }

                                    return true;
                                });
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

        internal static void MultiDecombinMatch(Scope scope, Component removed) {
            if (scope.multiMatchs == null) return;
            // component可以为空, 如果为空, 则代表断开所有组合
            // 当有组件移除时, 说明有可能存在组合被断开了, 遍历已经所有已经组合的组合, 挨个匹配, 看看断掉的是哪个组合
            var componentIndex = removed?.ComponentIndex ?? -1;
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

        // 每当scope的父子关系发生变化时, 都重新计算这整条父子链上每个节点上的最大formatter layer, 该数据后续的跨域组合中需要用到. 该值在每次变化时只需要计算一次.
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

        // 跨域组合
        internal static void CrossCombinMatchByComponent(Scope scope, Component added) {
            // 同样的, 先从跨域组合表里查询该组件到底和哪些组合有关系, 我们只对这些组合进行尝试
            if (Combiner.CombinerCache.CrossCombinLookupTable.TryGetValue(added.ComponentIndex, out var crossCombinInfo)) {
                // 该组件作为child的跨域组合有哪些
                if (crossCombinInfo.childCombiners.Count != 0) {
                    if (scope.HasComponentsAny(crossCombinInfo.totalChildTypeCacher)) {
                        // 只针对与该组件相关的组合, 进行匹配尝试
                        CrossCombinMatchForParent(scope, scope.parentScope, 1, crossCombinInfo.childCombiners);
                    }
                }

                // 该组件作为parent的跨域组合有哪些
                if (crossCombinInfo.parentCombiners.Count != 0) {
                    // 遍历这些组合, 一个个的试
                    foreach (var combiner in crossCombinInfo.parentCombiners) {
                        // 先判断自己作为父域是否符合匹配条件
                        if (scope.HasComponentsAll(combiner.crossParentTypeCacher)) {
                            // 然后再遍历自己的子域, 挨个向自己做该组合的匹配尝试
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

        // 子域尝试向父域做跨域组合
        internal static void CrossCombinMatchForParent(Scope child, Scope parent, int layer, List<Combiner> crossCombiners = null) {
            // 方案1
            if (crossCombiners == null) {
                if (!child.HasComponentsAny(Combiner.CombinerCache.TotalCrossCombinerChildTypeCacher))
                    return;

                // 如果没有指定要尝试哪些combiners, 那就默认所有跨域组合
                crossCombiners = Combiner.CombinerCache.CrossCombiners;
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

        /// <summary>
        /// 子域尝试向父域做跨域组合
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <param name="layer">该值需要调用时告知, 且必须是正确的</param>
        /// <param name="crossCombiner"></param>
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
                    if (child.parentCrossMatchQueue != null)
                        if (child.parentCrossMatchQueue.Contains(parent, crossCombiner))
                            goto CONTINUE;

                    // 再判断该组合有没有覆盖者, 如果有, 则不能形成组合
                    if (child.multiMatchs != null)
                        if (Combiner.CombinerCache.InverseOverrides.TryGetValue(crossCombiner.id, out var ids)) {
                            foreach (var overrideCombinerId in ids) {
                                if (child.multiMatchs.Contains(overrideCombinerId)) {
                                    goto CONTINUE;
                                }
                            }
                        }

                    // 到这里, 就说明可以形成组合了
                    child.parentCrossMatchQueue ??= new();
                    parent.childCrossMatchQueue ??= new();
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
        internal static void CrossDecombinMatchByComponent(Scope scope, Component removed) {
            var componentIndex = removed.ComponentIndex;
            scope.parentCrossMatchQueue?.Select((parentScope, combiner) => {
                if (combiner.crossChildTypeCacher.Contains(componentIndex)) {
                    parentScope.childCrossMatchQueue.Dequeue(scope, combiner);
                    if (parentScope.childCrossMatchQueue.Count == 0) parentScope.childCrossMatchQueue = null;
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
                    if (childScope.parentCrossMatchQueue.Count == 0) childScope.parentCrossMatchQueue = null;
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

        // 尝试向父域断开所有当前存在的跨域组合
        internal static void CrossDecombinMatchForParent(Scope child, Scope parent) {
            // 遍历child所有的父级跨域组合, 然后判断每个组合中的parent scope是否是目标parent, 或者是目标parent的父级, 这两种情况都说明该组合符合断开条件.
            child.parentCrossMatchQueue?.SelectMatchs(match => {
                var parentScope = match.scope;
                if (parentScope == parent || parentScope.Entity.IsParentOf(parent.Entity)) {
                    foreach (var combiner in match.combiners) {
                        if (parentScope.childCrossMatchQueue == null) break;
                        parentScope.childCrossMatchQueue.Dequeue(child, combiner);
                        if (parentScope.childCrossMatchQueue.Count == 0) parentScope.childCrossMatchQueue = null;
                        _componentCache.Clear();
                        parentScope.GetComponentsOfTypeCacher(combiner.crossParentTypeCacher, _componentCache);
                        child.GetComponentsOfTypeCacher(combiner.crossChildTypeCacher, _componentCache);
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

        /// 手动进行一次跨域匹配
        protected void CrossCombinMatch() {
            if (this.parentScope == null) return;
            CrossCombinMatchForParent(this, this.parentScope, 1);
            this.ForeachChildrenScope((childScope, layer) => {
                CrossCombinMatchForParent(childScope, this.parentScope, layer + 1); // 
            });
        }

        /// 手动进行一次跨域匹配
        protected void CrossCombinMatch(Scope parent) {
            if (parent == null) throw new NullReferenceException("parent scope is null");
            var parentLayer = this.GetLayerNum(parent);
            if (parentLayer == -1) return;

            CrossCombinMatchForParent(this, parent, parentLayer);
            this.ForeachChildrenScope((childScope, layer) => {
                CrossCombinMatchForParent(childScope, parent, layer + parentLayer); // 
            });
        }

        /// 手动进行一次跨域匹配
        protected void CrossCombinMatch(Component added) {
            CrossCombinMatchByComponent(this, added);
        }

        /// 手动进行一次跨域断开
        protected void CrossDecombinMatch() {
            if (this.parentScope == null) return;
            CrossDecombinMatchForParent(this, this.parentScope);
            this.ForeachChildrenScope((childScope, _) => {
                CrossDecombinMatchForParent(childScope, this.parentScope); // 
            });
        }

        /// 手动进行一次跨域断开
        protected void CrossDecombinMatch(Scope parent) {
            if (parent == null) throw new NullReferenceException("parent scope is null");
            CrossDecombinMatchForParent(this, parent);
            this.ForeachChildrenScope((childScope, _) => {
                CrossDecombinMatchForParent(childScope, parent); // 
            });
        }

        /// 手动进行一次跨域断开
        protected void CrossDecombinMatch(Component removed) {
            CrossDecombinMatchByComponent(this, removed);
        }

        internal void CombinAll() {
            MultiCombinMatch(this, null);
            this.CalcMaximumCrossLayerInTheory();
            CrossCombinMatchForParent(this, this.parentScope, 1);
        }

        internal void DecombinAll() {
            MultiDecombinMatch(this, null);
            CrossDecombinMatchForParent(this, this.parentScope);
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