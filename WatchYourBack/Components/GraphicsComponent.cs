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

        private Rectangle sprite;

        public GraphicsComponent(int x, int y, int width, int height)
        {
            sprite = new Rectangle(x, y, width, height);
        }

        public Rectangle Sprite { get { return sprite; } }
    }
}
