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
        WIDTH = 8,
        SPEED = 6,
        ATTACK_SPEED = 500,
        ARC = 110,
        ROTATION_X = (int)WIDTH/2,
        ROTATION_Y = (int)RANGE
    }

    public enum THROWN
    {
        RADIUS = 10,
        ROTATION_X = RADIUS/2,
        ROTATION_Y = RADIUS/2,
        SPEED = 11,
        ATTACK_SPEED = 500
    }

    
    /// <summary>
    /// The component which contains info regarding the various properties of weapons, such as their arc,
    /// and if they are anchored to their wielder (such as a sword) or not (thrown weapon)
    /// </summary>
    public class WeaponComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Weapon; } }
        public override Masks Mask { get { return Masks.Weapon; } }

        private bool anchored;
        private float arc;
        private float maxArc;

        private Entity wielder;

        public WeaponComponent(Entity wielder, float maxArc, bool anchored)
        {
            this.wielder = wielder;
            arc = 0;
            this.maxArc = maxArc;
            this.anchored = anchored;
        }

        public WeaponComponent()
        {
            this.wielder = null;
            arc = 0;
            maxArc = 0;
            this.anchored = false;
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

        public bool Anchored
        {
            get { return anchored; }
        }


    }
}
