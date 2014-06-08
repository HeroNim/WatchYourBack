using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace WatchYourBack
{
    /*
     * Holds the position of the entity 
    */

    class TransformComponent : EComponent
    {
        private Vector2 position;

        public override int Mask
        {  
                get { return (int)Masks.Transform; }
        }
        

        public TransformComponent(float x, float y)
        {
            position = new Vector2(x, y);
        }



        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
