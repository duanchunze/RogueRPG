using System;
using System.Collections.Generic;

namespace Hsenl {
    public class TypeRepository<T> where T : TypeAttribute {
        public static readonly TypeRepository<T> instance = new();
        private readonly Dictionary<string, Type> _typeDict = new();

        private TypeRepository() {
            var types = EventSystem.GetTypesOfAttribute(typeof(T));
            foreach (var type in types) {
                var attrs = type.GetCustomAttributes(typeof(T), true);
                if (attrs.Length == 0) {
                    continue;
                }

                if (attrs[0] is not T attribute) {
                    continue;
                }

                // 如果没指定类型名，那就使用类型的默认名
                var typeName = string.IsNullOrEmpty(attribute.TypeName) ? type.Name : attribute.TypeName;
                this._typeDict.Add(typeName, type);
            }
        }

        public Type GetTypeByString(string typeName) {
            this._typeDict.TryGetValue(typeName, out var result);
            return result;
        }
    }
}