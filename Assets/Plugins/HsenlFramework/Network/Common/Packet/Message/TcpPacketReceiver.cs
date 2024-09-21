using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hsenl.Network {
    public class TcpPacketReceiver : IPacketRecvProvider, IMessageReader {
        private const int BodySizeBits = 4;
        private const int TotalHeadBits = 4;

        private readonly HBuffer _bufferRecv = new();
        private int _completeMessageSize; // 完整的消息大小, 该值如果不为0, 代表当前消息没有接收完

        public Action<Memory<byte>> OnMessageReaded { get; set; } // 当读取出了包

        public virtual void Init() { }

        public void Init<T>(T t) { }

        public byte[] GetRecvBuffer(int len, out int offset, out int count) {
            throw new NotImplementedException();
        }

        // 该方法和Read方法在实际运行过程中, 不会有冲突关系, 不会有同时接收又同时read的情况, 所以不需要加锁
        public virtual Memory<byte> GetRecvBuffer(int length) {
            this._bufferRecv.Advance(length);
            // 我们用于接收的和用于做粘包操作的, 其实是同一块内存
            return this._bufferRecv.AsMemory(0, length);
        }

        // 读包, 拆包
        // 每次read, 都会把整条recvBuffer中的数据全部读完
        public virtual void Read(Memory<byte> data) {
            while (true) {
                var len = data.Length;
                if (this._completeMessageSize == 0) {
                    if (len <= TotalHeadBits) {
                        if (this._bufferRecv.Origin != 0) {
                            // 这种情况是因为上一轮留的尾巴, 所以导致的长度不足, 那就继续接收
                            this._bufferRecv.Origin = 0;
                            // 这里发生了数据拷贝, 这是没法避免的, 理想的状态是像<<flappy bird>>中无限续杯的水管那样, 让两段内存通过移动来实现一个闭环, 但
                            // 实际过程中, 我们无法从两个数组来截取一个独立的span(比如有一个消息横跨两个buffer的时候), 虽然我们采用了动态拓展的方案, 让一个消息只会被读取在一个buffer上, 
                            // 但实际上还有一个情况, 无法避免, 就是这里这种情况, 例如现在有2个100k的消息, 而我们每次接收30k, 那么当第四次接收时, 必然会出现10k + 20k这种情况, 
                            // 10k是第一个消息的尾巴, 而20k是第二个游戏的开头, 这是无法避免的, 所以我们必须把20k的数据拷贝到buffer的开头.
                            // 尽管如此, 实际上也已经很大程度上避免了数据的拷贝了, 特别当消息大小远小于或者远大于接收缓冲区大小
                            // 的时候(设消息大小为a, 接收缓冲区大小为b, 那么|a - b|的值越大, 则避免拷贝的效果越好), 因此不建议把接收缓冲区的大小设置的和消息大小相近.
                            data.CopyTo(this._bufferRecv.AsMemory(0, len));
                            this._bufferRecv.Origin += len;
                            break;
                        }

                        throw new ParseMessageException($"Message is fragmentary {len}");
                    }

                    // 能到这里, 说明之前的包已经读完了, 这是一个新的包
                    // 得到这个包体总共多大
                    var totalBodySize = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(data.Slice(0, BodySizeBits).Span));
                    if (totalBodySize <= 0) {
                        throw new ParseMessageException($"Invalid Head Message Size '{totalBodySize}'");
                    }

                    var messageTotalSize = totalBodySize + TotalHeadBits;
                    if (len < messageTotalSize) {
                        if (this._bufferRecv.Origin != 0) {
                            // origin不等于0, 说明上次读完整条message后, 还留了尾巴没处理完, 现在这条尾巴也并不是完整的message, 所以把这条尾巴拷贝到buffer的开始位置,
                            // 之所以这么处理, 是为了能避免多少拷贝数据, 就尽量避免多少拷贝数据.
                            this._bufferRecv.Origin = 0;
                            data.CopyTo(this._bufferRecv.AsMemory(0, len));
                        }

                        // 说明当前包没接收完, 暂时保留这些数据, 待接收完整再抛出去
                        this._bufferRecv.Origin += len;
                        this._completeMessageSize = messageTotalSize; // 记录该包的完整大小
                    }
                    else {
                        // 读完了, 抛出事件
                        try {
                            // 没有发生粘包, 直接就读完了
                            this.OnMessageReaded?.Invoke(data.Slice(TotalHeadBits, totalBodySize));
                        }
                        catch (Exception exception) {
                            Log.Error(exception);
                        }

                        // 还有剩余的数据, 说明剩余的数据流里面还含有新的包, 再次处理
                        if (len > messageTotalSize) {
                            data = data.Slice(messageTotalSize);
                            this._bufferRecv.Origin += messageTotalSize;
                            continue;
                        }
                    }
                }
                else {
                    // 之前的包没读完, 继续读之前的包
                    if (this._bufferRecv.Origin + len < this._completeMessageSize) {
                        // 还是没读完, 继续缓存
                        this._bufferRecv.Origin += len;
                    }
                    else {
                        // 之前的包读完了, 抛出事件
                        var origin = this._bufferRecv.Origin;
                        int lacking = 0;
                        try {
                            // 算出还缺多少数据
                            lacking = this._completeMessageSize - this._bufferRecv.Origin;
                            var totalBodySize = this._completeMessageSize - TotalHeadBits;
                            this._bufferRecv.Origin = 0;
                            this._completeMessageSize = 0;
                            // 发生了粘包, 且已经粘包完毕了.
                            // 因为接收使用的始终是同一块内存, 所以实际上这里的data就是_recvBuffer这块内存上的一部分, 所以这里会从_recvBuffer中来拿出整个消息的数据.
                            // 
                            // 我们每次接收都是从_recvBuffer划出一段缓冲区用来直接接收数据, 例如_recvBuffer是一块1024的缓冲区, 第一次接收, 我把
                            // 0-128这部分拿出来用来接受数据, 如果需要粘包, 那就把origin设置在128位置, 下次再接收数据就会用128-256这部分接受, 依次类推, 直到粘包完毕, 
                            // 我们就可以把整条消息数据拿出来, 之后, 我们把origin再次设置为0, 这样, 下次再接收数据, 又会截取0-128这段内存来接收.
                            // 实际上, origin不一定设置为0, 因为粘包完毕时, 很可能有新的消息数据跟在后面, 如果设置为0, 那么这部分数据就会丢失, 所以下面会计算剩余的数据, 用以
                            // 修改origin的值
                            this.OnMessageReaded?.Invoke(this._bufferRecv.AsMemory(TotalHeadBits, totalBodySize));
                        }
                        catch (Exception exception) {
                            Log.Error(exception);
                        }

                        // 还有剩余的数据, 则再次处理
                        if (len > lacking) {
                            data = data.Slice(lacking);
                            // 把origin依然设置回原来的, 在下次whine循环的时候, 就可以判断是否需要把这个尾巴拷贝到buffer的开始位置
                            // origin设置成其他值也行, 只要不是0, 就行了
                            this._bufferRecv.Origin = origin;
                            continue;
                        }
                    }
                }

                break;
            }
        }

        public virtual void Dispose() {
            this.OnMessageReaded = null;
            this._bufferRecv?.Reset();
            this._completeMessageSize = 0;
        }
    }
}