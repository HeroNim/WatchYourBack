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
        private Dictionary<int, Entity> inactiveEntities;
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, COMMANDS> changedEntities;
        private List<Entity> removal;
        private ContentManager content;
        private InputSystem input;
        private int id;

        

        public ClientECSManager()
        {
            id = 0;
            systems = new List<ESystem>();
            activeEntities = new Dictionary<int, Entity>();
            changedEntities = new Dictionary<int, COMMANDS>();
            removal = new List<Entity>();
            inactiveEntities = new Dictionary<int, Entity>();
        }

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
            if (activeEntities.Values.Contains(entity))
            {
                removal.Add(entity);
                addChangedEntities(entity, COMMANDS.REMOVE);
            }
        }
        
        public void addInput(InputSystem input)
        {
            this.input = input;
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
            RemoveAll();
            //Update the systems
            foreach (ESystem system in systems)
            {
                if (system.Loop == true)
                    system.updateEntities(gameTime);
            }    
        }

        public double[] Accumulator { get; set; }

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
                    if (graphics.Rotatable == true)
                        spriteBatch.Draw(graphics.Sprite, graphics.Body, new Rectangle(0,0, graphics.Sprite.Width, graphics.Sprite.Height), 
                            graphics.SpriteColor, graphics.RotationAngle, graphics.RotationOrigin, SpriteEffects.None, 1);
                    else
                        spriteBatch.Draw(graphics.Sprite, graphics.Body, graphics.SpriteColor);
                    
                    if (graphics.HasText)
                        spriteBatch.DrawString(graphics.Font, graphics.Text, new Vector2(graphics.X, graphics.Y), graphics.FontColor);
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
