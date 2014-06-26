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
                        if (entity != other && entity.hasComponent(Masks.VELOCITY) && !other.hasComponent(Masks.LINE_COLLIDER))
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

            WielderComponent weaponComponent = null;
            TransformComponent weaponTransformComponent = null;
            LineColliderComponent weaponCollider = null;

            VelocityComponent v1 = (VelocityComponent)e1.Components[typeof(VelocityComponent)];
            TransformComponent t1 = (TransformComponent)e1.Components[typeof(TransformComponent)];
            

            if (e1.hasComponent(Masks.WIELDER))
            {
                weaponComponent = (WielderComponent)e1.Components[typeof(WielderComponent)];
                Entity weapon = weaponComponent.Weapon;
                weaponTransformComponent = (TransformComponent)weapon.Components[typeof(TransformComponent)];
                weaponCollider = (LineColliderComponent)weapon.Components[typeof(LineColliderComponent)];
            }

            if (e1.hasComponent(Masks.LINE_COLLIDER))
            {
                if (e2.hasComponent(Masks.PLAYER_INPUT))
                    return;
                else
                {
                    LineColliderComponent c1 = (LineColliderComponent)e1.Components[typeof(LineColliderComponent)];
                    ColliderComponent c2 = (ColliderComponent)e2.Components[typeof(ColliderComponent)];
                    c1.X1 += v1.X;
                    c1.X2 += v1.X;
                    c1.Y1 += v1.Y;
                    c1.Y2 += v1.Y;
                    Vector2 rotation = Vector2.Transform(c1.P2 - c1.P1, Matrix.CreateRotationZ(v1.RotationSpeed))+c1.P1;
                    c1.P2 = rotation;
                    if (checkLineCollision(c1.P1, c1.P2, c2.Collider))
                    {
                        manager.removeEntity(e1);
                        if(e1.hasComponent(Masks.WEAPON))
                        {
                            WeaponComponent w1 = (WeaponComponent)e1.Components[typeof(WeaponComponent)];
                            if(w1.Wielder.hasComponent(Masks.WIELDER))
                            {
                                WielderComponent wielderComponent = (WielderComponent)w1.Wielder.Components[typeof(WielderComponent)];
                                w1.Wielder.removeComponent(wielderComponent);
                            }
                        }
                        return;
                    }
                    rotation = Vector2.Transform(c1.P2 - c1.P1, Matrix.CreateRotationZ(-v1.RotationSpeed)) + c1.P1;
                    c1.P2 = rotation;
                    c1.X1 -= v1.X;
                    c1.X2 -= v1.X;
                    c1.Y1 -= v1.Y;
                    c1.Y2 -= v1.Y;


                    
                        
                    
                }
            }
            else
            {
                ColliderComponent c1 = (ColliderComponent)e1.Components[typeof(ColliderComponent)];
                ColliderComponent c2 = (ColliderComponent)e2.Components[typeof(ColliderComponent)];

                c1.X += (int)v1.X;
                if (c1.Collider.Intersects(c2.Collider))
                    if (t1.XLock != true)
                    {
                        t1.X -= v1.X;
                        t1.XLock = true;
                        if (weaponTransformComponent != null)
                        {
                            weaponTransformComponent.X -= v1.X;
                            weaponCollider.X1 -= v1.X;
                            weaponCollider.X2 -= v1.X;
                        }
                    }
                c1.X -= (int)v1.X;


                c1.Y += (int)v1.Y;
                if (c1.Collider.Intersects(c2.Collider))
                    if (t1.YLock != true)
                    {
                        t1.Y -= v1.Y;
                        t1.YLock = true;
                        if (weaponTransformComponent != null)
                        {
                            weaponTransformComponent.Y -= v1.Y;
                            weaponCollider.Y1 -= v1.Y;
                            weaponCollider.Y2 -= v1.Y;
                        }
                    }
                c1.Y -= (int)v1.Y;
            }
        }

        /*
         * Checks for an intersection between a line defined by two endpoints and a rectangle. First, each corner of the rectangle
         * is checked to make sure it is on the same side of the line. If they are not, then the function then checks to see if
         * each set of endpoints has at least one value within the rectangle's width or height for each axis. If there is, there is
         * an intersection.
         */
        private bool checkLineCollision(Vector2 p1, Vector2 p2, Rectangle rect)
        {
            bool above = false;
            bool possibleIntersection = false;

            Vector2 topLeft = new Vector2(rect.Left, rect.Top);
            Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 topRight = new Vector2(rect.Right, rect.Top);
            Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);

            float [] results = new float[4];

            results[0] = lineEquation(p1, p2, topLeft);
            results[1] = lineEquation(p1, p2, bottomLeft);
            results[2] = lineEquation(p1, p2, topRight);
            results[3] = lineEquation(p1, p2, bottomRight);

            
            if (results[0] > 0)
                above = true;

            for (int i = 1; i < results.Length; i++ )
            {
                if (above)
                {
                    if (results[i] <= 0)
                        possibleIntersection = true;
                }
                else if (above == false)
                    if (results[i] >= 0)
                        possibleIntersection = true;
            }
            if (possibleIntersection == false)
                return false;
            else
                if (p1.X > bottomRight.X && p2.X > bottomRight.X)
                    return false;
                if (p1.X < topLeft.X && p2.X < topLeft.X)
                    return false;
                if (p1.Y > bottomRight.Y && p2.Y > bottomRight.Y)
                    return false;
                if (p1.Y < topLeft.Y && p2.Y < topLeft.Y)
                    return false;
                return true;
        }

        private float lineEquation(Vector2 p1, Vector2 p2, Vector2 corner)
        {
            return (p2.Y - p1.Y) * corner.X + (p1.X - p2.X) * corner.Y + (p2.X * p1.Y - p1.X * p2.Y);
        }
    }
}
