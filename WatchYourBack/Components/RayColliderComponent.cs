using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBack
{
    class RayColliderComponent : ColliderComponent
    {
        new public readonly static int bitMask = (int)Masks.COLLIDER + (int)Masks.RAY_COLLIDER;
        public override Masks Mask { get { return Masks.RAY_COLLIDER; } }

        private Ray collider;
        private float length;

        public RayColliderComponent(float x, float y, float xDir, float yDir, float length)
        {
            collider = new Ray(new Vector3(x, y, 0), new Vector3(xDir, yDir, 0));
            this.length = length;
        }

        new public float X
        {
            get { return collider.Position.X + (collider.Direction.X * length); }
            set { collider.Position.X = value; }
        }

        new public float Y
        {
            get { return collider.Position.Y + (collider.Direction.Y * length); }
            set { collider.Position.Y = value; }
        }

        public float XDir
        {
            get { return collider.Direction.X; }
            set { collider.Direction.X = value; }
        }

        public float YDir
        {
            get { return collider.Direction.Y; }
            set { collider.Direction.Y = value; }
        }

        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        new public Ray Collider
        {
            get { return collider; }
            set { collider = value; }
        }

        
    }
}
