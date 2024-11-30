using System;
using System.Collections.Generic;
using Hsenl.casterevaluate;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 尝试选择目标, 如果找不到目标, 则返回失败. 相反, 则返回成功
    [Serializable]
    [MemoryPackable]
    public partial class CePickTarget : CePickTargetBase<PickTargetInfo> {
        protected override ASelectionsSelect SelectTarget(SelectorDefault selector, float range, IReadOnlyBitlist constrainsTags, IReadOnlyBitlist exclusiveTags,
            int count) {
            return GameAlgorithm.SelectTargets(selector, range, constrainsTags, exclusiveTags, count);
        }
    }
}