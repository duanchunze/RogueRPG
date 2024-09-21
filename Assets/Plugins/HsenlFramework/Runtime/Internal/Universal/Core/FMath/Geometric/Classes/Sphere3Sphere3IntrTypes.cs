namespace Hsenl {
    public enum Sphere3Sphere3IntrTypes {
        /// <summary>
        /// Spheres are disjoint/separated
        /// </summary>
        Empty,

        /// <summary>
        /// Spheres touch at point, each sphere outside the other
        /// </summary>
        Point,

        /// <summary>
        /// Spheres intersect in a circle
        /// </summary>
        Circle,

        /// <summary>
        /// Sphere0 strictly contained in sphere1
        /// </summary>
        Sphere0,

        /// <summary>
        /// Sphere0 contained in sphere1, share common point
        /// </summary>
        Sphere0Point,

        /// <summary>
        /// Sphere1 strictly contained in sphere0
        /// </summary>
        Sphere1,

        /// <summary>
        /// Sphere1 contained in sphere0, share common point
        /// </summary>
        Sphere1Point,

        /// <summary>
        /// Spheres are the same
        /// </summary>
        Same
    }
}