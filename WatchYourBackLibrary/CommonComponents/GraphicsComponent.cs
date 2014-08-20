using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{   
    /// <summary>
    /// The component which holds all the graphical information of an entity, as well as get/setters for the information of the primary sprite of the entity.
    /// </summary>
    public class GraphicsComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Graphics; } }
        public override Masks Mask { get { return Masks.Graphics; } }
        
        private Dictionary<string, SpriteGraphicsInfo> sprites;
        private Dictionary<string, Polygon> polygons;

        List<Vector2> debugPoints;
        private SpriteGraphicsInfo mainSprite;
        private Polygon mainPolygon;

        public GraphicsComponent()
        {
            sprites = new Dictionary<string, SpriteGraphicsInfo>();
            polygons = new Dictionary<string, Polygon>();
            debugPoints = new List<Vector2>();
        }

        public GraphicsComponent(Rectangle body, Texture2D texture, float layer, string key)
            : this()
        {
            mainSprite = new SpriteGraphicsInfo(body, texture, layer);
            sprites.Add(key, mainSprite);         
        }

        public GraphicsComponent(Rectangle body, Texture2D texture, Rectangle sourceRectangle, float layer, string key)
            : this()
        {
            mainSprite = new SpriteGraphicsInfo(body, texture, sourceRectangle, layer);
            sprites.Add(key, mainSprite);            
        }

        public GraphicsComponent(Rectangle body, Texture2D texture, float rotationAngle, Vector2 rotationOrigin, float layer, string key)
            : this()
        {
            mainSprite = new SpriteGraphicsInfo(body, texture, rotationAngle, rotationOrigin, layer);
            sprites.Add(key, mainSprite);        
        }

        public GraphicsComponent(Rectangle body, Texture2D texture, Rectangle sourceRectangle, float rotationAngle, Vector2 rotationOrigin, Vector2 rotationOffset, float layer, string key)
            : this()
        {
            mainSprite = new SpriteGraphicsInfo(body, texture, sourceRectangle, rotationAngle, rotationOrigin, rotationOffset, layer);
            sprites.Add(key, mainSprite);          
        }

        public GraphicsComponent(Rectangle body, string text, SpriteFont font, Color fontColor, float layer, string key)
            : this()
        {
            mainSprite = new SpriteGraphicsInfo(body, text, font, fontColor, layer);
            sprites.Add(key, mainSprite);         
        }

        public GraphicsComponent(Polygon p, string key)
            :this()
        {
            polygons.Add(key, p);
        }
 
        public int X { get { return mainSprite.X; } set { mainSprite.X = value; } }
        public int Y { get { return mainSprite.Y; } set { mainSprite.Y = value; } }

        public Texture2D Sprite { get { return mainSprite.Sprite; } }
        public Rectangle SourceRectangle { get { return mainSprite.SourceRectangle; } set { mainSprite.SourceRectangle = value; } }
        public Rectangle Body { get { return mainSprite.Body; } set { mainSprite.Body = value; } }
        public Color SpriteColor { get { return mainSprite.SpriteColor; } set { mainSprite.SpriteColor = value; } }
        public Color FontColor { get { return mainSprite.FontColor; } set { mainSprite.FontColor = value; } }
        public SpriteFont Font { get { return mainSprite.Font; } set { mainSprite.Font = value; } }
        public bool HasText { get { return mainSprite.HasText; } set { mainSprite.HasText = value; } }
        public string Text { get { return mainSprite.Text; } set { mainSprite.Text = value; } }
        public float RotationAngle { get { return mainSprite.RotationAngle; } set { mainSprite.RotationAngle = value; } }
        public Vector2 RotationOrigin { get { return mainSprite.RotationOrigin; } }
        public Vector2 RotationOffset { get { return mainSprite.RotationOffset; } set { mainSprite.RotationOffset = value; } }
        public List<Vector2> DebugPoints { get { return debugPoints; } set { debugPoints = value; } }
        public float Layer { get { return mainSprite.Layer; } set { mainSprite.Layer = value; } }

        public Dictionary<string, SpriteGraphicsInfo> Sprites { get { return sprites; } }
        public Dictionary<string, Polygon> Polygons { get { return polygons; } }

        public void addSprite(string name, SpriteGraphicsInfo graphicstoAdd)
        {
            sprites.Add(name, graphicstoAdd);
            if (mainSprite == null)
                mainSprite = graphicstoAdd;
                
        }

        public void addPolygon(string name, Polygon poly)
        {
            if (polygons.ContainsKey(name))
                polygons.Remove(name);
            polygons.Add(name, poly);
            if (mainPolygon == null)
                mainPolygon = poly;
        }
       
        
    }
}
