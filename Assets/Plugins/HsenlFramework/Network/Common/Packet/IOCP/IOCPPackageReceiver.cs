using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hsenl.Network {
    public class IOCPPackageReceiver : IPackageRecvBufferProvider, IPackageReader {
        protected virtual int BodySizeBits => 4;
        protected virtual int OpcodeBits => 2;
        protected virtual int TotalHeadBits => 6;

        private PackageBuffer _recvBuffer = new();
        private int _completeMessageSize; // 完整的消息大小, 该值如果不为0, 代表当前消息没有接收完

        public Action<ushort, Memory<byte>> OnMessageReaded { get; set; } // 当读取出了包(包括解析)

        public virtual void Init() { }

        // 该方法和Read在实际运行过程中, 不会有冲突关系, 所以不需要加锁
        public virtual Memory<byte> GetRecvBuffer(int len) {
            this._recvBuffer.Advance(len);
            return this._recvBuffer.AsMemory(0, len);
        }

        // 读包, 拆包
        public virtual void Read(Memory<byte> data) {
            while (true) {
                var len = data.Length;
                if (this._completeMessageSize == 0) {
                    if (len <= this.TotalHeadBits) {
                        if (this._recvBuffer.Origin != 0) {
                            // 这种情况是因为上一轮留的尾巴, 所以导致的长度不足, 那就继续接收
                            this._recvBuffer.Origin = 0;
                            data.CopyTo(this._recvBuffer.AsMemory(0, len));
                            this._recvBuffer.Origin += len;
                            break;
                        }

                        throw new ParseMessageException($"Message is fragmentary {len}");
                    }

                    // 能到这里, 说明之前的包已经读完了, 这是一个新的包
                    // 得到这个包体总共多大
                    var totalBodySize = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(data.Slice(0, this.BodySizeBits).Span));
                    if (totalBodySize <= 0) {
                        throw new ParseMessageException($"Invalid Head Message Size '{totalBodySize}'");
                    }

                    var messageTotalSize = totalBodySize + this.TotalHeadBits;
                    if (len < messageTotalSize) {
                        if (this._recvBuffer.Origin != 0) {
                            // origin不等于0, 说明上次读完整条message后, 还留了尾巴没处理完, 现在这条尾巴也并是完整的message, 所以把这条尾巴拷贝到buffer的开始位置
                            // 之所以这么处理, 是为了能避免多少拷贝数据, 就尽量避免多少拷贝数据.
                            this._recvBuffer.Origin = 0;
                            data.CopyTo(this._recvBuffer.AsMemory(0, len));
                        }

                        // 说明当前包没接收完, 暂时保留这些数据, 待接收完整再抛出去
                        this._recvBuffer.Origin += len;
                        this._completeMessageSize = messageTotalSize; // 用point来记录该包的完整大小
                    }
                    else {
                        // 读完了, 抛出事件
                        try {
                            var opcode = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(data.Slice(this.BodySizeBits, this.OpcodeBits).Span));
                            this.OnMessageReaded?.Invoke(opcode, data.Slice(this.TotalHeadBits, totalBodySize));
                        }
                        catch (Exception exception) {
                            Log.Error(exception);
                        }

                        // 还有剩余的数据, 说明剩余的数据流里面还含有新的包, 再次处理
                        if (len > messageTotalSize) {
                            data = data.Slice(messageTotalSize);
                            this._recvBuffer.Origin += messageTotalSize;
                            continue;
                        }
                    }
                }
                else {
                    // 之前的包没读完, 继续读之前的包
                    if (this._recvBuffer.Origin + len < this._completeMessageSize) {
                        // 还是没读完, 继续缓存
                        this._recvBuffer.Origin += len;
                    }
                    else {
                        // 之前的包读完了, 抛出事件
                        var origin = this._recvBuffer.Origin;
                        int lacking = 0;
                        try {
                            // 算出还缺多少数据
                            lacking = this._completeMessageSize - this._recvBuffer.Origin;
                            var totalBodySize = this._completeMessageSize - this.TotalHeadBits;
                            this._recvBuffer.Origin = 0;
                            this._completeMessageSize = 0;
                            var opcode = Unsafe.ReadUnaligned<ushort>(
                                ref MemoryMarshal.GetReference(this._recvBuffer.AsMemory(this.BodySizeBits, this.OpcodeBits).Span));
                            this.OnMessageReaded?.Invoke(opcode, this._recvBuffer.AsMemory(this.TotalHeadBits, totalBodySize));
                        }
                        catch (Exception exception) {
                            Log.Error(exception);
                        }

                        // 还有剩余的数据, 则再次处理
                        if (len > lacking) {
                            data = data.Slice(lacking);
                            // 把origin依然设置回原来的, 在下次whine循环的时候, 就可以判断是否需要把这个尾巴拷贝到buffer的开始位置
                            // origin设置成其他值也行, 只要不是0, 就行了
                            this._recvBuffer.Origin = origin;
                            continue;
                        }
                    }
                }

                break;
            }
        }

        public virtual void Reset() {
            this.OnMessageReaded = null;
            this._recvBuffer?.Reset();
            this._completeMessageSize = 0;
        }

        public virtual void Dispose() {
            this.Reset();
            this._recvBuffer = null;
        }
    }
}