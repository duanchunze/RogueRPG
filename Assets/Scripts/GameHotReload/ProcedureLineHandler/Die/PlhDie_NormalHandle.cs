﻿namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDiePriority.NoramlHandle)]
    public class PlhDie_NormalHandle : AProcedureLineHandler<PliDieForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDieForm item, object userToken) {
            // 死亡的人物把agent关掉, 不然会挡住别人寻路
            var agent = item.target.transform.NavMeshAgent;
            if (agent != null) {
                agent.Enable = false;
            }
            
            // 清除所有的被选中
            var selectionTarget = item.target.GetComponent<SelectionTargetDefault>();
            selectionTarget?.ClearAllSelectors();

            return ProcedureLineHandleResult.Success;
        }
    }
}