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
#endregion

namespace WatchYourBack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameLoop : Game
    {

        Stack<World> worldStack;
        World activeWorld;

        World mainMenu;
        World inGame;
        World inGameMulti;
        World pauseMenu;

        InputListener inputListener;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Dictionary<LevelName, LevelTemplate> levels;

        Texture2D avatarTexture;

        SpriteFont testFont;

        public GameLoop()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = true;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
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
            testFont = Content.Load<SpriteFont>("TestFont");
            levels = new Dictionary<LevelName, LevelTemplate>();
            levels.Add(LevelName.TEST_LEVEL, new LevelTemplate(testLevelLayout));
            levels.Add(LevelName.FIRST_LEVEL, new LevelTemplate(levelOne));


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


            avatarTexture = new Texture2D(GraphicsDevice, 1, 1);
            avatarTexture.SetData(new[] { Color.White });



            createMainMenu();
            createGame();
            createGameMulti();
            createPauseMenu();

            worldStack.Push(mainMenu);
            activeWorld = worldStack.Peek();

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

            activeWorld.Manager.update(gameTime);
            activeWorld = worldStack.Peek();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            activeWorld.Manager.draw(spriteBatch);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void createMainMenu()
        {
            mainMenu = new World(Worlds.MAIN_MENU, Content);
            inputListener.addWorld(mainMenu, true);
            mainMenu.Manager.addEntity(EFactory.createButton(50, 50, 50, 50, Inputs.START_SINGLE, avatarTexture, "Singleplayer", testFont));
            mainMenu.Manager.addEntity(EFactory.createButton(50, 200, 50, 50, Inputs.START_MUTLI, avatarTexture, "Multiplayer", testFont));
            mainMenu.Manager.addEntity(EFactory.createButton(50, 350, 50, 50, Inputs.EXIT, avatarTexture, "Exit", testFont));
        }

        private void createGame()
        {

            inGame = new World(Worlds.IN_GAME, Content);
            inputListener.addWorld(inGame, false);
            GameInputSystem input = new GameInputSystem();

            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
            inGame.Manager.addSystem(new LevelSystem(levels));
            inGame.Manager.addSystem(new AttackSystem());

        }

        private void createGameMulti()
        {
            inGameMulti = new World(Worlds.IN_GAME, Content);
            inputListener.addWorld(inGameMulti, false);
            GameInputSystem input = new GameInputSystem();

            NetworkInputSystem networkInput = new NetworkInputSystem();
            inGameMulti.Manager.addSystem(new LevelSystem(levels));
            inGameMulti.Manager.addSystem(networkInput);
            inGameMulti.Manager.addSystem(new NetworkUpdaterSystem(networkInput));
        }

        private void createPauseMenu()
        {
            pauseMenu = new World(Worlds.PAUSE_MENU, Content);
            inputListener.addWorld(pauseMenu, true);
            pauseMenu.Manager.addEntity(EFactory.createButton(50, 50, 50, 50, Inputs.RESUME, avatarTexture, "Resume", testFont));
            pauseMenu.Manager.addEntity(EFactory.createButton(50, 200, 50, 50, Inputs.EXIT, avatarTexture, "Exit to menu", testFont));
        }

        private void reset(World world)
        {
            if (world.MenuType == Worlds.IN_GAME)
                createGame();
            else if (world.MenuType == Worlds.IN_GAME_MULTI)
                createGameMulti();
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
            GameLoop game;

            public InputListener(GameLoop game)
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

            private void inputFired(object sender, EventArgs e)
            {
                InputArgs args = (InputArgs)e;
                Console.WriteLine(game.worldStack.Peek().MenuType + "=> " + args.InputType);

                if (game.activeWorld.MenuType == Worlds.MAIN_MENU)
                {

                    if (args.InputType == Inputs.START_SINGLE)
                        game.worldStack.Push(game.inGame);
                    if (args.InputType == Inputs.START_MUTLI)
                        game.worldStack.Push(game.inGameMulti);
                    if (args.InputType == Inputs.EXIT)
                        game.Exit();
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
