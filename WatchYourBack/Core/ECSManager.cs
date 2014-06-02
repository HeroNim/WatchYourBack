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
        private List<Entity> entities;
        private List<Entity> activeEntities;

        public ECSManager(List<Entity> entities)
        {
            systems = new List<ESystem>();
            activeEntities = new List<Entity>();
            this.entities = entities;
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
            entities.Remove(entity);
            if (activeEntities.Contains(entity))
                activeEntities.Remove(entity);
        }
        
        public void addEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public List<Entity> Entities
        {
            get { return entities; } 
        }

        public List<Entity> ActiveEntities
        {
            get { return activeEntities; }
        }

        public void update(float delta)
        {
            foreach (Entity entity in entities)
                if (entity.IsActive)
                    activeEntities.Add(entity);

            foreach (ESystem system in systems)
                system.updateEntities();
            
        }
    }
}
