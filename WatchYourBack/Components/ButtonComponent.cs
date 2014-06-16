using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    public enum Buttons
    {
        Exit,
        Start,
        Pause
    };

    public class ButtonComponent : EComponent
    {
        public readonly static int bitMask = (int)Masks.Button;
        public override int Mask { get { return bitMask; } }

        private string buttonText;
        private Buttons buttonType;
        private ButtonArgs args;

        public ButtonComponent(Buttons type, string text)
        {
            buttonText = text;
            buttonType = type;
            args = new ButtonArgs(buttonType);
            
        }

        public ButtonArgs Args
        {
            get { return args; }
        }

        

    }
}
