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
        private int[,] cornerTextureIndex;

        private LevelName name;

        public LevelTemplate(Texture2D level, LevelName name)
        {
            this.name = name;
            levelImage = level;
            data = new Color[levelImage.Width * levelImage.Height];
            levelImage.GetData<Color>(data);

            levelData = new int[levelImage.Height, levelImage.Width];
            tileTextureIndex = new int[levelImage.Height, levelImage.Width];
            cornerTextureIndex = new int[levelImage.Height * 2, levelImage.Width * 2];
            for (int y = 0; y < cornerTextureIndex.GetLength(1); y++)
                for (int x = 0; x < cornerTextureIndex.GetLength(0); x++)
                    cornerTextureIndex[x, y] = -1;


                for (int y = 0; y < levelImage.Height; y++)
                {
                    for (int x = 0; x < levelImage.Width; x++)
                    {
                        if (data[y * levelImage.Width + x] == Color.Black)
                            levelData[y, x] = (int)TileType.WALL;
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
                        for (int i = y * 2; i <= y * 2 + 1; i++)
                        {
                            for (int j = x * 2; j <= x * 2 + 1; j++)
                            {
                                if (j % 2 == 0) //Left side
                                {

                                    if (x - 1 >= 0 && levelData[y, x - 1] == (int)TileType.WALL)
                                        wallAtlasIndex += 1; //Left

                                    if (i % 2 == 0) //Top
                                    {
                                        if (y - 1 >= 0 && x - 1 >= 0 && levelData[y - 1, x - 1] == (int)TileType.WALL)
                                            wallAtlasIndex += 2; //Top Left
                                        if (y - 1 >= 0 && levelData[y - 1, x] == (int)TileType.WALL)
                                            wallAtlasIndex += 4; //Top
                                    }
                                    else //Bottom
                                    {
                                        if (y + 1 < levelImage.Height && x - 1 >= 0 && levelData[y + 1, x - 1] == (int)TileType.WALL)
                                            wallAtlasIndex += 2; //Bottom Left
                                        if (y + 1 < levelImage.Height && levelData[y + 1, x] == (int)TileType.WALL)
                                            wallAtlasIndex += 4; //Bottom
                                    }
                                }
                                else //Right side
                                {
                                    if (x + 1 < levelImage.Width && levelData[y, x + 1] == (int)TileType.WALL)
                                        wallAtlasIndex += 1; //Right

                                    if (i % 2 == 0) //Top
                                    {
                                        if (y - 1 >= 0 && x + 1 < levelImage.Width && levelData[y - 1, x + 1] == (int)TileType.WALL)
                                            wallAtlasIndex += 2; //Top Right
                                        if (y - 1 >= 0 && levelData[y - 1, x] == (int)TileType.WALL)
                                            wallAtlasIndex += 4; //Top
                                    }
                                    else //Bottom
                                    {
                                        if (y + 1 < levelImage.Height && x + 1 < levelImage.Width && levelData[y + 1, x + 1] == (int)TileType.WALL)
                                            wallAtlasIndex += 2; //Bottom Right
                                        if (y + 1 < levelImage.Height && levelData[y + 1, x] == (int)TileType.WALL)
                                            wallAtlasIndex += 4; //Bottom
                                    }
                                }

                                if (i % 2 == 0)
                                {
                                    if(wallAtlasIndex == 3)                                        
                                        wallAtlasIndex = 1;

                                    if (j % 2 != 0 && (wallAtlasIndex == 2 || wallAtlasIndex == 5))
                                        wallAtlasIndex += 6;
                                    if (j % 2 != 0 && wallAtlasIndex == 0)
                                        wallAtlasIndex += 14;                                
                                }
                                if (i % 2 != 0)
                                {
                                    if (wallAtlasIndex == 1)
                                        wallAtlasIndex = 3;

                                    if (j % 2 == 0 && (wallAtlasIndex == 2 || wallAtlasIndex == 5))
                                        wallAtlasIndex += 7;
                                    if (j % 2 != 0 && (wallAtlasIndex == 2 || wallAtlasIndex == 5))
                                        wallAtlasIndex += 8;
                                    if (j % 2 == 0 && wallAtlasIndex == 0)
                                        wallAtlasIndex += 15;
                                    if (j % 2 != 0 && wallAtlasIndex == 0)
                                        wallAtlasIndex += 16;
                                }

                                if(j % 2 == 0)
                                    if (wallAtlasIndex == 4)
                                        wallAtlasIndex = 6;
                                if (j % 2 != 0)
                                    if (wallAtlasIndex == 6)
                                        wallAtlasIndex = 4;

                                cornerTextureIndex[i, j] = wallAtlasIndex;
                                wallAtlasIndex = 0;
                            }
                        }                      
                    }                         
                }
            }

            //for (int y = 0; y < cornerTextureIndex.GetLength(0); y++)
            //{
            //    for (int x = 0; x < cornerTextureIndex.GetLength(1); x++)
            //    {
            //        if (cornerTextureIndex[y, x] == -1)
            //            Console.Write(" ");
            //        else
            //        {
            //            switch(cornerTextureIndex[y, x])
            //            {
            //                case 0:
            //                    Console.Write("C");
            //                    break;
            //                case 1:
            //                    Console.Write("T");
            //                    break;
            //                case 2:
            //                    Console.Write("5");
            //                    break;
            //                case 3:
            //                    Console.Write("B");
            //                    break;
            //                case 4:
            //                    Console.Write("R");
            //                    break;
            //                case 5:
            //                    Console.Write("1");
            //                    break;
            //                case 6:
            //                    Console.Write("L");
            //                    break;
            //                case 7:
            //                    Console.Write("O");
            //                    break;
            //                case 8:
            //                    Console.Write("6");
            //                    break;
            //                case 9:
            //                    Console.Write("7");
            //                    break;
            //                case 10:
            //                    Console.Write("8");
            //                    break;
            //                case 11:
            //                    Console.Write("2");
            //                    break;
            //                case 12:
            //                    Console.Write("3");
            //                    break;
            //                case 13:
            //                    Console.Write("4");
            //                    break;
            //                case 14:
            //                    Console.Write("C");
            //                    break;
            //                case 15:
            //                    Console.Write("C");
            //                    break;
            //                case 16:
            //                    Console.Write("C");
            //                    break;
                            

            //            }
                        
            //        }
            //    }
            //    Console.WriteLine();
            //}



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
            info.AddValue("cornerIndex", cornerTextureIndex, typeof(int[,]));
            info.AddValue("levelName", name, typeof(LevelName));
        }

        public LevelTemplate(SerializationInfo info, StreamingContext context)
        {
            levelData = (int[,])info.GetValue("levelData", typeof(int[,]));
            name = (LevelName)info.GetValue("levelName", typeof(LevelName));
            tileTextureIndex = (int[,])info.GetValue("tileIndex", typeof(int[,]));
            cornerTextureIndex = (int[,])info.GetValue("cornerIndex", typeof(int[,]));
        }

        public int[,] LevelData
        {
            get { return levelData; }
        }

        public int[,] TileIndex
        {
            get { return tileTextureIndex; }
        }

        public int[,] CornerIndex
        {
            get { return cornerTextureIndex; }
        }

        public int[,] SubIndex(int y, int x)
        {
            return new int[2, 2] { {cornerTextureIndex[y * 2, x * 2], cornerTextureIndex[y * 2, x * 2 + 1]}, {cornerTextureIndex[y * 2 + 1, x * 2], cornerTextureIndex[y * 2 + 1, x * 2 + 1] }};
        }

        public LevelName Name
        {
            get { return name; }
        }


    }
}
