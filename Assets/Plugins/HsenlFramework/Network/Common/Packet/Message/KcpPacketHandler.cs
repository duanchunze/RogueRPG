using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Hsenl.Network {
    public class KcpPacketHandler : IPacketSendProvider, IMessageWriter, IPacketRecvProvider, IMessageReader, IKcpHandler {
        public Action<Memory<byte>> OnMessageWrited { get; set; }
        public Action<Memory<byte>> OnMessageReaded { get; set; }

        private KcpChannel _channel;
        private Kcp _kcp;

        private PackageBuffer _bufferRecv = new();
        private PackageBuffer _bufferWriting = new();
        private PackageBuffer _bufferSending = new();
        private PackageBuffer _bufferReading = new();
        private ConcurrentQueue<(byte[] bytes, int len)> _outputs = new();

        private readonly object _locker = new();

        public void Init() { }

        public void Init<T>(T t) {
            switch (t) {
                case KcpChannel channel: {
                    this._channel = channel;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Init(uint conv, ArrayPool<byte> arrayPool) {
            this._kcp = new Kcp(conv, this.Output);
            this._kcp.SetNoDelay(1, 10, 2, true);
            this._kcp.SetWindowSize(1024, 1024);
            this._kcp.SetMtu(512);
            this._kcp.SetMinrto(30);
            this._kcp.SetArrayPool(arrayPool);
        }

        private void Output(byte[] bytes, int len) {
            this._outputs.Enqueue((bytes, len));
            switch (this._channel.UserToken) {
                case KcpClient kcpClient: {
                    kcpClient.StartSend(this._channel.ChannelId);
                    break;
                }

                case KcpServer kcpServer: {
                    kcpServer.StartSend(this._channel.ChannelId);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public byte[] GetSendBuffer(int len, out int offset, out int count) {
            if (this._outputs.Count == 0) {
                goto FAIL_RETURN;
            }

            if (this._bufferSending.Length < this._kcp.WaitSnd)
                this._bufferSending.SetLength(this._kcp.WaitSnd);

            this._bufferSending.Advance(1);

            var pos = this._bufferSending.Position;
            while (this._outputs.TryPeek(out var result)) {
                // 可以比给的长度小, 但不可以比他大
                if (this._bufferSending.Position + result.len > len) {
                    if (result.len > len)
                        throw new ArgumentException($"kcp mtu cant greater than '{len}'");
                    break;
                }

                this._outputs.TryDequeue(out result);
                this._bufferSending.Write(result.bytes, 0, result.len);
            }

            if (this._bufferSending.Position - pos == 0) {
                goto FAIL_RETURN;
            }

            offset = 0;
            count = this._bufferSending.Position;
            this._bufferSending.Seek(0, SeekOrigin.Begin);
            var bytes = this._bufferSending.GetBuffer();
            bytes[0] = KcpProtocalType.MSG;
            return bytes;

            FAIL_RETURN:
            offset = 0;
            count = 0;
            return null;
        }

        public Memory<byte> GetSendBuffer(int length) {
            throw new InvalidOperationException();
        }

        public byte[] GetRecvBuffer(int len, out int offset, out int count) {
            this._bufferRecv.GetMemory(len);
            offset = this._bufferRecv.Origin;
            count = len;
            return this._bufferRecv.GetBuffer();
        }

        public Memory<byte> GetRecvBuffer(int length) {
            return this._bufferRecv.GetMemory(length);
        }

        public void Write(byte[] data, int offset, int count) {
            this.Write(data.AsSpan(offset, count));
        }

        public void Write(Span<byte> data) {
            var len = data.Length;
            if (len == 0)
                return;

            this._bufferWriting.Seek(0, SeekOrigin.Begin);
            this._bufferWriting.Write(data);
            try {
                this.OnMessageWrited?.Invoke(this._bufferWriting.AsMemory(0, len));
            }
            catch (Exception e) {
                Log.Error(e);
            }

            lock (this._locker) {
                this._kcp.Send(this._bufferWriting.AsSpan(0, len));
            }
        }

        public void Read(Memory<byte> data) {
            lock (this._locker) {
                this._kcp.Input(data.Slice(1).Span);
            }
        }

        public uint Update(uint currentTimeMS) {
            lock (this._locker) {
                if (this._kcp == null)
                    return 0;
                this._kcp.Update(currentTimeMS);
                var len = this._kcp.PeekSize();
                while (len > 0) {
                    this._kcp.Receive(this._bufferReading.GetSpan(len));
                    var memory = this._bufferReading.AsMemory(0, len);
                    try {
                        this.OnMessageReaded?.Invoke(memory);
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }

                    len = this._kcp.PeekSize();
                }

                return this._kcp.Check(currentTimeMS);
            }
        }

        public void Dispose() {
            this.OnMessageWrited = null;
            this.OnMessageReaded = null;

            this._channel = null;
            this._kcp = null;

            this._bufferRecv.Reset();
            this._bufferWriting.Reset();
            this._bufferSending.Reset();
            this._bufferReading.Reset();
            this._outputs.Clear();
        }
    }
}