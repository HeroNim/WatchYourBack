using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class WeaponComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.WEAPON;
        public override Masks Mask { get { return Masks.WEAPON; } }

        private Entity wielder;

        public WeaponComponent(Entity wielder)
        {
            this.wielder = wielder;
        }

        public Entity Wielder
        {
            get { return wielder; }
        }


    }
}
