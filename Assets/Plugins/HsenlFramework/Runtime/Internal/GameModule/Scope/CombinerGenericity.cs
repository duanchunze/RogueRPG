using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    // 一个也能组合, 比较牵强, 主要是为了把逻辑都集中在组合模块, 为了统一性. 效果等同于<当组件被添加时事件>
    [Combiner(CombinerType.SingleCombiner)]
    public abstract class SingleCombiner<T> : Combiner where T : Component {
        private T member; // 要禁止继承类直接使用member, 因为很容易误操作, 比如在一个独立的函数中, 直接使用该member

        private int actionCount;

        internal override void OnCombin(Component component) {
            base.OnCombin(component);
            if (component is not T t) {
                throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{component.GetType().Name}'");
            }

            this.member = t;

            try {
                this.OnCombin(this.member);
                this.actionCount = this.actionCounter;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal override void OnDecombin(Component component) {
            base.OnDecombin(component);
            if (component is not T t) {
                throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{component.GetType().Name}'");
            }

            this.member = t;

            try {
                this.OnDecombin(this.member);
                if (this.actionCounter != this.actionCount) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}'-'{this.actionCount}' '{this.GetType()}'");
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override int GetComponentCombineHashCode() {
            return this.member.InstanceId;
        }

        protected abstract void OnCombin(T arg);

        protected abstract void OnDecombin(T arg);
    }

    // multi combiner 是匹配两个及以上的组合, 且只会在单个领域内做组合匹配
    // 组合器的参数, 并不支持继承组件类型, 声明的是什么组件就只会匹配该组件
    [Combiner(CombinerType.MultiCombiner)]
    public abstract class MultiCombiner<T1, T2> : Combiner where T1 : Component where T2 : Component {
        private T1 member1;
        private T2 member2;

        private int actionCount;

        internal override void OnCombin(IList<Component> components) {
            base.OnCombin(components);
            if (components.Count != 2) Log.Warning($"combiner components count has something the matter '{components.Count}'");
            var len = components.Count;
            if (len != 2) throw new Exception($"combiner member mismatch: '{typeof(T1)}' '{typeof(T2)}'");
            for (var i = 0; i < len; i++) {
                switch (components[i]) {
                    case T1 t1:
                        this.member1 = t1;
                        break;
                    case T2 t2:
                        this.member2 = t2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{components[i]?.GetType().Name}'");
                }
            }

            // Debug.Log($"{this.GetType().Name}: {typeof(T1)} {typeof(T2)} - combiner - '{this.member1}' '{this.member2}'");
            try {
                this.OnCombin(this.member1, this.member2);
                this.actionCount = this.actionCounter;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal override void OnDecombin(IList<Component> components) {
            base.OnDecombin(components);
            if (components.Count != 2) Log.Warning($"combiner components count has something the matter '{components.Count}'");
            for (int i = 0, len = components.Count; i < len; i++) {
                switch (components[i]) {
                    case T1 t1:
                        this.member1 = t1;
                        break;
                    case T2 t2:
                        this.member2 = t2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{components[i]?.GetType().Name}'");
                }
            }

            // Debug.Log($"{this.GetType().Name}: {typeof(T1)} {typeof(T2)} - decombiner - '{this.member1}' '{this.member2}'");
            try {
                this.OnDecombin(this.member1, this.member2);
                if (this.actionCounter != this.actionCount) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}'-'{this.actionCount}' '{this.GetType()}'");
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override int GetComponentCombineHashCode() {
            return HashCode.Combine(this.member1.InstanceId, this.member2.InstanceId);
        }

        protected abstract void OnCombin(T1 arg1, T2 arg2);

        protected abstract void OnDecombin(T1 arg1, T2 arg2);
    }

    [Combiner(CombinerType.MultiCombiner)]
    public abstract class MultiCombiner<T1, T2, T3> : Combiner where T1 : Component where T2 : Component where T3 : Component {
        private T1 member1;
        private T2 member2;
        private T3 member3;

        private int actionCount;

        internal override void OnCombin(IList<Component> components) {
            base.OnCombin(components);
            if (components.Count != 3) Log.Warning($"combiner components count has something the matter '{components.Count}'");
            for (int i = 0, len = components.Count; i < len; i++) {
                switch (components[i]) {
                    case T1 t1:
                        this.member1 = t1;
                        break;
                    case T2 t2:
                        this.member2 = t2;
                        break;
                    case T3 t3:
                        this.member3 = t3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{components[i]?.GetType().Name}'");
                }
            }

            // Debug.Log($"{this.GetType().Name}: {typeof(T1)} {typeof(T2)} {typeof(T3)} - combiner - '{this.member1}' '{this.member2}' '{this.member3}'");
            try {
                this.OnCombin(this.member1, this.member2, this.member3);
                this.actionCount = this.actionCounter;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal override void OnDecombin(IList<Component> components) {
            base.OnDecombin(components);
            if (components.Count != 3) Log.Warning($"combiner components count has something the matter '{components.Count}'");
            for (int i = 0, len = components.Count; i < len; i++) {
                switch (components[i]) {
                    case T1 t1:
                        this.member1 = t1;
                        break;
                    case T2 t2:
                        this.member2 = t2;
                        break;
                    case T3 t3:
                        this.member3 = t3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{components[i]?.GetType().Name}'");
                }
            }

            // Debug.Log($"{this.GetType().Name}: {typeof(T1)} {typeof(T2)} {typeof(T3)} - decombiner - '{this.member1}' '{this.member2}' '{this.member3}'");
            try {
                this.OnDecombin(this.member1, this.member2, this.member3);
                if (this.actionCounter != this.actionCount) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}'-'{this.actionCount}' '{this.GetType()}'");
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override int GetComponentCombineHashCode() {
            return HashCode.Combine(this.member1.InstanceId, this.member2.InstanceId, this.member3.InstanceId);
        }

        protected abstract void OnCombin(T1 arg1, T2 arg2, T3 arg3);

        protected abstract void OnDecombin(T1 arg1, T2 arg2, T3 arg3);
    }

    [Combiner(CombinerType.MultiCombiner)]
    public abstract class MultiCombiner<T1, T2, T3, T4> : Combiner where T1 : Component where T2 : Component where T3 : Component where T4 : Component {
        private T1 member1;
        private T2 member2;
        private T3 member3;
        private T4 member4;

        private int actionCount;

        internal override void OnCombin(IList<Component> components) {
            base.OnCombin(components);
            if (components.Count != 4) Log.Warning($"combiner components count has something the matter '{components.Count}'");
            for (int i = 0, len = components.Count; i < len; i++) {
                switch (components[i]) {
                    case T1 t1:
                        this.member1 = t1;
                        break;
                    case T2 t2:
                        this.member2 = t2;
                        break;
                    case T3 t3:
                        this.member3 = t3;
                        break;
                    case T4 t4:
                        this.member4 = t4;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{components[i]?.GetType().Name}'");
                }
            }

            // Debug.Log(
            //     $"{this.GetType().Name}: {typeof(T1)} {typeof(T2)} {typeof(T3)} {typeof(T4)} - combiner - '{this.member1}' '{this.member2}' '{this.member3}' '{this.member4}'");
            try {
                this.OnCombin(this.member1, this.member2, this.member3, this.member4);
                this.actionCount = this.actionCounter;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal override void OnDecombin(IList<Component> components) {
            base.OnDecombin(components);
            if (components.Count != 4) Log.Warning($"combiner components count has something the matter '{components.Count}'");
            for (int i = 0, len = components.Count; i < len; i++) {
                switch (components[i]) {
                    case T1 t1:
                        this.member1 = t1;
                        break;
                    case T2 t2:
                        this.member2 = t2;
                        break;
                    case T3 t3:
                        this.member3 = t3;
                        break;
                    case T4 t4:
                        this.member4 = t4;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{components[i]?.GetType().Name}'");
                }
            }

            // Debug.Log(
            //     $"{this.GetType().Name}: {typeof(T1)} {typeof(T2)} {typeof(T3)} {typeof(T4)} - decombiner - '{this.member1}' '{this.member2}' '{this.member3}' '{this.member4}'");
            try {
                this.OnDecombin(this.member1, this.member2, this.member3, this.member4);
                if (this.actionCounter != this.actionCount) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}'-'{this.actionCount}' '{this.GetType()}'");
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override int GetComponentCombineHashCode() {
            return HashCode.Combine(this.member1.InstanceId, this.member2.InstanceId, this.member3.InstanceId, this.member4.InstanceId);
        }

        protected abstract void OnCombin(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        protected abstract void OnDecombin(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    [Combiner(CombinerType.MultiCombiner)]
    public abstract class MultiCombiner<T1, T2, T3, T4, T5> : Combiner
        where T1 : Component
        where T2 : Component
        where T3 : Component
        where T4 : Component
        where T5 : Component {
        private T1 member1;
        private T2 member2;
        private T3 member3;
        private T4 member4;
        private T5 member5;

        private int actionCount;

        internal override void OnCombin(IList<Component> components) {
            base.OnCombin(components);
            if (components.Count != 5) Log.Warning($"combiner components count has something the matter '{components.Count}'");
            for (int i = 0, len = components.Count; i < len; i++) {
                switch (components[i]) {
                    case T1 t1:
                        this.member1 = t1;
                        break;
                    case T2 t2:
                        this.member2 = t2;
                        break;
                    case T3 t3:
                        this.member3 = t3;
                        break;
                    case T4 t4:
                        this.member4 = t4;
                        break;
                    case T5 t5:
                        this.member5 = t5;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{components[i]?.GetType().Name}'");
                }
            }

            // Debug.Log(
            //     $"{this.GetType().Name}: {typeof(T1)} {typeof(T2)} {typeof(T3)} {typeof(T4)} {typeof(T5)}" +
            //     $"- combiner - '{this.member1}' '{this.member2}' '{this.member3}' '{this.member4}' '{this.member5}'");
            try {
                this.OnCombin(this.member1, this.member2, this.member3, this.member4, this.member5);
                this.actionCount = this.actionCounter;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal override void OnDecombin(IList<Component> components) {
            base.OnDecombin(components);
            if (components.Count != 5) Log.Warning($"combiner components count has something the matter '{components.Count}'");
            for (int i = 0, len = components.Count; i < len; i++) {
                switch (components[i]) {
                    case T1 t1:
                        this.member1 = t1;
                        break;
                    case T2 t2:
                        this.member2 = t2;
                        break;
                    case T3 t3:
                        this.member3 = t3;
                        break;
                    case T4 t4:
                        this.member4 = t4;
                        break;
                    case T5 t5:
                        this.member5 = t5;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"'{this.GetType().Name}' '{components[i]?.GetType().Name}'");
                }
            }

            // Debug.Log(
            //     $"{this.GetType().Name}: {typeof(T1)} {typeof(T2)} {typeof(T3)} {typeof(T4)} {typeof(T5)}" +
            //     $"- decombiner - '{this.member1}' '{this.member2}' '{this.member3}' '{this.member4}' '{this.member5}'");
            try {
                this.OnDecombin(this.member1, this.member2, this.member3, this.member4, this.member5);
                if (this.actionCounter != this.actionCount) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}'-'{this.actionCount}' '{this.GetType()}'");
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override int GetComponentCombineHashCode() {
            return HashCode.Combine(this.member1.InstanceId, this.member2.InstanceId, this.member3.InstanceId, this.member4.InstanceId,
                this.member5.InstanceId);
        }

        protected abstract void OnCombin(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        protected abstract void OnDecombin(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    // cross combiner 是在multi combiner的基础上, 增加了跨域, 会在父级和自己领域联合做匹配, 跨域组合的第一个参数为主类型, 该类型只从父级去匹配, 后面的参数为子类型, 这些类型只子域去匹配
    // 默认第一泛型参数为主类型, 后面的为子类型, 如果有需要, 可以使用 CombinArgAttribute 属性来指定
    // 例如技能需要用到人物的数值, 那么就说, 技能域需要角色域身上的数值组件, 那么当技能被添加的时候, 就会去角色域去匹配, 看有没有数值组件, 如果有, 则组合成立, 否则不做操作
    // 而我们要做的只需要定义一个组合类, 数值技能组合类, NumeratorAbilityCombiner : CrossCombiner<Numerator, Ability> { } 即可, 条件满足时, 
    // 系统会自动把数值和技能返给我们, 我们自行操作即可
    // 主类型从子级匹配, 副类型从父级匹配
    [Combiner(CombinerType.CrossCombiner)]
    public abstract class CrossCombiner<T1, T2> : MultiCombiner<T1, T2> where T1 : Component where T2 : Component { }

    [Combiner(CombinerType.CrossCombiner)]
    public abstract class CrossCombiner<T1, T2, T3> : MultiCombiner<T1, T2, T3> where T1 : Component where T2 : Component where T3 : Component { }

    [Combiner(CombinerType.CrossCombiner)]
    public abstract class CrossCombiner<T1, T2, T3, T4> : MultiCombiner<T1, T2, T3, T4>
        where T1 : Component where T2 : Component where T3 : Component where T4 : Component { }
}