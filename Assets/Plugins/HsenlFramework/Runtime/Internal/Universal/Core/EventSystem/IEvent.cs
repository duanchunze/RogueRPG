using System;

namespace Hsenl {
    public enum EventModel {
        Sync,
        Async,
    }
    public interface IEvent {
        Type Type { get; }
        EventModel EventModel { get; }
    }

    [Event]
    public abstract class AEventSync<TArg> : IEvent where TArg : struct {
        public Type Type => typeof(TArg);
        public EventModel EventModel => EventModel.Sync;

        protected abstract void Handle(TArg arg);

        public void Invoke(TArg arg) {
            this.Handle(arg);
        }
    }
    
    [Event]
    public abstract class AEventAsync<TArg> : IEvent where TArg : struct {
        public Type Type => typeof(TArg);
        public EventModel EventModel => EventModel.Async;

        protected abstract HTask Handle(TArg arg);

        public async HTask Invoke(TArg arg) {
            await this.Handle(arg);
        }
    }
}