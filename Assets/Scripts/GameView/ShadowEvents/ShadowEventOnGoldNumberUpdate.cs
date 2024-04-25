namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public partial class ShadowEventOnGoldNumberUpdate {
        [ShadowFunction]
        private static void OnGoldNumberUpdate(int goldNum) {
            var uiStore = UICardStore.instance;
            if (uiStore == null) return;
            uiStore.goldText.text = goldNum.ToString();
        }
    }
}