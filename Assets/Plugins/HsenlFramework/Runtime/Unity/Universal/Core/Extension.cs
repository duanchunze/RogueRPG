namespace Hsenl {
    public static partial class Extension {
        public static EntityReference GetOrCreateEntityReference(this UnityEngine.GameObject self) {
            var entityRef = self.GetComponent<EntityReference>();
            if (entityRef == null) {
                Entity.Create(self);
                entityRef = self.GetComponent<EntityReference>();
            }

            return entityRef;
        }

        public static Entity SetMonoParent(this Entity self, UnityEngine.Transform monoParent) {
            var parent = monoParent.gameObject.GetOrCreateEntityReference().Entity;
            self.SetParent(parent);
            return parent;
        }

        public static Entity SetMonoParent(this Component self, UnityEngine.Transform monoParent) {
            var parent = monoParent.gameObject.GetOrCreateEntityReference().Entity;
            self.SetParent(parent);
            return parent;
        }

        public static T GetFrameworkComponent<T>(this UnityEngine.Component self, bool polymorphic = false) where T : class {
            var entityRef = self.GetComponent<EntityReference>();
            return entityRef == null ? null : entityRef.Entity.GetComponent<T>(polymorphic);
        }

        public static T GetFrameworkComponentInParent<T>(this UnityEngine.Component self, bool polymorphic = false) where T : class {
            var parent = self.transform;
            while (parent != null) {
                var entityRef = parent.GetComponent<EntityReference>();
                if (entityRef != null) {
                    var t = entityRef.Entity.GetComponent<T>(polymorphic);
                    if (t != null) {
                        return t;
                    }
                }

                parent = parent.parent;
            }

            return null;
        }

        public static T GetFrameworkComponent<T>(this UnityEngine.GameObject self, bool polymorphic = false) where T : class {
            var entityRef = self.GetComponent<EntityReference>();
            return entityRef == null ? null : entityRef.Entity.GetComponent<T>(polymorphic);
        }

        public static T GetMonoComponent<T>(this Component self) {
            return self.Entity.GameObject.GetComponent<T>();
        }
        
        public static T[] GetMonoComponents<T>(this Component self) {
            return self.Entity.GameObject.GetComponents<T>();
        }

        public static T GetMonoComponentInParent<T>(this Component self) {
            return self.Entity.GameObject.GetComponentInParent<T>();
        }

        public static T GetMonoComponentInChildren<T>(this Component self) {
            return self.Entity.GameObject.GetComponentInChildren<T>();
        }
        
        public static T[] GetMonoComponentsInChildren<T>(this Component self) {
            return self.Entity.GameObject.GetComponentsInChildren<T>();
        }
    }
}