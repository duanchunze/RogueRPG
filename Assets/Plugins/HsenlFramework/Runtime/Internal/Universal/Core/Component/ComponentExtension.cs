using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hsenl {
    public static class ComponentExtension {
        public static void Reactivation(this Component self) {
            self.entity.Reactivation();
        }
        
        public static void SetParent(this Component self, Entity parent) {
            self.entity.SetParent(parent);
        }

        public static int GetOrder(this Component self) {
            return self.entity.GetOrder();
        }

        public static void SetOrder(this Component self, int order) {
            self.entity.SetSiblingIndex(order);
        }

        public static void SwapChildrenSeat(this Component self, Entity child1, Entity child2) {
            self.entity.SwapChildren(child1, child2);
        }

        public static Entity GetChild(this Component self, int index) {
            return self.entity.GetChild(index);
        }

        public static Entity FindChild(this Component self, string childName) {
            return self.entity.FindChild(childName);
        }

        public static T FindChild<T>(this Component self) where T : Component {
            return self.entity.FindChild<T>();
        }

        public static void SortChildren(this Component self, Comparison<Entity> comparison) {
            self.entity.SortChildren(comparison);
        }

        public static void SwapChild(this Component self, int idx1, int idx2) {
            self.entity.SwapChildren(idx1, idx2);
        }

        public static bool HasComponent<T>(this Component self, bool polymorphic = false) where T : class {
            return self.entity.HasComponent<T>(polymorphic);
        }

        public static bool HasComponentsAny(this Component self, ComponentTypeCacher typeCacher) {
            return self.entity.HasComponentsAny(typeCacher);
        }

        public static bool HasComponentsAny(this Component self, ComponentTypeCacher typeCacher, out int idx) {
            return self.entity.HasComponentsAny(typeCacher, out idx);
        }

        public static bool HasComponentsAll(this Component self, ComponentTypeCacher typeCacher) {
            return self.entity.HasComponentsAll(typeCacher);
        }

        public static T AddComponent<T>(this Component self) where T : Component {
            return self.entity.AddComponent<T>();
        }

        public static Component AddComponent(this Component self, Type type) {
            return self.entity.AddComponent(type);
        }

        public static T GetComponent<T>(this Component self, bool polymorphic = false) where T : class {
            return self.entity.GetComponent<T>(polymorphic);
        }

        public static T GetOrAddComponent<T>(this Component self, bool polymorphic = false) where T : Component {
            var t = self.entity.GetComponent<T>(polymorphic);
            if (t == null)
                t = self.entity.AddComponent<T>();
            return t;
        }

        public static Component GetComponent(this Component self, int componentIndex) {
            return self.entity.GetComponent(componentIndex);
        }

        public static T GetComponent<T>(this Component self, int componentIndex) where T : class {
            return self.entity.GetComponent<T>(componentIndex);
        }

        public static T[] GetComponents<T>(this Component self, bool polymorphic = false) where T : class {
            return self.entity.GetComponents<T>(polymorphic);
        }

        public static void GetComponents<T>(this Component self, List<T> results, bool polymorphic = false) where T : class {
            self.entity.GetComponents(results, polymorphic);
        }

        public static Component[] GetComponentsOfTypeCacher(this Component self, ComponentTypeCacher typeCacher) {
            return self.entity.GetComponentsOfTypeCacher(typeCacher);
        }

        public static void GetComponentsOfTypeCacher(this Component self, ComponentTypeCacher typeCacher, List<Component> results) {
            self.entity.GetComponentsOfTypeCacher(typeCacher, results);
        }

        public static T GetComponentInParent<T>(this Component self, bool includeInactive = false, bool polymorphic = false) where T : class {
            return self.entity.GetComponentInParent<T>(includeInactive, polymorphic);
        }

        public static T[] GetComponentsInParent<T>(this Component self, bool includeInactive = false, bool polymorphic = false) where T : class {
            return self.entity.GetComponentsInParent<T>(includeInactive, polymorphic);
        }

        public static Component[] GetComponentsInParentOfTypeCacher(this Component self, ComponentTypeCacher typeCacher, bool includeInactive = false) {
            return self.entity.GetComponentsInParentOfTypeCacher(typeCacher, includeInactive);
        }

        public static void GetComponentsInParentOfTypeCacher(this Component self, ComponentTypeCacher typeCacher, List<Component> results,
            bool includeInactive = false) {
            self.entity.GetComponentsInParentOfTypeCacher(typeCacher, results, includeInactive);
        }

        public static void GetComponentsInParent<T>(this Component self, List<T> results, bool includeInactive = false, bool polymorphic = false) where T : class {
            self.entity.GetComponentsInParent(results, includeInactive, polymorphic);
        }

        public static T GetComponentInChildren<T>(this Component self, bool includeInactive = false, bool polymorphic = false) where T : class {
            return self.entity.GetComponentInChildren<T>(includeInactive, polymorphic);
        }

        public static T[] GetComponentsInChildren<T>(this Component self, bool includeInactive = false, bool polymorphic = false) where T : class {
            return self.entity.GetComponentsInChildren<T>(includeInactive, polymorphic);
        }

        public static void GetComponentsInChildren<T>(this Component self, List<T> results, bool includeInactive = false, bool polymorphic = false) where T : class {
            self.entity.GetComponentsInChildren(results, includeInactive, polymorphic);
        }

        public static Component[] GetComponentsInChildrenOfTypeCacher(this Component self, ComponentTypeCacher typeCacher, bool includeInactive = false) {
            return self.entity.GetComponentsInChildrenOfTypeCacher(typeCacher, includeInactive);
        }

        public static void GetComponentsInChildrenOfTypeCacher(this Component self, ComponentTypeCacher typeCacher, List<Component> results,
            bool includeInactive = false) {
            self.entity.GetComponentsInChildrenOfTypeCacher(typeCacher, results, includeInactive);
        }

        public static Iterator<Entity> ForeachChildren(this Component self) {
            return self.Entity.ForeachChildren();
        }

        public static T GetOrAddComponent<T>(this Hsenl.Entity self, bool polymorphic = false) where T : Hsenl.Component {
            var c = self.GetComponent<T>(polymorphic);
            c ??= self.AddComponent<T>();

            return c;
        }
    }
}