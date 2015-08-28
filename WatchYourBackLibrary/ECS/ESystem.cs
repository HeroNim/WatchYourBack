using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// All Systems must inherit from this class. All logic for the game must be done in the systems. All systems are either exclusive or not: exclusive systems only act on entities that contain the
    /// exact components that the system acts on, whereas nonexclusive systems will act on all entities that contain the necessary components, even if they have extra components. Each system
    /// also must specifiy whether they want to be updated during the update loop, or the draw loop. Finally, all systems must have a priority value that is used to determine the order in which
    /// each system is updated during the update loop. 
    /// </summary>
    public abstract class ESystem
    {
        private Dictionary<int, Entity> entities;
        protected List<Entity> activeEntities;
        protected int components;
        private bool exclusive;
        private bool updateLoop;
        private int priority;
        
        public IECSManager manager;

        public ESystem(bool exclusive, bool updateLoop, int priority)
        {
            this.exclusive = exclusive;
            this.updateLoop = updateLoop;
            this.priority = priority;
            activeEntities = new List<Entity>();
            components = 0;
        }
               
        /// <summary>
        /// Initializes the system, pulling the entity list from the manager.
        /// </summary>
        /// <param name="manager">The ECS manager the system is attached to</param>
        public virtual void Initialize(IECSManager manager)
        {
            this.manager = manager;
            entities = manager.Entities;
        }
       
        /// <summary>
        /// Checks each entity on the entity list for matching components. If it is not exclusive, the entity must simply have the components; if it is,
        /// the entity must have only those components. The applicable entities are then updated.
        /// </summary>
        /// <param name="gameTime">The time since the last update</param>
        public void UpdateEntities(TimeSpan gameTime)
        {
            activeEntities.Clear();
            if (components != 0)
            {
                if (exclusive)
                {
                    foreach (Entity entity in entities.Values)
                        if ((entity.Mask ^ components) == 0)
                            activeEntities.Add(entity);
                }
                else
                    foreach (Entity entity in entities.Values)
                        if ((entity.Mask & components) == components)
                            activeEntities.Add(entity);
            }
            Update(gameTime);                                        
        }

        public int Priority
        {
            get { return priority; }
        }

        public bool Loop { get { return updateLoop; } }

        /// <summary>
        /// All the logic of the system is done here.
        /// </summary>
        /// <param name="gameTime">The time since the last update</param>
        public abstract void Update(TimeSpan gameTime);

        public event EventHandler inputFired;

        protected void OnFire(EventArgs e)
        {
            if (inputFired != null)
                inputFired(this, e);
        }

        protected void OnFire(Entity sender, EventArgs e)
        {
            if (inputFired != null)
                inputFired(sender, e);
        }

        public virtual void EventListener(object sender, EventArgs e) { }
    }
}
