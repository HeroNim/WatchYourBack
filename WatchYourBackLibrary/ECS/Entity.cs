using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{
    [Serializable()]
    public enum ENTITIES
    {
        PLAYER,
        AVATAR,
        WALL,
        SWORD,
        THROWN
    }
    
    
    /// <summary>
    /// Container for all the components that make up game objects. Contains a list of all the components contained, and methods to modify said components. Each entity
    /// can have only one of each component. Each component has a component-specific bit; combining these into a bitmask for the entity allows for quick lookup and comparison
    /// using bitwise operators.
    /// </summary>
    public class Entity
    {
        private int id;
        private bool destructable;
        private bool drawable;
        private ENTITIES type;
        private int mask;
        private bool isActive;
        private Dictionary<Masks, EComponent> components;

       

        public Entity()
        {
            isActive = false;
            components = new Dictionary<Masks, EComponent>();
            id = -1;
            drawable = true;
            destructable = false;
        }

        public Entity(params EComponent[] args)
        {
            isActive = false;
            components = new Dictionary<Masks, EComponent>();
            foreach (EComponent arg in args)
            {
                this.addComponent(arg);
            }
            id = -1;
            drawable = true;
            destructable = false;
        }

        public Entity(bool destructable, params EComponent[] args) 
            : this(args)
        {
            this.destructable = destructable;
        }

        public int ID
        {
            get { return this.id; }
            set
            {
                if (this.id == -1)
                    this.id = value;
            }
        }

        public ENTITIES Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public bool Drawable
        {
            get { return drawable; }
            set { drawable = value; }
        }
           

        /// <summary>
        /// Checks if the entity has a component of this type already
        /// </summary>
        /// <param name="bitMask">The bitmask of the component</param>
        /// <returns>True if the entity has the component</returns>
        public bool hasComponent(Masks bitMask)
        {
            if ((this.mask & (int)bitMask) != 0)
                return true;
            return false;
            
            
        }

        /// <summary>
        /// Add a component to the entity
        /// </summary>
        /// <param name="component">The component to add</param>
        public void addComponent(EComponent component)
        {
            if (!hasComponent(component.Mask))
            {
                components.Add(component.Mask, component);
                component.setEntity(this);
                mask |= component.BitMask;
            }
        }


        /// <summary>
        /// Remove a component from the entity
        /// </summary>
        /// <param name="component">The component to remove</param>
        public void removeComponent(EComponent component)
        {
            if (hasComponent(component.Mask))
            {
                components.Remove(component.Mask);
                mask = 0;
                foreach (EComponent existingComponent in components.Values)
                    mask |= existingComponent.BitMask;
                
            }
        }
      
        /// <summary>
        /// Initialize the entity. This sets the entity to active, and initializes all of it's components as well
        /// </summary>
        public void initialize()
        {
            isActive = true;
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public Dictionary<Masks, EComponent> Components
        {
            get { return components; }
        }

        public EComponent getGraphics
        {
            get { return components[Masks.GRAPHICS]; }
        }

        public bool IsDestructable
        {
            get{return destructable;}
        }

        public int Mask { get { return mask; } }
    }
}
