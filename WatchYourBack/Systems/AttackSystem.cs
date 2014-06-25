using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

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
            components += WeaponComponent.bitMask;
            listening = false;
        }

        public override void update(GameTime gameTime)
        {
            if (!listening)
            {
                manager.Input.inputFired += new EventHandler(checkInput);
                listening = true;
            }
        }

        private void checkInput(object sender, EventArgs e)
        {
            InputArgs args = (InputArgs)e;
            if (args.InputType == Inputs.ATTACK)
            {
                Entity source = (Entity)sender;
                if (!source.hasComponent(Masks.WEAPON))
                {
                    WeaponComponent weapon = new WeaponComponent(10, 5, true);
                    source.addComponent(weapon);
                    VelocityComponent anchorSpeed = (VelocityComponent)source.Components[typeof(VelocityComponent)];
                    TransformComponent anchorPosition = (TransformComponent)source.Components[typeof(TransformComponent)];
                    weapon.Weapon = EFactory.createWeapon(anchorPosition.X, anchorPosition.Y, new Rectangle((int)anchorPosition.X, (int)anchorPosition.Y, 5, (int)weapon.Range), anchorSpeed, manager.getTexture("WeaponTexture"));
                    manager.addEntity(weapon.Weapon);
                }
            }
            
        }
    }
}
