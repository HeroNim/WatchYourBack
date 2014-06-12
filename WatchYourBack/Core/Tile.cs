using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBack
{
    enum TileType
    {
        WALL,
        EMPTY,
        SPAWN
    };

    class Tile
    {
        private TileType type;

        public Tile(Color color) 
        {
            if (color == Color.Black)
                type = TileType.WALL;
            if (color == Color.White)
                type = TileType.EMPTY;
            if (color == Color.Red)
                type = TileType.SPAWN;
        }

        public TileType Type
        {
            get { return type; }
        }
    }
}
