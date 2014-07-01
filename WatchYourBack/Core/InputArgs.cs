using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
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
