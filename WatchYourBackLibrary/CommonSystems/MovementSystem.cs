using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    /*
     * The system used to move all entities. Finds all entities that have a transform and velocity component, then uses
     * the relevant data to adjust their position. Also adjusts any variables in extra components that rely on movement.
     */
    public class MovementSystem : ESystem
    {


        public MovementSystem() : base(false, true, 5)
        {
            components += (int)Masks.TRANSFORM;
            components += (int)Masks.VELOCITY;
        }

        public override void update(TimeSpan gameTime)
        {
            foreach (Entity entity in activeEntities)
            {
                TransformComponent transform = (TransformComponent)entity.Components[Masks.TRANSFORM];
                transform.HasMoved = false;
                
                VelocityComponent velocity = (VelocityComponent)entity.Components[Masks.VELOCITY];
                transform.Position = new Vector2(transform.X + velocity.X, transform.Y + velocity.Y);
                transform.Rotation += velocity.RotationSpeed;
                
       
                if(entity.hasComponent(Masks.GRAPHICS))
                {
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.GRAPHICS];
                    graphics.X = (int)transform.X;
                    graphics.Y = (int)transform.Y;
                    graphics.RotationAngle = transform.Rotation;
                    
                }

                if (entity.hasComponent(Masks.WEAPON))
                {
                    WeaponComponent weapon = (WeaponComponent)entity.Components[Masks.WEAPON];
                    weapon.Arc += Math.Abs(velocity.RotationSpeed);
                    
                }
                
                if (entity.hasComponent(Masks.COLLIDER))
                {
                    if (entity.hasComponent(Masks.LINE_COLLIDER))
                    {
                        LineColliderComponent collider = (LineColliderComponent)entity.Components[Masks.LINE_COLLIDER];
                        collider.P1 = new Vector2(collider.P1.X + velocity.X, collider.P1.Y + velocity.Y);
                        collider.P2 = new Vector2(collider.P2.X + velocity.X, collider.P2.Y + velocity.Y);
                        Vector2 rotation = Vector2.Transform(collider.P2 - collider.P1, Matrix.CreateRotationZ(velocity.RotationSpeed)) + collider.P1;
                        collider.P2 = rotation;
                        
                        //GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.GRAPHICS];
                        //graphics.DebugPoints.Add(collider.P1);
                        //graphics.DebugPoints.Add(collider.P2);
                    }
                    if (entity.hasComponent(Masks.RECTANGLE_COLLIDER))
                    {
                        RectangleColliderComponent collider = (RectangleColliderComponent)entity.Components[Masks.RECTANGLE_COLLIDER];
                        collider.X = (int)transform.X;
                        collider.Y = (int)transform.Y;
                    }
                    if(entity.hasComponent(Masks.PLAYER_HITBOX))
                    {
                        PlayerHitboxComponent collider = (PlayerHitboxComponent)entity.Components[Masks.PLAYER_HITBOX];

                        //GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.GRAPHICS];
                        //graphics.DebugPoints.Remove(collider.P1);
                        //graphics.DebugPoints.Remove(collider.P2); 
                             

                        
                        collider.P1 = new Vector2(collider.P1.X + velocity.X, collider.P1.Y + velocity.Y);
                        collider.P2 = new Vector2(collider.P2.X + velocity.X, collider.P2.Y + velocity.Y);
                        
                        Vector2 reverse = new Vector2(-transform.LookDirection.X, -transform.LookDirection.Y);
                        Vector2 perpendicular = new Vector2(reverse.Y, -reverse.X);
                        reverse.Normalize();
                        perpendicular.Normalize();
                        
                        reverse *= transform.Radius; //A line of length radius pointing in the opposite direction of the player
                        perpendicular *= collider.Width; //A line pointing units perpendicular to the look direction of the player

                        Vector2 midPoint = new Vector2(transform.Center.X + reverse.X, transform.Center.Y + reverse.Y);
                        collider.P1 = new Vector2(midPoint.X + perpendicular.X, midPoint.Y + perpendicular.Y); //A point on the tangent
                        collider.P2 = new Vector2(midPoint.X - perpendicular.X, midPoint.Y - perpendicular.Y); //A point on the tangent

                        //graphics.DebugPoints.Add(collider.P1);
                        //graphics.DebugPoints.Add(collider.P2); 


                        
                    }
                }

                if (transform.HasMoved)
                    manager.addChangedEntities(entity, COMMANDS.MODIFY);
            }
        }
    }
}
