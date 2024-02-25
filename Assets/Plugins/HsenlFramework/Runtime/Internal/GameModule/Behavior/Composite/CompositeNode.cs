using System;
using System.Collections.Generic;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#endif

namespace Hsenl {
    // 综合节点, 可以放无数个子节点, 是综合类节点的基类
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class CompositeNode<TManager, TNode> : Node<TManager>, IBreadNode<TNode>
        where TManager : class, IBehaviorTree where TNode : class, INode<TManager> {
        [MemoryPackIgnore]
        protected int position;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackInclude]
        protected List<TNode> children = new();

        public IReadOnlyList<TNode> Children => this.children;

        [MemoryPackOnSerialized]
        private void OnSerialized() {
            if (this.children == null) return;
            foreach (var child in this.children) {
                ((INode)child).Parent = this;
            }
        }

        public override IEnumerable<INode> ForeachChildren() {
            if (this.children == null) {
                yield break;
            }

            foreach (var child in this.children) {
                yield return child;
                foreach (var sub in child.ForeachChildren()) {
                    yield return sub;
                }
            }
        }

        public override void StartNode(IBehaviorTree tree) {
            if (this.manager != null) throw new Exception("already has manager");
            if (tree == null) throw new ArgumentNullException("start node failure, tree is null");

            this.manager = (TManager)tree;
            for (int i = 0, len = this.children.Count; i < len; i++) {
                this.children[i].StartNode(this.manager);
            }

            try {
                this.InternalStart();
            }
            catch (Exception e) {
                Log.Error($"<start node error> {e}");
            }
        }

        public override void OpenNode() {
            for (int i = 0, len = this.children.Count; i < len; i++) {
                this.children[i].OpenNode();
            }

            try {
                this.InternalOpen();
            }
            catch (Exception e) {
                Log.Error($"<open node error> {e}");
            }
        }

        public override void CloseNode() {
            for (int i = 0, len = this.children.Count; i < len; i++) {
                this.children[i].CloseNode();
            }

            try {
                this.InternalClose();
            }
            catch (Exception e) {
                Log.Error($"<close node error> {e}");
            }
        }

        public override void DestroyNode() {
            for (int i = 0, len = this.children.Count; i < len; i++) {
                this.children[i].DestroyNode();
            }

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
            for (int i = 0, len = this.children.Count; i < len; i++) {
                this.children[i].ResetNode();
            }

            try {
                this.InternalReset();
            }
            catch (Exception e) {
                Log.Error($"<reset node error> {e}");
            }
        }

        public override void AbortNode() {
            for (int i = 0, len = this.children.Count; i < len; i++) {
                this.children[i].AbortNode();
            }

            this.position = 0;
            this.InternalAbort();
        }

        public virtual void AddChild(TNode node) {
            if (node == null) return;
            this.children.Add(node);
            node.Parent = this;
            
            if (this.manager != null) {
                node.StartNode(this.manager);
                if (this.manager.RealEnable) {
                    node.OpenNode();
                }
            }
        }

        public virtual void RemoveChild(TNode node) {
            if (node == null) return;
            if (node.Parent != this) return;
            if (this.manager != null) {
                node.AbortNode();
                if (this.manager.RealEnable)
                    node.CloseNode();
                node.DestroyNode();
            }

            this.children.Remove(node);
            node.Parent = null;
        }

        public virtual void Clear() {
            foreach (var child in this.children) {
                if (this.manager != null) {
                    child.AbortNode();
                    if (this.manager.RealEnable)
                        child.CloseNode();
                    child.DestroyNode();
                }

                child.Parent = null;
            }

            this.children.Clear();
        }

        public override T GetNodeInChildren<T>(bool once = false) {
            if (this.children == null) return default;
            for (int i = 0, len = this.children.Count; i < len; i++) {
                var child = this.children[i];
                if (child is T t) {
                    return t;
                }
            }

            if (once) return default;

            for (int i = 0, len = this.children.Count; i < len; i++) {
                var child = this.children[i];
                var ret = child.GetNodeInChildren<T>();
                if (ret != null) {
                    return ret;
                }
            }

            return default;
        }
    }
}