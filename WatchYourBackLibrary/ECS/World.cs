using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;



namespace WatchYourBackLibrary
{
    public enum Worlds
    {
        MAIN_MENU,
        CONNECT_MENU,
        PAUSE_MENU,
        IN_GAME,
        IN_GAME_MULTI
    };

    /*
     * The base container class for everything in the game. Each world is equivalent to a screen; each world contains its own set of entities and systems, which
     * are then updated on a world by world basis. Generally only the currently active world is updated and drawn; the rest are on standby until they become active again.
     * However, this is not concrete, and being able to update and draw multiple worlds simultaenously could be useful, such as if one wanted to overlay screens.
     * 
     */

    public class World
    {
        private IECSManager systemManager;
        private Worlds menuType;


        public World(Worlds type)
        {
            menuType = type;
            systemManager = null;
        }

        public void addManager(IECSManager manager)
        {
            systemManager = manager;
        }

        public IECSManager Manager
        {
            get { return systemManager; }
        }

        public Worlds MenuType
        {
            get { return menuType; }
        }

        





       
    }
}
