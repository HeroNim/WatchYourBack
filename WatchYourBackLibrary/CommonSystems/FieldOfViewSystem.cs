using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public class FieldOfViewSystem : ESystem
    {
        List<Vector2> vertices;
        List<Vector2> endpoints;
        List<Vector2> possiblePoints;

        public FieldOfViewSystem()
            : base(false, true, 6)
        {
            components += (int)Masks.Transform;
            components += (int)Masks.Vision;
            endpoints = new List<Vector2>();
            possiblePoints = new List<Vector2>();
            
        }
        public override void update(TimeSpan gameTime)
        {
            Vector2 center;
            endpoints.Clear();            

            foreach (Entity e in activeEntities)
            {
                vertices = new List<Vector2>(manager.LevelInfo.Vertices);

                VisionComponent v = e.GetComponent<VisionComponent>();
                TransformComponent t = e.GetComponent<TransformComponent>();
                GraphicsComponent g = e.GetComponent<GraphicsComponent>();
                g.DebugPoints.Clear();
                center = t.Center;
                float upperBound = HelperFunctions.Normalize(t.Rotation + (v.FOV / 2));
                float lowerBound = HelperFunctions.Normalize(t.Rotation - (v.FOV / 2));
                
                //Remove vertices not in line of sight
                for (int i = 0; i < vertices.Count; i++)
                {
                    float vertexAngle = HelperFunctions.VectorToAngle(vertices[i] - center);
                    if (vertexAngle > upperBound || vertexAngle < lowerBound)
                    {
                        vertices.Remove(vertices[i]);
                        i--;
                    }
                }

                //Add vertices offset by tiny amounts to look around corners
                for (int i = 0, j = vertices.Count; i < j; i++)
                {
                    float vertexAngle = HelperFunctions.VectorToAngle(vertices[i] - center);
                    vertices.Add(center + (HelperFunctions.AngleToVector(vertexAngle + 0.0001f) * 2000));
                    vertices.Add(center + (HelperFunctions.AngleToVector(vertexAngle - 0.0001f) * 2000));
                }

                vertices.Add(HelperFunctions.AngleToVector(upperBound) * 2000);
                vertices.Add(HelperFunctions.AngleToVector(lowerBound) * 2000);

                vertices = HelperFunctions.SortVertices(vertices, center);
                for (int i = 0; i < vertices.Count(); i++)
                {
                    //g.DebugPoints.Add(vertices[i]);
                    Line ray = new Line(center, vertices[i]);
                    List<Entity> valid = manager.QuadTree.Intersects(ray);
                    foreach (Entity possible in valid)
     
                        if (possible.hasComponent(Masks.VisionBlock))
                        {
                            Rectangle collider = possible.GetComponent<VisionBlockComponent>().Collider;
                            //g.DebugPoints.Add(new Vector2(collider.Center.X, collider.Center.Y));
                            Vector2 point = HelperFunctions.GetClosestPoint(ray.P1, CollisionHelper.GetIntersection(ray, possible.GetComponent<VisionBlockComponent>().Collider));
                            if (point != Vector2.Zero)
                                possiblePoints.Add(point);
                        }
                    
                    endpoints.Add(HelperFunctions.GetClosestPoint(ray.P1, possiblePoints));
                    possiblePoints.Clear();
                }
                
                v.VisionField = new Polygon(endpoints);
                break;
            }
        }
    }
}
