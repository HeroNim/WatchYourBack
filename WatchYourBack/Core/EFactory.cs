using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WatchYourBack
{
    /*
     * A factory to create all the entities of the game. Can hold templates so that objects can be created by only specifying their location.
     * TODO: Figure out a better template system. Holding instances and setting them manually seems a bit stupid.
     */
    public class EFactory
    {
        private WallTemplate wallTemplate;
        private WallTemplate spawnTemplate;

        public Entity createAvatar(Rectangle rect, Texture2D texture)
        {
            Entity entity = new Entity();
            entity.addComponent(new TransformComponent(rect.X, rect.Y));
            entity.addComponent(new ColliderComponent(rect));
            entity.addComponent(new VelocityComponent(0, 0));
            entity.addComponent(new AvatarInputComponent());
            entity.addComponent(new GraphicsComponent(rect, texture));
            return entity;
        }

        public Entity createButton(int x, int y, int width, int height, Inputs type, Texture2D texture, string text, SpriteFont font)
        {
            Entity entity = new Entity();
            Rectangle body = new Rectangle(x, y, width, height);
            entity.addComponent(new ButtonComponent(type));
            entity.addComponent(new TransformComponent(x, y));
            entity.addComponent(new ColliderComponent(body));
            entity.addComponent(new GraphicsComponent(body, texture, text, font, Color.Blue));
            return entity;
        }

        public Entity createWall(int x, int y)
        {
            Entity entity = new Entity();
            Rectangle body = new Rectangle(x, y, wallTemplate.Width, wallTemplate.Height);
            entity.addComponent(new TileComponent(TileType.WALL));
            entity.addComponent(new TransformComponent(x, y));
            entity.addComponent(new ColliderComponent(body));
            entity.addComponent(new GraphicsComponent(body, wallTemplate.Texture));
            return entity;
        }

        public Entity createSpawn(int x, int y)
        {
            Entity entity = new Entity();
            Rectangle body = new Rectangle(x, y, spawnTemplate.Width, spawnTemplate.Height);
            entity.addComponent(new TileComponent(TileType.SPAWN));
            entity.addComponent(new TransformComponent(x, y));
            entity.addComponent(new GraphicsComponent(body, spawnTemplate.Texture));
            return entity;
        }

        

        public void setWallTemplate(WallTemplate template)
        {
            wallTemplate = template;
        }

        public void setSpawnTemplate(WallTemplate template)
        {
            spawnTemplate = template;
        }

        

        
    }
}
