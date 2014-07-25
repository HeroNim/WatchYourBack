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
                            if (entity.hasComponent(Masks.LINE_COLLIDER))
                            {
                                if (other.hasComponent(Masks.PLAYER_HITBOX)) // Line - Hitbox
                                    if (HelperFunctions.checkLine_HitboxCollision(entity, other))
                                        ResolveCollisions(entity, other, Masks.LINE_COLLIDER, Masks.PLAYER_HITBOX);
                                if (other.hasComponent(Masks.CIRCLE_COLLIDER)) // Line - Circle
                                    if (HelperFunctions.checkLine_CircleCollision(entity, other))
                                        ResolveCollisions(entity, other, Masks.LINE_COLLIDER, Masks.CIRCLE_COLLIDER);
                            }
                            if (entity.hasComponent(Masks.RECTANGLE_COLLIDER))
                            {
                                if (other.hasComponent(Masks.RECTANGLE_COLLIDER)) // Rectangle - Rectangle
                                    if (HelperFunctions.checkRectangle_RectangleCollisions(entity, other))
                                        ResolveCollisions(entity, other, Masks.RECTANGLE_COLLIDER, Masks.RECTANGLE_COLLIDER);                           
                            }                                                       
                        }
                    }
                }
            foreach (Entity e in removeList.Values)
                manager.removeEntity(e);
            removeList.Clear();
        }

       

        private void ResolveCollisions(Entity e1, Entity e2, Masks collider1, Masks collider2)
        {
            if (e1.IsDestructable)
            {
                remove(e1);
                if (e1.hasComponent(Masks.WEAPON))
                {
                    WeaponComponent wielder = (WeaponComponent)e1.Components[Masks.WEAPON];
                    if(wielder.Wielder != null)
                        ((WielderComponent)wielder.Wielder.Components[Masks.WIELDER]).RemoveWeapon();

                }
            }
            if (e2.IsDestructable)
            {
                remove(e2);
                if (e2.hasComponent(Masks.WEAPON))
                {
                    WeaponComponent wielder = (WeaponComponent)e2.Components[Masks.WEAPON];
                    if (wielder.Wielder != null)
                        ((WielderComponent)wielder.Wielder.Components[Masks.WIELDER]).RemoveWeapon();
                }
            }

            if(e1.Type == ENTITIES.SWORD)
                if(collider2 == Masks.PLAYER_HITBOX)
                {
                    WeaponComponent wielder = (WeaponComponent)e1.Components[Masks.WEAPON];
                    PlayerInfoComponent info = (PlayerInfoComponent)wielder.Wielder.Components[Masks.PLAYER_INFO];
                    info.Score++;
                    manager.LevelInfo.Reset = true;
                    Console.WriteLine("Player Hit");
                }

            if (e1.Type == ENTITIES.THROWN)
                if (e2.Type == ENTITIES.AVATAR)
                {
                    StatusComponent avatarInfo = (StatusComponent)e2.Components[Masks.STATUS];
                    avatarInfo.ApplyStatus(Status.Paralyzed, 1000f, 0);
                    Console.WriteLine("Paralyzed");
                }

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
