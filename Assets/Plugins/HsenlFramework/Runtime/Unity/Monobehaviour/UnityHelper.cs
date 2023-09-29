using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hsenl {
    public static class UnityHelper {
        public static class Path {
            /// <summary>
            /// 获取相对于unity的路径
            /// </summary>
            /// <param name="path"></param>
            /// <param name="containsAssets"></param>
            /// <returns></returns>
            public static string GetRelativePath(string path, bool containsAssets = false) {
                if (path.Contains("Assets")) {
                    var startIndex = path.LastIndexOf("Assets", StringComparison.Ordinal);
                    var endIndex = path.Length - startIndex;

                    path = path.Substring(startIndex, endIndex);
                }

                path = path.Replace("\\", "/");
                return containsAssets ? path : path.Replace("Assets/", "");
            }

            /// <summary>
            ///应用程序外部资源路径存放路径(热更新资源路径)
            /// </summary>
            public static string AppHotfixResPath {
                get {
                    var game = Application.productName;
                    var path = AppResPath;
                    if (Application.isMobilePlatform) {
                        path = $"{Application.persistentDataPath}/{game}/";
                    }

                    return path;
                }
            }

            /// <summary>
            /// 应用程序内部资源路径存放路径
            /// </summary>
            public static string AppResPath => Application.streamingAssetsPath;

            /// <summary>
            /// 应用程序内部资源路径存放路径(www/webrequest专用)
            /// </summary>
            public static string AppResPath4Web {
                get {
#if UNITY_IOS || UNITY_STANDALONE_OSX
                return $"file://{Application.streamingAssetsPath}";
#else
                    return Application.streamingAssetsPath;
#endif
                }
            }
        }

        public static class Other {
            private static bool _isProcessing;

            /// <summary>
            /// 调用手机的截图并分享
            /// </summary>
            /// <param name="onShared"></param>
            /// <returns></returns>
            public static IEnumerator ShareScreenshot(Action onShared) {
                if (_isProcessing) yield break;

                _isProcessing = true;
                yield return new WaitForEndOfFrame();
                ScreenCapture.CaptureScreenshot("screenshot.png", 2);
                var destination = System.IO.Path.Combine(Application.persistentDataPath, "screenshot.png");
                yield return new WaitForSeconds(0.3f); //WaitForSecondsRealtime(0.3f);
                if (!Application.isEditor) {
                    try {
                        var intentClass = new AndroidJavaClass("android.content.Intent");
                        var intentObject = new AndroidJavaObject("android.content.Intent");
                        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                        var uriClass = new AndroidJavaClass("android.net.Uri");
                        var uriObject =
                            uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
                        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),
                            uriObject);
                        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
                            "看看你能做对几道题？");
                        intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
                        var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                        var chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",
                            intentObject, "分享你的成绩单");
                        currentActivity.Call("startActivity", chooser);
                    }
                    catch (Exception e) {
                        Debug.LogError(e);
                    }

                    yield return new WaitForSeconds(1f); //WaitForSecondsRealtime(1f);
                }

                _isProcessing = false;
                onShared?.Invoke();
            }
        }

        public static class UI {
            /// <summary>
            /// 点是否在一个UI物体上
            /// </summary>
            public static bool IsPointerOverUI => UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

            /// <summary>
            /// 指定位置是否包含目标UI
            /// </summary>
            /// <param name="position"></param>
            /// <param name="target"></param>
            /// <returns></returns>
            public static bool IsHaveTarget(Vector2 position, GameObject target) {
                PointerEventData eventDataCurrentPosition = new(UnityEngine.EventSystems.EventSystem.current) {
                    position = position
                };
                var results = new List<RaycastResult>();
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                if (results.Count > 0) {
                    for (var i = 0; i < results.Count; i++) {
                        if (results[i].gameObject == target) return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 获取当前选中的UI物体
            /// </summary>
            public static GameObject GetCurrentSelectedTarget() {
                return UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            }

            /// <summary>
            /// 获取点下的UI物体
            /// </summary>
            public static GameObject GetPointCollider() {
                PointerEventData eventDataCurrentPosition = new(UnityEngine.EventSystems.EventSystem.current) {
                    position = Input.mousePosition
                };
                var results = new List<RaycastResult>();
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                if (results.Count > 0)
                    return results[0].gameObject;
                else
                    return null;
            }

            public static T GetComponentInPoint<T>() {
                return GetComponentInPoint<T>(Input.mousePosition);
            }

            public static T GetComponentInPoint<T>(Vector2 position) {
                PointerEventData eventDataCurrentPosition = new(UnityEngine.EventSystems.EventSystem.current) {
                    position = position
                };
                var results = new List<RaycastResult>();
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                if (results.Count > 0) {
                    foreach (var raycastResult in results) {
                        var t = raycastResult.gameObject.GetComponent<T>();
                        if (t != null) return t;
                    }
                }

                return default;
            }

            /// <summary>
            /// 获取点下的UI物体
            /// </summary>
            public static List<GameObject> GetPointColliders() {
                PointerEventData eventDataCurrentPosition = new(UnityEngine.EventSystems.EventSystem.current) {
                    position = Input.mousePosition
                };
                var targets = new List<RaycastResult>();
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, targets);

                var results = new List<GameObject>();
                for (var i = 0; i < targets.Count; i++) {
                    results.Add(targets[i].gameObject);
                }

                return results;
            }

            /// <summary>
            /// 获取点下的UI物体
            /// </summary>
            /// <param name="gms"></param>
            public static void GetPointCollidersNonAlloc(List<GameObject> gms) {
                if (gms == null) return;

                PointerEventData eventDataCurrentPosition = new(UnityEngine.EventSystems.EventSystem.current) {
                    position = Input.mousePosition
                };
                var targets = new List<RaycastResult>();
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, targets);

                gms.Clear();
                for (var i = 0; i < targets.Count; i++) {
                    gms.Add(targets[i].gameObject);
                }
            }

            public static bool WorldToUIPosition(RectTransform rect, Vector3 worldPos, out Vector3 uiWorldPos, Camera uiCamera, Camera worldCamera = null) {
                if (worldCamera == null) worldCamera = Camera.main;

                // 世界坐标在主相机中的位置
                var screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPos);
                // 把该位置转为UI相机中的世界位置
                return RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, uiCamera, out uiWorldPos);
            }

            public static bool WorldToUILocalPosition(RectTransform rect, Vector3 worldPos, out Vector2 uiLocalPos, Camera uiCamera,
                Camera worldCamera = null) {
                if (worldCamera == null) worldCamera = Camera.main;

                var screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPos);
                return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, uiCamera, out uiLocalPos);
            }
        }

        public static class Platform {
            public static string GetPlatformPath(RuntimePlatform platform) {
                var platformPath = string.Empty;
                switch (platform) {
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        platformPath = Application.streamingAssetsPath;
                        break;
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.Android:
                        platformPath = Application.persistentDataPath;
                        break;
                }

                return platformPath;
            }
        }

        public static class Vector {
            /// <summary>
            /// 获得相对于 Transform 偏移之后的向量
            /// 例如：相对于手部，向指定方向发射一条射线
            /// </summary>
            /// <param name="offsetDirection">xyz三个轴的值分别表示在各个轴的偏移量</param>
            /// <param name="tra">目标Transform</param>
            /// <returns></returns>
            public static Vector3 GetOffsetOfTransform(Vector3 offsetDirection, UnityEngine.Transform tra) {
                var localDirection =
                    Quaternion.Euler(offsetDirection) * Vector3.forward; // 相对于前方 旋转 offsetDirection，得到的是一个 local 的方向
                var worldDirection = tra.TransformDirection(localDirection);
                return worldDirection;
            }

            /// <summary>
            /// 判断目标是否在视野范围内
            /// 例如：判断敌人是否在主角的攻击的角度范围内
            /// </summary>
            /// <param name="trans"></param>
            /// <param name="target"></param>
            /// <param name="viewAngle"></param>
            /// <returns></returns>
            public static bool IsTargetInViewRound(UnityEngine.Transform trans, UnityEngine.Transform target, float viewAngle) {
                var position = trans.position;
                var pos1 = position;
                var pos2 = target.position;
                pos2.y = position.y;
                return Vector3.Angle(trans.forward, pos2 - pos1) < viewAngle / 2;
            }

            /// <summary>
            /// 获取两个向量的夹角，结果返回带正负的值
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static float Angle(Vector3 a, Vector3 b) {
                var c = Vector3.Cross(a, b);
                var angle = Vector3.Angle(a, b);

                return angle * Mathf.Sign(c.y);
            }

            /// <summary>
            /// 获取两个向量的夹角，结果返回带正负的值
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static float Angle(Vector2 a, Vector2 b) {
                var c = Vector3.Cross(a, b);
                var angle = Vector2.Angle(a, b);

                return angle * Mathf.Sign(-c.z);
            }

            /// <summary>
            /// 获得一个方向的大约方向
            /// </summary>
            /// <param name="direction"></param>
            /// <returns></returns>
            public static Vector2 GetRoundDirection(Vector2 direction) {
                var x = direction.x;
                var y = direction.y;
                var consult = Mathf.Abs(x) - Mathf.Abs(y);

                if (x == 0 && y == 0) return Vector2.zero;

                if (consult > 0) {
                    // 不是左就是右
                    if (x < 0) return Vector2.left;
                    if (x > 0) return Vector2.right;
                }
                else if (consult < 0) {
                    // 不是上就是下
                    if (y < 0) return Vector2.down;
                    if (y > 0) return Vector2.up;
                }
                else {
                    if (x > 0 && y > 0)
                        return Vector2.right;
                    else if (x > 0 && y < 0)
                        return Vector2.down;
                    else if (x < 0 && y > 0)
                        return Vector2.up;
                    else
                        return Vector2.left;
                }

                return Vector2.zero;
            }

            /// <summary>
            /// 获得一个方向的象限向量
            /// </summary>
            /// <param name="direction"></param>
            /// <returns></returns>
            public static Vector3Int GetQuadrant(Vector3 direction) {
                int x;
                int y;
                int z;

                if (direction.x > 0)
                    x = 1;
                else if (direction.x < 0)
                    x = -1;
                else
                    x = 0;

                if (direction.y > 0)
                    y = 1;
                else if (direction.y < 0)
                    y = -1;
                else
                    y = 0;

                if (direction.z > 0)
                    z = 1;
                else if (direction.z < 0)
                    z = -1;
                else
                    z = 0;

                return new Vector3Int(x, y, z);
            }

            /// <summary>
            /// 是否在屏幕内
            /// </summary>
            /// <param name="pos"></param>
            /// <returns></returns>
            public static bool IsWorld3DPosInScreen(Vector3 pos) {
                var cam3d = Camera.main;
                if (null == cam3d) return false;

                var screenPoint = cam3d.WorldToScreenPoint(pos);
                if (screenPoint.x < 0 || screenPoint.x > Screen.width || screenPoint.y < 0 || screenPoint.y > Screen.height) return false;
                return true;
            }

            /// <summary>
            /// 简单距离
            /// 当不需要真正计算距离，只是用来比大小的时候
            /// </summary>
            /// <param name="v1"></param>
            /// <param name="v2"></param>
            /// <returns></returns>
            public static float SimpleDistance(Vector3 v1, Vector3 v2) {
                return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y) + Mathf.Abs(v1.z - v2.z);
            }

            public static float Vector3ToAngle360(Vector3 from, Vector3 to) {
                var angle = Vector3.Angle(from, to);
                var cross = Vector3.Cross(from, to);
                return cross.y > 0 ? angle : 360 - angle;
            }

            /// <summary>
            /// 判断射线是否碰撞到球体，如果碰撞到，返回射线起点到碰撞点之间的距离
            /// </summary>
            /// <param name="ray"></param>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            /// <param name="dist"></param>
            /// <returns></returns>
            public static bool RayCastSphere(UnityEngine.Ray ray, Vector3 center, float radius, out float dist) {
                dist = 0;
                var ma = center - ray.origin;
                var distance = Vector3.Cross(ma, ray.direction).magnitude / ray.direction.magnitude;
                if (distance < radius) {
                    var op = GGTheorem(Vector3.Distance(center, ray.origin), distance);
                    var rp = GGTheorem(radius, distance);
                    dist = op - rp;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 勾股定理
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static float GGTheorem(float x, float y) {
                return Mathf.Sqrt(x * x + y * y);
            }
        }

        public static class Object {
            /// <summary>
            /// 拷贝组件
            /// </summary>
            /// <param name="src"></param>
            /// <param name="target"></param>
            /// <param name="includeInherit"></param>
            public static void CopyComponentTo(Component src, GameObject target, bool includeInherit = true) {
                var type = src.GetType();
                var copy = target.AddComponent(type);

                var flags = BindingFlags.Default;
                if (!includeInherit) {
                    flags |= BindingFlags.Public;
                    flags |= BindingFlags.NonPublic;
                    flags |= BindingFlags.Instance;
                    flags |= BindingFlags.DeclaredOnly;
                }

                foreach (var field in type.GetFields(flags)) {
                    field.SetValue(copy, field.GetValue(src));
                }

                foreach (var property in type.GetProperties(flags)) {
                    if (property.CanWrite) {
                        property.SetValue(copy, property.GetValue(src));
                    }
                }
            }

            public static void CopyComponentTo(Component src, Component target, bool includeInherit = true) {
                var type = src.GetType();
                var flags = BindingFlags.Default;
                if (!includeInherit) {
                    flags |= BindingFlags.Public;
                    flags |= BindingFlags.NonPublic;
                    flags |= BindingFlags.Instance;
                    flags |= BindingFlags.DeclaredOnly;
                }

                foreach (var field in type.GetFields(flags)) {
                    field.SetValue(target, field.GetValue(src));
                }

                foreach (var property in type.GetProperties(flags)) {
                    if (property.CanWrite) {
                        property.SetValue(target, property.GetValue(src));
                    }
                }
            }
        }

        public static class Resources { }
    }
}