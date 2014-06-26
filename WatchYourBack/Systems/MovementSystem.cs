using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBack
{
    /*
     * The system used to move all entities. Finds all entities that have a transform and velocity component, then uses
     * the relevant data to adjust their position. Also adjusts the position of the graphics and collider components accordingly if applicable.
     */
    class MovementSystem : ESystem
    {


        public MovementSystem() : base(false, true, 5)
        {
            components += TransformComponent.bitMask;
            components += VelocityComponent.bitMask;
        }

        public override void update(GameTime gameTime)
        {
            foreach (Entity entity in activeEntities)
            {
                TransformComponent transform = (TransformComponent)entity.Components[typeof(TransformComponent)];
                
                VelocityComponent velocity = (VelocityComponent)entity.Components[typeof(VelocityComponent)];
                transform.Position = new Vector2(transform.X + velocity.X, transform.Y + velocity.Y);
                transform.Rotation += velocity.RotationSpeed;
       
                if(entity.hasComponent(Masks.GRAPHICS))
                {
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[typeof(GraphicsComponent)];
                    graphics.X = (int)transform.X;
                    graphics.Y = (int)transform.Y;
                    graphics.RotationAngle = transform.Rotation;
                    
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
                        Console.WriteLine(collider.P1);
                        Console.WriteLine(collider.P2);

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
