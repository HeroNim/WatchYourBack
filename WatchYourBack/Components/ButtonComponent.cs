using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class ButtonComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.Button;
        public override int Mask { get { return bitMask; } }

        private string buttonText;

        public ButtonComponent(string text)
        {
            buttonText = text;
            
        }

        

    }
}
