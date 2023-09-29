using System.Collections.Generic;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// The sphere is represented as |X-C| = R where C is the center and R is
    /// the radius.
    /// </summary>
    public struct Sphere {
        // private static readonly FLOAT _4div3mulPi = 4.18879032f;

        /// <summary>
        /// Circle center
        /// </summary>
        public FVector3 center;

        /// <summary>
        /// Circle radius
        /// </summary>
        public FLOAT radius;

        /// <summary>
        /// Creates Sphere3 from center and radius
        /// </summary>
        public Sphere(ref FVector3 center, FLOAT radius) {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>
        /// Creates Sphere3 from center and radius
        /// </summary>
        public Sphere(FVector3 center, FLOAT radius) {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>
        /// Computes bounding sphere from a set of points.
        /// First compute the axis-aligned bounding box of the points, then compute the sphere containing the box.
        /// If a set is empty returns new Sphere3().
        /// </summary>
        public static Sphere CreateFromPointsAAB(IEnumerable<FVector3> points) {
            IEnumerator<FVector3> enumerator = points.GetEnumerator();
            enumerator.Reset();
            if (!enumerator.MoveNext()) {
                return default(Sphere);
            }

            AABB.CreateFromPoints(points).CalcCenterExtents(out var center, out var extents);
            Sphere result = default(Sphere);
            result.center = center;
            result.radius = extents.magnitude;
            return result;
        }

        /// <summary>
        /// Computes bounding sphere from a set of points.
        /// First compute the axis-aligned bounding box of the points, then compute the sphere containing the box.
        /// If a set is empty returns new Sphere3().
        /// </summary>
        public static Sphere CreateFromPointsAAB(IList<FVector3> points) {
            if (points.Count == 0) {
                return default(Sphere);
            }

            AABB.CreateFromPoints(points).CalcCenterExtents(out var center, out var extents);
            Sphere result = default(Sphere);
            result.center = center;
            result.radius = extents.magnitude;
            return result;
        }

        /// <summary>
        /// Computes bounding sphere from a set of points.
        /// Compute the smallest sphere whose center is the average of a point set.
        /// If a set is empty returns new Sphere3().
        /// </summary>
        public static Sphere CreateFromPointsAverage(IEnumerable<FVector3> points) {
            IEnumerator<FVector3> enumerator = points.GetEnumerator();
            enumerator.Reset();
            if (!enumerator.MoveNext()) {
                return default(Sphere);
            }

            FVector3 current = enumerator.Current;
            int num = 1;
            while (enumerator.MoveNext()) {
                current += enumerator.Current;
                num++;
            }

            current /= (FLOAT)num;
            FLOAT num2 = 0f;
            foreach (FVector3 point in points) {
                FLOAT sqrMagnitude = (point - current).sqrMagnitude;
                if (sqrMagnitude > num2) {
                    num2 = sqrMagnitude;
                }
            }

            Sphere result = default(Sphere);
            result.center = current;
            result.radius = FMath.Sqrt(num2);
            return result;
        }

        /// <summary>
        /// Computes bounding sphere from a set of points.
        /// Compute the smallest sphere whose center is the average of a point set.
        /// If a set is empty returns new Sphere3().
        /// </summary>
        public static Sphere CreateFromPointsAverage(IList<FVector3> points) {
            int count = points.Count;
            if (count == 0) {
                return default(Sphere);
            }

            FVector3 vector = points[0];
            for (int i = 1; i < count; i++) {
                vector += points[i];
            }

            vector /= (FLOAT)count;
            FLOAT num = 0f;
            for (int j = 0; j < count; j++) {
                FLOAT sqrMagnitude = (points[j] - vector).sqrMagnitude;
                if (sqrMagnitude > num) {
                    num = sqrMagnitude;
                }
            }

            Sphere result = default(Sphere);
            result.center = vector;
            result.radius = FMath.Sqrt(num);
            return result;
        }

        /// <summary>
        /// Creates sphere which is circumscribed around tetrahedron.
        /// Returns 'true' if sphere has been constructed, 'false' otherwise (input points are linearly dependent).
        /// </summary>
        public static bool CreateCircumscribed(FVector3 v0, FVector3 v1, FVector3 v2, FVector3 v3,
            out Sphere sphere) {
            FVector3 vector = v1 - v0;
            FVector3 vector2 = v2 - v0;
            FVector3 vector3 = v3 - v0;
            FLOAT[,] a = new FLOAT[3, 3] {
                { vector.x, vector.y, vector.z }, { vector2.x, vector2.y, vector2.z }, { vector3.x, vector3.y, vector3.z }
            };
            FLOAT[] b = new FLOAT[3] { 0.5f * vector.sqrMagnitude, 0.5f * vector2.sqrMagnitude, 0.5f * vector3.sqrMagnitude };
            if (LinearSystem.Solve3(a, b, out FVector3 X, 1E-05f)) {
                sphere.center = v0 + X;
                sphere.radius = X.magnitude;
                return true;
            }

            sphere = default(Sphere);
            return false;
        }

        /// <summary>
        /// Creates sphere which is insribed into tetrahedron.
        /// Returns 'true' if sphere has been constructed, 'false' otherwise (input points are linearly dependent).
        /// </summary>
        public static bool CreateInscribed(FVector3 v0, FVector3 v1, FVector3 v2, FVector3 v3,
            out Sphere sphere) {
            FVector3 vector = v1 - v0;
            FVector3 vector2 = v2 - v0;
            FVector3 vector3 = v3 - v0;
            FVector3 value = v2 - v1;
            FVector3 vector4 = v3 - v1;
            FVector3 vector5 = vector4.Cross(value);
            FVector3 vector6 = vector2.Cross(vector3);
            FVector3 vector7 = vector3.Cross(vector);
            FVector3 vector8 = vector.Cross(vector2);
            if (FMath.Abs(FVector3.Normalize(ref vector5)) < 1E-05f) {
                sphere = default(Sphere);
                return false;
            }

            if (FMath.Abs(FVector3.Normalize(ref vector6)) < 1E-05f) {
                sphere = default(Sphere);
                return false;
            }

            if (FMath.Abs(FVector3.Normalize(ref vector7)) < 1E-05f) {
                sphere = default(Sphere);
                return false;
            }

            if (FMath.Abs(FVector3.Normalize(ref vector8)) < 1E-05f) {
                sphere = default(Sphere);
                return false;
            }

            FLOAT[,] a = new FLOAT[3, 3] {
                { vector6.x - vector5.x, vector6.y - vector5.y, vector6.z - vector5.z },
                { vector7.x - vector5.x, vector7.y - vector5.y, vector7.z - vector5.z },
                { vector8.x - vector5.x, vector8.y - vector5.y, vector8.z - vector5.z }
            };
            FLOAT[] b = new FLOAT[3] { 0f, 0f, 0f - vector8.Dot(vector3) };
            if (LinearSystem.Solve3(a, b, out FVector3 X, 1E-05f)) {
                sphere.center = v3 + X;
                sphere.radius = FMath.Abs(vector5.Dot(X));
                return true;
            }

            sphere = default(Sphere);
            return false;
        }

        /// <summary>
        /// Returns sphere area
        /// </summary>
        public FLOAT CalcArea() {
            return (FLOAT)FMath.Pi * 4f * this.radius * this.radius;
        }

        /// <summary>
        /// Returns sphere volume
        /// </summary>
        public FLOAT CalcVolume() {
            return 4.18879032f * this.radius * this.radius * this.radius;
        }

        /// <summary>
        /// Evaluates sphere using formula X = C + R*[cos(theta)*sin(phi) , sin(theta)*sin(phi) , cos(phi)],
        /// where 0 &lt;= theta,phi &lt; 2*pi.
        /// </summary>
        public FVector3 Eval(FLOAT theta, FLOAT phi) {
            FLOAT num = FMath.Sin(phi);
            return new FVector3(this.center.x + this.radius * FMath.Cos(theta) * num, this.center.y + this.radius * FMath.Sin(theta) * num,
                this.center.z + this.radius * FMath.Cos(phi));
        }

        /// <summary>
        /// Returns distance to a point, distance is &gt;= 0f.
        /// </summary>
        public FLOAT DistanceTo(FVector3 point) {
            return Distance.Point3Sphere3(ref point, ref this);
        }

        /// <summary>
        /// Returns projected point
        /// </summary>
        public FVector3 Project(FVector3 point) {
            Distance.SqrPoint3Sphere3(ref point, ref this, out var closestPoint);
            return closestPoint;
        }

        /// <summary>
        /// Tests whether a point is contained by the sphere
        /// </summary>
        public bool Contains(ref FVector3 point) {
            return (point - this.center).sqrMagnitude <= this.radius * this.radius;
        }

        /// <summary>
        /// Tests whether a point is contained by the sphere
        /// </summary>
        public bool Contains(FVector3 point) {
            return (point - this.center).sqrMagnitude <= this.radius * this.radius;
        }

        /// <summary>
        /// Enlarges the sphere so it includes another sphere.
        /// </summary>
        public void Include(ref Sphere sphere) {
            FVector3 vector = sphere.center - this.center;
            FLOAT sqrMagnitude = vector.sqrMagnitude;
            FLOAT num = sphere.radius - this.radius;
            FLOAT num2 = num * num;
            if (num2 >= sqrMagnitude) {
                if (num >= 0f) {
                    this = sphere;
                }

                return;
            }

            FLOAT num3 = FMath.Sqrt(sqrMagnitude);
            if (num3 > 1E-05f) {
                FLOAT num4 = (num3 + num) / (2f * num3);
                this.center += num4 * vector;
            }

            this.radius = 0.5f * (num3 + this.radius + sphere.radius);
        }

        /// <summary>
        /// Enlarges the sphere so it includes another sphere.
        /// </summary>
        public void Include(Sphere sphere) {
            this.Include(ref sphere);
        }

        /// <summary>
        /// Returns string representation.
        /// </summary>
        public override string ToString() {
            return $"[Center: {this.center.ToString()} Radius: {this.radius.ToString()}]";
        }
    }
}