using System.Net;

namespace Hsenl.Network {
    public class ServiceConfig {
        public string ListenIPHost { get; set; }
        public int Port { get; set; }
        public int Backlog { get; set; }

        public int RecvBufferSize { get; set; }

        public int SendBufferSize { get; set; }

        public IPEndPoint GetListenIPEndPoint() {
            if (IPAddress.TryParse(this.ListenIPHost, out var address)) {
                var endPoint = new IPEndPoint(address, this.Port);
                return endPoint;
            }

            return null;
        }

        public void Reset() {
            this.ListenIPHost = null;
            this.Port = 0;
            this.Backlog = 0;
            this.RecvBufferSize = 0;
            this.SendBufferSize = 0;
        }

        public void Dispose() {
            this.Reset();
        }
    }
}