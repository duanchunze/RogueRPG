using System;

namespace Hsenl {
    public static class EnumHelper {
        public static int EnumIndex<T>(int value) {
            var i = 0;
            foreach (var v in Enum.GetValues(typeof(T))) {
                if ((int)v == value) {
                    return i;
                }

                ++i;
            }

            return -1;
        }

        // 根据值获得名称，如果没有该枚举元素，则返回该int的字符串
        public static string GetName<T>(int value) {
            return Enum.ToObject(typeof(T), value).ToString();
        }
        
        public static string GetName(Type enumType, int value) {
            return Enum.ToObject(enumType, value).ToString();
        }

        public static T FromString<T>(string str) {
            if (!Enum.IsDefined(typeof(T), str)) {
                return default(T);
            }

            return (T)Enum.Parse(typeof(T), str);
        }

        public static T[] FromString<T>(string[] strs) {
            var ts = new T[strs.Length];
            for (var i = 0; i < strs.Length; i++) {
                if (!Enum.IsDefined(typeof(T), strs[i])) {
                    throw new Exception($"convert string to {typeof(T)} fail");
                }

                ts[i] = (T)Enum.Parse(typeof(T), strs[i]);
            }

            return ts;
        }
    }
}