using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    public class ButtonArgs : EventArgs
    {
        private Buttons type;
        public ButtonArgs(Buttons type)
        {
            this.type = type;
        }
        public Buttons ButtonType { get { return type; } }
    }
}
