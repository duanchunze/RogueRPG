using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class LeafNode<T> : Node<T> where T : IBehaviorTree {
        public override IEnumerable<INode> ForeachChildren() {
            yield break;
        }

        public override void StartNode(IBehaviorTree tree) {
            if (this.manager != null) throw new Exception("already has manager");
            if (tree == null) throw new ArgumentNullException("start node failure, tree is null");
            
            this.manager = (T)tree;
            try {
                this.InternalStart();
            }
            catch (Exception e) {
                Log.Error($"<start node error> {e}");
            }
        }

        public override void OpenNode() {
            try {
                this.InternalOpen();
            }
            catch (Exception e) {
                Log.Error($"<open node error> {e}");
            }
        }

        public override void CloseNode() {
            try {
                this.InternalClose();
            }
            catch (Exception e) {
                Log.Error($"<close node error> {e}");
            }
        }

        public override void DestroyNode() {
            if (this.manager != null) {
                try {
                    this.InternalDestroy();
                }
                catch (Exception e) {
                    Log.Error($"<destroy node error> {e}");
                }

                this.manager = default;
            }
        }

        public override void ResetNode() {
            try {
                this.InternalReset();
            }
            catch (Exception e) {
                Log.Error($"<reset node error> {e}");
            }
        }

        public override void AbortNode() {
            this.InternalAbort();
        }
        
        public override TNode GetNodeInChildren<TNode>(bool once = false) {
            return default;
        }
    }
}