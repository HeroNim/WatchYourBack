using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    /*
     * A line collider that is the location a player can be hit in the game
     */
    public class PlayerHitboxComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.COLLIDER + (int)Masks.PLAYER_HITBOX; } }
        public override Masks Mask { get { return Masks.PLAYER_HITBOX; } }

        private float width;
        private Vector2 p1;
        private Vector2 p2;

        public PlayerHitboxComponent(Rectangle body, float width)
        {
            p1 = Vector2.Zero;
            p2 = Vector2.Zero;
            this.width = width;
        }

        public Vector2 P1
        {
            get { return p1; }
            set { p1 = value; }
        }

        public Vector2 P2
        {
            get { return p2; }
            set { p2 = value; }
        }

        public float X1
        {
            get { return p1.X; }
            set { p1.X = value; }
        }

        public float X2
        {
            get { return p2.X; }
            set { p2.X = value; }
        }

        public float Y1
        {
            get { return p1.Y; }
            set { p1.Y = value; }
        }

        public float Y2
        {
            get { return p2.Y; }
            set { p2.Y = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        
    }
}
