using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using WatchYourBackLibrary;

namespace WatchYourBack
{      
    /// <summary>
    /// The client side version of the ECS Manager. Manages the entities and systems in the game; it is responsible for initializing, updating, and removing them as needed.
    /// It is also responsible for drawing any graphical elements that entities might have via XNA's built-in draw functions.
    /// </summary>
    public class ClientECSManager : IECSManager
    {

        private QuadTree<Entity> entityQuadtree;
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, EntityCommands> changedEntities;
        private List<ESystem> systems;
        private List<Entity> removal;
        private LevelInfo levelInfo;
        private UI ui;
        private int currentID;
        private double drawTime;

        private bool playing;

        public ClientECSManager()
        {
            ui = null;
            currentID = 0;
            drawTime = 0;
            systems = new List<ESystem>();
            entityQuadtree = new QuadTree<Entity>(0, 0, GameData.gameWidth, GameData.gameHeight, 4);
            activeEntities = new Dictionary<int, Entity>();
            changedEntities = new Dictionary<int, EntityCommands>();
            removal = new List<Entity>();
        }

        public bool Playing { get { return playing; } set { playing = value; } }

        public void Initialize()
        {
            foreach(ESystem system in systems)
            {
                foreach(ESystem other in systems)
                {
                    if(other != system)
                    {
                        other.inputFired += new EventHandler(system.EventListener);
                    }
                }
                system.Initialize(this);                
            }
        }
      
        public void AddUI(UI info)
        {
            ui = info;
            foreach (Entity e in ui.UIElements)
            {
                AddEntity(e);
            }
        }

        public UI UI
        {
            get { return ui; }
        }

        public void AddEntity(Entity entity)
        {
            entity.ClientID = assignID();
            entity.Initialize();
            activeEntities.Add(entity.ClientID, entity);
            if (entity.HasComponent(Masks.Transform))
                entityQuadtree.Add(entity, entity.GetComponent<TransformComponent>().Body);
            AddChangedEntities(entity, EntityCommands.Add);
        }

        public void RemoveEntity(Entity entity)
        {
            if (activeEntities.Values.Contains(entity) && !removal.Contains(entity))
            {
                removal.Add(entity);
                AddChangedEntities(entity, EntityCommands.Remove);
            }
            entityQuadtree.Remove(entity);
        }

        public void AddSystem(ESystem system)
        {
            systems.Add(system);
            system.Initialize(this);
            systems = systems.OrderBy(o => o.Priority).ToList();
        }

        public void RemoveSystem(ESystem system)
        {
            systems.Remove(system);
        }      

        public int assignID()
        {
            currentID = 0;
            while (activeEntities.Keys.Contains(currentID))
                currentID++;
            return currentID;

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

        public QuadTree<Entity> QuadTree
        {
            get { return entityQuadtree; }
        }

        public void AddChangedEntities(Entity e, EntityCommands c)
        {
            if (!changedEntities.Keys.Contains(e.ClientID))
                changedEntities.Add(e.ClientID, c);
            else if (changedEntities.Keys.Contains(e.ClientID) && changedEntities[e.ClientID] != EntityCommands.Remove && c == EntityCommands.Remove)
                changedEntities[e.ClientID] = EntityCommands.Remove;
        }
             
        public void Update(TimeSpan gameTime)
        {
            //Update the systems
            foreach (ESystem system in systems)
                if (system.Loop == true)
                    system.UpdateEntities(gameTime);          
            RemoveAll();
        }

        public double DrawTime
        {
            get { return drawTime; }
            set { drawTime = value; }
        }

        public void RemoveAll()
        {
            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ClientID);
            removal.Clear();                 
            foreach (Entity entity in activeEntities.Values)
                if (!entity.IsActive)
                    removal.Add(entity);
            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ClientID);
            removal.Clear();
        }      

        public LevelInfo LevelInfo
        {
            get { return levelInfo; }
            set { levelInfo = value; }
        }

        public bool HasGraphics()
        {
            return true;
        }           
    }
}
