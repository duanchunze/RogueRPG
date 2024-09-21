namespace Hsenl {
    public static class NumericExtension {
        public static bool IsHasValue(this Numerator self, NumericType numericType) {
            return self.IsHasValue((uint)numericType);
        }
        
        public static Num GetValue(this Numerator self, NumericType numericType) {
            return self.GetFinalValue((uint)numericType);
        }

        public static void SetValue(this Numerator self, NumericType numericType, Num value, bool forceSendEvent = true) {
            self.SetRawValue((uint)numericType, value, forceSendEvent);
        }
    }
}