namespace Hsenl {
    public static class FixPhysic {
        public static void Init() {
            FixPhysicWorldManager._Instance.Init();
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public static bool Raycast(Ray ray) {
            return Raycast(ray.origin, ray.direction);
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool Raycast(Ray ray, out FixRaycastHit hit) {
            return Raycast(ray.origin, ray.direction, out hit);
        }

        /// <summary>
        /// 射线检测（大概检测，速度很快，但不精准，不推荐）
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public static bool RaycastRough(Ray ray) {
            return RaycastRough(ray.origin, ray.direction);
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static bool Raycast(Vector3 origin, Vector3 direction) {
            return FixPhysicWorldManager._Instance._defaultWorld._collisionWorld.RaycastToWorld(ref origin,
                ref direction, DetectPrecisionType.TryFast, out var hit);
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool Raycast(Vector3 origin, Vector3 direction, out FixRaycastHit hit) {
            return FixPhysicWorldManager._Instance._defaultWorld._collisionWorld.RaycastToWorld(ref origin,
                ref direction, DetectPrecisionType.Accurate, out hit);
        }

        /// <summary>
        /// 射线检测（大概检测，速度很快，但不精准，不推荐）
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public static bool RaycastRough(Vector3 origin, Vector3 direction) {
            return FixPhysicWorldManager._Instance._defaultWorld._collisionWorld.RaycastToWorld(ref origin,
                ref direction, DetectPrecisionType.Rough, out var hit);
        }
    }
}