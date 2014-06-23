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
     */
    public static class EFactory
    {
        
        public static Entity createAvatar(Rectangle rect, Texture2D texture)
        {

                return new Entity(
                new WeaponComponent(10, 5, true),
                new TransformComponent(rect.X, rect.Y),
                new ColliderComponent(rect),
                new VelocityComponent(0, 0),
                new AvatarInputComponent(),
                new GraphicsComponent(rect, texture));
        }

        public static Entity createWeapon(float xOrigin, float yOrigin, Rectangle body, Texture2D texture)
        {


                return new Entity(
                new TransformComponent(xOrigin, yOrigin),
                new ColliderComponent(body),
                new VelocityComponent(0, 0),
                new GraphicsComponent(body, texture));
        }

        public static Entity createButton(int x, int y, int width, int height, Inputs type, Texture2D texture, string text, SpriteFont font)
        {


                return new Entity(
                new ButtonComponent(type),
                new TransformComponent(x, y),
                new ColliderComponent(new Rectangle(x, y, width, height)),
                new GraphicsComponent(new Rectangle(x, y, width, height), texture, text, font, Color.Blue));
        }

        public static Entity createWall(int x, int y, int width, int height, Texture2D texture)
        {
            return new Entity(
                new TileComponent(TileType.WALL),
                new TransformComponent(x, y),
                new ColliderComponent(new Rectangle(x, y, width, height)),
                new GraphicsComponent(new Rectangle(x, y, width, height), texture));
        }

        public static Entity createSpawn(int x, int y, int width, int height, Texture2D texture)
        {
            return new Entity(
                new TileComponent(TileType.SPAWN),
                new TransformComponent(x, y),
                new GraphicsComponent(new Rectangle(x, y, width, height), texture));
        }

        

        
    }
}
