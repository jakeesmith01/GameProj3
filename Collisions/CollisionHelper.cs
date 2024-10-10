using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collisions
{
    public static class CollisionHelper
    {
        /// <summary>
        /// Determines if two BoundingCircles collide
        /// </summary>
        /// <param name="a">The first bounding circle</param>
        /// <param name="b">The second bounding circle</param>
        /// <returns>true for collision, false otherwise</returns>
        public static bool Collides(BoundingCircle a, BoundingCircle b)
        {
            return Math.Pow(a.Radius + b.Radius, 2) >= Math.Pow(a.Center.X - b.Center.X, 2) + Math.Pow(a.Center.Y - b.Center.Y, 2);
        }

        /// <summary>
        /// Checks if a BoundingTriangle is colliding with a BoundingCircle
        /// </summary>
        /// <param name="a">The BoundingTriangle</param>
        /// <param name="b">the BoundingCircle</param>
        /// <returns>true for collision, false otherwise</returns>
        public static bool Collides(BoundingTriangle a, BoundingCircle b){
            if(IsPointInTriangle(b.Center, a.Point1, a.Point2, a.Point3)){
                return true;
            }

            if(IsCircleIntersectingEdge(b, a.Point1, a.Point2) || 
                IsCircleIntersectingEdge(b, a.Point2, a.Point3) || 
                IsCircleIntersectingEdge(b, a.Point3, a.Point1))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if a point is within a triangle
        /// </summary>
        /// <param name="pt">The point to check</param>
        /// <param name="v1">The first vertex of the triangle</param>
        /// <param name="v2">The second vertex of the triangle</param>
        /// <param name="v3">The third vertex of the triangle</param>
        /// <returns>true for collision, false otherwise</returns>
        private static bool IsPointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float d1 = Sign(pt, v1, v2);
            float d2 = Sign(pt, v2, v3);
            float d3 = Sign(pt, v3, v1);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(hasNeg && hasPos);
        }

        /// <summary>
        /// Determines the signed area of the triangle
        /// </summary>
        /// <param name="p1">The first vertex of the triangle</param>
        /// <param name="p2">The second vertex of the triangle</param>
        /// <param name="p3">The third vertex of the triangle</param>
        /// <returns>A positive value means the points are in counter clockwise order, negative means clockwise, and 0 means collinear</returns>
        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        /// <summary>
        /// Determines if a circle is intersecting an edge
        /// </summary>
        /// <param name="circle">The bounding circle to check</param>
        /// <param name="v1">The first point of the edge</param>
        /// <param name="v2">The second point of the edge</param>
        /// <returns>true for collision, false otherwise</returns>
        private static bool IsCircleIntersectingEdge(BoundingCircle circle, Vector2 v1, Vector2 v2)
        {
            Vector2 edge = v2 - v1;
            Vector2 circleToV1 = circle.Center - v1;
            float edgeLengthSquared = edge.LengthSquared();
            float t = Math.Max(0, Math.Min(1, Vector2.Dot(circleToV1, edge) / edgeLengthSquared));
            Vector2 projection = v1 + t * edge;
            float distanceSquared = (circle.Center - projection).LengthSquared();

            return distanceSquared <= circle.Radius * circle.Radius;
        }

    }
}