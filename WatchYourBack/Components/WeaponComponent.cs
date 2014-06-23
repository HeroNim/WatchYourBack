using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBack
{
    enum Weapons
    {
        DAGGER,
        SWORD

    }

    class WeaponComponent : EComponent
    {

        public readonly static int bitMask = (int)Masks.WEAPON;
        public override Masks Mask { get { return Masks.WEAPON; } }

        private float range;
        private float speed;
        private bool melee;

        private Entity weapon;
        

        public WeaponComponent(float range, float speed, bool melee)
        {
            this.range = range;
            this.speed = speed;
            this.melee = melee;
            
        }

        public float Range
        {
            get { return range; }
        }

        public Entity Weapon
        {
            get { return weapon; }
            set { weapon = value; }
        }

        
    }
}
