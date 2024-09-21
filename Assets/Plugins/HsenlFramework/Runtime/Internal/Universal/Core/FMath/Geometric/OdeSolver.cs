#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public delegate void OdeFunction(FLOAT t, FLOAT[] y, FLOAT[] F);

    public abstract class OdeSolver {
        protected int _dim;

        protected FLOAT _step;

        protected OdeFunction _function;

        protected FLOAT[] _FValue;

        public virtual FLOAT step {
            get { return this._step; }
            set { this._step = value; }
        }

        public OdeSolver(int dim, FLOAT step, OdeFunction function) {
            this._dim = dim;
            this._step = step;
            this._function = function;
            this._FValue = new FLOAT[this._dim];
        }

        public abstract void Update(FLOAT tIn, FLOAT[] yIn, ref FLOAT tOut, FLOAT[] yOut);
    }
}