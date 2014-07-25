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
        //public static Entity createPlayer(Allegiance player)
        //{
        //    Entity e = new Entity(
        //        new PlayerInfoComponent(player),
        //    new AllegianceComponent(Allegiance.PLAYER_1));
        //    e.Type = ENTITIES.AVATAR;
        //    return e;
        //}


        public static Entity createAvatar(PlayerInfoComponent info, Rectangle rect, Allegiance player, Weapons weaponType, Texture2D texture, bool hasGraphics)
        {
            //Rectangle pos = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            Vector2 offset = new Vector2(rect.Width / 2, rect.Height / 2);
            Entity e = new Entity(false,
            info,
            new AllegianceComponent(player),
            new WielderComponent(weaponType),
            new TransformComponent(rect.X, rect.Y, rect.Width, rect.Height),
            new RectangleColliderComponent(rect),
            new PlayerHitboxComponent(rect, 10, -Vector2.UnitY),
            new CircleColliderComponent(new Vector2(rect.Center.X, rect.Center.Y), rect.Width/2, false),
            new VelocityComponent(0, 0),
            new StatusComponent(),
            new AvatarInputComponent());
            if (hasGraphics)
                e.addComponent(new GraphicsComponent(rect, texture, texture.Bounds, 0, new Vector2(texture.Width / 2, texture.Height / 2), offset, 0.9f, "Avatar"));
            e.Type = ENTITIES.AVATAR;
            return e;
        }

        public static Entity createGraphics(Rectangle rect, float rotation, Vector2 rotationOrigin, Vector2 rotationOffset, int ID, Texture2D texture, Rectangle sourceRectangle, ENTITIES type, float layer)
        { 
            Entity e = new Entity(
            new GraphicsComponent(rect, texture, sourceRectangle, rotation, rotationOrigin, rotationOffset, layer, type.ToString()));
            e.ID = ID;
            e.Type = type;
            return e;
        }

        public static Entity createGraphics(Rectangle rect, float rotation, Vector2 rotationOrigin, Vector2 rotationOffset, int ID, Texture2D texture, int[,] textureIndex, ENTITIES type, float layer)
        {
            Entity e = new Entity();
            GraphicsComponent g = new GraphicsComponent();
            g.Sprites.Add("TopLeft", new GraphicsInfo(new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * textureIndex[0, 0], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 1));
                g.Sprites.Add("TopRight", new GraphicsInfo(new Rectangle(rect.X + rect.Width / 2, rect.Y, rect.Width / 2, rect.Height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * textureIndex[0, 1], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 1));
                g.Sprites.Add("BottomLeft", new GraphicsInfo(new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * textureIndex[1, 0], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 1));
                g.Sprites.Add("BottomRight", new GraphicsInfo(new Rectangle(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * textureIndex[1, 1], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 1));
                e.addComponent(g);
            e.ID = ID;
            e.Type = type;
            return e;
        }

        
        //Creates a weapon at a point on an entity, while taking the holder's velocity component to allow it to 'stick' to the holder
        public static Entity createSword(Entity wielder, Allegiance wielderAllegiance, TransformComponent anchorTransform, float rotationAngle, VelocityComponent anchorMovement, Texture2D texture, bool hasGraphics)
        {
            Vector2 point = HelperFunctions.pointOnCircle(anchorTransform.Radius, rotationAngle, anchorTransform.Center);
            Vector2 rotation = Vector2.Transform(point - anchorTransform.Center, Matrix.CreateRotationZ((float)(Math.PI/4))) + anchorTransform.Center;
            point = rotation;
            Entity e = new Entity(true,
            new AllegianceComponent(wielderAllegiance),
            new TransformComponent(point, (int)SWORD.WIDTH, (int)SWORD.RANGE, rotationAngle),
            new WeaponComponent(wielder, MathHelper.ToRadians((float)SWORD.ARC), true),
            new VelocityComponent(anchorMovement.X, anchorMovement.Y, -(float)SWORD.SPEED),
            new LineColliderComponent(point, new Vector2(point.X, point.Y - (float)SWORD.RANGE), rotationAngle));

            if (hasGraphics)
                e.addComponent(new GraphicsComponent(new Rectangle((int)point.X, (int)point.Y, (int)SWORD.WIDTH, (int)SWORD.RANGE), texture, rotationAngle, new Vector2(texture.Width / 2, texture.Height), 0, "Sword"));
            e.Type = ENTITIES.SWORD;
            return e;
            
        }


        public static Entity createThrown(Allegiance wielderAllegiance, float xOrigin, float yOrigin, Vector2 rotationVector, float rotationAngle, Texture2D texture, bool hasGraphics)
        {

            Entity e = new Entity(true,
            new AllegianceComponent(wielderAllegiance),
            new TransformComponent(xOrigin, yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS, rotationAngle),
            new WeaponComponent(),
            new VelocityComponent(rotationVector.X * (float)THROWN.SPEED, rotationVector.Y * (float)THROWN.SPEED),
            new RectangleColliderComponent(new Rectangle((int)xOrigin, (int)yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS)));
            if (hasGraphics)
                e.addComponent(new GraphicsComponent(new Rectangle((int)xOrigin, (int)yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS), texture, rotationAngle, new Vector2(texture.Width / 2, texture.Height), 1, "Thrown"));
            e.Type = ENTITIES.THROWN;
            return e;

        }

        public static Entity createButton(int x, int y, int width, int height, Inputs type, Texture2D texture, string text, SpriteFont font)
        {

                return new Entity(false,
                new ButtonComponent(type),
                new TransformComponent(x, y, width, height),
                new RectangleColliderComponent(new Rectangle(x, y, width, height)),
                new GraphicsComponent(new Rectangle(x, y, width, height), texture, text, font, Color.Blue, 1, "Button"));
                
        }

        public static Entity createWall(int x, int y, int width, int height, Texture2D texture, int[,] atlasIndex, bool hasGraphics)
        {
            Entity e = new Entity(false,
                new TileComponent(TileType.WALL, atlasIndex),
                new TransformComponent(x, y, width, height),
                new RectangleColliderComponent(new Rectangle(x, y, width, height)));
            if (hasGraphics)
            {
                GraphicsComponent g = new GraphicsComponent();
                g.Sprites.Add("TopLeft", new GraphicsInfo(new Rectangle(x, y, width / 2, height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * atlasIndex[0, 0], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 1));
                g.Sprites.Add("TopRight", new GraphicsInfo(new Rectangle(x + width / 2, y, width / 2, height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * atlasIndex[0, 1], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 1));
                g.Sprites.Add("BottomLeft", new GraphicsInfo(new Rectangle(x, y + height / 2, width / 2, height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * atlasIndex[1, 0], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 1));
                g.Sprites.Add("BottomRight", new GraphicsInfo(new Rectangle(x + width / 2, y + height / 2, width / 2, height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * atlasIndex[1, 1], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 1));
                e.addComponent(g);
            }
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
