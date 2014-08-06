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
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, EntityCommands> changedEntities;
        private List<ESystem> systems;
        private List<Entity> removal;
        private LevelInfo levelInfo;
        private InputSystem input;
        private int currentID;

        const double timeStep = 1.0 / (double)ServerSettings.TimeStep;
        private bool playing;


    

        public ServerECSManager(int playerCount)
        {
            systems = new List<ESystem>();
            activeEntities = new Dictionary<int, Entity>();
            changedEntities = new Dictionary<int, EntityCommands>();
            removal = new List<Entity>();
            currentID = 0;

        }

        public bool Playing { get { return playing; } set { playing = value; } }

        public void Initialize()
        {
            foreach (ESystem system in systems)
            {
                foreach (ESystem other in systems)
                {
                    if (other != system)
                    {
                        other.inputFired += new EventHandler(system.EventListener);
                    }
                }
                system.initialize(this);
            }
        }

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
            addChangedEntities(entity, EntityCommands.Add);
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
                addChangedEntities(entity, EntityCommands.Remove);
            }
        }
        
        public void addInput(InputSystem input)
        {
            this.input = input;
        }

        public LevelInfo LevelInfo
        {
            get { return levelInfo; }
            set { levelInfo = value; }
        }        

        public Dictionary<int, Entity> Entities
        {
            get { return activeEntities; }
        }

        public Dictionary<int, EntityCommands> ChangedEntities
        {
            get { return changedEntities; }
        }

        public List<ESystem> Systems
        {
            get { return systems; }
        }

        public void addChangedEntities(Entity e, EntityCommands c)
        {
            if (!changedEntities.Keys.Contains(e.ServerID))
                changedEntities.Add(e.ServerID, c);
            else if (changedEntities.Keys.Contains(e.ServerID) && changedEntities[e.ServerID] != EntityCommands.Remove && c == EntityCommands.Remove)
                changedEntities[e.ServerID] = EntityCommands.Remove;
        }

       

        
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
