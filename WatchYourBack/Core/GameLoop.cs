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
        World pauseMenu;

        InputListener inputListener;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Dictionary<LevelName, LevelTemplate> levels; 

        Rectangle avatarBody;
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
            inGame = new World(Worlds.IN_GAME, Content);
            mainMenu = new World(Worlds.MAIN_MENU, Content);
            pauseMenu = new World(Worlds.PAUSE_MENU, Content);
            inputListener = new InputListener(this);

            Texture2D testLevelLayout = Content.Load<Texture2D>("TestLevel");
            Texture2D levelOne = Content.Load<Texture2D>("LevelOne");
            testFont = Content.Load<SpriteFont>("TestFont");
            levels = new Dictionary<LevelName, LevelTemplate>(); 
            levels.Add(LevelName.TEST_LEVEL, new LevelTemplate(testLevelLayout));
            levels.Add(LevelName.FIRST_LEVEL, new LevelTemplate(levelOne));

            worldStack.Push(mainMenu);
            activeWorld = worldStack.Peek();
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
            avatarBody = new Rectangle(100, 100, GraphicsDevice.Viewport.Width / 40, GraphicsDevice.Viewport.Width / 40);
            
        
            createMainMenu();
            createGame();
            createPauseMenu();

         

            
            
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

            activeWorld.Manager.update();
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
            inputListener.addMenu(mainMenu);
            mainMenu.Manager.addEntity(EFactory.createButton(50, 50, 50, 50, Inputs.START, avatarTexture, "Start Game", testFont));
            mainMenu.Manager.addEntity(EFactory.createButton(50, 200, 50, 50, Inputs.EXIT, avatarTexture, "Exit", testFont));
        }

        private void createGame()
        {
            inputListener.addGame(inGame);
            GameInputSystem input = new GameInputSystem();
            
            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
            inGame.Manager.addSystem(new LevelSystem(levels));
            inGame.Manager.addSystem(new AttackSystem());

            
            inGame.Manager.addEntity(EFactory.createAvatar(avatarBody, avatarTexture));
           
        }

        private void createPauseMenu()
        {
            inputListener.addMenu(pauseMenu);
            pauseMenu.Manager.addEntity(EFactory.createButton(50, 50, 50, 50, Inputs.START, avatarTexture, "Resume", testFont));
            pauseMenu.Manager.addEntity(EFactory.createButton(50, 200, 50, 50, Inputs.EXIT, avatarTexture, "Exit to menu", testFont));
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

            public void addMenu(World menu)
            {
                MenuInputSystem toAdd = new MenuInputSystem();
                menu.Manager.addInput(toAdd);
                menu.Manager.addSystem(toAdd);
                inputs.Add(menu, toAdd);
                toAdd.inputFired += new EventHandler(inputFired);
            }

            public void addGame(World game)
            {
                GameInputSystem toAdd = new GameInputSystem();
                game.Manager.addInput(toAdd);
                game.Manager.addSystem(toAdd);
                inputs.Add(game, toAdd);
                toAdd.inputFired += new EventHandler(inputFired);
            }

            private void inputFired(object sender, EventArgs e)
            {
                InputArgs args = (InputArgs)e;
                Console.WriteLine(game.worldStack.Peek().MenuType + "=> " + args.InputType);

                if(game.activeWorld.MenuType == Worlds.MAIN_MENU)
                {
                    if (args.InputType == Inputs.START)
                        game.worldStack.Push(game.inGame);
                    if (args.InputType == Inputs.EXIT)
                        game.Exit();
                }

                if (game.activeWorld.MenuType == Worlds.IN_GAME)
                {
                    if (args.InputType == Inputs.PAUSE)
                    {
                        game.worldStack.Push(game.pauseMenu);
                    }
                }

                if (game.activeWorld.MenuType == Worlds.PAUSE_MENU)
                {
                    if (args.InputType == Inputs.START)
                        game.worldStack.Pop();
                    if (args.InputType == Inputs.EXIT)
                    {
                        game.worldStack.Pop();
                        game.worldStack.Pop();
                    }
                }
                
                
                //Do stuff
                
            }
        }
    }
}
