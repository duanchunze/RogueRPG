using Hsenl.EventType;

namespace Hsenl.View {
    public class EventCreateActor : AEventSync<EventType.CreateActor> {
        protected override void Handle(CreateActor arg) {
            var entity = arg.actor.Entity;
            var config = arg.actor.Config;

            var appear = entity.AddComponent<Appearance>();
            appear.LoadModel(config.ModelName);
            var headMessage = entity.AddComponent<HeadMessage>();
            var followMessage = entity.AddComponent<FollowMessage>();
            followMessage.uiStayTime = 0.75f;
        }
    }
}