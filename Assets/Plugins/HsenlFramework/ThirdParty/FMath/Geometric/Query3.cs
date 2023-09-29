#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    internal class Query3 : Query {
        private static FLOAT _zero = 0;

        private FVector3[] _vertices;

        public Query3(FVector3[] vertices) {
            this._vertices = vertices;
        }

        public int ToPlane(int i, int v0, int v1, int v2) {
            return this.ToPlane(ref this._vertices[i], v0, v1, v2);
        }

        public int ToPlane(ref FVector3 test, int v0, int v1, int v2) {
            bool flag = Sort(ref v0, ref v1, ref v2);
            FVector3 vector = this._vertices[v0];
            FVector3 vector2 = this._vertices[v1];
            FVector3 vector3 = this._vertices[v2];
            FLOAT x = test.x - vector.x;
            FLOAT y = test.y - vector.y;
            FLOAT z = test.z - vector.z;
            FLOAT x2 = vector2.x - vector.x;
            FLOAT y2 = vector2.y - vector.y;
            FLOAT z2 = vector2.z - vector.z;
            FLOAT x3 = vector3.x - vector.x;
            FLOAT y3 = vector3.y - vector.y;
            FLOAT z3 = vector3.z - vector.z;
            FLOAT num = this.Det3(x, y, z, x2, y2, z2, x3, y3, z3);

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

        public int ToTetrahedron(int i, int v0, int v1, int v2, int v3) {
            return this.ToTetrahedron(ref this._vertices[i], v0, v1, v2, v3);
        }

        public int ToTetrahedron(ref FVector3 test, int v0, int v1, int v2, int v3) {
            int num = this.ToPlane(ref test, v1, v2, v3);
            if (num > 0) {
                return 1;
            }

            int num2 = this.ToPlane(ref test, v0, v2, v3);
            if (num2 < 0) {
                return 1;
            }

            int num3 = this.ToPlane(ref test, v0, v1, v3);
            if (num3 > 0) {
                return 1;
            }

            int num4 = this.ToPlane(ref test, v0, v1, v2);
            if (num4 < 0) {
                return 1;
            }

            if (num == 0 || num2 == 0 || num3 == 0 || num4 == 0) {
                return 0;
            }

            return -1;
        }

        public int ToCircumsphere(int i, int v0, int v1, int v2, int v3) {
            return this.ToCircumsphere(ref this._vertices[i], v0, v1, v2, v3);
        }

        public int ToCircumsphere(ref FVector3 test, int v0, int v1, int v2, int v3) {
            bool flag = Sort(ref v0, ref v1, ref v2, ref v3);
            FVector3 vector = this._vertices[v0];
            FVector3 vector2 = this._vertices[v1];
            FVector3 vector3 = this._vertices[v2];
            FVector3 vector4 = this._vertices[v3];
            FLOAT num = vector.x + test.x;
            FLOAT num2 = vector.x - test.x;
            FLOAT num3 = vector.y + test.y;
            FLOAT num4 = vector.y - test.y;
            FLOAT num5 = vector.z + test.z;
            FLOAT num6 = vector.z - test.z;
            FLOAT num7 = vector2.x + test.x;
            FLOAT num8 = vector2.x - test.x;
            FLOAT num9 = vector2.y + test.y;
            FLOAT num10 = vector2.y - test.y;
            FLOAT num11 = vector2.z + test.z;
            FLOAT num12 = vector2.z - test.z;
            FLOAT num13 = vector3.x + test.x;
            FLOAT num14 = vector3.x - test.x;
            FLOAT num15 = vector3.y + test.y;
            FLOAT num16 = vector3.y - test.y;
            FLOAT num17 = vector3.z + test.z;
            FLOAT num18 = vector3.z - test.z;
            FLOAT num19 = vector4.x + test.x;
            FLOAT num20 = vector4.x - test.x;
            FLOAT num21 = vector4.y + test.y;
            FLOAT num22 = vector4.y - test.y;
            FLOAT num23 = vector4.z + test.z;
            FLOAT num24 = vector4.z - test.z;
            FLOAT w = num * num2 + num3 * num4 + num5 * num6;
            FLOAT w2 = num7 * num8 + num9 * num10 + num11 * num12;
            FLOAT w3 = num13 * num14 + num15 * num16 + num17 * num18;
            FLOAT w4 = num19 * num20 + num21 * num22 + num23 * num24;
            FLOAT num25 = this.Det4(num2, num4, num6, w, num8, num10, num12, w2, num14, num16, num18, w3, num20, num22, num24,
                w4);
            if (!flag) {
                num25 = 0f - num25;
            }

            if (!(num25 > _zero)) {
                if (!(num25 < _zero)) {
                    return 0;
                }

                return -1;
            }

            return 1;
        }

        public FLOAT Dot(FLOAT x0, FLOAT y0, FLOAT z0, FLOAT x1, FLOAT y1, FLOAT z1) {
            return x0 * x1 + y0 * y1 + z0 * z1;
        }

        public FLOAT Det3(FLOAT x0, FLOAT y0, FLOAT z0, FLOAT x1, FLOAT y1, FLOAT z1, FLOAT x2, FLOAT y2, FLOAT z2) {
            FLOAT num = y1 * z2 - y2 * z1;
            FLOAT num2 = y2 * z0 - y0 * z2;
            FLOAT num3 = y0 * z1 - y1 * z0;
            return x0 * num + x1 * num2 + x2 * num3;
        }

        public FLOAT Det4(FLOAT x0, FLOAT y0, FLOAT z0, FLOAT w0, FLOAT x1, FLOAT y1, FLOAT z1, FLOAT w1, FLOAT x2, FLOAT y2,
            FLOAT z2, FLOAT w2, FLOAT x3, FLOAT y3, FLOAT z3, FLOAT w3) {
            FLOAT num = x0 * y1 - x1 * y0;
            FLOAT num2 = x0 * y2 - x2 * y0;
            FLOAT num3 = x0 * y3 - x3 * y0;
            FLOAT num4 = x1 * y2 - x2 * y1;
            FLOAT num5 = x1 * y3 - x3 * y1;
            FLOAT num6 = x2 * y3 - x3 * y2;
            FLOAT num7 = z0 * w1 - z1 * w0;
            FLOAT num8 = z0 * w2 - z2 * w0;
            FLOAT num9 = z0 * w3 - z3 * w0;
            FLOAT num10 = z1 * w2 - z2 * w1;
            FLOAT num11 = z1 * w3 - z3 * w1;
            FLOAT num12 = z2 * w3 - z3 * w2;
            return num * num12 - num2 * num11 + num3 * num10 + num4 * num9 - num5 * num8 + num6 * num7;
        }
    }
}