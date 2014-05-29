using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class TransformComponent : Component
    {
        private float xPos;
        private float yPos;

        public TransformComponent(float x, float y)
        {
            xPos = x;
            yPos = y;
        }
    }
}
