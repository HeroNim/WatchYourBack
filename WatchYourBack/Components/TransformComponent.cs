using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




namespace WatchYourBack
{
    class TransformComponent : EComponent
    {
        

        private float xPos;
        private float yPos;

        public TransformComponent(float x, float y)
        {
            xPos = x;
            yPos = y;
        }

        public float XPos
        {
            get { return xPos; }
            set { xPos = value; }
        }

        public float YPos
        {
            get { return yPos; }
            set { yPos = value; }
        }
    }
}
