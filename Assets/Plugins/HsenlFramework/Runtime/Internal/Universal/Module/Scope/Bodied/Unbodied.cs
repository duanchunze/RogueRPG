using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    // 一个无形体, 比如行走能力组件, 数值组件, 刚体组件. 无形体是作为一种能力赋予给有形体.
    // 每个无形体都必须有自己的IUnbodiedHead
    [RequireComponent(requireType: typeof(Scope), addType: typeof(UnbodiedHead))]
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Unbodied : Element {
        [MemoryPackIgnore]
        internal IUnbodiedHead unbodiedHead;

        [MemoryPackIgnore]
        public Bodied Bodied {
            get {
                if (this.IsDisposed) {
                    throw new NullReferenceException("The unbodied has been destroyed, but you're still trying to get it");
                }

                return this.unbodiedHead?.Bodied;
            }
        }

        [MemoryPackIgnore]
        public Bodied AttachedBodied => this.Bodied?.AttachedBodied;
        
        protected internal override void OnDestroyFinish() {
            base.OnDestroyFinish();
            this.unbodiedHead = null;
        }
    }
}