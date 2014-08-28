using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{   
    public enum GraphicsLayer
    {
        Background,
        Foreground
    }
    /// <summary>
    /// The component which holds all the graphical information of an entity, as well as get/setters for the information of the primary sprite of the entity.
    /// </summary>
    public class GraphicsComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Graphics; } }
        public override Masks Mask { get { return Masks.Graphics; } }
        
        private readonly Dictionary<string, SpriteGraphicsInfo> sprites;
        private Dictionary<string, Polygon> polygons;

        List<Vector2> debugPoints;
        private SpriteGraphicsInfo mainSprite;
        private Polygon mainPolygon;

        private Vector2 rotationOrigin;
        private float rotationAngle;

        private GraphicsLayer graphicsLayer;

        public GraphicsComponent(GraphicsLayer layer, Vector2 rotationOrigin, float rotationAngle = 0)
        {
            sprites = new Dictionary<string, SpriteGraphicsInfo>();
            polygons = new Dictionary<string, Polygon>();
            debugPoints = new List<Vector2>();
            this.rotationOrigin = rotationOrigin;
            this.rotationAngle = rotationAngle;
            this.graphicsLayer = layer;
        }

        public GraphicsComponent(GraphicsLayer layer, Point rotationOrigin, float rotationAngle = 0)
            : this(layer, new Vector2(rotationOrigin.X, rotationOrigin.Y), rotationAngle) { }

 
        public Dictionary<string, SpriteGraphicsInfo> Sprites { get { return sprites; } }
        public Dictionary<string, Polygon> Polygons { get { return polygons; } }

        public int X { get { return mainSprite.X; } set { mainSprite.X = value; } }
        public int Y { get { return mainSprite.Y; } set { mainSprite.Y = value; } }

        public Texture2D Sprite { get { return mainSprite.Sprite; } }
        public Color SpriteColor { get { return mainSprite.SpriteColor; } set { mainSprite.SpriteColor = value; } }
        public Rectangle SourceRectangle { get { return mainSprite.SourceRectangle; } set { mainSprite.SourceRectangle = value; } }

        public Rectangle Body { get { return mainSprite.Body; } set { mainSprite.Body = value; } }
        public float RotationAngle { get { return rotationAngle; } set { rotationAngle = value; } }
        public Vector2 RotationOrigin { get { return rotationOrigin; } }

        public string Text { get { return mainSprite.Text; } set { mainSprite.Text = value; } }
        public SpriteFont Font { get { return mainSprite.Font; } set { mainSprite.Font = value; } }
        public Color FontColor { get { return mainSprite.FontColor; } set { mainSprite.FontColor = value; } }
        public bool HasText { get { return mainSprite.HasText; } set { mainSprite.HasText = value; } }

        public List<Vector2> DebugPoints { get { return debugPoints; } set { debugPoints = value; } }
        public float Layer { get { return mainSprite.Layer; } set { mainSprite.Layer = value; } }
        public GraphicsLayer GraphicsLayer { get { return graphicsLayer; } }



        public void AddSprite(string name, SpriteGraphicsInfo graphicstoAdd)
        {
            graphicstoAdd.Anchor = this;
            if (sprites.ContainsKey(name))
                sprites.Remove(name);
            sprites.Add(name, graphicstoAdd);
            if (mainSprite == null)
                mainSprite = graphicstoAdd;
                
        }

        public SpriteGraphicsInfo GetSprite(string key)
        {
            return sprites[key];
        }

        public void AddPolygon(string name, Polygon poly)
        {
            if (polygons.ContainsKey(name))
                polygons.Remove(name);
            polygons.Add(name, poly);
            if (mainPolygon == null)
                mainPolygon = poly;
        }

        public Polygon GetPolygon(string key)
        {
            return polygons[key];
        }
       
        
    }
}
