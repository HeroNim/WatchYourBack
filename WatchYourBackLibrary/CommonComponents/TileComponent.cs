using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{
    //Used to store what type a terrain tile is, such as a wall or a spawn point
    public class TileComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.TILE;
        public override Masks Mask { get { return Masks.TILE; } }

        private TileType type;

        public TileComponent(TileType type)
        {
            this.type = type;
        }

        public TileType Type
        {
            get { return type; }
        }
      
        
    }
}
