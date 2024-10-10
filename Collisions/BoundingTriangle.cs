using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Collisions{
    public struct BoundingTriangle{
        /// <summary>
        /// The first point of the triangle
        /// </summary>
        public Vector2 Point1;

        /// <summary>
        /// The second point of the triangle
        /// </summary>
        public Vector2 Point2;

        /// <summary>
        /// The third point of the triangle
        /// </summary>
        public Vector2 Point3;

        public BoundingTriangle(Vector2 point1, Vector2 point2, Vector2 point3){
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
        }


        /// <summary>
        /// Determines if this BoundingTriangle collides with a BoundingCircle (in this case, a particle)
        /// </summary>
        /// <param name="other">The bounding circle to check for collisions with</param>
        /// <returns></returns>
        public bool CollidesWith(BoundingCircle other){
            return CollisionHelper.Collides(this, other);
        }

    }
}