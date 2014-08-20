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

    /// <summary>
    /// Takes an image of a level, and converts the image into all the information about the level, such as the location of the walls and spawns,
    /// which can then be read by the server or client to create the level itself.
    /// </summary>
    [Serializable()]
    public class LevelTemplate : ISerializable
    {
        private Texture2D levelImage;
        private Color[] data;
        private int[,] levelData;
        private int[,] cornerTextureIndex;

        private List<bool>[,] cornerVertices;

        private LevelName name;

        public LevelTemplate(Texture2D level, LevelName name)
        {
            this.name = name;
            levelImage = level;
            cornerVertices = new List<bool>[levelImage.Height, levelImage.Width];
            data = new Color[levelImage.Width * levelImage.Height];
            levelImage.GetData<Color>(data);

            levelData = new int[levelImage.Height, levelImage.Width];
            cornerTextureIndex = new int[levelImage.Height * 2, levelImage.Width * 2];
            for (int y = 0; y < cornerTextureIndex.GetLength(1); y++)
                for (int x = 0; x < cornerTextureIndex.GetLength(0); x++)
                    cornerTextureIndex[x, y] = -1;
            for (int y = 0; y < levelImage.Height; y++)
            {
                for (int x = 0; x < levelImage.Width; x++)
                {
                    if (data[y * levelImage.Width + x] == Color.Black)
                    {
                        levelData[y, x] = (int)TileType.WALL;
                        cornerVertices[y, x] = new List<bool>();
                    }
                    if (data[y * levelImage.Width + x] == Color.White)
                        levelData[y, x] = (int)TileType.EMPTY;
                    if (data[y * levelImage.Width + x] == Color.Red)
                        levelData[y, x] = (int)TileType.SPAWN;
                }
            }

            int wallAtlasIndex = 0;

            //Find the index of corners
            bool topRight;
            bool topLeft;
            bool bottomRight;
            bool bottomLeft;

            for (int y = 0; y < levelImage.Height; y++)
            {
                for (int x = 0; x < levelImage.Width; x++)
                {
                    if (levelData[y, x] == (int)TileType.WALL)
                    {
                        topRight = false;
                        topLeft = false;
                        bottomRight = false;
                        bottomLeft = false;

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

                                        if (wallAtlasIndex == 5 || wallAtlasIndex == 2 || wallAtlasIndex == 0)
                                            topLeft = true;
                                    }
                                    else //Bottom
                                    {
                                        if (y + 1 < levelImage.Height && x - 1 >= 0 && levelData[y + 1, x - 1] == (int)TileType.WALL)
                                            wallAtlasIndex += 2; //Bottom Left
                                        if (y + 1 < levelImage.Height && levelData[y + 1, x] == (int)TileType.WALL)
                                            wallAtlasIndex += 4; //Bottom

                                        if (wallAtlasIndex == 5 || wallAtlasIndex == 2 || wallAtlasIndex == 0)
                                            bottomLeft = true;
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

                                        if (wallAtlasIndex == 5 || wallAtlasIndex == 2 || wallAtlasIndex == 0)
                                            topRight = true;
                                    }
                                    else //Bottom
                                    {
                                        if (y + 1 < levelImage.Height && x + 1 < levelImage.Width && levelData[y + 1, x + 1] == (int)TileType.WALL)
                                            wallAtlasIndex += 2; //Bottom Right
                                        if (y + 1 < levelImage.Height && levelData[y + 1, x] == (int)TileType.WALL)
                                            wallAtlasIndex += 4; //Bottom

                                        if (wallAtlasIndex == 5 || wallAtlasIndex == 2 || wallAtlasIndex == 0)
                                            bottomRight = true;
                                    }
                                }


                                //Reorder some of the atlas indices to produce rotations
                                
                                if (i % 2 == 0)
                                {
                                    if (wallAtlasIndex == 3)
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
                                        wallAtlasIndex = 15;
                                    if (j % 2 != 0 && wallAtlasIndex == 0)
                                        wallAtlasIndex = 16;
                                }
                                if (j % 2 == 0)
                                    if (wallAtlasIndex == 4)
                                        wallAtlasIndex = 6;
                                if (j % 2 != 0)
                                    if (wallAtlasIndex == 6)
                                        wallAtlasIndex = 4;

                                cornerTextureIndex[i, j] = wallAtlasIndex;
                                wallAtlasIndex = 0;
                            }
                        }

                        cornerVertices[y, x].Add(topLeft);
                        cornerVertices[y, x].Add(topRight);
                        cornerVertices[y, x].Add(bottomLeft);
                        cornerVertices[y, x].Add(bottomRight);


                    }
                }
            }

            //Find the vertices of each shape
           

            //for (int y = 0; y < levelImage.Height; y++)
            //{
            //    for (int x = 0; x < levelImage.Width; x++)
            //    {
            //        if (levelData[y, x] == (int)TileType.WALL)
            //        {
            //            topRight = true;
            //            topLeft = true;
            //            bottomRight = true;
            //            bottomLeft = true;
            //            //Above
            //            if (y - 1 >= 0 && levelData[y - 1, x] == (int)TileType.WALL)
            //            {
            //                topLeft = false;
            //                topRight = false;
            //            }
            //            //Right
            //            if (x + 1 < levelImage.Width && levelData[y, x + 1] == (int)TileType.WALL)
            //            {
            //                topRight = false;
            //                bottomRight = false;
            //            }
            //            //Below
            //            if (y + 1 < levelImage.Height && levelData[y + 1, x] == (int)TileType.WALL)
            //            {
            //                bottomRight = false;
            //                bottomLeft = false;
            //            }
            //            //Left
            //            if (x - 1 >= 0 && levelData[y, x - 1] == (int)TileType.WALL)
            //            {
            //                bottomLeft = false;
            //                topLeft = false;
            //            }
            //                cornerVertices[y, x].Add(topLeft);
            //                cornerVertices[y, x].Add(topRight);
            //                cornerVertices[y, x].Add(bottomLeft);                       
            //                cornerVertices[y, x].Add(bottomRight);
            //        }
            //    }
            //}
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("levelName", name, typeof(LevelName));
            info.AddValue("levelData", levelData, typeof(int[,]));
            info.AddValue("cornerIndex", cornerTextureIndex, typeof(int[,]));
            info.AddValue("vertices", cornerVertices, typeof(List<bool>[,]));
        }

        public LevelTemplate(SerializationInfo info, StreamingContext context)
        {
            name = (LevelName)info.GetValue("levelName", typeof(LevelName));
            levelData = (int[,])info.GetValue("levelData", typeof(int[,]));
            cornerTextureIndex = (int[,])info.GetValue("cornerIndex", typeof(int[,]));
            cornerVertices = (List<bool>[,])info.GetValue("vertices", typeof(List<bool>[,]));
        }

        public int[,] LevelData
        {
            get { return levelData; }
        }

        

        public int[,] CornerIndex
        {
            get { return cornerTextureIndex; }
        }

        public int[,] TileIndex(int y, int x)
        {
            return new int[2, 2] { { cornerTextureIndex[y * 2, x * 2], cornerTextureIndex[y * 2, x * 2 + 1] }, { cornerTextureIndex[y * 2 + 1, x * 2], cornerTextureIndex[y * 2 + 1, x * 2 + 1] } };
        }

        public LevelName Name
        {
            get { return name; }
        }

        public List<bool>[,] CornerVertices
        {
            get { return cornerVertices; }
        }

        public override string ToString()
        {
            string debug = "";

            for (int y = 0; y < cornerTextureIndex.GetLength(0); y++)
            {
                for (int x = 0; x < cornerTextureIndex.GetLength(1); x++)
                {


                    if (cornerTextureIndex[y, x] >= 0 && cornerTextureIndex[y, x] <= 9)
                        debug += " ";
                    debug += cornerTextureIndex[y, x];
                    
                }
                debug += "\n";
            }
            return debug;
        }
    }
}
