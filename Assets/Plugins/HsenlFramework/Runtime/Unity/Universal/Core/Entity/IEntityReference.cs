namespace Hsenl {
    public interface IEntityReference {
        public Entity Entity { get; }
        internal void SetFrameworkReference(Entity reference);
    }
}