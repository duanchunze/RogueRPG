using Hsenl.Network;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    [MessageRequest]
    public partial struct C2L_Login {
        public string account;
        public string password;
    }
    
    [MemoryPackable]
    [MessageResponse(typeof(C2L_Login))]
    public partial struct L2C_Login {
        public string ip;
        public int token;
        public int uniqueId;
    }
}