﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace WatchYourBackLibrary
{
    /// <summary>
    /// The component which holds the data on the physical properties of an entity, such as it's position and size, as well as some
    /// methods for determining various aspects of those properties.
    /// </summary>
    public class TransformComponent : EComponent
    {
        private Vector2 position;
        private int width;
        private int height;

        private float rotation;
        private Vector2 rotationPoint;

        private bool hasMoved;
        private Vector2 lookDirection; //Vector between the mouse pointer and the avatar
        private float lookAngle; //Angle between the lookDirection vector and the y-axis

        private Entity parent;

        public override int BitMask { get { return (int)Masks.Transform; } }
        public override Masks Mask { get { return Masks.Transform; } }

        public TransformComponent(float x, float y)
            : this(x, y, 0, 0) {}           
        
        public TransformComponent(float x, float y, int width, int height)
        {
            position = new Vector2(x, y);
            this.width = width;
            this.height = height;
            rotation = 0;
            rotationPoint = position;
            hasMoved = false;
            lookDirection = Vector2.Zero;
            lookAngle = 0;
            parent = null;
        }

        public TransformComponent(Rectangle rect)
            : this(rect.X, rect.Y, rect.Width, rect.Height) { }
        
        public TransformComponent(Vector2 position, int width, int height, float rotation)
            : this(position.X, position.Y, width, height)
        {
            this.rotation = rotation;                    
        }

        public TransformComponent(float x, float y, int width, int height, float rotation)
            : this(new Vector2(x, y), width, height, rotation){ }

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

        public Rectangle Body
        {
            get { return new Rectangle((int)X, (int)Y, Width, Height); }
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
            get {
                if (rotation < 0)
                    rotation += (float)Math.PI * 2;
                if (rotation > Math.PI * 2)
                    rotation -= (float)Math.PI * 2; 
                return rotation % ((float)Math.PI * 2);
            }
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    hasMoved = true;
                }
            }
        }
        public Vector2 RotationPoint
        {
            get { return rotationPoint; }
            set { rotationPoint = value; }
        }

        public Vector2 LookDirection
        {
            get { return lookDirection; }
            set { lookDirection = new Vector2(value.X, value.Y); }
        }

        public float LookAngle
        {
            get
            {
                lookAngle = -(float)Math.Atan2(lookDirection.X * Vector2.UnitY.Y, lookDirection.Y * Vector2.UnitY.Y) + (float)Math.PI; //Stupid xna
                if (lookAngle < 0)
                    lookAngle += (float)Math.PI * 2;
                if (lookAngle > Math.PI * 2) 
                    lookAngle -= (float)Math.PI * 2;
                if (lookDirection == Vector2.Zero)
                    lookAngle = 0;
                return lookAngle;
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

        public float Diagonal
        {
            get { return (float)Math.Sqrt(Math.Pow(Width, 2) + Math.Pow(Height, 2))/2; }
        }

        public float Radius
        {
            get { return (float)Width / 2; }
        }

        public Entity Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public bool HasParent
        {
            get { return (parent != null); }
        }

        public static float DistanceBetween(TransformComponent t1, TransformComponent t2)
        {
            float y = Math.Abs(t1.Center.Y - t2.Center.Y);
            float x = Math.Abs(t1.Center.X - t2.Center.X);

            return (float)Math.Sqrt((x * x) + (y * y));
        }
    }
}
