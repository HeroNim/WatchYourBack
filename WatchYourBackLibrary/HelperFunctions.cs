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


    }
}
