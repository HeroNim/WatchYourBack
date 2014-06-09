using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBack
{
    class EFactory
    {
        public Entity createAvatar(int x, int y, Rectangle rect, Texture2D texture, Color color)
        {
            Entity entity = new Entity();
            entity.addComponent(new TransformComponent(x, y));
            entity.addComponent(new ColliderComponent(rect));
            entity.addComponent(new VelocityComponent(2, 1));
            entity.addComponent(new GraphicsComponent(rect, texture, color));
            return entity;
        }

        public Entity createWall(int x, int y, Rectangle rect, Texture2D texture, Color color)
        {
            Entity entity = new Entity();
            entity.addComponent(new TransformComponent(x, y));
            entity.addComponent(new ColliderComponent(rect));
            entity.addComponent(new GraphicsComponent(rect, texture, color));
            return entity;
        }
    }
}
