﻿using System;
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
        public AttackSystem() : base(false, true, 5)
        {
            components += (int)Masks.Wielder;
            components += (int)Masks.Velocity;
            components += (int)Masks.Transform;
            components += (int)Masks.Allegiance;
            components += (int)Masks.PlayerInput;
        }

        public override void Update(TimeSpan gameTime)
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
                          
                if (wielderComponent.HasWeapon)
                {
                    Entity weapon = wielderComponent.Weapon;
                    WeaponComponent weaponComponent = (WeaponComponent)weapon.Components[Masks.Weapon];
                    if (weaponComponent.Arc >= weaponComponent.MaxArc)
                    {
                        manager.RemoveEntity(weapon);
                        wielderComponent.RemoveWeapon();
                    }
                }

                if (input.SwingWeapon == true)
                {                   
                    if (wielderComponent.AttackOffCooldown)
                    {
                        if (wielderComponent.WeaponType == Weapons.SWORD)
                            if (!wielderComponent.HasWeapon)
                            {
                                Entity sword = EFactory.CreateSword(entity, anchorAllegiance.Allegiance, anchorTransform, perpAngle, (perpAngle + (float)Math.PI / 4), manager.HasGraphics());
                                wielderComponent.EquipWeapon(sword);
                                manager.AddEntity(sword);
                                SoundEffectComponent soundC = sword.GetComponent<SoundEffectComponent>();
                                OnFire(new SoundArgs(0, 0, soundC.Sounds[SoundTriggers.Initialize]));
                            }
                        wielderComponent.AttackOffCooldown = false;
                        wielderComponent.AttackCooldown.Start();
                    }
                    input.SwingWeapon = false;
                }
                if(input.ThrowWeapon == true)
                {
                    if (wielderComponent.ThrowOffCooldown)
                    {
                        Entity thrown = EFactory.CreateThrown(anchorAllegiance.Allegiance, anchorTransform.Center.X, anchorTransform.Center.Y, lookDir, lookAngle, manager.HasGraphics());
                        manager.AddEntity(thrown);                      
                        wielderComponent.ThrowOffCooldown = false;
                        wielderComponent.ThrowCooldown.Start();
                        SoundEffectComponent soundC = (SoundEffectComponent)thrown.Components[Masks.SoundEffect];
                        OnFire(new SoundArgs(0, 0, soundC.Sounds[SoundTriggers.Initialize]));
                        wielderComponent.ThrowCooldown.Start();
                    }
                    
                    input.ThrowWeapon = false;
                    
                }
            }           
        }       
    }
}
