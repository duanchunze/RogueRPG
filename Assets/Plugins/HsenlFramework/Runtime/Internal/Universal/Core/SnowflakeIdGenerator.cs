using System;

namespace Hsenl {
    public class SnowflakeIdGenerator {
        public static SnowflakeIdGenerator Instance;
        
        private const long Twepoch = 1288834974657L; // 起始时间戳 (2010-04-25 00:00:00 GMT)

        private const int WorkerIdBits = 5; // 机器ID所占的位数
        private const int DatacenterIdBits = 5; // 数据中心ID所占的位数
        private const int SequenceBits = 12; // 序列号所占的位数

        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits); // 最大机器ID
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits); // 最大数据中心ID
        private const long MaxSequence = -1L ^ (-1L << SequenceBits); // 最大序列号

        private readonly long workerId;
        private readonly long datacenterId;
        private long sequence = MaxSequence;

        private long lastTimestamp = -1L;

        public static long GenerateId() {
            Instance ??= new SnowflakeIdGenerator(0, 0);
            return Instance.NextId();
        }

        // 例如我有三个数据中心, 每个数据中心五个机子, 可以填(0-4, 0-2)
        public SnowflakeIdGenerator(long workerId, long datacenterId) {
            if (workerId > MaxWorkerId || workerId < 0) {
                throw new ArgumentException($"Worker ID can't be greater than {MaxWorkerId} or less than 0");
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0) {
                throw new ArgumentException($"Datacenter ID can't be greater than {MaxDatacenterId} or less than 0");
            }

            this.workerId = workerId;
            this.datacenterId = datacenterId;
        }

        public long NextId() {
            long timestamp = this.GetTimestamp();
            if (timestamp < this.lastTimestamp) {
                throw new Exception($"Clock moved backwards. Refusing to generate id for {this.lastTimestamp - timestamp} milliseconds");
            }

            if (this.lastTimestamp == timestamp) {
                this.sequence = (this.sequence + 1) & MaxSequence;
                if (this.sequence == 0) {
                    timestamp = this.WaitNextMillis(this.lastTimestamp);
                }
            }
            else {
                this.sequence = 0;
            }

            this.lastTimestamp = timestamp;

            long id = ((timestamp - Twepoch) << 22) | (this.datacenterId << 17) | (this.workerId << 12) | this.sequence;
            return id;
        }

        protected long GetTimestamp() {
            return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond - Twepoch;
        }

        protected long WaitNextMillis(long lastTimetamp) {
            long timestamp = this.GetTimestamp();
            while (timestamp <= lastTimetamp) {
                timestamp = this.GetTimestamp();
            }

            return timestamp;
        }
    }
}