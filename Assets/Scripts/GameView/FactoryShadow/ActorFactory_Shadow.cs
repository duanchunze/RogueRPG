namespace Hsenl.View {
    [ShadowFunction(typeof(ActorFactory))]
    public static partial class ActorFactory_Shadow {
        [ShadowFunction]
        private static Actor Create(int configId, Entity entity) {
            var actor = entity.GetComponent<Actor>();

            entity.AddComponent<Model>();
            entity.AddComponent<Motion>();
            var sound = entity.AddComponent<Sound>();
            sound.PlayOnAwake = false;
            entity.AddComponent<HeadInfo>();
            var followMessage = entity.AddComponent<HeadMessage>();
            followMessage.uiStayTime = 0.75f;
            return actor;
        }
    }
}