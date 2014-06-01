using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class SystemManager
    {
        private List<ESystem> systems;
        private List<Entity> entities;

        public SystemManager(List<Entity> entities)
        {
            systems = new List<ESystem>();
            this.entities = entities;
        }

        public void add(ESystem system)
        {
            
            systems.Add(system);
            system.initialize(this);
        }

        public List<Entity> Entities
        {
            get { return entities; } 
        }
    }
}
