using System;

namespace Hsenl {
    public static class ShadowFunctionExampleInvocation {
        public static void Invoke() {
            ShadowFunctionSystemExample_Source s1 = new();
            s1.Func1();
            s1.Func2();
            s1.Func3().Tail();
            s1.Func4().Tail();
            s1.Func5();
            s1.Func11();
            s1.Func22();
            s1.Func33().Tail();
            s1.Func44().Tail();
            s1.Func55();

            ShadowFunctionSystemExample_Source2.Func1();
            ShadowFunctionSystemExample_Source2.Func2();
            ShadowFunctionSystemExample_Source2.Func3().Tail();
            ShadowFunctionSystemExample_Source2.Func4().Tail();
            ShadowFunctionSystemExample_Source2.Func5();
            ShadowFunctionSystemExample_Source2.Func11();
            ShadowFunctionSystemExample_Source2.Func22();
            ShadowFunctionSystemExample_Source2.Func33().Tail();
            ShadowFunctionSystemExample_Source2.Func44().Tail();
            ShadowFunctionSystemExample_Source2.Func55();

            ShadowFunctionSystemExample_Source3Sub s3 = new();
            s3.Func1();
            s3.Func2();
            s3.Func3().Tail();
            s3.Func4().Tail();
            s3.Func5();
            s3.Func11();
            s3.Func22();
            s3.Func33().Tail();
            s3.Func44().Tail();
            s3.Func55();
        }
    }

    [ShadowFunction]
    public partial class ShadowFunctionSystemExample_Source { // 非静态类
        [ShadowFunction]
        public void Func1() {
            this.Func1Shadow(); // 最普通的用法
        }

        [ShadowFunction]
        public async void Func2() {
            await this.Func2Shadow(); // 异步用法
        }

        [ShadowFunction]
        public async HTask Func3() {
            await this.Func3Shadow(); // 等效上面
        }

        [ShadowFunction]
        public async HTask<int> Func4() {
            return await this.Func4Shadow(); // 带返回值的异步, 且AllowMultiShadowFuncs为false, 如果没实现影子函数, 调用会报错
        }

        [ShadowFunction]
        public int Func5() {
            return this.Func5Shadow(); // 带返回值的同步, 且AllowMultiShadowFuncs为false, 如果没实现影子函数, 调用会报错
        }

        [ShadowFunction(allowMultiShadowFuncs: true)]
        public void Func11() {
            this.Func11Shadow(); // AllowMultiShadowFuncs为true的版本
        }

        [ShadowFunction(allowMultiShadowFuncs: true)]
        public async void Func22() {
            await this.Func22Shadow();
        }

        [ShadowFunction(allowMultiShadowFuncs: true)]
        public async HTask Func33() {
            await this.Func33Shadow();
        }

        [ShadowFunction(allowMultiShadowFuncs: true)]
        public async HTask<int> Func44() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            var list = ListComponent<HTask<int>>.Rent();
            this.Func44Shadow(task => { list.Add(task); });

            foreach (var task in list) {
                var i = await task;
                if (i > max)
                    max = i;
            }

            return max;
        }

        [ShadowFunction(allowMultiShadowFuncs: true)]
        public int Func55() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            this.Func55Shadow(i => {
                if (i > max)
                    max = i;
            });

            return max;
        }
    }

    [ShadowFunction(allowMultiShadowFuncs: true)]
    public static partial class ShadowFunctionSystemExample_Source2 { // 静态类
        [ShadowFunction]
        public static void Func1() {
            Func1Shadow(); // 最普通的用法
        }

        [ShadowFunction]
        public static async void Func2() {
            await Func2Shadow(); // 异步用法
        }

        [ShadowFunction]
        public static async HTask Func3() {
            await Func3Shadow(); // 等效上面
        }

        [ShadowFunction]
        public static async HTask<int> Func4() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            var list = ListComponent<HTask<int>>.Rent();
            Func4Shadow(task => { list.Add(task); });

            foreach (var task in list) {
                var i = await task;
                if (i > max)
                    max = i;
            }

            return max;
        }

        [ShadowFunction]
        public static int Func5() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            Func5Shadow(i => {
                if (i > max)
                    max = i;
            });

            return max;
        }

        [ShadowFunction]
        public static void Func11() {
            Func11Shadow(); // AllowMultiShadowFuncs为true的版本
        }

        [ShadowFunction]
        public static async void Func22() {
            await Func22Shadow();
        }

        [ShadowFunction]
        public static async HTask Func33() {
            await Func33Shadow();
        }

        [ShadowFunction]
        public static async HTask<int> Func44() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            var list = ListComponent<HTask<int>>.Rent();
            Func44Shadow(task => { list.Add(task); });

            foreach (var task in list) {
                var i = await task;
                if (i > max)
                    max = i;
            }

            return max;
        }

        [ShadowFunction]
        public static int Func55() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            Func55Shadow(i => {
                if (i > max)
                    max = i;
            });

            return max;
        }
    }

    [ShadowFunction(allowMultiShadowFuncs: true)]
    public partial class ShadowFunctionSystemExample_Source3<T1, T2> { // 静态类
        public int a = 2;
        public string b = "dafad";

        [ShadowFunction]
        public void Func1() {
            Func1Shadow(); // 最普通的用法
        }

        [ShadowFunction]
        public async void Func2() {
            await Func2Shadow(); // 异步用法
        }

        [ShadowFunction]
        public async HTask Func3() {
            await Func3Shadow(); // 等效上面
        }

        [ShadowFunction]
        public async HTask<int> Func4() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            var list = ListComponent<HTask<int>>.Rent();
            Func4Shadow(task => { list.Add(task); });

            foreach (var task in list) {
                var i = await task;
                if (i > max)
                    max = i;
            }

            return max;
        }

        [ShadowFunction]
        public int Func5() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            Func5Shadow(i => {
                if (i > max)
                    max = i;
            });

            return max;
        }

        [ShadowFunction]
        public void Func11() {
            Func11Shadow(); // AllowMultiShadowFuncs为true的版本
        }

        [ShadowFunction]
        public async void Func22() {
            await Func22Shadow();
        }

        [ShadowFunction]
        public async HTask Func33() {
            await Func33Shadow();
        }

        [ShadowFunction]
        public async HTask<int> Func44() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            var list = ListComponent<HTask<int>>.Rent();
            Func44Shadow(task => { list.Add(task); });

            foreach (var task in list) {
                var i = await task;
                if (i > max)
                    max = i;
            }

            return max;
        }

        [ShadowFunction]
        public int Func55() { // AllowMultiShadowFuncs为true的版本, 即便没有实现影子函数, 调用也不会报错
            int max = 0;
            Func55Shadow(i => {
                if (i > max)
                    max = i;
            });

            return max;
        }
    }

    // 泛型类如果想给影子传self的话, 必须实现具体类型, 这是必须的
    public class ShadowFunctionSystemExample_Source3Sub : ShadowFunctionSystemExample_Source3<int, string> { }
}