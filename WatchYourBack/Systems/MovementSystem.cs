﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBack
{
    /*
     * The system used to move all entities. Finds all entities that have a transform and velocity component, then uses
     * the relevant data to adjust their position. Also adjusts the position of the graphics and collider components accordingly if applicable.
     */
    class MovementSystem : ESystem
    {
        public MovementSystem() : base(false, true)
        {
            components = 0;
            components += TransformComponent.bitMask;
            components += VelocityComponent.bitMask;
        }

        public override void update()
        {
            foreach (Entity entity in activeEntities)
            {
                TransformComponent transform = (TransformComponent)entity.Components[typeof(TransformComponent)];
                VelocityComponent velocity = (VelocityComponent)entity.Components[typeof(VelocityComponent)];
                transform.Position = new Vector2(transform.X + velocity.X, transform.Y + velocity.Y);
                
                if(entity.hasComponent(GraphicsComponent.bitMask))
                {
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[typeof(GraphicsComponent)];
                    graphics.X = (int)transform.X;
                    graphics.Y = (int)transform.Y;
                }
                
                if (entity.hasComponent(ColliderComponent.bitMask))
                {
                    ColliderComponent collider = (ColliderComponent)entity.Components[typeof(ColliderComponent)];
                    collider.X = (int)transform.X;
                    collider.Y = (int)transform.Y;
                }
            }
        }
    }
}
