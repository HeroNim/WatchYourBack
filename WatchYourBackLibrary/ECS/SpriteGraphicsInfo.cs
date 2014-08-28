using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{

    /// <summary>
    /// Contains all the information the game system needs to render an entity.
    /// </summary>
    public class SpriteGraphicsInfo
    {
        private Texture2D spriteTexture;
        private Rectangle sourceRectangle;
        private Color color;

        private Rectangle body;
        private GraphicsComponent anchor;
        private Vector2 rotationOffset;

        private string text;
        private SpriteFont font;
        private Color fontColor;
        private bool hasText;

        private float layer;
        private bool visible;

        public SpriteGraphicsInfo(Rectangle body, Texture2D texture, float layer)
        {
            spriteTexture = texture;
            sourceRectangle = spriteTexture.Bounds;
            this.body = body;
            color = Color.White;
            this.rotationOffset = Vector2.Zero;

            hasText = false;            
            this.layer = layer;
            visible = true;
        }

         public SpriteGraphicsInfo(Rectangle body, Texture2D texture, Rectangle sourceRectangle, float layer)
            : this(body, texture, layer)
        {            
            this.sourceRectangle = sourceRectangle;            
        }
        

        public SpriteGraphicsInfo(Rectangle body, Texture2D texture, Rectangle sourceRectangle, Vector2 rotationOffset, float layer)
            : this(body, texture, layer)
        {
            this.sourceRectangle = sourceRectangle;            
            this.rotationOffset = rotationOffset;          
        }

        public SpriteGraphicsInfo(Rectangle body, string text, SpriteFont font, Color fontColor, float layer)        
        {
            this.body = body;
            color = Color.White;
            this.rotationOffset = Vector2.Zero;
            this.fontColor = fontColor;            
            this.font = font;
            this.text = text;
            hasText = true;
            visible = true;
        }
        
        public int X { get { return Body.X; } set { body.X = value; } }
        public int Y { get { return Body.Y; } set { body.Y = value; } }

        public Texture2D Sprite { get { return spriteTexture; } }
        public Color SpriteColor { get { return color; } set { color = value; } }
        public Rectangle SourceRectangle { get { return sourceRectangle; } set { sourceRectangle = value; } }

        public Rectangle Body { get { return new Rectangle(body.X + (int)rotationOffset.X, body.Y + (int)rotationOffset.Y, body.Width, body.Height);} set { body = value; } }
        public GraphicsComponent Anchor { get { return anchor; } set { anchor = value; } }
        public float RotationAngle { get { return anchor.RotationAngle; } }
        public Vector2 RotationOrigin { get { return anchor.RotationOrigin; } }
        public Vector2 RotationOffset { get { return rotationOffset; } set { rotationOffset = value; } }
               
        public string Text { get { return text; } set { text = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }
        public Color FontColor { get { return fontColor; } set { fontColor = value; } }
        public bool HasText { get { return hasText; } set { hasText = value; } }
                     
        public float Layer { get { return layer; } set { layer = value; } }
        public bool Visible { get { return visible; } set { visible = value; } }
    }
}
