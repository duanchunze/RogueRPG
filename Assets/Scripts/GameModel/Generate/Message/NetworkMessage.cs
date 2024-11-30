using Hsenl.Network;
using MemoryPack;

namespace Hsenl {
    /*
     * 重要注意!!! 所有Rpc消息的第一个参数一定要是int类型的RpcId, 必须是int类型, 必须在第一位
     */
    [MemoryPackable]
    [MessageRequest(typeof(R2C_Login))]
    public partial struct C2R_Login : IRpcMessage {
        public int RpcId { get; set; }

        public string account;
        public string password;
    }

    [MemoryPackable]
    [MessageResponse]
    public partial struct R2C_Login : IRpcMessage {
        public int RpcId { get; set; }

        public string ip;
        public int token;
        public int uniqueId;
    }
}