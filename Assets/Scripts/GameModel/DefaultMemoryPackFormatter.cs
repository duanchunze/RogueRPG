using System;
using System.Collections.Generic;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;

namespace Hsenl {
    public class DefaultMemoryPackFormatter : MemoryPackFormatter {
        protected override void Register() {
            RegisterUnion<INode<Caster>>(
                typeof(SelectorNode<Caster, INode<Caster>>),
                typeof(SequentialNode<Caster, INode<Caster>>),
                typeof(ParallelNode<Caster, INode<Caster>>),
                typeof(ParalleSelectorNode<Caster, INode<Caster>>),
                typeof(ParalleSequentialNode<Caster, INode<Caster>>)
            );
            
            RegisterUnion<Node<Caster>>(
                typeof(SelectorNode<Caster, INode<Caster>>),
                typeof(SequentialNode<Caster, INode<Caster>>),
                typeof(ParallelNode<Caster, INode<Caster>>),
                typeof(ParalleSelectorNode<Caster, INode<Caster>>),
                typeof(ParalleSequentialNode<Caster, INode<Caster>>)
            );

            RegisterUnion<Collider>();
        }
    }
}