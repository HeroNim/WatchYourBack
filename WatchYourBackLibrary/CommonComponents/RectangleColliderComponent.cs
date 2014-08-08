using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// The component that represents an axis-alligned rectangle collider, determined by XNA's rectangle class.
    /// </summary>
    public class RectangleColliderComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Collider + (int)Masks.RectangleCollider; } }
        public override Masks Mask { get { return Masks.RectangleCollider; } }

        private Rectangle collider;
        private TransformComponent anchor;
      
        public RectangleColliderComponent(Rectangle r, TransformComponent anchor)
        {
            collider = r;
            this.anchor = anchor;
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

        public TransformComponent Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }      
    }
}
