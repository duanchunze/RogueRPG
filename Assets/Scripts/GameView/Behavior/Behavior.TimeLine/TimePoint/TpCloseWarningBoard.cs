using System.Collections.Generic;
using MemoryPack;

namespace Hsenl.View {
    [MemoryPackable()]
    public partial class TpCloseWarningBoard : TpInfo<timeline.CloseWarningBoardInfo> {
        protected override void OnTimePointTrigger() {
            var list = this.manager.Blackboard.GetOrCreateData<List<Entity>>("AbilityOpenWarnBoards");
            for (int i = list.Count - 1; i >= 0; i--) {
                var entity = list[i];
                WarningBoardManager.Instance.Return(entity);
                list.RemoveAt(i);
            }
        }
    }
}