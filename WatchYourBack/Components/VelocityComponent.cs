using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBack
{
    /*
     * Holds the velocity of the entity 
     */
    class VelocityComponent : EComponent
    {

        public readonly static int bitMask = (int)Masks.VELOCITY;
        public override int Mask { get { return bitMask; } }

        private Vector2 velocity;

        public VelocityComponent(float x, float y)
        {
            
            velocity = new Vector2(x,y);
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public float X
        {
            get { return velocity.X; }
            set { velocity.X = value; }
        }

        public float Y
        {
            get { return velocity.Y; }
            set { velocity.Y = value; }
        }
    }
}
