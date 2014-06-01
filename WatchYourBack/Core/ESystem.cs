using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    abstract class ESystem
    {
        private List<Entity> entities;
        protected List<Type> components;
        private bool exclusive;
        private SystemManager manager;

        public ESystem(bool exclusive)
        {
            this.exclusive = exclusive;
        }

        public void initialize(SystemManager manager)
        {
            this.manager = manager;
            entities = manager.Entities;

            foreach (Type type in components)
                if (!typeof(EComponent).IsAssignableFrom(type))
                    throw new ArgumentException();

            if (exclusive == true)
                foreach (Entity entity in entities)
                    if (entity.Components.Count != components.Count)
                        entities.Remove(entity);

            foreach (Type type in components)
                foreach (Entity entity in entities)
                    if (!entity.hasComponent(type))
                        entities.Remove(entity);

        }
    }
}
