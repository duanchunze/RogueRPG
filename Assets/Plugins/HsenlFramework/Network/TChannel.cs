using System.Net.Sockets;

namespace Hsenl.Network {
    public class TChannel {
        public string channelId;
        public Socket socket;
        public SocketAsyncEventArgs recvEventArgs;
        public SocketAsyncEventArgs sendEventArgs;
    }
}