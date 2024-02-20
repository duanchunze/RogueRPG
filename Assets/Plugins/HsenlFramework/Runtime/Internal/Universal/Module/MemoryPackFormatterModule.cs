namespace Hsenl {
    public class MemoryPackFormatterModule : MemoryPackFormatter {
        protected override void Register() {
            RegisterUnion<Bodied>();
            RegisterUnion<Unbodied>();
        }
    }
}