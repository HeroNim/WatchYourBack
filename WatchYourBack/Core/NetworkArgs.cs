using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBack
{
    [Serializable()]
    class NetworkArgs : EventArgs
    {
        private int xInput;
        private int yInput;
        private Vector2 mouseLoc;
        private bool clicked;


        public NetworkArgs(int xInput, int yInput, Vector2 mouseLoc, bool clicked)
        {
            this.xInput = xInput;
            this.yInput = yInput;
            this.mouseLoc = mouseLoc;
            this.clicked = clicked;
        }

        public override string ToString()
        {
            return xInput + ", " + yInput + ", " + mouseLoc + ", " + clicked;
        }
        
    }
}
