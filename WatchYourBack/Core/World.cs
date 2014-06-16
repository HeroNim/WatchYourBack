using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    public enum Worlds
    {
        MAIN_MENU,
        PAUSE_MENU,
        IN_GAME
    };

    public class World
    {
        private ECSManager systemManager;
        private Worlds menuType;


        public World(Worlds type)
        {
            systemManager = new ECSManager(new List<Entity>(), new EFactory());
            menuType = type;
            
        }

        public ECSManager Manager
        {
            get { return systemManager; }
        }

        public Worlds MenuType
        {
            get { return menuType; }
        }

       
    }
}
