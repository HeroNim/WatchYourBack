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
        Stack<World> worldStack;
        Stack<World> updateStack;
        Stack<World> drawStack;
        World activeWorld;
        ClientECSManager activeManager;
                
        World mainMenu;
        World connectMenu;
        World inGame;
        World inGameMulti;
        World pauseMenu;

        InputListener inputListener;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Dictionary<LevelName, LevelTemplate> levels;

        SongCollection gameSongs;

        //----------------------------------------------------------------------------------------------------------
        NetClient client;
        private bool isConnected;
        private bool isPlayingOnline;
        //----------------------------------------------------------------------------------------------------------
        private Stopwatch debug;

        public ClientGameLoop()
            : base()
        {
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
            debug = new Stopwatch();
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

            Texture2D levelOne = Content.Load<Texture2D>("LevelOne");
            levels = new Dictionary<LevelName, LevelTemplate>();
            levels.Add(LevelName.FIRST_LEVEL, new LevelTemplate(levelOne, LevelName.FIRST_LEVEL));

            gameSongs = new SongCollection();

            isConnected = false;
            isPlayingOnline = false;
            EFactory.content = Content;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            createMainMenu();
            createConnectMenu();
            createGame();
            createGameMulti();
            createPauseMenu();
          
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
            InputManager.setActiveWorld(activeWorld);
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
                activeManager.update(gameTime.ElapsedGameTime);                       
            }
            
            base.Update(gameTime);
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
            GraphicsDevice.Clear(Color.DarkGray);
            while(drawStack.Count != 0)
            {
                activeManager = (ClientECSManager)drawStack.Pop().Manager;
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                activeManager.draw(spriteBatch);
                spriteBatch.End();
                activeManager.DrawTime = gameTime.ElapsedGameTime.TotalSeconds;
            }
            base.Draw(gameTime);            
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
                        else if(confirmation == (int)ServerCommands.Start)
                        {    
                            worldStack.Push(inGameMulti);
                            activeWorld = inGameMulti;
                            inputListener.Unsubscribe(connectMenu);
                            inputListener.Subscribe(inGameMulti);
                            isPlayingOnline = true;
                            Console.WriteLine("Playing");         
                        }
                        break;
                }
            }            
        }

        private void reset(World world)
        {
            inputListener.removeWorld(world);
            InputManager.removeWorld(world);

            if (world.WorldType == Worlds.InGame)
                createGame();
            else if (world.WorldType == Worlds.InGameMulti)
            {
                client.Disconnect("Disconnecting");
                isConnected = false;
                isPlayingOnline = false;
                createGameMulti();
            }
            else if (world.WorldType == Worlds.MainMenu)
                createMainMenu();
            else if (world.WorldType == Worlds.PauseMenu)
                createPauseMenu();
        }

        private void createMainMenu()
        {
            mainMenu = new World(Worlds.MainMenu);
            mainMenu.addManager(new ClientECSManager());

            MenuInputSystem menuInput = new MenuInputSystem();
            mainMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 2f), 200, 50, Inputs.START_SINGLE, "Start"));
            mainMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f/6f)), 200, 50, Inputs.START_MUTLI, "Start"));
            mainMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f/8f)), 200, 50, Inputs.EXIT, "Exit"));           
            mainMenu.Manager.addSystem(new AudioSystem(Content));
            mainMenu.Manager.addSystem(menuInput);
            
            inputListener.addInput(mainMenu, menuInput);
            InputManager.addInput(mainMenu, menuInput);

            mainMenu.Manager.Initialize();
        }

        private void createConnectMenu()
        {
            connectMenu = new World(Worlds.ConnectMenu);
            connectMenu.addManager(new ClientECSManager());

            MenuInputSystem menuInput = new MenuInputSystem();
            connectMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 2f), 200, 50, Inputs.START_MUTLI, "Connect"));
            connectMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f / 8f)), 200, 50, Inputs.EXIT, "Back"));          
            connectMenu.Manager.addSystem(new AudioSystem(Content));
            connectMenu.Manager.addSystem(menuInput);
            
            inputListener.addInput(connectMenu, menuInput);
            InputManager.addInput(connectMenu, menuInput);

            connectMenu.Manager.Initialize();
        }

        private void createGame()
        {            
            inGame = new World(Worlds.InGame);
            ClientECSManager gameManager = new ClientECSManager();
            UIInfo gameUI = new UIInfo(GraphicsDevice);
            gameManager.addUI(gameUI);
            inGame.addManager(gameManager);
            
            AudioSystem audio = new AudioSystem(Content);
            GameInputSystem gameInput = new GameInputSystem();
            audio.Songs = gameSongs;
            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
            inGame.Manager.addSystem(new LevelSystem(levels));
            inGame.Manager.addSystem(new AttackSystem());
            inGame.Manager.addSystem(new UIUpdateSystem(gameUI));
            inGame.Manager.addSystem(audio);
            inGame.Manager.addSystem(gameInput);

            inputListener.addInput(inGame, gameInput);
            InputManager.addInput(inGame, gameInput);

            inGame.Manager.Initialize();
        }

        private void createGameMulti()
        {
            inGameMulti = new World(Worlds.InGameMulti);
            ClientECSManager gameManager = new ClientECSManager();
            UIInfo gameUI = new UIInfo(GraphicsDevice);
            gameManager.addUI(gameUI);
            inGameMulti.addManager(gameManager);

            ClientUpdateSystem networkInput = new ClientUpdateSystem(client);
            AudioSystem audio = new AudioSystem(Content);
            audio.Songs = gameSongs;
            inGameMulti.Manager.addSystem(networkInput);                       
            inGameMulti.Manager.addSystem(audio);

            inputListener.addInput(inGameMulti, networkInput);
            InputManager.addInput(inGameMulti, networkInput);

            inGameMulti.Manager.Initialize();
        }

        private void createPauseMenu()
        {
            pauseMenu = new World(Worlds.PauseMenu, false, false);
            pauseMenu.addManager(new ClientECSManager());

            MenuInputSystem menuInput = new MenuInputSystem();
            pauseMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 3.2f), 200, 50, Inputs.RESUME, "Resume"));
            pauseMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f / 8f)), 200, 50, Inputs.EXIT, "ExitToMenu"));
            pauseMenu.Manager.addSystem(new AudioSystem(Content));
            pauseMenu.Manager.addSystem(menuInput);

            inputListener.addInput(pauseMenu, menuInput);
            InputManager.addInput(pauseMenu, menuInput);

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
           
            public void addInput(World world, ESystem input)
            {
                if (!inputs.ContainsKey(world))
                    inputs.Add(world, new List<ESystem>());
                inputs[world].Add(input);                
            }

            public void removeWorld(World world)
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
                    system.inputFired += new EventHandler(inputFired);
            }

            public void Unsubscribe(World world)
            {
                if(inputs.ContainsKey(world))
                    foreach (ESystem system in inputs[world])
                        system.inputFired -= new EventHandler(inputFired);
            }

            private void inputFired(object sender, EventArgs e)
            {
                if (e is InputArgs)
                {                   
                    InputArgs args = (InputArgs)e;
                    Console.WriteLine("Input received " + args.InputType);
                    
                    Console.WriteLine(game.worldStack.Peek().WorldType + "=> " + args.InputType);
                    switch (game.activeWorld.WorldType)
                    {
                        case Worlds.MainMenu:
                            if (args.InputType == Inputs.START_SINGLE)
                                game.worldStack.Push(game.inGame);
                            if (args.InputType == Inputs.START_MUTLI)
                                game.worldStack.Push(game.connectMenu);
                            if (args.InputType == Inputs.EXIT)
                                game.Exit();
                            break;
                        case Worlds.ConnectMenu:
                            if (args.InputType == Inputs.START_MUTLI && !game.isConnected)
                            {
                                game.client.DiscoverKnownPeer("24.87.148.96", 14242);
                                Console.WriteLine("Ping");
                            }
                            if (args.InputType == Inputs.EXIT)
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
                        case Worlds.InGame:
                        case Worlds.InGameMulti:
                            if (args.InputType == Inputs.PAUSE)
                                game.worldStack.Push(game.pauseMenu);
                            if (args.InputType == Inputs.EXIT)
                            {
                                game.reset(game.activeWorld);
                                game.worldStack.Pop();
                            }
                            break;
                        case Worlds.PauseMenu:
                            if (args.InputType == Inputs.RESUME)
                                game.worldStack.Pop();
                            if (args.InputType == Inputs.EXIT)
                            {

                                game.worldStack.Pop();
                                game.reset(game.worldStack.Peek());
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
