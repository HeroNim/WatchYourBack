using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBack
{
    class TileComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.Tile;
        public override int Mask { get { return bitMask; } }

        
      
        
    }
}
