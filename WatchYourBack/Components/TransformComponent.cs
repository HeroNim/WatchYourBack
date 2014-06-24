using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace WatchYourBack
{
    /*
     * Holds the position of the entity 
    */

    public class TransformComponent : EComponent
    {
        private Vector2 position;

        private bool xLock;
        private bool yLock;

        public readonly static int bitMask = (int)Masks.TRANSFORM;
        public override Masks Mask { get { return Masks.TRANSFORM; } }
        

        public TransformComponent(float x, float y)
        {
            position = new Vector2(x, y);
        }



        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
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
