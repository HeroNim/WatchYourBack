using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;


namespace WatchYourBack
{
    class TransformComponent : EComponent
    {
        

        private Vector2 position;
        

        public TransformComponent(float x, float y)
        {
            position.X = x;
            position.Y = y;
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
