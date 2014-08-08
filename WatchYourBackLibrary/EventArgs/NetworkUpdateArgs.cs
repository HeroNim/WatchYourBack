using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{
    [Serializable()]
    public class NetworkUpdateArgs : EventArgs
    {
        private ServerCommands command;
        
        public NetworkUpdateArgs(ServerCommands command)
        {
            this.command = command;
        }

        public ServerCommands Command
        {
            get { return command; }
        }
    }
}
