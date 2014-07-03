using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
  

    public class ButtonComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.BUTTON;
        public override Masks Mask { get { return Masks.BUTTON; } }

        private Inputs buttonType;
        private InputArgs args;

        public ButtonComponent(Inputs type)
        {
            buttonType = type;
            args = new InputArgs(buttonType);
            
        }

        public InputArgs Args
        {
            get { return args; }
        }

        

    }
}
