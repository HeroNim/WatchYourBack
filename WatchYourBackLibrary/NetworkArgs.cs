using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    [Serializable()]
    public class NetworkArgs : EventArgs
    {
        private int xInput;
        private int yInput;
        private int mouseX;
        private int mouseY;
        private bool clicked;


        public NetworkArgs(int xInput, int yInput, Vector2 mouseLoc, bool clicked)
        {
            this.xInput = xInput;
            this.yInput = yInput;
            mouseX = (int)mouseLoc.X;
            mouseY = (int)mouseLoc.Y;
            this.clicked = clicked;
        }

        public override string ToString()
        {
            return xInput + ", " + yInput + ", (" + mouseX + ", " + mouseY+"), " + clicked;
        }
        
    }
}
