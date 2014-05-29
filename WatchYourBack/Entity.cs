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
        private List<Component> components;

        public Entity()
        {
            isActive = false;
            components = new List<Component>();
        }

        //Checks if the entity has a component of this type already
        public bool hasComponent(Component component)
        {
            foreach(Component current in components)
            {
                if(current.GetType() == component.GetType())
                    return true;
            }
            return false;
        }

        //Add a component to the entity
        public void addComponent(Component component)
        {
            if(!hasComponent(component))
                this.components.Add(component);
        }

        //Remove a component from the entity
        public void removeComponent(Component component)
        {
            if(hasComponent(component))
                this.components.Remove(component);
        }

        //Initialize the entity. This sets the entity to active, and initializes all of it's components as well
        public void initialize()
        {
            isActive = true;
            foreach (Component component in components)
                component.initialize();
        }

        public void update(float delta)
        {

        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

    }
}
