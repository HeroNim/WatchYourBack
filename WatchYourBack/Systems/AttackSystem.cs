using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WatchYourBack
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

        public override void update(GameTime gameTime)
        {
            if (!listening)
            {
                manager.Input.inputFired += new EventHandler(checkInput);
                listening = true;
            }

            foreach (Entity entity in activeEntities)
            {
                WielderComponent wielderComponent = (WielderComponent)entity.Components[typeof(WielderComponent)];
                Entity weapon = wielderComponent.Weapon;
                WeaponComponent weaponComponent = (WeaponComponent)weapon.Components[typeof(WeaponComponent)];
                if(weaponComponent.Arc >= weaponComponent.MaxArc)
                {
                    manager.removeEntity(weapon);
                    entity.removeComponent(wielderComponent);
                }
            }
        }

        private void checkInput(object sender, EventArgs e)
        {
            InputArgs args = (InputArgs)e;
            if (args.InputType == Inputs.ATTACK)
            {
                
                Entity source = (Entity)sender;
                if (!source.hasComponent(Masks.WIELDER))
                {
                    WielderComponent wielder = new WielderComponent((float)SWORD.RANGE, MathHelper.ToRadians((int)SWORD.ARC));
                    VelocityComponent anchorSpeed = (VelocityComponent)source.Components[typeof(VelocityComponent)];
                    TransformComponent anchorPosition = (TransformComponent)source.Components[typeof(TransformComponent)];
                    AllegianceComponent anchorAllegiance = (AllegianceComponent)source.Components[typeof(AllegianceComponent)];

                    /*
                     * Get the angle between the mouse and the sword, and start the sword rotated 90 degrees from the mouse vector
                     */
                    MouseState ms = Mouse.GetState();
                    float xDir = ms.X - anchorPosition.X;
                    float yDir = ms.Y - anchorPosition.Y;
                    Vector2 dir = new Vector2(yDir, -xDir);
                    float rotationAngle = -(float)Math.Atan2(dir.X * Vector2.UnitY.Y, dir.Y * Vector2.UnitY.Y);
                    if (rotationAngle < 0)
                        rotationAngle = (float)(rotationAngle + Math.PI * 2);

                    source.addComponent(wielder);
                    wielder.Weapon = EFactory.createWeapon(source, anchorAllegiance.Owner, anchorPosition.Center.X, anchorPosition.Center.Y, wielder.Range, rotationAngle, anchorSpeed, manager.getTexture("WeaponTexture"));
                    manager.addEntity(wielder.Weapon);
                }
            }
            
        }

       
    }
}
