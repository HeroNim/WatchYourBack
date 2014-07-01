using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WatchYourBackServer
{
    //Manages the systems in the game. Is responsible for initializing, updating, and removing systems as needed.
    public class ECSManager
    {
        private List<ESystem> systems;
        private List<Entity> inactiveEntities;
        private List<Entity> activeEntities;
        private List<Entity> removal;
        private InputSystem input;

        

        public ECSManager(List<Entity> entities)
        {
            systems = new List<ESystem>();
            activeEntities = new List<Entity>();
            removal = new List<Entity>();
            this.inactiveEntities = entities;
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
            entity.initialize();
            activeEntities.Add(entity);
        }

        public void removeEntity(Entity entity)
        {
            if (inactiveEntities.Contains(entity) || activeEntities.Contains(entity))
                removal.Add(entity);
        }
        
        public void addInput(InputSystem input)
        {
            this.input = input;
        }

        public List<Entity> Entities
        {
            get { return inactiveEntities; } 
        }

        public List<Entity> ActiveEntities
        {
            get { return activeEntities; }
        }

        public void clearEntities()
        {
            foreach(Entity entity in removal)
            {
                inactiveEntities.Remove(entity);
                activeEntities.Remove(entity);
            }
            removal.Clear();
        }

        /*
         * Updates the entity lists of the manager, moving active/inactive entities to their proper lists. Any systems that run
         * during the update loop are then updated.
         */
        public void update(GameTime gameTime)
        {
            clearEntities();
            foreach (Entity entity in inactiveEntities)
                if (entity.IsActive)
                {
                    activeEntities.Add(entity);
                    removal.Add(entity);
                }

            foreach (Entity entity in removal)
                inactiveEntities.Remove(entity);
            removal.Clear();

            foreach (Entity entity in activeEntities)
                if (!entity.IsActive)
                {
                    inactiveEntities.Add(entity);
                    removal.Add(entity);
                }

            foreach (Entity entity in removal)
                activeEntities.Remove(entity);
            removal.Clear();

            //Update the systems
            foreach (ESystem system in systems)
            {
                if (system.Loop == true)
                    system.updateEntities(gameTime);
            }
            
        }

        /*
         * Has the sprite batch draw all the entities that have a graphics component 
         */
        
    
        

        

        public InputSystem Input
        {
            get { return input; }
        }

        
    }
}
