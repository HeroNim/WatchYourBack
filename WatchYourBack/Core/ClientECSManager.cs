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
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, EntityCommands> changedEntities;
        private List<ESystem> systems;
        private List<Entity> removal;
        private LevelInfo levelInfo;
        private UIInfo ui;
        private InputSystem input;
        private int currentID;
        private double drawTime;

        private bool playing;

        public ClientECSManager()
        {
            ui = null;
            currentID = 0;
            drawTime = 0;
            systems = new List<ESystem>();
            activeEntities = new Dictionary<int, Entity>();
            changedEntities = new Dictionary<int, EntityCommands>();
            removal = new List<Entity>();
        }

        public bool Playing { get { return playing; } set { playing = value; } }

        public void Initialize()
        {
            foreach(ESystem system in systems)
            {
                foreach(ESystem other in systems)
                {
                    if(other != system)
                    {
                        other.inputFired += new EventHandler(system.EventListener);
                    }
                }
                system.initialize(this);                
            }
        }

        

        public void addUI(UIInfo info)
        {
            ui = info;
            foreach (Entity e in ui.UIElements)
            {
                addEntity(e);
            }
        }

        public UIInfo UI
        {
            get { return ui; }
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
            entity.ClientID = assignID();
            entity.initialize();
            activeEntities.Add(entity.ClientID, entity);
            addChangedEntities(entity, EntityCommands.Add);
        }

        public int assignID()
        {
            currentID = 0;
            while (activeEntities.Keys.Contains(currentID))
                currentID++;
            return currentID;

        }

        public void removeEntity(Entity entity)
        {
            if (activeEntities.Values.Contains(entity) && !removal.Contains(entity))
            {
                removal.Add(entity);
                addChangedEntities(entity, EntityCommands.Remove);
            }
        }
        
        public void addInput(InputSystem input)
        {
            this.input = input;
        }

        public LevelInfo LevelInfo
        {
            get { return levelInfo; }
            set { levelInfo = value; }
        }

       

        public Dictionary<int, Entity> Entities
        {
            get { return activeEntities; }
        }

        public Dictionary<int, EntityCommands> ChangedEntities
        {
            get { return changedEntities; }
        }

        public List<ESystem> Systems
        {
            get { return systems; }
        }

        public void addChangedEntities(Entity e, EntityCommands c)
        {
            if (!changedEntities.Keys.Contains(e.ClientID))
                changedEntities.Add(e.ClientID, c);
            else if (changedEntities.Keys.Contains(e.ClientID) && changedEntities[e.ClientID] != EntityCommands.Remove && c == EntityCommands.Remove)
                changedEntities[e.ClientID] = EntityCommands.Remove;
        }
             
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
                activeEntities.Remove(entity.ClientID);
            removal.Clear();                 
            foreach (Entity entity in activeEntities.Values)
                if (!entity.IsActive)
                    removal.Add(entity);
            foreach (Entity entity in removal)
                activeEntities.Remove(entity.ClientID);
            removal.Clear();
        }      

        
        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Entity entity in activeEntities.Values)
            {
                if (entity.hasComponent(Masks.Graphics))
                {
                    GraphicsComponent graphics = (GraphicsComponent)entity.Components[Masks.Graphics];
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
                                    spriteBatch.Draw(sprite.Sprite, new Rectangle((int)point.X, (int)point.Y, 3, 3), Color.Blue);
                                }
                                sprite.DebugPoints.Clear();
                            }
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
