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
        Graphics = 1 << 2,
        Collider = 1 << 3,
        PlayerInput = 1 << 4,
        Tile = 1 << 5,
        Level = 1 << 6
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
        public bool hasComponent(int bitMask)
        {
            if ((this.mask & bitMask) != 0)
                return true;
            return false;
            
            
        }

        //Add a component to the entity
        public void addComponent(EComponent component)
        {
            if (!hasComponent(component.Mask))
            {
                components.Add(component.GetType(), component);
                component.setEntity(this);
                mask += component.Mask;
            }
        }

        //Remove a component from the entity
        public void removeComponent(EComponent component)
        {
            if (hasComponent(component.Mask))
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
