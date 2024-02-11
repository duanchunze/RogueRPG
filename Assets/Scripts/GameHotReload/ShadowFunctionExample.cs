namespace Hsenl {
    // 这里做了一个演示, 把GameModel里的AIInfo类型的OnNodeStart方法放到了GameHotReload里面实现了.
    [ShadowFunction(typeof(AIInfo<>))]
    public static partial class ShadowFunctionExample {
        [ShadowFunctionMandatory]
        private static void OnNodeStart(AIInfo<ai.PatrolInfo> self) {
            if (self.info == null && self.infoInstanceId != 0) {
                var inf = behavior.Info.GetInfo(self.infoInstanceId);
                if (inf != null) {
                    self.InitInfo(inf);
                }
            }
        }
    }
}