using Hsenl.EventType;

namespace Hsenl {
    public class EventRefreshStoreCards : AEventSync<RefreshStoreCards> {
        protected override void Handle(RefreshStoreCards arg) {
            GameManager.Instance.ProcedureLine.StartLine(new PliRefreshStoreCardsForm());
        }
    }
}