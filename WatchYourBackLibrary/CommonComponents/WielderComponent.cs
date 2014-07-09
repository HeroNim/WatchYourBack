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
        SWORD
    }

   

    /*
     * Holds the info for the weapon, including it's range and the total extent of it's rotation if swung.
     */
    public class WielderComponent : EComponent
    {

        public override int BitMask { get { return (int)Masks.WIELDER; } }
        public override Masks Mask { get { return Masks.WIELDER; } }

        private Entity weapon;
        private Weapons weaponType;
        private double attackTimer;
        private double throwTimer;
        private double attackCooldown;
        private double throwCooldown;
        private double lastUpdate;
        
        
        public WielderComponent(Weapons weapon)
        {
            lastUpdate = 0;
            if (weapon == Weapons.SWORD)
                attackTimer = (double)SWORD.ATTACK_SPEED;
            throwTimer = (double)THROWN.ATTACK_SPEED;
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

        public double AttackSpeed
        {
            get { return attackTimer; }
            set { attackTimer = value; }
        }

        public double AttackCooldown
        {
            get { return attackCooldown; }
            set { attackCooldown = value; }
        }

        public double ThrowSpeed
        {
            get { return throwTimer; }
            set { throwTimer = value; }
        }

        public double ThrowCooldown
        {
            get { return throwCooldown; }
            set { throwCooldown = value; }
        }

        public double LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }

        
    }
}
