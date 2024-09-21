// ReSharper disable RedundantNameQualifier
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedTypeParameter

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
namespace Hsenl.View {
    [ShadowFunction(typeof(ShadowFunctionSystemExample_Source))]
    public partial class ShadowFunctionSystemExample_Shadow {
        [ShadowFunction]
        private void Func1() {
            Log.Debug($"{this.GetType()} func1");
        }

        [ShadowFunction]
        private async Hsenl.HTask Func2() {
            Log.Debug($"{this.GetType()} func2");
        }

        [ShadowFunction]
        private async HTask Func3() {
            Log.Debug($"{this.GetType()} func3");
        }

        [ShadowFunction]
        private async HTask<int> Func4() {
            Log.Debug($"{this.GetType()} func4");
            return 1;
        }

        [ShadowFunction]
        private int Func5() {
            Log.Debug($"{this.GetType()} func5");
            return 1;
        }

        [ShadowFunction]
        private void Func11() {
            Log.Debug($"{this.GetType()} func11");
        }

        [ShadowFunction]
        private async Hsenl.HTask Func22() {
            Log.Debug($"{this.GetType()} func22");
        }

        [ShadowFunction]
        private async HTask Func33() {
            Log.Debug($"{this.GetType()} func33");
        }

        [ShadowFunction]
        private async HTask<int> Func44() {
            Log.Debug($"{this.GetType()} func44");
            return 1;
        }

        [ShadowFunction]
        private int Func55() {
            Log.Debug($"{this.GetType()} func55");
            return 1;
        }
    }

    [ShadowFunction(typeof(ShadowFunctionSystemExample_Source2))]
    public static partial class ShadowFunctionSystemExample_Shadow2 {
        [ShadowFunction]
        private static void Func1() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func1");
        }

        [ShadowFunction]
        private static async Hsenl.HTask Func2() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func2");
        }

        [ShadowFunction]
        private static async HTask Func3() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func3");
        }

        [ShadowFunction]
        private static async HTask<int> Func4() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func4");
            return 1;
        }

        [ShadowFunction]
        private static int Func5() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func5");
            return 1;
        }

        [ShadowFunction]
        private static void Func11() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func11");
        }

        [ShadowFunction]
        private static async Hsenl.HTask Func22() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func22");
        }

        [ShadowFunction]
        private static async HTask Func33() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func33");
        }

        [ShadowFunction]
        private static async HTask<int> Func44() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func44");
            return 1;
        }

        [ShadowFunction]
        private static int Func55() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow2)} func55");
            return 1;
        }
    }

    [ShadowFunction(typeof(ShadowFunctionSystemExample_Source3<,>))]
    public static partial class ShadowFunctionSystemExample_Shadow3 {
        [ShadowFunction]
        private static void Func1(ShadowFunctionSystemExample_Source3<int, string> self) { // 传入泛型的self
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func1 {self.a} {self.b}");
        }

        [ShadowFunction]
        private static async Hsenl.HTask Func2() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func2");
        }

        [ShadowFunction]
        private static async HTask Func3() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func3 priority: 0");
        }

        [ShadowFunction(priority: -1)]
        private static async HTask Func3(ShadowFunctionSystemExample_Source3<int, string> self) { // 给源函数Func3实现了2个影子函数, 并设置优先级, 让该函数优先于上面的重载函数执行
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func3 priority: -1 {self.a} {self.b}");
        }

        [ShadowFunction]
        private static async HTask<int> Func4() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func4");
            return 1;
        }

        [ShadowFunction]
        private static int Func5() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func5");
            return 1;
        }

        [ShadowFunction]
        private static void Func11() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func11");
        }

        [ShadowFunction]
        private static async Hsenl.HTask Func22() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func22");
        }

        [ShadowFunction]
        private static async HTask Func33() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func33");
        }

        [ShadowFunction]
        private static async HTask<int> Func44() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func44");
            return 1;
        }

        [ShadowFunction]
        private static int Func55() {
            Log.Debug($"{typeof(ShadowFunctionSystemExample_Shadow3)} func55");
            return 1;
        }
    }
}