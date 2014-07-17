using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    /*
     * Checks for collisions between game objects, and resolves them appropriately; destroys destructible objects, stops moving objects etc.
     */
    public class GameCollisionSystem : ESystem
    {
        Dictionary<int, Entity> removeList;

        public GameCollisionSystem()
            : base(false, true, 4)
        {
            components += (int)Masks.TRANSFORM;
            components += (int)Masks.COLLIDER;
            removeList = new Dictionary<int, Entity>();
        }

        public override void update(TimeSpan gameTime)
        {
            
            foreach (Entity entity in activeEntities)
                if (entity.hasComponent(Masks.VELOCITY))
                {
                    TransformComponent t1 = (TransformComponent)entity.Components[Masks.TRANSFORM];

                    foreach (Entity other in activeEntities)
                    {
                        TransformComponent t2 = (TransformComponent)other.Components[Masks.TRANSFORM];
                        if (!haveSameAllegiance(entity, other) && TransformComponent.distanceBetween(t1, t2) < 100 && entity.hasComponent(Masks.VELOCITY))
                        {
                            if (entity.hasComponent(Masks.LINE_COLLIDER)) //Weapons have to check hitboxes
                            {
                                if (other.hasComponent(Masks.PLAYER_HITBOX))
                                    checkHitboxCollision(entity, other);                                
                            }
                            if (entity.hasComponent(Masks.RECTANGLE_COLLIDER)) //Bodies have to check other bodies and weapons
                            {
                                RectangleColliderComponent c1 = (RectangleColliderComponent)entity.Components[Masks.RECTANGLE_COLLIDER];
                                if (other.hasComponent(Masks.RECTANGLE_COLLIDER))
                                {
                                    RectangleColliderComponent c2 = (RectangleColliderComponent)other.Components[Masks.RECTANGLE_COLLIDER];
                                    if (HelperFunctions.checkBoxCollisions(entity, other))
                                    {
                                        if (c1.IsDestructable)
                                            remove(entity);
                                        if (c2.IsDestructable)
                                            remove(other);
                                    }
                                }
                             
                            }                                                       
                        }
                    }
                }
            foreach (Entity e in removeList.Values)
                manager.removeEntity(e);
            removeList.Clear();
        }

        private void checkHitboxCollision(Entity e1, Entity e2)
        {
            LineColliderComponent c1 = (LineColliderComponent)e1.Components[Masks.LINE_COLLIDER];
            PlayerHitboxComponent c2 = (PlayerHitboxComponent)e2.Components[Masks.PLAYER_HITBOX];

            float result1 = HelperFunctions.lineEquation(c1.P1, c1.P2, c2.P1);
            float result2 = HelperFunctions.lineEquation(c1.P1, c1.P2, c2.P2);
            float result3 = HelperFunctions.lineEquation(c2.P1, c2.P2, c1.P1);
            float result4 = HelperFunctions.lineEquation(c2.P1, c2.P2, c1.P2);

            if (result1 * result2 > 0 || result3 * result4 > 0)
                return;
                      
            remove(e1);
            if (e1.hasComponent(Masks.WEAPON))
            {
                WeaponComponent w1 = (WeaponComponent)e1.Components[Masks.WEAPON];
                ((WielderComponent)w1.Wielder.Components[Masks.WIELDER]).RemoveWeapon();
                PlayerInfoComponent info = (PlayerInfoComponent)w1.Wielder.Components[Masks.PLAYER_INFO];
                info.Score++;
            }

            manager.LevelInfo.Reset = true;
            Console.WriteLine("Player Hit");
        }

        

        private bool haveSameAllegiance(Entity e1, Entity e2)
        {
            if (!e1.hasComponent(Masks.ALLEGIANCE) || !e2.hasComponent(Masks.ALLEGIANCE))
                return false;
            AllegianceComponent a1 = (AllegianceComponent)e1.Components[Masks.ALLEGIANCE];
            AllegianceComponent a2 = (AllegianceComponent)e2.Components[Masks.ALLEGIANCE];

            if (a1.Owner == a2.Owner)
                return true;
            return false;
        }

        private void remove(Entity e)
        {
            if (!removeList.ContainsKey(e.ID))
                removeList.Add(e.ID, e);
        }
    }
}
