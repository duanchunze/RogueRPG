#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public class OdeMidpoint : OdeSolver {
        private FLOAT _halfStep;

        private FLOAT[] _yTemp;

        public override FLOAT step {
            get { return base.step; }
            set {
                this._step = value;
                this._halfStep = this._step * 0.5f;
            }
        }

        public OdeMidpoint(int dim, FLOAT step, OdeFunction function) : base(dim, step, function) {
            this._halfStep = this._step * 0.5f;
            this._yTemp = new FLOAT[this._dim];
        }

        public override void Update(FLOAT tIn, FLOAT[] yIn, ref FLOAT tOut, FLOAT[] yOut) {
            this._function(tIn, yIn, this._FValue);
            for (int i = 0; i < this._dim; i++) {
                this._yTemp[i] = yIn[i] + this._halfStep * this._FValue[i];
            }

            FLOAT t = tIn + this._halfStep;
            this._function(t, this._yTemp, this._FValue);
            for (int i = 0; i < this._dim; i++) {
                yOut[i] = yIn[i] + this._step * this._FValue[i];
            }

            tOut = tIn + this._step;
        }
    }
}