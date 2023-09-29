#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    internal struct Float6 {
        private FLOAT _0;

        private FLOAT _1;

        private FLOAT _2;

        private FLOAT _3;

        private FLOAT _4;

        private FLOAT _5;

        public FLOAT this[int i] {
            get {
                return i switch {
                    0 => this._0,
                    1 => this._1,
                    2 => this._2,
                    3 => this._3,
                    4 => this._4,
                    5 => this._5,
                    _ => 0f,
                };
            }
            set {
                switch (i) {
                    case 0:
                        this._0 = value;
                        break;
                    case 1:
                        this._1 = value;
                        break;
                    case 2:
                        this._2 = value;
                        break;
                    case 3:
                        this._3 = value;
                        break;
                    case 4:
                        this._4 = value;
                        break;
                    case 5:
                        this._5 = value;
                        break;
                }
            }
        }
    }
}