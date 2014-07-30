using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// A component that represents a line collider, determined by two points, that is contained only on the avatars and 
    /// is where the players are attempting to hit.
    /// </summary>
    public class PlayerHitboxComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Collider + (int)Masks.PlayerHitbox; } }
        public override Masks Mask { get { return Masks.PlayerHitbox; } }

        private float width;
        private Vector2 p1;
        private Vector2 p2;

        public PlayerHitboxComponent(Rectangle body, float width, Vector2 direction)
        {

            this.width = width;
            Vector2 reverse = new Vector2(-direction.X, -direction.Y);
            Vector2 perpendicular = new Vector2(reverse.Y, -reverse.X);
            reverse.Normalize();
            perpendicular.Normalize();

            reverse *= HelperFunctions.Diagonal(body); //A line of length radius pointing in the opposite direction of the player
            perpendicular *= width; //A line pointing units perpendicular to the look direction of the player

            Vector2 midPoint = new Vector2(body.Center.X + reverse.X, body.Center.Y + reverse.Y);
            p1 = new Vector2(midPoint.X + perpendicular.X, midPoint.Y + perpendicular.Y); //A point on the tangent
            p2 = new Vector2(midPoint.X - perpendicular.X, midPoint.Y - perpendicular.Y); //A point on the tangent
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
