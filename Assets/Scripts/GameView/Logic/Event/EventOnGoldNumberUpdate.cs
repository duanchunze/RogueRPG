using Hsenl.EventType;

namespace Hsenl.View {
    public class EventOnGoldNumberUpdate : AEventSync<OnGoldNumberUpdate> {
        protected override void Handle(OnGoldNumberUpdate arg) {
            var uiStore = UICardStore.instance;
            if (uiStore == null) return;
            uiStore.goldText.text = arg.goldNum.ToString();
        }
    }
}