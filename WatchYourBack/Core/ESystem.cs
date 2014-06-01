using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    //All Systems must inherit from this class. All logic for the game must be done here. All systems are either exclusive or inclusive: exclusive systems only act on entities that contain the
    //exact components that the system acts on, whereas inclusive systems will act on all entities that contain the necessary components, even if they have extra components.
    abstract class ESystem
    {
        private List<Entity> entities;
        protected List<Type> components;
        private bool exclusive;
        private ECSManager manager;

        public ESystem(bool exclusive)
        {
            this.exclusive = exclusive;
        }
        
        public void initialize(ECSManager manager)
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

        //Logic goes here
        public abstract void update();
    }
}
