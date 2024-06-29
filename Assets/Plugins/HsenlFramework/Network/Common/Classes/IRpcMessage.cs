namespace Hsenl.Network {
    public interface IRpcMessage : IMessage {
        public int RpcId { get; set; }
    }
}