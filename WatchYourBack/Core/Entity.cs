using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    //Container for all the components that make up game objects. Contains a list of all the components contained, and methods to modify said components. Each entity
    //can have only one of each component.
    class Entity
    {

        private bool isActive;
        private Dictionary<Type, EComponent> components;

        public Entity()
        {
            isActive = false;
            components = new Dictionary<Type, EComponent>();
        }

        //Checks if the entity has a component of this type already
        public bool hasComponent(Type type)
        {
            if (components.ContainsKey(type))
                return true;
            return false;
            
        }

        //Add a component to the entity
        public void addComponent(EComponent component)
        {
            if (!hasComponent(component.GetType()))
            {
                components.Add(component.GetType(), component);
                component.setEntity(this);
            }
        }

        //Remove a component from the entity
        public void removeComponent(Type component)
        {
            if (hasComponent(component))
                components.Remove(component);
        }

        //Initialize the entity. This sets the entity to active, and initializes all of it's components as well
        public void initialize()
        {
            isActive = true;
            foreach (EComponent component in components.Values)
                component.initialize();
        }

        public void update(float delta)
        {
            foreach (EComponent component in components.Values)
                component.update(delta);
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

    }
}
