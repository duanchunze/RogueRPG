using FixedMath;

namespace Hsenl {
    /// <summary>
    /// 包围盒
    /// </summary>
    public struct FixBoundingBox {
        public FVector3 min;
        public FVector3 max;
        public FVector3 center;

        public FVector3 axisX;
        public FVector3 axisY;
        public FVector3 axisZ;

        public FixBoundingBox(FVector3 min, FVector3 max) {
            this.min = min;
            this.max = max;
            this.center = (max - min) * Fixp.Half;

            this.axisX = FVector3.Right;
            this.axisY = FVector3.Up;
            this.axisZ = FVector3.Forward;
        }

        public override string ToString() {
            return $"{min} | {max}";
        }
    }
}