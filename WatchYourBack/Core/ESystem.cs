using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    /* All Systems must inherit from this class. All logic for the game must be done in the systems. All systems are either exclusive or not: exclusive systems only act on entities that contain the
     * exact components that the system acts on, whereas nonexclusive systems will act on all entities that contain the necessary components, even if they have extra components. Each system
     * also must specifiy whether they want to be updated during the update loop, or the draw loop.
     */
    abstract class ESystem
    {
        private List<Entity> entities;
        protected List<Entity> activeEntities;
        protected int components;
        private bool exclusive;
        private bool updateLoop;
        
        private ECSManager manager;

        public ESystem(bool exclusive, bool updateLoop)
        {
            this.exclusive = exclusive;
            this.updateLoop = updateLoop;
            activeEntities = new List<Entity>();
            components = 0;
        }
        
        //Initializes the system, pulling the entity list from the manager, and making sure that all of it's components are actually components.
        public void initialize(ECSManager manager)
        {
            this.manager = manager;
            entities = manager.ActiveEntities;

        }

        //Checks each entity on the entity list for matching components. If it is not exclusive, the entity must simply have the components; if it is,
        //the entity must have only those components. The applicable entities are then updated.
        public void updateEntities()
        {
            activeEntities.Clear();
            if (exclusive)
            {
                foreach (Entity entity in entities)
                    if ((entity.Mask ^ components) == 0)
                        activeEntities.Add(entity);
            }
            else
                foreach (Entity entity in entities)
                    if ((entity.Mask & components) == components)
                        activeEntities.Add(entity);
            update();
                            
            
        }

        public bool Loop { get { return updateLoop; } }

        //Logic goes here
        public abstract void update();
    }
}
