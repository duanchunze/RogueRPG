using System;
using System.Net.Sockets;

namespace Hsenl.Network {
    // 外包socket, 对socket进行封装
    public abstract class EpibolySocket {
        private Socket _socket;
        
        public Socket Socket => this._socket;
        
        public bool IsDisposed => this._socket == null;
        
        public Action<int> OnError { get; set; }

        protected EpibolySocket(Socket socket) {
            this._socket = socket;
        }
        
        protected void Error(int errorCode) {
            try {
                this.OnError?.Invoke(errorCode);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }
        
        public virtual void Dispose() {
            this.OnError = null;
            this._socket.Close();
            this._socket = null;
        }
        
        protected void CheckDisposedException() {
            if (this.IsDisposed)
                throw new Exception("Channel is disposed!");
        }
    }
}