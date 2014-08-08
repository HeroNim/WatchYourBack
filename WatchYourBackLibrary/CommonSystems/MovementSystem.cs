using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// The system used to move all entities. Finds all entities that have a transform and velocity component, then uses
    /// the relevant data to adjust their position. Also adjusts any variables in extra components that rely on movement.
    /// </summary>
    public class MovementSystem : ESystem
    {
        TransformComponent transform;
        VelocityComponent velocity;

        TransformComponent anchorTransform;
        VelocityComponent anchorVelocity;

        float xVel;
        float yVel;
        float rotationSpeed;

        public MovementSystem()
            : base(false, true, 5)
        {
            components += (int)Masks.Transform;
            components += (int)Masks.Velocity;
        }

        public override void update(TimeSpan gameTime)
        {
            foreach (Entity entity in activeEntities)
            {
                transform = (TransformComponent)entity.Components[Masks.Transform];
                velocity = (VelocityComponent)entity.Components[Masks.Velocity];
                anchorTransform = null;
                anchorVelocity = null;
                xVel = velocity.X;
                yVel = velocity.Y;
                rotationSpeed = velocity.RotationSpeed;

                if (transform.hasParent)
                {
                    anchorTransform = (TransformComponent)transform.Parent.Components[Masks.Transform];
                    anchorVelocity = (VelocityComponent)transform.Parent.Components[Masks.Velocity];
                    xVel += anchorVelocity.X;
                    yVel += anchorVelocity.Y;
                    rotationSpeed += anchorVelocity.RotationSpeed;
                }
                
                //Move entity
                transform.Position = new Vector2(transform.X + xVel, transform.Y + yVel);
                transform.RotationPoint = new Vector2(transform.RotationPoint.X + xVel, transform.RotationPoint.Y + yVel);
                
                //Rotate entity
                transform.Rotation += rotationSpeed;
                Vector2 rotation = Vector2.Transform(transform.Position - transform.RotationPoint, Matrix.CreateRotationZ(rotationSpeed)) + transform.RotationPoint;
                transform.Position = rotation;

                //Update weapons
                if (entity.hasComponent(Masks.Weapon))
                {
                    WeaponComponent weapon = (WeaponComponent)entity.Components[Masks.Weapon];
                    weapon.Arc += Math.Abs(velocity.RotationSpeed);            
                }

                //Update graphics
                if (entity.hasComponent(Masks.Graphics))
                {
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.Graphics];
                    graphics.X = (int)transform.X;
                    graphics.Y = (int)transform.Y;
                    graphics.RotationAngle = transform.Rotation;
                }

                //Update colliders
                if (entity.hasComponent(Masks.Collider))
                {
                    if (entity.hasComponent(Masks.RectangleCollider))
                    {
                        RectangleColliderComponent collider = (RectangleColliderComponent)entity.Components[Masks.RectangleCollider];
                        collider.X = (int)collider.Anchor.X;
                        collider.Y = (int)collider.Anchor.Y;
                    }

                    if (entity.hasComponent(Masks.CircleCollider))
                    {
                        CircleColliderComponent collider = (CircleColliderComponent)entity.Components[Masks.CircleCollider];
                        collider.X = (int)collider.Anchor.Center.X;
                        collider.Y = (int)collider.Anchor.Center.Y;
                    }

                    if (entity.hasComponent(Masks.LineCollider))
                    {
                        GraphicsComponent g = (GraphicsComponent)entity.Components[Masks.Graphics];
                        g.DebugPoints.Clear();

                        LineColliderComponent collider = (LineColliderComponent)entity.Components[Masks.LineCollider];
                       
                        collider.P1 = new Vector2(collider.P1.X + xVel, collider.P1.Y + yVel);
                        collider.P2 = new Vector2(collider.P2.X + xVel, collider.P2.Y + yVel);
                        Vector2 rotation1 = Vector2.Transform(collider.P1 - anchorTransform.Center, Matrix.CreateRotationZ(rotationSpeed)) + anchorTransform.Center;
                        Vector2 rotation2 = Vector2.Transform(collider.P2 - anchorTransform.Center, Matrix.CreateRotationZ(rotationSpeed)) + anchorTransform.Center;

                        collider.P1 = rotation1;
                        collider.P2 = rotation2;                       

                        g.DebugPoints.Add(collider.P1);
                        g.DebugPoints.Add(collider.P2);
                        
                    }

                    if (entity.hasComponent(Masks.PlayerHitbox))
                    {
                       GraphicsComponent g = (GraphicsComponent)entity.Components[Masks.Graphics];
                       g.DebugPoints.Clear();

                        PlayerHitboxComponent collider = (PlayerHitboxComponent)entity.Components[Masks.PlayerHitbox];

                        collider.P1 = new Vector2(collider.P1.X + xVel, collider.P1.Y + yVel);
                        collider.P2 = new Vector2(collider.P2.X + xVel, collider.P2.Y + yVel);

                        Vector2 rotation1 = Vector2.Transform(collider.P1 - transform.Center, Matrix.CreateRotationZ(rotationSpeed)) + transform.Center;
                        Vector2 rotation2 = Vector2.Transform(collider.P2 - transform.Center, Matrix.CreateRotationZ(rotationSpeed)) + transform.Center;
                        collider.P1 = rotation1;
                        collider.P2 = rotation2;

                        g.DebugPoints.Add(collider.P1);
                        g.DebugPoints.Add(collider.P2);
                    }
                }

                if (transform.HasMoved)
                    manager.addChangedEntities(entity, EntityCommands.Modify);
                transform.HasMoved = false;
            }
        }
    }
}
