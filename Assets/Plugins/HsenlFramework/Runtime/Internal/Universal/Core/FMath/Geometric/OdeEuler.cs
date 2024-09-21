#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public class OdeEuler : OdeSolver {
        public OdeEuler(int dim, FLOAT step, OdeFunction function) : base(dim, step, function) { }

        public override void Update(FLOAT tIn, FLOAT[] yIn, ref FLOAT tOut, FLOAT[] yOut) {
            this._function(tIn, yIn, this._FValue);
            for (int i = 0; i < this._dim; i++) {
                yOut[i] = yIn[i] + this._step * this._FValue[i];
            }

            tOut = tIn + this._step;
        }
    }
}