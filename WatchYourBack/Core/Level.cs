using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBack
{
   
    class Level
    {
        private Texture2D levelImage;
        private Color[] data;
        private Tile[,] tiles;

        public Level(Texture2D level)
        {
            levelImage = level;
            data = new Color[levelImage.Width * levelImage.Height];
            levelImage.GetData<Color>(data);
            tiles = new Tile[levelImage.Height, levelImage.Width];

            for (int y = 0; y < levelImage.Height; y++)
            {
                for (int x = 0; x < levelImage.Width; x++)
                {
                    tiles[y, x] = new Tile(data[y * levelImage.Width + x]);
                }
            }
        }

        public Tile[,] levelData
        {
            get { return tiles; }
        }


    }
}
