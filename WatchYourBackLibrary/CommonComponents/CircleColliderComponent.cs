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

        private float radius;
        private Vector2 center;

        private TransformComponent anchor;
       
        public CircleColliderComponent(Vector2 center, float radius, TransformComponent anchor)
        {
            this.center = center;
            this.radius = radius;
            this.anchor = anchor;
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

        public TransformComponent Anchor
        {
            get { return anchor; }
            set { anchor = value;  }
        }
      
        public Vector2 PointOnCircle (Vector2 vector)
        {
            float angle = HelperFunctions.VectorToAngle(vector);
            return HelperFunctions.pointOnCircle(this.radius, angle, this.center);
        }

        public Vector2 PointOnCircle (float angle)
        {
            return HelperFunctions.pointOnCircle(this.radius, angle, this.center);
        }   
    }
}
