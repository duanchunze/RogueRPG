using System;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 有形的, 也可以看成是一个域, 比如一个角色, 一个技能, 一件装备, 这些都是一个域, 是一个独立存在的有形体
    [Serializable]
    [MemoryPackable(GenerateType.CircularReference)]
    // 这里本来声明的是抽象类, 但是因为要使用memory pack的循环引用, 所以改为普通类
    // 循环引用的条件:
    // - 目标类以及其衍生类, 只要用到了, 都必须使用 GenerateType.CircularReference
    // - 还有, 即便以及去掉了abstract, substantive 也需要用 MemoryPackFormatterProvider.Register 注册. 不清楚为什么
    public partial class Substantive : Component, ISubstantive {
        private static List<Component> _componentCache = new();

        // 父域与子域之间也可以看做是一种<御统和被御统的关系>
        [MemoryPackIgnore]
        private Substantive _parentSubstantive;

        [MemoryPackIgnore]
        private List<Substantive> _childrenSubstantives; // 只有下面一层

        [MemoryPackIgnore]
        private HashSet<int> _componentInstances = new(); // 单领域下, 组件不可以重复, 所以, 单领域下每种组合最多存在一个. 如果有需要多组件的情况, 可以使用子物体来解决

        [MemoryPackIgnore]
        private Queue<int> _multiCombinerIndexes = new(); // value: conbinerId;

        [MemoryPackIgnore]
        private MultiQueue<int, int> _crossCombinerIndexes = new(); // key: childInstanceId, value: combinerId

        [MemoryPackIgnore]
        private bool _hasCrossCombinFormatter;

        [MemoryPackIgnore]
        private Type _thisType;

        [MemoryPackIgnore]
        public Substantive ParentSubstantive {
            get => this._parentSubstantive;
            internal set {
                if (this._parentSubstantive == value)
                    return;

                // 如果有父域且其实体处于即将销毁状态, 那就表示当前subs是被一个父域连带着destroy的, 这种情况下, 不触发父子关系事件
                var childDestroy = this._parentSubstantive?.Entity.imminentDispose ?? false;

                if (!childDestroy)
                    this.OnBeforeParentSubstantiveChangeInternal(value);

                var previousParent = this._parentSubstantive;
                this._parentSubstantive = value;
                previousParent?.OnChildSubstantiveRemoveInternal(this);
                this._parentSubstantive?.OnChildSubstantiveAddInternal(this);

                if (!childDestroy)
                    this.OnAfterParentSubstantiveChangedInternal(previousParent);
            }
        }

        [MemoryPackIgnore]
        public IReadOnlyList<Substantive> ChildrenSubstantives => this._childrenSubstantives;

        private List<Substantive> GetOrCreateChildrenSubstantives() => this._childrenSubstantives ??= new();

        public T GetSubstantiveInParent<T>() where T : class, ISubstantive {
            var curr = this.ParentSubstantive;
            while (curr != null) {
                if (curr is T t)
                    return t;

                curr = curr.ParentSubstantive;
            }

            return default;
        }

        public T GetSubstaintiveInChildren<T>() where T : class, ISubstantive {
            if (this._childrenSubstantives == null) return null;
            for (int i = 0, len = this._childrenSubstantives.Count; i < len; i++) {
                if (this._childrenSubstantives[i] is T t) {
                    return t;
                }
            }

            for (int i = 0, len = this._childrenSubstantives.Count; i < len; i++) {
                this._childrenSubstantives[i].GetSubstaintiveInChildren<T>();
            }

            return null;
        }

        public T[] GetSubstaintivesInChildren<T>() where T : class, ISubstantive {
            if (this._childrenSubstantives == null) return null;
            using var list = ListComponent<T>.Create();
            this.GetSubstaintivesInChildren(list);
            return list.ToArray();
        }

        public void GetSubstaintivesInChildren<T>(List<T> list) where T : class, ISubstantive {
            if (this._childrenSubstantives == null) return;

            for (int i = 0, len = this._childrenSubstantives.Count; i < len; i++) {
                if (this._childrenSubstantives[i] is T t) {
                    list.Add(t);
                }
            }

            for (int i = 0, len = this._childrenSubstantives.Count; i < len; i++) {
                this._childrenSubstantives[i].GetSubstaintivesInChildren(list);
            }
        }

        public void ForeachChildrenSubstaintive(Action<Substantive> callback) {
            if (this._childrenSubstantives == null) return;

            for (int i = 0, len = this._childrenSubstantives.Count; i < len; i++) {
                callback.Invoke(this._childrenSubstantives[i]);
            }

            for (int i = 0, len = this._childrenSubstantives.Count; i < len; i++) {
                this._childrenSubstantives[i].ForeachChildrenSubstaintive(callback);
            }
        }

        public T GetParentSubstantiveAs<T>() where T : class, ISubstantive {
            return this.ParentSubstantive as T;
        }

        /// <summary>
        /// 只从自己的children里找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindSubstaintiveInChildren<T>() where T : class, ISubstantive {
            if (this._childrenSubstantives == null) return null;
            for (int i = 0, len = this._childrenSubstantives.Count; i < len; i++) {
                if (this._childrenSubstantives[i] is T t) {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// 只从自己的children里找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] FindSubstaintivesInChildren<T>() where T : class, ISubstantive {
            if (this._childrenSubstantives == null) return null;
            using var list = ListComponent<T>.Create();
            for (int i = 0, len = this._childrenSubstantives.Count; i < len; i++) {
                if (this._childrenSubstantives[i] is T t) {
                    list.Add(t);
                }
            }

            return list.ToArray();
        }

        internal override void OnConstructionInternal() {
            this._hasCrossCombinFormatter = CombinFormatter.CrossCombinFormatters?.ContainsKey(this.GetType()) ?? false;
            this._thisType = this.GetType();
        }

        internal override void OnAwakeInternal() {
            // 无论Substaintive是在unbodied前还是后添加, 都能确保所有应该匹配的都会被匹配
            this.Entity.ForeachComponents(component => {
                // 把之前组件连同自己一起补处理
                this._componentInstances.Add(component.GetType().GetHashCode());
                this.SingleCombinMatch(component);
            });

            this.MultiCombinMatch();

            // subs间的父子关系都是由子级向父级去报备, 因为如果父级也向子级报备一方面会重复报备, 而且父级向子级报备需要采用遍历的方式, 比较费性能
            // 也正因为都是子级向父级报备, 假如一个subs先添加一个空的entity, 然后再添加subs组件, 就会因为before parent事件不触发, 导致没报备, 所以针对这种情况, 在awake的时候,
            // 进行一次补报备. 且对于设置ParentSubstantive来说, OnAwakeInternal和OnBeforeParentChangeInternal只有一个会触发
            this.ParentSubstantive = this.Parent?.GetComponentInParent<Substantive>(true);
        }

        internal override void OnDestroyInternal() {
            // 当Substaintive被销毁时, 所有的组合都应该被断开.
            this.Entity.ForeachComponents(this.SingleDecombinMatch);
            this.MultiDecombinMatch(null);
            this.ParentSubstantive = null;
            if (this._crossCombinerIndexes.Count != 0)
                Debug.LogError("why?" + this.Name);
            // 当销毁时, 之所以不对所有子域做断开组合, 是因为Entity销毁的规则是从子实体先开始, 所以, 如果某个父域如果销毁了, 那么他的所有子域肯定已经做过断开组合的操作了
        }

        internal override void OnComponentAddInternal(Component component) {
            if (component is Substantive) throw new Exception($"one entity only one scope '{this.GetType().Name}' '{component.GetType().Name}'");
            if (component is Unbodied unbodied) unbodied.Substantive = this;
            var hash = component.GetType().GetHashCode();
            if (this._componentInstances.Contains(hash))
                throw new Exception($"this component is alrealy has in scope '{this.GetType().Name}' '{component.GetType().Name}'");
            this._componentInstances.Add(hash);
            this.SingleCombinMatch(component);
            this.MultiCombinMatch();
            // 当组件添加时, 影响的是上下所有的subs, 所以都要做一次跨域匹配
            this.ParentSubstantive?.CrossCombinMatch(this, 0);
            this.ForeachChildrenSubstaintive(childSubs => {
                // 如果不是自己的子域, 且该子域也没有指定组合格式, 就不需要尝试判断了, 因为变化的只是自身, 在这种情况下, 只会影响自己和自己的子域
                if (childSubs.ParentSubstantive != this) {
                    if (!childSubs._hasCrossCombinFormatter)
                        return;
                }

                childSubs.ParentSubstantive.CrossCombinMatch(childSubs, 0);
            });
        }

        internal override void OnComponentRemoveInternal(Component component) {
            if (component is Unbodied unbodied) {
                if (unbodied.Substantive == this) {
                    unbodied.Substantive = null;
                }
            }

            this._componentInstances.Remove(component.GetType().GetHashCode());
            this.SingleDecombinMatch(component);
            this.MultiDecombinMatch(component);
            // 当组件移除时, 可能影响上下所有的subs, 所以都要做匹配解除
            // 从父域向自己做匹配解除尝试, 被移除的组件来自于自己, 且如果有指定格式, 则持续向上尝试解组
            this.ParentSubstantive?.CrossDecombinMatch(this, component, true, this._hasCrossCombinFormatter);
            this.ForeachChildrenSubstaintive(childSubs => {
                if (childSubs.ParentSubstantive != this) {
                    if (!childSubs._hasCrossCombinFormatter)
                        return;
                }

                // 从自己向子域做匹配解除尝试, 被移除的组件来自自己, 因为只有自己发生了变化, 所以不需要持续向上尝试
                this.CrossDecombinMatch(childSubs, component, false, false);
            });
        }

        // 这里使用before parent change, 是因为他的执行顺序是最前面的, 在它之后, 会执行 child remove, 如果使用 after changed的话, 它执行在 child remove事件之后,
        // 如此, 如果我们在child remove事件有写逻辑的话, 当执行这段逻辑时, 关于subs 与 unbodied之间的父子关系的数据还没来得及更新.
        // 后来有加了forward和back的区别, 因为理论上BeforeParent, 父级还未实际改变, 但subs与unbodied系统都已经更改彼此关系, 如果用户使用before parent事件会产生疑问.
        internal override void OnParentChangedInternal(Entity previousParent) {
            this.ParentSubstantive = this.Parent?.GetComponentInParent<Substantive>(true);
        }

        private void SingleCombinMatch(Component component) {
            var componentIndex = component.ComponentIndex;
            if (Combiner.SingleCombiners.TryGetValue(componentIndex, out var combiner)) {
                combiner.OnCombin(component);
            }
        }

        private void SingleDecombinMatch(Component component) {
            var componentIndex = component.ComponentIndex;
            if (Combiner.SingleCombiners.TryGetValue(componentIndex, out var combiner)) {
                combiner.OnDecombin(component);
            }
        }

        private void MultiCombinMatch() {
            var combiners = Combiner.MultiCombiners;
            for (int i = 0, len = combiners.Count; i < len; i++) {
                var combiner = combiners[i];
                if (this._multiCombinerIndexes.Contains(combiner.id)) continue; // 已经组合过的, 就跳过
                // 遍历所有组合, 一个个匹配, 看看自己有没有符合条件的
                if (this.HasComponentsAll(combiner.allTypeCacher)) {
                    // 如果符合, 先判断该组合有没有覆盖者, 如果有, 则不能形成组合
                    if (Combiner.InverseOverrides.TryGetValue(combiner, out var list)) {
                        foreach (var overrideCombiner in list) {
                            if (this._multiCombinerIndexes.Contains(overrideCombiner.id)) {
                                goto CONTINUE;
                            }
                        }
                    }

                    // 再判断该组合有没有需要覆盖的, 如果有, 就断开其组合
                    if (Combiner.Overrides.TryGetValue(combiner, out list)) {
                        foreach (var overrideCombiner in list) {
                            if (this._multiCombinerIndexes.Contains(overrideCombiner.id)) {
                                var decombiner = Combiner.MultiCombiners[overrideCombiner.id];
                                _componentCache.Clear();
                                this.GetComponentsOfTypeCacher(decombiner.allTypeCacher, _componentCache);
                                decombiner.OnDecombin(_componentCache);
                            }
                        }
                    }

                    // 最后, 保存这个组合, 并且触发该组合的事件
                    this._multiCombinerIndexes.Enqueue(combiner.id);
                    _componentCache.Clear();
                    this.GetComponentsOfTypeCacher(combiner.allTypeCacher, _componentCache);
                    combiner.OnCombin(_componentCache);
                }

                CONTINUE:
                continue;
            }
        }

        private void MultiDecombinMatch(Component component) {
            // component可以为空, 如果为空, 则代表断开所有组合
            // 当有组件移除时, 说明有可能存在组合被断开了, 遍历已经所有已经组合的组合, 挨个匹配, 看看断掉的是哪个组合
            var componentIndex = component?.ComponentIndex ?? -1;
            var combiners = Combiner.MultiCombiners;
            var count = this._multiCombinerIndexes.Count;
            while (count-- > 0) {
                var combinerId = this._multiCombinerIndexes.Dequeue();
                var combiner = combiners[combinerId];
                if (combiner.id != combinerId) throw new Exception("It shouldn't be different");
                // 如果组合包含这个组件, 则删除该组件编号, 并触发事件
                if (componentIndex == -1 || combiner.allTypeCacher.Contains(componentIndex)) {
                    _componentCache.Clear();
                    this.GetComponentsOfTypeCacher(combiner.allTypeCacher, _componentCache);
                    if (componentIndex != -1) _componentCache.Add(component);
                    combiner.OnDecombin(_componentCache);
                    continue;
                }

                this._multiCombinerIndexes.Enqueue(combinerId);
            }
        }

        private void CrossCombinMatch(Substantive child, int layer) {
            var match = false;
            // 如果子域有formatter, 则使用该formatter覆盖默认的formatter
            if (child._hasCrossCombinFormatter) {
                var formatters = CombinFormatter.CrossCombinFormatters[child.GetType()];
                var succ = false;
                foreach (var formatter in formatters) {
                    // 对于第二层和之后的层, 跳过失败的formatter
                    if (!formatter.succ && layer != 0)
                        continue;

                    // 对于第一层, 要对所有的formatter都进行判断, 必须要得出一个结果
                    if (layer < formatter.types.Count) {
                        var type = formatter.types[layer];
                        if (type == null) {
                            // 如果该层的type被设置为null, 代表该层可以是任意subs
                            formatter.succ = true;
                        }
                        else {
                            // 否则就要判断该层是不是指定的type
                            formatter.succ = this._thisType == type;
                        }
                    }
                    else {
                        // 超出范围的肯定也是失败的
                        formatter.succ = false;
                    }

                    if (formatter.succ) {
                        // 只要有一个formatter成功, 就算这一层成功
                        succ = true;
                    }
                }

                // 如果该层formatter成功, 则该层会进行一次匹配尝试
                if (succ) {
                    match = true;
                }
            }
            else {
                // 对于没有指定formatter的子域, 则使用默认formatter的规则 -> 只向自己的父域做匹配
                if (layer == 0) {
                    match = true;
                }
            }

            if (match) {
                var combiners = Combiner.CrossCombiners;
                for (int i = 0, len = combiners.Count; i < len; i++) {
                    var combiner = combiners[i];
                    // 如果已经和这个子域做过了这个组合, 则跳过
                    if (this._crossCombinerIndexes.Contains(child.InstanceId, combiner.id)) continue;
                    // 父域做parentType匹配, 子域做childType匹配
                    if (this.HasComponentsAll(combiner.parentTypeCacher) && child.HasComponentsAll(combiner.childTypeCacher)) {
                        // 匹配成功, 把子域id 和 组合的id, 记录在父域里
                        this._crossCombinerIndexes.Enqueue(child.InstanceId, combiner.id);
                        _componentCache.Clear();
                        // 分别从自己和子域身上找到这些组件, 然后触发事件
                        this.GetComponentsOfTypeCacher(combiner.parentTypeCacher, _componentCache);
                        child.GetComponentsOfTypeCacher(combiner.childTypeCacher, _componentCache);
                        combiner.OnCombin(_componentCache);
                        // Debug.LogError("组合: " + combiner.GetType().Name);
                    }
                }

                // 跨域组合默认只会向上一级做跨域匹配, 但如果有特殊需求, 需要在组合拓展规则里注册, 注册指定的跨域组合格式
                if (child._hasCrossCombinFormatter) {
                    this.ParentSubstantive?.CrossCombinMatch(child, ++layer);
                }
            }
        }

        // 跨域组合解除, 如果解除的是整个子域, 那么该子域所有的组件产生的组合都解除
        // 如果解除的是某个指定的组件, 那么就只解除和该组件相关的组合
        private void CrossDecombinMatch(Substantive child, Component component, bool childComponent, bool keepGoing) {
            // 先判断该父域有没有和该子域做过任何组合
            if (this._crossCombinerIndexes.TryGetValue(child.InstanceId, out var indexes)) {
                // 如果指定了child component, 则只比对该组件, 否则就比对所有组件
                var componentIndex = component?.ComponentIndex ?? -1;
                var combiners = Combiner.CrossCombiners;
                var count = indexes.Count;
                // 遍历所有combiner
                while (count-- > 0) {
                    var combinerId = indexes.Dequeue();
                    var combiner = combiners[combinerId];
                    if (combiner.id != combinerId)
                        throw new Exception("wht?");

                    // 如果组合包含这个组件, 则删除该组件编号, 并触发事件
                    if (childComponent) {
                        if (componentIndex == -1 || combiner.childTypeCacher.Contains(componentIndex)) {
                            goto DECOMBIN;
                        }

                        goto CONTINUE;
                    }
                    else {
                        if (componentIndex == -1) {
                            // 如果被移除的组件是来自父域的, 则必须指定该组件
                            throw new Exception("cross decombin match, component is null");
                        }

                        if (combiner.parentTypeCacher.Contains(componentIndex)) {
                            goto DECOMBIN;
                        }

                        goto CONTINUE;
                    }

                    DECOMBIN:
                    _componentCache.Clear();
                    this.GetComponentsOfTypeCacher(combiner.parentTypeCacher, _componentCache);
                    child.GetComponentsOfTypeCacher(combiner.childTypeCacher, _componentCache);
                    if (componentIndex != -1)
                        _componentCache.Add(component);

                    combiner.OnDecombin(_componentCache);
                    // Debug.LogError("解组合: " + combiner.GetType().Name);
                    continue;

                    CONTINUE:
                    // 没被解除的组合, 重新存起来
                    indexes.Enqueue(combinerId);
                }

                if (indexes.Count == 0) {
                    this._crossCombinerIndexes.Remove(child.InstanceId);
                }
            }

            // 因为向自己的父域做解组合的判断是非常高效的, 所以不需要像组合时那样, 做很复杂的判断, 也能达到解组合的目的
            if (keepGoing) {
                this.ParentSubstantive?.CrossDecombinMatch(child, component, childComponent, true);
            }
        }

        internal void OnBeforeParentSubstantiveChangeInternal(Substantive futrueParent) {
            try {
                this.OnBeforeParentSubstantiveChange(futrueParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.Entity.ForeachComponents((component) => {
                if (component is not Unbodied unbodied)
                    return;

                unbodied.OnBeforeParentSubstantiveChangeInternal(futrueParent);
            });
        }

        internal void OnAfterParentSubstantiveChangedInternal(Substantive previousParent) {
            try {
                this.OnAfterParentSubstantiveChanged(previousParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.Entity.ForeachComponents((component) => {
                if (component is not Unbodied unbodied)
                    return;

                unbodied.OnAfterParentSubstantiveChangedInternal(previousParent);
            });
        }

        internal void OnChildSubstantiveAddInternal(Substantive child) {
            this.GetOrCreateChildrenSubstantives().Add(child);
            this.CrossCombinMatch(child, 0);

            try {
                this.OnChildSubstantiveAdd(child);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.Entity.ForeachComponents((component) => {
                if (component is not Unbodied unbodied)
                    return;

                unbodied.OnChildSubstantiveAddInternal(child);
            });
        }

        internal void OnChildSubstantiveRemoveInternal(Substantive child) {
            // 当有子域被移除时, 对该子域做一次组合解除
            this.CrossDecombinMatch(child, null, true, child._hasCrossCombinFormatter);
            if (!this._childrenSubstantives.Remove(child)) throw new Exception("why remove fail?");
            if (this._childrenSubstantives.Count == 0) this._childrenSubstantives = null;

            if (this.Entity.imminentDispose)
                return;

            try {
                this.OnChildSubstantiveRemove(child);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.Entity.ForeachComponents((component) => {
                if (component is not Unbodied unbodied)
                    return;

                unbodied.OnChildSubstantiveRemoveInternal(child);
            });
        }

        protected virtual void OnBeforeParentSubstantiveChange(Substantive futrueParentSubs) { }

        protected virtual void OnAfterParentSubstantiveChanged(Substantive previousParentSubs) { }

        protected virtual void OnChildSubstantiveAdd(Substantive childSubs) { }

        protected virtual void OnChildSubstantiveRemove(Substantive childSubs) { }
    }
}