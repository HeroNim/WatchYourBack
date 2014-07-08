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
    

    /* Manages the systems in the game. Is responsible for initializing, updating, and removing systems as needed. Also contains a list of
     * all the entities which has changed during the last update cycle; this allows for the server to send data to the client on what needs
     * to be removed, added, or modified.
     */
    
    public class ServerECSManager : IECSManager
    {
        private int currentID;
        private List<ESystem> systems;
        private Dictionary<int, Entity> inactiveEntities;
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, COMMANDS> changedEntities;
        private List<Entity> removal;
        private InputSystem input;
        const double timeStep = 1.0 / (double)SERVER_PROPERTIES.TIME_STEP;
        private Stopwatch debug;

        private double[] accumulator;
        private bool playing;

    

        public ServerECSManager(int playerCount)
        {
            systems = new List<ESystem>();
            activeEntities = new Dictionary<int, Entity>();
            changedEntities = new Dictionary<int, COMMANDS>();
            removal = new List<Entity>();
            inactiveEntities = new Dictionary<int, Entity>();
            currentID = 0;
            accumulator = new double[playerCount];
            debug = new Stopwatch();

        }

        public bool Playing { get { return playing; } set { playing = value; } }

        public void addContent(ContentManager content) { }

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
            entity.ID = currentID;
            currentID++;
            entity.initialize();
            activeEntities.Add(entity.ID, entity);
            addChangedEntities(entity, COMMANDS.ADD);
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

        public Dictionary<int, Entity> Entities
        {
            get { return inactiveEntities; } 
        }

        public Dictionary<int, Entity> ActiveEntities
        {
            get { return activeEntities; }
        }

        public Dictionary<int, COMMANDS> ChangedEntities
        {
            get { return changedEntities; }
        }

        public void addChangedEntities(Entity e, COMMANDS c)
        {
            if (!changedEntities.Keys.Contains(e.ID))
                changedEntities.Add(e.ID, c);
            else if (changedEntities.Keys.Contains(e.ID) && changedEntities[e.ID] != COMMANDS.REMOVE && c == COMMANDS.REMOVE)
                changedEntities[e.ID] = COMMANDS.REMOVE;
        }

        public void clearEntities()
        {
            foreach(Entity entity in removal)
            {
                inactiveEntities.Remove(entity.ID);
                activeEntities.Remove(entity.ID);
            }
            removal.Clear();
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
            {            
                if (system.Loop == true)
                    system.updateEntities(gameTime);   
            }
        }

        public double[] Accumulator
        {
            get { return accumulator; }
            set { accumulator = value; }
        }

        public double DrawTime { get; set; }

        public void RemoveAll()
        {
            clearEntities();
            foreach (Entity entity in inactiveEntities.Values)
                if (entity.IsActive)
                {
                    activeEntities.Add(entity.ID, entity);
                    removal.Add(entity);
                }

            foreach (Entity entity in removal)
                inactiveEntities.Remove(entity.ID);
            removal.Clear();

            foreach (Entity entity in activeEntities.Values)
                if (!entity.IsActive)
                {
                    inactiveEntities.Add(entity.ID, entity);
                    removal.Add(entity);
                }

            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ID);
            removal.Clear();
        }

        /*
         * Has the sprite batch draw all the entities that have a graphics component 
         */

        public bool hasGraphics()
        {
            return false;
        }

        public void draw(SpriteBatch spriteBatch) { }
        

        public InputSystem Input
        {
            get { return input; }
        }

        public Texture2D getTexture(string fileName)
        {
            return null;
        }

        
    }
}
