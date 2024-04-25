using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hsenl {
    public static class AssemblyHelper {
        private static readonly Assembly[] _assemblies;

        public const BindingFlags BindingFlagsInstanceIgnorePublic =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public const BindingFlags BindingFlagsStaticIgnorePublic =
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        public const BindingFlags All = BindingFlagsInstanceIgnorePublic | BindingFlagsStaticIgnorePublic;

        static AssemblyHelper() {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        public static Type GetType(string typeName) {
            foreach (var assembly in _assemblies) {
                var type = assembly.GetType(typeName);
                if (type != null) {
                    return type;
                }
            }

            return null;
        }

        // 获得一个类的所有父类和接口
        public static IEnumerable<Type> GetBaseTypes(Type type, bool includeSelf = false) {
            var first = GetBaseClasses(type, includeSelf).Concat(type.GetInterfaces());
            if (includeSelf && type.IsInterface) {
                return first.Concat(new Type[1] {
                    type
                });
            }

            return first;
        }

        public static IEnumerable<Type> GetBaseClasses(Type type, bool includeSelf = false) {
            if (!(type == null) && !(type.BaseType == null)) {
                if (includeSelf) yield return type;
                for (var current = type.BaseType; current != null; current = current.BaseType) {
                    yield return current;
                }
            }
        }

        // 获得一个类的泛型父类, 如果有的话
        public static Type GetGenericBaseClass(Type type, bool includeSelf = false) {
            Type curr = null;
            if (includeSelf)
                curr = type;
            else
                curr = type.BaseType;

            while (curr != null) {
                if (curr.IsGenericType) return curr;
                curr = curr.BaseType;
            }

            return null;
        }

        public static Type GetBaseInterface(Type type, Type interfaceType) {
            var interfaces = type.GetInterfaces();
            foreach (var iface in interfaces) {
                if (interfaceType.IsGenericType) {
                    if (iface.IsGenericType && iface.GetGenericTypeDefinition() == interfaceType) {
                        return iface;
                    }
                }
                else {
                    if (iface == interfaceType) {
                        return iface;
                    }
                }
            }

            return null;
        }

        public static Type[] FindTypes(string typeName) {
            List<Type> types = new();
            foreach (var ass in _assemblies) {
                var type = ass.GetType(typeName);
                if (type == null) continue;
                types.Add(type);
            }

            return types.ToArray();
        }

        public static Type[] GetAssemblyTypes(params Assembly[] assemblies) {
            List<Type> list = new();

            foreach (var ass in assemblies) {
                foreach (var type in ass.GetTypes()) {
                    list.Add(type);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获得指定基类的所有子类的名称
        /// </summary>
        /// <param name="typeBase"></param>
        /// <param name="assemblies"></param>
        /// <param name="includeAssemblyName"></param>
        /// <returns></returns>
        public static string[] GetSubTypeNames(Type typeBase, Assembly[] assemblies = null, bool includeAssemblyName = false) {
            assemblies ??= _assemblies;
            var typeNames = new List<string>();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    if (type.IsClass && type != typeBase && !type.IsAbstract && typeBase.IsAssignableFrom(type)) {
                        if (includeAssemblyName) {
                            typeNames.Add($"{assembly.FullName}.{type.FullName}");
                        }
                        else {
                            typeNames.Add(type.FullName);
                        }
                    }
                }
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }

        public static string[] GetSubTypeNamesInAssembly(Type typeBase, Assembly assembly) {
            var typeNames = new List<string>();
            foreach (var type in assembly.GetTypes()) {
                if (type.IsClass && type != typeBase && !type.IsAbstract && typeBase.IsAssignableFrom(type)) {
                    typeNames.Add(type.FullName);
                }
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }

        /// <summary>
        /// 获得指定基类的所有子类
        /// </summary>
        /// <param name="typeBase"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static Type[] GetSubTypes(Type typeBase, Assembly[] assemblies = null) {
            assemblies ??= _assemblies;
            var results = new List<Type>();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    if (type.IsClass && type != typeBase && !type.IsAbstract && typeBase.IsAssignableFrom(type)) {
                        results.Add(type);
                    }
                }
            }

            return results.ToArray();
        }

        public static Type[] GetSubTypesInAssembly(Type typeBase, Assembly assembly) {
            var results = new List<Type>();
            foreach (var type in assembly.GetTypes()) {
                if (type.IsClass && type != typeBase && !type.IsAbstract && typeBase.IsAssignableFrom(type)) {
                    results.Add(type);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 找到所有的继承自接口的子类
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static Type[] GetDerivedTypesOfInterface(Type interfaceType) {
            return _assemblies.SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(interfaceType)))
                .ToArray();
        }

        public static bool GetPropertyValue<T>(Type type, string propertyName, object o, out T result) {
            var propertyInfo = type.GetProperty(propertyName, BindingFlagsInstanceIgnorePublic | BindingFlags.GetProperty);
            if (propertyInfo == null) {
                result = default;
                return false;
            }

            if (propertyInfo.GetValue(o) is T t) {
                result = t;
                return true;
            }

            result = default;
            return false;
        }

        public static T GetPropertyValue<T>(Type type, string propertyName, object o) {
            var propertyInfo = type.GetProperty(propertyName, BindingFlagsInstanceIgnorePublic | BindingFlags.GetProperty);
            if (propertyInfo == null) {
                return default;
            }

            if (propertyInfo.GetValue(o) is T t) {
                return t;
            }

            return default;
        }

        public static bool SetPropertyValue(Type type, object o, string propertyName, object value) {
            var propertyInfo = type.GetProperty(propertyName, BindingFlagsInstanceIgnorePublic);
            if (propertyInfo == null) {
                return false;
            }

            propertyInfo.SetValue(o, value);
            return true;
        }


        public static bool GetFieldValue<T>(Type type, object o, string fieldName, out T result) {
            var fieldInfo = type.GetField(fieldName, BindingFlagsInstanceIgnorePublic);
            if (fieldInfo == null) {
                result = default;
                return false;
            }

            if (fieldInfo.GetValue(o) is T t) {
                result = t;
                return true;
            }

            result = default;
            return false;
        }

        public static T GetFieldValue<T>(Type type, object o, string fieldName) {
            var fieldInfo = type.GetField(fieldName, BindingFlagsInstanceIgnorePublic);
            if (fieldInfo == null) {
                return default;
            }

            if (fieldInfo.GetValue(o) is T t) {
                return t;
            }

            return default;
        }

        public static bool SetFieldValue(Type type, object o, string fieldName, object value) {
            var fieldInfo = type.GetField(fieldName, BindingFlagsInstanceIgnorePublic);
            if (fieldInfo == null) {
                return false;
            }

            fieldInfo.SetValue(o, value);
            return true;
        }

        public static FieldInfo GetFieldInBase(this Type self, string fieldName, BindingFlags bindingFlags) {
            var baseType = self;
            FieldInfo result = null;
            while (baseType != null) {
                result = baseType.GetField(fieldName, bindingFlags);
                if (result != null) break;
                baseType = baseType.BaseType;
            }

            return result;
        }

        public static MemberInfo[] GetMembersInBase(this Type self, BindingFlags bindingFlags) {
            if (self.IsInterface) {
                return Array.Empty<MemberInfo>();
            }

            var baseType = self;
            var result = new List<MemberInfo>();
            while (baseType != null) {
                var members = baseType.GetMembers(bindingFlags);
                if (members.Length == 0) continue;
                foreach (var member in members) {
                    if (!result.Contains(member)) result.Add(member);
                }

                baseType = baseType.BaseType;
            }

            return result.ToArray();
        }

        public static void Invoke(this Type self, string methodName, object o1, params object[] o2) {
            var method = self.GetMethod(methodName, All);
            if (method == null) return;
            method.Invoke(o1, o2);
        }
    }
}