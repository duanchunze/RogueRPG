#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    internal class Query2 : Query {
        private static FLOAT _zero = 0;

        private Vector2[] _vertices;

        public Query2(Vector2[] vertices) {
            this._vertices = vertices;
        }

        public int ToLine(int i, int v0, int v1) {
            return this.ToLine(ref this._vertices[i], v0, v1);
        }

        public int ToLine(ref Vector2 test, int v0, int v1) {
            bool flag = Sort(ref v0, ref v1);
            Vector2 vector = this._vertices[v0];
            Vector2 vector2 = this._vertices[v1];
            FLOAT x = test.x - vector.x;
            FLOAT y = test.y - vector.y;
            FLOAT x2 = vector2.x - vector.x;
            FLOAT y2 = vector2.y - vector.y;
            FLOAT num = this.Det2(x, y, x2, y2);
            if (!flag) {
                num = 0f - num;
            }

            if (!(num > _zero)) {
                if (!(num < _zero)) {
                    return 0;
                }

                return -1;
            }

            return 1;
        }

        public int ToTriangle(int i, int v0, int v1, int v2) {
            return this.ToTriangle(ref this._vertices[i], v0, v1, v2);
        }

        public int ToTriangle(ref Vector2 test, int v0, int v1, int v2) {
            int num = this.ToLine(ref test, v1, v2);
            if (num > 0) {
                return 1;
            }

            int num2 = this.ToLine(ref test, v0, v2);
            if (num2 < 0) {
                return 1;
            }

            int num3 = this.ToLine(ref test, v0, v1);
            if (num3 > 0) {
                return 1;
            }

            if (num == 0 || num2 == 0 || num3 == 0) {
                return 0;
            }

            return -1;
        }

        public int ToCircumcircle(int i, int v0, int v1, int v2) {
            return this.ToCircumcircle(ref this._vertices[i], v0, v1, v2);
        }

        public int ToCircumcircle(ref Vector2 test, int v0, int v1, int v2) {
            bool flag = Sort(ref v0, ref v1, ref v2);
            Vector2 vector = this._vertices[v0];
            Vector2 vector2 = this._vertices[v1];
            Vector2 vector3 = this._vertices[v2];
            FLOAT num = vector.x + test.x;
            FLOAT num2 = vector.x - test.x;
            FLOAT num3 = vector.y + test.y;
            FLOAT num4 = vector.y - test.y;
            FLOAT num5 = vector2.x + test.x;
            FLOAT num6 = vector2.x - test.x;
            FLOAT num7 = vector2.y + test.y;
            FLOAT num8 = vector2.y - test.y;
            FLOAT num9 = vector3.x + test.x;
            FLOAT num10 = vector3.x - test.x;
            FLOAT num11 = vector3.y + test.y;
            FLOAT num12 = vector3.y - test.y;
            FLOAT z = num * num2 + num3 * num4;
            FLOAT z2 = num5 * num6 + num7 * num8;
            FLOAT z3 = num9 * num10 + num11 * num12;
            FLOAT num13 = this.Det3(num2, num4, z, num6, num8, z2, num10, num12, z3);
            if (!flag) {
                num13 = 0f - num13;
            }

            if (!(num13 < _zero)) {
                if (!(num13 > _zero)) {
                    return 0;
                }

                return -1;
            }

            return 1;
        }

        public FLOAT Dot(FLOAT x0, FLOAT y0, FLOAT x1, FLOAT y1) {
            return x0 * x1 + y0 * y1;
        }

        public FLOAT Det2(FLOAT x0, FLOAT y0, FLOAT x1, FLOAT y1) {
            return x0 * y1 - x1 * y0;
        }

        public FLOAT Det3(FLOAT x0, FLOAT y0, FLOAT z0, FLOAT x1, FLOAT y1, FLOAT z1, FLOAT x2, FLOAT y2, FLOAT z2) {
            FLOAT num = y1 * z2 - y2 * z1;
            FLOAT num2 = y2 * z0 - y0 * z2;
            FLOAT num3 = y0 * z1 - y1 * z0;
            return x0 * num + x1 * num2 + x2 * num3;
        }
    }
}