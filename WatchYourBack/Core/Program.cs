#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace WatchYourBack
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    /// 
    

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
<<<<<<< HEAD
            using (var game = new ClientGameLoop())
=======
            using (var game = new GameLoop())
>>>>>>> origin/Networking
                game.Run();
        }
    }
#endif
}
