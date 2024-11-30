using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class LeafNode<T> : Node<T> where T : IBehaviorTree {
        public sealed override void AwakeNode(IBehaviorTree tree) {
            if (this.manager != null) throw new Exception("already has manager");
            if (tree == null) throw new ArgumentNullException(nameof(tree));

            this.manager = (T)tree;
            try {
                this.InternalAwake();
            }
            catch (Exception e) {
                Log.Error($"<start node error> {e}");
            }
        }

        public sealed override void OpenNode() {
            try {
                this.InternalOpen();
            }
            catch (Exception e) {
                Log.Error($"<open node error> {e}");
            }
        }

        public sealed override void CloseNode() {
            try {
                this.InternalClose();
            }
            catch (Exception e) {
                Log.Error($"<close node error> {e}");
            }
        }

        public sealed override void DestroyNode() {
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

        public sealed override void StartNode() {
            try {
                this.InternalStart();
            }
            catch (Exception e) {
                Log.Error($"<reset node error> {e}");
            }
        }

        public sealed override void AbortNode() {
            this.InternalAbort();
        }

        public sealed override Iterator<INode> ForeachChildren() {
            return default;
        }

        public sealed override TNode GetNodeInChildren<TNode>() {
            return default;
        }

        public sealed override TNode[] GetNodesInChildren<TNode>() {
            return default;
        }

        public sealed override void GetNodesInChildren<TNode>(List<TNode> cache) { }
    }
}