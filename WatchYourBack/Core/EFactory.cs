using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

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
        
        public static Entity createAvatar(Rectangle rect, Texture2D texture, Allegiance player, Weapons weaponType)
        {

                return new Entity(
                new AllegianceComponent(player),
                new WielderComponent(weaponType),
                new TransformComponent(rect.X, rect.Y, rect.Width, rect.Height),
                new ColliderComponent(rect, false),
                new VelocityComponent(0, 0),
                new AvatarInputComponent(),
                new GraphicsComponent(rect, texture));
        }

        
        //Creates a weapon at a point on an entity, while taking the holder's velocity component to allow it to 'stick' to the holder
        public static Entity createSword(Entity wielder, Allegiance wielderAllegiance, float xOrigin, float yOrigin, float rotationAngle, VelocityComponent anchorMovement, Texture2D texture)
        {
            return new Entity(
            new AllegianceComponent(wielderAllegiance),
            new TransformComponent(xOrigin, yOrigin, rotationAngle),
            new WeaponComponent(wielder, MathHelper.ToRadians((float)SWORD.ARC)),
            new VelocityComponent(anchorMovement.X, anchorMovement.Y, -(float)SWORD.SPEED),
            new LineColliderComponent(new Vector2(xOrigin + (float)SWORD.WIDTH / 2, yOrigin), new Vector2(xOrigin + (float)SWORD.WIDTH / 2 / 2, yOrigin - (float)SWORD.RANGE), rotationAngle),
            new GraphicsComponent(new Rectangle((int)xOrigin, (int)yOrigin, (int)SWORD.WIDTH, (int)SWORD.RANGE), texture, 0.1f, new Vector2(texture.Width / 2, texture.Height)));
        }
            

        public static Entity createThrown(Allegiance wielderAllegiance, float xOrigin, float yOrigin, Vector2 rotationVector, Texture2D texture)
        {
            
                return new Entity(
                new AllegianceComponent(wielderAllegiance),
                new TransformComponent(xOrigin, yOrigin),
                new WeaponComponent(),
                new VelocityComponent(rotationVector.X * (float)THROWN.SPEED, rotationVector.Y * (float)THROWN.SPEED),
                new ColliderComponent(new Rectangle((int)xOrigin, (int)yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS), true),
                new GraphicsComponent(new Rectangle((int)xOrigin, (int)yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS), texture, 0.1f, new Vector2(texture.Width / 2, texture.Height)));
            
        }

        public static Entity createButton(int x, int y, int width, int height, Inputs type, Texture2D texture, string text, SpriteFont font)
        {

                return new Entity(
                new ButtonComponent(type),
                new TransformComponent(x, y, width, height),
                new ColliderComponent(new Rectangle(x, y, width, height), false),
                new GraphicsComponent(new Rectangle(x, y, width, height), texture, text, font, Color.Blue));
        }

        public static Entity createWall(int x, int y, int width, int height, Texture2D texture)
        {
            return new Entity(
                new TileComponent(TileType.WALL),
                new TransformComponent(x, y, width, height),
                new ColliderComponent(new Rectangle(x, y, width, height), false),
                new GraphicsComponent(new Rectangle(x, y, width, height), texture));
        }

        public static Entity createSpawn(int x, int y, int width, int height, Texture2D texture)
        {
            return new Entity(
                new TileComponent(TileType.SPAWN),
                new TransformComponent(x, y, width, height));
        }

        

        
    }
}
