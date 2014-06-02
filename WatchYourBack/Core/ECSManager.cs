using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    //Manages the systems in the game. Is responsible for initializing, updating, and removing systems as needed.
    class ECSManager
    {
        private List<ESystem> systems;
        private List<Entity> inactiveEntities;
        private List<Entity> activeEntities;
        private List<Entity> removal;

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
        }

        public void removeSystem(ESystem system)
        {
            systems.Remove(system);
        }

        public void removeEntity(Entity entity)
        {
            if (inactiveEntities.Contains(entity))
                inactiveEntities.Remove(entity);
            if (activeEntities.Contains(entity))
                activeEntities.Remove(entity);
        }
        
        public void addEntity(Entity entity)
        {
            inactiveEntities.Add(entity);
        }

        public List<Entity> Entities
        {
            get { return inactiveEntities; } 
        }

        public List<Entity> ActiveEntities
        {
            get { return activeEntities; }
        }

        public void update()
        {
            foreach (Entity entity in inactiveEntities)
                if (entity.IsActive)
                {
                    activeEntities.Add(entity);
                    removal.Add(entity);
                }

            foreach (Entity entity in removal)
                inactiveEntities.Remove(entity);
            removal.Clear();


            foreach (ESystem system in systems)
                system.updateEntities();
            
        }
    }
}
