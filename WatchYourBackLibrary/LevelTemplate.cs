using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{
    [Serializable()]
    public enum TileType
    {
        WALL = 20,
        SPAWN = 21,
        EMPTY = 0
        
    };

    [Serializable()]
    public enum LevelName
    {
        FIRST_LEVEL
    };

   [Serializable()]
    public class LevelTemplate : ISerializable
    {
        private Texture2D levelImage;
        private Color[] data;
        private int[,] levelData;
        private int[,] tileTextureIndex;

        private LevelName name;

        public LevelTemplate(Texture2D level, LevelName name)
        {
            this.name = name;
            levelImage = level;
            data = new Color[levelImage.Width * levelImage.Height];
            levelImage.GetData<Color>(data);

            levelData = new int[levelImage.Height, levelImage.Width];
            tileTextureIndex = new int[levelImage.Height, levelImage.Width];

            for (int y = 0; y < levelImage.Height; y++)
            {
                for (int x = 0; x < levelImage.Width; x++)
                {
                    if (data[y * levelImage.Width + x] == Color.Black)
                        levelData[y,x] = (int)TileType.WALL;
                    if (data[y * levelImage.Width + x] == Color.White)
                        levelData[y, x] = (int)TileType.EMPTY;
                    if (data[y * levelImage.Width + x] == Color.Red)
                        levelData[y, x] = (int)TileType.SPAWN;
                }
            }

            int wallAtlasIndex = 0;

            for (int y = 0; y < levelImage.Height; y++)
            {
                for (int x = 0; x < levelImage.Width; x++)
                {
                    if (levelData[y, x] == (int)TileType.WALL)
                    {
                        if (y - 1 >= 0 && levelData[y - 1, x] == (int)TileType.WALL)
                            wallAtlasIndex += 1;
                        if (x + 1 < levelImage.Width && levelData[y, x + 1] == (int)TileType.WALL)
                            wallAtlasIndex += 2;
                        if (y + 1 < levelImage.Height && levelData[y + 1, x] == (int)TileType.WALL)
                            wallAtlasIndex += 4;
                        if (x - 1 >= 0 && levelData[y, x - 1] == (int)TileType.WALL)
                            wallAtlasIndex += 8;

                        tileTextureIndex[y, x] = wallAtlasIndex;
                        wallAtlasIndex = 0;
                    }
                    
                }
            }
        }

       public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("levelData", levelData, typeof(int[,]));
            info.AddValue("tileIndex", tileTextureIndex, typeof(int[,]));
            info.AddValue("levelName", name, typeof(LevelName));
        }

       public LevelTemplate(SerializationInfo info, StreamingContext context)
       {
           levelData = (int[,])info.GetValue("levelData", typeof(int[,]));
           name = (LevelName)info.GetValue("levelName", typeof(LevelName));
           tileTextureIndex = (int[,])info.GetValue("tileIndex", typeof(int[,]));
       }

        public int[,] LevelData
        {
            get { return levelData; }
        }

        public int[,] TileIndex
        {
            get { return tileTextureIndex; }
        }

        public LevelName Name
        {
            get { return name; }
        }


    }
}
