using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack.Core
{
    abstract class ESystem
    {
        private List<Entity> entities;
        private abstract List<Type> components;

        public ESystem(List<Entity> entities, bool exclusive)
        {
           foreach(Type type in components)
               if(!type.IsAssignableFrom(typeof(EComponent)))
                   throw new ArgumentException();

           if (exclusive == true)
               foreach (Entity entity in entities)
                   if (entity.Components.Count != components.Count)
                       entities.Remove(entity);

               foreach (Type type in components)
                   foreach (Entity entity in entities)
                       if (!entity.hasComponent(type))
                           entities.Remove(entity);


               
                
        }
    }
}
