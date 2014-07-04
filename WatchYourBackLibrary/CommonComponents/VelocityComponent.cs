using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    /*
     * Holds the velocity of the entity 
     */
    public class VelocityComponent : EComponent
    {

        public override int BitMask { get { return (int)Masks.VELOCITY; } }
        public override Masks Mask { get { return Masks.VELOCITY; } }

        private Vector2 velocity;
        private float rotationSpeed;

        public VelocityComponent(float x, float y)
        {  
            velocity = new Vector2(x,y);
            rotationSpeed = 0;
        }

        public VelocityComponent(float x, float y, float rotation)
        {
            velocity = new Vector2(x, y);
            rotationSpeed = rotation/100;
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

        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value/100; }
        }
    }
}
