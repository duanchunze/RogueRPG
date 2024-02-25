using System.Runtime.CompilerServices;

namespace Hsenl {
    public static class HashcodeHelper {
        private const uint Prime1 = 2654435761U;
        private const uint Prime2 = 2246822519U;
        private const uint Prime3 = 3266489917U;
        private const uint Prime4 = 668265263U;
        private const uint Prime5 = 374761393U;

        private static readonly uint s_seed = GenerateGlobalSeed();

        private static uint GenerateGlobalSeed() {
            return 0721775;
        }

        public static uint Combine<T1, T2>(T1 value1, T2 value2) {
            uint hc1 = (uint)(value1?.GetHashCode() ?? 0);
            uint hc2 = (uint)(value2?.GetHashCode() ?? 0);

            uint hash = s_seed + Prime5;
            hash += 8;

            hash = QueueRound(hash, hc1);
            hash = QueueRound(hash, hc2);

            hash = MixFinal(hash);
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint QueueRound(uint hash, uint queuedValue) {
            var value = hash + queuedValue * Prime3;
            var offset = 17;
            var rotateLeft = (value << offset) | (value >> (32 - offset));
            return rotateLeft * Prime4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixFinal(uint hash) {
            hash ^= hash >> 15;
            hash *= Prime2;
            hash ^= hash >> 13;
            hash *= Prime3;
            hash ^= hash >> 16;
            return hash;
        }
    }
}