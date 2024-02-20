using System;
using System.Runtime.CompilerServices;

namespace Hsenl {
    // 模拟异步函数的底层原理
    internal class AsyncFunctionSimulated {
        public async HTask Func1() {
            // do1_1
            await Func2();
            // do1_2
        }

        public async HTask Func2() {
            // do2_1
            await Func3();
            // do2_2
        }

        public async HTask Func3() {
            // do3_1
            await HTask.Completed;
            // do3_2
        }

        // --- 以下是我根据上面三个异步函数, 模拟的一份相同效果的显式版本, 可以看出异步的原理是什么, 各个函数都是干什么的, 返回的参数都有什么用

        public HTask Func1Internal() {
            // 我们原Func1里的内容会在编译时, 被替换成类似下面的内容
            var sm = new StateMachine_Func1();
            sm.builder = AsyncHTaskMethodBuilder.Create();
            sm.simulated = this;
            sm.builder.Start(ref sm);
            sm.MoveNext(); // 这一步是我们写在Start里的, 我把他拿到这, 看的更清楚
            return sm.builder.Task;
        }

        // StateMachine_Func1是由系统在编译时自动创建的, 就像迭代器那样.
        // 假如是async void这种, 则系统会指定默认的 AsyncVoidMethodBuilder, 作为Func1的builder
        // 假如我们不写async, 那么将不会生成状态机
        private struct StateMachine_Func1 : IAsyncStateMachine {
            public AsyncHTaskMethodBuilder builder; // 这个builder是Func1返回的那个task的builder
            public AsyncFunctionSimulated simulated;

            public void MoveNext() {
                try {
                    // do1_1 // 我们Func1中的内容被移到这里来了

                    // 这里是单个await, 如果对于写在foreach中的await, 具体实现也会相应变化, 但意思不变, 都是找到await就处理, 找不到, 就跳到最后一步 builder.SetResult(), 并结束状态机
                    var task = this.simulated.Func2Internal(); // 这里往下的内容就是await关键字实际干的事
                    var awaiter = task.GetAwaiter();
                    if (awaiter.IsCompleted) {
                        awaiter.GetResult(); // 如果Task是泛型, 该方法会有返回值的, 返回值会返回给await之前的变量
                    }
                    else {
                        this.builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                        // 这句本是我们在AwaitUnsafeOnCompleted方法里写的, 我把他拿到这, 能更清楚的看到这个关键的回调是在干什么, 当然这步操作我们可以自定义, 下面这种是最基础写法
                        // 当前因为completed判断失败, 导致return, 那就把回调交给对方, 等对方完成的时候, 通知我们再MoveNext一次,
                        // 值得注意的是, 通知我们再MoveNext的这一次, 将不再判断awaiter的IsCompleted, 而直接执行awaiter.GetResult()
                        awaiter.OnCompleted(this.MoveNext);
                        return;
                    }

                    // do1_2 放到这里, 以实现do1_2能挂起
                }
                catch (Exception e) {
                    this.builder.SetException(e);
                    return;
                }

                // 同样, 如果task是泛型, 该方法也会有参数
                // 需要注意的是, SetResult并不是发生在GetResult后面, 而是先set, 再get, 上面的那个GetResult虽然看起来在前面, 但他获取的其实是StateMachine_Func2最后SetResult后的值.
                // 而这里SetResult的值, 是用于Func0的GetResult去用的.
                this.builder.SetResult();
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine) { }
        }

        public HTask Func2Internal() {
            var sm = new StateMachine_Func2();
            sm.builder = AsyncHTaskMethodBuilder.Create();
            sm.simulated = this;
            sm.builder.Start(ref sm);
            sm.MoveNext();
            return sm.builder.Task;
        }

        private struct StateMachine_Func2 : IAsyncStateMachine {
            public AsyncHTaskMethodBuilder builder;
            public AsyncFunctionSimulated simulated;

            public void MoveNext() {
                try {
                    // do2_1

                    var task = this.simulated.Func3Internal();
                    var awaiter = task.GetAwaiter();
                    if (awaiter.IsCompleted) {
                        awaiter.GetResult();
                    }
                    else {
                        this.builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                        awaiter.OnCompleted(this.MoveNext);
                        return;
                    }

                    // do2_2
                }
                catch (Exception e) {
                    this.builder.SetException(e);
                    return;
                }

                this.builder.SetResult();
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine) { }
        }

        public HTask Func3Internal() {
            var sm = new StateMachine_Func3();
            sm.builder = AsyncHTaskMethodBuilder.Create();
            sm.simulated = this;
            sm.builder.Start(ref sm);
            sm.MoveNext();
            return sm.builder.Task;
        }

        private struct StateMachine_Func3 : IAsyncStateMachine {
            public AsyncHTaskMethodBuilder builder;
            public AsyncFunctionSimulated simulated;

            public void MoveNext() {
                try {
                    // do3_1

                    var task = HTask.Completed;
                    var awaiter = task.GetAwaiter();
                    if (awaiter.IsCompleted) { // HTask.Completed 的 IsCompleted 会始终返回true
                        awaiter.GetResult();
                    }
                    else {
                        this.builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                        awaiter.OnCompleted(this.MoveNext);
                        return;
                    }

                    // do3_2
                }
                catch (Exception e) {
                    this.builder.SetException(e);
                    return;
                }

                this.builder.SetResult();
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine) { }
        }
    }
}