using System;
using System.Collections.Generic;

namespace Hsenl {
    // multi combiner 是匹配两个及以上的组合, 且只会在单个领域内做组合匹配
    // 组合器的参数, 并不支持继承组件类型, 声明的是什么组件就只会匹配该组件
    [CombinerOptions]
    public abstract class Combiner<T1, T2> : Combiner where T1 : Component where T2 : Component {
        private T1 member1;
        private T2 member2;

        internal override void Combin(IList<Component> components) {
            base.Combin(components);
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
            }
            catch (Exception e) {
                Log.Error(e);
            }

            if (this.actionCounter != 0) {
                this.actionCounters[this.GetComponentCombineHashCode()] = this.actionCounter;
            }
        }

        internal override void Decombin(IList<Component> components) {
            base.Decombin(components);
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
            }
            catch (Exception e) {
                Log.Error(e);
            }

            var hashcode = this.GetComponentCombineHashCode();
            if (this.actionCounters.TryGetValue(hashcode, out var counter)) {
                if (counter != this.actionCounter) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}/{counter}' '{this.GetType().Name}'");
                }

                this.actionCounters.Remove(hashcode);
            }

            this.member1 = null;
            this.member2 = null;
        }

        protected override int GetComponentCombineHashCode() {
            return HashCode.Combine(this.member1.InstanceId, this.member2.InstanceId);
        }

        protected abstract void OnCombin(T1 arg1, T2 arg2);

        protected abstract void OnDecombin(T1 arg1, T2 arg2);
    }

    [CombinerOptions]
    public abstract class Combiner<T1, T2, T3> : Combiner where T1 : Component where T2 : Component where T3 : Component {
        private T1 member1;
        private T2 member2;
        private T3 member3;

        internal override void Combin(IList<Component> components) {
            base.Combin(components);
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
            }
            catch (Exception e) {
                Log.Error(e);
            }

            if (this.actionCounter != 0) {
                this.actionCounters[this.GetComponentCombineHashCode()] = this.actionCounter;
            }
        }

        internal override void Decombin(IList<Component> components) {
            base.Decombin(components);
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
            }
            catch (Exception e) {
                Log.Error(e);
            }

            var hashcode = this.GetComponentCombineHashCode();
            if (this.actionCounters.TryGetValue(hashcode, out var counter)) {
                if (counter != this.actionCounter) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}/{counter}' '{this.GetType().Name}'");
                }

                this.actionCounters.Remove(hashcode);
            }

            this.member1 = null;
            this.member2 = null;
            this.member3 = null;
        }

        protected override int GetComponentCombineHashCode() {
            return HashCode.Combine(this.member1.InstanceId, this.member2.InstanceId, this.member3.InstanceId);
        }

        protected abstract void OnCombin(T1 arg1, T2 arg2, T3 arg3);

        protected abstract void OnDecombin(T1 arg1, T2 arg2, T3 arg3);
    }

    [CombinerOptions]
    public abstract class Combiner<T1, T2, T3, T4> : Combiner where T1 : Component where T2 : Component where T3 : Component where T4 : Component {
        private T1 member1;
        private T2 member2;
        private T3 member3;
        private T4 member4;

        internal override void Combin(IList<Component> components) {
            base.Combin(components);
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
            }
            catch (Exception e) {
                Log.Error(e);
            }

            if (this.actionCounter != 0) {
                this.actionCounters[this.GetComponentCombineHashCode()] = this.actionCounter;
            }
        }

        internal override void Decombin(IList<Component> components) {
            base.Decombin(components);
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
            }
            catch (Exception e) {
                Log.Error(e);
            }

            var hashcode = this.GetComponentCombineHashCode();
            if (this.actionCounters.TryGetValue(hashcode, out var counter)) {
                if (counter != this.actionCounter) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}/{counter}' '{this.GetType().Name}'");
                }

                this.actionCounters.Remove(hashcode);
            }

            this.member1 = null;
            this.member2 = null;
            this.member3 = null;
            this.member4 = null;
        }

        protected override int GetComponentCombineHashCode() {
            return HashCode.Combine(this.member1.InstanceId, this.member2.InstanceId, this.member3.InstanceId, this.member4.InstanceId);
        }

        protected abstract void OnCombin(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        protected abstract void OnDecombin(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    [CombinerOptions]
    public abstract class Combiner<T1, T2, T3, T4, T5> : Combiner
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

        internal override void Combin(IList<Component> components) {
            base.Combin(components);
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
            }
            catch (Exception e) {
                Log.Error(e);
            }

            if (this.actionCounter != 0) {
                this.actionCounters[this.GetComponentCombineHashCode()] = this.actionCounter;
            }
        }

        internal override void Decombin(IList<Component> components) {
            base.Decombin(components);
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
            }
            catch (Exception e) {
                Log.Error(e);
            }

            var hashcode = this.GetComponentCombineHashCode();
            if (this.actionCounters.TryGetValue(hashcode, out var counter)) {
                if (counter != this.actionCounter) {
                    // 调用过多少次EnqueueAction, 那么断开组合时, 就必须要DequeueAction多少次. 以保证每个回调都被正确的移除了
                    Log.Error($"decombin action counter error '{this.actionCounter}/{counter}' '{this.GetType().Name}'");
                }

                this.actionCounters.Remove(hashcode);
            }

            this.member1 = null;
            this.member2 = null;
            this.member3 = null;
            this.member4 = null;
            this.member5 = null;
        }

        protected override int GetComponentCombineHashCode() {
            return HashCode.Combine(this.member1.InstanceId, this.member2.InstanceId, this.member3.InstanceId, this.member4.InstanceId,
                this.member5.InstanceId);
        }

        protected abstract void OnCombin(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        protected abstract void OnDecombin(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    [Combiner(CombinerType.MultiCombiner)]
    public abstract class MultiCombiner<T1, T2> : Combiner<T1, T2> where T1 : Component where T2 : Component { }

    [Combiner(CombinerType.MultiCombiner)]
    public abstract class MultiCombiner<T1, T2, T3> : Combiner<T1, T2, T3> where T1 : Component where T2 : Component where T3 : Component { }

    [Combiner(CombinerType.MultiCombiner)]
    public abstract class MultiCombiner<T1, T2, T3, T4> : Combiner<T1, T2, T3, T4>
        where T1 : Component
        where T2 : Component
        where T3 : Component
        where T4 : Component { }

    [Combiner(CombinerType.MultiCombiner)]
    public abstract class MultiCombiner<T1, T2, T3, T4, T5> : Combiner<T1, T2, T3, T4, T5>
        where T1 : Component
        where T2 : Component
        where T3 : Component
        where T4 : Component
        where T5 : Component { }

    [Combiner(CombinerType.CrossCombiner)]
    public abstract class CrossCombiner<T1, T2> : Combiner<T1, T2> where T1 : Component where T2 : Component { }

    [Combiner(CombinerType.CrossCombiner)]
    public abstract class CrossCombiner<T1, T2, T3> : Combiner<T1, T2, T3> where T1 : Component where T2 : Component where T3 : Component { }

    [Combiner(CombinerType.CrossCombiner)]
    public abstract class CrossCombiner<T1, T2, T3, T4> : Combiner<T1, T2, T3, T4>
        where T1 : Component
        where T2 : Component
        where T3 : Component
        where T4 : Component { }

    [Combiner(CombinerType.CrossCombiner)]
    public abstract class CrossCombiner<T1, T2, T3, T4, T5> : Combiner<T1, T2, T3, T4, T5>
        where T1 : Component
        where T2 : Component
        where T3 : Component
        where T4 : Component
        where T5 : Component { }
}