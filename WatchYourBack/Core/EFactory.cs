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
                new TransformComponent(rect.X, rect.Y),
                new ColliderComponent(rect),
                new VelocityComponent(0, 0),
                new AvatarInputComponent(),
                new GraphicsComponent(rect, texture));
        }

        
        //Creates a weapon at a point on an entity, while taking the holder's velocity component to allow it to 'stick' to the holder
        public static Entity createWeapon(Entity wielder, float xOrigin, float yOrigin, Rectangle body, VelocityComponent anchorMovement, Texture2D texture)
        {
                return new Entity(
                new TransformComponent(xOrigin, yOrigin),
                new WeaponComponent(wielder),
                new VelocityComponent(anchorMovement.X, anchorMovement.Y, 0.01f),
                new LineColliderComponent(new Vector2(xOrigin + body.Width/2, yOrigin), new Vector2(xOrigin + body.Width/2, yOrigin - body.Height)),
                new GraphicsComponent(body, texture, 0.1f, new Vector2(texture.Width/2, texture.Height)));
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
