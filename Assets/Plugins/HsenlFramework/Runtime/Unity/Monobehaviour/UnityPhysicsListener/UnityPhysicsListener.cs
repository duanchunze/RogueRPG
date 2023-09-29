using UnityEngine;

namespace Hsenl {
    public abstract class UnityPhysicsListener<T> : MonoBehaviour where T : UnityPhysicsListener<T> {
        public static T Get(GameObject go) {
            var result = go.GetComponent<T>();
            if (result == null) {
                result = go.AddComponent<T>();
            }

            return result;
        }
    }
}