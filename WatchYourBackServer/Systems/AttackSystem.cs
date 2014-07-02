using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Lidgren.Network;

namespace WatchYourBackServer
{
    /*
     * The system responsible for waiting for attack commands, and creating the appropriate attacks, such as sword swings. (abilities?)
     */
    class AttackSystem : ESystem
    {
        private bool listening;


        public AttackSystem() : base(false, true, 7)
        {
            components += WielderComponent.bitMask;
            listening = false;
        }

        public override void update(double lastUpdate)
        {
            if (!listening)
            {
                manager.Input.inputFired += new EventHandler(checkInput);
                listening = true;

            }
            Console.WriteLine(NetTime.Now - lastUpdate);

            foreach (Entity entity in activeEntities)
            {
                WielderComponent wielderComponent = (WielderComponent)entity.Components[typeof(WielderComponent)];
                wielderComponent.ElapsedTime += NetTime.Now - lastUpdate;
                
                
                if (wielderComponent.hasWeapon)
                {
                    Entity weapon = wielderComponent.Weapon;
                    WeaponComponent weaponComponent = (WeaponComponent)weapon.Components[typeof(WeaponComponent)];
                    if (weaponComponent.Arc >= weaponComponent.MaxArc)
                    {
                        manager.removeEntity(weapon);
                        wielderComponent.RemoveWeapon();
                    }
                }
            }
        }

        private void checkInput(object sender, EventArgs e)
        {
            InputArgs args = (InputArgs)e;
            if (args.InputType == Inputs.ATTACK)
            {

                Entity source = (Entity)sender;
                WielderComponent wielderComponent = (WielderComponent)source.Components[typeof(WielderComponent)];
                VelocityComponent anchorSpeed = (VelocityComponent)source.Components[typeof(VelocityComponent)];
                TransformComponent anchorPosition = (TransformComponent)source.Components[typeof(TransformComponent)];
                AllegianceComponent anchorAllegiance = (AllegianceComponent)source.Components[typeof(AllegianceComponent)];

                /*
                 * Get the angle between the mouse and the sword, and start the sword rotated 90 degrees from the mouse vector
                 */
                MouseState ms = Mouse.GetState();
                float xDir = ms.X - anchorPosition.Center.X;
                float yDir = ms.Y - anchorPosition.Center.Y;
                Vector2 dir = new Vector2(xDir, yDir);
                dir.Normalize();
                Vector2 perpDir = new Vector2(yDir, -xDir);
                float rotationAngle = -(float)Math.Atan2(perpDir.X * Vector2.UnitY.Y, perpDir.Y * Vector2.UnitY.Y);
                if (rotationAngle < 0)
                    rotationAngle = (float)(rotationAngle + Math.PI * 2);

                if (wielderComponent.ElapsedTime >= wielderComponent.AttackSpeed)
                {
                    if (wielderComponent.WeaponType == Weapons.THROWN)
                    {                       
                            manager.addEntity(EFactory.createThrown(anchorAllegiance.Owner, anchorPosition.Center.X, anchorPosition.Center.Y, dir));
                            
                    }
                    else if (wielderComponent.WeaponType == Weapons.SWORD)
                    {
                        if (!wielderComponent.hasWeapon)
                        {
                            wielderComponent.EquipWeapon(EFactory.createSword(source, anchorAllegiance.Owner, anchorPosition.Center.X, anchorPosition.Center.Y, rotationAngle, anchorSpeed));
                            manager.addEntity(wielderComponent.Weapon);
                        }
                    }
                    wielderComponent.ElapsedTime = 0;
                    
                }
                

            }

        }

       
    }
}
