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

            foreach (Entity e in activeEntities)
            {
                vertices = new List<Vector2>(manager.LevelInfo.Vertices);
                
                VisionComponent v = e.GetComponent<VisionComponent>();              
                TransformComponent t = e.GetComponent<TransformComponent>();

                center = t.Center;
                v.CloseRangeVision.Center = t.Center;
                float upperBound = HelperFunctions.Normalize(t.Rotation + (v.FOV / 2));
                float lowerBound = HelperFunctions.Normalize(t.Rotation - (v.FOV / 2));

                Vector2 a = HelperFunctions.AngleToVector(upperBound);
                Vector2 b = HelperFunctions.AngleToVector(lowerBound);

                vertices = HelperFunctions.SortVertices(vertices, center, center);

                //Remove vertices not in line of sight
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (HelperFunctions.Angle(vertices[i] - center, a) > v.FOV || HelperFunctions.Angle(vertices[i] - center, b) > v.FOV)
                    {
                        vertices.Remove(vertices[i]);
                        i--;
                    }                    
                }

                //Add vertices offset by tiny amounts to look around corners
                for (int i = 0, j = vertices.Count; i < j; i++)
                {
                    float vertexAngle = HelperFunctions.VectorToAngle(vertices[i] - center);
                    vertices.Add(center + (HelperFunctions.AngleToVector(vertexAngle + 0.01f) * 2000));
                    vertices.Add(center + (HelperFunctions.AngleToVector(vertexAngle - 0.01f) * 2000));
                }
               
                vertices.Add((HelperFunctions.AngleToVector(upperBound) * 2000) + center);
                vertices.Add((HelperFunctions.AngleToVector(lowerBound) * 2000) + center);

                //Find the closest collision point for each vertex
                for (int i = 0; i < vertices.Count(); i++)
                {
                    Line ray = new Line(center, vertices[i]);
                    List<Entity> collidingEntities = manager.QuadTree.Intersects(ray);
                    if (collidingEntities.Count == 0)
                        continue;
                    possiblePoints.Add(vertices[i]);
                    foreach (Entity possibleEntity in collidingEntities)
     
                        if (possibleEntity.hasComponent(Masks.VisionBlock))
                        {
                            Rectangle collider = possibleEntity.GetComponent<VisionBlockComponent>().Collider;
                            Vector2 point = HelperFunctions.GetClosestPoint(ray.P1, CollisionHelper.GetIntersection(ray, possibleEntity.GetComponent<VisionBlockComponent>().Collider));
                            if (point != Vector2.Zero)
                                possiblePoints.Add(point);
                        }
                    
                    endpoints.Add(HelperFunctions.GetClosestPoint(ray.P1, possiblePoints));
                    possiblePoints.Clear();
                }
               
                endpoints = HelperFunctions.SortVertices(endpoints, center, endpoints[endpoints.Count - 1]);
                endpoints.Add(center);
                endpoints.Insert(0, center);
                Console.WriteLine(endpoints[0]);

                //Close range vision
                for (float angle = -MathHelper.PiOver4 / 4; angle <= MathHelper.TwoPi; angle += MathHelper.PiOver4/4)
                {                   
                        endpoints.Add(Circle.PointOnCircle(v.CloseRangeVision, angle + MathHelper.PiOver4 / 4));
                }

                v.VisionField = new Polygon(endpoints, PolyType.TriangleFan);
              
                if (manager.hasGraphics())
                {                 
                    GraphicsComponent g = e.GetComponent<GraphicsComponent>();
                    g.AddPolygon("Vision", v.VisionField);                   
                }
                endpoints.Clear();
                
            }
        }
    }
}
