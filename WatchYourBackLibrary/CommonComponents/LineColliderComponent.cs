using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace WatchYourBackLibrary
{
    /// <summary>
    /// The component that represents a line collider, determined by two points.
    /// </summary>
    public class LineColliderComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.LineCollider + (int)Masks.Collider; } }
        public override Masks Mask { get { return Masks.LineCollider; } }

        private Line collider;
      
        public LineColliderComponent(Vector2 point1, Vector2 point2, float rotation = 0)
        {
            collider = new Line(point1, point2, rotation);
        }

        public Line Collider
        {
            get { return collider; }
            set { collider = value; }
        }

        public Vector2 P1
        {
            get { return collider.P1; }
            set { collider.P1 = value; }
        }

        public Vector2 P2
        {
            get { return collider.P2; }
            set { collider.P2 = value; }
        }

        public float X1
        {
            get { return collider.X1; }
            set { collider.X1 = value; }
        }

        public float X2
        {
            get { return collider.X2; }
            set { collider.X2 = value; }
        }

        public float Y1
        {
            get { return collider.Y1; }
            set { collider.Y1 = value; }
        }

        public float Y2
        {
            get { return collider.Y2; }
            set { collider.Y2 = value; }
        }
    }
}
