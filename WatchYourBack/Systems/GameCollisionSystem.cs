using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    /*
     * Checks for collisions between game objects, and resolves them appropriately; destroys destructible objects, stops moving objects etc.
     */
    class GameCollisionSystem : ESystem
    {
        public GameCollisionSystem() : base(false, true, 4)
        {
            components += TransformComponent.bitMask;
            components += ColliderComponent.bitMask;
        }

        public override void update()
        {
            foreach (Entity entity in activeEntities)
                if (entity.hasComponent(Masks.VELOCITY))
                {
                    foreach (Entity other in activeEntities)
                        if (entity != other)
                            checkCollisions(entity, other);
                    TransformComponent t1 = (TransformComponent)entity.Components[typeof(TransformComponent)];
                    t1.resetLocks();
                }
                
        }


        /*
         * Checks two entities for collisions. The first entity extends its collider in the x and y directions, and checks for collisions. If they will collide,
         * the entity is moved back one step, and the collider returns to its original position. After all collisions have been checked, the movement
         * system resets the colliders to be centered on their respective entities again.
         * */
        private void checkCollisions(Entity e1, Entity e2)
        {
            ColliderComponent c1;
            ColliderComponent c2;

            VelocityComponent v1 = (VelocityComponent)e1.Components[typeof(VelocityComponent)];
            TransformComponent t1 = (TransformComponent)e1.Components[typeof(TransformComponent)];
                c1 = (ColliderComponent)e1.Components[typeof(ColliderComponent)];
                c2 = (ColliderComponent)e2.Components[typeof(ColliderComponent)];



            if(v1 != null)
            {
                c1.X += (int)v1.X;
                if (c1.Collider.Intersects(c2.Collider))
                    if (t1.XLock != true)
                    {
                        t1.X -= v1.X;
                        t1.XLock = true;
                    }
                c1.X -= (int)v1.X;


                c1.Y += (int)v1.Y;
                if (c1.Collider.Intersects(c2.Collider))
                    if (t1.YLock != true)
                    {
                        t1.Y -= v1.Y;
                        t1.YLock = true;
                    }
                c1.Y -= (int)v1.Y;
            }
                
            
            
            
        }
    }
}
