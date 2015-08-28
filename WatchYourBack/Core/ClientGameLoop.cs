#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;

using Lidgren.Network;
using WatchYourBackLibrary;
using System.Threading;
using System.Diagnostics;
#endregion

namespace WatchYourBack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ClientGameLoop : Game
    {
        public static ClientGameLoop Instance;

        Stack<World> worldStack;
        Stack<World> updateStack;
        Stack<World> drawStack;
        World activeWorld;
        ClientECSManager activeManager;
                
        World mainMenu;
        World connectMenu;
        World debug;
        World inGame;
        World pauseMenu;

        InputListener inputListener;

        Dictionary<LevelName, LevelTemplate> levels;

        SongCollection gameSongs;

        //----------------------------------------------------------------------------------------------------------
        NetClient client;
        private bool isConnected;
        private bool isPlayingOnline;
        //----------------------------------------------------------------------------------------------------------
        Matrix projection;
        Matrix halfPixelOffset;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;     
        GraphicsComponent currentGraphics;
        RasterizerState rasterizerState;
        BasicEffect effect;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        RenderTarget2D sceneTarget;
        RenderTarget2D objectTarget;
        RenderTarget2D lightMap;
        Texture2D lightMask;
        Texture2D debugTexture;
        Texture2D levelOne;
        Texture2D background;
        Texture2D lightMaskBackground;
        SpriteFont buttonFont;

        Effect makeShadows;

        public ClientGameLoop()
            : base()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = true;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 120.0f);
            
            //----------------------------------------------------------------------------------------------------------
            NetPeerConfiguration config = new NetPeerConfiguration("WatchYourBack");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();
            //----------------------------------------------------------------------------------------------------------
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            worldStack = new Stack<World>();
            updateStack = new Stack<World>();
            drawStack = new Stack<World>();
            inputListener = new InputListener(this);          
            levels = new Dictionary<LevelName, LevelTemplate>();
            gameSongs = new SongCollection();

            isConnected = false;
            isPlayingOnline = false;
            EFactory.content = Content;


            sceneTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            objectTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            lightMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            effect = new BasicEffect(GraphicsDevice);
            projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
            halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = halfPixelOffset * projection;
            rasterizerState = new RasterizerState();


            GameServices.AddService<GraphicsDevice>(GraphicsDevice);
            GameServices.AddService<ContentManager>(Content);
            base.Initialize();
            
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            debugTexture = EFactory.content.Load<Texture2D>("PlayerTexture");
            levelOne = Content.Load<Texture2D>("LevelOne");
            levels.Add(LevelName.FIRST_LEVEL, new LevelTemplate(levelOne, LevelName.FIRST_LEVEL));
            makeShadows = Content.Load<Effect>("Shaders/Effect1");
            buttonFont = Content.Load<SpriteFont>("Fonts/TestFont");
            
            background = new Texture2D(GraphicsDevice, 1, 1);
            background.SetData(new[] { Color.DarkGray });

            lightMaskBackground = new Texture2D(GraphicsDevice, 1, 1);
            lightMaskBackground.SetData(new[] { Color.Black });
            

            CreateMainMenu();
            CreateConnectMenu();
            CreateDebug();
            CreateGame();
            CreatePauseMenu();
          
            gameSongs.Add(Content.Load<Song>("Sounds/Music/HeroRemix"));

            worldStack.Push(mainMenu);
            activeWorld = mainMenu;
            inputListener.Subscribe(mainMenu);
            activeManager = (ClientECSManager)activeWorld.Manager;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputManager.SetActiveWorld(activeWorld);
            foreach (World world in worldStack)
            {             
                updateStack.Push(world);
                if (world.UpdateExclusive)
                    break;
            }

            while(updateStack.Count != 0)
            {
                if (updateStack.Peek() == connectMenu)
                    if (!isPlayingOnline)
                        Connect();
                activeManager = (ClientECSManager)updateStack.Pop().Manager;
                activeManager.Update(gameTime.ElapsedGameTime);                       
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Tries to connect to the server and start a new game
        /// </summary>
        private void Connect()
        {
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        // just connect to first server discovered
                        if (!isConnected)
                        {
                            client.Connect(msg.SenderEndpoint);
                            isConnected = true;
                            Console.WriteLine("Connected");
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        // server sent initialization command
                        int confirmation = msg.ReadInt32();
                        if (confirmation == (int)ServerCommands.SendLevels)
                        {
                            NetOutgoingMessage om = client.CreateMessage();
                            om.Write(SerializationHelper.Serialize(levels));
                            client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
                        }
                        else if (confirmation == (int)ServerCommands.Start)
                        {
                            worldStack.Push(inGame);
                            activeWorld = inGame;
                            inputListener.Unsubscribe(connectMenu);
                            inputListener.Subscribe(inGame);
                            isPlayingOnline = true;
                            Console.WriteLine("Playing");
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            foreach (World world in worldStack)
            {
                drawStack.Push(world);
                if (world.DrawExclusive)
                    break;
            }

            while(drawStack.Count != 0)
            {
                activeManager = (ClientECSManager)drawStack.Pop().Manager;                
                DrawScene();                
                activeManager.DrawTime = gameTime.ElapsedGameTime.TotalSeconds;
            }
            base.Draw(gameTime);            
        }

        private void DrawScene()
        {

            //Draw Lightmap
            GraphicsDevice.SetRenderTarget(lightMap);
            GraphicsDevice.Clear(Color.Black);
            if (activeWorld.WorldType == Worlds.InGame || activeWorld.WorldType == Worlds.Debug)
            {                            
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                foreach (Entity entity in activeManager.Entities.Values)
                {
                    if (entity.HasComponent(Masks.Graphics))
                    {
                        GraphicsComponent graphics = entity.GetComponent<GraphicsComponent>();
                        if (graphics.Polygons.ContainsKey("Vision"))
                            DrawPolygon(graphics.Polygons["Vision"]);
                    }
                }
                lightMask = (Texture2D)lightMap;
            }

           

            //Draw Scene
            GraphicsDevice.SetRenderTarget(sceneTarget);
            GraphicsDevice.Clear(Color.DarkGray);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(background, GraphicsDevice.Viewport.Bounds, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            foreach (Entity entity in activeManager.Entities.Values)
            {
                if (entity.HasComponent(Masks.Graphics))
                {
                    currentGraphics = entity.GetComponent<GraphicsComponent>();
                    if (currentGraphics.GraphicsLayer == GraphicsLayer.Background)
                        foreach (SpriteGraphicsInfo sprite in currentGraphics.Sprites.Values)
                        {
                            if (sprite.Visible == true)
                            {
                                if (sprite.HasText)
                                    HelperFunctions.DrawString(spriteBatch, sprite.Font, sprite.FontColor, sprite.Text, sprite.Body);
                                else
                                {
                                    spriteBatch.Draw(sprite.Sprite, sprite.Body, sprite.SourceRectangle,
                                            sprite.SpriteColor, sprite.RotationAngle, sprite.RotationOrigin, SpriteEffects.None, sprite.Layer);
                                    foreach (Vector2 point in currentGraphics.DebugPoints)
                                    {
                                        spriteBatch.Draw(debugTexture, new Rectangle((int)point.X, (int)point.Y, 3, 3), Color.Blue);
                                    }
                                    //graphics.DebugPoints.Clear();
                                }
                            }
                        }
                }
            }
            spriteBatch.End();

            //Draw Objects
            GraphicsDevice.SetRenderTarget(objectTarget);
            GraphicsDevice.Clear(Color.DarkGray);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(background, GraphicsDevice.Viewport.Bounds, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            foreach (Entity entity in activeManager.Entities.Values)
            {
                if (entity.HasComponent(Masks.Graphics))
                {
                    currentGraphics = entity.GetComponent<GraphicsComponent>();
                    //if (currentGraphics.GraphicsLayer == GraphicsLayer.Foreground)
                        foreach (SpriteGraphicsInfo sprite in currentGraphics.Sprites.Values)
                        {
                            if (sprite.Visible == true)
                            {
                                if (sprite.HasText)
                                    HelperFunctions.DrawString(spriteBatch, sprite.Font, sprite.FontColor, sprite.Text, sprite.Body);
                                else
                                {
                                    spriteBatch.Draw(sprite.Sprite, sprite.Body, sprite.SourceRectangle,
                                            sprite.SpriteColor, sprite.RotationAngle, sprite.RotationOrigin, SpriteEffects.None, sprite.Layer);
                                    foreach (Vector2 point in currentGraphics.DebugPoints)
                                    {
                                        spriteBatch.Draw(debugTexture, new Rectangle((int)point.X, (int)point.Y, 3, 3), Color.Blue);
                                    }
                                    //graphics.DebugPoints.Clear();
                                }
                            }
                        }
                }
            }
            spriteBatch.End();

            //Apply Lightmask
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.DarkGray);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (lightMask != null)
            {
                makeShadows.Parameters["lightMask"].SetValue(lightMask);
                makeShadows.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(sceneTarget, Vector2.Zero, Color.White);
                makeShadows.CurrentTechnique.Passes[1].Apply();
                spriteBatch.Draw(objectTarget, Vector2.Zero, Color.White);
            }
            else
            {
                spriteBatch.Draw(sceneTarget, Vector2.Zero, Color.White);
                spriteBatch.Draw(objectTarget, Vector2.Zero, Color.White);
            }
            spriteBatch.End();
            lightMask = null;                   

            //Draw UI
            if (activeManager.UI != null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                foreach (Entity e in activeManager.UI.UIElements)
                {
                    GraphicsComponent g = e.GetComponent<GraphicsComponent>();
                    foreach (SpriteGraphicsInfo sprite in g.Sprites.Values)
                    {
                        HelperFunctions.DrawString(spriteBatch, sprite.Font, sprite.FontColor, sprite.Text, sprite.Body);
                    }
                }
                spriteBatch.End();
            }
            

        }

        private void DrawPolygon(Polygon e)
        {                                               
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.BlendState = BlendState.Additive;
            GraphicsDevice.RasterizerState = rasterizerState;
                       
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(Vertex2D), e.VertexList.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), e.IndexList.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<Vertex2D>(e.VertexList);
            indexBuffer.SetData(e.IndexList);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, e.VertexList.Length, 0, e.VertexList.Length - 2);
            }
            
            
        }
      
        

        private void Reset(World world)
        {
            inputListener.RemoveWorld(world);
            InputManager.RemoveWorld(world);

            if (world.WorldType == Worlds.Debug)
                CreateDebug();
            else if (world.WorldType == Worlds.InGame)
            {
                client.Disconnect("Disconnecting");
                isConnected = false;
                isPlayingOnline = false;
                CreateGame();
            }
            else if (world.WorldType == Worlds.MainMenu)
                CreateMainMenu();
            else if (world.WorldType == Worlds.PauseMenu)
                CreatePauseMenu();
        }

        private void CreateMainMenu()
        {
            mainMenu = new World(Worlds.MainMenu);
            mainMenu.AddManager(new ClientECSManager());

            MenuInputSystem menuInput = new MenuInputSystem();
            mainMenu.Manager.AddEntity(EFactory.CreateButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 2f), 200, 50, Inputs.StartDebug, "Debug", buttonFont));
            mainMenu.Manager.AddEntity(EFactory.CreateButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f / 6f)), 200, 50, Inputs.Start, "Start", buttonFont));
            mainMenu.Manager.AddEntity(EFactory.CreateButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f / 8f)), 200, 50, Inputs.Exit, "Exit", buttonFont));           
            mainMenu.Manager.AddSystem(new AudioSystem(Content));
            mainMenu.Manager.AddSystem(menuInput);
            
            inputListener.AddInput(mainMenu, menuInput);
            InputManager.AddInput(mainMenu, menuInput);

            mainMenu.Manager.Initialize();
        }

        private void CreateConnectMenu()
        {
            connectMenu = new World(Worlds.ConnectMenu);
            connectMenu.AddManager(new ClientECSManager());

            MenuInputSystem menuInput = new MenuInputSystem();
            connectMenu.Manager.AddEntity(EFactory.CreateButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 2f), 200, 50, Inputs.Start, "Connect", buttonFont));
            connectMenu.Manager.AddEntity(EFactory.CreateButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f / 8f)), 200, 50, Inputs.Exit, "Back", buttonFont));          
            connectMenu.Manager.AddSystem(new AudioSystem(Content));
            connectMenu.Manager.AddSystem(menuInput);
            
            inputListener.AddInput(connectMenu, menuInput);
            InputManager.AddInput(connectMenu, menuInput);

            connectMenu.Manager.Initialize();
        }

        private void CreateDebug()
        {            
            debug = new World(Worlds.Debug);
            ClientECSManager gameManager = new ClientECSManager();
            UI gameUI = new UI(GraphicsDevice);
            gameManager.AddUI(gameUI);
            debug.AddManager(gameManager);
            
            AudioSystem audio = new AudioSystem(Content);
            GameInputSystem gameInput = new GameInputSystem();
            audio.Songs = gameSongs;
            debug.Manager.AddSystem(new AvatarInputSystem());
            debug.Manager.AddSystem(new GameCollisionSystem());
            debug.Manager.AddSystem(new MovementSystem());
            debug.Manager.AddSystem(new LevelSystem(levels));
            debug.Manager.AddSystem(new AttackSystem());
            debug.Manager.AddSystem(new FieldOfViewSystem());
            debug.Manager.AddSystem(new UIUpdateSystem(gameUI));
            debug.Manager.AddSystem(audio);
            debug.Manager.AddSystem(gameInput);

            inputListener.AddInput(debug, gameInput);
            InputManager.AddInput(debug, gameInput);

            debug.Manager.Initialize();
        }

        private void CreateGame()
        {
            inGame = new World(Worlds.InGame);
            ClientECSManager gameManager = new ClientECSManager();
            UI gameUI = new UI(GraphicsDevice);
            gameManager.AddUI(gameUI);
            inGame.AddManager(gameManager);

            ClientUpdateSystem networkInput = new ClientUpdateSystem(client);
            AudioSystem audio = new AudioSystem(Content);
            audio.Songs = gameSongs;
            inGame.Manager.AddSystem(networkInput);                       
            inGame.Manager.AddSystem(audio);

            inputListener.AddInput(inGame, networkInput);
            InputManager.AddInput(inGame, networkInput);

            inGame.Manager.Initialize();
        }

        private void CreatePauseMenu()
        {
            pauseMenu = new World(Worlds.PauseMenu, false, false);
            pauseMenu.AddManager(new ClientECSManager());

            MenuInputSystem menuInput = new MenuInputSystem();
            pauseMenu.Manager.AddEntity(EFactory.CreateButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 3.2f), 200, 50, Inputs.Resume, "Resume", buttonFont));
            pauseMenu.Manager.AddEntity(EFactory.CreateButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f / 8f)), 200, 50, Inputs.Exit, "Exit To Menu", buttonFont));
            pauseMenu.Manager.AddSystem(new AudioSystem(Content));
            pauseMenu.Manager.AddSystem(menuInput);

            inputListener.AddInput(pauseMenu, menuInput);
            InputManager.AddInput(pauseMenu, menuInput);

            pauseMenu.Manager.Initialize();
        }
        
        /// <summary>
        /// Listens for the events from menu and game elements, and uses the information to manage what screens are active.
        /// </summary>
        private class InputListener
        {
            private Dictionary<World, List<ESystem>> inputs;
            ClientGameLoop game;

            public InputListener(ClientGameLoop game)
            {
                this.game = game;
                inputs = new Dictionary<World, List<ESystem>>();
            }
           
            public void AddInput(World world, ESystem input)
            {
                if (!inputs.ContainsKey(world))
                    inputs.Add(world, new List<ESystem>());
                inputs[world].Add(input);                
            }

            public void RemoveWorld(World world)
            {
                if (inputs.ContainsKey(world))
                {
                    Unsubscribe(world);
                    inputs.Remove(world);
                }
            }

            public void Subscribe(World world)
            {
                foreach (ESystem system in inputs[world])
                    system.inputFired += new EventHandler(InputFired);
            }

            public void Unsubscribe(World world)
            {
                if(inputs.ContainsKey(world))
                    foreach (ESystem system in inputs[world])
                        system.inputFired -= new EventHandler(InputFired);
            }

            private void InputFired(object sender, EventArgs e)
            {
                if (e is InputArgs)
                {                   
                    InputArgs args = (InputArgs)e;
                    Console.WriteLine("Input received " + args.InputType);
                    
                    Console.WriteLine(game.worldStack.Peek().WorldType + "=> " + args.InputType);
                    switch (game.activeWorld.WorldType)
                    {
                        case Worlds.MainMenu:
                            if (args.InputType == Inputs.StartDebug)
                                game.worldStack.Push(game.debug);
                            if (args.InputType == Inputs.Start)
                                game.worldStack.Push(game.connectMenu);
                            if (args.InputType == Inputs.Exit)
                                game.Exit();
                            break;
                        case Worlds.ConnectMenu:
                            if (args.InputType == Inputs.Start && !game.isConnected)
                            {
                                game.client.DiscoverKnownPeer("70.79.66.246", 14242);
                                Console.WriteLine("Ping");
                            }
                            if (args.InputType == Inputs.Exit)
                            {
                                if(game.isConnected)
                                {
                                    game.client.Disconnect("Quit while waiting");
                                    game.isConnected = false;
                                    game.isPlayingOnline = false;
                                }
                                game.worldStack.Pop();
                            }
                            break;
                        case Worlds.Debug:
                        case Worlds.InGame:
                            if (args.InputType == Inputs.Pause)
                                game.worldStack.Push(game.pauseMenu);
                            if (args.InputType == Inputs.Exit)
                            {
                                game.Reset(game.activeWorld);
                                game.worldStack.Pop();
                            }
                            break;
                        case Worlds.PauseMenu:
                            if (args.InputType == Inputs.Resume)
                                game.worldStack.Pop();
                            if (args.InputType == Inputs.Exit)
                            {

                                game.worldStack.Pop();
                                game.Reset(game.worldStack.Peek());
                                game.worldStack.Pop();
                            }
                            break;
                    }
                    Unsubscribe(game.activeWorld);
                    game.activeWorld = game.worldStack.Peek();
                    Subscribe(game.activeWorld);
                }
            }
        }     
    }
}
