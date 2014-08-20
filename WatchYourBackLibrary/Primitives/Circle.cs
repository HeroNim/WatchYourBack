using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public class Circle
    {
        private float radius;
        private Vector2 center;
       
        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public float X
        {
            get { return center.X; }
            set { center.X = value; }
        }

        public float Y
        {
            get { return center.Y; }
            set { center.Y = value; }
        }

        public Vector2 Center
        {
            get { return center; }
            set { center = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }       
      
        public Vector2 PointOnCircle (Vector2 vector)
        {
            float angle = HelperFunctions.VectorToAngle(vector);
            return PointOnCircle(this.radius, angle, this.center);
        }

        public Vector2 PointOnCircle (float angle)
        {
            return PointOnCircle(this.radius, angle, this.center);
        }   

        
        public static Vector2 PointOnCircle(float radius, float angle, Vector2 origin)
        {
            float x = -((float)(radius * Math.Cos(angle))) + origin.X;
            float y = -((float)(radius * Math.Sin(angle))) + origin.Y;
            return new Vector2(x, y);
        }

        public static Vector2 PointOnCircle(Circle circle, float angle)
        {
            return PointOnCircle(circle.Radius, angle, circle.Center);
        }

        public static List<Vector2> TangentPoints(Circle circle, Vector2 externalPoint)
        {
            Vector2 hypotenuse = externalPoint - circle.Center;
            double hypAngle = HelperFunctions.VectorToAngle(hypotenuse);

            float hypotenuseLength = hypotenuse.Length();
            float oppositeLength = circle.Radius;

            double centerAngle = Math.Asin(oppositeLength / hypotenuseLength);

            List<Vector2> values = new List<Vector2>();
            values.Add(PointOnCircle(circle, (float)(hypAngle + centerAngle)));
            values.Add(PointOnCircle(circle, (float)(hypAngle - centerAngle)));
            return values;



        }
    }
}
