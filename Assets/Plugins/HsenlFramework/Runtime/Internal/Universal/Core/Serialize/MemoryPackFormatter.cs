using System;
using System.Collections.Generic;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

namespace Hsenl {
    // 这是另一种方式, 也可以注册, 但不优雅
    // [MemoryPackUnionFormatter(typeof(Component))]
    // [MemoryPackUnion(0, typeof(Substantive))]
    // [MemoryPackUnion(1, typeof(Unbodied))]
    // [MemoryPackUnion(2, typeof(Transform))]
    // public partial class ComponentFormatter {
    //     static ComponentFormatter() {
    //         ComponentFormatterInitializer.RegisterFormatter();
    //     }
    // }
    
    [FrameworkMember]
    public abstract class MemoryPackFormatter {
        [OnEventSystemInitialized]
        private static void RegisterFormatter() {
            RegisterUnion<Object>();
            RegisterUnion<Component>();
            RegisterUnion<Substantive>();
            RegisterUnion<Unbodied>();

            RegisterUnion<IBlackboard>();
            RegisterUnion<INode>();
            RegisterUnion<INode<BehaviorTree>>();
            RegisterUnion<Node<BehaviorTree>>();

            RegisterUnion<IStageNode>();
            RegisterUnion<ITimeLine>();
            RegisterUnion<ITimeNode>();
            RegisterUnion<IProcedureLineWorker>();
            RegisterUnion<IRecord>();
            
            foreach (var type in AssemblyHelper.GetSubTypes(typeof(MemoryPackFormatter), EventSystem.GetAssemblies())) {
                var formatter = (MemoryPackFormatter)Activator.CreateInstance(type);
                formatter.Register();
            }
        }

        protected abstract void Register();
        
        protected static void RegisterUnion<T>(params Type[] types) where T : class {
            var subTypes = AssemblyHelper.GetSubTypes(typeof(T), EventSystem.GetAssemblies());
            var unions = new List<(ushort tag, Type type)>();
            ushort tag = 0;
            for (ushort i = 0; i < subTypes.Length; i++) {
                var subType = subTypes[i];
                var attr = subType.GetCustomAttribute<MemoryPackableAttribute>();
                if (attr == null) continue;
                unions.Add(new(tag++, subType));
            }

            if (types != null) {
                foreach (var type in types) {
                    unions.Add(new(tag++, type));
                }
            }

            MemoryPackFormatterProvider.Register(new DynamicUnionFormatter<T>(unions.ToArray()));
        }
    }
}