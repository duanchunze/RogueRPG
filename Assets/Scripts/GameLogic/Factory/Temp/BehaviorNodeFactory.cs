using System;
using System.Collections.Generic;
using Hsenl.behavior;

namespace Hsenl {
    public static class BehaviorNodeFactory {
        public static T CreateNode<T>(behavior.Info info) where T : INode {
            var nodeType = BehaviorNodeManager.GetTypeOfInfoType(info.GetType());
            if (nodeType == null) throw new Exception($"cant find behavior info of '{info.GetType()}'");
            var node = (T)Activator.CreateInstance(nodeType);
            ((IBehaviorNodeInitializer)node).Init(info);
            return node;
        }

        public static INode<T> CreateNodeLink<T>(IList<behavior.Info> infos) where T : class, IBehaviorTree {
            using var breadNodeCaches = DictionaryComponent<int, IBreadNode<INode<T>>>.Create();
            INode<T> entryNode = default;

            for (int i = 0, len = infos.Count; i < len; i++) {
                var info = infos[i];
                switch (info) {
                    case SelectorNodeInfo selectorNodeInfo: {
                        var selectorNode = new SelectorNode<T, INode<T>>();
                        breadNodeCaches.Add(selectorNodeInfo.Index, selectorNode);
                        entryNode ??= selectorNode;
                        if (selectorNodeInfo.ParentIndex >= 0) {
                            breadNodeCaches[selectorNodeInfo.ParentIndex].AddChild(selectorNode);
                        }

                        break;
                    }

                    case SequentialNodeInfo sequentialNodeInfo: {
                        var sequentialNode = new SequentialNode<T, INode<T>>();
                        breadNodeCaches.Add(sequentialNodeInfo.Index, sequentialNode);
                        entryNode ??= sequentialNode;
                        if (sequentialNodeInfo.ParentIndex >= 0) {
                            breadNodeCaches[sequentialNodeInfo.ParentIndex].AddChild(sequentialNode);
                        }

                        break;
                    }

                    case ParalleNodeInfo paralleNodeInfo: {
                        var parallelNode = new ParallelNode<T, INode<T>>();
                        breadNodeCaches.Add(paralleNodeInfo.Index, parallelNode);
                        entryNode ??= parallelNode;
                        if (paralleNodeInfo.ParentIndex >= 0) {
                            breadNodeCaches[paralleNodeInfo.ParentIndex].AddChild(parallelNode);
                        }

                        break;
                    }

                    case ParalleSelectorNodeInfo paralleSelectorNodeInfo: {
                        var paralleSelectorNode = new ParalleSelectorNode<T, INode<T>>();
                        breadNodeCaches.Add(paralleSelectorNodeInfo.Index, paralleSelectorNode);
                        entryNode ??= paralleSelectorNode;
                        if (paralleSelectorNodeInfo.ParentIndex >= 0) {
                            breadNodeCaches[paralleSelectorNodeInfo.ParentIndex].AddChild(paralleSelectorNode);
                        }

                        break;
                    }

                    case ParalleSequentialNodeInfo paralleSequentialNodeInfo: {
                        var paralleSequentialNode = new ParalleSequentialNode<T, INode<T>>();
                        breadNodeCaches.Add(paralleSequentialNodeInfo.Index, paralleSequentialNode);
                        entryNode ??= paralleSequentialNode;
                        if (paralleSequentialNodeInfo.ParentIndex >= 0) {
                            breadNodeCaches[paralleSequentialNodeInfo.ParentIndex].AddChild(paralleSequentialNode);
                        }

                        break;
                    }

                    case AINodeInfo aiNodeInfo: {
                        var aiNode = new AICompositeNode<T>();
                        breadNodeCaches.Add(aiNodeInfo.Index, aiNode);
                        entryNode ??= aiNode;
                        if (aiNodeInfo.ParentIndex >= 0) {
                            breadNodeCaches[aiNodeInfo.ParentIndex].AddChild(aiNode);
                        }

                        break;
                    }

                    case LeafNodeInfo leafNodeInfo: {
                        var node = BehaviorNodeFactory.CreateNode<Node<T>>(leafNodeInfo);
                        entryNode ??= node;
                        if (leafNodeInfo.ParentIndex >= 0) {
                            breadNodeCaches[leafNodeInfo.ParentIndex].AddChild(node);
                        }

                        break;
                    }

                    default: {
                        throw new ArgumentOutOfRangeException(info.GetType().Name);
                    }
                }
            }

            return entryNode;
        }
    }
}