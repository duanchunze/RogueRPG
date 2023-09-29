using System;
using System.Collections.Generic;

namespace Hsenl {
    public class Factory : IDisposable {
        private readonly List<FactoryBuilder> _builders = new();
        private Entity _rootEntity;

        public static Factory Create(Entity root) {
            var factory = ObjectPool.Fetch<Factory>();
            factory._rootEntity = root;
            return factory;
        }

        public Factory AddBuilder(FactoryBuilder builder) {
            this._builders.Add(builder);
            return this;
        }

        public void Build() {
            for (int i = 0, len = this._builders.Count; i < len; i++) {
                var builder = this._builders[i];
                builder.Build(this._rootEntity);
            }
        }

        public void Dispose() {
            this._builders.Clear();
            ObjectPool.Recycle(this);
        }
    }
}