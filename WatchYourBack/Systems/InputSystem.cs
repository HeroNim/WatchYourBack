using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class InputSystem : ESystem
    {
        public InputSystem() : base(false, true)
        {
            components = 0;
            components += VelocityComponent.bitMask;
        }
    }
}
