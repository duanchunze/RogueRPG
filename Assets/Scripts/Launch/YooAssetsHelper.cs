using UnityEngine;

namespace Hsenl {
    public class YooAssetsHelper {
        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        public static string GetHostServerURL(string package) {
            //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
            string hostServerIP = "http://127.0.0.1";
            string appVersion = "v1.0";

#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/CDN/Android/{package}/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/CDN/IPhone/{package}/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return $"{hostServerIP}/CDN/WebGL/{package}/{appVersion}";
            else
                return $"{hostServerIP}/CDN/PC/{package}/{appVersion}";
#else
            if (Application.platform == RuntimePlatform.Android)
                return $"{hostServerIP}/CDN/Android/{package}/{appVersion}";
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                return $"{hostServerIP}/CDN/IPhone/{package}/{appVersion}";
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
                return $"{hostServerIP}/CDN/WebGL/{package}/{appVersion}";
            else
                return $"{hostServerIP}/CDN/PC/{package}/{appVersion}";
#endif
        }
    }
}