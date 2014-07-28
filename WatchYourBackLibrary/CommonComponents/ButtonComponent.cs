using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

namespace WatchYourBackLibrary
{
  
    /// <summary>
    /// The component that holds information relevant to a button, such as the function of the button and whether
    /// it has been focused.
    /// </summary>
    public class ButtonComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.BUTTON; } }
        public override Masks Mask { get { return Masks.BUTTON; } }

        private Inputs buttonType;
        private bool focused;
        private InputArgs args;

        public ButtonComponent(Inputs type)
        {
            buttonType = type;
            args = new InputArgs(buttonType);
            focused = false;
            
        }

        public InputArgs Args
        {
            get { return args; }
        }

        public bool Focused
        {
            get { return focused; }
            set { focused = value; }
        }

        

    }
}
