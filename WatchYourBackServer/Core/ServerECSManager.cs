using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Lidgren.Network;
using WatchYourBackLibrary;
using System.Diagnostics;

namespace WatchYourBackServer
{
    


    
    /// <summary>
    /// The server side version of the ECSManager. Manages the entities and systems in the game; it is responsible for initializing, updating, and removing them as needed.
    /// Unlike the client version of the ECSManager, it is not responsible for drawing any graphical elements that entities might have.
    /// </summary>
    public class ServerECSManager : IECSManager
    {
        private int currentID;
        private List<ESystem> systems;
        private LevelComponent levelInfo;
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, COMMANDS> changedEntities;
        private List<Entity> removal;
        private InputSystem input;
        const double timeStep = 1.0 / (double)SERVER_PROPERTIES.TIME_STEP;

        private bool playing;


    

        public ServerECSManager(int playerCount)
        {
            systems = new List<ESystem>();
            activeEntities = new Dictionary<int, Entity>();
            changedEntities = new Dictionary<int, COMMANDS>();
            removal = new List<Entity>();
            currentID = 0;

        }

        public bool Playing { get { return playing; } set { playing = value; } }

        public void addSystem(ESystem system)
        {
            systems.Add(system);
            system.initialize(this);
            systems = systems.OrderBy(o => o.Priority).ToList();
        }

        public void removeSystem(ESystem system)
        {
            systems.Remove(system);
        }

        public void addEntity(Entity entity)
        {
            entity.ServerID = assignID();
            entity.initialize();
            activeEntities.Add(entity.ServerID, entity);
            addChangedEntities(entity, COMMANDS.ADD);
        }

        public int assignID()
        {
            currentID = 0;
            while (activeEntities.Keys.Contains(currentID))
                currentID++;
            return currentID;

        }

        public void removeEntity(Entity entity)
        {
            if (activeEntities.Values.Contains(entity) && !removal.Contains(entity))
            {
                removal.Add(entity);
                addChangedEntities(entity, COMMANDS.REMOVE);
            }
        }
        
        public void addInput(InputSystem input)
        {
            this.input = input;
        }

        public LevelComponent LevelInfo
        {
            get { return levelInfo; }
            set { levelInfo = value; }
        }        

        public Dictionary<int, Entity> Entities
        {
            get { return activeEntities; }
        }

        public Dictionary<int, COMMANDS> ChangedEntities
        {
            get { return changedEntities; }
        }

        public List<ESystem> Systems
        {
            get { return systems; }
        }

        public void addChangedEntities(Entity e, COMMANDS c)
        {
            if (!changedEntities.Keys.Contains(e.ServerID))
                changedEntities.Add(e.ServerID, c);
            else if (changedEntities.Keys.Contains(e.ServerID) && changedEntities[e.ServerID] != COMMANDS.REMOVE && c == COMMANDS.REMOVE)
                changedEntities[e.ServerID] = COMMANDS.REMOVE;
        }

       

        /*
         * Updates the entity lists of the manager, moving active/inactive entities to their proper lists. Any systems that run
         * during the update loop are then updated.
         * 
         * 
         */
        public void update(TimeSpan gameTime)
        {            
            gameTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * timeStep));
            //Update the systems
            foreach (ESystem system in systems)            
                if (system.Loop == true)
                    system.updateEntities(gameTime);   
        }      

        public void RemoveAll()
        {
            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ServerID);
            removal.Clear();
            foreach (Entity entity in activeEntities.Values)
                if (!entity.IsActive)
                    removal.Add(entity);
            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ServerID);
            removal.Clear();
        }       

        public bool hasGraphics()
        {
            return false;
        }

        public InputSystem Input
        {
            get { return input; }
        }

        
    }
}
