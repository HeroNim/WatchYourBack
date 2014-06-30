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
        private int elapsedTime;


        public AttackSystem() : base(false, true, 7)
        {
            components += WielderComponent.bitMask;
            listening = false;
            elapsedTime = 0;
        }

        public override void update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
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
                    float xDir = ms.X - anchorPosition.Center.X;
                    float yDir = ms.Y - anchorPosition.Center.Y;
                    Vector2 dir = new Vector2(xDir, yDir);
                    dir.Normalize();
                    Vector2 perpDir = new Vector2(yDir, -xDir);
                    float rotationAngle = -(float)Math.Atan2(perpDir.X * Vector2.UnitY.Y, perpDir.Y * Vector2.UnitY.Y);
                    if (rotationAngle < 0)
                        rotationAngle = (float)(rotationAngle + Math.PI * 2);
                    if (elapsedTime > (int)THROWN.ATTACK_SPEED)
                    {
                        manager.addEntity(EFactory.createRangedWeapon(anchorAllegiance.Owner, anchorPosition.Center.X, anchorPosition.Center.Y, dir, manager.getTexture("WeaponTexture")));
                        elapsedTime %= (int)THROWN.ATTACK_SPEED;
                    }
                    //source.addComponent(wielder);
                    //wielder.Weapon = EFactory.createMeleeWeapon(source, anchorAllegiance.Owner, anchorPosition.Center.X, anchorPosition.Center.Y, wielder.Range, rotationAngle, anchorSpeed, manager.getTexture("WeaponTexture"));
                    //manager.addEntity(wielder.Weapon);
                }
            }
            
        }

       
    }
}
