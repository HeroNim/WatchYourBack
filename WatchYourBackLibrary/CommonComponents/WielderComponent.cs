using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public enum Weapons
    {
        SWORD
    }
    
    /// <summary>
    /// The component which holds the info for the wielder of a weapon, such as the cooldown on their attacks and the type of weapons they use.
    /// </summary>
    public class WielderComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Wielder; } }
        public override Masks Mask { get { return Masks.Wielder; } }

        private Entity weapon;
        private Weapons weaponType;
        private Timer attackTimer;
        private Timer throwTimer;
        private bool attackCooldown;
        private bool throwCooldown;
        private double lastUpdate;
               
        public WielderComponent(Weapons weapon)
        {
            lastUpdate = 0;
            if (weapon == Weapons.SWORD)
            {
                attackTimer = new Timer((double)SWORD.ATTACK_SPEED);
                attackTimer.Elapsed += WeaponReady;
            }
            throwTimer = new Timer((double)THROWN.ATTACK_SPEED);
            throwTimer.Elapsed += ThrownReady;
            weaponType = weapon;
            attackTimer.Start();
            throwTimer.Start();
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

        public bool HasWeapon
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

        public Timer AttackCooldown
        {
            get { return attackTimer; }
            set { attackTimer = value; }
        }

        public bool AttackOffCooldown
        {
            get { return attackCooldown; }
            set { attackCooldown = value; }
        }

        public Timer ThrowCooldown
        {
            get { return throwTimer; }
            set { throwTimer = value; }
        }

        public bool ThrowOffCooldown
        {
            get { return throwCooldown; }
            set { throwCooldown = value; }
        }

        public double LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }

        private void WeaponReady(object sender, EventArgs e)
        {
            attackCooldown = true;
            attackTimer.Stop();
        }

        private void ThrownReady(object sender, EventArgs e)
        {
            throwCooldown = true;
            throwTimer.Stop();
        }       
    }
}
