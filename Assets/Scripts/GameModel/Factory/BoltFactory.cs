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
                    collider.Center = config.Center.ToVector3();
                    collider.Radius = config.Size.X;
                    collider.IsTrigger = true;
                    bolt.AddComponent<CollisionEventListener>();
                    break;
                }

                case 1: {
                    var collider = entity.AddComponent<BoxCollider>();
                    collider.Center = config.Center.ToVector3();
                    collider.Size = config.Size.ToVector3();
                    collider.IsTrigger = true;
                    bolt.AddComponent<CollisionEventListener>();
                    break;
                }

                case -1: {
                    break;
                }
            }

            CreateBolt(bolt);

            return bolt;
        }

        [ShadowFunction]
        private static void CreateBolt(Bolt bolt) {
            CreateBoltShadow(bolt);
        }
    }
}