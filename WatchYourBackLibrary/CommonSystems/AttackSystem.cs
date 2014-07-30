using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    
    /// <summary>
    /// The system responsible for waiting for attack commands, and creating the appropriate attacks, such as sword swings.
    /// </summary>
    public class AttackSystem : ESystem
    {

        public AttackSystem() : base(false, true, 6)
        {
            components += (int)Masks.Wielder;
            components += (int)Masks.Velocity;
            components += (int)Masks.Transform;
            components += (int)Masks.Allegiance;
            components += (int)Masks.PlayerInput;
        }

        public override void update(TimeSpan gameTime)
        {           
            foreach (Entity entity in activeEntities)
            {
                WielderComponent wielderComponent = (WielderComponent)entity.Components[Masks.Wielder];
                VelocityComponent anchorVelocity = (VelocityComponent)entity.Components[Masks.Velocity];
                TransformComponent anchorTransform = (TransformComponent)entity.Components[Masks.Transform];
                AllegianceComponent anchorAllegiance = (AllegianceComponent)entity.Components[Masks.Allegiance];
                AvatarInputComponent input = (AvatarInputComponent)entity.Components[Masks.PlayerInput];

                Vector2 lookDir = anchorTransform.LookDirection;
                float lookAngle = anchorTransform.LookAngle;

                //Get the angle between the mouse and the player, and start the sword rotated 90 degrees clockwise from the resulting vector
                float perpAngle = anchorTransform.Rotation + (float)Math.PI / 2;  
                          
                if (wielderComponent.hasWeapon)
                {
                    Entity weapon = wielderComponent.Weapon;
                    WeaponComponent weaponComponent = (WeaponComponent)weapon.Components[Masks.Weapon];
                    if (weaponComponent.Arc >= weaponComponent.MaxArc)
                    {
                        manager.removeEntity(weapon);
                        wielderComponent.RemoveWeapon();
                    }
                }
              
                
                if (input.SwingWeapon == true)
                {                   
                    if (wielderComponent.AttackCooldown)
                    {
                        if (wielderComponent.WeaponType == Weapons.SWORD)
                            if (!wielderComponent.hasWeapon)
                            {
                                wielderComponent.EquipWeapon(EFactory.createSword(entity, anchorAllegiance.MyAllegiance, anchorTransform, perpAngle, manager.hasGraphics()));
                                manager.addEntity(wielderComponent.Weapon);
                                wielderComponent.Weapon.hasComponent(Masks.Collider);
                            }
                        wielderComponent.AttackCooldown = false;
                        wielderComponent.AttackSpeed.Start();

                    }
                    input.SwingWeapon = false;
                }
                if(input.ThrowWeapon == true)
                {
                    if (wielderComponent.ThrowCooldown)
                    {
                        manager.addEntity(EFactory.createThrown(anchorAllegiance.MyAllegiance, anchorTransform.Center.X, anchorTransform.Center.Y, lookDir, lookAngle, manager.hasGraphics()));
                        wielderComponent.ThrowCooldown = false;
                        wielderComponent.ThrowSpeed.Start();
                    }
                    input.ThrowWeapon = false;
                }
            }           
        }       
    }
}
