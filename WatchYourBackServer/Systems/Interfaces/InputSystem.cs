using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackServer
{
    public interface InputSystem
    {
        event EventHandler inputFired;
    }
}
