using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackServer
{
    class TimerComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.TIMER;
        public override Masks Mask { get { return Masks.TIMER; } }

        
    }
}
