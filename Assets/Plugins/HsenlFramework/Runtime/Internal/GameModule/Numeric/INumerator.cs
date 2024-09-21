namespace Hsenl {
    public interface INumerator {
        bool Attach(INumericNode node);
        bool Detach(INumericNode node);
        Num GetRawValue(uint numericType);
        void SetRawValue(uint numericType, Num value, bool sendEvent = true);
        Num GetFinalValue(uint numericType);
        void Recalculate(bool sendEvent = false);
        void Recalculate(uint numericType, bool sendEvent = true);
    }
}