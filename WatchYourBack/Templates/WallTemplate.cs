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
    class WallTemplate : ETemplate
    {
        private Texture2D texture;
        private int width;
        private int height;

        public WallTemplate(Texture2D texture, int width, int height)
        {
            this.texture = texture;
            this.width = width;
            this.height = height;
        }

        public Texture2D Texture
        {
            get { return texture; }
        }

        public int Width
        {
            get { return width; }
            
        }

        public int Height
        {
            get { return height; }
           
        }
    }
}
