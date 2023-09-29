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
            // 写法三要素
        
            // 1、先判断是否过时, 不过时就直接退出就好了
            if (!this.IsObsolete) {
                return this;
            }

            // 2、清空之前的数据
            this.searched.Clear();

            var count = Physics.OverlapSphereNonAlloc(this.position, this.radius, searchCaches, this.layerMask);
            if (count == 0) return this;

            for (int i = 0; i < count; i++) {
                var target = searchCaches[i].GetFrameworkComponentInParent<SelectionTarget>();
                if (target == null) continue;
                if (target.Substantive == this.selector.Substantive) continue;
                this.searched.Add(target);
            }

            // 3、重新计算过时帧
            this.RecalculateObsoleteFrame();
            return this;
        }
    }
}