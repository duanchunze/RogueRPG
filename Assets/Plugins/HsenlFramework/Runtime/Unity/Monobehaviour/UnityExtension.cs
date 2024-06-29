using System;
using Unity.Mathematics;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace Hsenl {
    public static class UnityExtension {
        // 规范子物体
        public static void NormalizeChildren(this UnityEngine.Transform self, UnityEngine.Transform template, int count) {
            if (self.childCount > count) {
                for (var i = 0; i < self.childCount; i++) {
                    if (i < count) {
                        self.GetChild(i).gameObject.SetActive(true);
                    }
                    else {
                        self.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else if (self.childCount < count) {
                for (var i = 0; i < self.childCount; i++) {
                    self.GetChild(i).gameObject.SetActive(true);
                }

                for (var i = self.childCount; i < count; i++) {
                    UnityEngine.Object.Instantiate(template, self).gameObject.SetActive(true);
                }
            }
            else {
                for (var i = 0; i < self.childCount; i++) {
                    self.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        public static T GetOrAddComponent<T>(this UnityEngine.GameObject self) where T : UnityEngine.Component {
            var t = self.GetComponent<T>();
            if (t) {
                return t;
            }

            return self.AddComponent<T>();
        }

        public static UnityEngine.Component GetOrAddComponent(this UnityEngine.GameObject self, Type type) {
            var t = self.GetComponent(type);
            if (t) {
                return t;
            }

            return self.AddComponent(type);
        }

        public static T GetOrAddComponent<T>(this UnityEngine.Component self) where T : UnityEngine.Component {
            var t = self.GetComponent<T>();
            if (t) {
                return t;
            }

            return self.gameObject.AddComponent<T>();
        }

        public static UnityEngine.Component GetOrAddComponent(this UnityEngine.MonoBehaviour self, Type type) {
            var t = self.GetComponent(type);
            if (t) {
                return t;
            }

            return self.gameObject.AddComponent(type);
        }

        public static UnityEngine.Transform FindOrCreateChild(this UnityEngine.Transform self, string holderName) {
            var tra = self.Find(holderName);
            if (tra != null) return tra;
            tra = new GameObject(holderName).transform;
            tra.SetParent(self, false);
            return tra;
        }

        public static void ForeachAllChildren<T>(this UnityEngine.Transform self, Action<UnityEngine.Transform, T> action, T data = default) {
            for (int i = 0, len = self.childCount; i < len; i++) {
                var child = self.GetChild(i);
                action.Invoke(child, data);
                child.ForeachAllChildren(action, data);
            }
        }

        /// <summary>
        /// 取 <see cref="Vector3" /> 的 (x, y, z) 转换为 <see cref="Vector2" /> 的 (x, z)。
        /// </summary>
        /// <param name="vector3">要转换的 Vector3。</param>
        /// <returns>转换后的 Vector2。</returns>
        public static Vector2 ToVector2(this Vector3 vector3) {
            return new Vector2(vector3.x, vector3.z);
        }

        /// <summary>
        /// 取 <see cref="Vector2" /> 的 (x, y) 转换为 <see cref="Vector3" /> 的 (x, 0, y)。
        /// </summary>
        /// <param name="vector2">要转换的 Vector2。</param>
        /// <returns>转换后的 Vector3。</returns>
        public static Vector3 ToVector3(this Vector2 vector2) {
            return new Vector3(vector2.x, 0f, vector2.y);
        }

        /// <summary>
        /// 取 <see cref="Vector2" /> 的 (x, y) 和给定参数 y 转换为 <see cref="Vector3" /> 的 (x, 参数 y, y)。
        /// </summary>
        /// <param name="vector2">要转换的 Vector2。</param>
        /// <param name="y">Vector3 的 y 值。</param>
        /// <returns>转换后的 Vector3。</returns>
        public static Vector3 ToVector3(this Vector2 vector2, float y) {
            return new Vector3(vector2.x, y, vector2.y);
        }

        #region Transform

        public static void Reset(this UnityEngine.Transform transform) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 设置绝对位置的 x 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">x 坐标值。</param>
        public static void SetPositionX(this UnityEngine.Transform transform, float newValue) {
            var v = transform.position;
            v.x = newValue;
            transform.position = v;
        }

        /// <summary>
        /// 设置绝对位置的 y 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">y 坐标值。</param>
        public static void SetPositionY(this UnityEngine.Transform transform, float newValue) {
            var v = transform.position;
            v.y = newValue;
            transform.position = v;
        }

        /// <summary>
        /// 设置绝对位置的 z 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">z 坐标值。</param>
        public static void SetPositionZ(this UnityEngine.Transform transform, float newValue) {
            var v = transform.position;
            v.z = newValue;
            transform.position = v;
        }

        /// <summary>
        /// 增加绝对位置的 x 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">x 坐标值增量。</param>
        public static void AddPositionX(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.position;
            v.x += deltaValue;
            transform.position = v;
        }

        /// <summary>
        /// 增加绝对位置的 y 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">y 坐标值增量。</param>
        public static void AddPositionY(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.position;
            v.y += deltaValue;
            transform.position = v;
        }

        /// <summary>
        /// 增加绝对位置的 z 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">z 坐标值增量。</param>
        public static void AddPositionZ(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.position;
            v.z += deltaValue;
            transform.position = v;
        }

        /// <summary>
        /// 设置相对位置的 x 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">x 坐标值。</param>
        public static void SetLocalPositionX(this UnityEngine.Transform transform, float newValue) {
            var v = transform.localPosition;
            v.x = newValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// 设置相对位置的 y 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">y 坐标值。</param>
        public static void SetLocalPositionY(this UnityEngine.Transform transform, float newValue) {
            var v = transform.localPosition;
            v.y = newValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// 设置相对位置的 z 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">z 坐标值。</param>
        public static void SetLocalPositionZ(this UnityEngine.Transform transform, float newValue) {
            var v = transform.localPosition;
            v.z = newValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// 增加相对位置的 x 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">x 坐标值。</param>
        public static void AddLocalPositionX(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.localPosition;
            v.x += deltaValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// 增加相对位置的 y 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">y 坐标值。</param>
        public static void AddLocalPositionY(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.localPosition;
            v.y += deltaValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// 增加相对位置的 z 坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">z 坐标值。</param>
        public static void AddLocalPositionZ(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.localPosition;
            v.z += deltaValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// 设置相对尺寸的 x 分量。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">x 分量值。</param>
        public static void SetLocalScaleX(this UnityEngine.Transform transform, float newValue) {
            var v = transform.localScale;
            v.x = newValue;
            transform.localScale = v;
        }

        /// <summary>
        /// 设置相对尺寸的 y 分量。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">y 分量值。</param>
        public static void SetLocalScaleY(this UnityEngine.Transform transform, float newValue) {
            var v = transform.localScale;
            v.y = newValue;
            transform.localScale = v;
        }

        /// <summary>
        /// 设置相对尺寸的 z 分量。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="newValue">z 分量值。</param>
        public static void SetLocalScaleZ(this UnityEngine.Transform transform, float newValue) {
            var v = transform.localScale;
            v.z = newValue;
            transform.localScale = v;
        }

        /// <summary>
        /// 增加相对尺寸的 x 分量。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">x 分量增量。</param>
        public static void AddLocalScaleX(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.localScale;
            v.x += deltaValue;
            transform.localScale = v;
        }

        /// <summary>
        /// 增加相对尺寸的 y 分量。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">y 分量增量。</param>
        public static void AddLocalScaleY(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.localScale;
            v.y += deltaValue;
            transform.localScale = v;
        }

        /// <summary>
        /// 增加相对尺寸的 z 分量。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="deltaValue">z 分量增量。</param>
        public static void AddLocalScaleZ(this UnityEngine.Transform transform, float deltaValue) {
            var v = transform.localScale;
            v.z += deltaValue;
            transform.localScale = v;
        }

        /// <summary>
        /// 二维空间下使 <see cref="UnityEngine.Transform" /> 指向指向目标点的算法，使用世界坐标。
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> 对象。</param>
        /// <param name="lookAtPoint2D">要朝向的二维坐标点。</param>
        /// <remarks>假定其 forward 向量为 <see cref="Vector3.up" />。</remarks>
        public static void LookAt2D(this UnityEngine.Transform transform, Vector2 lookAtPoint2D) {
            var vector = lookAtPoint2D.ToVector3() - transform.position;
            vector.y = 0f;

            if (vector.magnitude > 0f) {
                transform.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
            }
        }

        public static void LookAtLerp(this UnityEngine.Transform self, float3 forward, float t) {
            var forwardRotation = quaternion.LookRotation(forward, math.up());
            self.rotation = math.nlerp(self.rotation, forwardRotation, t);
        }

        public static void LookAtPointLerp(this UnityEngine.Transform self, float3 point, float t) {
            var position = (float3)self.transform.position;
            if (point.Equals(position)) return;

            var forward = point - position;
            var forwardRotation = quaternion.LookRotation(forward, math.up());
            self.rotation = math.nlerp(self.rotation, forwardRotation, t);
        }

        #endregion Transform
    }
}