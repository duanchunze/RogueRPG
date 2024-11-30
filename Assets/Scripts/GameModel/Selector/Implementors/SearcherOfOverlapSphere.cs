using System;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class SearcherOfOverlapSphere : ASelectionsSearcher {
        public static readonly UnityEngine.Collider[] searchCaches = new UnityEngine.Collider[100];
        
        public Vector3 position;
        public float radius;
        public int layerMask;

        public override ASelectionsSearcher Search() {
            this.searched.Clear();

            var count = Physics.OverlapSphereNonAlloc(this.position, this.radius, searchCaches, this.layerMask);
            if (count == 0) return this;

            for (int i = 0; i < count; i++) {
                var target = searchCaches[i].GetFrameworkComponent<SelectionTargetDefault>();
                if (target == null) continue;
                if (target.Bodied == this.Selector.Bodied) continue;
                this.searched.Add(target);
            }

            return this;
        }
    }
}