namespace Hsenl.View {
    [ShadowFunction(typeof(ActorFactory))]
    public static partial class ActorFactory_Shadow {
        [ShadowFunction]
        private static Actor Create(int configId, Entity entity) {
            var actor = entity.GetComponent<Actor>();

            entity.AddComponent<Model>();
            var motion = entity.AddComponent<Motion>();
            var sound = entity.AddComponent<Sound>();
            sound.PlayOnAwake = false;
            var headMessage = entity.AddComponent<HeadMessage>();
            var followMessage = entity.AddComponent<FollowMessage>();
            followMessage.uiStayTime = 0.75f;
            return actor;
        }
    }
}