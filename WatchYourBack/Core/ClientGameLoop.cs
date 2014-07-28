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

        List<LevelTemplate> levels;

        //----------------------------------------------------------------------------------------------------------
        NetClient client;
        bool isPinging;
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
            // TODO: Add your initialization logic here
            worldStack = new Stack<World>();
            inputListener = new InputListener(this);

            Texture2D testLevelLayout = Content.Load<Texture2D>("TestLevel");
            Texture2D levelOne = Content.Load<Texture2D>("LevelOne");
            levels = new List<LevelTemplate>();
            levels.Add(new LevelTemplate(levelOne, LevelName.FIRST_LEVEL));
            


            isPinging = false;
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
            // Create a new SpriteBatch, which can be used to draw textures.

            spriteBatch = new SpriteBatch(GraphicsDevice);

            createMainMenu();
            createConnectMenu();
            createGame();
            createGameMulti();
            createPauseMenu();

            worldStack.Push(mainMenu);
            activeWorld = worldStack.Peek();
            activeManager = (ClientECSManager)activeWorld.Manager;

            // TODO: use this.Content to load your game content here
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


            // TODO: Add your update logic here
            if(activeWorld == connectMenu)
                if (!isPlayingOnline)
                    Connect();
            activeManager.update(gameTime.ElapsedGameTime);
            activeWorld = worldStack.Peek();
            activeManager = (ClientECSManager)activeWorld.Manager;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            activeManager.draw(spriteBatch);
            spriteBatch.End();

            activeManager.DrawTime = gameTime.ElapsedGameTime.TotalSeconds;
            
            base.Draw(gameTime);
            
        }

       

        private void Connect()
        {
            //----------------------------------------------------------------------------------------------------------
            
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        // just connect to first server discovered
                        if (isPinging)
                        {
                            client.Connect(msg.SenderEndpoint);
                            isPinging = false;
                            isConnected = true;                           
                            Console.WriteLine("Connected");
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        // server sent initialization command
                        int confirmation = msg.ReadInt32();
                        if (confirmation == (int)SERVER_COMMANDS.SEND_LEVELS)
                        {
                            NetOutgoingMessage om = client.CreateMessage();
                            om.Write(SerializationHelper.Serialize(levels));
                            client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
                        }
                        else if(confirmation == (int)SERVER_COMMANDS.START)
                        {    
                            worldStack.Push(inGameMulti);
                            activeWorld = worldStack.Peek();
                            isPlayingOnline = true;
                            
                        }
                        break;
                }
            }
            
        }

        private void createMainMenu()
        {
            mainMenu = new World(Worlds.MAIN_MENU);
            mainMenu.addManager(new ClientECSManager());
            inputListener.addWorld(mainMenu, true);
            mainMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 2f), 200, 50, Inputs.START_SINGLE, "Start"));
            mainMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f/6f)), 200, 50, Inputs.START_MUTLI, "Start"));
            mainMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f/8f)), 200, 50, Inputs.EXIT, "Exit"));
        }

        private void createConnectMenu()
        {
            connectMenu = new World(Worlds.CONNECT_MENU);
            connectMenu.addManager(new ClientECSManager());
            inputListener.addWorld(connectMenu, true);
            connectMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 2f), 200, 50, Inputs.START_MUTLI, "Connect"));
            connectMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f / 8f)), 200, 50, Inputs.EXIT, "Back"));
        }

        private void createGame()
        {
            ClientECSManager gameManager = new ClientECSManager();
            UIInfo gameUI = new UIInfo(GraphicsDevice);
            gameManager.addUI(gameUI);
            inGame = new World(Worlds.IN_GAME);
            inGame.addManager(gameManager);
            
            inputListener.addWorld(inGame, false);

            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
            inGame.Manager.addSystem(new LevelSystem(levels));
            inGame.Manager.addSystem(new AttackSystem());
            inGame.Manager.addSystem(new UIUpdateSystem(gameUI));

        }

        private void createGameMulti()
        {
            ClientECSManager gameManager = new ClientECSManager();
            UIInfo gameUI = new UIInfo(GraphicsDevice);
            gameManager.addUI(gameUI);
            inGameMulti = new World(Worlds.IN_GAME_MULTI);
            inGameMulti.addManager(gameManager);

            inputListener.addWorld(inGameMulti, false);

            ClientUpdateSystem networkInput = new ClientUpdateSystem(client);
            inGameMulti.Manager.addSystem(networkInput);
        }

        private void createPauseMenu()
        {
            pauseMenu = new World(Worlds.PAUSE_MENU);
            pauseMenu.addManager(new ClientECSManager());
            inputListener.addWorld(pauseMenu, true);
            pauseMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / 3.2f), 200, 50, Inputs.RESUME, "Resume"));
            pauseMenu.Manager.addEntity(EFactory.createButton(GraphicsDevice.Viewport.Width / 2, (int)((float)GraphicsDevice.Viewport.Height / (10f / 8f)), 200, 50, Inputs.EXIT, "ExitToMenu"));
        }

        private void reset(World world)
        {
            inputListener.removeWorld(world);

            if (world.MenuType == Worlds.IN_GAME)
                createGame();
            else if (world.MenuType == Worlds.IN_GAME_MULTI)
            {
                client.Disconnect("Disconnecting");
                isConnected = false;
                isPlayingOnline = false;
                createGameMulti();
            }
            else if (world.MenuType == Worlds.MAIN_MENU)
                createMainMenu();
            else if (world.MenuType == Worlds.PAUSE_MENU)
                createPauseMenu();
        }

        /*
         * Listens for the events from menu and game elements, and uses the information to manage what screens are active.
         */
        private class InputListener
        {
            private Dictionary<World, InputSystem> inputs;
            ClientGameLoop game;

            public InputListener(ClientGameLoop game)
            {
                this.game = game;
                inputs = new Dictionary<World, InputSystem>();
            }



            public void addWorld(World world, bool isMenu)
            {
                InputSystem toAdd;
                if (inputs.ContainsKey(world))
                    return;
                if (isMenu)
                    toAdd = new MenuInputSystem();
                else
                    toAdd = new GameInputSystem();
                world.Manager.addInput(toAdd);
                world.Manager.addSystem((ESystem)toAdd);
                inputs.Add(world, toAdd);
                toAdd.inputFired += new EventHandler(inputFired);
            }

            public void removeWorld(World world)
            {
                if (inputs.ContainsKey(world))
                    inputs.Remove(world);
            }

            private void inputFired(object sender, EventArgs e)
            {
                InputArgs args = (InputArgs)e;
                Console.WriteLine(game.worldStack.Peek().MenuType + "=> " + args.InputType);

                if (game.activeWorld.MenuType == Worlds.MAIN_MENU)
                {

                    if (args.InputType == Inputs.START_SINGLE)
                        game.worldStack.Push(game.inGame);
                    if (args.InputType == Inputs.START_MUTLI)
                        game.worldStack.Push(game.connectMenu);
                    if (args.InputType == Inputs.EXIT)
                        game.Exit();
                }

                else if (game.activeWorld.MenuType == Worlds.CONNECT_MENU)
                {


                    if (args.InputType == Inputs.START_MUTLI && !game.isConnected && !game.isPinging)
                    {
                        game.isPinging = true;
                        game.client.DiscoverLocalPeers(14242);
                        Console.WriteLine("Starting pings");
                    }
                    if (args.InputType == Inputs.EXIT)
                        game.worldStack.Pop();
                }

                else if (game.activeWorld.MenuType == Worlds.IN_GAME || game.activeWorld.MenuType == Worlds.IN_GAME_MULTI)
                {
                    if (args.InputType == Inputs.PAUSE)
                    {
                        game.worldStack.Push(game.pauseMenu);
                    }
                }

                else if (game.activeWorld.MenuType == Worlds.PAUSE_MENU)
                {
                    if (args.InputType == Inputs.RESUME)
                        game.worldStack.Pop();
                    if (args.InputType == Inputs.EXIT)
                    {

                        game.worldStack.Pop();
                        World currentGame = game.worldStack.Peek();
                        game.worldStack.Pop();
                        game.reset(currentGame);
                    }
                }
                game.activeWorld = game.worldStack.Peek();
            }
        }


    }
}
