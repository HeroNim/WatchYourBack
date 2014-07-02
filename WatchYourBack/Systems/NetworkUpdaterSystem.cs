using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBack
{
    class NetworkUpdaterSystem : ESystem
    {
        NetworkIOSystem input;
        NetworkArgs toSend;

        public NetworkUpdaterSystem(NetworkIOSystem inputs) : base(false, true, 2)
        {
            input = inputs;
            toSend = null;
            input.inputFired += checkInput;
        }

        public override void update(GameTime gameTime)
        {
            Console.WriteLine(toSend.ToString());
            
        }

        private void checkInput(Object sender, EventArgs e)
        {
            toSend = (NetworkArgs)e;
        }
    }
}
