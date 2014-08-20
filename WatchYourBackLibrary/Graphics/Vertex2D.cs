using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// A simple vertex containing a 2D position, used for creating 2D shapes.
    /// </summary>
    [Serializable()]
    public struct Vertex2D : IVertexType, ISerializable
    {
        Vector2 vertexPosition;
        
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
        );

        public Vertex2D(Vector2 pos)
        {
            vertexPosition = pos;
        }

        public Vector2 Position
        {
            get { return vertexPosition; }
            set { vertexPosition = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            float[] vertex = new float[] { vertexPosition.X, vertexPosition.Y };
            info.AddValue("position", vertex, typeof(float[]));
            
        }

        public Vertex2D(SerializationInfo info, StreamingContext context)
        {
            float[] vertex = (float[])info.GetValue("position", typeof(float[]));
            vertexPosition = new Vector2(vertex[0], vertex[1]);
            
        }

    }
}
