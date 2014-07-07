using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace WatchYourBackLibrary
{
    /*
     * A factory to create all the entities of the game. Can hold templates so that objects can be created by only specifying their location.
     */
    public static class EFactory
    {

        public static Entity createAvatar(Rectangle rect, Allegiance player, Weapons weaponType, Texture2D texture, bool hasGraphics)
        {
            Entity e = new Entity(
            new AllegianceComponent(player),
            new WielderComponent(weaponType),
            new TransformComponent(rect.X, rect.Y, rect.Width, rect.Height),
            new ColliderComponent(rect, false),
            new VelocityComponent(0, 0),
            new AvatarInputComponent());
            if (hasGraphics)
                e.addComponent(new GraphicsComponent(rect, texture));
            e.Type = ENTITIES.AVATAR;
            return e;
        }

        public static Entity createGraphics(Rectangle rect, float rotation, int ID, Texture2D texture, ENTITIES type)
        { 
            Entity e = new Entity(
            new GraphicsComponent(rect, texture, rotation, new Vector2(texture.Width/2, texture.Height)));
            e.ID = ID;
            e.Type = type;
            return e;
        }

        
        //Creates a weapon at a point on an entity, while taking the holder's velocity component to allow it to 'stick' to the holder
        public static Entity createSword(Entity wielder, Allegiance wielderAllegiance, float xOrigin, float yOrigin, float rotationAngle, VelocityComponent anchorMovement, Texture2D texture, bool hasGraphics)
        {
            Entity e = new Entity(
            new AllegianceComponent(wielderAllegiance),
            new TransformComponent(xOrigin, yOrigin, (int)SWORD.WIDTH, (int)SWORD.RANGE, rotationAngle),
            new WeaponComponent(wielder, MathHelper.ToRadians((float)SWORD.ARC)),
            new VelocityComponent(anchorMovement.X, anchorMovement.Y, -(float)SWORD.SPEED),
            new LineColliderComponent(new Vector2(xOrigin + (float)SWORD.WIDTH / 2, yOrigin), new Vector2(xOrigin + (float)SWORD.WIDTH / 2 / 2, yOrigin - (float)SWORD.RANGE), rotationAngle));
            if (hasGraphics)
                e.addComponent(new GraphicsComponent(new Rectangle((int)xOrigin, (int)yOrigin, (int)SWORD.WIDTH, (int)SWORD.RANGE), texture, rotationAngle, new Vector2(texture.Width / 2, texture.Height)));
            e.Type = ENTITIES.SWORD;
            return e;
            
        }


        public static Entity createThrown(Allegiance wielderAllegiance, float xOrigin, float yOrigin, Vector2 rotationVector, Texture2D texture, bool hasGraphics)
        {

            Entity e = new Entity(
            new AllegianceComponent(wielderAllegiance),
            new TransformComponent(xOrigin, yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS),
            new WeaponComponent(),
            new VelocityComponent(rotationVector.X * (float)THROWN.SPEED, rotationVector.Y * (float)THROWN.SPEED),
            new ColliderComponent(new Rectangle((int)xOrigin, (int)yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS), true));
            if (hasGraphics)
                e.addComponent(new GraphicsComponent(new Rectangle((int)xOrigin, (int)yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS), texture, 0, new Vector2(texture.Width / 2, texture.Height)));
            e.Type = ENTITIES.THROWN;
            return e;

        }

        public static Entity createButton(int x, int y, int width, int height, Inputs type, Texture2D texture, string text, SpriteFont font)
        {

                return new Entity(
                new ButtonComponent(type),
                new TransformComponent(x, y, width, height),
                new ColliderComponent(new Rectangle(x, y, width, height), false),
                new GraphicsComponent(new Rectangle(x, y, width, height), texture, text, font, Color.Blue));
                
        }

        public static Entity createWall(int x, int y, int width, int height, Texture2D texture, bool hasGraphics)
        {
            Entity e = new Entity(
                new TileComponent(TileType.WALL),
                new TransformComponent(x, y, width, height),
                new ColliderComponent(new Rectangle(x, y, width, height), false));
            if (hasGraphics)
                e.addComponent(new GraphicsComponent(new Rectangle(x, y, width, height), texture));
            e.Type = ENTITIES.WALL;
            return e;
        }

        public static Entity createSpawn(int x, int y, int width, int height)
        {
            Entity e = new Entity(
                new TileComponent(TileType.SPAWN),
                new TransformComponent(x, y, width, height));
            e.Drawable = false;
            return e;
        }

        

        
    }
}
