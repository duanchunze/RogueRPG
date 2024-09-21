namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Sphere3 and Sphere3
    /// </summary>
    public struct Sphere3Sphere3Intr {
        /// <summary>
        /// Equals to:
        /// Sphere3Sphere3IntersectionTypes.Empty if no intersection occurs;
        /// Sphere3Sphere3IntersectionTypes.Point if spheres are touching in a point and outside of each other;
        /// Sphere3Sphere3IntersectionTypes.Circle is spheres intersect (common case);
        /// Sphere3Sphere3IntersectionTypes.Sphere0 or Sphere3Sphere3IntersectionTypes.Sphere1 if sphere0 is strictly contained inside sphere1, or
        /// sphere1 is strictly contained in sphere0 respectively;
        /// Sphere3Sphere3IntersectionTypes.Sphere0Point or Sphere3Sphere3IntersectionTypes.Sphere1Point if sphere0 is contained inside sphere1 and share common point or
        /// sphere1 is contained inside sphere0 and share common point;
        /// Sphere3Sphere3IntersectionTypes.Same if spheres are esssentialy the same.
        /// </summary>
        public Sphere3Sphere3IntrTypes IntersectionType;

        /// <summary>
        /// Circle of intersection in case of Sphere3Sphere3IntersectionTypes.Circle
        /// </summary>
        public Circle Circle;

        /// <summary>
        /// Contact point in case of Sphere3Sphere3IntersectionTypes.Point,
        /// Sphere3Sphere3IntersectionTypes.Sphere0Point, Sphere3Sphere3IntersectionTypes.Sphere1Point
        /// </summary>
        public Vector3 ContactPoint;
    }
}