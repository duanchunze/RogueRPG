#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    public class OdeRungeKutta4 : OdeSolver {
        private FLOAT _halfStep;

        private FLOAT _sixthStep;

        private FLOAT[] _temp1;

        private FLOAT[] _temp2;

        private FLOAT[] _temp3;

        private FLOAT[] _temp4;

        private FLOAT[] _yTemp;

        public override FLOAT step {
            get { return base.step; }
            set {
                this._step = value;
                this._halfStep = this._step * 0.5f;
                this._sixthStep = this._step / 6f;
            }
        }

        public OdeRungeKutta4(int dim, FLOAT step, OdeFunction function) : base(dim, step, function) {
            this._halfStep = 0.5f * step;
            this._sixthStep = step / 6f;
            this._temp1 = new FLOAT[this._dim];
            this._temp2 = new FLOAT[this._dim];
            this._temp3 = new FLOAT[this._dim];
            this._temp4 = new FLOAT[this._dim];
            this._yTemp = new FLOAT[this._dim];
        }

        public override void Update(FLOAT tIn, FLOAT[] yIn, ref FLOAT tOut, FLOAT[] yOut) {
            this._function(tIn, yIn, this._temp1);
            for (int i = 0; i < this._dim; i++) {
                this._yTemp[i] = yIn[i] + this._halfStep * this._temp1[i];
            }

            FLOAT t = tIn + this._halfStep;
            this._function(t, this._yTemp, this._temp2);
            for (int i = 0; i < this._dim; i++) {
                this._yTemp[i] = yIn[i] + this._halfStep * this._temp2[i];
            }

            this._function(t, this._yTemp, this._temp3);
            for (int i = 0; i < this._dim; i++) {
                this._yTemp[i] = yIn[i] + this._step * this._temp3[i];
            }

            tOut = tIn + this._step;
            this._function(tOut, this._yTemp, this._temp4);
            for (int i = 0; i < this._dim; i++) {
                yOut[i] = yIn[i] + this._sixthStep * (this._temp1[i] + 2f * (this._temp2[i] + this._temp3[i]) + this._temp4[i]);
            }
        }
    }
}