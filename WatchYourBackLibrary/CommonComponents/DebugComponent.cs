using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    public class DebugComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.DEBUG; } }
        public override Masks Mask { get { return Masks.DEBUG; } }
    }
}
