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
        /// Checks if two rectangles intersect.
        /// </summary>
        /// <param name="c1">A rectangle</param>
        /// <param name="c2">A rectangle</param>
        /// <returns>True if colliding</returns>
        public static bool CheckCollision(Rectangle c1, Rectangle c2)
        {
            return c1.Intersects(c2);
        }

        /// <summary>
        /// Check if a line and a rectangle intersect.
        /// </summary>
        /// <param name="c1">A line</param>
        /// <param name="c2">A rectangle</param>
        /// <returns>True if colliding</returns>
        public static bool CheckCollision(Line c1, Rectangle c2)
        {


            bool possible = false;

            bool[] pos = new bool[4];
            pos[0] = (lineEquation(c1.P1, c1.P2, c2.Right, c2.Top) > 0);
            pos[1] = (lineEquation(c1.P1, c1.P2, c2.Left, c2.Bottom) > 0);
            pos[2] = (lineEquation(c1.P1, c1.P2, c2.Right, c2.Bottom) > 0);
            pos[3] = (lineEquation(c1.P1, c1.P2, c2.Left, c2.Top) > 0);

            for (int i = 0; i < pos.Length - 1; i++)
            {
                if (pos[i] != pos[i + 1])
                {
                    possible = true;
                    break;
                }
            }

            if (!possible)
                return false;

            if (c1.X1 > c2.Right && c1.X2 > c2.Right)
                return false;
            if (c1.X1 < c2.Left && c1.X2 < c2.Left)
                return false;
            if (c1.Y1 < c2.Top && c1.Y2 < c2.Top)
                return false;
            if (c1.Y1 > c2.Bottom && c1.Y2 > c2.Bottom)
                return false;
            return true;
        }

        /// <summary>
        /// Checks if two lines intersect.
        /// </summary>
        /// <param name="c1">A line</param>
        /// <param name="c2">A line</param>
        /// <returns>True if colliding</returns>
        public static bool CheckCollision(Line c1, Line c2)
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
        /// Checks if a line and a circle intersect.
        /// </summary>
        /// <param name="c1">A line</param>
        /// <param name="c2">A circle</param>
        /// <returns>True if colliding</returns>
        public static bool CheckCollision(Line c1, Circle c2)
        {
            //d = direction vector of line
            //f = vector from line to circle
            // t^2 * (d DOT d) + 2t * ( f DOT d ) + ( f DOT f - r^2 ) = 0, so solve quadratic equation

            Vector2 d = c1.P2 - c1.P1;
            Vector2 f = c1.P1 - c2.Center;

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - (c2.Radius * c2.Radius);

            float discriminant = (b * b) - (4 * a * c);
            if (discriminant < 0)
                return false; //Missed
            else
            {
                discriminant = (float)Math.Sqrt(discriminant);
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                if (t1 >= 0 && t1 <= 1)
                    return true;
                if (t2 >= 0 && t2 <= 1)
                    return true;
                return false;
            }
        }

        

        /// <summary>
        /// Returns the intersection point(s) of a line-rectangle intersection.
        /// </summary>
        /// <param name="c1">A line</param>
        /// <param name="c2">A rectangle</param>
        /// <returns>The intersection point(s)</returns>
        public static List<Vector2> GetIntersection(Line c1, Rectangle c2)
        {
            List<Vector2> intersectionPoints = new List<Vector2>();

            Line top = new Line(new Vector2(c2.Left, c2.Top), new Vector2(c2.Right, c2.Top));
            Line left = new Line(new Vector2(c2.Left, c2.Top), new Vector2(c2.Left, c2.Bottom));
            Line bottom = new Line(new Vector2(c2.Left, c2.Bottom), new Vector2(c2.Right, c2.Bottom));
            Line right = new Line(new Vector2(c2.Right, c2.Top), new Vector2(c2.Right, c2.Bottom));

            Vector2 topInt = GetIntersection(c1, top);
            Vector2 leftInt = GetIntersection(c1, left);
            Vector2 bottomInt = GetIntersection(c1, bottom);
            Vector2 rightInt = GetIntersection(c1, right);

            if (topInt != Vector2.Zero)
                intersectionPoints.Add(topInt);
            if (leftInt != Vector2.Zero)
                intersectionPoints.Add(leftInt);
            if (bottomInt != Vector2.Zero)
                intersectionPoints.Add(bottomInt);
            if (rightInt != Vector2.Zero)
                intersectionPoints.Add(rightInt);

            if (intersectionPoints.Count == 0)
                return null;
            
            return intersectionPoints;
        }

        /// <summary>
        /// Returns the intersection point of two lines.
        /// </summary>
        /// <param name="c1">A line</param>
        /// <param name="c2">A line</param>
        /// <returns>The intersection point</returns>
        public static Vector2 GetIntersection(Line c1, Line c2)
        {
            Vector2 p = c1.P1;
            Vector2 q = c2.P1;
            Vector2 r = c1.P2 - c1.P1;
            Vector2 s = c2.P2 - c2.P1;

            float v = HelperFunctions.Cross(r, s);
            float w = HelperFunctions.Cross(q - p, r);

            float t = HelperFunctions.Cross((q - p), s) / v;
            float u = HelperFunctions.Cross((q - p), r) / v;

            if(v != 0)
            {
                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    p += (t * r);
                    return p;
                }
            }
            return Vector2.Zero;


           
        }

        /// <summary>
        /// Returns the intersection point(s) of a line-circle intersection.
        /// </summary>
        /// <param name="c1">A line</param>
        /// <param name="c2">A circle</param>
        /// <returns>The intersection point(s)</returns>
        public static List<Vector2> GetIntersection(Line c1, Circle c2)
        {
            List<Vector2> intersectionPoints = new List<Vector2>();
            // t^2 * (d DOT d) + 2t*( f DOT d ) + ( f DOT f - r^2 ) = 0

            Vector2 d = c1.P2 - c1.P1;
            Vector2 f = c1.P1 - c2.Center;

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - (c2.Radius * c2.Radius);

            float discriminant = (b * b) - (4 * a * c);
            if (discriminant < 0)
                return null;
            else
            {
                discriminant = (float)Math.Sqrt(discriminant);
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                if (t1 >= 0 && t1 <= 1)
                    intersectionPoints.Add(c1.P1 + t1 * d);
                if (t2 >= 0 && t2 <= 1)
                    intersectionPoints.Add(c1.P1 + t2 * d);
                if (intersectionPoints.Count == 0)
                    return null;               
                return intersectionPoints;
            }
        }

        /// <summary>
        /// Checks if a given point is contained above, below, or on a line
        /// </summary>
        /// <param name="p1">A point on the line</param>
        /// <param name="p2">A point on the line</param>
        /// <param name="point">The point to check</param>
        /// <returns>Zero if on the line, positive or negative if not</returns>
        public static float lineEquation(Vector2 p1, Vector2 p2, float x, float y)
        {
            return (p2.Y - p1.Y) * x + (p1.X - p2.X) * y + (p2.X * p1.Y - p1.X * p2.Y);
        }

        public static float lineEquation(Vector2 p1, Vector2 p2, Vector2 point)
        {
            return (p2.Y - p1.Y) * point.X + (p1.X - p2.X) * point.Y + (p2.X * p1.Y - p1.X * p2.Y);
        }

        
    }
}
