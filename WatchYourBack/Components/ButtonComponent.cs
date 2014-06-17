using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
  

    public class ButtonComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.BUTTON;
        public override int Mask { get { return bitMask; } }

        private string buttonText;
        private Inputs buttonType;
        private InputArgs args;

        public ButtonComponent(Inputs type, string text)
        {
            buttonText = text;
            buttonType = type;
            args = new InputArgs(buttonType);
            
        }

        public InputArgs Args
        {
            get { return args; }
        }

        

    }
}
