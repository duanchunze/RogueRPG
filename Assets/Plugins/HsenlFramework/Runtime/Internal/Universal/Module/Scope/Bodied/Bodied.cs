using System;
using System.Collections.Generic;
using System.Reflection;
using MemoryPack;

namespace Hsenl {
    /* Bodied系统旨在定义一种关系, 将代表功能的组件(unbodied)与代表身份的组件(bodied)区分开
     * 如果要使用该系统, 首先需要区分哪些组件是有形体, 哪些组件是无形体
     * 一个有形体, 比如一个人, 一个子弹, 一个物品, 都属于一个有形体
     * 如何去判断一个事物是bodied还是unbodied?
     * 除非你能确定这个事物一定是一个无形体, 否则的话, 都定义成bodied, 比如数值组件, 一般来说可以定义成一个无形体, 而像有些模糊的, 比如技能, 虽然以常识来说, 技能是一个虚无缥缈的东西,
     * 但在游戏中, 技能一般都是作为一个独立的事物存在的, 它可以拆卸, 可以拥有属于自己的组件, 所以把技能定义成一个有形体.
     * 一个事物被定义成有形体, 那他就可以灵活的摇摆, 在独立个体与依赖者两种身份之间摇摆, 比如一个人是一个独立个体, 一个背包也是一个独立个体, 但如果人背起了背包, 那背包便不再是一个独立个体, 
     * 而是这个人的依赖者
     *
     * 默认情况下, 我们规定最上面的bodied为attachedBodied, 其下其他的bodied都只是普通的bodied, 当然我们也可以自定义我们自己的规则.
     * 例如一个actor下挂载了abilityBar, abilityBar下又挂载了一些ability, 那么actor因为在最上面的, 所以他就是ab, 而其余的像abilityBar, abilities, 都是普通的bodied, 而这些bodied的
     * ab, 指的自然就是最上面的那个actor.
     *
     * 意义: bodied系统用一种规则定义了所属关系, 在做沙盒类游戏的时候, 这很有用, 比如现在有一种需求, 我们把身上的背包丢到地上, 然后背包自己长腿跑了. 我们只需要把背包从actor父级上拿下来,
     * 然后给他挂上数值组件、技能、AI、等组件, 他就能跑了, 当他的父级从actor变为null的时候, 他自己就变成了ab, 他下面的移动模块就会以它的transform作为操作对象.
     *
     * 不足: 即便如此, 在面对复杂的沙盒游戏的时候, 依然不够, 例如, 现在actor骑上了一个装甲, 理论上, ab就由actor变成了装甲, 移动技能也改为操作装甲的transform这没问题, 但是获取
     * 移动速度依然要从actor的身上去获取, 更别说假如人物在装甲上也能移动呢? 移动速度是人物移速+装甲移速呢? 仅仅定义一个所属关系的规则, 显然不足以处理这种复杂的情况.
     * 这就让bodied系统的存在显得尴尬, 要么就完善, 要么干脆移除该系统.
     */
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Bodied : Scope, IBodied, IUnbodiedHead {
        [MemoryPackOrder(3)]
        [MemoryPackInclude]
        protected internal BodiedStatus status;

        [MemoryPackIgnore]
        private Bodied _attachedBodied;

        [MemoryPackIgnore]
        private Bodied _parentAttachedBodied;

        [MemoryPackIgnore]
        public BodiedStatus Status {
            get => this.status;
            set {
                if (this.status == value)
                    return;

                this.status = value;

                switch (value) {
                    case BodiedStatus.Individual:
                        // 升为独立个体, 把自己的所有者改为自己, 原来的所有者则改为自己的父所有者
                        var prev = this._attachedBodied;
                        this.AttachedBodied = this;
                        this._parentAttachedBodied = prev;
                        break;
                    case BodiedStatus.Dependent:
                        // 自己降为依赖者, 把自己的所有者改为自己上面的独立体, 而父所有者自然就是新所有者的父级, 自己原来的父级可以清空了
                        this.AttachedBodied = this.FindScopeByStatusInParent(BodiedStatus.Individual);
                        this._parentAttachedBodied = null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        [MemoryPackIgnore]
        Bodied IUnbodiedHead.Bodied => this;

        [MemoryPackIgnore]
        public Bodied AttachedBodied {
            get {
                if (this.IsDisposed) {
                    throw new NullReferenceException("The bodied has been destroyed, but you're still trying to get it");
                }

                return this.status == BodiedStatus.Individual ? this : this._attachedBodied;
            }
            private set {
                if (this._attachedBodied == value)
                    return;

                this._attachedBodied = value;

                // 递归把自己所有的非独立体的子域的所有者, 都修改为目标
                RecursiveModifiChildrenOwner(this.childrenScopes);

                void RecursiveModifiChildrenOwner(List<Scope> children) {
                    if (children == null) return;

                    for (int i = 0, len = children.Count; i < len; i++) {
                        var child = children[i];
                        if (child is Bodied bodied) {
                            if (bodied.Status == BodiedStatus.Individual) {
                                continue;
                            }

                            bodied._attachedBodied = value;
                        }

                        // 如果是一个非独立体, 则继续向下递归
                        RecursiveModifiChildrenOwner(child.childrenScopes);
                    }
                }
            }
        }

        [MemoryPackIgnore]
        public Bodied ParentAttachedBodied {
            get {
                if (this.IsDisposed) {
                    throw new NullReferenceException("The bodied has been destroyed, but you're still trying to get it");
                }

                return this.status == BodiedStatus.Individual ? this._parentAttachedBodied : this._attachedBodied?.ParentAttachedBodied;
            }
        }

        public override Scope ParentScope {
            get => this.parentScope;
            internal set {
                if (this.parentScope == value)
                    return;

                this.OnBeforeParentScopeChangeInternal(value);

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

                // 重写父级属性, 把这段代码插入到这里, 目的是为了在形成组合时, bodied的关系也ok了
                switch (this.status) {
                    case BodiedStatus.Individual:
                        this._parentAttachedBodied = this.FindScopeByStatusInParent(BodiedStatus.Individual);
                        break;
                    case BodiedStatus.Dependent:
                        this.AttachedBodied = this.FindScopeByStatusInParent(BodiedStatus.Individual);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
                if (component is not Unbodied unbodied)
                    return;

                unbodied.unbodiedHead = this;

                this.elements.Add(component.ComponentIndex, unbodied);
            });
        }

        protected internal override void OnDestroyFinish() {
            base.OnDestroyFinish();
            this.status = default;
            this._attachedBodied = null;
            this._parentAttachedBodied = null;
        }

        internal override void OnComponentAddInternal(Component component) {
            if (component is Scope) throw new Exception($"one entity only one scope. '{this.Name}' '{component.GetType().Name}'");
            if (component is not Unbodied unbodied)
                return;

            if (!this.elements.TryAdd(component.ComponentIndex, unbodied)) {
                throw new Exception($"this component is alrealy has in scope. '{this.GetType().Name}' '{component.GetType().Name}'");
            }

            unbodied.unbodiedHead = this;

            MultiCombinMatch(this, component);
            if (this.crossMatchMode != CrossMatchMode.Manual) {
                CrossCombinMatchByComponent(this, component);
            }
        }

        internal override void OnComponentRemoveInternal(Component component) {
            if (component is not Unbodied unbodied)
                return;

            this.elements.Remove(component.ComponentIndex);

            unbodied.unbodiedHead = null;

            MultiDecombinMatch(this, component);
            if (this.crossMatchMode != CrossMatchMode.Manual) {
                CrossDecombinMatchByComponent(this, component);
            }
        }

        private Bodied FindScopeByStatusInParent(BodiedStatus sta) {
            var curr = this.parentScope;
            while (curr != null) {
                if (curr is Bodied bodied) {
                    if (bodied.status == sta) {
                        return bodied;
                    }
                }

                curr = curr.parentScope;
            }

            return null;
        }

        /// 从整个有形体域内获取组件
        public T GetComponentInBodied<T>() where T : class {
            var comp = this.GetComponent<T>();
            if (comp == null) {
                comp = GetByChildren(this.childrenScopes);
            }

            T GetByChildren(List<Scope> children) {
                T t = null;
                if (children != null) {
                    for (int i = 0, len = children.Count; i < len; i++) {
                        var child = children[i];
                        // if (child is Substantive { _status: SubstantiveStatus.Principal }) continue;

                        t = child.GetComponent<T>();
                        if (t != null)
                            break;
                    }

                    for (int i = 0, len = children.Count; i < len; i++) {
                        var child = children[i];
                        if (child is Bodied { status: BodiedStatus.Individual }) continue;

                        t = GetByChildren(child.childrenScopes);
                        if (t != null)
                            break;
                    }
                }

                return t;
            }

            return comp;
        }

        /// 从整个有形体域内寻找指定域
        public T FindScopeInBodied<T>() where T : class {
            T GetByChildren(List<Scope> children) {
                if (children != null) {
                    for (int i = 0, len = children.Count; i < len; i++) {
                        switch (children[i]) {
                            // case Substantive { _status: SubstantiveStatus.Principal }:
                            //     continue;

                            case T t:
                                return t;
                        }
                    }

                    for (int i = 0, len = children.Count; i < len; i++) {
                        if (children[i] is Bodied { status: BodiedStatus.Individual })
                            continue;

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

        /// 从整个有形体域内寻找指定域s
        public T[] FindScopesInBodied<T>() where T : class {
            using var list = ListComponent<T>.Create();

            void GetByChildren(List<Scope> children, List<T> l) {
                if (children != null) {
                    for (int i = 0, len = children.Count; i < len; i++) {
                        switch (children[i]) {
                            // case Substantive { _status: SubstantiveStatus.Principal }:
                            //     continue;

                            case T t:
                                l.Add(t);
                                break;
                        }
                    }

                    for (int i = 0, len = children.Count; i < len; i++) {
                        if (children[i] is Bodied { status: BodiedStatus.Individual })
                            continue;

                        GetByChildren(children[i].childrenScopes, l);
                    }
                }
            }

            GetByChildren(this.childrenScopes, list);
            return list.ToArray();
        }

        protected override void OnBeforeParentScopeChanged(Scope future) {
            // 默认最上面的bodied为individual, 下面的其他bodied都是dependent
            this.status = future is Bodied ? BodiedStatus.Dependent : BodiedStatus.Individual;
        }
    }
}