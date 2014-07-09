using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    /*
     * The system responsible for waiting for attack commands, and creating the appropriate attacks, such as sword swings. (abilities?)
     */
    public class AttackSystem : ESystem
    {

        public AttackSystem() : base(false, true, 6)
        {
            components += (int)Masks.WIELDER;
            components += (int)Masks.VELOCITY;
            components += (int)Masks.TRANSFORM;
            components += (int)Masks.ALLEGIANCE;
            components += (int)Masks.PLAYER_INPUT;
        }

        public override void update(TimeSpan gameTime)
        {           
            foreach (Entity entity in activeEntities)
            {
                WielderComponent wielderComponent = (WielderComponent)entity.Components[Masks.WIELDER];
                VelocityComponent anchorVelocity = (VelocityComponent)entity.Components[Masks.VELOCITY];
                TransformComponent anchorTransform = (TransformComponent)entity.Components[Masks.TRANSFORM];
                AllegianceComponent anchorAllegiance = (AllegianceComponent)entity.Components[Masks.ALLEGIANCE];
                AvatarInputComponent input = (AvatarInputComponent)entity.Components[Masks.PLAYER_INPUT];

                Vector2 dir = anchorTransform.LookDirection;
                float dirAngle = anchorTransform.LookAngle;
                float perpAngle = dirAngle - (float)Math.PI / 2;

                wielderComponent.AttackCooldown += gameTime.TotalMilliseconds;
                wielderComponent.ThrowCooldown += gameTime.TotalMilliseconds;

                if (wielderComponent.hasWeapon)
                {
                    Entity weapon = wielderComponent.Weapon;
                    WeaponComponent weaponComponent = (WeaponComponent)weapon.Components[Masks.WEAPON];
                    if (weaponComponent.Arc >= weaponComponent.MaxArc)
                    {
                        manager.removeEntity(weapon);
                        wielderComponent.RemoveWeapon();
                    }
                }

              
                /*
                 * Get the angle between the mouse and the player, and start the sword rotated 90 degrees clockwise from the resulting vector
                 */
                if (input.SwingWeapon == true)
                {                   
                    if (wielderComponent.AttackCooldown >= wielderComponent.AttackSpeed)
                    {
                        if (wielderComponent.WeaponType == Weapons.SWORD)
                            if (!wielderComponent.hasWeapon)
                            {
                                wielderComponent.EquipWeapon(EFactory.createSword(entity, anchorAllegiance.Owner, anchorTransform.Center.X, anchorTransform.Center.Y, perpAngle, anchorVelocity, manager.getTexture("WeaponTexture"), manager.hasGraphics()));
                                manager.addEntity(wielderComponent.Weapon);
                                wielderComponent.Weapon.hasComponent(Masks.COLLIDER);
                            }
                        wielderComponent.AttackCooldown = 0;

                    }
                    input.SwingWeapon = false;
                }
                if(input.ThrowWeapon == true)
                {
                    if (wielderComponent.ThrowCooldown >= wielderComponent.ThrowSpeed)
                    {
                        manager.addEntity(EFactory.createThrown(anchorAllegiance.Owner, anchorTransform.Center.X, anchorTransform.Center.Y, dir, manager.getTexture("WeaponTexture"), manager.hasGraphics()));
                        wielderComponent.ThrowCooldown = 0;
                    }
                    input.ThrowWeapon = false;

                }
            }           
        }       
    }
}
