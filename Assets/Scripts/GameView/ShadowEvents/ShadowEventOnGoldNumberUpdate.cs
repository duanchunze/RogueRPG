namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public partial class ShadowEventOnGoldNumberUpdate {
        [ShadowFunction]
        private static void OnGoldNumberUpdate(int goldNum) {
            // var uiStore = UIManager.GetSingleUI<UICardStore>();
            // if (uiStore == null) return;
            // uiStore.goldText.text = goldNum.ToString();
        }
    }
}