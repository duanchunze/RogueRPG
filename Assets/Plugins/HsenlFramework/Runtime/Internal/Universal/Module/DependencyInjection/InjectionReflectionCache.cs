using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Hsenl {
    // 第一次反射的时候, 把对应的info缓存下来, 下次再用的时候, 就不用重新获取了
    // 不过问映射关系, 只关注反射相关
    public class InjectionReflectionCache {
        private static InjectionReflectionCache ReflectionCache { get; } = new();

        private ConcurrentDictionary<Type, ConcurrentQueue<InjectionReflectionInfo>> _reflectionInfos = new();

        public static ConcurrentQueue<InjectionReflectionInfo> GetInjectionReflectionInfos(Type type) {
            if (!ReflectionCache._reflectionInfos.TryGetValue(type, out var list)) {
                list = new();

                var bindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                foreach (var fieldInfo in type.GetFields(bindingFlag)) {
                    list.Enqueue(new InjectionReflectionInfo(fieldInfo, fieldInfo.FieldType, fieldInfo.Name));
                }

                foreach (var propertyInfo in type.GetProperties(bindingFlag)) {
                    list.Enqueue(new InjectionReflectionInfo(propertyInfo, propertyInfo.PropertyType, propertyInfo.Name));
                }

                ReflectionCache._reflectionInfos.TryAdd(type, list);
            }

            return list;
        }
    }

    public struct InjectionReflectionInfo {
        private object _source;
        public Type TargetType { get; }
        public string Name { get; }

        public InjectionReflectionInfo(object source, Type targetType, in string name) {
            this._source = source;
            this.TargetType = targetType;
            this.Name = name;
        }

        public void SetValue(object obj, object value) {
            if (this._source is FieldInfo fieldInfo)
                fieldInfo.SetValue(obj, value);
            else if (this._source is PropertyInfo propertyInfo)
                propertyInfo.SetValue(obj, value);
        }
    }
}