﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBackLibrary
{
    public class GraphicsInfo
    {
        private Texture2D spriteTexture;
        private Rectangle sourceRectangle;
        private Rectangle body;
        private Color color;
        private Color fontColor;
        private SpriteFont font;
        private string text;
        private float layer;
        private bool hasText;

        private float rotationAngle;
        private Vector2 rotationOrigin;
        private Vector2 rotationOffset;

        private List<Vector2> debugPoints;

        public GraphicsInfo(Rectangle body, Texture2D texture, float layer)
        {
            spriteTexture = texture;
            sourceRectangle = spriteTexture.Bounds;
            this.body = body;
            color = Color.White;
            this.rotationAngle = 0;
            this.rotationOrigin = Vector2.Zero;
            this.rotationOffset = Vector2.Zero;

            hasText = false;
            debugPoints = new List<Vector2>();            
            this.layer = layer;
        }

         public GraphicsInfo(Rectangle body, Texture2D texture, Rectangle sourceRectangle, float layer)
            : this(body, texture, layer)
        {            
            this.sourceRectangle = sourceRectangle;            
        }

        public GraphicsInfo(Rectangle body, Texture2D texture, float rotationAngle, Vector2 rotationOrigin, float layer)
            : this(body, texture, layer)
        {            
            this.rotationAngle = rotationAngle;
            this.rotationOrigin = rotationOrigin;            
        }

        public GraphicsInfo(Rectangle body, Texture2D texture, Rectangle sourceRectangle, float rotationAngle, Vector2 rotationOrigin, Vector2 rotationOffset, float layer)
            : this(body, texture, rotationAngle, rotationOrigin, layer)
        {
            this.sourceRectangle = sourceRectangle;            
            this.rotationOffset = rotationOffset;          
        }

        public GraphicsInfo(Rectangle body, Texture2D texture, string text, SpriteFont font, Color fontColor, float layer)
            : this(body, texture, layer)
        {           
            this.fontColor = fontColor;            
            this.font = font;
            this.text = text;
            hasText = true;            
        }

        
        public int X { get { return Body.X; } set { body.X = value; } }
        public int Y { get { return Body.Y; } set { body.Y = value; } }

        public Texture2D Sprite { get { return spriteTexture; } }
        public Rectangle SourceRectangle { get { return sourceRectangle; } set { sourceRectangle = value; } }
        public Rectangle Body { get { return new Rectangle(body.X + (int)rotationOffset.X, body.Y + (int)rotationOffset.Y, body.Width, body.Height);} set { body = value; } }
        public Color SpriteColor { get { return color; } set { color = value; } }
        public Color FontColor { get { return fontColor; } set { fontColor = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }
        public bool HasText { get { return hasText; } set { hasText = value; } }
        public string Text { get { return text; } set { text = value; } }
        public float RotationAngle { get { return rotationAngle; } set { rotationAngle = value; } }
        public Vector2 RotationOrigin { get { return rotationOrigin; } }
        public Vector2 RotationOffset { get { return rotationOffset; } set { rotationOffset = value; } }
        public bool Rotatable { get { return (rotationAngle != 0); } }
        public List<Vector2> DebugPoints { get { return debugPoints; } set { debugPoints = value; } }
        public float Layer { get { return layer; } set { layer = value; } }
    }
}
