using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class CollisionSystem : ESystem
    {
        public CollisionSystem() : base(false, true)
        {
            components = 0;
            components += TransformComponent.bitMask;
            components += ColliderComponent.bitMask;
        }

        public override void update()
        {
            foreach (Entity entity in activeEntities)
                foreach (Entity other in activeEntities)
                    if(entity != other)
                        checkCollisions(entity, other);
                
        }


        /*
         * Checks two entities for collisions. The first entity extends its collider in the x and y directions, and checks for collisions. If they will collide,
         * the entity is moved back one step, and the collider returns to its original position. After all collisions have been checked, the movement
         * system resets the colliders to be centered on their respective entities again.
         * */
        private void checkCollisions(Entity e1, Entity e2)
        {
            VelocityComponent v1 = null;
            TransformComponent t1 = (TransformComponent)e1.Components[typeof(TransformComponent)];
            ColliderComponent c1 = (ColliderComponent)e1.Components[typeof(ColliderComponent)];
            ColliderComponent c2 = (ColliderComponent)e2.Components[typeof(ColliderComponent)];

            if (e1.hasComponent(VelocityComponent.bitMask))
                v1 = (VelocityComponent)e1.Components[typeof(VelocityComponent)];

            if(v1 != null)
            {
                c1.X += (int)v1.X;
                if (c1.Collider.Intersects(c2.Collider))
                    t1.X -= v1.X;
                c1.X -= (int)v1.X;


                c1.Y += (int)v1.Y;
                if (c1.Collider.Intersects(c2.Collider))
                    t1.Y -= v1.Y;
                c1.Y -= (int)v1.Y;
            }
                
            
            
            
        }
    }
}
