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
        public GameCollisionSystem()
            : base(false, true, 4)
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
                        if(!haveSameAllegiance(entity, other))
                            if (entity != other && entity.hasComponent(Masks.VELOCITY) && !other.hasComponent(Masks.LINE_COLLIDER))
                            {
                                if (!entity.hasComponent(Masks.LINE_COLLIDER))
                                    checkBoxCollisions(entity, other);
                                else if (entity.hasComponent(Masks.LINE_COLLIDER))
                                    checkLineCollision(entity, other);
                            }
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
        private void checkBoxCollisions(Entity e1, Entity e2)
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

        /*
         * Checks for an intersection between a line defined by two endpoints and a rectangle. After predicting forward the collider, each corner of the rectangle
         * is checked to make sure it is on the same side of the line. If they are not, then the function then checks to see if
         * each set of endpoints has at least one value within the rectangle's width or height for each axis. If there is, there is
         * an intersection.
         * 
         * At this point, only weapons have line colliders, so if there is an intersection, the method simply removes the weapon
         */
        private void checkLineCollision(Entity e1, Entity e2)
        {
            
            LineColliderComponent c1 = (LineColliderComponent)e1.Components[typeof(LineColliderComponent)];
            ColliderComponent c2 = (ColliderComponent)e2.Components[typeof(ColliderComponent)];
            VelocityComponent v1 = (VelocityComponent)e1.Components[typeof(VelocityComponent)];

            c1.X1 += v1.X;
            c1.X2 += v1.X;
            c1.Y1 += v1.Y;
            c1.Y2 += v1.Y;
            Vector2 rotation = Vector2.Transform(c1.P2 - c1.P1, Matrix.CreateRotationZ(v1.RotationSpeed)) + c1.P1;
            c1.P2 = rotation;

            bool above = false;
            bool possibleIntersection = false;
            bool intersection = false;

            Vector2 topLeft = new Vector2(c2.Collider.Left, c2.Collider.Top);
            Vector2 bottomLeft = new Vector2(c2.Collider.Left, c2.Collider.Bottom);
            Vector2 topRight = new Vector2(c2.Collider.Right, c2.Collider.Top);
            Vector2 bottomRight = new Vector2(c2.Collider.Right, c2.Collider.Bottom);

            float[] results = new float[4];

            results[0] = lineEquation(c1.P1, c1.P2, topLeft);
            results[1] = lineEquation(c1.P1, c1.P2, bottomLeft);
            results[2] = lineEquation(c1.P1, c1.P2, topRight);
            results[3] = lineEquation(c1.P1, c1.P2, bottomRight);


            if (results[0] > 0)
                above = true;

            for (int i = 1; i < results.Length; i++)
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
                intersection = false;
            else
            {
                if (c1.X1 > bottomRight.X && c1.X2 > bottomRight.X)
                    intersection = false;
                else if (c1.X1 < topLeft.X && c1.X2 < topLeft.X)
                    intersection = false;
                else if (c1.Y1 > bottomRight.Y && c1.Y2 > bottomRight.Y)
                    intersection = false;
                else if (c1.Y1 < topLeft.Y && c1.Y2 < topLeft.Y)
                    intersection = false;
                else
                    intersection = true;
            }

            rotation = Vector2.Transform(c1.P2 - c1.P1, Matrix.CreateRotationZ(-v1.RotationSpeed)) + c1.P1;
            c1.P2 = rotation;
            c1.X1 -= v1.X;
            c1.X2 -= v1.X;
            c1.Y1 -= v1.Y;
            c1.Y2 -= v1.Y;

            if (intersection == true)
            {
                manager.removeEntity(e1);
                if (e1.hasComponent(Masks.WEAPON))
                {
                    WeaponComponent w1 = (WeaponComponent)e1.Components[typeof(WeaponComponent)];
                    if (w1.Wielder.hasComponent(Masks.WIELDER))
                        w1.Wielder.removeComponent((WielderComponent)w1.Wielder.Components[typeof(WielderComponent)]);
                }
            }
        }

        private float lineEquation(Vector2 p1, Vector2 p2, Vector2 corner)
        {
            return (p2.Y - p1.Y) * corner.X + (p1.X - p2.X) * corner.Y + (p2.X * p1.Y - p1.X * p2.Y);
        }

        private bool haveSameAllegiance(Entity e1, Entity e2)
        {
            if (!e1.hasComponent(Masks.ALLEGIANCE) || !e2.hasComponent(Masks.ALLEGIANCE))
                return false;
            AllegianceComponent a1 = (AllegianceComponent)e1.Components[typeof(AllegianceComponent)];
            AllegianceComponent a2 = (AllegianceComponent)e2.Components[typeof(AllegianceComponent)];

            if (a1.Owner == a2.Owner)
                return true;
            return false;
        }
    }
}
