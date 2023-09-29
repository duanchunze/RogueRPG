using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Hsenl {
    [Serializable]
    public class SelectorOfNearest : ASelectionsSelector {
        public float3 position;

        public override ASelectionsSelector Select(IReadOnlyList<SelectionTarget> ins) {
            if (!this.IsObsolete) {
                return this;
            }

            var nearestDis = float.MaxValue;
            SelectionTarget nearestTarget = null;
            // todo gc ok
            for (int i = 0, len = ins.Count; i < len; i++) {
                var target = ins[i];
                var dis = math.distancesq(this.position, target.transform.Position);
                if (dis >= nearestDis) continue;

                nearestDis = dis;
                nearestTarget = target;
            }

            this.selected = nearestTarget;

            this.RecalculateObsoleteFrame();

            return this;
        }
    }
}