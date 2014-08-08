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
        private Line collider;

        public PlayerHitboxComponent(Vector2 point1, Vector2 point2)
        {
            collider = new Line(point1, point2);           
        }

        public Line Collider
        {
            get { return collider; }
            set { collider = value; }
        }

        public PlayerHitboxComponent(List<Vector2> points)
            : this(points[0], points[1]) { }


        public Vector2 P1
        {
            get { return collider.P1; }
            set { collider.P1 = value; }
        }

        public Vector2 P2
        {
            get { return collider.P2; }
            set { collider.P2 = value; }
        }

        public float X1
        {
            get { return collider.X1; }
            set { collider.X1 = value; }
        }

        public float X2
        {
            get { return collider.X2; }
            set { collider.X2 = value; }
        }

        public float Y1
        {
            get { return collider.Y1; }
            set { collider.Y1 = value; }
        }

        public float Y2
        {
            get { return collider.Y2; }
            set { collider.Y2 = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public static List<Vector2> setAvatarHitbox(Rectangle body, float width, Vector2 direction)
        {
            List<Vector2> playerPoints = new List<Vector2>();
            Vector2 reverse = new Vector2(-direction.X, -direction.Y);
            Vector2 perpendicular = new Vector2(reverse.Y, -reverse.X);
            reverse.Normalize();
            perpendicular.Normalize();

            reverse *= body.Height / 2 + 6; //A line pointing in the opposite direction of the player
            perpendicular *= width; //A line pointing units perpendicular to the look direction of the player

            Vector2 midPoint = new Vector2(body.Center.X + reverse.X, body.Center.Y + reverse.Y);
            playerPoints.Add(new Vector2(midPoint.X + perpendicular.X, midPoint.Y + perpendicular.Y)); //A point on the tangent
            playerPoints.Add(new Vector2(midPoint.X - perpendicular.X, midPoint.Y - perpendicular.Y)); //A point on the tangent
            return playerPoints;
        }

        
    }
}
