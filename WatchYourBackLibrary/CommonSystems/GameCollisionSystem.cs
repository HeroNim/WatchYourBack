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

    /// <summary>
    /// The system responsible for checking for collisions between game objects, and resolving them appropriately; destroy destructible objects, stop moving objects etc.
    /// </summary>
    public class GameCollisionSystem : ESystem
    {
        Dictionary<int, Entity> removeList;

        public GameCollisionSystem()
            : base(false, true, 4)
        {
            components += (int)Masks.Transform;
            components += (int)Masks.Collider;
            removeList = new Dictionary<int, Entity>();
        }

        /// <summary>
        /// Checks entities against other entities every update cycle, and calls the appropriate collision check for each pairing; then, any collisions are resolved.
        /// </summary>
        /// <remarks>Checks entities that have velocity components against other entities with different allegiances. This reduces the majority of unneccessary collision checks.</remarks>
        /// <param name="gameTime">The time since the last update</param>
        public override void update(TimeSpan gameTime)
        {    
            foreach (Entity entity in activeEntities)
                if (entity.hasComponent(Masks.Velocity))
                {
                    TransformComponent t1 = (TransformComponent)entity.Components[Masks.Transform];
                    foreach (Entity other in activeEntities)
                    {
                        TransformComponent t2 = (TransformComponent)other.Components[Masks.Transform];
                        if (!haveSameAllegiance(entity, other) && TransformComponent.distanceBetween(t1, t2) < 100)
                        {
                            if (entity.hasComponent(Masks.LineCollider))
                            {
                                if (other.hasComponent(Masks.PlayerHitbox)) // Line - Hitbox
                                    if (HelperFunctions.checkLine_HitboxCollision(entity, other))
                                        ResolveCollisions(entity, other, Masks.LineCollider, Masks.PlayerHitbox);
                                if (other.hasComponent(Masks.CircleCollider)) // Line - Circle
                                    if (HelperFunctions.checkLine_CircleCollision(entity, other))
                                        ResolveCollisions(entity, other, Masks.LineCollider, Masks.CircleCollider);
                            }
                            if (entity.hasComponent(Masks.RectangleCollider))
                            {
                                if (other.hasComponent(Masks.RectangleCollider)) // Rectangle - Rectangle
                                    if (HelperFunctions.checkRectangle_RectangleCollisions(entity, other))
                                        ResolveCollisions(entity, other, Masks.RectangleCollider, Masks.RectangleCollider);                           
                            }                                                       
                        }
                    }
                }
            foreach (Entity e in removeList.Values)
                manager.removeEntity(e);
            removeList.Clear();
        }

       
        /// <summary>
        /// Resolves collisions, with different results depending on the types of colliders that interacted
        /// </summary>
        /// <param name="e1">The first entity</param>
        /// <param name="e2">The second entity</param>
        /// <param name="collider1">The type of the first entity's collider</param>
        /// <param name="collider2">The type of the second entity's collider</param>
        private void ResolveCollisions(Entity e1, Entity e2, Masks collider1, Masks collider2)
        {           
            if (e1.IsDestructable)
            {
                remove(e1);
                if (e1.hasComponent(Masks.Weapon))
                {
                    WeaponComponent weaponC = (WeaponComponent)e1.Components[Masks.Weapon];
                    if(weaponC.Wielder != null)
                        ((WielderComponent)weaponC.Wielder.Components[Masks.Wielder]).RemoveWeapon();
                }
                if(e1.hasComponent(Masks.SoundEffect))
                {
                    SoundEffectComponent soundC = (SoundEffectComponent)e1.Components[Masks.SoundEffect];
                    onFire(new SoundArgs(0, 0, soundC.Sounds[SoundTriggers.Destroy]));
                }
            }
            if (e2.IsDestructable)
            {
                remove(e2);
                if (e2.hasComponent(Masks.Weapon))
                {
                    WeaponComponent wielder = (WeaponComponent)e2.Components[Masks.Weapon];
                    if (wielder.Wielder != null)
                        ((WielderComponent)wielder.Wielder.Components[Masks.Wielder]).RemoveWeapon();
                }
                if (e2.hasComponent(Masks.SoundEffect))
                {
                    SoundEffectComponent soundC = (SoundEffectComponent)e2.Components[Masks.SoundEffect];
                    onFire(new SoundArgs(0, 0, soundC.Sounds[SoundTriggers.Destroy]));
                }
            }

            if(e1.Type == Entities.Sword)
                if(collider2 == Masks.PlayerHitbox)
                {
                    WeaponComponent wielder = (WeaponComponent)e1.Components[Masks.Weapon];
                    PlayerInfoComponent info = (PlayerInfoComponent)wielder.Wielder.Components[Masks.PlayerInfo];
                    info.Score++;
                    manager.LevelInfo.Reset = true;
                    Console.WriteLine("Player Hit");
                }

            if (e1.Type == Entities.Thrown)
                if (e2.Type == Entities.Avatar)
                {
                    StatusComponent avatarInfo = (StatusComponent)e2.Components[Masks.Status];
                    avatarInfo.ApplyStatus(Status.Paralyzed, 1000f, 0);
                    Console.WriteLine("Paralyzed");
                    onFire(new SoundArgs(0, 0, "Sounds/SFX/StunSound"));
                }

        }

        

        private bool haveSameAllegiance(Entity e1, Entity e2)
        {
            if (!e1.hasComponent(Masks.Allegiance) || !e2.hasComponent(Masks.Allegiance))
                return false;
            AllegianceComponent a1 = (AllegianceComponent)e1.Components[Masks.Allegiance];
            AllegianceComponent a2 = (AllegianceComponent)e2.Components[Masks.Allegiance];

            if (a1.MyAllegiance == a2.MyAllegiance)
                return true;
            return false;
        }

        private void remove(Entity e)
        {
            if (!removeList.ContainsKey(e.ClientID))
                removeList.Add(e.ClientID, e);
        }
    }
}
