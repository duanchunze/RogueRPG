using System;
using System.Collections.Generic;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;

namespace Hsenl {
    public class DefaultMemoryPackFormatter : MemoryPackFormatter {
        protected override void Register() {
            RegisterUnion<INode<CasterEvaluate>>(
                typeof(SelectorNode<CasterEvaluate, INode<CasterEvaluate>>),
                typeof(SequentialNode<CasterEvaluate, INode<CasterEvaluate>>),
                typeof(ParallelNode<CasterEvaluate, INode<CasterEvaluate>>),
                typeof(ParalleSelectorNode<CasterEvaluate, INode<CasterEvaluate>>),
                typeof(ParalleSequentialNode<CasterEvaluate, INode<CasterEvaluate>>)
            );
            
            RegisterUnion<Node<CasterEvaluate>>(
                typeof(SelectorNode<CasterEvaluate, INode<CasterEvaluate>>),
                typeof(SequentialNode<CasterEvaluate, INode<CasterEvaluate>>),
                typeof(ParallelNode<CasterEvaluate, INode<CasterEvaluate>>),
                typeof(ParalleSelectorNode<CasterEvaluate, INode<CasterEvaluate>>),
                typeof(ParalleSequentialNode<CasterEvaluate, INode<CasterEvaluate>>)
            );

            RegisterUnion<Collider>();
        }
    }
}