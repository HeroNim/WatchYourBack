using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public static class HelperFunctions
    {
        
        //Returns the diagonal length of a rectangle
        public static float Diagonal (Rectangle rect)
        {
           return (float)Math.Sqrt(Math.Pow(rect.Width, 2) + Math.Pow(rect.Height, 2)) / 2;
        }

        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, vector.Y);
        }

        //Returns the angle between two vectors
        public static float Angle(Vector2 v1, Vector2 v2)
        {
            float dotProduct = Vector2.Dot(v1, v2);

            float mag1 = (float)Math.Sqrt(Math.Pow(v1.X, 2) + Math.Pow(v1.Y, 2));
            float mag2 = (float)Math.Sqrt(Math.Pow(v2.X, 2) + Math.Pow(v2.Y, 2));

            float cosAngle = dotProduct / (Math.Abs(mag1) * Math.Abs(mag2));

            return (float)Math.Acos(cosAngle);
        }

        public static float Normalize (float angle)
        {
            if(angle > Math.PI * 2)
                angle -= (float)Math.PI * 2;
            if(angle < 0)
                angle += (float)Math.PI * 2;
            return angle;
        }

        public static Vector2 pointOnCircle(float radius, float angle, Vector2 origin)
        {
            float x = -((float)(radius * Math.Cos(angle))) + origin.X;
            float y = -((float)(radius * Math.Sin(angle))) + origin.Y;

            return new Vector2(x, y);
        }


         /*
         * Checks two entities for collisions. The first entity extends its collider in the x and y directions, and checks for collisions. If they will collide,
         * the entity is moved back one step, and the collider returns to its original position. After all collisions have been checked, the movement
         * system resets the colliders to be centered on their respective entities again. Also checks for weapons on the entity; if it has one, it makes sure
         * the weapon moves with the entity.
         */
        
        public static bool checkRectangle_RectangleCollisions(Entity e1, Entity e2)
        {
            bool collided = false;
            int displacement;
            //Assign local variables

            VelocityComponent v1 = (VelocityComponent)e1.Components[Masks.VELOCITY];

            RectangleColliderComponent c1 = (RectangleColliderComponent)e1.Components[Masks.RECTANGLE_COLLIDER];
            RectangleColliderComponent c2 = (RectangleColliderComponent)e2.Components[Masks.RECTANGLE_COLLIDER];


            //Check collisions

            displacement = (int)v1.X;
            c1.X += displacement;
            if (c1.Collider.Intersects(c2.Collider))
            {
                collided = true;
                v1.X = 0;
            }
            c1.X -= displacement;

            displacement = (int)v1.Y;
            c1.Y += displacement;
            if (c1.Collider.Intersects(c2.Collider))
            {
                collided = true;
                v1.Y = 0;
            }
            c1.Y -= displacement;

            return collided;
        }

        public static bool checkLine_HitboxCollision(Entity e1, Entity e2)
        {
            LineColliderComponent c1 = (LineColliderComponent)e1.Components[Masks.LINE_COLLIDER];
            PlayerHitboxComponent c2 = (PlayerHitboxComponent)e2.Components[Masks.PLAYER_HITBOX];

            float result1 = HelperFunctions.lineEquation(c1.P1, c1.P2, c2.P1);
            float result2 = HelperFunctions.lineEquation(c1.P1, c1.P2, c2.P2);
            float result3 = HelperFunctions.lineEquation(c2.P1, c2.P2, c1.P1);
            float result4 = HelperFunctions.lineEquation(c2.P1, c2.P2, c1.P2);

            if (result1 * result2 > 0 || result3 * result4 > 0)
                return false;

            return true;
        }

        public static bool checkLine_CircleCollision(Entity e1, Entity e2)
        {

            LineColliderComponent c1 = (LineColliderComponent)e1.Components[Masks.LINE_COLLIDER];
            CircleColliderComponent c2 = (CircleColliderComponent)e2.Components[Masks.CIRCLE_COLLIDER];


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

        /*
        * Checks for an intersection between a line defined by two endpoints and a rectangle. After predicting forward the collider, each corner of the rectangle
        * is checked to make sure it is on the same side of the line. If they are not, then the function then checks to see if
        * each set of endpoints has at least one value within the rectangle's width or height for each axis. If there is, there is
        * an intersection.
        * 
        * At this point, only weapons have line colliders, so if there is an intersection, the method simply removes the weapon
        */
        public static bool checkLineCollision(Entity e1, Entity e2)
        {
            LineColliderComponent c1 = (LineColliderComponent)e1.Components[Masks.LINE_COLLIDER];
            RectangleColliderComponent c2 = (RectangleColliderComponent)e2.Components[Masks.RECTANGLE_COLLIDER];
            VelocityComponent v1 = (VelocityComponent)e1.Components[Masks.VELOCITY];

            //Predict collider forward

            c1.X1 += v1.X;
            c1.X2 += v1.X;
            c1.Y1 += v1.Y;
            c1.Y2 += v1.Y;
            Vector2 rotation = Vector2.Transform(c1.P2 - c1.P1, Matrix.CreateRotationZ(v1.RotationSpeed)) + c1.P1;
            c1.P2 = rotation;

            bool above = false;
            bool possibleIntersection = false;
            bool intersection = false;

            //Check if corners are all above or below the line

            Vector2 topLeft = new Vector2(c2.Collider.Left, c2.Collider.Top);
            Vector2 bottomLeft = new Vector2(c2.Collider.Left, c2.Collider.Bottom);
            Vector2 topRight = new Vector2(c2.Collider.Right, c2.Collider.Top);
            Vector2 bottomRight = new Vector2(c2.Collider.Right, c2.Collider.Bottom);

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

            //Move the collider back

            rotation = Vector2.Transform(c1.P2 - c1.P1, Matrix.CreateRotationZ(-v1.RotationSpeed)) + c1.P1;
            c1.P2 = rotation;
            c1.X1 -= v1.X;
            c1.X2 -= v1.X;
            c1.Y1 -= v1.Y;
            c1.Y2 -= v1.Y;

            return intersection;
        }

        public static float lineEquation(Vector2 p1, Vector2 p2, Vector2 point)
        {
            return (p2.Y - p1.Y) * point.X + (p1.X - p2.X) * point.Y + (p2.X * p1.Y - p1.X * p2.Y);
        }


    }
}
