using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{  
    /// <summary>
    /// The component used to store what type of terrain a tile is, such as a wall or a spawn point.
    /// </summary>
    public class TileComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Tile; } }
        public override Masks Mask { get { return Masks.Tile; } }

        private TileType type;
        private int[,] subIndex;

        public TileComponent(TileType type)
        {
            this.type = type;
        }       

        public TileComponent(TileType type, int[,] index)
        {
            this.type = type;
            subIndex = index;
        }

        public TileType Type
        {
            get { return type; }
        }        

        public int[,] AtlasIndex
        {
            get { return subIndex; }
            set { subIndex = value; }
        }      
    }
}
