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

    /*
     * The base container class for everything in the game. Each world is equivalent to a screen; each world contains its own set of entities and systems, which
     * are then updated on a world by world basis. Generally only the currently active world is updated and drawn; the rest are on standby until they become active again.
     * However, this is not concrete, and being able to draw multiple worlds simultaenously could be useful, such as if one wanted to overlay screens.
     * 
     */

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
