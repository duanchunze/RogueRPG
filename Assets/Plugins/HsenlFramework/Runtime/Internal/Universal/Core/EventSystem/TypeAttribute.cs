namespace Hsenl {
    public abstract class TypeAttribute : BaseAttribute {
        public string TypeName { get; }

        protected TypeAttribute(string typeName) {
            this.TypeName = typeName;
        }
    }
}