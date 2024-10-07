using UnityEngine;

namespace Hsenl.View {
    [ProcedureLineHandlerPriority(PliStatusChangesPriority.Shentibianhong)]
    public class PlhStatusChangesBodyUndergoChanges : AProcedureLineHandler<PliStatusChangesForm> {
        private static readonly Color _BaTiColor = new Color(1, 101 / 255f, 0, 1);

        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliStatusChangesForm item, object userToken) {
            switch (item.statusAlias) {
                case StatusAlias.BaTi: {
                    var model = item.target.GetComponent<Model>();
                    if (item.changeType == 0) {
                        // model.SetModelOutlineColor(Color.black);
                    }
                    else {
                        // model.SetModelOutlineColor(Color.white);
                    }

                    break;
                }

                case StatusAlias.HitStun: {
                    var model = item.target.GetComponent<Model>();
                    if (item.changeType == 0) {
                        // model.SetModelColor(Color.red);
                    }
                    else {
                        // model.SetModelColor(Color.white);
                    }

                    break;
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}