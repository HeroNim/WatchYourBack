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
    public enum PolyType
    {
        LineList,
        LineStrip,
        TriangleList,
        TriangleStrip,
        TriangleFan
    }

    [Serializable()]
    public class Polygon : ISerializable
    {
        List<Vector2> vertices;
        Vertex2D[] vertexList;
        short[] indexList;
        int numTriangles;
        PrimitiveType renderType;
        PolyType type;
                     
        
        public Polygon(List<Vector2> vertices, PolyType type)
        {
            this.vertices = vertices;
            this.type = type;
            vertexList = new Vertex2D[vertices.Count];
            for(int i = 0; i < vertices.Count; i++)
            {
                vertexList[i] = new Vertex2D(vertices[i]);
            }
            Format();
        }

        private void Format()
        {
            switch (type)
            {
                case PolyType.TriangleFan:
                    renderType = PrimitiveType.TriangleList;
                    indexList = new short[(vertexList.Length * 3) - 6];
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

        public Vertex2D[] VertexList { get { return vertexList; } }
        public short[] IndexList { get { return indexList; } }
        public int NumTriangles { get { return numTriangles; } }
        public PrimitiveType PrimitiveType { get { return renderType; } }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {          
            info.AddValue("vertexList", vertexList, typeof(Vertex2D[]));         
            info.AddValue("type", type, typeof(PolyType));
        }

        public Polygon(SerializationInfo info, StreamingContext context)
        {           
            vertexList = (Vertex2D[])info.GetValue("vertexList", typeof(Vertex2D[]));          
            type = (PolyType)info.GetValue("type", typeof(PolyType));         
            Format();          
        }

        
    }
}
