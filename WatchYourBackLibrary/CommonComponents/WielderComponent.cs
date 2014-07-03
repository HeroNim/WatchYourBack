using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public enum Weapons
    {
        THROWN,
        SWORD

    }

   

    /*
     * Holds the info for the weapon, including it's range and the total extent of it's rotation if swung.
     */
    public class WielderComponent : EComponent
    {

        public readonly static int bitMask = (int)Masks.WIELDER;
        public override Masks Mask { get { return Masks.WIELDER; } }

        private Entity weapon;
        private Weapons weaponType;
        private double attackTimer;
        private double elapsedTime;
        private double lastUpdate;
        
        
        public WielderComponent(Weapons weapon)
        {
            lastUpdate = 0;
            if (weapon == Weapons.SWORD)
                attackTimer = (int)SWORD.ATTACK_SPEED;
            else if (weapon == Weapons.THROWN)
                attackTimer = (int)THROWN.ATTACK_SPEED;
            weaponType = weapon;
        }

        public Entity Weapon
        {
            get { return weapon; }
            set { weapon = value; }
        }

        public Weapons WeaponType
        {
            get { return weaponType; }
            set { weaponType = value; }
        }

        public bool hasWeapon
        {
            get { return (weapon != null); }
        }

        public void RemoveWeapon()
        {
            if (weapon == null)
                return;
            else
                weapon = null;
        }

        public void EquipWeapon(Entity weapon)
        {
            if (this.weapon != null)
                return;
            else
                this.weapon = weapon;
        }

        public double ElapsedTime
        {
            get { return elapsedTime; }
            set { elapsedTime = value; }
        }

        public double AttackSpeed
        {
            get { return attackTimer; }
            set { attackTimer = value; }
        }

        public double LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }

        
    }
}
