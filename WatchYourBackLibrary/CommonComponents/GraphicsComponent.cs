using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{

    /* 
     * Holds all the information the graphics device needs to render an entity.
     */
    public class GraphicsComponent : EComponent
    {

        public readonly static int bitMask = (int)Masks.GRAPHICS;
        public override Masks Mask { get { return Masks.GRAPHICS; } }

        private Texture2D spriteTexture;
        private Rectangle body;
        private Color color;
        private Color fontColor;
        private SpriteFont font;
        private string text;
        private bool hasText;

        private float rotationAngle;
        private Vector2 rotationOrigin;

        public GraphicsComponent(Rectangle rectangle, Texture2D texture)
        {
            spriteTexture = texture;
            body = rectangle;
            color = Color.White;
            hasText = false;
        }

        public GraphicsComponent(Rectangle rectangle, Texture2D texture, float rotationAngle, Vector2 rotationOrigin)
        {
            spriteTexture = texture;
            body = rectangle;
            color = Color.White;
            this.rotationAngle = rotationAngle;
            this.rotationOrigin = rotationOrigin;
            hasText = false;
        }

        public GraphicsComponent(Rectangle rectangle, Texture2D texture, string text, SpriteFont font, Color fontColor)
        {
            spriteTexture = texture;
            body = rectangle;
            this.fontColor = fontColor;
            color = Color.White;
            this.font = font;
            this.text = text;
            hasText = true;
        }

 
        public int X { get { return body.X; } set { body.X = value; } }
        public int Y { get { return body.Y; } set { body.Y = value; } }

        public Texture2D Sprite { get { return spriteTexture; } }
        public Rectangle Body { get { return body; } }
        public Color SpriteColor { get { return color; } set { color = value; } }
        public Color FontColor { get { return fontColor; } set { fontColor = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }
        public bool HasText { get { return hasText; } set { hasText = value; } }
        public string Text { get { return text; } set { text = value; } }
        public float RotationAngle { get { return rotationAngle; } set { rotationAngle = value; } }
        public Vector2 RotationOrigin { get { return rotationOrigin; } }
        public bool Rotatable { get { return (rotationAngle != 0); } }
    }
}
