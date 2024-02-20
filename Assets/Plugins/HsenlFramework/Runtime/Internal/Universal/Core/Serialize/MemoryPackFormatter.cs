using System;
using System.Collections.Generic;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;

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
            
            foreach (var type in AssemblyHelper.GetSubTypes(typeof(MemoryPackFormatter), EventSystem.GetAssemblies())) {
                var formatter = (MemoryPackFormatter)Activator.CreateInstance(type);
                formatter.Register();
            }
        }

        protected abstract void Register();
        
        // 在override里, 调用RegisterUnion()以注册formatter
        // 会自动找到T的所有子类进行注册, 但如果是未明确定义的, 但运行过程中才会用到的泛型, 可以通过types参数, 来指定添加
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