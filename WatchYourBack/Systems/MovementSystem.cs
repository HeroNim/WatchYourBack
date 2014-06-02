using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class MovementSystem : ESystem
    {
        

        public MovementSystem(bool exclusive) : base(exclusive)
        {
            components = new List<Type>();
            components.Add(typeof(TransformComponent));
        }

        public override void update(Entity entity)
        {
            TransformComponent transform = (TransformComponent)entity.Components[typeof(TransformComponent)];
            transform.XPos += 5;
        }
    }
}
