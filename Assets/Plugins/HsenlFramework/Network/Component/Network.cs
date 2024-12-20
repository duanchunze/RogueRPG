﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hsenl.Network {
    public abstract class Network : Component, IUpdate {
        private HBuffer _bufferWrite = new();
        private int _rpcId;
        private Dictionary<int, RpcInfo> _rpcInfos = new(); // key: rpcId

        private uint _rpcTimeOutTime = 5000;
        private long _currentTimeoutTimeMin;
        private Queue<int> _timeoutQueue = new();

        private ArrayPool<byte> _arrayPool = ArrayPool<byte>.Create();

        protected override void OnDestroy() {
            this._bufferWrite.Dispose();
            this._rpcId = 0;
            this._rpcInfos.Clear();
        }

        protected abstract Service Service { get; }

        public void Send<T>(T message, long channelId) {
            this.Write(message);
            this.Service.Write(channelId, this._bufferWrite.AsSpan(0, this._bufferWrite.Position));
            this.Service.StartSend(channelId);
        }

        public void SendWithRpcId<T>(T message, int rpcId, long channelId) {
            this.Write(message);
            this._bufferWrite.AsSpan(3, 4).WriteTo(rpcId);
            this.Service.Write(channelId, this._bufferWrite.AsSpan(0, this._bufferWrite.Position));
            this.Service.StartSend(channelId);
        }

        public RpcInfo Call<T>(T message, long channelId) {
            var rpcId = ++this._rpcId;
            this.SendWithRpcId(message, rpcId, channelId);
            var rpcInfo = new RpcInfo(HTask<Memory<byte>>.Create(), (uint)TimeInfo.Now, this._arrayPool);
            this._rpcInfos.Add(rpcId, rpcInfo);
            return rpcInfo;
        }

        private void Write<T>(T message) {
            var opcode = OpcodeLookupTable.GetOpcodeOfType(message.GetType());
            this._bufferWrite.Seek(0, SeekOrigin.Begin);
            this._bufferWrite.GetSpan(2).WriteTo(opcode);
            this._bufferWrite.Advance(2);
            SerializeHelper.SerializeOfMemoryPack(this._bufferWrite, message);
        }

        protected void OnRecvMessage(long channelId, Memory<byte> data) {
            var opcode = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(data.Slice(0, 2).Span));
            var message = data.Slice(2); // 消息本体
            if (OpcodeLookupTable.ContainsResponseOpcode(opcode)) {
                // 这是一个rpc的回复消息, 提取data1-4位的rpcId, 并交给rpcInfo处理
                var rpcId = BitConverter.ToInt32(message.Slice(1, 4).Span);
                if (this._rpcInfos.TryGetValue(rpcId, out var rpcInfo)) {
                    rpcInfo.Response(message);
                    this._rpcInfos.Remove(rpcId);
                }
            }
            else {
                // 这是普通消息, 则走消息分发路线, 把数据原封不动的发出去, 交给终端去反序列化
                var type = OpcodeLookupTable.GetTypeOfOpcode(opcode);
                MessageDispatcher.DispatcherMessage(type, message.Span, this, channelId);
            }
        }

        public void SetRpcTimeoutTime(uint time) {
            this._rpcTimeOutTime = time;
        }

        public void Update() {
            // 清除超时的rpc消息
            var now = TimeInfo.Now;
            if (now > this._currentTimeoutTimeMin) {
                this._timeoutQueue.Clear();
                var minTime = long.MaxValue;
                foreach (var kv in this._rpcInfos) {
                    var startTime = kv.Value.StartTime;
                    if (now > startTime + this._rpcTimeOutTime) {
                        Log.Info("Rpc timeout!");
                        this._timeoutQueue.Enqueue(kv.Key);
                        continue;
                    }

                    if (startTime < minTime) {
                        minTime = startTime;
                    }
                }

                while (this._timeoutQueue.Count > 0) {
                    this._rpcInfos.Remove(this._timeoutQueue.Dequeue());
                }

                this._currentTimeoutTimeMin = minTime;
            }
        }

        public struct RpcInfo {
            private HTask<Memory<byte>> _task;
            private ArrayPool<byte> _arrayPool;
            private byte[] _cache;

            public uint StartTime { get; }

            public RpcInfo(HTask<Memory<byte>> task, uint startTime) {
                this._task = task;
                this.StartTime = startTime;
                this._arrayPool = null;
                this._cache = default;
            }

            public RpcInfo(HTask<Memory<byte>> task, uint startTime, ArrayPool<byte> pool) {
                this._task = task;
                this.StartTime = startTime;
                this._arrayPool = pool;
                this._cache = default;
            }

            public async HTask<T> As<T>() {
                var memory = await this._task;
                var t = SerializeHelper.DeserializeOfMemoryPack<T>(memory.Span);
                return t;
            }

            // 同As<T>的唯一区别就是task会在主线程里被返回
            public async HTask<T> MainThreadAs<T>() {
                var memory = await this._task;
                this._cache = this._arrayPool.Rent(memory.Length);
                memory.CopyTo(this._cache.AsMemory());
                await HTask.ReturnToMainThread();
                var t = SerializeHelper.DeserializeOfMemoryPack<T>(this._cache);
                return t;
            }

            public void Response(Memory<byte> data) {
                this._task.SetResult(data);
            }

            public void Dispose() {
                this._task = default;
                if (this._cache != null) {
                    this._arrayPool.Return(this._cache);
                }

                this._arrayPool = null;
                this._cache = null;
            }
        }
    }
}