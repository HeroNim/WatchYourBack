using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{  
    /// <summary>
    /// Arguments sent by the client to the server, informing it of player input
    /// </summary>
    [Serializable()]
    public class NetworkInputArgs : EventArgs
    {
        private long sender;
        private int xInput;
        private int yInput;
        private int mouseX;
        private int mouseY;
        private bool leftClicked;
        private bool rightClicked;
        private bool dash;
        private double drawTime;

        public NetworkInputArgs(long sender, int xInput, int yInput, Vector2 mouseLoc, bool leftClicked, bool rightClicked, double drawTime, bool dash)
        {
            this.sender = sender;
            this.xInput = xInput;
            this.yInput = yInput;
            mouseX = (int)mouseLoc.X;
            mouseY = (int)mouseLoc.Y;
            this.leftClicked = leftClicked;
            this.rightClicked = rightClicked;
            this.dash = dash;
            this.drawTime = drawTime;
        }

        public override string ToString()
        {
            return sender + ", " + xInput + ", " + yInput + ", (" + mouseX + ", " + mouseY+"), " + leftClicked;
        }

        public long Sender { get { return sender; } }
        public int XInput { get { return xInput; } }
        public int YInput { get { return yInput; } }
        public int MouseX { get { return mouseX; } }
        public int MouseY { get { return mouseY; } }
        public bool LeftClicked { get { return leftClicked; } }
        public bool RightClicked { get { return rightClicked; } }
        public bool Dash { get { return dash; } }
        public double DrawTime { get { return drawTime; } }       
    }
}
