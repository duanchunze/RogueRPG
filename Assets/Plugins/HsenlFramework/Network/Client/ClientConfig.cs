using System.Net;

namespace Hsenl.Network {
    public class ClientConfig {
        public string RemoteIPHost { get; set; }

        public int Port { get; set; }
        
        public int RecvBufferSize { get; set; }

        public int SendBufferSize { get; set; }

        public IPEndPoint GetRemoteIPEndPoint() {
            if (IPAddress.TryParse(this.RemoteIPHost, out var address)) {
                var endPoint = new IPEndPoint(address, this.Port);
                return endPoint;
            }

            return null;
        }

        public void Reset() {
            this.RemoteIPHost = null;
            this.Port = 0;
            this.RecvBufferSize = 0;
            this.SendBufferSize = 0;
        }

        public void Dispose() {
            this.Reset();
        }
    }
}