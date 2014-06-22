using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class AttackSystem : ESystem
    {
        public AttackSystem() : base(false, true, 6)
        {
            components += WeaponComponent.bitMask;
        }

        public override void update()
        {
            throw new NotImplementedException();
        }
    }
}
