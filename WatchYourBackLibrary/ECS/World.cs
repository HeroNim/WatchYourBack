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
        MainMenu,
        ConnectMenu,
        PauseMenu,
        InGame,
        InGameMulti
    };

   

    /// <summary>
    /// A base container class for each game state.
    /// </summary>
    /// <remarks>
    /// Each world is equivalent to a screen; each world contains its own set of entities and systems, which
    /// are then updated on a world by world basis. Generally only the currently active world is updated and drawn; 
    /// the rest are on standby until they become active again. However, this is not concrete, and being able to update 
    /// and draw multiple worlds simultaenously could be useful, such as if one wanted to overlay screens.
    /// </remarks>
    public class World
    {
        private IECSManager systemManager;
        private Worlds worldType;


        public World(Worlds type)
        {
            worldType = type;
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

        public Worlds WorldType
        {
            get { return worldType; }
        }

        





       
    }
}
