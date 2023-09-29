using System;
using System.Collections;
using System.Reflection;

namespace Hsenl {
    public static class ObjectHelper {
        public static void Swap<T>(ref T t1, ref T t2) {
            (t1, t2) = (t2, t1);
        }

        // 克隆对象
        public static object CloneObject(object src) {
            var type = src.GetType();
            var target = Activator.CreateInstance(type);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields) {
                var fieldType = field.FieldType;
                if (fieldType.IsPrimitive || fieldType.IsEnum || fieldType == typeof(string)) {
                    field.SetValue(target, field.GetValue(src));
                }
                else if (typeof(IEnumerable).IsAssignableFrom(fieldType)) {
                    if (fieldType.IsGenericType) {
                        var cloneObj = Activator.CreateInstance(fieldType);

                        var addMethod = fieldType.GetMethod("Add", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (addMethod == null) throw new NullReferenceException("addMethod == null");

                        if (field.GetValue(src) is not IEnumerable currentValues) {
                            field.SetValue(target, null);
                        }
                        else {
                            if (cloneObj is IDictionary dict) {
                                cloneObj = dict;
                                foreach (var currentValue in currentValues) {
                                    var propInfoKey = currentValue.GetType().GetProperty("Key");
                                    var propInfoValue = currentValue.GetType().GetProperty("Value");
                                    if (propInfoKey != null && propInfoValue != null) {
                                        var keyValue = propInfoKey.GetValue(currentValue, null);
                                        var valueValue = propInfoValue.GetValue(currentValue, null);

                                        object finalKeyValue, finalValueValue;

                                        var currentKeyType = keyValue.GetType();
                                        if (currentKeyType.IsPrimitive || currentKeyType.IsEnum || currentKeyType == typeof(string)) {
                                            finalKeyValue = keyValue;
                                        }
                                        else {
                                            var copyObj = CloneObject(keyValue);
                                            finalKeyValue = copyObj;
                                        }

                                        var currentValueType = valueValue.GetType();
                                        if (currentValueType.IsPrimitive || currentValueType.IsEnum || currentValueType == typeof(string)) {
                                            finalValueValue = valueValue;
                                        }
                                        else {
                                            var copyObj = CloneObject(valueValue);
                                            finalValueValue = copyObj;
                                        }

                                        addMethod.Invoke(cloneObj, new[] { finalKeyValue, finalValueValue });
                                    }
                                }

                                field.SetValue(target, cloneObj);
                            }
                            else {
                                foreach (var currentValue in currentValues) {
                                    var currentType = currentValue.GetType();
                                    if (currentType.IsPrimitive || currentType.IsEnum || currentType == typeof(string)) {
                                        addMethod.Invoke(cloneObj, new[] { currentValue });
                                    }
                                    else {
                                        var copyObj = CloneObject(currentValue);
                                        addMethod.Invoke(cloneObj, new[] { copyObj });
                                    }
                                }

                                field.SetValue(target, cloneObj);
                            }
                        }
                    }
                    else {
                        if (field.GetValue(src) is not Array currentValues) {
                            field.SetValue(target, null);
                        }
                        else {
                            var cloneObj = Activator.CreateInstance(fieldType, currentValues.Length) as Array;
                            var arrayList = new ArrayList();
                            for (var i = 0; i < currentValues.Length; i++) {
                                var currentType = currentValues.GetValue(i).GetType();
                                if (currentType.IsPrimitive || currentType.IsEnum || currentType == typeof(string)) {
                                    arrayList.Add(currentValues.GetValue(i));
                                }
                                else {
                                    var copyObj = CloneObject(currentValues.GetValue(i));
                                    arrayList.Add(copyObj);
                                }
                            }

                            arrayList.CopyTo(cloneObj, 0);
                            field.SetValue(target, cloneObj);
                        }
                    }
                }
                else {
                    var fieldValue = field.GetValue(src);
                    if (fieldValue == null) {
                        field.SetValue(target, null);
                    }
                    else if (fieldValue.GetType().IsPrimitive || fieldValue.GetType().IsEnum || fieldValue is string) {
                        field.SetValue(target, fieldValue);
                    }
                    else {
                        field.SetValue(target, CloneObject(fieldValue));
                    }
                }
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in properties) {
                if (!property.CanWrite) continue;
                if (property.PropertyType.IsPrimitive || property.PropertyType.IsEnum || property.PropertyType == typeof(string)) {
                    property.SetValue(target, property.GetValue(src, null), null);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) {
                    if (property.PropertyType.IsGenericType) {
                        var cloneObj = Activator.CreateInstance(property.PropertyType);

                        var addMethod = property.PropertyType.GetMethod("Add", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (addMethod == null) throw new NullReferenceException("addMethod == null");

                        if (property.GetValue(src, null) is not IEnumerable currentValues) {
                            property.SetValue(target, null, null);
                        }
                        else {
                            if (cloneObj is IDictionary dict) {
                                cloneObj = dict;
                                foreach (var currentValue in currentValues) {
                                    var propInfoKey = currentValue.GetType().GetProperty("Key");
                                    var propInfoValue = currentValue.GetType().GetProperty("Value");
                                    if (propInfoKey != null && propInfoValue != null) {
                                        var keyValue = propInfoKey.GetValue(currentValue, null);
                                        var valueValue = propInfoValue.GetValue(currentValue, null);

                                        object finalKeyValue, finalValueValue;

                                        var currentKeyType = keyValue.GetType();
                                        if (currentKeyType.IsPrimitive || currentKeyType.IsEnum || currentKeyType == typeof(string)) {
                                            finalKeyValue = keyValue;
                                        }
                                        else {
                                            var copyObj = CloneObject(keyValue);
                                            finalKeyValue = copyObj;
                                        }

                                        var currentValueType = valueValue.GetType();
                                        if (currentValueType.IsPrimitive || currentValueType.IsEnum || currentValueType == typeof(string)) {
                                            finalValueValue = valueValue;
                                        }
                                        else {
                                            var copyObj = CloneObject(valueValue);
                                            finalValueValue = copyObj;
                                        }

                                        addMethod.Invoke(cloneObj, new[] { finalKeyValue, finalValueValue });
                                    }
                                }

                                property.SetValue(target, cloneObj, null);
                            }
                            else {
                                foreach (var currentValue in currentValues) {
                                    var currentType = currentValue.GetType();
                                    if (currentType.IsPrimitive || currentType.IsEnum || currentType == typeof(string)) {
                                        addMethod.Invoke(cloneObj, new[] { currentValue });
                                    }
                                    else {
                                        var copyObj = CloneObject(currentValue);
                                        addMethod.Invoke(cloneObj, new[] { copyObj });
                                    }
                                }

                                property.SetValue(target, cloneObj, null);
                            }
                        }
                    }
                    else {
                        if (property.GetValue(src, null) is not Array currentValues) {
                            property.SetValue(target, null, null);
                        }
                        else {
                            var cloneObj = Activator.CreateInstance(property.PropertyType, currentValues.Length) as Array;
                            var arrayList = new ArrayList();
                            for (var i = 0; i < currentValues.Length; i++) {
                                var currentType = currentValues.GetValue(i).GetType();
                                if (currentType.IsPrimitive || currentType.IsEnum || currentType == typeof(string)) {
                                    arrayList.Add(currentValues.GetValue(i));
                                }
                                else {
                                    var copyObj = CloneObject(currentValues.GetValue(i));
                                    arrayList.Add(copyObj);
                                }
                            }

                            arrayList.CopyTo(cloneObj, 0);
                            property.SetValue(target, cloneObj, null);
                        }
                    }
                }
                else {
                    var propertyValue = property.GetValue(src, null);
                    if (propertyValue == null) {
                        property.SetValue(target, null, null);
                    }
                    else if (propertyValue.GetType().IsPrimitive || propertyValue.GetType().IsEnum || propertyValue is string) {
                        property.SetValue(target, propertyValue, null);
                    }
                    else {
                        property.SetValue(target, CloneObject(propertyValue), null);
                    }
                }
            }

            return target;
        }

        // https://www.cnblogs.com/zhaoshujie/p/16417922.html
        // 没效果
        // 速度比上面反射CloneObject要快一点点
        // public static class ExpressionCloner<TIn, TOut>
        //     where TIn : new()
        //     where TOut : new() {
        //     private static readonly Func<TIn, TOut> _cache = GetFunc();
        //
        //     private static Func<TIn, TOut> GetFunc() {
        //         var parameterExpression = Expression.Parameter(typeof(TIn), "p");
        //         var memberBindingList = new List<MemberBinding>();
        //
        //         foreach (var item in typeof(TOut).GetProperties()) {
        //             if (!item.CanWrite) continue;
        //             var property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name) ?? throw new InvalidOperationException());
        //             MemberBinding memberBinding = Expression.Bind(item, property);
        //             memberBindingList.Add(memberBinding);
        //         }
        //
        //         var memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
        //         var lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new[] { parameterExpression });
        //
        //         return lambda.Compile();
        //     }
        //
        //     public static TOut Clone(TIn tIn) {
        //         return _cache(tIn);
        //     }
        // }
    }
}