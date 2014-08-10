using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    

    public class Polygon
    {
        List<Vector2> vertices;
        List<Line> walls;
        

        public Polygon(List<Vector2> vertices)
        {
            walls = new List<Line>();
            this.vertices = vertices;
            if (vertices == null || vertices.Count <= 1)
                return;
            else if (vertices.Count == 2)
                walls.Add(new Line(vertices[0], vertices[1]));
            else
                for (int i = 0; i < vertices.Count; i++)
                    walls.Add(new Line(vertices[i], vertices[(i + 1) % vertices.Count]));
        }
    }
}
