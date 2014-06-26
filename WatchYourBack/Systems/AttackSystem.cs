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
                WielderComponent weaponComponent = (WielderComponent)entity.Components[typeof(WielderComponent)];
                Entity weapon = weaponComponent.Weapon;
                TransformComponent transform = (TransformComponent)weapon.Components[typeof(TransformComponent)];
                if(transform.Rotation >= weaponComponent.Arc)
                {
                    manager.removeEntity(weapon);
                    entity.removeComponent(weaponComponent);
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
                    WielderComponent wielder = new WielderComponent(10, (float)(1000*Math.PI));
                    source.addComponent(wielder);
                    VelocityComponent anchorSpeed = (VelocityComponent)source.Components[typeof(VelocityComponent)];
                    TransformComponent anchorPosition = (TransformComponent)source.Components[typeof(TransformComponent)];
                    wielder.Weapon = EFactory.createWeapon(source, anchorPosition.X - 5, anchorPosition.Y - wielder.Range, new Rectangle((int)anchorPosition.X, (int)anchorPosition.Y, 5, (int)wielder.Range), anchorSpeed, manager.getTexture("WeaponTexture"));
                    manager.addEntity(wielder.Weapon);
                }
            }
            
        }
    }
}
