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
        public override int Mask { get { return bitMask; } }

        private Rectangle collider;
        private bool xLock;
        private bool yLock;

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

        /*
         * The locks are used to stop the entity from jittering when there are multiple collisions 
         */
        public bool XLock
        {
            get { return xLock; }
            set { xLock = value; }
        }

        public bool YLock
        {
            get { return yLock; }
            set { yLock = value; }
        }

        public void resetLocks()
        {
            xLock = false;
            yLock = false;
        }


    }
}
