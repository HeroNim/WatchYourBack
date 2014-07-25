using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using WatchYourBackLibrary;

namespace WatchYourBack
{
    //Manages the systems in the game. Is responsible for initializing, updating, and removing systems as needed.
    public class ClientECSManager : IECSManager
    {
        private List<ESystem> systems;
        private LevelComponent levelInfo;
        private Dictionary<int, Entity> inactiveEntities;
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, COMMANDS> changedEntities;
        private List<Entity> removal;
        private ContentManager content;
        private InputSystem input;
        private int id;
        private double drawTime;

        private bool playing;

        

        public ClientECSManager()
        {
            id = 0;
            drawTime = 0;
            systems = new List<ESystem>();
            activeEntities = new Dictionary<int, Entity>();
            changedEntities = new Dictionary<int, COMMANDS>();
            removal = new List<Entity>();
            inactiveEntities = new Dictionary<int, Entity>();
        }

        public bool Playing { get { return playing; } set { playing = value; } }

        public void addContent(ContentManager content)
        {
            this.content = content;
        }

        public void addSystem(ESystem system)
        {
            systems.Add(system);
            system.initialize(this);
            systems = systems.OrderBy(o => o.Priority).ToList();
        }

        public void removeSystem(ESystem system)
        {
            systems.Remove(system);
        }

        public void addEntity(Entity entity)
        {
            
            entity.ID = id;
            id++;
            entity.initialize();
            activeEntities.Add(entity.ID, entity);
        }

        public void removeEntity(Entity entity)
        {
            if (activeEntities.Values.Contains(entity) && !removal.Contains(entity))
            {
                removal.Add(entity);
                addChangedEntities(entity, COMMANDS.REMOVE);
            }
        }
        
        public void addInput(InputSystem input)
        {
            this.input = input;
        }

        public LevelComponent LevelInfo
        {
            get { return levelInfo; }
            set { levelInfo = value; }
        }

        public Dictionary<int, Entity> Entities
        {
            get { return inactiveEntities; } 
        }

        public Dictionary<int, Entity> ActiveEntities
        {
            get { return activeEntities; }
        }

        public Dictionary<int, COMMANDS> ChangedEntities
        {
            get { return changedEntities; }
        }

        public void addChangedEntities(Entity e, COMMANDS c)
        {
            if (!changedEntities.Keys.Contains(e.ID))
                changedEntities.Add(e.ID, c);
            else if (changedEntities.Keys.Contains(e.ID) && changedEntities[e.ID] != COMMANDS.REMOVE && c == COMMANDS.REMOVE)
                changedEntities[e.ID] = COMMANDS.REMOVE;
        }

        public void clearEntities()
        {
            foreach(Entity entity in removal)
            {
                inactiveEntities.Remove(entity.ID);
                activeEntities.Remove(entity.ID);
            }
            removal.Clear();
        }

        /*
         * Updates the entity lists of the manager, moving active/inactive entities to their proper lists. Any systems that run
         * during the update loop are then updated.
         */
        

        public void update(TimeSpan gameTime)
        {
            //Update the systems
            foreach (ESystem system in systems)
            {
                if (system.Loop == true)
                    system.updateEntities(gameTime);
            }
            RemoveAll();
        }

        public double[] Accumulator { get; set; }

        public double DrawTime
        {
            get { return drawTime; }
            set { drawTime = value; }
        }

        public void RemoveAll()
        {
            clearEntities();
            foreach (Entity entity in inactiveEntities.Values)
                if (entity.IsActive)
                {
                    activeEntities.Add(entity.ID, entity);
                    removal.Add(entity);
                }

            foreach (Entity entity in removal)
                inactiveEntities.Remove(entity.ID);
            removal.Clear();

            foreach (Entity entity in activeEntities.Values)
                if (!entity.IsActive)
                {
                    inactiveEntities.Add(entity.ID, entity);
                    removal.Add(entity);
                }

            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ID);
            removal.Clear();
        }

        /*
         * Has the sprite batch draw all the entities that have a graphics component 
         */
        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Entity entity in activeEntities.Values)
            {
                if (entity.hasComponent(Masks.GRAPHICS))
                {
                    
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.GRAPHICS];
                    foreach (GraphicsInfo sprite in graphics.Sprites.Values)
                    {
                        spriteBatch.Draw(sprite.Sprite, sprite.Body, sprite.SourceRectangle,
                                sprite.SpriteColor, sprite.RotationAngle, sprite.RotationOrigin, SpriteEffects.None, sprite.Layer);

                        if (sprite.HasText)
                            spriteBatch.DrawString(sprite.Font, sprite.Text, new Vector2(sprite.X, sprite.Y), sprite.FontColor, 0, sprite.RotationOrigin, 1, SpriteEffects.None, 0);
                        foreach (Vector2 point in sprite.DebugPoints)
                        {
                            spriteBatch.Draw(sprite.Sprite, new Rectangle((int)point.X, (int)point.Y, 3, 3), Color.Black);
                        }
                        sprite.DebugPoints.Clear();
                    }
                }
            }
        }
    
        public bool hasGraphics()
        {
            return true;
        }

        public Texture2D getTexture(string fileName)
        {
            return content.Load<Texture2D>(fileName);
        }

        public InputSystem Input
        {
            get { return input; }
        }

        
    }
}
