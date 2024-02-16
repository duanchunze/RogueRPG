namespace Hsenl.View {
    [ShadowFunction(typeof(ActorFactory))]
    public static partial class ActorFactoryShadow {
        [ShadowFunction]
        private static Actor Create(int configId, Entity entity) {
            var actor = entity.GetComponent<Actor>();

            var appear = entity.AddComponent<Appearance>();
            appear.LoadModel(actor.Config.ModelName);
            var headMessage = entity.AddComponent<HeadMessage>();
            var followMessage = entity.AddComponent<FollowMessage>();
            followMessage.uiStayTime = 0.75f;
            return actor;
        }
    }
}