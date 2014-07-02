using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
namespace WatchYourBackServer
{
    class NetworkIOSystem : ESystem, InputSystem
    {
        public NetworkIOSystem()
           : base(false, true, 2) 
        {
            components += AvatarInputComponent.bitMask;
        }

        public override void update(double lastUpdate)
        {
            int x = 5;
        }

        public event EventHandler inputFired;

        private void onFire(EventArgs e)
        {
            if (inputFired != null)
                inputFired(this, e);
        }

        private void onFire(Entity sender, EventArgs e)
        {
            if (inputFired != null)
                inputFired(sender, e);
        }
    }
}
