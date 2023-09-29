using UnityEngine;

namespace Hsenl.View {
    [ProcedureLineHandlerPriority(PliStatusChangesPriority.Shentibianhong)]
    public class PlhStatusChangesBodyUndergoChanges : AProcedureLineHandler<PliStatusChangesForm> {
        private static readonly Color _BaTiColor = new Color(1, 101 / 255f, 0, 1);

        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliStatusChangesForm item) {
            switch (item.statusAlias) {
                case StatusAlias.BaTi: {
                    if (item.changeType == 0) {
                        var apper = item.target.GetComponent<Appearance>();
                        apper.SetModelOutlineColor(Color.black);
                    }
                    else {
                        var apper = item.target.GetComponent<Appearance>();
                        apper.SetModelOutlineColor(Color.white);
                    }

                    break;
                }

                case StatusAlias.HitStun: {
                    if (item.changeType == 0) {
                        var apper = item.target.GetComponent<Appearance>();
                        apper.SetModelColor(Color.red);
                    }
                    else {
                        var apper = item.target.GetComponent<Appearance>();
                        apper.SetModelColor(Color.white);
                    }

                    break;
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}