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

    /*
     * Holds the info for the weapon, including it's range and the total extent of it's rotation if swung.
     */
    class WielderComponent : EComponent
    {

        public readonly static int bitMask = (int)Masks.WIELDER;
        public override Masks Mask { get { return Masks.WIELDER; } }

        private float range;
        private float arc;
        

        private Entity weapon;
        

        public WielderComponent(float range, float arc)
        {
            this.range = range;
            this.arc = arc;
        }

        public float Range
        {
            get { return range; }
        }

        public float Arc
        {
            get { return arc; }
        }

        public Entity Weapon
        {
            get { return weapon; }
            set { weapon = value; }
        }

        
    }
}
