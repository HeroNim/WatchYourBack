using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack.Core
{
    abstract class ESystem
    {
        public ESystem(List<Entity> entities, params Type[] components)
        {
            if(components.Any(t => !typeof(EComponent).IsAssignableFrom(t)))
                throw new ArgumentException();
        }
    }
}
