using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    public enum SWORD
    {
        RANGE = 50,
        WIDTH = 5,
        SPEED = 20,
        ARC = 180
    }
    class WeaponComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.WEAPON;
        public override Masks Mask { get { return Masks.WEAPON; } }

        private float arc;
        private float maxArc;
        private Entity wielder;

        public WeaponComponent(Entity wielder, float maxArc)
        {
            this.wielder = wielder;
            arc = 0;
            this.maxArc = maxArc;
        }

        public Entity Wielder
        {
            get { return wielder; }
        }

        public float Arc
        {
            get { return arc; }
            set { arc = value; }
        }

        public float MaxArc
        {
            get { return maxArc; }
        }


    }
}
