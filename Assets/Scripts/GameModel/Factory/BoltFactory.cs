using Hsenl.bolt;

namespace Hsenl {
    [ShadowFunction]
    public static partial class BoltFactory {
        public static Bolt Create(BoltConfig config) {
            var entity = Entity.Create(config.Alias);
            entity.Active = false;

            var bolt = entity.AddComponent<Bolt>();
            bolt.Init(config.Id);

            switch (config.Shape) {
                case 0: {
                    var collider = entity.AddComponent<SphereCollider>();
                    collider.IsTrigger = true;
                    bolt.Entity.AddComponent<CollisionEventListener>();
                    break;
                }

                case 1: {
                    var collider = entity.AddComponent<BoxCollider>();
                    collider.IsTrigger = true;
                    bolt.Entity.AddComponent<CollisionEventListener>();
                    break;
                }

                case -1: {
                    break;
                }
            }

            var tra = bolt.transform;
            var size = config.Size;
            tra.LocalScale = new Vector3(size.X, size.Y, size.Z);

            CreateBolt(bolt);

            return bolt;
        }

        [ShadowFunction]
        private static void CreateBolt(Bolt bolt) {
            CreateBoltShadow(bolt);
        }
    }
}