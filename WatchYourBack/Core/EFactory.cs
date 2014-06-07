using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBack
{
    class EFactory
    {
        public Entity createAvatar(int x, int y)
        {
            Entity entity = new Entity();
            entity.addComponent(new TransformComponent(x, y));
            entity.addComponent(new VelocityComponent(0, 0));
            entity.addComponent(new GraphicsComponent(x, y, 20, 20));
            return entity;
        }
    }
}
