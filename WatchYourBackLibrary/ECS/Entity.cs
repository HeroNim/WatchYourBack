using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{
    [Serializable()]
    public enum Entities
    {
        Player,
        Avatar,
        Wall,
        Sword,
        Thrown
    }
      
    /// <summary>
    /// Container for all the components that make up game objects. Contains a list of all the components contained, and methods to modify said components. Each entity
    /// can have only one of each component. Each component has a component-specific bit; combining these into a bitmask for the entity allows for quick lookup and comparison
    /// using bitwise operators.
    /// </summary>
    public class Entity
    {
        private int clientID;
        private int serverID;
        private bool destructable;
        private bool drawable;
        private Entities type;
        private int mask;
        private bool isActive;
        private Dictionary<Masks, EComponent> components;

        public Entity()
        {
            isActive = false;
            components = new Dictionary<Masks, EComponent>();
            clientID = -1;
            serverID = -1;
            drawable = true;
            destructable = false;
        }

        public Entity(params EComponent[] args)
        {
            isActive = false;
            components = new Dictionary<Masks, EComponent>();
            foreach (EComponent arg in args)
            {
                this.AddComponent(arg);
            }
            clientID = -1;
            serverID = -1;
            drawable = true;
            destructable = false;
        }

        public Entity(bool destructable, params EComponent[] args) 
            : this(args)
        {
            this.destructable = destructable;
        }

        /// <summary>
        /// Checks if the entity has a component of this type already
        /// </summary>
        /// <param name="bitMask">The bitmask of the component</param>
        /// <returns>True if the entity has the component</returns>
        public bool HasComponent(Masks bitMask)
        {
            if ((this.mask & (int)bitMask) != 0)
                return true;
            return false;         
        }

        /// <summary>
        /// Add a component to the entity
        /// </summary>
        /// <param name="component">The component to add</param>
        public void AddComponent(EComponent component)
        {
            if (!HasComponent(component.Mask))
            {
                components.Add(component.Mask, component);
                component.SetEntity(this);
                mask |= component.BitMask;
            }
        }

        /// <summary>
        /// Remove a component from the entity
        /// </summary>
        /// <param name="component">The component to remove</param>
        public void RemoveComponent(EComponent component)
        {
            if (HasComponent(component.Mask))
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
        public void Initialize()
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

        public EComponent GetGraphics
        {
            get { return components[Masks.Graphics]; }
        }

        public bool IsDestructable
        {
            get{return destructable;}
        }

        public int Mask { get { return mask; } }

        public int ClientID
        {
            get { return this.clientID; }
            set { this.clientID = value; }
        }

        public int ServerID
        {
            get { return this.serverID; }
            set
            {
                this.serverID = value;
            }
        }

        public Entities Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public bool Drawable
        {
            get { return drawable; }
            set { drawable = value; }
        }

        public T GetComponent<T>() where T : EComponent
        {
            foreach(EComponent c in components.Values)
                if (c is T)          
                    return (T)(object)c;
            throw new Exception("No such component");
                
        }       
    }
}
