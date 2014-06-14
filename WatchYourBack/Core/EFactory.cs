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
        private WallTemplate wallTemplate;

        public Entity createAvatar(Rectangle rect, Texture2D texture, Color color)
        {
            Entity entity = new Entity();
            entity.addComponent(new TransformComponent(rect.X, rect.Y));
            entity.addComponent(new ColliderComponent(rect));
            entity.addComponent(new VelocityComponent(0, 0));
            entity.addComponent(new PlayerInputComponent());
            entity.addComponent(new GraphicsComponent(rect, texture, color));
            return entity;
        }

        public Entity createWall(int x, int y)
        {
            Entity entity = new Entity();
            Rectangle body = new Rectangle(x, y, wallTemplate.Width, wallTemplate.Height);
            entity.addComponent(new TileComponent());
            entity.addComponent(new TransformComponent(x, y));
            entity.addComponent(new ColliderComponent(body));
            entity.addComponent(new GraphicsComponent(body, wallTemplate.Texture, Color.Black));
            return entity;
        }

        public void setWallTemplate(WallTemplate template)
        {
            wallTemplate = template;
        }

        
    }
}
