using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    /* 
     * Container for all the components that make up game objects. Contains a list of all the components contained, and methods to modify said components. Each entity
     * can have only one of each component. Each component has a component-specific bit; combining these into a bitmask for the entity allows for quick lookup and comparison
     * using bitwise operators.
     */

    public enum Masks 
    { 
        Transform = 1 << 0, 
        Velocity = 1 << 1, 
        Graphics = 1 << 2 
    };

    class Entity
    {
        private int mask;
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
                mask += component.Mask;
            }
        }

        //Remove a component from the entity
        public void removeComponent(EComponent component)
        {
            if (hasComponent(component.GetType()))
            {
                components.Remove(component.GetType());
                mask -= component.Mask;
            }
        }

        //Initialize the entity. This sets the entity to active, and initializes all of it's components as well
        public void initialize()
        {
            isActive = true;
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public Dictionary<Type, EComponent> Components
        {
            get { return components; }
        }

        public int Mask { get { return mask; } }
    }
}
