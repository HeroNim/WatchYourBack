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
        MenuListener menuListener;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Dictionary<LevelName, LevelTemplate> levels; 
        Rectangle body;
        Texture2D bodyTexture;
        Texture2D wallTexture;
        WallTemplate wallTemplate;

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
            mainMenu = new World(Worlds.MAIN_MENU);
            inGame = new World(Worlds.IN_GAME);
            pauseMenu = new World(Worlds.PAUSE_MENU);
            menuListener = new MenuListener(this);


            levels = new Dictionary<LevelName, LevelTemplate>();

            Texture2D testLevelLayout = Content.Load<Texture2D>("TestLevel");
            LevelTemplate firstLevel = new LevelTemplate(testLevelLayout);
            levels.Add(LevelName.firstLevel, firstLevel);

            menuListener.addMenu(mainMenu);
            //mainMenu.Manager.addSystem(new MenuInputSystem());

            inGame.Manager.addSystem(new GameInputSystem());
            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
            inGame.Manager.addSystem(new LevelSystem(levels));

            pauseMenu.Manager.addSystem(new GameInputSystem());
            menuListener.addMenu(pauseMenu);

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
            
            bodyTexture = new Texture2D(GraphicsDevice, 1, 1);
            bodyTexture.SetData(new[] { Color.White });
            wallTexture = new Texture2D(GraphicsDevice, 1, 1);
            wallTexture.SetData(new[] { Color.Black });
            body = new Rectangle(100, 100, GraphicsDevice.Viewport.Width / 40, GraphicsDevice.Viewport.Width / 40);
            
            wallTemplate = new WallTemplate(wallTexture, GraphicsDevice.Viewport.Width / 32, GraphicsDevice.Viewport.Height / 18);

            mainMenu.Manager.addEntity(mainMenu.Manager.Factory.createButton(50, 50, 50, 50, Buttons.Start, bodyTexture, "Start"));

            inGame.Manager.Factory.setWallTemplate(wallTemplate);
            inGame.Manager.addEntity(inGame.Manager.Factory.createAvatar(body, bodyTexture, Color.White));

            
            
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
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

        private class MenuListener
        {
            private Dictionary<World, MenuInputSystem> menuInputs;
            GameLoop game;

            public MenuListener(GameLoop game)
            {
                this.game = game;
                menuInputs = new Dictionary<World, MenuInputSystem>();
            }

            public void addMenu(World menu)
            {
                MenuInputSystem toAdd = new MenuInputSystem(menu);
                menu.Manager.addSystem(toAdd);
                menuInputs.Add(menu, toAdd);
                toAdd.buttonClicked += new EventHandler(buttonClicked);
            }

            private void buttonClicked(object sender, EventArgs e)
            {
                ButtonArgs args = (ButtonArgs)e;
                World input = (World)sender;
                Console.WriteLine(game.worldStack.Peek().MenuType);
                Console.WriteLine(menuInputs[input].ToString());
                Console.WriteLine(args.ButtonType);

                if(game.activeWorld.MenuType == Worlds.MAIN_MENU)
                {
                    if (args.ButtonType == Buttons.Start)
                        game.worldStack.Push(game.inGame);
                }
                
                //Do stuff
                
            }
        }
    }
}
