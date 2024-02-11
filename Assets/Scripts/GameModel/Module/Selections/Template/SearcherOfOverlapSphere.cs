using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class SearcherOfOverlapSphere : ASelectionsSearcher {
        public float3 position;
        public float radius;
        public int layerMask;

        public override ASelectionsSearcher Search() {
            this.searched.Clear();

            var count = Physics.OverlapSphereNonAlloc(this.position, this.radius, searchCaches, this.layerMask);
            if (count == 0) return this;

            for (int i = 0; i < count; i++) {
                var target = searchCaches[i].GetFrameworkComponent<SelectionTarget>();
                if (target == null) continue;
                if (target.Bodied == this.selector.Bodied) continue;
                this.searched.Add(target);
            }
            
            return this;
        }
    }
}