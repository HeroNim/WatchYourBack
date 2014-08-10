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
        
        private Dictionary<string, GraphicsInfo> graphics;
        List<Vector2> debugPoints;
        private GraphicsInfo main;

        public GraphicsComponent()
        {
            graphics = new Dictionary<string, GraphicsInfo>();
            debugPoints = new List<Vector2>();
        }

        public GraphicsComponent(Rectangle body, Texture2D texture, float layer, string key)
            : this()
        {
            main = new GraphicsInfo(body, texture, layer);
            graphics.Add(key, main);         
        }

        public GraphicsComponent(Rectangle body, Texture2D texture, Rectangle sourceRectangle, float layer, string key)
            : this()
        {
            main = new GraphicsInfo(body, texture, sourceRectangle, layer);
            graphics.Add(key, main);            
        }

        public GraphicsComponent(Rectangle body, Texture2D texture, float rotationAngle, Vector2 rotationOrigin, float layer, string key)
            : this()
        {
            main = new GraphicsInfo(body, texture, rotationAngle, rotationOrigin, layer);
            graphics.Add(key, main);        
        }

        public GraphicsComponent(Rectangle body, Texture2D texture, Rectangle sourceRectangle, float rotationAngle, Vector2 rotationOrigin, Vector2 rotationOffset, float layer, string key)
            : this()
        {
            main = new GraphicsInfo(body, texture, sourceRectangle, rotationAngle, rotationOrigin, rotationOffset, layer);
            graphics.Add(key, main);          
        }

        public GraphicsComponent(Rectangle body, string text, SpriteFont font, Color fontColor, float layer, string key)
            : this()
        {
            main = new GraphicsInfo(body, text, font, fontColor, layer);
            graphics.Add(key, main);         
        }
 
        public int X { get { return main.X; } set { main.X = value; } }
        public int Y { get { return main.Y; } set { main.Y = value; } }

        public Texture2D Sprite { get { return main.Sprite; } }
        public Rectangle SourceRectangle { get { return main.SourceRectangle; } set { main.SourceRectangle = value; } }
        public Rectangle Body { get { return main.Body; } set { main.Body = value; } }
        public Color SpriteColor { get { return main.SpriteColor; } set { main.SpriteColor = value; } }
        public Color FontColor { get { return main.FontColor; } set { main.FontColor = value; } }
        public SpriteFont Font { get { return main.Font; } set { main.Font = value; } }
        public bool HasText { get { return main.HasText; } set { main.HasText = value; } }
        public string Text { get { return main.Text; } set { main.Text = value; } }
        public float RotationAngle { get { return main.RotationAngle; } set { main.RotationAngle = value; } }
        public Vector2 RotationOrigin { get { return main.RotationOrigin; } }
        public Vector2 RotationOffset { get { return main.RotationOffset; } set { main.RotationOffset = value; } }
        public List<Vector2> DebugPoints { get { return debugPoints; } set { debugPoints = value; } }
        public float Layer { get { return main.Layer; } set { main.Layer = value; } }

        public Dictionary<string, GraphicsInfo> Sprites { get { return graphics; } }

        public void addSprite(string name, GraphicsInfo graphicstoAdd)
        {
            graphics.Add(name, graphicstoAdd);
            if (main == null)
                main = graphicstoAdd;
                
        }
       
        
    }
}
