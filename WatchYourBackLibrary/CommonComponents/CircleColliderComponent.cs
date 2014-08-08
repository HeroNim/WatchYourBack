using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// The component that represents a circular collider, determined by it's radius and position, as well
    /// as some helpful methods for determining various properties of the circle.
    /// </summary>
    public class CircleColliderComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Collider + (int)Masks.CircleCollider; } }
        public override Masks Mask { get { return Masks.CircleCollider; } }

        private Circle collider;
        private TransformComponent anchor;
       
        public CircleColliderComponent(Vector2 center, float radius, TransformComponent anchor)
        {
            collider = new Circle(center, radius);           
            this.anchor = anchor;
        }

        public Circle Collider
        {
            get { return collider; }
            set { collider = value; }
        }

        public float X
        {
            get { return collider.X; }
            set { collider.X = value; }
        }

        public float Y
        {
            get { return collider.Y; }
            set { collider.Y = value; }
        }

        public Vector2 Center
        {
            get { return collider.Center; }
            set { collider.Center = value; }
        }

        public float Radius
        {
            get { return collider.Radius; }
            set { collider.Radius = value; }
        }

        public TransformComponent Anchor
        {
            get { return anchor; }
            set { anchor = value;  }
        }
      
        public Vector2 PointOnCircle (Vector2 vector)
        {
            float angle = HelperFunctions.VectorToAngle(vector);
            return HelperFunctions.pointOnCircle(this.Radius, angle, this.Center);
        }

        public Vector2 PointOnCircle (float angle)
        {
            return HelperFunctions.pointOnCircle(this.Radius, angle, this.Center);
        }   
    }
}
