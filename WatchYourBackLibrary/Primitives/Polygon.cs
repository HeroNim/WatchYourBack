using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{
    public enum PolyType
    {
        LineList,
        LineStrip,
        TriangleList,
        TriangleStrip,
        TriangleFan
    }

    public class Polygon
    {
        List<Vector2> vertices;
        VertexPositionColor[] vertexList;
        short[] indexList;
        int numTriangles;
        PrimitiveType renderType;
        PolyType type;
        
        
        

        public Polygon(List<Vector2> vertices, PolyType type)
        {
            this.vertices = vertices;
            this.type = type;
            vertexList = new VertexPositionColor[vertices.Count];
            for(int i = 0; i < vertices.Count; i++)
            {
                vertexList[i] = new VertexPositionColor(new Vector3(vertices[i],0) , Color.White);
            }
            Format();
        }

        private void Format()
        {
            switch (type)
            {
                case PolyType.TriangleFan:
                    renderType = PrimitiveType.TriangleList;
                    indexList = new short[(vertices.Count * 3) - 6];
                    numTriangles = vertexList.Length - 2;
                    int j = 0;
                    for (short i = 2; i < vertexList.Length; j++, i++)
                    {
                        if (j % 3 == 0)
                        {
                            indexList[j] = 0;
                            i-= 2;
                        }
                        else
                            indexList[j] = i;
                    }
                    break;
            }
        }

        public VertexPositionColor[] VertexList { get { return vertexList; } }
        public short[] IndexList { get { return indexList; } }

        public int NumTriangles { get { return numTriangles; } }
        public PrimitiveType PrimitiveType { get { return renderType; } }

        
    }
}
