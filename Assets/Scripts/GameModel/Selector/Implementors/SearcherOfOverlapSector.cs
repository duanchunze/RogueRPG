using System;
using UnityEngine;

namespace Hsenl {
    // 搜索扇形区域
    [Serializable]
    public class SearcherOfOverlapSector : ASelectionsSearcher {
        public static readonly UnityEngine.Collider[] searchCaches = new UnityEngine.Collider[100];

        public Vector3 position;
        public float radius;
        public float angle;
        public Vector3 dir;
        public Vector3 axis;
        public int layerMask;

        public override ASelectionsSearcher Search() {
            this.searched.Clear();

            var count = Physics.OverlapSphereNonAlloc(this.position, this.radius, searchCaches, this.layerMask);
            if (count == 0) return this;

            for (int i = 0; i < count; i++) {
                var target = searchCaches[i].GetFrameworkComponent<SelectionTarget>();
                if (target == null) continue;
                if (target.Bodied == this.selector.Bodied) continue;
                var targetDir = target.transform.Position - this.position;
                var targetAngle = Vector3.Angle(targetDir, this.dir);
                if (targetAngle <= this.angle / 2) {
                    this.searched.Add(target);
                }
            }

            return this;
        }
    }
}