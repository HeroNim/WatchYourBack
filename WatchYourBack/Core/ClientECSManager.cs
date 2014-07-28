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
    
    
    /// <summary>
    /// The client side version of the ECS Manager. Manages the entities and systems in the game; it is responsible for initializing, updating, and removing them as needed.
    /// It is also responsible for drawing any graphical elements that entities might have via XNA's built-in draw functions.
    /// </summary>
    public class ClientECSManager : IECSManager
    {
        private UIInfo ui;
        private List<ESystem> systems;
        private LevelComponent levelInfo;
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, Entity> uiEntities;
        private Dictionary<int, COMMANDS> changedEntities;
        private List<Entity> removal;
        private InputSystem input;
        private int id;
        private double drawTime;

        private bool playing;

        

        public ClientECSManager()
        {
            ui = null;
            id = 0;
            drawTime = 0;
            systems = new List<ESystem>();
            uiEntities = new Dictionary<int, Entity>();
            activeEntities = new Dictionary<int, Entity>();
            changedEntities = new Dictionary<int, COMMANDS>();
            removal = new List<Entity>();
        }

        public bool Playing { get { return playing; } set { playing = value; } }

        public void addUI(UIInfo info)
        {
            ui = info;
            foreach (Entity e in ui.UIElements)
            {
                if (e.ID == -1)
                {
                    e.ID = id;
                    id++;
                }
                e.initialize();
                uiEntities.Add(e.ID, e);
            }
        }

        public UIInfo UI
        {
            get { return ui; }
        }

        public Dictionary<int, Entity> UIEntities
        {
            get { return uiEntities; }
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
            if (entity.ID == -1)
            {
                entity.ID = id;
                id++;
            }
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

       

        /*
         * Updates the entity lists of the manager, moving active/inactive entities to their proper lists. Any systems that run
         * during the update loop are then updated.
         */
        

        public void update(TimeSpan gameTime)
        {
            //Update the systems
            foreach (ESystem system in systems)
                if (system.Loop == true)
                    system.updateEntities(gameTime);
            
            RemoveAll();
        }

        public double DrawTime
        {
            get { return drawTime; }
            set { drawTime = value; }
        }

        public void RemoveAll()
        {
            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ID);
            removal.Clear();                 
            foreach (Entity entity in activeEntities.Values)
                if (!entity.IsActive)
                    removal.Add(entity);
            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ID);
            removal.Clear();
        }      

        
        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Entity entity in activeEntities.Values)
            {
                if (entity.hasComponent(Masks.GRAPHICS))
                {
                    
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.GRAPHICS];
                    foreach (GraphicsInfo sprite in graphics.Sprites.Values)
                    {
                        if (sprite.Visible == true)
                        {
                            if (sprite.HasText)
                                spriteBatch.DrawString(sprite.Font, sprite.Text, new Vector2(sprite.X, sprite.Y), sprite.FontColor, 0, sprite.RotationOrigin, 1, SpriteEffects.None, 0);
                            else
                            {
                                spriteBatch.Draw(sprite.Sprite, sprite.Body, sprite.SourceRectangle,
                                        sprite.SpriteColor, sprite.RotationAngle, sprite.RotationOrigin, SpriteEffects.None, sprite.Layer);
                                foreach (Vector2 point in sprite.DebugPoints)
                                {
                                    spriteBatch.Draw(sprite.Sprite, new Rectangle((int)point.X, (int)point.Y, 3, 3), Color.Black);
                                }
                                sprite.DebugPoints.Clear();
                            }
                        }
                    }
                }
            }
            foreach (Entity entity in UIEntities.Values)
            {
                if (entity.hasComponent(Masks.GRAPHICS))
                {

                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.GRAPHICS];
                    foreach (GraphicsInfo sprite in graphics.Sprites.Values)
                    {
                        if (sprite.HasText)
                            spriteBatch.DrawString(sprite.Font, sprite.Text, new Vector2(sprite.X, sprite.Y), sprite.FontColor, 0, sprite.RotationOrigin, 1, SpriteEffects.None, 0);
                        else
                        {
                            spriteBatch.Draw(sprite.Sprite, sprite.Body, sprite.SourceRectangle,
                                    sprite.SpriteColor, sprite.RotationAngle, sprite.RotationOrigin, SpriteEffects.None, sprite.Layer);
                            foreach (Vector2 point in sprite.DebugPoints)
                            {
                                spriteBatch.Draw(sprite.Sprite, new Rectangle((int)point.X, (int)point.Y, 3, 3), Color.Black);
                            }
                            sprite.DebugPoints.Clear();
                        }
                    }
                }
            }
        }
    
        public bool hasGraphics()
        {
            return true;
        }
      
        public InputSystem Input
        {
            get { return input; }
        }

        
    }
}
