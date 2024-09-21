using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class SelectOfNearest : ASelectionsSelect {
        public Vector3 position;

        public override ASelectionsSelect Select(IReadOnlyList<SelectionTarget> sts) {
            this.selected.Clear();
            var nearestDis = float.MaxValue;
            SelectionTarget nearestTarget = null;
            // todo gc ok
            for (int i = 0, len = sts.Count; i < len; i++) {
                var target = sts[i];
                var dis = Vector3.DistanceSquared(this.position, target.transform.Position);
                if (dis >= nearestDis) continue;

                nearestDis = dis;
                nearestTarget = target;
            }

            this.selected.Add(nearestTarget);

            return this;
        }
    }
}