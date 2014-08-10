using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public class VisionBlockComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.VisionBlock; } }
        public override Masks Mask { get { return Masks.VisionBlock; } }

        Rectangle visionBlocker;

        public VisionBlockComponent(Rectangle r)
        {
            visionBlocker = r;
        }

        public Rectangle Collider
        {
            get { return visionBlocker; }
        }
    }
}
