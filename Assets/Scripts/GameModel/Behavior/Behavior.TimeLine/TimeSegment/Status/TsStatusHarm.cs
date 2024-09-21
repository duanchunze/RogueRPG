using System.Collections.Generic;
using Hsenl.numeric;

namespace Hsenl {
    public abstract class TsStatusHarm<T> : TsHarm<T> where T : timeline.TsStatusHarmInfo {
        protected override void OnEnable() { }

        protected override void OnReset() {
            switch (this.manager.Bodied) {
                case Status status: {
                    this.harmable = status.inflictor.GetComponent<Harmable>();

                    this.procedureLines ??= new(1);
                    this.procedureLines.Clear();
                    var pl = status.MainBodied?.GetComponent<ProcedureLine>();
                    this.procedureLines.Add(pl);
                    break;
                }
            }
        }
    }
}