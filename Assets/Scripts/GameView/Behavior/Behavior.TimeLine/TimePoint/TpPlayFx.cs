using System;
using MemoryPack;

namespace Hsenl.View {
    [Serializable]
    [MemoryPackable]
    public partial class TpPlayFx : TpInfo<timeline.PlayFxInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    foreach (var selectionTarget in ability.targets) {
                        FxManager.Instance.Play(this.info.FxName, selectionTarget.transform.Position);
                    }

                    break;
                }
            }
        }
    }
}