using Hsenl.Network;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    [MessageRequest]
    public partial struct C2G_Login {
        public string account;
        public string password;
    }
    
    [MemoryPackable]
    [MessageResponse(typeof(C2G_Login))]
    public partial struct G2C_Login {
        public string ip;
        public int verificationCode;
    }
}