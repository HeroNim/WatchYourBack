using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// An interface which input systems can use to identify themselves
    /// </summary>
    public interface InputSystem
    {
        event EventHandler inputFired;
    }
}
