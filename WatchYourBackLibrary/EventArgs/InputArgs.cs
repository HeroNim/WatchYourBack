using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
    public enum Inputs
    {
        EXIT,
        START_SINGLE,
        START_MUTLI,
        RESUME,
        PAUSE,
        ATTACK
    };

    /// <summary>
    /// Arguments sent by the client locally, informing the game of player input
    /// </summary>
    public class InputArgs : EventArgs
    {
        private Inputs type;
        private int mouseX;
        private int mouseY;

        public InputArgs(Inputs type)
        {
            this.type = type;
            mouseX = -1;
            mouseY = -1;
        }

        public InputArgs(Inputs type, int x, int y)
        {
            this.type = type;
            mouseX = x;
            mouseY = y;
        }


        public Inputs InputType { get { return type; } }
        public int MouseX { get { return mouseX; } }
        public int MouseY { get { return mouseY; } }
    }
}
