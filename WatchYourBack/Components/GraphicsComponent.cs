using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBack
{
    class GraphicsComponent : EComponent
    {

        public override int Mask { get { return (int)Masks.Graphics; } }

        private Texture2D spriteTexture;
        private Rectangle body;
        private Color color;

        public GraphicsComponent(Rectangle rectangle, Texture2D texture, Color color)
        {
            spriteTexture = texture;
            body = rectangle;
            this.color = color;
        }

        public int X { get { return body.X; } set { body.X = value; } }
        public int Y { get { return body.Y; } set { body.Y = value; } }

        public Texture2D Sprite { get { return spriteTexture; } }
        public Rectangle Body { get { return body; } }
        public Color SpriteColor { get { return color; } }
    }
}
