namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public static partial class ShadowEventOnAbilitesBarChanged {
        [ShadowFunction]
        private static void OnAbilitesBarChanged(Hsenl.AbilitesBar abilitesBar) {
            if (!Shortcut.IsMainMan(abilitesBar.MainBodied))
                return;
            
            var bar = UIManager.GetSingleUI<UIAbilitesBar>();
            if (bar == null)
                return;

            bar.FillIn(abilitesBar);
        }
    }
}