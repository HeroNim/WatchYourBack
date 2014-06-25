using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

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

        public override void update(GameTime gameTime)
        {
            foreach (Entity entity in activeEntities)
                if (entity.hasComponent(Masks.VELOCITY))
                {
                    foreach (Entity other in activeEntities)
                        if (entity != other && entity.hasComponent(Masks.VELOCITY))
                            checkCollisions(entity, other);
                    TransformComponent t1 = (TransformComponent)entity.Components[typeof(TransformComponent)];
                    t1.resetLocks();
                }
                
        }


        /*
         * Checks two entities for collisions. The first entity extends its collider in the x and y directions, and checks for collisions. If they will collide,
         * the entity is moved back one step, and the collider returns to its original position. After all collisions have been checked, the movement
         * system resets the colliders to be centered on their respective entities again. Also checks for weapons on the entity; if it has one, it makes sure
         * the weapon moves with the entity.
         * */
        private void checkCollisions(Entity e1, Entity e2)
        {

            WeaponComponent weaponComponent = null;
            TransformComponent weaponTransformComponent = null;

            VelocityComponent v1 = (VelocityComponent)e1.Components[typeof(VelocityComponent)];
            TransformComponent t1 = (TransformComponent)e1.Components[typeof(TransformComponent)];
            ColliderComponent c1 = (ColliderComponent)e1.Components[typeof(ColliderComponent)];
            ColliderComponent c2 = (ColliderComponent)e2.Components[typeof(ColliderComponent)];

            if (e1.hasComponent(Masks.WEAPON))
            {
                weaponComponent = (WeaponComponent)e1.Components[typeof(WeaponComponent)];
                Entity weapon = weaponComponent.Weapon;
                weaponTransformComponent = (TransformComponent)weapon.Components[typeof(TransformComponent)];
            }
 
                c1.X += (int)v1.X;
                if (c1.Collider.Intersects(c2.Collider))
                    if (t1.XLock != true)
                    {
                        t1.X -= v1.X;
                        t1.XLock = true;
                        if (weaponTransformComponent != null)
                            weaponTransformComponent.X -= v1.X;
                    }
                c1.X -= (int)v1.X;


                c1.Y += (int)v1.Y;
                if (c1.Collider.Intersects(c2.Collider))
                    if (t1.YLock != true)
                    {
                        t1.Y -= v1.Y;
                        t1.YLock = true;
                        if (weaponTransformComponent != null)
                            weaponTransformComponent.Y -= v1.Y;
                    }
                c1.Y -= (int)v1.Y;
        }

        private bool checkIntersection(Vector2 lineStart, Vector2 lineEnd, Rectangle rect)
        {
            bool above = false;

            Vector2 topLeft = new Vector2(rect.Left, rect.Top);
            Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 topRight = new Vector2(rect.Right, rect.Top);
            Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);

            float [] results = new float[4];

            results[0] = lineEquation(lineStart, lineEnd, topLeft);
            results[1] = lineEquation(lineStart, lineEnd, bottomLeft);
            results[2] = lineEquation(lineStart, lineEnd, topRight);
            results[3] = lineEquation(lineStart, lineEnd, bottomRight);

            if (results[0] == 0)
                return true;
            else if (results[0] > 0)
                above = true;

            for (int i = 1; i < results.Length; i++ )
            {
                if (above)
                {
                    if (results[i] <= 0)
                        return true;
                }
                else if (above == false)
                    if (results[i] >= 0)
                        return true;
            }
            return false;
        }

        private float lineEquation(Vector2 p1, Vector2 p2, Vector2 corner)
        {
            return (p2.Y - p1.Y) * corner.X + (p1.X - p2.X) * corner.Y + (p2.X * p1.Y - p1.X * p2.Y);
        }
    }
}
