using System;
using Unity.Mathematics;

namespace Hsenl {
    public static class ControlExtension {
        public static void SetStart<T>(this Control self, T controlCode) where T : Enum => self.SetStart(controlCode.GetHashCode());
        public static void SetSustained<T>(this Control self, T controlCode) where T : Enum => self.SetSustained(controlCode.GetHashCode());
        public static void SetEnd<T>(this Control self, T controlCode) where T : Enum => self.SetEnd(controlCode.GetHashCode());
        public static void SetValue<T>(this Control self, T controlCode, float3 val) where T : Enum => self.SetValue(controlCode.GetHashCode(), val);
        public static bool GetStart<T>(this Control self, T controlCode) where T : Enum => self.GetStart(controlCode.GetHashCode());
        public static bool GetSustained<T>(this Control self, T controlCode) where T : Enum => self.GetSustained(controlCode.GetHashCode());
        public static bool GetEnd<T>(this Control self, T controlCode) where T : Enum => self.GetEnd(controlCode.GetHashCode());

        public static bool GetValue<T>(this Control self, T controlCode, out float3 result) where T : Enum =>
            self.GetValue(controlCode.GetHashCode(), out result);

        public static void AddStartListener<T>(this Control self, T controlCode, Action onStart) where T : Enum =>
            self.AddStartListener(controlCode.GetHashCode(), onStart);

        public static void AddSustainedListener<T>(this Control self, T controlCode, Action onSustained) where T : Enum =>
            self.AddSustainedListener(controlCode.GetHashCode(), onSustained);

        public static void AddEndListener<T>(this Control self, T controlCode, Action onEnd) where T : Enum =>
            self.AddEndListener(controlCode.GetHashCode(), onEnd);

        public static void RemoveStartListener<T>(this Control self, T controlCode, Action onStart) where T : Enum =>
            self.RemoveStartListener(controlCode.GetHashCode(), onStart);

        public static void RemoveSustainedListener<T>(this Control self, T controlCode, Action onSustained) where T : Enum =>
            self.RemoveSustainedListener(controlCode.GetHashCode(), onSustained);

        public static void RemoveEndListener<T>(this Control self, T controlCode, Action onEnd) where T : Enum =>
            self.RemoveEndListener(controlCode.GetHashCode(), onEnd);
    }
}