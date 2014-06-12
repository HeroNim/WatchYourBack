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
            entity.addComponent(new TransformComponent(x, y));
            wallTemplate.X = x;
            wallTemplate.Y = y;
            entity.addComponent(new ColliderComponent(wallTemplate.WallBody));
            entity.addComponent(new GraphicsComponent(wallTemplate.WallBody, wallTemplate.WallTexture, Color.Black));
            return entity;
        }

        public void setWallTemplate(WallTemplate template)
        {
            wallTemplate = template;
        }

        
    }
}
