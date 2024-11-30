namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public static partial class ShadowEventOnPropBarChanged {
        [ShadowFunction]
        private static void OnPropBarChanged(Hsenl.PropBar propBar) {
            if (!Shortcut.IsMainMan(propBar.MainBodied))
                return;

            var bar = UIManager.GetSingleUI<UIPropBar>();
            if (bar == null)
                return;

            bar.FillIn(propBar);
        }
    }
}