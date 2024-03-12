using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class RcdDefaultCheckpointsAdventure : IRecord {
        public int totalCheckpoint;
        public int currentCheckpoint;
    }
}