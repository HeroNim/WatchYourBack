using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchYourBackLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace WatchYourBackLibrary
{    
    /// <summary>
    /// A factory to create all the entities of the game. Each method in the factory is a template for an entity in the game; the entities can be
    /// created using these methods by specifying their parameters. Also contains a method for creating a graphical representation of an entity,
    /// without creating the entity itself.
    /// </summary>
    public static class EFactory
    {
        public static ContentManager content;

        /// <summary>
        /// Creates an avatar for the player, which also contains the player's info, such as their score.
        /// </summary>
        /// <param name="info">The information for the player</param>
        /// <param name="rect">The location and size of the avatar</param>
        /// <param name="player">The player's allegiance, used to determine what should collide with what</param>
        /// <param name="weaponType">The identity of the avatar's primary weapon</param>
        /// <param name="hasGraphics">A flag identifying whether the entity should have graphics or not</param>
        /// <returns>An avatar entity</returns>
        public static Entity createAvatar(PlayerInfoComponent info, Rectangle rect, Allegiance player, Weapons weaponType, bool hasGraphics)
        {
            Vector2 offset = new Vector2(rect.Width / 2, rect.Height / 2);
            TransformComponent transform = new TransformComponent(rect);
            Entity e = new Entity(false,
            info,
            transform,
            new VelocityComponent(0, 0),
            new AllegianceComponent(player),
            new RectangleColliderComponent(rect, transform),
            new CircleColliderComponent(new Vector2(rect.Center.X, rect.Center.Y), rect.Width/2, transform),
            new PlayerHitboxComponent(PlayerHitboxComponent.setAvatarHitbox(rect, 10, -Vector2.UnitY)),
            new WielderComponent(weaponType),
            new StatusComponent(),
            new VisionComponent(60, new Vector2(rect.Center.X, rect.Center.Y), rect.Width),
            new AvatarInputComponent());
            if (hasGraphics)
            {
                Texture2D myTexture = content.Load<Texture2D>("PlayerTexture");
                GraphicsComponent graphics = new GraphicsComponent(GraphicsLayer.Foreground, new Vector2(myTexture.Width / 2, myTexture.Height / 2));
                graphics.AddSprite("Avatar", new SpriteGraphicsInfo(rect, myTexture, myTexture.Bounds, offset, 0.5f));               
                e.addComponent(graphics);
            }
            e.Type = Entities.Avatar;
            return e;
        }
     
        /// <summary>
        /// Creates a sword. Takes various parameters of the wielder that allows it to accurately move with the wielder.
        /// </summary>
        /// <param name="wielder">The entity which is wielding the sword</param>
        /// <param name="wielderAllegiance">The allegiance of the wielder</param>
        /// <param name="anchorTransform">The position of the wielder.</param>
        /// <param name="rotationAngle">The initial rotation of the sword</param>
        /// /// <param name="rotationAngle">The initial position of the sword</param>
        /// <param name="hasGraphics">A flag identifying whether the enttiy should have graphics or not</param>
        /// <returns>A sword entity anchored at the wielder</returns>
        public static Entity createSword(Entity wielder, Allegiance wielderAllegiance, TransformComponent anchorTransform, float rotationAngle, float positionAngle, bool hasGraphics)
        {
            SoundEffectComponent soundC = new SoundEffectComponent();
            soundC.AddSound(SoundTriggers.Initialize, "Sounds/SFX/SwordSwing");
            soundC.AddSound(SoundTriggers.Destroy, "Sounds/SFX/ImpactSound");
            
            Vector2 point = Circle.PointOnCircle(anchorTransform.Radius, positionAngle, anchorTransform.Center);

            Vector2 collider1 = new Vector2(point.X, point.Y + 10);
            Vector2 collider2 = new Vector2(point.X, point.Y - (float)SWORD.RANGE);
            collider1 = Vector2.Transform(collider1 - point, Matrix.CreateRotationZ(rotationAngle)) + point;
            collider2 = Vector2.Transform(collider2 - point, Matrix.CreateRotationZ(rotationAngle)) + point;

            TransformComponent transform = new TransformComponent(point, (int)SWORD.WIDTH, (int)SWORD.RANGE, rotationAngle);
            transform.Parent = wielder;
            transform.RotationPoint = anchorTransform.Center;

            Entity e = new Entity(true,
            transform,
            new VelocityComponent(0, 0, -(float)SWORD.SPEED),
            new AllegianceComponent(wielderAllegiance),
            new WeaponComponent(wielder, MathHelper.ToRadians((float)SWORD.ARC), true),
            new LineColliderComponent(collider1, collider2),
            soundC);

            if (hasGraphics)
            {
                Texture2D myTexture = content.Load<Texture2D>("SwordTexture");
                GraphicsComponent graphics = new GraphicsComponent(GraphicsLayer.Foreground, new Vector2(myTexture.Width / 2, myTexture.Height), rotationAngle);
                graphics.AddSprite("Sword", new SpriteGraphicsInfo(new Rectangle((int)point.X, (int)point.Y, (int)SWORD.WIDTH, (int)SWORD.RANGE), myTexture, 0.4f));
                e.addComponent(graphics);
            }
            e.Type = Entities.Sword;
            return e;           
        }

        /// <summary>
        /// Creates a thrown weapon.
        /// </summary>
        /// <param name="wielderAllegiance">The allegiance of the wielder</param>
        /// <param name="xOrigin">The x-coordinate of the weapon</param>
        /// <param name="yOrigin">The y-coordinate of the weapon</param>
        /// <param name="rotationVector">A unit vector representing the direction of the weapon's velocity</param>
        /// <param name="rotationAngle">The angle of the weapon</param>
        /// <param name="hasGraphics">A flag identifying whether the enttiy should have graphics or not</param>
        /// <returns>A thrown weapon entity</returns>
        public static Entity createThrown(Allegiance wielderAllegiance, float xOrigin, float yOrigin, Vector2 rotationVector, float rotationAngle, bool hasGraphics)
        {
            SoundEffectComponent soundC = new SoundEffectComponent();
            soundC.AddSound(SoundTriggers.Initialize, "Sounds/SFX/ThrowSound");
            soundC.AddSound(SoundTriggers.Destroy, "Sounds/SFX/ImpactSound");

            TransformComponent transform = new TransformComponent(xOrigin, yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS, rotationAngle);
            Entity e = new Entity(true,
            transform,
            new VelocityComponent(rotationVector.X * (float)THROWN.SPEED, rotationVector.Y * (float)THROWN.SPEED),
            new AllegianceComponent(wielderAllegiance),
            new RectangleColliderComponent(transform.Body, transform),
            new WeaponComponent(),
            soundC);
            if (hasGraphics)
            {
                Texture2D myTexture = content.Load<Texture2D>("ThrownTexture");
                GraphicsComponent graphics = new GraphicsComponent(GraphicsLayer.Foreground, new Vector2(myTexture.Width / 2, myTexture.Height), rotationAngle);
                graphics.AddSprite("Thrown", new SpriteGraphicsInfo(new Rectangle((int)xOrigin, (int)yOrigin, (int)THROWN.RADIUS, (int)THROWN.RADIUS), myTexture, 0.7f));
                e.addComponent(graphics);
            }
            e.Type = Entities.Thrown;
            return e;
        }

        /// <summary>
        /// Creates a button for a menu, containing both it's graphical representation as well as it's effect when clicked.
        /// </summary>
        /// <param name="x">The x-coordinate of the button</param>
        /// <param name="y">The y-coordinate of the button</param>
        /// <param name="width">The width of the button</param>
        /// <param name="height">The height of the button</param>
        /// <param name="type">The role of the button</param>
        /// <param name="text">The text of the button</param>
        /// <returns>A button entity</returns>
        public static Entity createButton(int x, int y, int width, int height, Inputs type, string text, SpriteFont font)
        {
            SoundEffectComponent soundC = new SoundEffectComponent();            
            soundC.AddSound(SoundTriggers.Action, "Sounds/SFX/ButtonClick");

            Texture2D frame = content.Load<Texture2D>("Buttons/ButtonFrame");
            Texture2D clickedFrame = content.Load<Texture2D>("Buttons/ButtonFrameClicked");         
            x -= frame.Width / 2;
            y -= frame.Height / 2;
            Rectangle bounds = new Rectangle(x, y, width, height);

            TransformComponent transform = new TransformComponent(x, y, width, height);
            GraphicsComponent g = new GraphicsComponent(GraphicsLayer.Foreground, new Vector2(frame.Bounds.X, frame.Bounds.Y));
            g.AddSprite("Frame", new SpriteGraphicsInfo(bounds, frame, 0.95f));
            g.AddSprite("ClickedFrame", new SpriteGraphicsInfo(bounds, clickedFrame, 0.96f));
            g.AddSprite("Text", new SpriteGraphicsInfo(bounds, text, font, Color.White, 0.5f));
           
            return new Entity(false,
            transform,
            new AllegianceComponent(Allegiance.Neutral),
            new ButtonComponent(type),
            new RectangleColliderComponent(transform.Body, transform),
            g,
            soundC);
        }

        /// <summary>
        /// Creates a wall segment.
        /// </summary>
        /// <param name="x">The x-coordinate of the wall</param>
        /// <param name="y">The y-coordinate of the wall</param>
        /// <param name="width">The width of the wall</param>
        /// <param name="height">The height of the wall</param>
        /// <param name="atlasIndex">The index of the texture atlas to be drawn</param>
        /// /// <param name="hasGraphics">A flag identifying whether the entity should have graphics or not</param>
        /// <returns>A wall segment entity</returns>
        public static Entity createWall(int x, int y, int width, int height, int[,] atlasIndex, bool hasGraphics)
        {
            TransformComponent transform = new TransformComponent(x, y, width, height);
            Entity e = new Entity(false,
                transform,
                new TileComponent(TileType.WALL, atlasIndex),
                new AllegianceComponent(Allegiance.Neutral),
                new RectangleColliderComponent(transform.Body, transform),
                new VisionBlockComponent(transform.Body));
            if (hasGraphics)
            {
                Texture2D myTexture = content.Load<Texture2D>("TileTextures/WallTextureAtlas");
                GraphicsComponent graphics = new GraphicsComponent(GraphicsLayer.Background, new Vector2(myTexture.Bounds.X, myTexture.Bounds.Y));
                graphics.AddSprite("TopLeft", new SpriteGraphicsInfo(new Rectangle(x, y, width / 2, height / 2), myTexture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * atlasIndex[0, 0], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 0.9f));
                graphics.AddSprite("TopRight", new SpriteGraphicsInfo(new Rectangle(x + width / 2, y, width / 2, height / 2), myTexture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * atlasIndex[0, 1], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 0.9f));
                graphics.AddSprite("BottomLeft", new SpriteGraphicsInfo(new Rectangle(x, y + height / 2, width / 2, height / 2), myTexture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * atlasIndex[1, 0], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 0.9f));
                graphics.AddSprite("BottomRight", new SpriteGraphicsInfo(new Rectangle(x + width / 2, y + height / 2, width / 2, height / 2), myTexture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * atlasIndex[1, 1], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 0.9f));
                e.addComponent(graphics);
            }
            e.Type = Entities.Wall;
            return e;
        }

        /// <summary>
        /// Creates a spawn point. Has no graphical component.
        /// </summary>
        /// <param name="x">The x-coordinate of the spawn</param>
        /// <param name="y">The y-coordinate of the spawn</param>
        /// <param name="width">The width of the spawn</param>
        /// <param name="height">The height of the spawn</param>
        /// <returns>A spawn entity</returns>
        public static Entity createSpawn(int x, int y, int width, int height)
        {
            Entity e = new Entity(
                new TransformComponent(x, y, width, height),
                new AllegianceComponent(Allegiance.Neutral),
                new TileComponent(TileType.SPAWN));
            e.Drawable = false;
            return e;
        }

        /// <summary>
        /// Creates a simple text display for showing values such as scores or times.
        /// </summary>
        /// <param name="rect">The location and size of the display</param>
        /// <returns>A display entity</returns>
        public static Entity createDisplay(Rectangle rect, SpriteFont font)
        {
            GraphicsComponent graphics = new GraphicsComponent(GraphicsLayer.Foreground, rect.Center);
            graphics.AddSprite("Display", new SpriteGraphicsInfo(rect, "", font, Color.Red, 0));
            Entity e = new Entity(false,
                new TransformComponent(rect),
                new AllegianceComponent(Allegiance.Neutral),
                graphics);
            return e;
        }

        /// <summary>
        /// Creates a graphical representation of an entity
        /// </summary>
        /// <param name="rect">The location and size of the entity</param>
        /// <param name="rotation">The rotation of the entity</param>
        /// <param name="rotationOrigin">The point the entity rotates around</param>
        /// <param name="rotationOffset">Changes the origin of the entity</param>
        /// <param name="ID">The unique ID of the entity</param>
        /// <param name="sourceRectangle">The area of the texture to be drawn</param>
        /// <param name="type">The type of entity</param>
        /// <param name="layer">The layer to be drawn on</param>
        /// <returns>An entity containing only a graphics component</returns>
        public static Entity createGraphics(Rectangle rect, float rotation, int ID, Entities type, GraphicsLayer graphicsLayer, int[,] textureIndex = null, Polygon poly = null)
        {
            float layer = 0;
            Vector2 rotationOrigin = Vector2.Zero;
            Vector2 rotationOffset = Vector2.Zero;
            Texture2D texture = null;
            Rectangle sourceRectangle = Rectangle.Empty;
            Entity e;
            GraphicsComponent graphics;

            switch (type)
            {
                case Entities.Avatar:
                    texture = content.Load<Texture2D>("PlayerTexture");
                    layer = 0.5f;
                    rotationOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                    rotationOffset = new Vector2(rect.Width / 2, rect.Height / 2);
                    sourceRectangle = texture.Bounds;
                    break;
                case Entities.Sword:
                    texture = content.Load<Texture2D>("SwordTexture");
                    layer = 0.4f;
                    rotationOrigin = new Vector2(texture.Width / 2, texture.Height);
                    rotationOffset = Vector2.Zero;
                    sourceRectangle = texture.Bounds;
                    break;
                case Entities.Thrown:
                    texture = content.Load<Texture2D>("ThrownTexture");
                    layer = 0.2f;
                    rotationOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                    rotationOffset = Vector2.Zero;
                    
                    sourceRectangle = texture.Bounds;
                    break;
                case Entities.Wall:
                    texture = content.Load<Texture2D>("TileTextures/WallTextureAtlas");
                    e = new Entity();
                    graphics = new GraphicsComponent(graphicsLayer, rotationOrigin, rotation);
                    graphics.AddSprite("TopLeft", new SpriteGraphicsInfo(new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * textureIndex[0, 0], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 0.9f));
                    graphics.AddSprite("TopRight", new SpriteGraphicsInfo(new Rectangle(rect.X + rect.Width / 2, rect.Y, rect.Width / 2, rect.Height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * textureIndex[0, 1], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 0.9f));
                    graphics.AddSprite("BottomLeft", new SpriteGraphicsInfo(new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * textureIndex[1, 0], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 0.9f));
                    graphics.AddSprite("BottomRight", new SpriteGraphicsInfo(new Rectangle(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2), texture, new Rectangle((int)LevelDimensions.X_SCALE / 2 * textureIndex[1, 1], 0, (int)LevelDimensions.X_SCALE / 2, (int)LevelDimensions.Y_SCALE / 2), 0.9f));
                    e.addComponent(graphics);
                    e.ServerID = ID;
                    e.Type = type;
                    return e;                
            }
            graphics = new GraphicsComponent(graphicsLayer, rotationOrigin, rotation);
            graphics.AddSprite(type.ToString(), new SpriteGraphicsInfo(rect, texture, sourceRectangle, rotationOffset, layer));
            e = new Entity(graphics);
            if (poly != null)
                graphics.AddPolygon("Vision", poly);
            e.ServerID = ID;
            e.Type = type;
            return e;
        }
    }
}
