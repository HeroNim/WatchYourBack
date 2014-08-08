using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public static class CollisionHelper
    {
        /// <summary>
        /// Checks two entities for collisions, given that both have rectangle colliders.
        /// </summary>
        /// <param name="e1">The first entity</param>
        /// <param name="e2">The second entity</param>
        /// <returns>True if colliding</returns>
        public static bool checkRectangle_RectangleCollisions(Rectangle c1, Rectangle c2)
        {
            return c1.Intersects(c2);
        }

        /// <summary>
        /// Checks two entities for collisions, given that both have line colliders. 
        /// </summary>
        /// <param name="e1">The first entity</param>
        /// <param name="e2">The second entity</param>
        /// <returns>True if colliding</returns>
        public static bool checkLine_LineCollision(Line c1, Line c2)
        {           
            float result1 = lineEquation(c1.P1, c1.P2, c2.P1);
            float result2 = lineEquation(c1.P1, c1.P2, c2.P2);
            float result3 = lineEquation(c2.P1, c2.P2, c1.P1);
            float result4 = lineEquation(c2.P1, c2.P2, c1.P2);

            if (result1 * result2 > 0 || result3 * result4 > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks two entities for collisions, given the first has a line collider and the second a circle collider.
        /// </summary>
        /// <param name="e1">The first entity</param>
        /// <param name="e2">The second entity</param>
        /// <returns>True if colliding</returns>
        public static bool checkLine_CircleCollision(Line c1, Circle c2)
        {          
            Vector2 toCircle = new Vector2(c2.Center.X - c1.P2.X, c2.Center.Y - c1.P2.Y);
            if (toCircle.Length() <= c2.Radius)
                return true;

            Vector2 weaponCollider = new Vector2(c1.P1.X - c1.P2.X, c1.P1.Y - c1.P2.Y);
            float weaponColliderLength = weaponCollider.Length();

            Vector2 direction = weaponCollider;
            direction.Normalize();

            float sProjection = Vector2.Dot(toCircle, direction);
            Vector2 projectionPoint = direction * sProjection;

            toCircle = c1.P2 + projectionPoint;

            float test = Vector2.Dot(projectionPoint, weaponCollider);
            if (test <= 0 || test >= Math.Pow(weaponColliderLength, 2))
                return false;

            Vector2 perpVector = new Vector2(c2.Center.X - toCircle.X, c2.Center.Y - toCircle.Y);
            float length = perpVector.Length();

            if (length <= c2.Radius)
                return true;

            return false;
        }

        /// <summary>
        /// Checks two entities for collisions, given the first has a line collider and the second a rectangle collider.
        /// </summary>
        /// <param name="e1">The first entity</param>
        /// <param name="e2">The second entity</param>
        /// <returns>True if colliding</returns>
        public static bool checkLine_RectangleCollision(Line c1, Rectangle c2)
        {                   
            bool above = false;
            bool possibleIntersection = false;
            bool intersection = false;

            //Check if corners are all above or below the line

            Vector2 topLeft = new Vector2(c2.Left, c2.Top);
            Vector2 bottomLeft = new Vector2(c2.Left, c2.Bottom);
            Vector2 topRight = new Vector2(c2.Right, c2.Top);
            Vector2 bottomRight = new Vector2(c2.Right, c2.Bottom);

            float[] results = new float[4];

            results[0] = lineEquation(c1.P1, c1.P2, topLeft);
            results[1] = lineEquation(c1.P1, c1.P2, bottomLeft);
            results[2] = lineEquation(c1.P1, c1.P2, topRight);
            results[3] = lineEquation(c1.P1, c1.P2, bottomRight);

            if (results[0] > 0)
                above = true;

            for (int i = 1; i < results.Length; i++)
            {
                if (above)
                {
                    if (results[i] <= 0)
                        possibleIntersection = true;
                }
                else if (above == false)
                    if (results[i] >= 0)
                        possibleIntersection = true;
            }
            if (possibleIntersection == false)
                intersection = false;
            else //Check that at least one coordinate of the line on both axis' is contained within the rectangle
            {
                if (c1.X1 > bottomRight.X && c1.X2 > bottomRight.X)
                    intersection = false;
                else if (c1.X1 < topLeft.X && c1.X2 < topLeft.X)
                    intersection = false;
                else if (c1.Y1 > bottomRight.Y && c1.Y2 > bottomRight.Y)
                    intersection = false;
                else if (c1.Y1 < topLeft.Y && c1.Y2 < topLeft.Y)
                    intersection = false;
                else
                    intersection = true;
            }           
            return intersection;
        }

        /// <summary>
        /// Checks if a given point is contained above, below, or on a line
        /// </summary>
        /// <param name="p1">A point on the line</param>
        /// <param name="p2">A point on the line</param>
        /// <param name="point">The point to check</param>
        /// <returns>Zero if on the line, positive or negative if not</returns>
        public static float lineEquation(Vector2 p1, Vector2 p2, Vector2 point)
        {
            return (p2.Y - p1.Y) * point.X + (p1.X - p2.X) * point.Y + (p2.X * p1.Y - p1.X * p2.Y);
        }
    }
}
