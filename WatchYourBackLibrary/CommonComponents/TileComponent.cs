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
        public override int BitMask { get { return (int)Masks.TILE; } }
        public override Masks Mask { get { return Masks.TILE; } }

        private TileType type;
        private int atlasIndex;

        public TileComponent(TileType type)
        {
            this.type = type;
            atlasIndex = 0;
        }

        public TileComponent(TileType type, int index)
        {
            this.type = type;
            atlasIndex = index;
        }

        public TileType Type
        {
            get { return type; }
        }

        public int AtlasIndex
        {
            get { return atlasIndex; }
            set { atlasIndex = value; }
        }
      
        
    }
}
