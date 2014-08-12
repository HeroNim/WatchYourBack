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

        private QuadTree<Entity> quadtree;
        private Dictionary<int, Entity> activeEntities;
        private Dictionary<int, EntityCommands> changedEntities;
        private List<ESystem> systems;
        private List<Entity> removal;
        private LevelInfo levelInfo;
        private UIInfo ui;
        private int currentID;
        private double drawTime;

        private bool playing;

        public ClientECSManager()
        {
            ui = null;
            currentID = 0;
            drawTime = 0;
            systems = new List<ESystem>();
            quadtree = new QuadTree<Entity>(0, 0, GameData.gameWidth, GameData.gameHeight, 5);
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

        public void addEntity(Entity entity)
        {
            entity.ClientID = assignID();
            entity.initialize();
            activeEntities.Add(entity.ClientID, entity);
            if (entity.hasComponent(Masks.Transform))
                quadtree.Add(entity, entity.GetComponent<TransformComponent>().Body);
            addChangedEntities(entity, EntityCommands.Add);
        }

        public void removeEntity(Entity entity)
        {
            if (activeEntities.Values.Contains(entity) && !removal.Contains(entity))
            {
                removal.Add(entity);
                addChangedEntities(entity, EntityCommands.Remove);
            }
            quadtree.Remove(entity);
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

        public int assignID()
        {
            currentID = 0;
            while (activeEntities.Keys.Contains(currentID))
                currentID++;
            return currentID;

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

        public QuadTree<Entity> QuadTree
        {
            get { return quadtree; }
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
                    Texture2D test = EFactory.content.Load<Texture2D>("PlayerTexture");
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
                                foreach (Vector2 point in graphics.DebugPoints)
                                {
                                    spriteBatch.Draw(test, new Rectangle((int)point.X, (int)point.Y, 3, 3), Color.Blue);
                                }
                                //graphics.DebugPoints.Clear();
                            }
                        }
                    }
                }
                
            }

            foreach (Entity entity in activeEntities.Values)
            {
                if (entity.hasComponent(Masks.Vision))
                {
                    VisionComponent vision = entity.GetComponent<VisionComponent>();
                    if (vision.VisionField != null)
                        DrawPolygon(vision.VisionField);
                }
            }
            
        }

        private void DrawPolygon(Polygon e)
        {
            GraphicsDevice device = ClientGameLoop.Instance.GraphicsDevice;
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
           
            BasicEffect effect = new BasicEffect(device);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = halfPixelOffset * projection;            

            VertexBuffer vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), e.VertexList.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(e.VertexList);
            IndexBuffer indexBuffer = new IndexBuffer(device, typeof(short), e.IndexList.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(e.IndexList);

            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, e.VertexList.Length, 0, e.VertexList.Length - 2);
            }


        }

        public LevelInfo LevelInfo
        {
            get { return levelInfo; }
            set { levelInfo = value; }
        }

        public bool hasGraphics()
        {
            return true;
        }           
    }
}
