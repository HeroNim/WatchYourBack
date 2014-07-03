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
        WALL = 1,
        EMPTY = 0,
        SPAWN = 2
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
        private LevelName name;

        public LevelTemplate(Texture2D level, LevelName name)
        {
            this.name = name;
            levelImage = level;
            data = new Color[levelImage.Width * levelImage.Height];
            levelImage.GetData<Color>(data);

            levelData = new int[levelImage.Height, levelImage.Width];

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
        }

       public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("levelData", levelData, typeof(int[,]));
            info.AddValue("levelName", name, typeof(LevelName));
        }

       public LevelTemplate(SerializationInfo info, StreamingContext context)
       {
           levelData = (int[,])info.GetValue("levelData", typeof(int[,]));
           name = (LevelName)info.GetValue("levelName", typeof(LevelName));
       }

        public int[,] LevelData
        {
            get { return levelData; }
        }

        public LevelName Name
        {
            get { return name; }
        }


    }
}
