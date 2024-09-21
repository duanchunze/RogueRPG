namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public static partial class ShadowEventOnStatusBarChanged {
        [ShadowFunction]
        private static void OnStatusBarChanged(Hsenl.StatusBar statusBar) {
            var headInfo = statusBar.MainBodied.GetComponent<HeadInfo>();
            if (headInfo != null) {
                headInfo.FillInStatusBar(statusBar);
            }
        }
    }
}