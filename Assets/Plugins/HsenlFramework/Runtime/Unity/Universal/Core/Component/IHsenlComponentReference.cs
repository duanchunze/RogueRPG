namespace Hsenl {
    public interface IHsenlComponentReference {
        protected internal void SetFrameworkReference(Component reference);
    }

    public interface IHsenlComponentReference<out T> : IHsenlComponentReference {
        public T HsenlComponent { get; }
    }
}