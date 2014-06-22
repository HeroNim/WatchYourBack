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
        TRANSFORM = 1 << 0, 
        VELOCITY = 1 << 1, 
        GRAPHICS = 1 << 2,
        COLLIDER = 1 << 3,
        PLAYER_INPUT = 1 << 4,
        TILE = 1 << 5,
        LEVEL = 1 << 6,
        BUTTON = 1 << 7,
        WEAPON = 1 << 8,
        RAY_COLLIDER = 1 << 9
        
    };

    public class Entity
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
        public bool hasComponent(Masks bitMask)
        {
            if ((this.mask & (int)bitMask) != 0)
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
                mask += (int)component.Mask;
            }
        }

        //Remove a component from the entity
        public void removeComponent(EComponent component)
        {
            if (hasComponent(component.Mask))
            {
                components.Remove(component.GetType());
                mask -= (int)component.Mask;
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
