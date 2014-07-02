using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackServer
{
    /*
     * The system used to move all entities. Finds all entities that have a transform and velocity component, then uses
     * the relevant data to adjust their position. Also adjusts any variables in extra components that rely on movement.
     */
    class MovementSystem : ESystem
    {


        public MovementSystem() : base(false, true, 5)
        {
            components += TransformComponent.bitMask;
            components += VelocityComponent.bitMask;
        }

        public override void update(double lastUpdate)
        {
            foreach (Entity entity in activeEntities)
            {
                TransformComponent transform = (TransformComponent)entity.Components[typeof(TransformComponent)];
                
                VelocityComponent velocity = (VelocityComponent)entity.Components[typeof(VelocityComponent)];
                transform.Position = new Vector2(transform.X + velocity.X, transform.Y + velocity.Y);
                transform.Rotation += velocity.RotationSpeed;


                if (entity.hasComponent(Masks.WEAPON))
                {
                    WeaponComponent weapon = (WeaponComponent)entity.Components[typeof(WeaponComponent)];
                    weapon.Arc += Math.Abs(velocity.RotationSpeed);
                }
                
                if (entity.hasComponent(Masks.COLLIDER))
                {
                    if (entity.hasComponent(Masks.LINE_COLLIDER))
                    {
                        LineColliderComponent collider = (LineColliderComponent)entity.Components[typeof(LineColliderComponent)];
                        collider.P1 = new Vector2(collider.P1.X + velocity.X, collider.P1.Y + velocity.Y);
                        collider.P2 = new Vector2(collider.P2.X + velocity.X, collider.P2.Y + velocity.Y);
                        Vector2 rotation = Vector2.Transform(collider.P2 - collider.P1, Matrix.CreateRotationZ(velocity.RotationSpeed)) + collider.P1;
                        collider.P2 = rotation;                                            
                    }
                    else
                    {
                        ColliderComponent collider = (ColliderComponent)entity.Components[typeof(ColliderComponent)];
                        collider.X = (int)transform.X;
                        collider.Y = (int)transform.Y;
                    }
                }
            }
        }
    }
}
