using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackServer
{
    class DebugComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.DEBUG;
        public override Masks Mask { get { return Masks.DEBUG; } }
    }
}
