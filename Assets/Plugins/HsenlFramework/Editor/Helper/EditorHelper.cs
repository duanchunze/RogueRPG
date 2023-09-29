using System.Reflection;

namespace Hsenl {
    public static class EditorHelper {
        public static object GetFieldValueOfPath(UnityEngine.Object o, string path, out FieldInfo field) {
            field = null;
            var type = o.GetType();
            object obj = o;
            var splits = path.Split('.');
            foreach (var split in splits) {
                if (string.IsNullOrEmpty(split)) continue;
                field = type.GetFieldInBase(split, AssemblyHelper.BindingFlagsInstanceIgnorePublic);
                if (field == null) return default;

                obj = field.GetValue(obj);
                if (obj == null) return default;

                type = field.FieldType;
            }

            return obj;
        }
    }
}