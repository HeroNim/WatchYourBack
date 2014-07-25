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
                VelocityComponent velocity = (VelocityComponent)entity.Components[Masks.VELOCITY];

                if (entity.hasComponent(Masks.WEAPON))
                {                 
                        WeaponComponent weapon = (WeaponComponent)entity.Components[Masks.WEAPON];
                        weapon.Arc += Math.Abs(velocity.RotationSpeed);

                        if (weapon.Anchored == true)
                        {
                            VelocityComponent anchorVelocity = (VelocityComponent)weapon.Wielder.Components[Masks.VELOCITY];
                            transform.Rotation += anchorVelocity.RotationSpeed;
                            velocity.X = anchorVelocity.X;
                            velocity.Y = anchorVelocity.Y;
                        }                    
                }
                transform.Position = new Vector2(transform.X + velocity.X, transform.Y + velocity.Y);
                transform.Rotation += velocity.RotationSpeed;
                
                if(entity.hasComponent(Masks.GRAPHICS))
                {
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.GRAPHICS];
                    graphics.X = (int)transform.X;
                    graphics.Y = (int)transform.Y;
                    graphics.RotationAngle = transform.Rotation;                   
                }
                              
                if (entity.hasComponent(Masks.COLLIDER))
                {
                    if (entity.hasComponent(Masks.RECTANGLE_COLLIDER))
                    {
                        RectangleColliderComponent collider = (RectangleColliderComponent)entity.Components[Masks.RECTANGLE_COLLIDER];
                        collider.X = (int)transform.X;
                        collider.Y = (int)transform.Y;
                    }

                    if(entity.hasComponent(Masks.CIRCLE_COLLIDER))
                    {
                        CircleColliderComponent collider = (CircleColliderComponent)entity.Components[Masks.CIRCLE_COLLIDER];
                        collider.X = (int)transform.Center.X;
                        collider.Y = (int)transform.Center.Y;

                       
                    }

                    if (entity.hasComponent(Masks.LINE_COLLIDER))
                    {
                        LineColliderComponent collider = (LineColliderComponent)entity.Components[Masks.LINE_COLLIDER];
                        WeaponComponent weapon = (WeaponComponent)entity.Components[Masks.WEAPON];
                        TransformComponent anchorTransform = (TransformComponent)weapon.Wielder.Components[Masks.TRANSFORM];
                        VelocityComponent anchorVelocity = (VelocityComponent)weapon.Wielder.Components[Masks.VELOCITY];
                        
                        collider.P1 = new Vector2(collider.P1.X + velocity.X, collider.P1.Y + velocity.Y);
                        collider.P2 = new Vector2(collider.P2.X + velocity.X, collider.P2.Y + velocity.Y);
                        Vector2 rotation1 = Vector2.Transform(collider.P1 - anchorTransform.Center, Matrix.CreateRotationZ(velocity.RotationSpeed + anchorVelocity.RotationSpeed)) + anchorTransform.Center;
                        Vector2 rotation2 = Vector2.Transform(collider.P2 - anchorTransform.Center, Matrix.CreateRotationZ(velocity.RotationSpeed + anchorVelocity.RotationSpeed)) + anchorTransform.Center;

                        collider.P1 = rotation1;
                        collider.P2 = rotation2;

                        Vector2 rotation = Vector2.Transform(transform.Position - anchorTransform.Center, Matrix.CreateRotationZ(velocity.RotationSpeed + anchorVelocity.RotationSpeed)) + anchorTransform.Center;
                        transform.Position = rotation;


                    }
                    
                    if(entity.hasComponent(Masks.PLAYER_HITBOX))
                    {
                        PlayerHitboxComponent collider = (PlayerHitboxComponent)entity.Components[Masks.PLAYER_HITBOX];
                                                    
                        collider.P1 = new Vector2(collider.P1.X + velocity.X, collider.P1.Y + velocity.Y);
                        collider.P2 = new Vector2(collider.P2.X + velocity.X, collider.P2.Y + velocity.Y);
                                              
                        Vector2 rotation1 = Vector2.Transform(collider.P1 - transform.Center, Matrix.CreateRotationZ(velocity.RotationSpeed)) + transform.Center;
                        Vector2 rotation2 = Vector2.Transform(collider.P2 - transform.Center, Matrix.CreateRotationZ(velocity.RotationSpeed)) + transform.Center;
                        collider.P1 = rotation1;
                        collider.P2 = rotation2;


                    }
                }

                if (transform.HasMoved)
                    manager.addChangedEntities(entity, COMMANDS.MODIFY);
                transform.HasMoved = false;
            }
        }
    }
}
