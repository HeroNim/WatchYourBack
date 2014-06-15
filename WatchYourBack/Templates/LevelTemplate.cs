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
   
    class LevelTemplate
    {
        private Texture2D levelImage;
        private Color[] data;
        private TileType[,] levelData;

        public LevelTemplate(Texture2D level)
        {
            levelImage = level;
            data = new Color[levelImage.Width * levelImage.Height];
            levelImage.GetData<Color>(data);

            levelData = new TileType[levelImage.Height, levelImage.Width];

            for (int y = 0; y < levelImage.Height; y++)
            {
                for (int x = 0; x < levelImage.Width; x++)
                {
                    if (data[y * levelImage.Width + x] == Color.Black)
                        levelData[y,x] = TileType.WALL;
                    if (data[y * levelImage.Width + x] == Color.White)
                        levelData[y, x] = TileType.EMPTY;
                    if (data[y * levelImage.Width + x] == Color.Red)
                        levelData[y, x] = TileType.SPAWN;
                }
            }
        }

        public TileType[,] LevelData
        {
            get { return levelData; }
        }


    }
}
