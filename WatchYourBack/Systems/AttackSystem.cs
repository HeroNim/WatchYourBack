using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBack
{
    class AttackSystem : ESystem
    {
        public AttackSystem(GameInputSystem input) : base(false, true, 7)
        {
            components += WeaponComponent.bitMask;
            input.inputFired += new EventHandler(checkInput);
        }

        public override void update()
        {
            throw new NotImplementedException();
        }

        private void checkInput(object sender, EventArgs e)
        {
            Entity source = (Entity)sender;
            WeaponComponent weapon = (WeaponComponent)source.Components[typeof(WeaponComponent)];
            TransformComponent anchor = (TransformComponent)source.Components[typeof(TransformComponent)];
            if (weapon.Weapon == null)
                weapon.Weapon = EFactory.createWeapon(anchor.X,anchor.Y, new Rectangle((int)anchor.X, (int)anchor.Y, 5, (int)weapon.Range), manager.getTexture("WeaponTexture"));
            
        }
    }
}
