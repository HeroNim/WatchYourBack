using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{

    //Components that make up each entity. Ideally contains only data and the methods needed to access them; however, for smaller programs and tightly coupled components, it may
    //be simpler to include the logic in the components themselves.
    abstract class Component
    {
        private bool isActive;
        private Entity entity;


        //Set the component to an entity
        public void setEntity(Entity entity)
        {
            this.entity = entity;
        }

        //Get the entity the component is attached to
        public Entity getEntity()
        {
            return this.entity;
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public void initialize() { }

        public void update(float delta) { }
    }
}
