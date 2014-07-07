using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;


namespace WatchYourBackLibrary
{
    /*
     * Holds the position and rotation of the entity
    */

    public class TransformComponent : EComponent
    {
        private Vector2 position;
        private int width;
        private int height;

        private bool xLock;
        private bool yLock;
        private float rotation;

        private bool hasMoved;
        private Vector2 lookDirection; //Vector between the mouse pointer and the avatar
        private float lookAngle; //Angle between the lookDirection vector and the y-axis

        public override int BitMask { get { return (int)Masks.TRANSFORM; } }
        public override Masks Mask { get { return Masks.TRANSFORM; } }

        public TransformComponent(float x, float y, int width, int height)
        {
            position = new Vector2(x, y);
            this.width = width;
            this.height = height;
            rotation = 0;
            hasMoved = false;
            lookDirection = Vector2.Zero;
            lookAngle = 0;
        }

        public TransformComponent(float x, float y, int width, int height, float rotation)
        {
            position = new Vector2(x, y);
            this.rotation = rotation;
            this.width = width;
            this.height = height;
            hasMoved = false;
            lookDirection = Vector2.Zero;
            lookAngle = 0;
        }

        public float X
        {
            get { return position.X; }
            set
            {
                if (position.X != value)
                {
                    position.X = value;
                    hasMoved = true;
                }
            }
        }

        public float Y
        {
            get { return position.Y; }
            set
            {
                if (position.Y != value)
                {
                    position.Y = value;
                    hasMoved = true;
                }
            }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    hasMoved = true;
                }
            }
        }

        public float Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    hasMoved = true;
                }
            }
        }

        public bool HasMoved
        {
            get { return hasMoved; }
            set { hasMoved = value; }
        }

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public Vector2 Center
        {
            get { return new Vector2(position.X + width/2, position.Y + height/2); }
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

        public static float distanceBetween(TransformComponent t1, TransformComponent t2)
        {
            float y = Math.Abs(t1.Center.Y - t2.Center.Y);
            float x = Math.Abs(t1.Center.X - t2.Center.X);

            return (float)Math.Sqrt((x * x) + (y * y));
        }

        public Vector2 LookDirection
        {
            get { return lookDirection; }
            set { lookDirection = new Vector2(value.X, value.Y); }
        }

        public float LookAngle
        {
            get { lookAngle = -(float)Math.Atan2(lookDirection.X * Vector2.UnitY.Y, lookDirection.Y * Vector2.UnitY.Y);
                    if (lookAngle < 0)
                        lookAngle = (float)(lookAngle + Math.PI * 2);
                    return lookAngle;
                    
            }
        }


    }
}
