using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBack
{
    class ColliderComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.COLLIDER;
        public override Masks Mask { get { return Masks.COLLIDER; } }

        private Rectangle collider;
       

        public ColliderComponent()
        {
        }

        public ColliderComponent(Rectangle r)
        {
            collider = r;
        }

        public int X
        {
            get { return collider.X; }
            set { collider.X = value; }
        }

        public int Y
        {
            get { return collider.Y; }
            set { collider.Y = value; }
        }

        public int Width
        {
            get { return collider.Width; }
            set { collider.Width = value; }
        }

        public int Height
        {
            get { return collider.Height; }
            set { collider.Height = value; }
        }

        public Rectangle Collider
        {
            get { return collider; }
            set { collider = value; }
        }

        


    }
}
