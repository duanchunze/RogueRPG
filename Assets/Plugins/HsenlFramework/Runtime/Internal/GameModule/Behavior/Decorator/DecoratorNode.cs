﻿using System;
using System.Collections.Generic;
using MemoryPack;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Hsenl {
    // 只有一个子节点, 放在某个子节点上面, 用于修饰该子节点
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class DecoratorNode<TManager, TNode> : Node<TManager>, IBreadNode<TNode>
        where TManager : IBehaviorTree where TNode : class, INode<TManager> {
        public override NodeType NodeType => NodeType.Decorator;

        [ShowInInspector]
        [MemoryPackInclude]
        protected TNode child;

        [MemoryPackOnSerialized]
        private void OnSerialized() {
            if (this.child == null) return;
            this.child.Parent = this;
        }

        public override IEnumerable<INode> ForeachChildren() {
            if (this.child == null)
                yield break;

            yield return this.child;
        }

        public override void StartNode(IBehaviorTree tree) {
            if (this.manager != null) throw new Exception("already has manager");
            if (tree == null) throw new ArgumentNullException("start node failure, tree is null");

            this.manager = (TManager)tree;
            this.child?.StartNode(this.manager);
            try {
                this.InternalStart();
            }
            catch (Exception e) {
                Log.Error($"<start node error> {e}");
            }
        }

        public override void OpenNode() {
            this.child?.OpenNode();
            try {
                this.InternalOpen();
            }
            catch (Exception e) {
                Log.Error($"<open node error> {e}");
            }
        }

        public override void CloseNode() {
            this.child?.CloseNode();
            try {
                this.InternalClose();
            }
            catch (Exception e) {
                Log.Error($"<close node error> {e}");
            }
        }

        public override void DestroyNode() {
            this.child?.DestroyNode();
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
            this.child?.ResetNode();
            try {
                this.InternalReset();
            }
            catch (Exception e) {
                Log.Error($"<reset node error> {e}");
            }
        }

        public override void AbortNode() {
            this.child?.AbortNode();
            this.InternalAbort();
        }

        public void AddChild(TNode node) {
            if (node == null) return;
            this.child = node;
            node.Parent = this;

            if (this.manager != null) {
                node.StartNode(this.manager);
                if (this.manager.RealEnable) {
                    node.OpenNode();
                }
            }
        }

        public void RemoveChild(TNode node) {
            if (node == null) return;
            if (this.child != node) return;
            if (this.manager != null) {
                node.AbortNode();
                if (this.manager.RealEnable)
                    node.CloseNode();
                node.DestroyNode();
            }

            this.child = null;
            node.Parent = null;
        }

        public void Clear() {
            this.RemoveChild(this.child);
        }

        public override T GetNodeInChildren<T>(bool once = false) {
            if (this.child == null) return default;

            if (this.child is T t) {
                return t;
            }

            if (once) return default;

            return this.child.GetNodeInChildren<T>();
        }
    }
}