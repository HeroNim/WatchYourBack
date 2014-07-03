using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackServer
{
    public enum Inputs
    {
        EXIT,
        START,
        PAUSE,
        ATTACK
    };

    public class InputArgs : EventArgs
    {
        private Inputs type;
        public InputArgs(Inputs type)
        {
            this.type = type;
        }
        public Inputs InputType { get { return type; } }
    }
}
