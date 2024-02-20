using FixedMath;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public class FixBoxShape : FixShape {
        public override FittingDegreeType fittingDegreeType => FittingDegreeType.Perfect;

        public FixBoxShape(FLOAT xyz) {
            var size = new FVector3(xyz);
            Init(ref size);
        }

        public FixBoxShape(FVector3 size) {
            Init(ref size);
        }

        public FixBoxShape(FLOAT length, FLOAT width, FLOAT height) {
            var size = new FVector3(length, height, width);
            Init(ref size);
        }

        public FixBoxShape(FVector3 center, FVector3 size) {
            Init(ref center, ref size);
        }

        public void Init(ref FVector3 size) {
            FVector3.Multiply(ref size, Fixp.Half, out var halfSize);
            this._boundingBox = new AABB(-halfSize, halfSize);
            this._box = new Box(FVector3.Zero, FVector3.Right, FVector3.Up, FVector3.Forward, halfSize);
        }

        public void Init(ref FVector3 center, ref FVector3 size) {
            FVector3.Multiply(ref size, Fixp.Half, out var halfSize);
            FVector3.Subtract(ref center, ref halfSize, out var min);
            FVector3.Add(ref halfSize, ref center, out var max);
            this._boundingBox = new AABB(min, max);
            this._box = new Box(center, FVector3.Right, FVector3.Up, FVector3.Forward, halfSize);
        }

        public override void SupportPoint(ref FVector3 direction, out FVector3 point) {
            // var signX = FixMath.Abs(direction.x) < Fixp.Epsilon ? 0 : FixMath.Sign(direction.x);
            // var signY = FixMath.Abs(direction.y) < Fixp.Epsilon ? 0 : FixMath.Sign(direction.y);
            // var signZ = FixMath.Abs(direction.z) < Fixp.Epsilon ? 0 : FixMath.Sign(direction.z);
            // point.x = signX * _box.extents.x;
            // point.y = signY * _box.extents.y;
            // point.z = signZ * _box.extents.z;
            point.x = FMath.Sign(direction.x) * _box.extents.x;
            point.y = FMath.Sign(direction.y) * _box.extents.y;
            point.z = FMath.Sign(direction.z) * _box.extents.z;
        }
    }
}