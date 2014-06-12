using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBack
{

    /*
     * A modifiable template for the walls. 
     */
    class WallTemplate
    {
        private Texture2D wallTexture;
        private Rectangle body;

        public WallTemplate(Texture2D texture, Rectangle rect)
        {
            wallTexture = texture;
            body = rect;
        }

        public Texture2D WallTexture
        {
            get { return wallTexture; }
        }

        public Rectangle WallBody
        {
            get { return body; }
            set { body = value; }
        }

        public int X
        {
            get { return body.X; }
            set { body.X = value; }
        }

        public int Y
        {
            get { return body.Y; }
            set { body.Y = value; }
        }
    }
}
