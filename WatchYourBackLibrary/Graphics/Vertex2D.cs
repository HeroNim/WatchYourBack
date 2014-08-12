using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{
    public struct Vertex2D : IVertexType
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
    }
}
