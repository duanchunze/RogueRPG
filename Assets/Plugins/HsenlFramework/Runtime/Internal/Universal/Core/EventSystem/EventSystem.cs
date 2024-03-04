using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hsenl {
    public static class EventSystem {
        public static void RegisterAttributeType(Type type) => EventSystemManager.Instance.RegisterAttributeType(type);

        public static void AddAssembles(Assembly[] assemblies) =>
            EventSystemManager.Instance.AddAssembles(assemblies);

        public static Assembly[] GetAssemblies() => EventSystemManager.Instance.GetAssemblies();

        public static void AddTypes(Type[] types) => EventSystemManager.Instance.AddTypes(types);

        public static Type[] GetAllTypes() => EventSystemManager.Instance.GetAllTypes();
        public static Type FindType(string typeName) => EventSystemManager.Instance.FindType(typeName);
        
        /// <param name="attributeType"></param>
        /// <param name="polymorphism">是否支持多态</param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetTypesOfAttribute(Type attributeType, bool polymorphism = false) => EventSystemManager.Instance.GetTypesOfAttribute(attributeType, polymorphism);

        public static IReadOnlyList<FieldInfo> GetFieldsOfAttribute(Type attributeType, Type classType = null) =>
            EventSystemManager.Instance.GetFieldsOfAttribute(attributeType, classType);

        public static IReadOnlyList<PropertyInfo> GetPropertiesOfAttribute(Type attributeType, Type classType = null) =>
            EventSystemManager.Instance.GetPropertiesOfAttribute(attributeType, classType);

        public static IReadOnlyList<MethodInfo> GetMethodsOfAttribute(Type attributeType, Type classType = null) =>
            EventSystemManager.Instance.GetMethodsOfAttribute(attributeType, classType);

        public static void SetEventEnable<TEventKey, TEvent>(bool value) => EventSystemManager.Instance.SetEventEnable<TEventKey, TEvent>(value);
        public static Object GetInstance(int instanceId) => EventSystemManager.Instance.GetInstance(instanceId);
        public static T GetInstance<T>(int instanceId) where T : Object => EventSystemManager.Instance.GetInstance<T>(instanceId);
        public static async HTask PublishAsync<T>(T a) where T : struct => await EventSystemManager.Instance.PublishAsync(a);
        public static void Publish<T>(T a) where T : struct => EventSystemManager.Instance.Publish(a);
        public static void Invoke<TArg>(TArg args) where TArg : struct => EventSystemManager.Instance.Invoke(args);
        public static TReturn Invoke<TArg, TReturn>(TArg args) where TArg : struct => EventSystemManager.Instance.Invoke<TArg, TReturn>(args);

        public static void Link<T>(int instanceId, int instanceIdTarget, int key = 0) where T : Object => EventSystemManager.Instance.Link<T>(instanceId, instanceIdTarget, key);
        public static void Link(int instanceId, int instanceIdTarget, int key) => EventSystemManager.Instance.Link(instanceId, instanceIdTarget, key);
        public static void Unlink(int instanceId) => EventSystemManager.Instance.Unlink(instanceId);
        public static void Unlink<T>(int instanceId, int key = 0) where T : Object => EventSystemManager.Instance.Unlink<T>(instanceId, key);
        public static void Unlink(int instanceId, int key) => EventSystemManager.Instance.Unlink(instanceId, key);
        public static T GetLinker<T>(int instanceId, int key = 0) where T : Object => EventSystemManager.Instance.GetLinker<T>(instanceId, key);
    }
}