namespace Hsenl {
    public static class BitHelper {
        public static int AlignDown(int value, int alignPow2) {
            return value & ~(alignPow2 - 1);
        }

        public static int AlignUp(int value, int alignPow2) {
            return AlignDown(value + alignPow2 - 1, alignPow2);
        }
    }
}