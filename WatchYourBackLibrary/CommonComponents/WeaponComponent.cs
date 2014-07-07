using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    public enum SWORD
    {
        RANGE = 50,
        WIDTH = 6,
        SPEED = 20,
        ATTACK_SPEED = 500,
        ARC = 180,
        ROTATION_X = (int)WIDTH/2,
        ROTATION_Y = (int)RANGE
    }

    public enum THROWN
    {
        RADIUS = 10,
        ROTATION_X = RADIUS/2,
        ROTATION_Y = RADIUS/2,
        SPEED = 10,
        ATTACK_SPEED = 100
    }
    public class WeaponComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.WEAPON; } }
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

        public WeaponComponent()
        {
            this.wielder = null;
            arc = 0;
            maxArc = 0;
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
