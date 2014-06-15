using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class World
    {
        private ECSManager systemManager;
        
        public World()
        {
            systemManager = new ECSManager(new List<Entity>(), new EFactory());
        }

        public ECSManager Manager
        {
            get { return systemManager; }
        }
    }
}
