namespace Hsenl {
    public static class FQuaternionEx {
        /// <summary>
        /// Calculates difference from this quaternion to given target quaternion. I.e. if you have quaternions Q1 and Q2,
        /// this method will return quaternion Q such that Q2 == Q * Q1 (remember that quaternions are multiplied right-to-left).
        /// </summary>
        public static Quaternion DeltaTo(this Quaternion quaternion, Quaternion target) {
            Quaternion.Inverse(ref quaternion, out var inv);
            Quaternion.Multiply(ref target, ref inv, out var result);
            return result;
        }
    }
}